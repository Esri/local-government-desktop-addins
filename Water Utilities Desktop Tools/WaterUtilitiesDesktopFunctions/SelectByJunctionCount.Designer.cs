namespace A4WaterUtilities
{
    partial class frmSelectByJunctionCount
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
            this.gpBoxLayers = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.lstJunctionLayers = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblYMin = new System.Windows.Forms.Label();
            this.lblYMax = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblXMin = new System.Windows.Forms.Label();
            this.lblXMax = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDrawExtent = new System.Windows.Forms.Button();
            this.rdoUserDefExt = new System.Windows.Forms.RadioButton();
            this.rdoCurrentExtent = new System.Windows.Forms.RadioButton();
            this.rdoFullExtent = new System.Windows.Forms.RadioButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numMaxEdge = new System.Windows.Forms.NumericUpDown();
            this.numMinEdge = new System.Windows.Forms.NumericUpDown();
            this.gpBoxLayers.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxEdge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinEdge)).BeginInit();
            this.SuspendLayout();
            // 
            // gpBoxLayers
            // 
            this.gpBoxLayers.Controls.Add(this.btnSelectAll);
            this.gpBoxLayers.Controls.Add(this.btnDeselectAll);
            this.gpBoxLayers.Controls.Add(this.lstJunctionLayers);
            this.gpBoxLayers.Location = new System.Drawing.Point(3, 12);
            this.gpBoxLayers.Name = "gpBoxLayers";
            this.gpBoxLayers.Size = new System.Drawing.Size(424, 183);
            this.gpBoxLayers.TabIndex = 0;
            this.gpBoxLayers.TabStop = false;
            this.gpBoxLayers.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_1");
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(30, 150);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_2");
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Location = new System.Drawing.Point(111, 150);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnDeselectAll.TabIndex = 1;
            this.btnDeselectAll.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_3");
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // lstJunctionLayers
            // 
            this.lstJunctionLayers.FormattingEnabled = true;
            this.lstJunctionLayers.Location = new System.Drawing.Point(9, 19);
            this.lstJunctionLayers.Name = "lstJunctionLayers";
            this.lstJunctionLayers.Size = new System.Drawing.Size(409, 124);
            this.lstJunctionLayers.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblYMin);
            this.groupBox2.Controls.Add(this.lblYMax);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.lblXMin);
            this.groupBox2.Controls.Add(this.lblXMax);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnDrawExtent);
            this.groupBox2.Controls.Add(this.rdoUserDefExt);
            this.groupBox2.Controls.Add(this.rdoCurrentExtent);
            this.groupBox2.Controls.Add(this.rdoFullExtent);
            this.groupBox2.Location = new System.Drawing.Point(3, 262);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(424, 124);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_4");
           // this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // lblYMin
            // 
            this.lblYMin.AutoSize = true;
            this.lblYMin.Location = new System.Drawing.Point(191, 97);
            this.lblYMin.Name = "lblYMin";
            this.lblYMin.Size = new System.Drawing.Size(41, 13);
            this.lblYMin.TabIndex = 11;
            this.lblYMin.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_5");
            // 
            // lblYMax
            // 
            this.lblYMax.AutoSize = true;
            this.lblYMax.Location = new System.Drawing.Point(191, 52);
            this.lblYMax.Name = "lblYMax";
            this.lblYMax.Size = new System.Drawing.Size(35, 13);
            this.lblYMax.TabIndex = 10;
            this.lblYMax.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_6");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(154, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_7");
            // 
            // lblXMin
            // 
            this.lblXMin.AutoSize = true;
            this.lblXMin.Location = new System.Drawing.Point(54, 74);
            this.lblXMin.Name = "lblXMin";
            this.lblXMin.Size = new System.Drawing.Size(35, 13);
            this.lblXMin.TabIndex = 8;
            this.lblXMin.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_8");
            //this.lblXMin.Click += new System.EventHandler(this.label7_Click);
            // 
            // lblXMax
            // 
            this.lblXMax.AutoSize = true;
            this.lblXMax.Location = new System.Drawing.Point(268, 74);
            this.lblXMax.Name = "lblXMax";
            this.lblXMax.Size = new System.Drawing.Size(35, 13);
            this.lblXMax.TabIndex = 7;
            this.lblXMax.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_9");
           // this.lblXMax.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(229, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_10");
           // this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_11");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_12");
            //this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // btnDrawExtent
            // 
            this.btnDrawExtent.Location = new System.Drawing.Point(332, 21);
            this.btnDrawExtent.Name = "btnDrawExtent";
            this.btnDrawExtent.Size = new System.Drawing.Size(26, 23);
            this.btnDrawExtent.TabIndex = 3;
            this.btnDrawExtent.UseVisualStyleBackColor = true;
            this.btnDrawExtent.Visible = false;
            this.btnDrawExtent.Click += new System.EventHandler(this.btnDrawExtent_Click);
            // 
            // rdoUserDefExt
            // 
            this.rdoUserDefExt.AutoSize = true;
            this.rdoUserDefExt.Location = new System.Drawing.Point(232, 24);
            this.rdoUserDefExt.Name = "rdoUserDefExt";
            this.rdoUserDefExt.Size = new System.Drawing.Size(87, 17);
            this.rdoUserDefExt.TabIndex = 2;
            this.rdoUserDefExt.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_13");
            this.rdoUserDefExt.UseVisualStyleBackColor = true;
            this.rdoUserDefExt.Visible = false;
            this.rdoUserDefExt.CheckedChanged += new System.EventHandler(this.rdoExtent_CheckedChanged);
            // 
            // rdoCurrentExtent
            // 
            this.rdoCurrentExtent.AutoSize = true;
            this.rdoCurrentExtent.Checked = true;
            this.rdoCurrentExtent.Location = new System.Drawing.Point(31, 24);
            this.rdoCurrentExtent.Name = "rdoCurrentExtent";
            this.rdoCurrentExtent.Size = new System.Drawing.Size(92, 17);
            this.rdoCurrentExtent.TabIndex = 1;
            this.rdoCurrentExtent.TabStop = true;
            this.rdoCurrentExtent.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_14");
            this.rdoCurrentExtent.UseVisualStyleBackColor = true;
            this.rdoCurrentExtent.CheckedChanged += new System.EventHandler(this.rdoExtent_CheckedChanged);
            // 
            // rdoFullExtent
            // 
            this.rdoFullExtent.AutoSize = true;
            this.rdoFullExtent.Location = new System.Drawing.Point(138, 24);
            this.rdoFullExtent.Name = "rdoFullExtent";
            this.rdoFullExtent.Size = new System.Drawing.Size(74, 17);
            this.rdoFullExtent.TabIndex = 0;
            this.rdoFullExtent.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_15");
            this.rdoFullExtent.UseVisualStyleBackColor = true;
            this.rdoFullExtent.CheckedChanged += new System.EventHandler(this.rdoExtent_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(325, 392);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_16");
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(235, 392);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_17");
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numMaxEdge);
            this.groupBox1.Controls.Add(this.numMinEdge);
            this.groupBox1.Location = new System.Drawing.Point(3, 201);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 55);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_18");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(209, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_19");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_20");
            // 
            // numMaxEdge
            // 
            this.numMaxEdge.Location = new System.Drawing.Point(329, 20);
            this.numMaxEdge.Name = "numMaxEdge";
            this.numMaxEdge.Size = new System.Drawing.Size(68, 20);
            this.numMaxEdge.TabIndex = 1;
            this.numMaxEdge.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // numMinEdge
            // 
            this.numMinEdge.Location = new System.Drawing.Point(131, 20);
            this.numMinEdge.Name = "numMinEdge";
            this.numMinEdge.Size = new System.Drawing.Size(60, 20);
            this.numMinEdge.TabIndex = 0;
            this.numMinEdge.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // frmSelectByJunctionCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 426);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gpBoxLayers);
            this.Name = "frmSelectByJunctionCount";
            this.Text = A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_21");
            this.gpBoxLayers.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxEdge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinEdge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpBoxLayers;
        private System.Windows.Forms.CheckedListBox lstJunctionLayers;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDrawExtent;
        private System.Windows.Forms.RadioButton rdoUserDefExt;
        private System.Windows.Forms.RadioButton rdoCurrentExtent;
        private System.Windows.Forms.RadioButton rdoFullExtent;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numMaxEdge;
        private System.Windows.Forms.NumericUpDown numMinEdge;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblYMin;
        private System.Windows.Forms.Label lblYMax;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblXMin;
        private System.Windows.Forms.Label lblXMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}
