namespace RemoteDiskImagerUI {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            label1 = new Label();
            txtUsername = new TextBox();
            txtCertFile = new TextBox();
            label2 = new Label();
            btnSelectCertFile = new Button();
            txtPassword = new TextBox();
            label3 = new Label();
            txtServer = new TextBox();
            label4 = new Label();
            label5 = new Label();
            txtBlockSize = new TextBox();
            txtCount = new TextBox();
            label6 = new Label();
            label7 = new Label();
            optSu = new ComboBox();
            txtSuPassword = new TextBox();
            label8 = new Label();
            btnStart = new Button();
            proProgress = new ProgressBar();
            txtDevice = new TextBox();
            label9 = new Label();
            label10 = new Label();
            numPort = new NumericUpDown();
            txtTargetFile = new TextBox();
            label11 = new Label();
            btnSelectTargetFile = new Button();
            lblStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(78, 20);
            label1.TabIndex = 0;
            label1.Text = "Username:";
            // 
            // txtUsername
            // 
            txtUsername.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUsername.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsername.Location = new Point(12, 32);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(755, 30);
            txtUsername.TabIndex = 1;
            // 
            // txtCertFile
            // 
            txtCertFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCertFile.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCertFile.Location = new Point(12, 88);
            txtCertFile.Name = "txtCertFile";
            txtCertFile.Size = new Size(701, 30);
            txtCertFile.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 65);
            label2.Name = "label2";
            label2.Size = new Size(175, 20);
            label2.TabIndex = 2;
            label2.Text = "Certificate file (optional):";
            // 
            // btnSelectCertFile
            // 
            btnSelectCertFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectCertFile.Location = new Point(719, 88);
            btnSelectCertFile.Name = "btnSelectCertFile";
            btnSelectCertFile.Size = new Size(48, 30);
            btnSelectCertFile.TabIndex = 4;
            btnSelectCertFile.Text = "...";
            btnSelectCertFile.UseVisualStyleBackColor = true;
            btnSelectCertFile.Click += this.btnSelectCertFile_Click;
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.Location = new Point(12, 144);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.Size = new Size(755, 30);
            txtPassword.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 121);
            label3.Name = "label3";
            label3.Size = new Size(202, 20);
            label3.TabIndex = 5;
            label3.Text = "Password (user or certificate):";
            // 
            // txtServer
            // 
            txtServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServer.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtServer.Location = new Point(12, 200);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(654, 30);
            txtServer.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 177);
            label4.Name = "label4";
            label4.Size = new Size(53, 20);
            label4.TabIndex = 7;
            label4.Text = "Server:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 233);
            label5.Name = "label5";
            label5.Size = new Size(77, 20);
            label5.TabIndex = 9;
            label5.Text = "Block size:";
            // 
            // txtBlockSize
            // 
            txtBlockSize.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtBlockSize.Location = new Point(12, 256);
            txtBlockSize.Name = "txtBlockSize";
            txtBlockSize.Size = new Size(145, 30);
            txtBlockSize.TabIndex = 10;
            txtBlockSize.Text = "8M";
            // 
            // txtCount
            // 
            txtCount.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCount.Location = new Point(188, 256);
            txtCount.Name = "txtCount";
            txtCount.Size = new Size(145, 30);
            txtCount.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(188, 233);
            label6.Name = "label6";
            label6.Size = new Size(51, 20);
            label6.TabIndex = 11;
            label6.Text = "Count:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 289);
            label7.Name = "label7";
            label7.Size = new Size(112, 20);
            label7.TabIndex = 13;
            label7.Text = "Elivate method:";
            // 
            // optSu
            // 
            optSu.DropDownStyle = ComboBoxStyle.DropDownList;
            optSu.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optSu.FormattingEnabled = true;
            optSu.Items.AddRange(new object[] { "None", "sudo", "su" });
            optSu.Location = new Point(12, 312);
            optSu.Name = "optSu";
            optSu.Size = new Size(145, 31);
            optSu.TabIndex = 14;
            // 
            // txtSuPassword
            // 
            txtSuPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSuPassword.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSuPassword.Location = new Point(188, 312);
            txtSuPassword.Name = "txtSuPassword";
            txtSuPassword.PasswordChar = '●';
            txtSuPassword.Size = new Size(579, 30);
            txtSuPassword.TabIndex = 16;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(188, 289);
            label8.Name = "label8";
            label8.Size = new Size(95, 20);
            label8.TabIndex = 15;
            label8.Text = "Su password:";
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnStart.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(637, 504);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(130, 53);
            btnStart.TabIndex = 100;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += this.btnStart_Click;
            // 
            // proProgress
            // 
            proProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            proProgress.Location = new Point(12, 504);
            proProgress.Maximum = 10000;
            proProgress.Name = "proProgress";
            proProgress.Size = new Size(619, 53);
            proProgress.TabIndex = 18;
            proProgress.Visible = false;
            // 
            // txtDevice
            // 
            txtDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDevice.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtDevice.Location = new Point(12, 369);
            txtDevice.Name = "txtDevice";
            txtDevice.Size = new Size(755, 30);
            txtDevice.TabIndex = 20;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 346);
            label9.Name = "label9";
            label9.Size = new Size(188, 20);
            label9.TabIndex = 19;
            label9.Text = "Device (empty = open list):";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(682, 177);
            label10.Name = "label10";
            label10.Size = new Size(38, 20);
            label10.TabIndex = 22;
            label10.Text = "Port:";
            // 
            // numPort
            // 
            numPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numPort.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numPort.Location = new Point(682, 200);
            numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPort.Name = "numPort";
            numPort.Size = new Size(85, 30);
            numPort.TabIndex = 9;
            numPort.TextAlign = HorizontalAlignment.Right;
            numPort.Value = new decimal(new int[] { 22, 0, 0, 0 });
            // 
            // txtTargetFile
            // 
            txtTargetFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTargetFile.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTargetFile.Location = new Point(12, 425);
            txtTargetFile.Name = "txtTargetFile";
            txtTargetFile.Size = new Size(701, 30);
            txtTargetFile.TabIndex = 24;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 402);
            label11.Name = "label11";
            label11.Size = new Size(78, 20);
            label11.TabIndex = 23;
            label11.Text = "Target file:";
            // 
            // btnSelectTargetFile
            // 
            btnSelectTargetFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectTargetFile.Location = new Point(719, 425);
            btnSelectTargetFile.Name = "btnSelectTargetFile";
            btnSelectTargetFile.Size = new Size(48, 30);
            btnSelectTargetFile.TabIndex = 25;
            btnSelectTargetFile.Text = "...";
            btnSelectTargetFile.UseVisualStyleBackColor = true;
            btnSelectTargetFile.Click += this.btnSelectTargetFile_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 481);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(18, 20);
            lblStatus.TabIndex = 101;
            lblStatus.Text = "...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(779, 569);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnSelectTargetFile);
            this.Controls.Add(txtTargetFile);
            this.Controls.Add(label11);
            this.Controls.Add(numPort);
            this.Controls.Add(label10);
            this.Controls.Add(txtDevice);
            this.Controls.Add(label9);
            this.Controls.Add(proProgress);
            this.Controls.Add(btnStart);
            this.Controls.Add(txtSuPassword);
            this.Controls.Add(label8);
            this.Controls.Add(optSu);
            this.Controls.Add(label7);
            this.Controls.Add(txtCount);
            this.Controls.Add(label6);
            this.Controls.Add(txtBlockSize);
            this.Controls.Add(label5);
            this.Controls.Add(txtServer);
            this.Controls.Add(label4);
            this.Controls.Add(txtPassword);
            this.Controls.Add(label3);
            this.Controls.Add(btnSelectCertFile);
            this.Controls.Add(txtCertFile);
            this.Controls.Add(label2);
            this.Controls.Add(txtUsername);
            this.Controls.Add(label1);
            this.Name = "Form1";
            this.Text = "Remote Disk Imager";
            ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtUsername;
        private TextBox txtCertFile;
        private Label label2;
        private Button btnSelectCertFile;
        private TextBox txtPassword;
        private Label label3;
        private TextBox txtServer;
        private Label label4;
        private Label label5;
        private TextBox txtBlockSize;
        private TextBox txtCount;
        private Label label6;
        private Label label7;
        private ComboBox optSu;
        private TextBox txtSuPassword;
        private Label label8;
        private Button btnStart;
        private ProgressBar proProgress;
        private TextBox txtDevice;
        private Label label9;
        private Label label10;
        private NumericUpDown numPort;
        private TextBox txtTargetFile;
        private Label label11;
        private Button btnSelectTargetFile;
        private Label lblStatus;
    }
}
