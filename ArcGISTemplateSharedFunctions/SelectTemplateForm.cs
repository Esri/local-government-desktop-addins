/*
 | Version 1.17.2018
 | Copyright 2018 Esri
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

namespace A4LGSharedFunctions
{
    public partial class SelectTemplateForm : Form
    {
        //public static Label s_lblLayer;
        //public static ComboBox s_cboSelectTemplate;
        //public static string s_selectedTemplate;
        public SelectTemplateForm()
        {            
            InitializeComponent();
            try{
            this.Text= A4LGSharedFunctions.Localizer.GetString("AAOptionDialogTemplate");
            }
            catch
            {
            }
            //s_lblLayer =  lblLayer;
            //s_cboSelectTemplate = cboSelectTemplate ;
        }
        private int list_count;
        private void SelectTemplateForm_Load(object sender, EventArgs e)
        {
            list_count = cboSelectTemplate.Items.Count;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
          //  s_selectedTemplate = s_cboSelectTemplate.Text;

            this.Close();

        }
        public void setWidth(int Width)
        {
            if (Width < 180)
            {
                this.Width = 280;
            }
            else
            {
                this.Width = Width + 100;
            }
            //cboSelectTemplate.Width = this.Width - 50;
            cboSelectTemplate.Width = this.Width - 50 - cboSelectTemplate.Left;
        }
        public void setComboType(ComboBoxStyle combSty)
        {
            cboSelectTemplate.DropDownStyle = combSty;

        }

        private void SelectTemplateForm_Resize(object sender, EventArgs e)
        {
            cboSelectTemplate.Width = this.Width - 50 - cboSelectTemplate.Left;
            comments.Width = this.Width - 50 - comments.Left;
        }

        private void cboSelectTemplate_TextChanged(object sender, EventArgs e)
        {
            if (cboSelectTemplate.DataSource == null || list_count == 0)
            {
                return;
            }
            
            if (cboSelectTemplate.SelectedIndex == -1)
            {
                btnSelect.Enabled = false;
                return;
            }
            btnSelect.Enabled = true;
        }
    }
}
