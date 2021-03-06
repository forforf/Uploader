﻿namespace Uploader
{
    partial class Uploader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Uploader));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxLocalPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textBoxS3Path = new System.Windows.Forms.TextBox();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnWatchStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnWatchStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxLocalPath
            // 
            this.textBoxLocalPath.Location = new System.Drawing.Point(135, 22);
            this.textBoxLocalPath.Name = "textBoxLocalPath";
            this.textBoxLocalPath.ReadOnly = true;
            this.textBoxLocalPath.Size = new System.Drawing.Size(396, 20);
            this.textBoxLocalPath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(537, 19);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textBoxS3Path
            // 
            this.textBoxS3Path.Location = new System.Drawing.Point(135, 70);
            this.textBoxS3Path.Name = "textBoxS3Path";
            this.textBoxS3Path.Size = new System.Drawing.Size(297, 20);
            this.textBoxS3Path.TabIndex = 2;
            this.textBoxS3Path.TextChanged += new System.EventHandler(this.textBoxS3Path_TextChanged);
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.BackColor = System.Drawing.SystemColors.Menu;
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.Location = new System.Drawing.Point(12, 159);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(552, 160);
            this.listBoxStatus.TabIndex = 3;
            // 
            // btnUpload
            // 
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpload.Location = new System.Drawing.Point(452, 54);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(53, 36);
            this.btnUpload.TabIndex = 4;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Local Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(12, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "S3 Bucket/Folder";
            // 
            // btnWatchStart
            // 
            this.btnWatchStart.Image = global::Uploader.Properties.Resources.cloud_spinner_off;
            this.btnWatchStart.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnWatchStart.Location = new System.Drawing.Point(511, 54);
            this.btnWatchStart.Name = "btnWatchStart";
            this.btnWatchStart.Size = new System.Drawing.Size(53, 36);
            this.btnWatchStart.TabIndex = 8;
            this.btnWatchStart.UseVisualStyleBackColor = true;
            this.btnWatchStart.Click += new System.EventHandler(this.btnWatchStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Status";
            // 
            // btnWatchStop
            // 
            this.btnWatchStop.Image = global::Uploader.Properties.Resources.cloud_spinner_on;
            this.btnWatchStop.Location = new System.Drawing.Point(511, 54);
            this.btnWatchStop.Name = "btnWatchStop";
            this.btnWatchStop.Size = new System.Drawing.Size(53, 36);
            this.btnWatchStop.TabIndex = 10;
            this.btnWatchStop.UseVisualStyleBackColor = true;
            this.btnWatchStop.Visible = false;
            this.btnWatchStop.Click += new System.EventHandler(this.btnWatchStop_Click);
            // 
            // Uploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(577, 331);
            this.Controls.Add(this.btnWatchStop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnWatchStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.textBoxS3Path);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.textBoxLocalPath);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Uploader";
            this.Text = "Uploader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Uploader_FormClosed);
            this.Load += new System.EventHandler(this.Uploader_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBoxLocalPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox textBoxS3Path;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnWatchStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWatchStop;
    }
}

