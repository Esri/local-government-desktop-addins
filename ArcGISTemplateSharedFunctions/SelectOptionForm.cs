/*
 | Version 10.4
 | Copyright 2016 Esri
 |
 | Licensed under the Apache License, Version 2.0 (the "License");
 | you may not use this file except in compliance with the License.
 | You may obtain a copy of the License at
 |
 |    http://www.apache.org/licenses/LICENSE-2.0
 |
 | Unless required by applicable law or agreed to in writing, software
 | distributed under the License is distributed on an "AS IS" BASIS,
 | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 | See the License for the specific language governing permissions and
 | limitations under the License.
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace A4LGSharedFunctions
{
    public partial class SelectOptionForm : Form
    {
        public bool Cancel = false;
        //public static ComboBox s_cboSelectTemplate;
        //public static string s_selectedTemplate;
        public SelectOptionForm()
        {
            InitializeComponent();
            try
            {
                this.Text = A4LGSharedFunctions.Localizer.GetString("AAOptionDialogOption");
            }
            catch
            { 
            }
            //s_lblLayer =  lblLayer;
            //s_cboSelectTemplate = cboSelectTemplate ;
        }

        private void SelectFeatureForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //  s_selectedTemplate = s_cboSelectTemplate.Text;


            if (cboSelectTemplate.SelectedIndex < 0)
            {
                MessageBox.Show("The text entered is not in the list, aborting");
                Cancel = true;
            }
            //if (!cboSelectTemplate.Items.Contains(cboSelectTemplate.Text))
            //{
            //    MessageBox.Show("The text entered is not in the list, aborting");
            //    Cancel = true;
            //}
            else
            { 
            
            }
            this.Close();

        }

        private void SelectFeatureForm_Load_1(object sender, EventArgs e)
        {

        }
        public void setWidth(int Width)
        {
            if (Width < 180)
            {
                this.Width = 280;
            }
            else
            {
                this.Width = Width + 50;
            }
            cboSelectTemplate.Width = this.Width -50;
        }
        public void showCancelButton()
        {
            btnCancel.Visible = true;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel = true;

            this.Close();

        }
    }
}
