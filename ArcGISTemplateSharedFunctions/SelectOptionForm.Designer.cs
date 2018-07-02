namespace A4LGSharedFunctions
{
    partial class SelectOptionForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.comments = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(22, 25);
            this.lblLayer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(51, 20);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "label1";
            // 
            // cboSelectTemplate
            // 
            this.cboSelectTemplate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboSelectTemplate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSelectTemplate.FormattingEnabled = true;
            this.cboSelectTemplate.Location = new System.Drawing.Point(22, 62);
            this.cboSelectTemplate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboSelectTemplate.Name = "cboSelectTemplate";
            this.cboSelectTemplate.Size = new System.Drawing.Size(1078, 28);
            this.cboSelectTemplate.TabIndex = 1;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(27, 103);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(159, 35);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(27, 148);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(159, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comments
            // 
            this.comments.Location = new System.Drawing.Point(193, 103);
            this.comments.Name = "comments";
            this.comments.Size = new System.Drawing.Size(907, 80);
            this.comments.TabIndex = 4;
            // 
            // SelectOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1353, 295);
            this.ControlBox = false;
            this.Controls.Add(this.comments);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.cboSelectTemplate);
            this.Controls.Add(this.lblLayer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectOptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Option";
            this.Load += new System.EventHandler(this.SelectFeatureForm_Load_1);
            this.Resize += new System.EventHandler(this.SelectOptionForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblLayer;
        public  System.Windows.Forms.ComboBox cboSelectTemplate;
        public System.Windows.Forms.Button btnSelect;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label comments;
    }
}