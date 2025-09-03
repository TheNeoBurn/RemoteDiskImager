using RemoteDiskImanger;

namespace RemoteDiskImagerUI {
    public partial class DeviceSelectForm : Form {
        public BlockDeviceInfo? SelectedDevice { get; private set; }

        public DeviceSelectForm(List<BlockDeviceInfo> devices) {
            InitializeComponent();

            lstDevices.Items.Clear();
            foreach (BlockDeviceInfo device in devices) {
                var lvi = new ListViewItem(new string[] {
                    ((device.Children?.Length ?? 0) == 0 ? " - " : "") + device.Path,
                    device.HumanReadableSize,
                    device.Type
                });
                lvi.Tag = device;
                lstDevices.Items.Add(lvi);
            }

            this.SelectedDevice = null;

            btnOk.Enabled = false;

            if (FormsHelper.IsWindowsDarkMode())
                FormsHelper.ApplyThemeColors(this);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            FormsHelper.TryEnableDarkMode(this.Handle);
        }

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
