namespace A4WaterUtilities
{
    partial class frmMergeGNLines
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
            this.lstMergeFeatures = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chkMergeElevationData = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lstMergeFeatures
            // 
            this.lstMergeFeatures.Location = new System.Drawing.Point(19, 34);
            this.lstMergeFeatures.MultiSelect = false;
            this.lstMergeFeatures.Name = "lstMergeFeatures";
            this.lstMergeFeatures.Size = new System.Drawing.Size(350, 202);
            this.lstMergeFeatures.TabIndex = 0;
            this.lstMergeFeatures.UseCompatibleStateImageBehavior = false;
            this.lstMergeFeatures.View = System.Windows.Forms.View.List;
            this.lstMergeFeatures.SelectedIndexChanged += new System.EventHandler(this.lstMergeFeatures_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(375, 42);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = A4LGSharedFunctions.Localizer.GetString("OK");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(375, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = A4LGSharedFunctions.Localizer.GetString("Cancel");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(291, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = A4LGSharedFunctions.Localizer.GetString("MergeSlct_1");
            // 
            // chkMergeElevationData
            // 
            this.chkMergeElevationData.AutoSize = true;
            this.chkMergeElevationData.Location = new System.Drawing.Point(31, 242);
            this.chkMergeElevationData.Name = "chkMergeElevationData";
            this.chkMergeElevationData.Size = new System.Drawing.Size(330, 17);
            this.chkMergeElevationData.TabIndex = 4;
            this.chkMergeElevationData.Text = A4LGSharedFunctions.Localizer.GetString("MergeSlct_2");
            this.chkMergeElevationData.UseVisualStyleBackColor = true;
            // 
            // frmMergeGNLines
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 282);
            this.Controls.Add(this.chkMergeElevationData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstMergeFeatures);
            this.Name = "frmMergeGNLines";
            this.Text = A4LGSharedFunctions.Localizer.GetString("MergeOprt_4");
            this.Load += new System.EventHandler(this.frmMergeGNLines_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstMergeFeatures;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkMergeElevationData;
    }
}
