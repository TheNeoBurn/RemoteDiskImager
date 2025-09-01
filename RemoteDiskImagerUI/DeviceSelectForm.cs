using RemoteDiskImanger;

namespace RemoteDiskImagerUI {
    public partial class DeviceSelectForm : Form {
        public BlockDeviceInfo? SelectedDevice { get; private set; }

        public DeviceSelectForm(List<BlockDeviceInfo> devices) {
            InitializeComponent();

            lstDevices.Items.Clear();
            foreach (BlockDeviceInfo device in devices) {
                var lvi = new ListViewItem(new string[] {
                    device.Path,
                    device.HumanReadableSize,
                    device.Type
                });
                lvi.Tag = device;
                lstDevices.Items.Add(lvi);
            }

            this.SelectedDevice = null;

            btnOk.Enabled = false;

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


        private void lstDevices_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            btnOk.Enabled = lstDevices.SelectedItems.Count == 1;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            if (lstDevices.SelectedItems.Count == 1) {
                this.SelectedDevice = (BlockDeviceInfo?)lstDevices.SelectedItems[0].Tag;
            } else {
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
