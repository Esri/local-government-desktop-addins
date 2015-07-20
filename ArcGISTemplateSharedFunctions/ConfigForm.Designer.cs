namespace A4LGSharedFunctions
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.btnOpenConfigLoc = new System.Windows.Forms.Button();
            this.txtBxPath = new System.Windows.Forms.TextBox();
            this.cboConfigs = new System.Windows.Forms.ComboBox();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gpBxConfig = new System.Windows.Forms.GroupBox();
            this.btnPreviewLoaded = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtBxLoadedConfig = new System.Windows.Forms.TextBox();
            this.gpBxconfigFiles = new System.Windows.Forms.GroupBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.gpBxLog = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPrevLog = new System.Windows.Forms.Button();
            this.btnOpenLogLoc = new System.Windows.Forms.Button();
            this.txtBoxLogPath = new System.Windows.Forms.TextBox();
            this.gpBxConfig.SuspendLayout();
            this.gpBxconfigFiles.SuspendLayout();
            this.gpBxLog.SuspendLayout();
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
            this.gpBxConfig.Controls.Add(this.btnReload);
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
            this.btnPreviewLoaded.Location = new System.Drawing.Point(316, 52);
            this.btnPreviewLoaded.Name = "btnPreviewLoaded";
            this.btnPreviewLoaded.Size = new System.Drawing.Size(145, 23);
            this.btnPreviewLoaded.TabIndex = 5;
            this.btnPreviewLoaded.Text = "Preview";
            this.btnPreviewLoaded.UseVisualStyleBackColor = true;
            this.btnPreviewLoaded.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(316, 23);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(145, 23);
            this.btnReload.TabIndex = 7;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
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
            this.gpBxconfigFiles.Controls.Add(this.btnPreview);
            this.gpBxconfigFiles.Controls.Add(this.btnLoadConfig);
            this.gpBxconfigFiles.Controls.Add(this.cboConfigs);
            this.gpBxconfigFiles.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpBxconfigFiles.Location = new System.Drawing.Point(0, 251);
            this.gpBxconfigFiles.Name = "gpBxconfigFiles";
            this.gpBxconfigFiles.Size = new System.Drawing.Size(495, 92);
            this.gpBxconfigFiles.TabIndex = 6;
            this.gpBxconfigFiles.TabStop = false;
            this.gpBxconfigFiles.Text = "Configuration Files";
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
            // gpBxLog
            // 
            this.gpBxLog.Controls.Add(this.label2);
            this.gpBxLog.Controls.Add(this.btnPrevLog);
            this.gpBxLog.Controls.Add(this.btnOpenLogLoc);
            this.gpBxLog.Controls.Add(this.txtBoxLogPath);
            this.gpBxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpBxLog.Location = new System.Drawing.Point(0, 158);
            this.gpBxLog.Name = "gpBxLog";
            this.gpBxLog.Size = new System.Drawing.Size(495, 93);
            this.gpBxLog.TabIndex = 7;
            this.gpBxLog.TabStop = false;
            this.gpBxLog.Text = "Log File";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(13, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Note:  You must have the AA turned off to open this file.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPrevLog
            // 
            this.btnPrevLog.Location = new System.Drawing.Point(162, 46);
            this.btnPrevLog.Name = "btnPrevLog";
            this.btnPrevLog.Size = new System.Drawing.Size(145, 23);
            this.btnPrevLog.TabIndex = 6;
            this.btnPrevLog.Text = "Preview";
            this.btnPrevLog.UseVisualStyleBackColor = true;
            this.btnPrevLog.Click += new System.EventHandler(this.btnPrevLog_Click);
            // 
            // btnOpenLogLoc
            // 
            this.btnOpenLogLoc.Location = new System.Drawing.Point(12, 46);
            this.btnOpenLogLoc.Name = "btnOpenLogLoc";
            this.btnOpenLogLoc.Size = new System.Drawing.Size(145, 23);
            this.btnOpenLogLoc.TabIndex = 2;
            this.btnOpenLogLoc.Text = "Open location";
            this.btnOpenLogLoc.UseVisualStyleBackColor = true;
            this.btnOpenLogLoc.Click += new System.EventHandler(this.btnOpenLogLoc_Click);
            // 
            // txtBoxLogPath
            // 
            this.txtBoxLogPath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtBoxLogPath.Location = new System.Drawing.Point(12, 21);
            this.txtBoxLogPath.Name = "txtBoxLogPath";
            this.txtBoxLogPath.ReadOnly = true;
            this.txtBoxLogPath.Size = new System.Drawing.Size(471, 20);
            this.txtBoxLogPath.TabIndex = 3;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 343);
            this.Controls.Add(this.gpBxLog);
            this.Controls.Add(this.gpBxconfigFiles);
            this.Controls.Add(this.gpBxConfig);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.Text = "Config and Log File Dialog";
            this.gpBxConfig.ResumeLayout(false);
            this.gpBxConfig.PerformLayout();
            this.gpBxconfigFiles.ResumeLayout(false);
            this.gpBxLog.ResumeLayout(false);
            this.gpBxLog.PerformLayout();
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
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnPreviewLoaded;
        private System.Windows.Forms.GroupBox gpBxLog;
        private System.Windows.Forms.Button btnOpenLogLoc;
        private System.Windows.Forms.TextBox txtBoxLogPath;
        private System.Windows.Forms.Button btnPrevLog;
        private System.Windows.Forms.Label label2;
    }
}