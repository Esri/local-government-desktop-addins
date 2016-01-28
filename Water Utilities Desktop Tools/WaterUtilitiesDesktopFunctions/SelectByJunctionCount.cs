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
using System.Collections;

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
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.SystemUI;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    public partial class frmSelectByJunctionCount : Form
    {
        private IApplication _app;
        private IEnvelope _env;
        public frmSelectByJunctionCount(IApplication app)
        {
            InitializeComponent();
            _app = app;

        }
        public void LoadJunctionsFeatureLayers()
        {
            IMap pMap = (_app.Document as IMxDocument).FocusMap as IMap;

            // ArrayList pJuncLayers = Globals.GetGeometricNetworksJunctionsLayers(ref pMap, false);
            Hashtable pJuncLayers = Globals.GetGeometricNetworksJunctionsLayersHT(ref pMap, false);
            lstJunctionLayers.Items.Clear();
            //lstJunctionLayers.DataSource = pJuncLayers;
            //lstJunctionLayers.DisplayMember= "Name";
            //lstJunctionLayers.ValueMember = "Name";

            try
            {
                foreach (string key in pJuncLayers.Keys)
                {
                    lstJunctionLayers.Items.Add((pJuncLayers[key] as IFeatureLayer).Name);
                    //Console.WriteLine(key + '=' + pJuncLayers[key]);
                }
                lstJunctionLayers.Sorted = true;

                //IEnumerator pcol = pJuncLayers.Keys.GetEnumerator();
                //pcol.Reset();
                //pcol.MoveNext();
                //while (pcol.Current != null)
                //{
                //    lstJunctionLayers.Items.Add((pJuncLayers[pcol.Current] as IFeatureLayer).Name);
                //    pcol.MoveNext();

                //}(
                setExtent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "LoadJunctionsFeatureLayers\n" + ex.Message);
            }
            //foreach (IFeatureLayer pJL in pJuncLayers)
            //{
            //    lstJunctionLayers.Items.Add(pJL.Name);

            //}

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstJunctionLayers.Items.Count; i++)
            {

                lstJunctionLayers.SetItemCheckState(i, System.Windows.Forms.CheckState.Checked);
                //pJL.Checked = true;
            }

        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstJunctionLayers.Items.Count; i++)
            {

                lstJunctionLayers.SetItemCheckState(i, System.Windows.Forms.CheckState.Unchecked);
                //pJL.Checked = true;
            }

        }

        private void btnDrawExtent_Click(object sender, EventArgs e)
        {
            _env = null;
            if (rdoUserDefExt.Checked)
            {
                GetUserEnv();
                setExtent();
            }
            else
            {
                rdoUserDefExt.Checked = true;
            }
            // GetUserEnv();
            //setExtent();

        }

        //private void label3_Click(object sender, EventArgs e)
        //{

        //}

        //private void label7_Click(object sender, EventArgs e)
        //{

        //}

        //private void label5_Click(object sender, EventArgs e)
        //{

        //}

        //private void groupBox2_Enter(object sender, EventArgs e)
        //{

        //}

        //private void label6_Click(object sender, EventArgs e)
        //{

        //}

        private void rdoExtent_CheckedChanged(object sender, EventArgs e)
        {
            setExtent();
        }
        private void setExtent()
        {
            if (rdoCurrentExtent.Checked == true)
            {
                _env = (_app.Document as IMxDocument).ActiveView.Extent.Envelope;
            }
            else if (rdoFullExtent.Checked == true)
            {
                _env = (_app.Document as IMxDocument).ActiveView.FullExtent.Envelope;
            }
            else if (rdoUserDefExt.Checked == true)
            {
                if (_env == null)
                {
                    _env = GetUserEnv().Envelope;

                }
                else
                {

                }
            }
            lblXMax.Text = _env.XMax.ToString();
            lblXMin.Text = _env.XMin.ToString();
            lblYMax.Text = _env.YMax.ToString();
            lblYMin.Text = _env.YMin.ToString();
        }
        private IGeometry GetUserEnv()
        {
            IRubberBand rubberEnv = new RubberEnvelope();

            return rubberEnv.TrackNew((_app.Document as IMxDocument).ActiveView.ScreenDisplay, null);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
            ISpatialFilter pSpatFilt = null;
            IFeatureLayer pFL = null;
              IFeatureCursor pFCurs = null;
              IFeature pFeat = null;
              ISimpleJunctionFeature pSimpFeat = null;
            // Create an edit operation enabling undo/redo
            try
            {
                (_app.Document as IMxDocument).FocusMap.ClearSelection();


                trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();
                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = _app.hWnd;
                progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);

                stepProgressor.MinRange = 0;
                stepProgressor.MaxRange = lstJunctionLayers.Items.Count;
                stepProgressor.StepValue = 1;
                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_1");
                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = true;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_1");
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_1");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;
                progressDialog.ShowDialog();

                
                for (int i = 0; i < lstJunctionLayers.Items.Count; i++)
                {
                    bool boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_2") + lstJunctionLayers.Items[i].ToString();
                    if (lstJunctionLayers.GetItemCheckState(i) == CheckState.Checked)
                    {
                        bool FCorLayer = true;
                        pFL = (IFeatureLayer)Globals.FindLayer(_app, lstJunctionLayers.Items[i].ToString(), ref FCorLayer);
                        if (pFL != null)
                        {
                            pSpatFilt = new SpatialFilterClass();
                            pSpatFilt.Geometry = _env as IGeometry;
                            pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            pSpatFilt.GeometryField = pFL.FeatureClass.ShapeFieldName;
                            int featCnt = pFL.FeatureClass.FeatureCount(pSpatFilt);

                            if (featCnt > 0)
                            {
                                pFCurs = pFL.Search(pSpatFilt, true);
                                 pFeat = pFCurs.NextFeature();
                                int loopCnt = 1;

                                while (pFeat != null)
                                {
                                     boolean_Continue = trackCancel.Continue();
                                    if (!boolean_Continue)
                                    {
                                        return;
                                    }
                                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_3") + loopCnt + A4LGSharedFunctions.Localizer.GetString("Of") + featCnt;
                                
                                    if (pFeat is SimpleJunctionFeature)
                                    {
                                        
                                        pSimpFeat = (ISimpleJunctionFeature)pFeat;
                                        if (pSimpFeat.EdgeFeatureCount >= numMinEdge.Value && pSimpFeat.EdgeFeatureCount <= numMaxEdge.Value)
                                        {
                                            (_app.Document as IMxDocument).FocusMap.SelectFeature(pFL as ILayer, pFeat);

                                        }
                                    }
                                    loopCnt++;
                                    pFeat = pFCurs.NextFeature();
                                }
                            }
                        }
                    }
                    stepProgressor.Step();

                }

            }
            catch (Exception Ex)

            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("SltByJctCountLbl_22") + "\r\n" + Ex.Message);

            }
            finally
            {
                if (progressDialog != null)
                {
                    progressDialog.HideDialog();

                }

                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog = null;
                trackCancel = null;
                pSpatFilt = null;
                pFL = null;
                if (pFCurs != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCurs);
                }
                pFCurs = null;
                pFeat = null;
                pSimpFeat = null;
                this.Hide();
                (_app.Document as IMxDocument).ActiveView.Refresh();
                
                MessageBox.Show((_app.Document as IMxDocument).FocusMap.SelectionCount + A4LGSharedFunctions.Localizer.GetString("SltByJctCountMess_1"));

            }
        }
    }

}
