using RemoteDiskImanger;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RemoteDiskImagerUI;

public partial class Form1 : Form {
    private static readonly Regex REX_Server = new(@"^(?<user>[^@]+)@(?<host>\[[0-9a-fA-F:]+\]|[^:]+)(:(?<path>.+))?$", RegexOptions.Compiled);
    private static readonly Regex REX_BlockSize = new(@"^\d+[KMG]?$", RegexOptions.Compiled);

    private CancellationTokenSource? _cancellationTokenSource = null;

    public Form1(string[] args) {
        InitializeComponent();

        optSu.SelectedIndex = 0;

        #region Parse arguments
        bool sourceDone = false;
        try {
            for (int i = 0; i < args.Length; i++) {
                string arg = args[i];
                switch (arg) {
                    case "--port":
                    case "-p":
                        i++;
                        if (i >= args.Length) throw new Exception("Port is number missing!");
                        if (!ushort.TryParse(args[i], out ushort p)) throw new Exception("Invalid port number!");
                        numPort.Value = p;
                        break;
                    case "--password":
                    case "-P":
                        i++;
                        if (i >= args.Length) throw new Exception("Password missing");
                        txtPassword.Text = args[i];
                        break;
                    case "--block-size":
                    case "-b":
                        i++;
                        if (i >= args.Length) throw new Exception("Block size missing!");
                        if (!REX_BlockSize.IsMatch(args[i])) throw new Exception("Invalid block size!");
                        txtBlockSize.Text = args[i];
                        break;
                    case "--count":
                    case "-c":
                        i++;
                        if (i >= args.Length) throw new Exception("Count missing!");
                        if (!REX_BlockSize.IsMatch(args[i])) throw new Exception("Invalid block size!");
                        txtCount.Text = args[i];
                        break;
                    case "--certificate":
                    case "--cert":
                    case "-I":
                        i++;
                        if (i >= args.Length) throw new Exception("Certificate file missing!");
                        FileInfo certFile = new FileInfo(args[i]);
                        txtCertFile.Text = certFile.FullName;
                        break;
                    case "--su":
                    case "-s":
                        optSu.SelectedIndex = 2;
                        break;
                    case "--sudo":
                    case "-d":
                        optSu.SelectedIndex = 1;
                        break;
                    case "--su-password":
                    case "-S":
                        i++;
                        if (i >= args.Length) throw new Exception("su/sudo password missing!");
                        txtSuPassword.Text = args[i];
                        break;
                    default:
                        if (!sourceDone) {
                            Match match = REX_Server.Match(arg);
                            if (!match.Success) throw new Exception("Invalid source format!");
                            txtUsername.Text = match.Groups["user"].Value;
                            txtServer.Text = match.Groups["host"].Value;
                            if (match.Groups["path"].Success) txtDevice.Text = match.Groups["path"].Value;
                            sourceDone = true;
                        } else {
                            FileInfo _targetFile = new FileInfo(arg);
                            txtTargetFile.Text = _targetFile.FullName;
                        }
                        break;
                }
            }
        } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Argument exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }
        #endregion

        if (IsWindowsDarkMode())
            ApplyThemeColors(this);
    }

    private static bool IsWindowsDarkMode() {
        try {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null) {
                object? value = key.GetValue("AppsUseLightTheme");
                if (value is int intValue)
                    return intValue == 0;
            }
        } catch { }
        return false;
    }

    private static void ApplyThemeColors(Control control) {
        control.BackColor = Color.FromArgb(32, 32, 32);
        control.ForeColor = Color.White;
        foreach (Control child in control.Controls) {
            ApplyThemeColors(child);
        }
    }

    protected override void OnHandleCreated(EventArgs e) {
        base.OnHandleCreated(e);
        TryEnableDarkMode(this.Handle);
    }

    private void TryEnableDarkMode(IntPtr handle) {
        if (Environment.OSVersion.Version.Major >= 10) {
            int attribute = 20; // DWMWA_USE_IMMERSIVE_DARK_MODE
            int useDark = 1;
            DwmSetWindowAttribute(handle, attribute, ref useDark, sizeof(int));
        }
    }

    [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private void btnSelectCertFile_Click(object sender, EventArgs e) {
        OpenFileDialog ofd = new();
        ofd.Title = "Select Certificate File";
        ofd.Filter = "All Files (*.*)|*.*";
        if (ofd.ShowDialog() == DialogResult.OK) {
            txtCertFile.Text = ofd.FileName;
        }
    }

    private void btnSelectTargetFile_Click(object sender, EventArgs e) {
        SaveFileDialog ofd = new();
        ofd.Title = "Select Target File";
        ofd.Filter = "All Files (*.*)|*.*";
        if (ofd.ShowDialog() == DialogResult.OK) {
            txtTargetFile.Text = ofd.FileName;
        }
    }

    private void btnStart_Click(object sender, EventArgs e) {
        if (btnStart.Text == "Cancel") {
            _cancellationTokenSource?.Cancel();
            lblStatus.Text = "Cancelling...";
            return;
        }

        string username = txtUsername.Text.Trim();
        if (string.IsNullOrEmpty(username)) {
            MessageBox.Show("Username is required!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        string server = txtServer.Text.Trim();
        if (string.IsNullOrEmpty(server)) {
            MessageBox.Show("Server is required!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        ushort port = (ushort)numPort.Value;
        string password = txtPassword.Text;
        string certFile = txtCertFile.Text.Trim();
        SuCommandType suType = optSu.SelectedIndex switch {
            1 => SuCommandType.Sudo,
            2 => SuCommandType.Su,
            _ => SuCommandType.None
        };
        string device = txtDevice.Text.Trim();
        string suPassword = txtSuPassword.Text;
        string blockSize = txtBlockSize.Text.Trim();
        if (string.IsNullOrEmpty(blockSize) || !REX_BlockSize.IsMatch(blockSize)) {
            MessageBox.Show("Block size is required!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        string count = txtCount.Text.Trim();
        if (!string.IsNullOrEmpty(count) && !REX_BlockSize.IsMatch(count)) {
            MessageBox.Show("Invalid count!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        string targetFile = txtTargetFile.Text.Trim();
        if (string.IsNullOrEmpty(targetFile)) {
            MessageBox.Show("Target file is required!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        btnStart.Text = "Cancel";
        proProgress.Value = 0;
        proProgress.Visible = true;
        lblStatus.Text = "Starting...";

        Task.Run(() => ExecuteDump(
            username,
            server,
            port,
            password,
            string.IsNullOrEmpty(certFile) ? null : certFile,
            suType,
            string.IsNullOrEmpty(suPassword) ? null : suPassword,
            device,
            new FileInfo(targetFile),
            blockSize,
            string.IsNullOrEmpty(count) ? null : count,
            _cancellationTokenSource.Token
        ), _cancellationTokenSource.Token);
    }

    private async Task ExecuteDump(string username, string server, ushort port, string password, string? certFile, SuCommandType suType, string? suPassword, string device, FileInfo targetFile, string blockSize, string? count, CancellationToken cancelToken) {
        string title = "Init";
        try {
            title = "Connection error";
            this.Invoke(() => { lblStatus.Text = $"Connecting..."; });
            SshClient client;
            if (!string.IsNullOrEmpty(certFile)) {
                PrivateKeyFile cert;
                try {
                    cert = new(certFile, password);
                } catch (SshPassPhraseNullOrEmptyException ex) {
                    throw new Exception("Wrong passphrase for certificate!");
                }

                client = new SshClient(new ConnectionInfo(server, port, username, new PrivateKeyAuthenticationMethod(username, cert)));
            } else {
                client = new SshClient(server, port, username, password);
            }

            await client.ConnectAsync(cancelToken);

            title = "Device info error";
            this.Invoke(() => { lblStatus.Text = $"Retrieving block device information..."; });
            List<BlockDeviceInfo> deviceInfos = await client.GetBlockDeviceInfosAsync(suType, suPassword, cancelToken);
            List<BlockDeviceInfo> flatDeviceInfos = deviceInfos.SelectMany(d => d.AllDevices()).ToList();

            title = "Select device";
            this.Invoke(() => { lblStatus.Text = $"Block device selection..."; });
            if (string.IsNullOrEmpty(device)) {
                this.Invoke(() => {
                    DeviceSelectForm dsf = new(flatDeviceInfos);
                    if (dsf.ShowDialog() == DialogResult.OK) {
                        if (dsf.SelectedDevice is not null) {
                            device = dsf.SelectedDevice.Path;
                        }
                    }
                });
            }
            if (string.IsNullOrEmpty(device)) {
                this.Invoke(() => {
                    lblStatus.Text = "Cancelled.";
                });
                return;
            }

            BlockDeviceInfo? selectedDevice = flatDeviceInfos.FirstOrDefault(d => d.Path == device);


            title = "Create target file error";
            this.Invoke(() => { lblStatus.Text = $"Opening target file..."; });
            using (FileStream targetStream = targetFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
                title = "Data copy error";
                this.Invoke(() => { lblStatus.Text = $"Starting copying data..."; });
                Stopwatch progressTimer = Stopwatch.StartNew();
                Stopwatch speedTimer = Stopwatch.StartNew();
                await client.ReadBlockDeviceAsync(
                    suType,
                    suPassword,
                    selectedDevice?.Path ?? device,
                    blockSize,
                    count,
                    selectedDevice?.Size,
                    targetStream,
                    (bytesDone, bytesFull) => {
                        if (!speedTimer.IsRunning) speedTimer.Start();

                        if (progressTimer.ElapsedMilliseconds > 200) {
                            progressTimer.Restart();

                            string speed = "";
                            if (speedTimer.IsRunning && speedTimer.ElapsedMilliseconds > 1000) {
                                long bps = Convert.ToInt64(Math.Floor(bytesDone / ((double)speedTimer.ElapsedMilliseconds / 1000.0)));
                                speed = $" ({bps.ToHumanReadableSize()}/s)";
                            }
                            string progressText;
                            double percent;
                            if (bytesFull.HasValue) {
                                percent = (double)bytesDone / (double)bytesFull.Value * 100.0;
                                progressText = $"{bytesDone.ToHumanReadableSize()} / {bytesFull.Value.ToHumanReadableSize()} ({percent:0.00}%)";
                            } else {
                                // Display a progress bar which slows down to never reach 100.0
                                percent = (1.0 - (1.0 / ((double)bytesDone / (500.0 * 1024.0 * 1024.0) + 1.0))) * 100.0;
                                progressText = $"{bytesDone} ({bytesDone.ToHumanReadableSize()})";
                            }
                            this.Invoke(() => {
                                lblStatus.Text = $"Copying data... {progressText}{speed}";
                                proProgress.Value = int.Max(0, (int)Math.Min(10000.0, Math.Floor(percent * 100.0)));
                            });
                        }
                    },
                    cancelToken
                );
            }

            this.Invoke(() => {
                lblStatus.Text = $"Done.";
                proProgress.Value = 10000;
            });
        } catch (Exception ex) {
            this.Invoke(() => {
                lblStatus.Text = "Error: " + ex.Message;
                MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        } finally {
            this.Invoke(() => {
                btnStart.Text = "Start";
                proProgress.Visible = false;
            });
        }
    }
}
