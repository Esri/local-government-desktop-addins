namespace ArcGIS4LocalGovernment
{
    partial class LastValueForm
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
            this.gpBxHeader = new System.Windows.Forms.GroupBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.gpBoxFooter = new System.Windows.Forms.GroupBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gpBoxMain = new System.Windows.Forms.GroupBox();
            this.dgLastVal = new System.Windows.Forms.DataGridView();
            this.gpBxHeader.SuspendLayout();
            this.gpBoxFooter.SuspendLayout();
            this.gpBoxMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLastVal)).BeginInit();
            this.SuspendLayout();
            // 
            // gpBxHeader
            // 
            this.gpBxHeader.Controls.Add(this.lblHeader);
            this.gpBxHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpBxHeader.Location = new System.Drawing.Point(0, 0);
            this.gpBxHeader.Name = "gpBxHeader";
            this.gpBxHeader.Size = new System.Drawing.Size(399, 41);
            this.gpBxHeader.TabIndex = 0;
            this.gpBxHeader.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.Location = new System.Drawing.Point(3, 16);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(393, 22);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Select the Last Value entry to change, use <null> for null.";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpBoxFooter
            // 
            this.gpBoxFooter.Controls.Add(this.btnOk);
            this.gpBoxFooter.Controls.Add(this.btnCancel);
            this.gpBoxFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpBoxFooter.Location = new System.Drawing.Point(0, 344);
            this.gpBoxFooter.Name = "gpBoxFooter";
            this.gpBoxFooter.Size = new System.Drawing.Size(399, 52);
            this.gpBoxFooter.TabIndex = 1;
            this.gpBoxFooter.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(231, 17);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gpBoxMain
            // 
            this.gpBoxMain.Controls.Add(this.dgLastVal);
            this.gpBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpBoxMain.Location = new System.Drawing.Point(0, 41);
            this.gpBoxMain.Name = "gpBoxMain";
            this.gpBoxMain.Size = new System.Drawing.Size(399, 303);
            this.gpBoxMain.TabIndex = 2;
            this.gpBoxMain.TabStop = false;
            // 
            // dgLastVal
            // 
            this.dgLastVal.AllowUserToAddRows = false;
            this.dgLastVal.AllowUserToDeleteRows = false;
            this.dgLastVal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLastVal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLastVal.Location = new System.Drawing.Point(3, 16);
            this.dgLastVal.Name = "dgLastVal";
            this.dgLastVal.RowHeadersVisible = false;
            this.dgLastVal.Size = new System.Drawing.Size(393, 284);
            this.dgLastVal.TabIndex = 0;
            // 
            // LastValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 396);
            this.Controls.Add(this.gpBoxMain);
            this.Controls.Add(this.gpBoxFooter);
            this.Controls.Add(this.gpBxHeader);
            this.Name = "LastValueForm";
            this.Text = "Last Value Array Entries";
            this.gpBxHeader.ResumeLayout(false);
            this.gpBoxFooter.ResumeLayout(false);
            this.gpBoxMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLastVal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpBxHeader;
        private System.Windows.Forms.GroupBox gpBoxFooter;
        private System.Windows.Forms.GroupBox gpBoxMain;
        private System.Windows.Forms.DataGridView dgLastVal;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}