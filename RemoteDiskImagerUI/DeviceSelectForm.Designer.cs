namespace RemoteDiskImagerUI {
    partial class DeviceSelectForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            lstDevices = new ListView();
            colPath = new ColumnHeader();
            colSize = new ColumnHeader();
            colType = new ColumnHeader();
            btnOk = new Button();
            btnCancel = new Button();
            this.SuspendLayout();
            // 
            // lstDevices
            // 
            lstDevices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstDevices.Columns.AddRange(new ColumnHeader[] { colPath, colSize, colType });
            lstDevices.FullRowSelect = true;
            lstDevices.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstDevices.Location = new Point(10, 9);
            lstDevices.Margin = new Padding(3, 2, 3, 2);
            lstDevices.MultiSelect = false;
            lstDevices.Name = "lstDevices";
            lstDevices.ShowGroups = false;
            lstDevices.Size = new Size(680, 278);
            lstDevices.TabIndex = 0;
            lstDevices.UseCompatibleStateImageBehavior = false;
            lstDevices.View = View.Details;
            lstDevices.ItemSelectionChanged += this.lstDevices_ItemSelectionChanged;
            // 
            // colPath
            // 
            colPath.Text = "Path";
            colPath.Width = 320;
            // 
            // colSize
            // 
            colSize.Text = "Size";
            colSize.Width = 180;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 80;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnOk.Location = new Point(599, 290);
            btnOk.Margin = new Padding(3, 2, 3, 2);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(90, 38);
            btnOk.TabIndex = 1;
            btnOk.Text = "Select";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += this.btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.Location = new Point(10, 290);
            btnCancel.Margin = new Padding(3, 2, 3, 2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 38);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += this.btnCancel_Click;
            // 
            // DeviceSelectForm
            // 
            this.AcceptButton = btnOk;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = btnCancel;
            this.ClientSize = new Size(700, 338);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnOk);
            this.Controls.Add(lstDevices);
            this.Margin = new Padding(3, 2, 3, 2);
            this.Name = "DeviceSelectForm";
            this.Text = "DeviceSelectForm";
            this.ResumeLayout(false);
        }

        #endregion

        private ListView lstDevices;
        private ColumnHeader colPath;
        private ColumnHeader colSize;
        private ColumnHeader colType;
        private Button btnOk;
        private Button btnCancel;
    }
}