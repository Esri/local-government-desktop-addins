﻿/*
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

using System.Diagnostics;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
using System.IO;
using ESRI.ArcGIS.Analyst3D;

namespace A4LGSharedFunctions
{
    public partial class ConfigFormNoLog : Form
    {
        ConfigEntries m_LoadedConfig;
        public ConfigFormNoLog()
        {
            InitializeComponent();

            try
            {
                this.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgText");

                this.btnLoadConfig.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgLoad");
                this.btnOpenConfigLoc.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgOpenLoc");
                this.label1.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgFilePath");
                this.btnPreviewLoaded.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgPreview");
                this.btnPreview.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgPreview");
                this.lblConfig.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgLoadedConfig");
                this.gpBxconfigFiles.Text = A4LGSharedFunctions.Localizer.GetString("ConfigDlgConfigFiles");
             
            }
            catch
            { }
            initForm();

        }
        private string configType;
        private void initForm()
        {
            
            txtBxPath.Text = ConfigUtil.generateUserCachePath();

            List<ConfigEntries> ConfigNames = ConfigUtil.GetAllConfigFilesNames(true);

            foreach (ConfigEntries pConEn in ConfigNames)
            {
                if (pConEn.Loaded == true)
                {
                    m_LoadedConfig = pConEn;
                    txtBxLoadedConfig.Text = pConEn.Name;
                    ConfigNames.Remove(pConEn);
                    break;
                }
            }

            cboConfigs.DataSource = ConfigNames;
            cboConfigs.DisplayMember = "Name";

            
        }
        private void btnOpenLoc_Click(object sender, EventArgs e)
        {



            string myDocspath = txtBxPath.Text;

            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = myDocspath;
            prc.Start();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (cboConfigs.Items.Count == 0) return;

            ProcessStartInfo psi = new ProcessStartInfo(((ConfigEntries)cboConfigs.SelectedItem).FullName);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboConfigs.Items.Count == 0) return;

                ConfigUtil.ChangeConfig(m_LoadedConfig, ((ConfigEntries)cboConfigs.SelectedItem));

                MessageBox.Show("Config file has been changed");
            }
            catch (Exception ex)
            {
                MessageBox.Show("btnLoadConfig\n" + ex.ToString());

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string myDocspath = m_LoadedConfig.FullName;


            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = myDocspath;
            prc.Start();
        }

        private void ConfigFormNoLog_Load(object sender, EventArgs e)
        {

        }

      

    }
}
