namespace IPA.MainApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ApplicationlblSerialNumber = new System.Windows.Forms.Label();
            this.ApplicationPicBoxWait = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ApplicationFirmwarelblVersion = new System.Windows.Forms.Label();
            this.ApplicationlblModelName = new System.Windows.Forms.Label();
            this.ApplicationlblPort = new System.Windows.Forms.Label();
            this.ApplicationlblDeviceOS = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ApplicationlblStatus = new System.Windows.Forms.Label();
            this.ApplicationgroupBoxFirmwareUpdate = new System.Windows.Forms.GroupBox();
            this.ApplicationgrpBoxRBAUpdateVersion = new System.Windows.Forms.GroupBox();
            this.ApplicationrBtnVITAL = new System.Windows.Forms.RadioButton();
            this.ApplicationrBtnFDRC = new System.Windows.Forms.RadioButton();
            this.ApplicationbtnUpdate = new System.Windows.Forms.Button();
            this.ApplicationlblUpdate = new System.Windows.Forms.Label();
            this.ApplicationlblHardwareVersion = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationPicBoxWait)).BeginInit();
            this.ApplicationgroupBoxFirmwareUpdate.SuspendLayout();
            this.ApplicationgrpBoxRBAUpdateVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(784, 56);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Serial Number:";
            // 
            // ApplicationlblSerialNumber
            // 
            this.ApplicationlblSerialNumber.AutoSize = true;
            this.ApplicationlblSerialNumber.Location = new System.Drawing.Point(171, 147);
            this.ApplicationlblSerialNumber.Name = "ApplicationlblSerialNumber";
            this.ApplicationlblSerialNumber.Size = new System.Drawing.Size(65, 13);
            this.ApplicationlblSerialNumber.TabIndex = 2;
            this.ApplicationlblSerialNumber.Text = "UNKNOWN";
            // 
            // ApplicationPicBoxWait
            // 
            this.ApplicationPicBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("ApplicationPicBoxWait.Image")));
            this.ApplicationPicBoxWait.Location = new System.Drawing.Point(26, 69);
            this.ApplicationPicBoxWait.Name = "ApplicationPicBoxWait";
            this.ApplicationPicBoxWait.Size = new System.Drawing.Size(736, 413);
            this.ApplicationPicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ApplicationPicBoxWait.TabIndex = 3;
            this.ApplicationPicBoxWait.TabStop = false;
            this.ApplicationPicBoxWait.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Firmware Version:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Model Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 327);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Port:";
            // 
            // ApplicationFirmwarelblVersion
            // 
            this.ApplicationFirmwarelblVersion.AutoSize = true;
            this.ApplicationFirmwarelblVersion.Location = new System.Drawing.Point(171, 192);
            this.ApplicationFirmwarelblVersion.Name = "ApplicationFirmwarelblVersion";
            this.ApplicationFirmwarelblVersion.Size = new System.Drawing.Size(65, 13);
            this.ApplicationFirmwarelblVersion.TabIndex = 7;
            this.ApplicationFirmwarelblVersion.Text = "UNKNOWN";
            // 
            // ApplicationlblModelName
            // 
            this.ApplicationlblModelName.AutoSize = true;
            this.ApplicationlblModelName.Location = new System.Drawing.Point(171, 237);
            this.ApplicationlblModelName.Name = "ApplicationlblModelName";
            this.ApplicationlblModelName.Size = new System.Drawing.Size(65, 13);
            this.ApplicationlblModelName.TabIndex = 8;
            this.ApplicationlblModelName.Text = "UNKNOWN";
            // 
            // ApplicationlblPort
            // 
            this.ApplicationlblPort.AutoSize = true;
            this.ApplicationlblPort.Location = new System.Drawing.Point(171, 327);
            this.ApplicationlblPort.Name = "ApplicationlblPort";
            this.ApplicationlblPort.Size = new System.Drawing.Size(65, 13);
            this.ApplicationlblPort.TabIndex = 9;
            this.ApplicationlblPort.Text = "UNKNOWN";
            // 
            // ApplicationlblDeviceOS
            // 
            this.ApplicationlblDeviceOS.AutoSize = true;
            this.ApplicationlblDeviceOS.Location = new System.Drawing.Point(171, 102);
            this.ApplicationlblDeviceOS.Name = "ApplicationlblDeviceOS";
            this.ApplicationlblDeviceOS.Size = new System.Drawing.Size(65, 13);
            this.ApplicationlblDeviceOS.TabIndex = 11;
            this.ApplicationlblDeviceOS.Text = "UNKNOWN";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(57, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Device OS:";
            // 
            // ApplicationlblStatus
            // 
            this.ApplicationlblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ApplicationlblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplicationlblStatus.Location = new System.Drawing.Point(23, 497);
            this.ApplicationlblStatus.Name = "ApplicationlblStatus";
            this.ApplicationlblStatus.Size = new System.Drawing.Size(739, 41);
            this.ApplicationlblStatus.TabIndex = 12;
            this.ApplicationlblStatus.Text = "STATUS:";
            this.ApplicationlblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ApplicationgroupBoxFirmwareUpdate
            // 
            this.ApplicationgroupBoxFirmwareUpdate.Controls.Add(this.ApplicationgrpBoxRBAUpdateVersion);
            this.ApplicationgroupBoxFirmwareUpdate.Controls.Add(this.ApplicationbtnUpdate);
            this.ApplicationgroupBoxFirmwareUpdate.Controls.Add(this.ApplicationlblUpdate);
            this.ApplicationgroupBoxFirmwareUpdate.Location = new System.Drawing.Point(26, 373);
            this.ApplicationgroupBoxFirmwareUpdate.Name = "ApplicationgroupBoxFirmwareUpdate";
            this.ApplicationgroupBoxFirmwareUpdate.Size = new System.Drawing.Size(735, 98);
            this.ApplicationgroupBoxFirmwareUpdate.TabIndex = 13;
            this.ApplicationgroupBoxFirmwareUpdate.TabStop = false;
            this.ApplicationgroupBoxFirmwareUpdate.Text = "Firmware Update";
            this.ApplicationgroupBoxFirmwareUpdate.Visible = false;
            // 
            // ApplicationgrpBoxRBAUpdateVersion
            // 
            this.ApplicationgrpBoxRBAUpdateVersion.Controls.Add(this.ApplicationrBtnVITAL);
            this.ApplicationgrpBoxRBAUpdateVersion.Controls.Add(this.ApplicationrBtnFDRC);
            this.ApplicationgrpBoxRBAUpdateVersion.Location = new System.Drawing.Point(466, 28);
            this.ApplicationgrpBoxRBAUpdateVersion.Name = "ApplicationgrpBoxRBAUpdateVersion";
            this.ApplicationgrpBoxRBAUpdateVersion.Size = new System.Drawing.Size(216, 47);
            this.ApplicationgrpBoxRBAUpdateVersion.TabIndex = 2;
            this.ApplicationgrpBoxRBAUpdateVersion.TabStop = false;
            this.ApplicationgrpBoxRBAUpdateVersion.Text = "RBA Firmware Version";
            this.ApplicationgrpBoxRBAUpdateVersion.Visible = false;
            // 
            // ApplicationrBtnVITAL
            // 
            this.ApplicationrBtnVITAL.AutoSize = true;
            this.ApplicationrBtnVITAL.Location = new System.Drawing.Point(132, 16);
            this.ApplicationrBtnVITAL.Name = "ApplicationrBtnVITAL";
            this.ApplicationrBtnVITAL.Size = new System.Drawing.Size(55, 17);
            this.ApplicationrBtnVITAL.TabIndex = 1;
            this.ApplicationrBtnVITAL.TabStop = true;
            this.ApplicationrBtnVITAL.Text = "VITAL";
            this.ApplicationrBtnVITAL.UseVisualStyleBackColor = true;
            // 
            // ApplicationrBtnFDRC
            // 
            this.ApplicationrBtnFDRC.AutoSize = true;
            this.ApplicationrBtnFDRC.Location = new System.Drawing.Point(65, 16);
            this.ApplicationrBtnFDRC.Name = "ApplicationrBtnFDRC";
            this.ApplicationrBtnFDRC.Size = new System.Drawing.Size(54, 17);
            this.ApplicationrBtnFDRC.TabIndex = 0;
            this.ApplicationrBtnFDRC.TabStop = true;
            this.ApplicationrBtnFDRC.Text = "FDRC";
            this.ApplicationrBtnFDRC.UseVisualStyleBackColor = true;
            // 
            // ApplicationbtnUpdate
            // 
            this.ApplicationbtnUpdate.Location = new System.Drawing.Point(321, 37);
            this.ApplicationbtnUpdate.Name = "ApplicationbtnUpdate";
            this.ApplicationbtnUpdate.Size = new System.Drawing.Size(97, 30);
            this.ApplicationbtnUpdate.TabIndex = 1;
            this.ApplicationbtnUpdate.Text = "UPDATE";
            this.ApplicationbtnUpdate.UseVisualStyleBackColor = true;
            this.ApplicationbtnUpdate.Visible = false;
            this.ApplicationbtnUpdate.Click += new System.EventHandler(this.ApplicationbtnUpdate_Click);
            // 
            // ApplicationlblUpdate
            // 
            this.ApplicationlblUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplicationlblUpdate.Location = new System.Drawing.Point(18, 37);
            this.ApplicationlblUpdate.Name = "ApplicationlblUpdate";
            this.ApplicationlblUpdate.Size = new System.Drawing.Size(246, 30);
            this.ApplicationlblUpdate.TabIndex = 0;
            this.ApplicationlblUpdate.Text = "UPDATE TO UIA";
            this.ApplicationlblUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationlblUpdate.Visible = false;
            // 
            // ApplicationlblHardwareVersion
            // 
            this.ApplicationlblHardwareVersion.AutoSize = true;
            this.ApplicationlblHardwareVersion.Location = new System.Drawing.Point(171, 282);
            this.ApplicationlblHardwareVersion.Name = "ApplicationlblHardwareVersion";
            this.ApplicationlblHardwareVersion.Size = new System.Drawing.Size(65, 13);
            this.ApplicationlblHardwareVersion.TabIndex = 15;
            this.ApplicationlblHardwareVersion.Text = "UNKNOWN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 282);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Hardware Version:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 567);
            this.Controls.Add(this.ApplicationPicBoxWait);
            this.Controls.Add(this.ApplicationlblHardwareVersion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ApplicationgroupBoxFirmwareUpdate);
            this.Controls.Add(this.ApplicationlblStatus);
            this.Controls.Add(this.ApplicationlblDeviceOS);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ApplicationlblPort);
            this.Controls.Add(this.ApplicationlblModelName);
            this.Controls.Add(this.ApplicationFirmwarelblVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ApplicationlblSerialNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationPicBoxWait)).EndInit();
            this.ApplicationgroupBoxFirmwareUpdate.ResumeLayout(false);
            this.ApplicationgrpBoxRBAUpdateVersion.ResumeLayout(false);
            this.ApplicationgrpBoxRBAUpdateVersion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ApplicationlblSerialNumber;
        private System.Windows.Forms.PictureBox ApplicationPicBoxWait;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ApplicationFirmwarelblVersion;
        private System.Windows.Forms.Label ApplicationlblModelName;
        private System.Windows.Forms.Label ApplicationlblPort;
        private System.Windows.Forms.Label ApplicationlblDeviceOS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label ApplicationlblStatus;
        private System.Windows.Forms.GroupBox ApplicationgroupBoxFirmwareUpdate;
        private System.Windows.Forms.Button ApplicationbtnUpdate;
        private System.Windows.Forms.Label ApplicationlblUpdate;
        private System.Windows.Forms.GroupBox ApplicationgrpBoxRBAUpdateVersion;
        private System.Windows.Forms.RadioButton ApplicationrBtnVITAL;
        private System.Windows.Forms.RadioButton ApplicationrBtnFDRC;
        private System.Windows.Forms.Label ApplicationlblHardwareVersion;
        private System.Windows.Forms.Label label7;
    }
}

