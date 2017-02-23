﻿namespace A4LGSharedFunctions
{
    partial class ConfigFormNoLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFormNoLog));
            this.btnOpenConfigLoc = new System.Windows.Forms.Button();
            this.txtBxPath = new System.Windows.Forms.TextBox();
            this.cboConfigs = new System.Windows.Forms.ComboBox();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gpBxConfig = new System.Windows.Forms.GroupBox();
            this.btnPreviewLoaded = new System.Windows.Forms.Button();
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtBxLoadedConfig = new System.Windows.Forms.TextBox();
            this.gpBxconfigFiles = new System.Windows.Forms.GroupBox();
            this.chkBxBackupConfig = new System.Windows.Forms.CheckBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.gpBxConfig.SuspendLayout();
            this.gpBxconfigFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenConfigLoc
            // 
            this.btnOpenConfigLoc.Location = new System.Drawing.Point(13, 123);
            this.btnOpenConfigLoc.Name = "btnOpenConfigLoc";
            this.btnOpenConfigLoc.Size = new System.Drawing.Size(144, 23);
            this.btnOpenConfigLoc.TabIndex = 0;
            this.btnOpenConfigLoc.Text = "Open location";
            this.btnOpenConfigLoc.UseVisualStyleBackColor = true;
            this.btnOpenConfigLoc.Click += new System.EventHandler(this.btnOpenLoc_Click);
            // 
            // txtBxPath
            // 
            this.txtBxPath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtBxPath.Location = new System.Drawing.Point(13, 95);
            this.txtBxPath.Name = "txtBxPath";
            this.txtBxPath.ReadOnly = true;
            this.txtBxPath.Size = new System.Drawing.Size(471, 20);
            this.txtBxPath.TabIndex = 1;
            // 
            // cboConfigs
            // 
            this.cboConfigs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConfigs.FormattingEnabled = true;
            this.cboConfigs.Location = new System.Drawing.Point(13, 29);
            this.cboConfigs.Name = "cboConfigs";
            this.cboConfigs.Size = new System.Drawing.Size(276, 21);
            this.cboConfigs.TabIndex = 2;
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(13, 57);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(145, 23);
            this.btnLoadConfig.TabIndex = 3;
            this.btnLoadConfig.Text = "Load";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Config File Path";
            // 
            // gpBxConfig
            // 
            this.gpBxConfig.Controls.Add(this.btnPreviewLoaded);
            this.gpBxConfig.Controls.Add(this.lblConfig);
            this.gpBxConfig.Controls.Add(this.txtBxLoadedConfig);
            this.gpBxConfig.Controls.Add(this.label1);
            this.gpBxConfig.Controls.Add(this.btnOpenConfigLoc);
            this.gpBxConfig.Controls.Add(this.txtBxPath);
            this.gpBxConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpBxConfig.Location = new System.Drawing.Point(0, 0);
            this.gpBxConfig.Name = "gpBxConfig";
            this.gpBxConfig.Size = new System.Drawing.Size(495, 158);
            this.gpBxConfig.TabIndex = 5;
            this.gpBxConfig.TabStop = false;
            // 
            // btnPreviewLoaded
            // 
            this.btnPreviewLoaded.Location = new System.Drawing.Point(315, 40);
            this.btnPreviewLoaded.Name = "btnPreviewLoaded";
            this.btnPreviewLoaded.Size = new System.Drawing.Size(145, 23);
            this.btnPreviewLoaded.TabIndex = 5;
            this.btnPreviewLoaded.Text = "Preview";
            this.btnPreviewLoaded.UseVisualStyleBackColor = true;
            this.btnPreviewLoaded.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Location = new System.Drawing.Point(13, 20);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(76, 13);
            this.lblConfig.TabIndex = 6;
            this.lblConfig.Text = "Loaded Config";
            // 
            // txtBxLoadedConfig
            // 
            this.txtBxLoadedConfig.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtBxLoadedConfig.Location = new System.Drawing.Point(13, 40);
            this.txtBxLoadedConfig.Name = "txtBxLoadedConfig";
            this.txtBxLoadedConfig.ReadOnly = true;
            this.txtBxLoadedConfig.Size = new System.Drawing.Size(276, 20);
            this.txtBxLoadedConfig.TabIndex = 5;
            // 
            // gpBxconfigFiles
            // 
            this.gpBxconfigFiles.Controls.Add(this.chkBxBackupConfig);
            this.gpBxconfigFiles.Controls.Add(this.btnPreview);
            this.gpBxconfigFiles.Controls.Add(this.btnLoadConfig);
            this.gpBxconfigFiles.Controls.Add(this.cboConfigs);
            this.gpBxconfigFiles.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpBxconfigFiles.Location = new System.Drawing.Point(0, 164);
            this.gpBxconfigFiles.Name = "gpBxconfigFiles";
            this.gpBxconfigFiles.Size = new System.Drawing.Size(495, 92);
            this.gpBxconfigFiles.TabIndex = 6;
            this.gpBxconfigFiles.TabStop = false;
            this.gpBxconfigFiles.Text = "Configuration Files";
            // 
            // chkBxBackupConfig
            // 
            this.chkBxBackupConfig.AutoSize = true;
            this.chkBxBackupConfig.Checked = true;
            this.chkBxBackupConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxBackupConfig.Location = new System.Drawing.Point(164, 61);
            this.chkBxBackupConfig.Name = "chkBxBackupConfig";
            this.chkBxBackupConfig.Size = new System.Drawing.Size(80, 17);
            this.chkBxBackupConfig.TabIndex = 5;
            this.chkBxBackupConfig.Text = "checkBox1";
            this.chkBxBackupConfig.UseVisualStyleBackColor = true;
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(300, 28);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(145, 23);
            this.btnPreview.TabIndex = 4;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // ConfigFormNoLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 256);
            this.Controls.Add(this.gpBxconfigFiles);
            this.Controls.Add(this.gpBxConfig);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigFormNoLog";
            this.Text = "Config File Dialog";
            this.Load += new System.EventHandler(this.ConfigFormNoLog_Load);
            this.gpBxConfig.ResumeLayout(false);
            this.gpBxConfig.PerformLayout();
            this.gpBxconfigFiles.ResumeLayout(false);
            this.gpBxconfigFiles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenConfigLoc;
        private System.Windows.Forms.TextBox txtBxPath;
        private System.Windows.Forms.ComboBox cboConfigs;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gpBxConfig;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.TextBox txtBxLoadedConfig;
        private System.Windows.Forms.GroupBox gpBxconfigFiles;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPreviewLoaded;
        private System.Windows.Forms.CheckBox chkBxBackupConfig;
    }
}