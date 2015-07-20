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
            resources.ApplyResources(this.btnOpenConfigLoc, "btnOpenConfigLoc");
            this.btnOpenConfigLoc.Name = "btnOpenConfigLoc";
            this.btnOpenConfigLoc.UseVisualStyleBackColor = true;
            this.btnOpenConfigLoc.Click += new System.EventHandler(this.btnOpenLoc_Click);
            // 
            // txtBxPath
            // 
            this.txtBxPath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.txtBxPath, "txtBxPath");
            this.txtBxPath.Name = "txtBxPath";
            this.txtBxPath.ReadOnly = true;
            // 
            // cboConfigs
            // 
            this.cboConfigs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConfigs.FormattingEnabled = true;
            resources.ApplyResources(this.cboConfigs, "cboConfigs");
            this.cboConfigs.Name = "cboConfigs";
            // 
            // btnLoadConfig
            // 
            resources.ApplyResources(this.btnLoadConfig, "btnLoadConfig");
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            resources.ApplyResources(this.gpBxConfig, "gpBxConfig");
            this.gpBxConfig.Name = "gpBxConfig";
            this.gpBxConfig.TabStop = false;
            // 
            // btnPreviewLoaded
            // 
            resources.ApplyResources(this.btnPreviewLoaded, "btnPreviewLoaded");
            this.btnPreviewLoaded.Name = "btnPreviewLoaded";
            this.btnPreviewLoaded.UseVisualStyleBackColor = true;
            this.btnPreviewLoaded.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnReload
            // 
            resources.ApplyResources(this.btnReload, "btnReload");
            this.btnReload.Name = "btnReload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lblConfig
            // 
            resources.ApplyResources(this.lblConfig, "lblConfig");
            this.lblConfig.Name = "lblConfig";
            // 
            // txtBxLoadedConfig
            // 
            this.txtBxLoadedConfig.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.txtBxLoadedConfig, "txtBxLoadedConfig");
            this.txtBxLoadedConfig.Name = "txtBxLoadedConfig";
            this.txtBxLoadedConfig.ReadOnly = true;
            // 
            // gpBxconfigFiles
            // 
            this.gpBxconfigFiles.Controls.Add(this.btnPreview);
            this.gpBxconfigFiles.Controls.Add(this.btnLoadConfig);
            this.gpBxconfigFiles.Controls.Add(this.cboConfigs);
            resources.ApplyResources(this.gpBxconfigFiles, "gpBxconfigFiles");
            this.gpBxconfigFiles.Name = "gpBxconfigFiles";
            this.gpBxconfigFiles.TabStop = false;
            // 
            // btnPreview
            // 
            resources.ApplyResources(this.btnPreview, "btnPreview");
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // gpBxLog
            // 
            this.gpBxLog.Controls.Add(this.label2);
            this.gpBxLog.Controls.Add(this.btnPrevLog);
            this.gpBxLog.Controls.Add(this.btnOpenLogLoc);
            this.gpBxLog.Controls.Add(this.txtBoxLogPath);
            resources.ApplyResources(this.gpBxLog, "gpBxLog");
            this.gpBxLog.Name = "gpBxLog";
            this.gpBxLog.TabStop = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnPrevLog
            // 
            resources.ApplyResources(this.btnPrevLog, "btnPrevLog");
            this.btnPrevLog.Name = "btnPrevLog";
            this.btnPrevLog.UseVisualStyleBackColor = true;
            this.btnPrevLog.Click += new System.EventHandler(this.btnPrevLog_Click);
            // 
            // btnOpenLogLoc
            // 
            resources.ApplyResources(this.btnOpenLogLoc, "btnOpenLogLoc");
            this.btnOpenLogLoc.Name = "btnOpenLogLoc";
            this.btnOpenLogLoc.UseVisualStyleBackColor = true;
            this.btnOpenLogLoc.Click += new System.EventHandler(this.btnOpenLogLoc_Click);
            // 
            // txtBoxLogPath
            // 
            this.txtBoxLogPath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.txtBoxLogPath, "txtBoxLogPath");
            this.txtBoxLogPath.Name = "txtBoxLogPath";
            this.txtBoxLogPath.ReadOnly = true;
            // 
            // ConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpBxLog);
            this.Controls.Add(this.gpBxconfigFiles);
            this.Controls.Add(this.gpBxConfig);
            this.Name = "ConfigForm";
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