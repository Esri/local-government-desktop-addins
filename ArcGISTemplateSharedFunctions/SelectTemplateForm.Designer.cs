namespace A4LGSharedFunctions
{
    partial class SelectTemplateForm
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
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboSelectTemplate = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(15, 16);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(35, 13);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "label1";
            // 
            // cboSelectTemplate
            // 
            this.cboSelectTemplate.FormattingEnabled = true;
            this.cboSelectTemplate.Location = new System.Drawing.Point(15, 40);
            this.cboSelectTemplate.Name = "cboSelectTemplate";
            this.cboSelectTemplate.Size = new System.Drawing.Size(277, 21);
            this.cboSelectTemplate.TabIndex = 1;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(15, 67);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // SelectTemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 102);
            this.ControlBox = false;
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.cboSelectTemplate);
            this.Controls.Add(this.lblLayer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTemplateForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Template";
            this.Load += new System.EventHandler(this.SelectTemplateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblLayer;
        public  System.Windows.Forms.ComboBox cboSelectTemplate;
        public System.Windows.Forms.Button btnSelect;
    }
}