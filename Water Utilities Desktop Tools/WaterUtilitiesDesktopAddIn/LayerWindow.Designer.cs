namespace A4WaterUtilities
{
    partial class LayerWindow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCount = new System.Windows.Forms.Label();
            this.splContMain = new System.Windows.Forms.SplitContainer();
            this.tbCntlDisplay = new System.Windows.Forms.TabControl();
            this.btnZoomTo = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.gpBoxOptions = new System.Windows.Forms.GroupBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.chkZoomToOnAdvance = new System.Windows.Forms.CheckBox();
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.cboLayers = new System.Windows.Forms.ComboBox();
            this.splContMain.Panel1.SuspendLayout();
            this.splContMain.Panel2.SuspendLayout();
            this.splContMain.SuspendLayout();
            this.gpBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(195, 26);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(35, 13);
            this.lblCount.TabIndex = 9;
            this.lblCount.Text = A4LGSharedFunctions.Localizer.GetString("Count");
            // 
            // splContMain
            // 
            this.splContMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splContMain.IsSplitterFixed = true;
            this.splContMain.Location = new System.Drawing.Point(0, 0);
            this.splContMain.Name = "splContMain";
            this.splContMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splContMain.Panel1
            // 
            this.splContMain.Panel1.Controls.Add(this.tbCntlDisplay);
            // 
            // splContMain.Panel2
            // 
            this.splContMain.Panel2.Controls.Add(this.btnZoomTo);
            this.splContMain.Panel2.Controls.Add(this.lblCount);
            this.splContMain.Panel2.Controls.Add(this.btnNext);
            this.splContMain.Panel2.Controls.Add(this.btnRefresh);
            this.splContMain.Panel2.Controls.Add(this.btnPrevious);
            this.splContMain.Panel2.Controls.Add(this.gpBoxOptions);
            this.splContMain.Panel2MinSize = 35;
            this.splContMain.Size = new System.Drawing.Size(481, 357);
            this.splContMain.SplitterDistance = 242;
            this.splContMain.TabIndex = 1;
            // 
            // tbCntlDisplay
            // 
            this.tbCntlDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCntlDisplay.Location = new System.Drawing.Point(0, 0);
            this.tbCntlDisplay.Name = "tbCntlDisplay";
            this.tbCntlDisplay.SelectedIndex = 0;
            this.tbCntlDisplay.Size = new System.Drawing.Size(481, 242);
            this.tbCntlDisplay.TabIndex = 0;
            // 
            // btnZoomTo
            // 
            this.btnZoomTo.Location = new System.Drawing.Point(121, 3);
            this.btnZoomTo.Name = "btnZoomTo";
            this.btnZoomTo.Size = new System.Drawing.Size(75, 23);
            this.btnZoomTo.TabIndex = 10;
            this.btnZoomTo.Text = A4LGSharedFunctions.Localizer.GetString("ZoomTo");
            this.btnZoomTo.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(295, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = ">>";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(214, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = A4LGSharedFunctions.Localizer.GetString("Refresh");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(40, 3);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 8;
            this.btnPrevious.Text = "<<";
            this.btnPrevious.UseVisualStyleBackColor = true;
            // 
            // gpBoxOptions
            // 
            this.gpBoxOptions.Controls.Add(this.Label3);
            this.gpBoxOptions.Controls.Add(this.txtScale);
            this.gpBoxOptions.Controls.Add(this.Label2);
            this.gpBoxOptions.Controls.Add(this.chkZoomToOnAdvance);
            this.gpBoxOptions.Controls.Add(this.txtQuery);
            this.gpBoxOptions.Controls.Add(this.Label1);
            this.gpBoxOptions.Controls.Add(this.cboLayers);
            this.gpBoxOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gpBoxOptions.Location = new System.Drawing.Point(0, 24);
            this.gpBoxOptions.Name = "gpBoxOptions";
            this.gpBoxOptions.Size = new System.Drawing.Size(481, 87);
            this.gpBoxOptions.TabIndex = 1;
            this.gpBoxOptions.TabStop = false;
            this.gpBoxOptions.Text = A4LGSharedFunctions.Localizer.GetString("Options");
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(216, 53);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(37, 13);
            this.Label3.TabIndex = 7;
            this.Label3.Text = A4LGSharedFunctions.Localizer.GetString("Scale");
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(259, 49);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(100, 20);
            this.txtScale.TabIndex = 6;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(6, 53);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(38, 13);
            this.Label2.TabIndex = 5;
            this.Label2.Text = A4LGSharedFunctions.Localizer.GetString("Query");
            // 
            // chkZoomToOnAdvance
            // 
            this.chkZoomToOnAdvance.AutoSize = true;
            this.chkZoomToOnAdvance.Location = new System.Drawing.Point(219, 20);
            this.chkZoomToOnAdvance.Name = "chkZoomToOnAdvance";
            this.chkZoomToOnAdvance.Size = new System.Drawing.Size(140, 17);
            this.chkZoomToOnAdvance.TabIndex = 4;
            this.chkZoomToOnAdvance.Text = A4LGSharedFunctions.Localizer.GetString("ZoomOn");
            this.chkZoomToOnAdvance.UseVisualStyleBackColor = true;
            // 
            // txtQuery
            // 
            this.txtQuery.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtQuery.Location = new System.Drawing.Point(77, 49);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.ReadOnly = true;
            this.txtQuery.Size = new System.Drawing.Size(121, 20);
            this.txtQuery.TabIndex = 3;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(6, 22);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(69, 13);
            this.Label1.TabIndex = 2;
            this.Label1.Text = A4LGSharedFunctions.Localizer.GetString("ActiveLayer");
            // 
            // cboLayers
            // 
            this.cboLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayers.FormattingEnabled = true;
            this.cboLayers.Location = new System.Drawing.Point(77, 18);
            this.cboLayers.Name = "cboLayers";
            this.cboLayers.Size = new System.Drawing.Size(121, 21);
            this.cboLayers.TabIndex = 0;
            // 
            // LayerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splContMain);
            this.Name = "LayerWindow";
            this.Size = new System.Drawing.Size(481, 357);
            this.splContMain.Panel1.ResumeLayout(false);
            this.splContMain.Panel2.ResumeLayout(false);
            this.splContMain.Panel2.PerformLayout();
            this.splContMain.ResumeLayout(false);
            this.gpBoxOptions.ResumeLayout(false);
            this.gpBoxOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label lblCount;
        internal System.Windows.Forms.SplitContainer splContMain;
        internal System.Windows.Forms.TabControl tbCntlDisplay;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnRefresh;
        internal System.Windows.Forms.Button btnPrevious;
        internal System.Windows.Forms.GroupBox gpBoxOptions;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtScale;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.CheckBox chkZoomToOnAdvance;
        internal System.Windows.Forms.TextBox txtQuery;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.ComboBox cboLayers;
        internal System.Windows.Forms.Button btnZoomTo;

    }
}
