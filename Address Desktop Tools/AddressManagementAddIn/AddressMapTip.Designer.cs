namespace A4LGAddressManagement
{
    partial class AddressMapTip
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
            this.mapTipLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mapTipLabel
            // 
            this.mapTipLabel.AutoSize = true;
            this.mapTipLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapTipLabel.Location = new System.Drawing.Point(0, 0);
            this.mapTipLabel.Margin = new System.Windows.Forms.Padding(0);
            this.mapTipLabel.Name = "mapTipLabel";
            this.mapTipLabel.Padding = new System.Windows.Forms.Padding(1);
            this.mapTipLabel.Size = new System.Drawing.Size(39, 17);
            this.mapTipLabel.TabIndex = 0;
            this.mapTipLabel.Text = "label1";
            this.mapTipLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AddressMapTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(39, 17);
            this.Controls.Add(this.mapTipLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddressMapTip";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "AddressMapTip";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mapTipLabel;
    }
}