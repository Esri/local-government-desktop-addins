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

namespace ArcGIS4LocalGovernment
{
    public partial class LastValueForm : Form
    {
        public LastValueForm()
        {
            InitializeComponent();
            lblHeader.Text = A4LGSharedFunctions.Localizer.GetString("lstValueFormText");
            this.Text = A4LGSharedFunctions.Localizer.GetString("lstValueFormTitle");
            btnOk.Text = A4LGSharedFunctions.Localizer.GetString("OK");
            btnCancel.Text = A4LGSharedFunctions.Localizer.GetString("Cancel");
        }
        public void setDataTable(DataTable dt)
        {
            dt.Columns["Changed"].ColumnMapping = MappingType.Hidden;
            dgLastVal.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgLastVal.CellEndEdit += dataGridView1_CellEndEdit;


            dgLastVal.DataSource = dt;

        }
        void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
               // string str = ((DataTable)dgLastVal.DataSource).Rows[e.RowIndex][1].ToString();
 
                ((DataTable)dgLastVal.DataSource).Rows[e.RowIndex][2] = "T";
               // str = ((DataTable)dgLastVal.DataSource).Rows[e.RowIndex][2].ToString();
              //  str = str;
                //dgLastVal.Rows[0].Cells[1].Value = "T";
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
          

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            btnOk.Focus();

          
        }
        public DataTable getDataTable()
        {
            return (DataTable)dgLastVal.DataSource;

        }

    }

}
