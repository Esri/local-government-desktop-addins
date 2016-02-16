/*
 | Version 10.4
 | Copyright 2014 Esri
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
using System.Collections;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    public class BarClassIDS
    {
        public int ClassID { get; set; }

        public int[] IDs { get; set; }
    }
    public class FeatureOIDs
    {
        public int ClassID { get; set; }

        public int ID { get; set; }
    }
    public class mainDetails
    {
        public mainDetails() { }
        public string MainID { get; set; }
        public double UpElev { get; set; }
        public double DownElev { get; set; }
        public double UpM { get; set; }
        public double DownM { get; set; }
        public string Label { get; set; }

    }
    public class manholeDetails
    {
        public manholeDetails() { }
        public string ManholeID { get; set; }
        //public double Invert { get; set; }
        //public double Rim { get; set; }
        //public double InvertElev { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }

        public double M { get; set; }


    }
    public class tapDetails
    {
        public tapDetails() { }

        public string tapID { get; set; }
        public string tapLabel { get; set; }

        public double M { get; set; }
        public double Elevation { get; set; }
        public bool Added { get; set; }
    }
    public static class GeoNetTools
    {

        public enum GNTraceType { Upstream, Downstream, Isolation, SummaryIsolation, SecondaryIsolation, NewIsolation, IsolationRerun }
        private static IFields createProfileFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 8;

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "X";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(0, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "Y";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(1, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "Z";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_1");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "M";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 10;
            pFieldEdit.Scale_2 = 1;

            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(3, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "DOWNELEV";
            pFieldEdit.IsNullable_2 = true;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_2");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(4, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "UPELEV";
            pFieldEdit.IsNullable_2 = true;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_3");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(5, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "TOPELEV";
            pFieldEdit.IsNullable_2 = true;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_4");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(6, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "BOTELEV";
            pFieldEdit.IsNullable_2 = true;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_5");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(7, pField);
            return pFields;
        }
        private static IFields createProfilePointFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 4;

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "X";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(0, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "TOPELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_6");
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(1, pField);

            //pField = new FieldClass();
            //pFieldEdit = (IFieldEdit)pField;

            //pFieldEdit.Name_2 = "INVERT";
            //pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            //pFieldEdit.Precision_2 = 20;
            //pFieldEdit.Scale_2 = 10;
            //pFieldEdit.AliasName_2 = "Invert";
            //pFieldEdit.IsNullable_2 = false;

            //pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "BOTELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_7");
            pFieldEdit.Scale_2 = 1;

            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "ID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_8");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(3, pField);
            return pFields;
        }
        private static IFields createPointFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 4;

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "X";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(0, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "TOPELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_6");
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(1, pField);

            //pField = new FieldClass();
            //pFieldEdit = (IFieldEdit)pField;

            //pFieldEdit.Name_2 = "INVERT";
            //pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            //pFieldEdit.Precision_2 = 20;
            //pFieldEdit.Scale_2 = 10;
            //pFieldEdit.AliasName_2 = "Invert";
            //pFieldEdit.IsNullable_2 = false;

            //pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "BOTELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_7");
            pFieldEdit.Scale_2 = 1;

            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "ID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_8");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(3, pField);
            return pFields;
        }

        private static IFields createLineLabelFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 4;



            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "FACILITYID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_11");
            pFieldEdit.IsNullable_2 = true;
            pFieldsEdit.set_Field(0, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "MEASURE";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_12");
            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(1, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "ELEVATION";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_13");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(2, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "LABEL";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.Length_2 = 150;

            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_14");
            pFieldEdit.IsNullable_2 = true;
            pFieldsEdit.set_Field(3, pField);


            return pFields;
        }
        private static IFields createLineFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 3;



            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "FACILITYID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_11");
            pFieldEdit.IsNullable_2 = true;
            pFieldsEdit.set_Field(0, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "MEASURE";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_12");
            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(1, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "ELEVATION";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_13");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(2, pField);


            return pFields;
        }
        private static IFields createLine2Fields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 5;



            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "FACILITYID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_11");
            pFieldEdit.IsNullable_2 = true;
            pFieldsEdit.set_Field(0, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "FROMM";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_15");
            pFieldEdit.IsNullable_2 = false;
            pFieldsEdit.set_Field(1, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "TOM";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 10;
            pFieldEdit.Scale_2 = 1;
            pFieldEdit.IsNullable_2 = false;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_16");
            pFieldsEdit.set_Field(2, pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "FROMELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_17");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(3, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;

            pFieldEdit.Name_2 = "TOELEV";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_18");
            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(4, pField);

            return pFields;
        }
        private static IFields createSurfaceFields()
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;


            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            pFieldsEdit.FieldCount_2 = 2;

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "X";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.IsNullable_2 = false;


            pFieldsEdit.set_Field(0, pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "Y";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            pFieldEdit.Precision_2 = 20;
            pFieldEdit.Scale_2 = 8;
            pFieldEdit.AliasName_2 = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_1");

            pFieldEdit.IsNullable_2 = false;

            pFieldsEdit.set_Field(1, pField);

            return pFields;
        }

        public static void batchLoadBarriers(IApplication app)
        {

            bool fndAsLayer = false;
            List<string> strFiles = new List<string>();

            IEnumLayer pLays = null;
            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;
            IFeatureLayer pFl = null;
            IFeatureCursor featureCursor = null;

            IFeature feature = null;
            IFeatureLayerDefinition2 pFLD = null;
            IQueryFilter pQF = null;
            ILayer pLay = null;
            bool boolCont = true;
            int featCount = 0;
            try
            {
                double seTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
                pLays = Globals.GetLayers(app, "VECTOR");
                if (pLays != null)
                {
                    pLay = pLays.Next();

                    while (pLay != null)
                    {
                        if (pLay is IFeatureLayer)
                        {
                            if (((IFeatureLayer)pLay).FeatureClass != null)
                            {
                                if (((IFeatureLayer)pLay).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                                {
                                    strFiles.Add(pLay.Name);

                                }
                            }

                        }
                        pLay = pLays.Next();
                    }
                    //MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_1") + "\n" + ex.Message, ex.Source);
                    string strRetVal = Globals.showOptionsForm(strFiles, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsBatchBarrier"), ComboBoxStyle.DropDownList);
                    if (strRetVal != null && strRetVal != "||Cancelled||")
                    {
                        pFl = (IFeatureLayer)Globals.FindLayer(app, strRetVal, ref fndAsLayer);
                        if (pFl == null)
                        {

                            MessageBox.Show(strRetVal + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorMess_14bb"));
                            return;
                        }
                        if (pFl.FeatureClass == null)
                        {
                            MessageBox.Show(strRetVal + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18d"));
                            return;
                        }
                        pFLD = (IFeatureLayerDefinition2)pFl;


                        // Create a CancelTracker  
                        pTrkCan = new CancelTrackerClass();
                        // Create the ProgressDialog. This automatically displays the dialog  
                        pProDFact = new ProgressDialogFactoryClass();
                        pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                        // Set the properties of the ProgressDialog  
                        pProDlg.CancelEnabled = true;


                        pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                        // Set the properties of the Step Progressor  
                        pStepPro = (IStepProgressor)pProDlg;

                        pStepPro.MinRange = 0;
                        pQF = new QueryFilterClass();
                        if (pFLD.DefinitionExpression.Trim() == "")
                        {

                            featCount = pFl.FeatureClass.FeatureCount(null);
                        }
                        else
                        {
                            pQF.WhereClause = pFLD.DefinitionExpression;
                            featCount = pFl.FeatureClass.FeatureCount(pQF);
                        }
                        if (featCount == 0) { return; }

                        pStepPro.MaxRange = featCount;
                        pStepPro.StepValue = 1;
                        pStepPro.Position = 0;
                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_6");

                        featureCursor = pFl.Search(null, false);
                        feature = featureCursor.NextFeature();
                        while (feature != null)
                        {
                            if (!boolCont)
                            {

                                pStepPro.Hide();
                                pProDlg.HideDialog();
                                pStepPro = null;
                                pProDlg = null;
                                pProDFact = null;
                                return;
                            }

                            if (A4WaterUtilities.GeoNetTools.AddBarrier(feature.Shape as IPoint, app, seTol, false) == false)
                            {
                                return;
                            }
                            feature = featureCursor.NextFeature();
                            pStepPro.Step();
                            boolCont = pTrkCan.Continue();


                        }
                        if (featureCursor != null)
                        {
                            Marshal.ReleaseComObject(featureCursor);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
            finally
            {
                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;
                pTrkCan = null;

                pLays = null;
                pFl = null;
                featureCursor = null;

                feature = null;
                pFLD = null;
                pQF = null;
                pLay = null;
            }

        }
        public static void ShowArrows(IApplication app)
        {
            INetworkAnalysisExt netExt = null;
            UID pUID = null;
            IMxDocument pMxDoc = null;
            IMap pMap = null;
            IActiveView av = null;
            List<IGeometricNetwork> gnList = null;
            IGeometricNetwork gn = null;
            IUtilityNetworkAnalysisExt unetExt = null;
            IUtilityNetworkAnalysisExtFlow unetExtArr = null;
            try
            {

                //Get NA Extension in order to update the current network with the first visible network

                pUID = new UIDClass();
                pUID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                netExt = app.FindExtensionByCLSID(pUID) as INetworkAnalysisExt;

                pMxDoc = app.Document as IMxDocument;
                pMap = pMxDoc.FocusMap;
                av = pMap as IActiveView;
                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref pMap);

                if (gnList.Count > 0 && netExt != null)
                {
                    try
                    {
                        unetExt = netExt as IUtilityNetworkAnalysisExt;
                        if (gnList.Contains(netExt.CurrentNetwork))
                        {
                            //Get first visible geometric network
                            gn = netExt.CurrentNetwork;


                            if (gn != null && unetExt != null)
                            {
                                //netExt.CurrentNetwork = gn;
                                unetExt = netExt as IUtilityNetworkAnalysisExt;
                                unetExtArr = unetExt as IUtilityNetworkAnalysisExtFlow;
                                unetExtArr.ShowFlow = !(unetExtArr.ShowFlow);
                                av.Refresh();

                            }
                        }
                        else
                        {
                            //Get first visible geometric network
                            gn = gnList[0] as IGeometricNetwork;

                            if (gn != null && unetExt != null)
                            {
                                netExt.CurrentNetwork = gn;
                                unetExt = netExt as IUtilityNetworkAnalysisExt;
                                unetExtArr = unetExt as IUtilityNetworkAnalysisExtFlow;
                                unetExtArr.ShowFlow = !(unetExtArr.ShowFlow);
                                av.Refresh();

                            }
                        }
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_1") + "\n" + ex.Message, ex.Source);
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_1") + "\n" + ex.Message, ex.Source);
                return;
            }
            finally
            {

                netExt = null;
                pUID = null;
                pMxDoc = null;
                pMap = null;
                av = null;
                gnList = null;
                gn = null;
                unetExt = null;
                unetExtArr = null;
            }
        }

        public static void EstablishFlow(Globals.GNFlowDirection flowDirection, IApplication app)
        {
            IProgressDialog2 progressDialog = default(IProgressDialog2);
            IProgressDialogFactory progressDialogFactory = null;
            IEditor editor = null;
            IEditLayers eLayers = null;
            IMouseCursor appCursor = null;
            INetworkAnalysisExt netExt = null;
            UID pUID = null;
            IMap pMap = null;
            List<IGeometricNetwork> gnList = null;
            ITrackCancel trackCancel = null;
            Int32 int32_hWnd;
            IStepProgressor stepProgressor = null;
            IMxDocument mxdoc = null;
            try
            {




                int calcCount = 0;

                //Get editor

                editor = Globals.getEditor(ref app);

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }

                eLayers = editor as IEditLayers;

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);

                ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                statusBar.set_Message(0, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsWait_1"));

                //Get NA Extension in order to update the current network with the first visible network
                pUID = new UIDClass();
                pUID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                netExt = app.FindExtensionByCLSID(pUID) as INetworkAnalysisExt;

                //Get Visible geometric networks
                pMap = editor.Map;
                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref pMap);


                if (gnList.Count > 0)
                {


                    //ProgressBar
                    progressDialogFactory = new ProgressDialogFactoryClass();

                    // Create a CancelTracker
                    trackCancel = new CancelTrackerClass();

                    // Set the properties of the Step Progressor
                    int32_hWnd = app.hWnd;
                    stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = gnList.Count;
                    stepProgressor.StepValue = 1;
                    stepProgressor.Message = "";
                    stepProgressor.Hide();

                    // Create the ProgressDialog. This automatically displays the dialog
                    progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                    // Set the properties of the ProgressDialog
                    progressDialog.CancelEnabled = false;
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_1");
                    progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_1");
                    progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral;

                    bool editStarted = false;
                    try
                    {// Create an edit operation enabling undo/redo
                        editor.StartOperation();
                        editStarted = true;
                    }
                    catch
                    {
                        editStarted = false;
                    }
                    IGeometricNetwork gn = null;
                    IEnumFeatureClass enumFC = null;
                    INetwork net = null;
                    IUtilityNetworkGEN unet = null;
                    IEnumNetEID edgeEIDs = null;
                    //IFeatureLayer fLayer = null;
                    try
                    {

                        for (int i = 0; i < gnList.Count; i++)
                        {

                            gn = gnList[i] as IGeometricNetwork;
                            // fLayer = Globals.FindLayerByFeatureClass(pMap, gn.OrphanJunctionFeatureClass, false);
                            //if (fLayer == null)
                            //{
                            //    MessageBox.Show("Unable to set flow direction for " + gn.FeatureDataset.Name + ".  Add the " + gn.OrphanJunctionFeatureClass.AliasName + " to your map and try again, if needed", "Establish Flow Direction");
                            //    stepProgressor.Step();
                            //    continue;
                            //}
                            //if (!eLayers.IsEditable(fLayer))
                            //{
                            //    MessageBox.Show("Unable to set flow direction for " + gn.FeatureDataset.Name + ".  It is visible but not editable.", "Establish Flow Direction");
                            //    stepProgressor.Step();
                            //    continue;
                            //}
                            stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_1");// +fLayer.Name;
                            //Establish flow using AncillaryRole values
                            if (flowDirection == Globals.GNFlowDirection.AncillaryRole)
                            {
                                if (editor.EditWorkspace.Equals(gn.FeatureDataset.Workspace))
                                {
                                    enumFC = gn.get_ClassesByNetworkAncillaryRole(esriNetworkClassAncillaryRole.esriNCARSourceSink);
                                    if (enumFC.Next() == null)
                                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_1a") + gn.FeatureDataset.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_1b") + Environment.NewLine +
                                                        A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_1c"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_2"));
                                    else
                                    {
                                        gn.EstablishFlowDirection();
                                        calcCount += 1;
                                    }
                                }

                            }

                            //Establish flow direction based on digitized direction.
                            else
                            {
                                net = gn.Network;
                                if (editor.EditWorkspace.Equals(gn.FeatureDataset.Workspace))
                                {

                                    unet = net as IUtilityNetworkGEN;
                                    edgeEIDs = net.CreateNetBrowser(esriElementType.esriETEdge);
                                    edgeEIDs.Reset(); int edgeEID;
                                    for (long j = 0; j < edgeEIDs.Count; j++)
                                    {
                                        edgeEID = edgeEIDs.Next();
                                        unet.SetFlowDirection(edgeEID, esriFlowDirection.esriFDWithFlow);
                                    }
                                    calcCount += 1;
                                }
                            }
                            stepProgressor.Step();

                        }
                    }

                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_2") + "\n" + ex.Message, ex.Source);
                    }
                    finally
                    {
                        if (enumFC != null)
                            Marshal.ReleaseComObject(enumFC);

                        gn = null;
                        enumFC = null;
                        net = null;
                        unet = null;
                        edgeEIDs = null;
                        //fLayer = null;
                    }
                    if (editStarted)
                    {   // Stop the edit operation 
                        if (flowDirection == Globals.GNFlowDirection.AncillaryRole)
                            editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_2"));
                        else
                            editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_3"));
                    }
                    object Missing = Type.Missing;
                    mxdoc = app.Document as IMxDocument;
                    mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, Missing, mxdoc.ActiveView.Extent);

                    if (app != null)
                        app.StatusBar.set_Message(2, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_1a") + calcCount + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_1b"));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_2") + "\n" + ex.Message, ex.Source);
                return;


            }
            finally
            {
                // Cleanup
                if (progressDialog != null)
                    progressDialog.HideDialog();
                progressDialog = null;
                progressDialogFactory = null;
                editor = null;
                eLayers = null;
                appCursor = null;
                netExt = null;
                pUID = null;
                pMap = null;
                gnList = null;
                trackCancel = null;

                stepProgressor = null;
                mxdoc = null;
            }

        }
        public static void AddFlag(IPoint pPnt, IApplication app, double snapTol)
        {


            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;
            List<IGeometricNetwork> gnList = null;
            IGeometricNetwork gn = null;
            IPoint snappedPoint = null;
            IFlagDisplay pFlagDisplay = null;
            INetFlag startNetFlag = null;

            INetworkAnalysisExt pNetAnalysisExt = null;
            IMap pMap = null;
            UID pID = null;
            int EID = -1;
            double distanceAlong;
            try
            {

                pMap = (app.Document as IMxDocument).FocusMap;
                bool boolCont = true;
                // Create a CancelTracker  
                pTrkCan = new CancelTrackerClass();
                // Create the ProgressDialog. This automatically displays the dialog  
                pProDFact = new ProgressDialogFactoryClass();
                pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                // Set the properties of the ProgressDialog  
                pProDlg.CancelEnabled = true;


                pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                // Set the properties of the Step Progressor  
                pStepPro = (IStepProgressor)pProDlg;

                pStepPro.MinRange = 0;
                pStepPro.MaxRange = 6;
                pStepPro.StepValue = 1;
                pStepPro.Position = 0;
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");


                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref pMap);
                int gnIdx = -1;

                if (gnList == null || gnList.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_2"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsErrorLbl_2"));
                    return;
                }

                // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_5");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }


                startNetFlag = Globals.GetJunctionFlag(ref pPnt, ref pMap, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out  pFlagDisplay, true) as INetFlag;
                if (startNetFlag == null)
                {
                    startNetFlag = Globals.GetEdgeFlag(ref pPnt, ref pMap, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out distanceAlong, out  pFlagDisplay, true) as INetFlag;
                }

                //Set network to trace
                if (gnIdx > -1)
                    gn = gnList[gnIdx] as IGeometricNetwork;

                // Stop if user point was not on a visible network feature, old trace results and selection are cleared
                if (gn == null || startNetFlag == null)
                {
                    return;
                }


                if (app != null)
                {
                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref  gn);
                    Globals.AddFlagToGN(ref pNetAnalysisExt, ref  gn, ref pFlagDisplay);
                    //  pFlagDisplay
                    pNetAnalysisExt = null;
                    pID = null;

                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_3") + ": " + ex.ToString());

            }
            finally
            {
                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;

                pTrkCan = null;
                gnList = null;
                gn = null;
                snappedPoint = null;
                pFlagDisplay = null;
                startNetFlag = null;
                pNetAnalysisExt = null;

                pID = null;
            }
        }
        public static bool AddBarrier(IPoint pPnt, IApplication app, double snapTol, bool showDialog)
        {
            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;

            List<IGeometricNetwork> gnList = null;
            IGeometricNetwork gn = null;
            IPoint snappedPoint = null;
            IFlagDisplay pFlagDisplay = null;
            INetFlag startNetFlag = null;
            INetworkAnalysisExt pNetAnalysisExt = null;
            UID pID = null;
            IMap pMap = null;
            int EID = -1;
            double distanceAlong;

            try
            {

                pMap = (app.Document as IMxDocument).FocusMap;
                bool boolCont = true;
                if (showDialog)
                {
                    // Create a CancelTracker  
                    pTrkCan = new CancelTrackerClass();
                    // Create the ProgressDialog. This automatically displays the dialog  
                    pProDFact = new ProgressDialogFactoryClass();
                    pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                    // Set the properties of the ProgressDialog  
                    pProDlg.CancelEnabled = true;


                    pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                    // Set the properties of the Step Progressor  
                    pStepPro = (IStepProgressor)pProDlg;

                    pStepPro.MinRange = 0;
                    pStepPro.MaxRange = 6;
                    pStepPro.StepValue = 1;
                    pStepPro.Position = 0;
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");

                }
                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref pMap);
                int gnIdx = -1;

                if (gnList == null || gnList.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_2"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsErrorLbl_2"));
                    return false;
                }

                if (showDialog)
                {
                    // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_6");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();

                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return true;
                    }
                }
                startNetFlag = Globals.GetJunctionFlag(ref pPnt, ref  pMap, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out  pFlagDisplay, false) as INetFlag;
                if (startNetFlag == null)
                {
                    startNetFlag = Globals.GetEdgeFlag(ref pPnt, ref pMap, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out distanceAlong, out  pFlagDisplay, false) as INetFlag;
                }

                //Set network to trace
                if (gnIdx > -1)
                    gn = gnList[gnIdx] as IGeometricNetwork;

                // Stop if user point was not on a visible network feature, old trace results and selection are cleared
                if (gn == null || startNetFlag == null)
                {
                    return true;
                }


                if (app != null)
                {
                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref gn);
                    Globals.AddBarrierToGN(pNetAnalysisExt, gn, pFlagDisplay);
                    //  pFlagDisplay
                    pNetAnalysisExt = null;
                    pID = null;

                }

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_3") + ": " + ex.ToString());
                return false;
            }
            finally
            {
                if (showDialog)
                {
                    if (pProDlg != null)
                    {

                        pProDlg.HideDialog();
                    }
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    pTrkCan = null;
                }

                pMap = null;
                gnList = null;
                gn = null;
                snappedPoint = null;
                pFlagDisplay = null;
                startNetFlag = null;
                pNetAnalysisExt = null;
                pID = null;
                pMap = null;
            }
        }
        public static void RemoveFlagBarrier(IPoint pPnt, IApplication app, double snapTol)
        {
            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;
            List<IGeometricNetwork> gnList = null;

            IMap pMap = null;
            INetworkAnalysisExt pNetAnalysisExt = null;

            UID pID = null;
            try
            {

                pMap = (app.Document as IMxDocument).FocusMap;
                bool boolCont = true;
                // Create a CancelTracker  
                pTrkCan = new CancelTrackerClass();
                // Create the ProgressDialog. This automatically displays the dialog  
                pProDFact = new ProgressDialogFactoryClass();
                pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                // Set the properties of the ProgressDialog  
                pProDlg.CancelEnabled = true;


                pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                // Set the properties of the Step Progressor  
                pStepPro = (IStepProgressor)pProDlg;

                pStepPro.MinRange = 0;
                pStepPro.MaxRange = 6;
                pStepPro.StepValue = 1;
                pStepPro.Position = 0;
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");


                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref pMap);

                if (gnList == null || gnList.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_2"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsErrorLbl_2"));
                    return;
                }

                // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_5") + "/" + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_6");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }




                if (app != null)
                {
                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    Globals.RemoveFlagBarrierAtLocation(pPnt.X, pPnt.Y, ref pNetAnalysisExt, snapTol);
                }





            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_3") + ": " + ex.ToString());

            }
            finally
            {
                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;
                pTrkCan = null;
                gnList = null;
                pNetAnalysisExt = null;
                pMap = null;
                pID = null;
            }
        }
        public static void ConnectSelected(IApplication app)
        {
            IEditor editor = null;
            IEditLayers eLayers = null;

            IMap pMap = null;
            IActiveView av = null;
            try
            {
                editor = Globals.getEditor(ref app);


                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }
                eLayers = (IEditLayers)editor;

                pMap = editor.Map;
                av = pMap as IActiveView;

                if (pMap.SelectionCount > 0)
                {
                    ////If above threshold, prompt to cancel
                    //if ((map.SelectionCount > 1) &&
                    //   (MessageBox.Show("Are you sure you wish to connect the selected " + map.SelectionCount + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_9b"), A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No))
                    //  return;

                    bool test = false;
                    ESRI.ArcGIS.esriSystem.IStatusBar statusBar = null;
                    ESRI.ArcGIS.esriSystem.IAnimationProgressor animationProgressor = null;
                    ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
                    ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
                    ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
                    ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = null;
                    IEnumLayer enumLayer = null;
                    IFeatureLayer fLayer = null;
                    IFeatureSelection fSel = null;
                    ILayer layer = null;
                    ICursor cursor = null;
                    IFeatureCursor fCursor = null;
                    UID geoFeatureLayerID = null;
                    INetworkClass netClass = null;
                    INetworkFeature netFeature = null;
                    IFeature feat = null;
                    int lyrCnt = 0;
                    try
                    {
                        // Create an edit operation enabling undo/redo
                        editor.StartOperation();

                        //Get list of feature layers
                        geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        enumLayer = pMap.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                        while ((layer = enumLayer.Next()) != null)
                        {
                            lyrCnt = lyrCnt + 1;

                        }
                        enumLayer.Reset();

                        statusBar = app.StatusBar;
                        animationProgressor = statusBar.ProgressAnimation;

                        animationProgressor.Show();
                        animationProgressor.Play(0, -1, -1);


                        statusBar.set_Message(0, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_7"));


                        // Create a CancelTracker
                        trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = app.hWnd;
                        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 0;
                        stepProgressor.MaxRange = lyrCnt - 1;
                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_7");

                        // Create the ProgressDialog. This automatically displays the dialog
                        progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_7");
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_7");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        bool boolean_Continue = false;
                        boolean_Continue = true;




                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        while ((layer = enumLayer.Next()) != null)
                        {


                            // Verify that this is a valid, visible point layer and that this layer is editable
                            fLayer = (IFeatureLayer)layer;
                            if (fLayer.Valid && fLayer.Visible && eLayers.IsEditable(fLayer))
                            {
                                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_7") + fLayer.Name;
                                // Verify selected features and network  
                                netClass = fLayer.FeatureClass as INetworkClass;
                                fSel = (IFeatureSelection)fLayer;
                                if ((netClass != null) && (fSel.SelectionSet.Count > 0))
                                {
                                    test = true;

                                    fSel.SelectionSet.Search(null, false, out cursor);
                                    fCursor = cursor as IFeatureCursor;

                                    while ((netFeature = (INetworkFeature)fCursor.NextFeature()) != null)
                                    {
                                        try
                                        {
                                            netFeature.Connect();
                                        }
                                        catch
                                        { }
                                        //feat = (IFeature)netFeature;
                                        //int fieldPos = feat.Fields.FindField("ENABLED");

                                        //if (fieldPos > -1)
                                        //{
                                        //    feat.set_Value(fieldPos, feat.get_Value(fieldPos));
                                        //    feat.Store();
                                        //}
                                    }

                                    Marshal.ReleaseComObject(cursor);
                                    Marshal.ReleaseComObject(fCursor);
                                }
                            }
                            stepProgressor.Step();

                            boolean_Continue = trackCancel.Continue();
                            if (!boolean_Continue)
                            {
                                break;
                            }


                        }

                        // Stop the edit operation 
                        editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_8"));
                    }

                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_8") + "\n" + ex.Message, ex.Source);
                        return;
                    }
                    finally
                    {
                        if (animationProgressor != null)
                        {
                            animationProgressor.Stop();
                            animationProgressor.Hide();
                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        if (progressDialog2 != null)
                        {
                            progressDialog2.HideDialog();
                        }

                        progressDialog2 = null;

                        statusBar = null;
                        animationProgressor = null;

                        progressDialogFactory = null;


                        enumLayer = null;
                        fLayer = null;
                        fSel = null;
                        layer = null;
                        if (cursor != null)
                            Marshal.ReleaseComObject(cursor);
                        if (fCursor != null)
                            Marshal.ReleaseComObject(fCursor);
                        cursor = null;
                        fCursor = null;
                        geoFeatureLayerID = null;
                        netClass = null;
                        netFeature = null;
                        feat = null;


                    }

                    //Alert the user know if no work was performed
                    if (!(test))
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_8a"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_8b"));
                    else
                        av.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_8") + "\n" + ex.Message, ex.Source);
                return;
            }
            finally
            {

                editor = null;
                eLayers = null;

                pMap = null;
                av = null;
            }
        }
        public static void DisconnectSelected(IApplication app)
        {
            try
            {
                //Get list of editable layers
                IEditor editor = Globals.getEditor(ref app);

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }
                IEditLayers eLayers = (IEditLayers)editor;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;

                if (map.SelectionCount > 0)
                {
                    //If above threshold, prompt to cancel
                    if ((map.SelectionCount > 1) &&
                       (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_9a") + map.SelectionCount + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_9b"), A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No))
                        return;

                    bool test = false;
                    ESRI.ArcGIS.esriSystem.IStatusBar statusBar = null;
                    ESRI.ArcGIS.esriSystem.IAnimationProgressor animationProgressor = null;
                    ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
                    ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
                    ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
                    ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = null;
                    try
                    {
                        // Create an edit operation enabling undo/redo
                        editor.StartOperation();



                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        ICursor cursor = null; IFeatureCursor fCursor = null;



                        int lyrCnt = 0;
                        while ((layer = enumLayer.Next()) != null)
                        {
                            lyrCnt = lyrCnt + 1;

                        }
                        enumLayer.Reset();

                        statusBar = app.StatusBar;
                        animationProgressor = statusBar.ProgressAnimation;

                        animationProgressor.Show();
                        animationProgressor.Play(0, -1, -1);


                        statusBar.set_Message(0, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_7"));


                        // Create a CancelTracker
                        trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = app.hWnd;
                        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 0;
                        stepProgressor.MaxRange = lyrCnt - 1;
                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_10");

                        // Create the ProgressDialog. This automatically displays the dialog
                        progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_10");
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_10");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;


                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        while ((layer = enumLayer.Next()) != null)
                        {
                            // Verify that this is a valid, visible layer and that this layer is editable
                            fLayer = (IFeatureLayer)layer;
                            if (fLayer.Valid && fLayer.Visible && eLayers.IsEditable(fLayer))
                            {
                                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_10a") + fLayer.Name;

                                // Verify selected features and network  
                                INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
                                fSel = (IFeatureSelection)fLayer;
                                if ((netClass != null) && (fSel.SelectionSet.Count > 0))
                                {
                                    test = true;

                                    fSel.SelectionSet.Search(null, false, out cursor);
                                    fCursor = cursor as IFeatureCursor;
                                    INetworkFeature netFeature;
                                    while ((netFeature = (INetworkFeature)fCursor.NextFeature()) != null)
                                    {
                                        try
                                        {
                                            netFeature.Disconnect();
                                        }
                                        catch
                                        { }
                                        //IFeature feat = (IFeature)netFeature;
                                        //int fieldPos = feat.Fields.FindField("ENABLED");

                                        //if (fieldPos > -1)
                                        //{
                                        //    feat.set_Value(fieldPos, feat.get_Value(fieldPos));
                                        //    feat.Store();
                                        //}

                                    }

                                    Marshal.ReleaseComObject(cursor);
                                    Marshal.ReleaseComObject(fCursor);

                                }

                            }
                            stepProgressor.Step();

                            boolean_Continue = trackCancel.Continue();
                            if (!boolean_Continue)
                            {
                                break;
                            }
                        }

                        // Stop the edit operation 
                        editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_10"));

                    }
                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_10") + "\n" + ex.Message, ex.Source);
                        return;
                    }
                    finally
                    {
                        if (animationProgressor != null)
                        {
                            animationProgressor.Stop();
                            animationProgressor.Hide();
                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        if (progressDialog2 != null)
                        {
                            progressDialog2.HideDialog();
                        }

                        progressDialog2 = null;




                    }

                    //Alert the user know if no work was performed
                    if (!(test))
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_8a"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_10"));
                    else
                        activeView.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_10") + "\n" + ex.Message, ex.Source);
                return;
            }

        }
        public static void CheckConnections(IApplication app, bool CheckVisibleOnly)
        {
            IProgressDialog2 progressDialog = default(IProgressDialog2);
            IGeometricNetwork geometricNetwork = null;
            ISelectionEvents selEvents = null;
            IMxDocument mxDoc = null;
            IActiveView activeView = null;
            IMap map = null;
            List<IGeometricNetwork> gnList = null;
            IMouseCursor appCursor = null;
            IEnumFeatureClass enumClass = null;
            IFeatureClass featureClass = null;
            IFeatureLayer featureLayer = null;
            IProgressDialogFactory progressDialogFactory = null;
            ITrackCancel trackCancel = null;
            IStepProgressor stepProgressor = null;
            IFeatureSelection fSel = null;
            IEnumFeature enumFeatures = null;
            IEditor editor = null;

            try
            {
                editor = Globals.getEditor(ref app);
                if (editor == null)
                    return;

                if (editor.EditState == esriEditState.esriStateNotEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }
                mxDoc = (IMxDocument)app.Document;
                activeView = (IActiveView)mxDoc.FocusMap;
                map = activeView.FocusMap;
                int countDeleted = 0;

                if (activeView == null) return;
                if (map.LayerCount == 0) return;
                long total = Globals.GetTotalVisibleNetworkFeatures(map);
                if (total > 1000)
                {
                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_11a") + total + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_11b"), A4LGSharedFunctions.Localizer.GetString("Proceed"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }

                }
                string resultMessage = "";
                string resultMessage2 = "";


                //Get visible networks
                gnList = Globals.GetGeometricNetworksCheckedVisible(ref map);


                if (gnList.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_11a"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);

                //This step is required to avoid accidently deleting features
                if (map.SelectionCount > 0)
                {
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    map.ClearSelection();
                }

                int itotal = Convert.ToInt32(total);

                //ProgressBar
                progressDialogFactory = new ProgressDialogFactoryClass();

                // Create a CancelTracker

                trackCancel = new CancelTrackerClass();

                // Set the properties of the Step Progressor
                Int32 int32_hWnd = editor.Parent.hWnd;
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                stepProgressor.MinRange = 0;
                // stepProgressor.MaxRange = itotal 
                stepProgressor.StepValue = 1;
                stepProgressor.Message = "";
                stepProgressor.Hide();

                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = false;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_11");
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_11");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                for (int i = 0; i < gnList.Count; i++)
                {
                    geometricNetwork = gnList[i] as IGeometricNetwork;
                    enumClass = null;
                    featureClass = null;
                    featureLayer = null;

                    int count = 0;

                    enumClass = geometricNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);


                    //Find all 'disconnected junctions' in orphan junction layer (edges=0)
                    //and also junctions in orphan junction that are unneed because they connect only 2 edges of the same feature
                    // Do this first so we can safely delete them(if editing)
                    string junFCName = "";
                    featureClass = geometricNetwork.OrphanJunctionFeatureClass;
                    junFCName = featureClass.AliasName;
                    bool FCorLayer = true;
                    featureLayer = Globals.FindLayer((IMap)mxDoc.FocusMap, ((IDataset)featureClass).Name, ref FCorLayer) as IFeatureLayer;

                    if (featureLayer != null &&
                            (
                                (Globals.isVisible((ILayer)featureLayer, (IMap)mxDoc.FocusMap) && CheckVisibleOnly) ||
                                (CheckVisibleOnly == false)
                            )
                        )
                    {
                        count = Globals.SelectJunctions(featureLayer, (IGeometry)activeView.Extent, 0, "ORPHAN", ref progressDialog, ref stepProgressor, ref trackCancel);
                        if ((count > 0) && (editor != null) && (editor.EditState == esriEditState.esriStateEditing))
                        {
                            try
                            {
                                editor.StartOperation();
                                fSel = (IFeatureSelection)featureLayer;
                                enumFeatures = editor.EditSelection as IEnumFeature;
                                Globals.DeleteFeatures(enumFeatures);
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_11a") + featureLayer.Name);
                            }
                            catch (Exception ex)
                            {
                                editor.AbortOperation();
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_11") + "\n" + ex.Message, ex.Source);
                            }
                            countDeleted = +count;
                            if (count == 1)
                            {
                                resultMessage2 += +count + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11a") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11b") + Environment.NewLine;
                            }
                            else if (count > 1)
                            {
                                resultMessage2 += count + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11c") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11d") + Environment.NewLine;
                            }
                        }
                        else
                        {
                            if (count == 1)
                            {
                                resultMessage2 += +count + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11a") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11e") + Environment.NewLine;
                            }
                            else if (count > 1)
                            {
                                resultMessage2 += count + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11c") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11f") + Environment.NewLine;
                            }
                        }
                    }

                    //Step through each feature layer
                    enumClass.Reset();
                    while ((featureClass = (IFeatureClass)enumClass.Next()) != null)
                    {

                        int numberJunctions;
                        count = 0;
                        // FCorLayer = true;
                        featureLayer = Globals.FindLayer((IMap)mxDoc.FocusMap, ((IDataset)featureClass).Name, ref FCorLayer) as IFeatureLayer;
                        if (featureLayer != null)
                        {

                            //Handle all non-orphan junction feature layers
                            if (junFCName != featureClass.AliasName && featureLayer != null && (
                                    (Globals.isVisible((ILayer)featureLayer, (IMap)mxDoc.FocusMap) && CheckVisibleOnly) ||
                                    (CheckVisibleOnly == false)
                                ))
                            {
                                numberJunctions = 1;
                                count = Globals.SelectJunctions(featureLayer, (IGeometry)activeView.Extent, numberJunctions, "LT", ref progressDialog, ref stepProgressor, ref trackCancel);

                                if (count == 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11g") + Environment.NewLine;// "\r\n";
                                }

                                if (count > 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetsFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11h") + Environment.NewLine;// "\r\n";
                                }


                            }
                            //Handle orphan junction feature layers
                            else if (featureLayer != null && (
                                    (Globals.isVisible((ILayer)featureLayer, (IMap)mxDoc.FocusMap) && CheckVisibleOnly) ||
                                    (CheckVisibleOnly == false)
                                ))
                            {
                                numberJunctions = 0;
                                count = Globals.SelectJunctions(featureLayer, (IGeometry)activeView.Extent, numberJunctions, "EQ", ref progressDialog, ref stepProgressor, ref trackCancel);

                                if (count == 1)
                                {
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("JunctionInThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11i") + Environment.NewLine;// "\r\n";
                                }

                                if (count > 1)
                                {
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("JunctionsInThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11j") + Environment.NewLine;// "\r\n";
                                }

                                numberJunctions = 1;
                                count = Globals.SelectJunctions(featureLayer, (IGeometry)activeView.Extent, numberJunctions, "EQ", ref progressDialog, ref stepProgressor, ref trackCancel);

                                if (count == 1)
                                {
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("JunctionInThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11k") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_11b") + Environment.NewLine;// "\r\n";
                                }

                                if (count > 1)
                                {
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("JunctionsInThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11l") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAsk_11b") + Environment.NewLine;// "\r\n";
                                }

                            }
                        }

                    }



                    //****


                    enumClass = geometricNetwork.get_ClassesByType(esriFeatureType.esriFTComplexEdge);

                    //Step through each feature layer
                    enumClass.Reset();
                    while ((featureClass = (IFeatureClass)enumClass.Next()) != null)
                    {
                        count = 0;
                        FCorLayer = true;
                        featureLayer = Globals.FindLayer((IMap)mxDoc.FocusMap, ((IDataset)featureClass).Name, ref  FCorLayer) as IFeatureLayer;
                        if (featureLayer != null)
                        {
                            //Handle all non-orphan junction feature layers
                            if ((Globals.isVisible((ILayer)featureLayer, (IMap)mxDoc.FocusMap) && CheckVisibleOnly) ||
                                    (CheckVisibleOnly == false))
                            {

                                count = Globals.SelectEdges(featureLayer, (IGeometry)activeView.Extent, ref progressDialog, ref stepProgressor, ref trackCancel, geometricNetwork.OrphanJunctionFeatureClass.ObjectClassID);
                                if (count == 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11g") + Environment.NewLine;// "\r\n";
                                }

                                if (count > 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetsFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11h") + Environment.NewLine;// "\r\n";
                                }


                            }
                        }


                    }

                    enumClass = geometricNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);

                    //Step through each feature layer
                    enumClass.Reset();
                    while ((featureClass = (IFeatureClass)enumClass.Next()) != null)
                    {

                        count = 0;
                        bool FCorLayerFeat = true;
                        featureLayer = Globals.FindLayer((IMap)mxDoc.FocusMap, ((IDataset)featureClass).Name, ref FCorLayerFeat) as IFeatureLayer;
                        if (featureLayer != null)
                        {

                            //Handle all non-orphan junction feature layers
                            if ((Globals.isVisible((ILayer)featureLayer, (IMap)mxDoc.FocusMap) && CheckVisibleOnly) ||
                                    (CheckVisibleOnly == false))
                            {

                                count = Globals.SelectEdges(featureLayer, (IGeometry)activeView.Extent, ref progressDialog, ref stepProgressor, ref trackCancel, geometricNetwork.OrphanJunctionFeatureClass.ObjectClassID);

                                if (count == 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11g") + Environment.NewLine;// "\r\n";
                                }

                                if (count > 1)
                                {
                                    //Add lookup to config to see how many junctions a FC should connect to
                                    resultMessage += count + A4LGSharedFunctions.Localizer.GetString("AssetsFromThe") + featureLayer.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_11h") + Environment.NewLine;// "\r\n";
                                }


                            }
                        }

                    }
                }



                //**







                selEvents = (ISelectionEvents)mxDoc.FocusMap;

                if (selEvents != null)
                {
                    selEvents.SelectionChanged();


                }

                if (countDeleted == 0)
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                else
                    activeView.Refresh();

                resultMessage += resultMessage2;

                if (resultMessage == "")
                    resultMessage = A4LGSharedFunctions.Localizer.GetString("NoError");


                if (progressDialog != null)
                    progressDialog.HideDialog();
                //Report results
                System.Windows.Forms.MessageBox.Show(resultMessage, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_11"), System.Windows.Forms.MessageBoxButtons.OK);

            }

            catch (Exception ex)
            {


                System.Windows.Forms.MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_11") + "\n" + ex.Message);
            }
            finally
            {
                // Cleanup
                if (progressDialog != null)
                {
                    progressDialog.HideDialog();

                    //progressDialog = null;
                    Marshal.ReleaseComObject(progressDialog);
                    //progressDialogFactory = null;
                    Marshal.ReleaseComObject(progressDialogFactory);
                    //trackCancel = null;
                    Marshal.ReleaseComObject(trackCancel);
                    //stepProgressor = null;
                    Marshal.ReleaseComObject(stepProgressor);
                    //appCursor = null;
                }
                if (appCursor != null)
                    Marshal.ReleaseComObject(appCursor);
                gnList = null;
                // Marshal.ReleaseComObject(gnList);
                //enumClass = null;
                if (enumClass != null)
                    Marshal.ReleaseComObject(enumClass);
                fSel = null;
                //Marshal.ReleaseComObject(fSel);
                enumFeatures = null;
                //Marshal.ReleaseComObject(enumFeatures);
                geometricNetwork = null;
                selEvents = null;
                mxDoc = null;
                activeView = null;
                map = null;

                featureClass = null;
                featureLayer = null;
                editor = null;


            }

        }
        public static IEnvelope ToggleOperableStatus(IApplication app, IPoint point, bool showMessage, string ISOvalveFeatureLayerName, string ISOsourceFeatureLayerName, string ISOoperableFieldNameValves, string ISOoperableFieldNameSources, string[] ISOoperableValues, double SnapTol)
        {

            IFeatureCursor fcursor = null;
            ICursor ccursor = null;
            IFeature feat = null;
            UID uID = null;
            IEditor editor = null;
            IMxDocument mxdoc = null;
            IMap map = null;
            IFeatureLayer[] valveFLs = null;
            IFeatureClass[] valveFCs = null;
            IFeatureLayer[] sourceFLs = null;
            IFeatureClass[] sourceFCs = null;
            IEnvelope retEnv = null;
            //IGeometry geom = null;
            //IProximityOperator proxOp = null;
            IEnvelope env = null;
            ISpatialFilter sf = null;
            IWorkspace work = null;
            IWorkspaceEdit workEdit = null;
            IField opField = null;
            IPoint pTmpPnt = null;

            IEnvelope ptmpEnv = null;



            IDisplayExpressionProperties pDEP = null;
            IDisplayString pIDS = null;
            try
            {
                //Get editor 
                uID = new UID();
                uID.Value = "esriEditor.Editor";
                editor = app.FindExtensionByCLSID(uID) as IEditor;

                mxdoc = app.Document as IMxDocument;
                map = mxdoc.ActiveView.FocusMap;
                SnapTol = Globals.ConvertPixelsToMap(SnapTol, map);
               
                string[] strValveFLs = ISOvalveFeatureLayerName.Split('|');
                // string[] strOpValues = .Split('|');
                valveFLs = new IFeatureLayer[strValveFLs.Length];//(IFeatureLayer)Globals.FindLayer(map, valveFLName);
                valveFCs = new IFeatureClass[strValveFLs.Length];
                bool lyrFnd = false;

                for (int i = 0; i < valveFLs.Length; i++)
                {
                    bool FCorLayerTemp = true;

                    valveFLs[i] = (IFeatureLayer)Globals.FindLayer(map, strValveFLs[i], ref FCorLayerTemp);
                    if (valveFLs[i] != null)
                    {
                        valveFCs[i] = valveFLs[i].FeatureClass;
                        lyrFnd = true;
                    }

                }
                string[] strSourceFLs = ISOsourceFeatureLayerName.Split('|');
                // string[] strOpValues = .Split('|');
                sourceFLs = new IFeatureLayer[strSourceFLs.Length];//(IFeatureLayer)Globals.FindLayer(map, valveFLName);
                sourceFCs = new IFeatureClass[strSourceFLs.Length];

                for (int i = 0; i < sourceFLs.Length; i++)
                {
                    bool FCorLayerTemp = true;

                    sourceFLs[i] = (IFeatureLayer)Globals.FindLayer(map, strSourceFLs[i], ref FCorLayerTemp);
                    if (sourceFLs[i] != null)
                    {
                        sourceFCs[i] = sourceFLs[i].FeatureClass;
                        lyrFnd = true;
                    }

                }
                if (lyrFnd == false)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12a") + ISOvalveFeatureLayerName + A4LGSharedFunctions.Localizer.GetString("Or") + ISOsourceFeatureLayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12b"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12c"));
                    return null;
                }


                // Globals.ClearSelected(map, false);



                foreach (IFeatureLayer valveFLayer in valveFLs)
                {

                    if (point != null)
                    {

                        env = new EnvelopeClass();
                        env.DefineFromPoints(1, ref point);
                        env.Expand(SnapTol / 2, SnapTol / 2, false);

                        sf = new SpatialFilterClass();
                        sf.Geometry = env;
                        sf.GeometryField = valveFLayer.FeatureClass.ShapeFieldName;
                        sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        fcursor = valveFLayer.Search(sf, false);

                    }
                    else
                    {

                        ((IFeatureSelection)valveFLayer).SelectionSet.Search(null, false, out ccursor);
                        fcursor = (IFeatureCursor)ccursor;
                    }

                    work = valveFLayer.FeatureClass.FeatureDataset.Workspace;
                    workEdit = work as IWorkspaceEdit;

                    if (!workEdit.IsBeingEdited())
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("StrtEditing"));
                        return null;
                    }

                    //Find closest feature (in this feature layer)
                    double testDistance = SnapTol;
                    double thisDistance;

                    //IFeature targetFeature = null; int targetOID = -1;
                    workEdit.StartEditOperation();

                    while ((feat = fcursor.NextFeature()) != null)
                    {

                        try
                        {

                            int operableFieldPos = valveFLayer.FeatureClass.FindField(ISOoperableFieldNameValves);
                            if (operableFieldPos < 0)
                            {
                                //MessageBox.Show("Operable status cannot be toggled since " + m_ISOoperableFieldName + " field is not found in " + m_ISOvalveFeatureLayerName + ".", messageBoxHeader);
                                continue;
                            }

                            opField = valveFLayer.FeatureClass.Fields.get_Field(operableFieldPos);


                            //      targetFeature = valveFLayer.FeatureClass.GetFeature(targetOID);
                            string disVal;
                            if (opField.Type == esriFieldType.esriFieldTypeString)
                            {
                                if (feat.get_Value(operableFieldPos) == null)
                                {
                                    feat.set_Value(operableFieldPos, ISOoperableValues[1]);
                                    disVal = ISOoperableValues[1];
                                }

                                else if (feat.get_Value(operableFieldPos) is DBNull)
                                {
                                    feat.set_Value(operableFieldPos, ISOoperableValues[1]);
                                    disVal = ISOoperableValues[1];
                                }
                                else if (feat.get_Value(operableFieldPos).ToString() == ISOoperableValues[0])
                                {
                                    feat.set_Value(operableFieldPos, ISOoperableValues[1]);
                                    disVal = ISOoperableValues[1];
                                }
                                else
                                {
                                    feat.set_Value(operableFieldPos, ISOoperableValues[0]);
                                    disVal = ISOoperableValues[0];
                                }
                            }
                            else
                            {
                                if (feat.get_Value(operableFieldPos) == null)
                                {
                                    feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                    disVal = "True";
                                }

                                else if (feat.get_Value(operableFieldPos) is DBNull)
                                {
                                    feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                    disVal = "True";
                                }
                                else if (Convert.ToInt32(feat.get_Value(operableFieldPos)) == Convert.ToInt32(ISOoperableValues[0]))
                                {
                                    feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                    disVal = "True";
                                }
                                else
                                {
                                    feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[0]));
                                    disVal = "False";
                                }
                            }

                            if (retEnv == null)
                            {
                                retEnv = new EnvelopeClass();
                                pTmpPnt = feat.ShapeCopy as IPoint;

                                retEnv.DefineFromPoints(1, ref  pTmpPnt);
                                retEnv.Expand(SnapTol / 2, SnapTol / 2, false);
                                pTmpPnt = null;

                            }
                            else
                            {

                                pTmpPnt = feat.ShapeCopy as IPoint;
                                ptmpEnv = new EnvelopeClass();
                                ptmpEnv.DefineFromPoints(1, ref  pTmpPnt);
                                ptmpEnv.Expand(SnapTol / 2, SnapTol / 2, false);
                                retEnv.Union(ptmpEnv);

                                pTmpPnt = null;
                                ptmpEnv = null;

                            }
                            feat.Store();



                            pIDS = (IDisplayString)valveFLayer;
                            pDEP = pIDS.ExpressionProperties;
                            string disEx = pIDS.FindDisplayString((IObject)feat);


                            // MessageBox.Show("Valve: " + targetFeature.get_Value(targetFeature.Fields.FindField(valveFLayer.DisplayField)).ToString() + A4LGSharedFunctions.Localizer.GetString("IsNowOperable") + disVal);
                            if (showMessage)
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ValveFrom") + valveFLayer.Name + ": " + disEx + opField.AliasName +  A4LGSharedFunctions.Localizer.GetString("IsNowOperable") + disVal);

                            Globals.FlashGeometry(feat.Shape, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);

                        }

                        catch (Exception ex)
                        {
                            editor.AbortOperation();

                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12c") + "\n" + ex.Message, ex.Source);
                            return null;
                        }
                        finally
                        {


                        }


                        if (feat != null)
                        {
                            Marshal.ReleaseComObject(feat);
                        }

                    }
                    try
                    {
                        workEdit.StopEditOperation();


                    }
                    catch
                    { }







                }

                if (point != null)
                {

                    foreach (IFeatureLayer sourceFLayer in sourceFLs)
                    {
                        fcursor = null;
                        if (sourceFLayer == null)
                        {

                            continue;
                        }
                        if (sourceFLayer.FeatureClass != null)
                        {
                            continue;
                        }
                        if (retEnv != null)
                        {
                            sf = new SpatialFilterClass();
                            sf.Geometry = retEnv;

                            sf.GeometryField = sourceFLayer.FeatureClass.ShapeFieldName;
                            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            fcursor = sourceFLayer.Search(sf, false);

                        }
                        else
                        {

                            ((IFeatureSelection)sourceFLayer).SelectionSet.Search(null, false, out ccursor);
                            fcursor = (IFeatureCursor)ccursor;
                        }



                        work = sourceFLayer.FeatureClass.FeatureDataset.Workspace;
                        workEdit = work as IWorkspaceEdit;


                        if (!workEdit.IsBeingEdited())
                        {
                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("StrtEditing"));
                            continue;
                        }




                        workEdit.StartEditOperation();

                        while ((feat = fcursor.NextFeature()) != null)
                        {



                            try
                            {

                                int operableFieldPos = sourceFLayer.FeatureClass.FindField(ISOoperableFieldNameSources);
                                if (operableFieldPos < 0)
                                {
                                    //MessageBox.Show("Operable status cannot be toggled since " + m_ISOoperableFieldName + " field is not found in " + m_ISOsourceFeatureLayerName + ".", messageBoxHeader);
                                    continue;
                                }
                                opField = sourceFLayer.FeatureClass.Fields.get_Field(operableFieldPos);


                                //      targetFeature = sourceFLayer.FeatureClass.GetFeature(targetOID);
                                string disVal;

                                if (opField.Type == esriFieldType.esriFieldTypeString)
                                {
                                    if (feat.get_Value(operableFieldPos) == null)
                                    {
                                        feat.set_Value(operableFieldPos, ISOoperableValues[1]);
                                        disVal = "True";
                                    }

                                    else if (feat.get_Value(operableFieldPos) is DBNull)
                                    {
                                        feat.set_Value(operableFieldPos, (ISOoperableValues[1]));
                                        disVal = "True";
                                    }
                                    else if (feat.get_Value(operableFieldPos).ToString() == ISOoperableValues[0])
                                    {
                                        feat.set_Value(operableFieldPos, ISOoperableValues[1]);
                                        disVal = "True";
                                    }
                                    else
                                    {
                                        feat.set_Value(operableFieldPos, ISOoperableValues[0]);
                                        disVal = "False";
                                    }
                                }
                                else
                                {
                                    if (feat.get_Value(operableFieldPos) == null)
                                    {
                                        feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                        disVal = "True";
                                    }

                                    else if (feat.get_Value(operableFieldPos) is DBNull)
                                    {
                                        feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                        disVal = "True";
                                    }

                                    else if (Convert.ToInt32(feat.get_Value(operableFieldPos)) == Convert.ToInt32(ISOoperableValues[0]))
                                    {
                                        feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[1]));
                                        disVal = "True";
                                    }
                                    else
                                    {
                                        feat.set_Value(operableFieldPos, Convert.ToInt32(ISOoperableValues[0]));
                                        disVal = "False";
                                    }
                                }

                                feat.Store();




                                pIDS = (IDisplayString)sourceFLayer;
                                pDEP = pIDS.ExpressionProperties;
                                string disEx = pIDS.FindDisplayString((IObject)feat);


                                // MessageBox.Show("source: " + targetFeature.get_Value(targetFeature.Fields.FindField(sourceFLayer.DisplayField)).ToString() + A4LGSharedFunctions.Localizer.GetString("IsNowOperable") + disVal);
                                if (showMessage)
                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("SourceFrom") + sourceFLayer.Name + ": " + disEx + opField.AliasName + A4LGSharedFunctions.Localizer.GetString("IsNowOperable") + disVal);
                                //  MessageBox.Show("source: " + disEx + A4LGSharedFunctions.Localizer.GetString("IsNowOperable") + disVal);

                                Globals.FlashGeometry(feat.Shape, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);

                            }

                            catch (Exception ex)
                            {
                                editor.AbortOperation();
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12c") + "\n" + ex.Message, ex.Source);
                                return null;
                            }
                            finally
                            {


                            }


                            if (feat != null)
                            {
                                Marshal.ReleaseComObject(feat);
                            }
                            // feat = fcursor.NextFeature();
                        }
                        try
                        {
                            workEdit.StopEditOperation();
                        }
                        catch
                        { }



                        // if (targetOID < 0) continue;


                    }
                }
                if (retEnv == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_13a"));
                }

                return retEnv;

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_12c") + "\n" + ex.Message, ex.Source);
                return null;
            }
            finally
            {

                if (fcursor != null)
                {
                    Marshal.ReleaseComObject(fcursor);

                }

                if (ccursor != null)
                {
                    Marshal.ReleaseComObject(ccursor);

                }
                ccursor = null;
                fcursor = null;

                feat = null;
                uID = null;
                editor = null;
                mxdoc = null;
                map = null;
                valveFLs = null;
                valveFCs = null;
                sourceFLs = null;
                sourceFCs = null;
                retEnv = null;
                //geom = null;
                //proxOp = null;
                env = null;
                sf = null;
                work = null;
                workEdit = null;
                opField = null;
                pTmpPnt = null;
                ptmpEnv = null;
                pDEP = null;
                pIDS = null;
            }


        }
        public static void MoveConnectionsToNewLine(IApplication app, double snapTol, List<MoveConnectionsDetails> moveConDetails)
        {

            IEditor editor = null;

            ESRI.ArcGIS.esriSystem.IStatusBar statusBar = null;
            ESRI.ArcGIS.esriSystem.IAnimationProgressor animationProgressor = null;
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = null;
            ICommandItem pCmdItem;


            IMxDocument pMxDoc = ((IMxDocument)app.Document);
            IGeometricNetwork pGN = null;
            IFeature pFeature = null;
            IPoint pTracePoint = null;
            ISimpleLineSymbol pSimpleLineSym = null;
            IRgbColor pRGBColor = null;

            IGraphicsContainer gc = null;

            IElement pMoveElemFirst = null;
            IElementProperties3 pMoveElemPropFirst = null;

            IElement element = null;
            IElementProperties3 elementProp = null;
            ILineElement lineElem = null;
            IJunctionFeature pJuncFeat = null;


            IFeatureLayer pSourceLayer = null;
            IFeature pSourceFeature = null;
            IFeature pMoveFeat = null;
            INetworkFeature pNetworkSourceFeature = null;
            IEdgeFeature iTargetEdgeFeat = null;
            IComplexEdgeFeature iCEdge = null;
            IFeatureClass pOraphFC = null;

            List<int> OIDs = new List<int>();
            List<string> FeatLoc = new List<string>();
            List<IObjectClass> FCs = new List<IObjectClass>();
            IPolyline pL;

            IHitTest pHtTest = null;//= pPolyline as IHitTest;

            IPoint pHitPntOne = new PointClass();
            double pHitDistOne = -1;
            int pHitPrtOne = -1;
            int pHitSegOne = -1;
            bool pHitSideOne = false;




            //IFeatureCursor pFeatCursor = null;
            IFeatureLayer pMainLayer = null;
            //IFeatureLayer pMainLayer = null;
            //IFeatureLayer pTapLayer = null;
            ILayer pLay = null;
            try
            {
                editor = Globals.getEditor(ref app);


                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_2"));
                    return;
                }

                pTracePoint = pMxDoc.CurrentLocation;
                pFeature = Globals.GetNetworkAndFeatureAtLocation(pTracePoint, app, esriElementType.esriETEdge, out pGN, snapTol);


                if (pFeature == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("NoFtrFndOnClck"));
                    return;
                }

                int idxDet = -1;

                for (int i = 0; i < moveConDetails.Count; i++)
                {
                    //MoveConnectionsDetails conDet = moveConDetails[i];
                    bool FCorLayerTemp = true;
                    pMainLayer = (IFeatureLayer)Globals.FindLayer(app, moveConDetails[i].LineLayer, ref FCorLayerTemp);
                    if (pMainLayer != null)
                    {
                        if (pMainLayer.FeatureClass.ObjectClassID == pFeature.Class.ObjectClassID)
                        {
                            idxDet = i;

                            break;

                        }
                    }
                }

                if (idxDet == -1)
                    return;



                if (Globals.IsEditable(ref pFeature, ref editor) == false)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_13b"));
                    return;
                }
                if (pFeature.FeatureType != esriFeatureType.esriFTComplexEdge)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_13c"));
                    return;
                }
                pRGBColor = Globals.GetColor(255, 0, 0);

                pSimpleLineSym = Globals.CreateSimpleLineSymbol(pRGBColor, 2, esriSimpleLineStyle.esriSLSSolid);



                gc = pMxDoc.FocusMap as IGraphicsContainer;




                gc.Reset();
                element = gc.Next();


                while (element != null)
                {
                    elementProp = element as IElementProperties3;
                    if (elementProp.Name.Contains("MoveFeatureFlag"))
                    {
                        if (pMoveElemFirst == null)
                        {

                            pMoveElemFirst = element;

                            pMoveElemPropFirst = elementProp;
                            string[] firstVals = pMoveElemPropFirst.Name.ToString().Split(':');
                            if (firstVals[1] != pFeature.Class.ObjectClassID.ToString())
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13a"));
                                return;

                            }

                            break;
                        }

                    }
                    element = gc.Next();
                }






                lineElem = new LineElementClass();
                lineElem.Symbol = pSimpleLineSym;
                element = (IElement)lineElem;
                element.Geometry = pFeature.ShapeCopy;
                elementProp = element as IElementProperties3;
                elementProp.Name = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_13a") + ": " + pFeature.Class.ObjectClassID.ToString() + ":" + pFeature.OID;

                elementProp.ReferenceScale = pMxDoc.FocusMap.ReferenceScale;
                gc.AddElement(element, 0);
                pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pFeature.Shape.Envelope);

                if (pMoveElemFirst == null)
                    return;
                else
                {
                    List<int> LayersToMoveIDs = new List<int>();
                    foreach (string Lay in moveConDetails[idxDet].LayersToMove)
                    {
                        bool FCorLayerTemp = true;
                        pLay = Globals.FindLayer(app, Lay, ref FCorLayerTemp);
                        if (pLay != null)
                        {
                            LayersToMoveIDs.Add(((IFeatureLayer)pLay).FeatureClass.ObjectClassID);

                        }
                    }
                    statusBar = app.StatusBar;
                    animationProgressor = statusBar.ProgressAnimation;

                    animationProgressor.Show();
                    animationProgressor.Play(0, -1, -1);


                    statusBar.set_Message(0, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13a"));


                    // Create a CancelTracker
                    trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                    progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                    // Set the properties of the Step Progressor
                    System.Int32 int32_hWnd = app.hWnd;
                    stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = 4;
                    stepProgressor.StepValue = 1;
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13a");

                    // Create the ProgressDialog. This automatically displays the dialog
                    progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                    // Set the properties of the ProgressDialog
                    progressDialog2.CancelEnabled = true;
                    progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13a");
                    progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13a");
                    progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral;


                    System.Boolean boolean_Continue = true;

                    stepProgressor.Step();


                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_13b");
                    //ProfileFindPath();
                    pCmdItem = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
                    if (pCmdItem != null)
                    {
                        pCmdItem.Execute();
                    }
                    string[] elemInfo = (elementProp = pMoveElemFirst as IElementProperties3).Name.Split(':');

                    pSourceLayer = Globals.FindLayerByClassID(pMxDoc.FocusMap, elemInfo[1]) as IFeatureLayer;
                    pSourceFeature = pSourceLayer.FeatureClass.GetFeature(Convert.ToInt32(elemInfo[2]));
                    pNetworkSourceFeature = (INetworkFeature)pSourceFeature;
                    iTargetEdgeFeat = (IEdgeFeature)pNetworkSourceFeature;
                    iCEdge = (pNetworkSourceFeature) as IComplexEdgeFeature;
                    pOraphFC = pGN.OrphanJunctionFeatureClass;


                    editor.StartOperation();
                    pL = pSourceFeature.ShapeCopy as IPolyline;
                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13b");


                    for (int i = 0; i < iCEdge.JunctionFeatureCount; i++)
                    {
                        pJuncFeat = iCEdge.get_JunctionFeature(i);

                        if ((pJuncFeat as IFeature).Class.ObjectClassID != pOraphFC.ObjectClassID && LayersToMoveIDs.Contains((pJuncFeat as IFeature).Class.ObjectClassID))
                        {

                            if (pL.FromPoint.X == ((pJuncFeat as IFeature).Shape as IPoint).X && pL.FromPoint.Y == ((pJuncFeat as IFeature).Shape as IPoint).Y)
                            {
                                FeatLoc.Add("From");
                            }
                            else if (pL.ToPoint.X == ((pJuncFeat as IFeature).Shape as IPoint).X && pL.ToPoint.Y == ((pJuncFeat as IFeature).Shape as IPoint).Y)
                            {
                                FeatLoc.Add("To");
                            }
                            else
                            {
                                FeatLoc.Add("Along");
                            }

                            OIDs.Add((pJuncFeat as IFeature).OID);
                            FCs.Add((pJuncFeat as IFeature).Class);
                        }
                    }
                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13c");

                    pNetworkSourceFeature.Disconnect();


                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_13d");
                    string msg = "";
                    List<IFeature> pM = new List<IFeature>();
                    for (int i = 0; i < OIDs.Count; i++)
                    {
                        pMoveFeat = (FCs[i] as IFeatureClass).GetFeature(OIDs[i]);
                        esriGeometryHitPartType pSearchLoc;
                        if (FeatLoc[i] == "From" || FeatLoc[i] == "To")
                        {
                            pSearchLoc = esriGeometryHitPartType.esriGeometryPartEndpoint;
                        }
                        else
                        {
                            pSearchLoc = esriGeometryHitPartType.esriGeometryPartBoundary;
                        }
                        pHtTest = pFeature.ShapeCopy as IHitTest;
                        bool bHitOne = pHtTest.HitTest(pMoveFeat.Shape as IPoint, 50, pSearchLoc,
                                        pHitPntOne, ref pHitDistOne, ref pHitPrtOne, ref pHitSegOne, ref pHitSideOne);

                        if (bHitOne != false)
                        {
                            if ((pMoveFeat.Shape as IPoint).Z != null && Globals.IsNumeric((pMoveFeat.Shape as IPoint).Z.ToString()))
                                pHitPntOne.Z = (pMoveFeat.Shape as IPoint).Z;

                            pMoveFeat.Shape = pHitPntOne;
                            pM.Add(pMoveFeat);

                            try
                            {
                                pMoveFeat.Store();
                                INetworkFeature netFeature = null;
                                netFeature = pMoveFeat as INetworkFeature;
                                netFeature.Connect();

                            }
                            catch
                            {
                                if (msg == "")
                                {
                                    msg = A4LGSharedFunctions.Localizer.GetString("FeatureIn") + (FCs[i] as IFeatureClass).AliasName + A4LGSharedFunctions.Localizer.GetString("WithOID") + pMoveFeat.OID + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13b");

                                }
                                else
                                {
                                    msg = msg + "\n" + A4LGSharedFunctions.Localizer.GetString("FeatureIn") + (FCs[i] as IFeatureClass).AliasName + A4LGSharedFunctions.Localizer.GetString("WithOID") + pMoveFeat.OID + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13b");

                                }
                            }

                        }
                    }

                    ////foreach (IFeature pF in pM)
                    ////{
                    ////    try
                    ////    {
                    ////        pF.Store();
                    ////        INetworkFeature netFeature = null;
                    ////        netFeature = pF as INetworkFeature;
                    ////        netFeature.Connect();

                    ////    }
                    ////    catch
                    ////    {
                    ////        if (msg == "")
                    ////        {
                    ////            msg = A4LGSharedFunctions.Localizer.GetString("FeatureIn") + (pF.Class as IFeatureClass).AliasName + A4LGSharedFunctions.Localizer.GetString("WithOID") + pMoveFeat.OID + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13b");

                    ////        }
                    ////        else
                    ////        {
                    ////            msg = msg + "\n" + A4LGSharedFunctions.Localizer.GetString("FeatureIn") + (pF.Class as IFeatureClass).AliasName + A4LGSharedFunctions.Localizer.GetString("WithOID") + pMoveFeat.OID + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13b");

                    ////        }
                    ////    }


                    ////    if (msg != "")
                    ////    {
                    ////        MessageBox.Show(msg);

                    ////    }
                    ////}
                    //INetworkFeature pNetworkTargetFeature = (INetworkFeature)pFeature;
                    // IFeatureLayer pTargetLayer = Globals.FindLayerByClassID(pMxDoc.FocusMap,pFeature.Class.CLSID.ToString()) as IFeatureLayer;




                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("Complete");



                    try
                    {
                        Globals.RemoveTraceGraphics(pMxDoc.FocusMap, true);
                    }
                    catch
                    { }
                    // unpress the UIToolControl button
                    app.CurrentTool = null;
                    app.RefreshWindow();
                    pMxDoc.ActiveView.Refresh();
                    return;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    editor.AbortOperation();

                }
                catch
                { }

                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_13c") + ex.Message);
                return;
            }
            finally
            {
                try
                {
                    editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_13a"));
                }
                catch
                { }



                if (progressDialog2 != null)
                    progressDialog2.HideDialog();
                progressDialog2 = null;
                pCmdItem = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                if (pCmdItem != null)
                {
                    pCmdItem.Execute();
                }
                pLay = null;
                statusBar = null;
                animationProgressor = null;
                trackCancel = null;
                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog2 = null;
                pCmdItem = null;


                pMxDoc = null;
                pGN = null;
                pFeature = null;
                pTracePoint = null;
                pSimpleLineSym = null;
                pRGBColor = null;

                gc = null;

                pMoveElemFirst = null;
                pMoveElemPropFirst = null;

                element = null;
                elementProp = null;
                lineElem = null;
                pJuncFeat = null;


                pSourceLayer = null;
                pSourceFeature = null;
                pMoveFeat = null;
                pNetworkSourceFeature = null;
                iTargetEdgeFeat = null;
                iCEdge = null;
                pOraphFC = null;

                OIDs = null;
                FeatLoc = null;
                FCs = null;
                pL = null;

                pHtTest = null;//= pPolyline as IHitTest;

                pHitPntOne = null;
                editor = null;

            }

        }
        public static void TraceFlow(ref IPoint point, IApplication app, esriFlowMethod flow, double snapTol, bool traceIndeterminate, bool selectEdges)
        {
            IMap map = null;
            IProgressDialogFactory pProDFact;
            IStepProgressor pStepPro;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan;


            List<IGeometricNetwork> gnList;
            IGeometricNetwork gn = null;
            IPoint snappedPoint = null;
            IFlagDisplay pFlagDisplay;
            INetFlag startNetFlag;
            ITraceFlowSolverGEN traceFlowSolver;
            List<IEdgeFlag> pEdgeFlags = null;
            List<IJunctionFlag> pJunctionFlags = null;
            //List<IEdgeFlag> pEdgeFlagsBar = null;
            //List<IJunctionFlag> pJunctionFlagsBar = null;
            INetElementBarriers pEdgeElementBarriers;
            INetElementBarriers pJunctionElementBarriers;
            ISelectionSetBarriers pSelectionSetBarriers;
            INetworkAnalysisExt pNetAnalysisExt = null;
            List<INetFlag> pNetFlags = new List<INetFlag>();
            IJunctionFlag[] junctionFlag;
            IEdgeFlag[] edgeFlag;
            IEnumNetEID juncEIDs;
            IEnumNetEID edgeEIDs;
            IEnvelope env;
            int EID = -1;
            double distanceAlong;

            UID pID = null;
            INetSolver netSolver = null;

            try
            {

                map = ((IMxDocument)app.Document).FocusMap;



                bool boolCont = true;
                // Create a CancelTracker  
                pTrkCan = new CancelTrackerClass();
                // Create the ProgressDialog. This automatically displays the dialog  
                pProDFact = new ProgressDialogFactoryClass();
                pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                // Set the properties of the ProgressDialog  
                pProDlg.CancelEnabled = true;
                if (flow == esriFlowMethod.esriFMConnected)
                {
                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_14a");
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_14a");
                }
                else if (flow == esriFlowMethod.esriFMDownstream)
                {
                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_14b");
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_14b");
                }
                else if (flow == esriFlowMethod.esriFMUpstream)
                {
                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_14c");
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_14c");
                }

                pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                // Set the properties of the Step Progressor  
                pStepPro = (IStepProgressor)pProDlg;

                pStepPro.MinRange = 0;
                pStepPro.MaxRange = 8;
                pStepPro.StepValue = 1;
                pStepPro.Position = 0;
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");


                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref map);
                int gnIdx = -1;

                if (gnList == null || gnList.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_2"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsErrorLbl_2"));
                    return;
                }

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14a");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                //Remove old trace graphics (flags and results)
                Globals.RemoveTraceGraphics(map, true);
                Globals.ClearSelected(map, true);

                // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_5");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }

                startNetFlag = Globals.GetJunctionFlag(ref point, ref map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out pFlagDisplay, true) as INetFlag;
                if (startNetFlag == null)
                {
                    startNetFlag = Globals.GetEdgeFlag(ref point, ref map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out distanceAlong, out pFlagDisplay, true) as INetFlag;
                }

                //Set network to trace
                if (gnIdx > -1)
                    gn = gnList[gnIdx] as IGeometricNetwork;

                // Stop if user point was not on a visible network feature, old trace results and selection are cleared
                if (gn == null || startNetFlag == null)
                {
                    return;
                }


                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14b");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                //Setup trace solver
                //  ITraceFlowSolverGEN traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                // ITraceFlowSolver traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(gn);

                if (app != null)
                {

                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref gn);
                    traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(ref pNetAnalysisExt, out pEdgeFlags, out pJunctionFlags, out pEdgeElementBarriers, out pJunctionElementBarriers, out pSelectionSetBarriers) as ITraceFlowSolverGEN;
                    pID = null;

                }
                else
                {
                    traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                    netSolver = traceFlowSolver as INetSolver;

                    netSolver.SourceNetwork = gn.Network;
                    netSolver = null;

                }




                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14c");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                //Add the flag to the trace solver


                if (pEdgeFlags != null)
                {
                    foreach (IEdgeFlag pEdFl in pEdgeFlags)
                    {
                        pNetFlags.Add((INetFlag)pEdFl);

                    }
                }
                if (pJunctionFlags != null)
                {

                    foreach (IJunctionFlag pJcFl in pJunctionFlags)
                    {
                        pNetFlags.Add((INetFlag)pJcFl);
                    }
                }
                if (startNetFlag != null)
                {


                    pNetFlags.Add((INetFlag)startNetFlag);

                }

                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14d");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                //Run the trace

                traceFlowSolver.FindFlowElements(flow, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);

                if (juncEIDs.Count == 0)
                {
                    if (flow == esriFlowMethod.esriFMDownstream)
                        MessageBox.Show(
                          A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14e") + Environment.NewLine + Environment.NewLine +
                          A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14f")
                        , A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_14b"));
                    else
                        MessageBox.Show(
                          A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14g") + Environment.NewLine + Environment.NewLine +
                          A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14f")
                          , A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_14c"));
                    return;
                }

                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14h");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                //Select junction features
                Globals.SelectJunctions(ref map, ref gn, ref juncEIDs, ref junctionFlag, "", "", "", true);

                if (selectEdges)
                    Globals.SelectEdges(ref map, ref gn, ref edgeEIDs);
                else
                    Globals.DrawEdges(ref map, ref gn, ref edgeEIDs);


                //edgeEIDs.Reset();

                //Draw edge graphics

                //Draw graphic point at start location of trace
                if (pNetAnalysisExt != null)
                {
                    Globals.AddFlagToGN(ref pNetAnalysisExt, ref gn, ref pFlagDisplay);
                }
                else
                {
                    Globals.AddPointGraphic(map, snappedPoint, false);
                }
                Globals.GetCommand("esriArcMapUI.ZoomToSelectedCommand", app).Execute();

                //Open identify dialog with selected features
                //IdentifySelected(map);


                // add set flow direction buttons
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("Complete");
                pStepPro.Step();
                boolCont = pTrkCan.Continue();

                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;
                }
                ((IMxDocument)app.Document).ActiveView.Refresh();

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_14a") + ": " + ex.ToString());
                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                    //pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return;

                }

            }
            finally
            {
                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;





                gnList = null;
                gn = null;
                snappedPoint = null;
                pFlagDisplay = null;
                startNetFlag = null;
                traceFlowSolver = null;
                pEdgeFlags = null;
                pJunctionFlags = null;
                pNetAnalysisExt = null;
                pNetFlags = null;
                junctionFlag = null;
                edgeFlag = null;
                juncEIDs = null;
                edgeEIDs = null;
                env = null;
                map = null;

                pID = null;
                netSolver = null;
            }

        }


        public static string TraceFindClosest(double[] x, double[] y, IApplication app, string targetFLName, string sWeightName,
                                double snapTol, bool processEvent, bool traceIndeterminate)
        {

            IMap map = null;

            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;


            List<IGeometricNetwork> gnList = null;
            int gnIdx;
            IGeometricNetwork gn = null;
            IPoint snappedPoint = null;
            int EID = -1;
            double distanceAlong;
            List<INetFlag> startNetFlag = null;
            List<IFlagDisplay> pFlagsDisplay = null;
            //Find feature classes

            IJunctionFlag[] junctionFlag = null;
            IEdgeFlag[] edgeFlag = null;
            ITraceFlowSolverGEN traceFlowSolver = null;
            INetSolver netSolver = null;
            INetElementBarriersGEN netElementBarriers = null;

            INetElementBarriers nb = null;
            IEnumNetEID juncEIDs = null;
            IEnumNetEID edgeEIDs = null;
            IEIDInfo eidInfo = null;
            IEIDInfo valveEIDInfo = null;
            IEIDInfo sourceEIDInfo = null;
            IEIDInfo vEIDInfo = null;
            List<int[]> userIds = null;

            IEIDHelper eidHelper = null;
            List<Hashtable> valveEIDInfoHT = null;
            Hashtable sourceEIDInfoHT = null;
            System.Object[] segCosts = null;
            ISelectionSetBarriers netElementBarrier = null;


            List<IEdgeFlag> pEdgeFlags = null;
            List<IJunctionFlag> pJunctionFlags = null;
            //List<IEdgeFlag> pEdgeFlagsBar = null;
            //List<IJunctionFlag> pJunctionFlagsBar = null;
            INetElementBarriers pEdgeElementBarriers = null;
            INetElementBarriers pJunctionElementBarriers = null;
            ISelectionSetBarriers pSelectionSetBarriers = null;
            List<INetFlag> pNetFlags;
            //ITraceResult traceRes = null;
            IFlagDisplay pFlagDisplay = null;
            INetworkAnalysisExt pNetAnalysisExt = null;
            UID pID = null;

            IJunctionFlag[] junctionFlags = null;
            IEdgeFlag[] edgeFlags = null;


            List<BarClassIDS> barrierIds = null;

            Hashtable sourceDirectEIDInfoHT = null;
            INetFlag netFlag1 = null;
            INetFlag netFlag2 = null;

            Hashtable htClosestAsset = null;
            IFeatureLayer pClosestLayer = null;
            INetwork pNetwork = null;
            INetSchema pNetSchema = null;
            INetWeight pNetWeight = null;
            INetSolverWeights pNetSolverW = null;

            try
            {
                map = ((app.Document as IMxDocument).FocusMap);

                bool boolCont = true;
                if (processEvent)
                {
                    // Create a CancelTracker  
                    pTrkCan = new CancelTrackerClass();
                    // Create the ProgressDialog. This automatically displays the dialog  
                    pProDFact = new ProgressDialogFactoryClass();
                    pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                    // Set the properties of the ProgressDialog  
                    pProDlg.CancelEnabled = true;

                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_15a");
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_15a");


                    pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                    // Set the properties of the Step Progressor  
                    pStepPro = (IStepProgressor)pProDlg;

                    pStepPro.MinRange = 0;
                    pStepPro.MaxRange = 18;
                    pStepPro.StepValue = 1;
                    pStepPro.Position = 0;
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");



                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();

                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                }
                if (processEvent)
                {
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref map);
                gnIdx = -1;
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction

                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                startNetFlag = new List<INetFlag>();// null;// Globals.GetJunctionFlag(x, y, map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID) as INetFlag;
                //if (startNetFlag == null)
                pFlagsDisplay = new List<IFlagDisplay>();
                if (x != null)
                {
                    for (int l = 0; l < x.Length; l++)
                    {
                        startNetFlag.Add(Globals.GetEdgeFlag(x[l], y[l], ref  map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out distanceAlong, out pFlagDisplay, true) as INetFlag);
                        pFlagsDisplay.Add(pFlagDisplay);

                    }
                }

                //Set network to trace
                if (gnIdx > -1)
                    gn = gnList[gnIdx] as IGeometricNetwork;


                if (app != null)
                {


                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    if (gn != null)
                    {
                        Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref gn);
                    }

                    traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(ref pNetAnalysisExt, out pEdgeFlags, out pJunctionFlags, out pEdgeElementBarriers, out pJunctionElementBarriers, out pSelectionSetBarriers) as ITraceFlowSolverGEN;


                    gn = pNetAnalysisExt.CurrentNetwork;
                    netSolver = traceFlowSolver as INetSolver;

                }
                else
                {
                    if (gn == null || startNetFlag.Count == 0)
                    {

                        return A4LGSharedFunctions.Localizer.GetString("NoFlagReturnStatement");
                    }
                    traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                    netSolver = traceFlowSolver as INetSolver;

                    netSolver.SourceNetwork = gn.Network;


                }
                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;

                pNetFlags = new List<INetFlag>();

                if (pEdgeFlags != null)
                {
                    foreach (IEdgeFlag pEdFl in pEdgeFlags)
                    {
                        pNetFlags.Add((INetFlag)pEdFl);

                    }
                }
                if (pJunctionFlags != null)
                {

                    foreach (IJunctionFlag pJcFl in pJunctionFlags)
                    {
                        pNetFlags.Add((INetFlag)pJcFl);
                    }
                }
                if (startNetFlag != null)
                {
                    if (startNetFlag.Count > 0)
                    {
                        foreach (INetFlag pNF in startNetFlag)
                        {
                            if (pNF != null)
                            {
                                pNetFlags.Add((INetFlag)pNF);
                            }
                        }
                    }

                    // pNetFlags.Add((INetFlag)startNetFlag);

                }
                if (pNetFlags.Count == 0)
                {
                    return A4LGSharedFunctions.Localizer.GetString("AddFlagOrClickReturnStatement");

                }

                // Stop if user point was not on a visible network feature, old trace results and selection are cleared
                if (gn == null || pNetFlags.Count == 0)
                {

                    return A4LGSharedFunctions.Localizer.GetString("NotIntersectReturnStatement");
                }

                bool fndAsLayer = false;
                pClosestLayer = Globals.FindLayer(app, targetFLName, ref fndAsLayer) as IFeatureLayer;
                if (pClosestLayer == null)
                {
                    return A4LGSharedFunctions.Localizer.GetString("LayerNotFoundReturnStatement");
                }
                if (processEvent)
                {


                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_15a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }



                try
                {
                    Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);

                    traceFlowSolver.FindFlowElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctions, out juncEIDs, out edgeEIDs);
                }
                catch
                {
                    juncEIDs = null;
                    edgeEIDs = null;
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "FindFlowEndElements";
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                    return A4LGSharedFunctions.Localizer.GetString("DontFindFlowEltReturnStatement");
                    //MessageBox.Show("Error in the FindFlowEndElements");
                }

                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.ReturnFeatures = true;

                //Save valves which stopped the trace
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_15b");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                if (pClosestLayer.FeatureClass != null)
                {
                    htClosestAsset = Globals.GetEIDInfoListByFC(pClosestLayer.FeatureClass.FeatureClassID, juncEIDs, eidHelper);

                }

                if (htClosestAsset.Count == 0)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = (A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15b") + targetFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15c"));

                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                        return A4LGSharedFunctions.Localizer.GetString("NoFeaturesReturnStatement");
                    }

                }
                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }


                traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                netSolver = traceFlowSolver as INetSolver;

                netSolver.SourceNetwork = gn.Network;
                ////Globals.AddFlagsToTraceSolver(startNetFlag.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                //Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);
                pNetwork = gn.Network;
                pNetSchema = pNetwork as INetSchema;
                for (int i = 0; i < pNetSchema.WeightCount; i++)
                {
                    pNetWeight = pNetSchema.get_Weight(i);
                    if (pNetWeight.WeightName == sWeightName)
                        break;

                }
                if (pNetWeight != null)
                {
                    if (pNetWeight.WeightType == esriWeightType.esriWTBitGate || pNetWeight.WeightType == esriWeightType.esriWTNull)
                    {
                        pNetWeight = null;
                    }
                }
                //Get trace weights
                if (pNetWeight != null)
                {
                    pNetSolverW = traceFlowSolver as INetSolverWeights;
                    pNetSolverW.JunctionWeight = pNetWeight;
                    pNetSolverW.FromToEdgeWeight = pNetWeight;
                    pNetSolverW.ToFromEdgeWeight = pNetWeight;

                }
                double shortest = 9999999.9;
                int pntAlong = 0;
                foreach (DictionaryEntry entry in htClosestAsset)
                {
                    pntAlong++;
                    ////Set the first junction flag for path finding based this current valve
                    //netFlag1 = new JunctionFlagClass();
                    //netFlag1.UserClassID = valveEIDInfo.Feature.Class.ObjectClassID;
                    //netFlag1.UserID = valveEIDInfo.Feature.OID;
                    //netFlag1.UserSubID = 0;
                    //netFlag1.Label = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_16a");
                    //AddFlagToTraceSolver(netFlag1, ref traceFlowSolver, out junctionFlag, out edgeFlag);
                    // startNetFlag[0].Label = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_16a");
                    //    startNetFlag[0].UserSubID = 0;

                    //Set the second (and last) trace flag at this source
                    //netFlag2 = new JunctionFlagClass();
                    //eidInfo = entry.Value as IEIDInfo;

                    //netFlag2.UserClassID = eidInfo.Feature.Class.ObjectClassID;
                    //netFlag2.UserID = eidInfo.Feature.OID;
                    //netFlag2.UserSubID = 0;
                    //netFlag2.Label = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_16b");

                    eidInfo = entry.Value as IEIDInfo;
                    IPoint pTmpPnt = eidInfo.Feature.Shape as IPoint;

                    //ref point, ref map, ref gn, snapTol, out snappedPoint,
                    //                             out EID, out distanceAlong, out  pFlagDisplay, Flag);
                    bool Flag = true;
                    netFlag2 = Globals.GetEdgeFlagWithGN(ref pTmpPnt, ref map, ref gn, snapTol, out snappedPoint,
                                                 out EID, out distanceAlong, out  pFlagDisplay, Flag) as INetFlag;
                    // Globals.AddTwoJunctionFlagsToTraceSolver(ref traceFlowSolver, netFlag1, netFlag2);
                    Globals.AddTwoJunctionFlagsToTraceSolver(ref traceFlowSolver, startNetFlag[0], netFlag2);


                    //Run trace
                    segCosts = new System.Object[1];
                    segCosts[0] = new System.Object(); edgeEIDs = null;
                    object pTotalCost = null;
                    IEnumNetEID pJuncSel = null;
                    IEnumNetEID pEdgeSel = null;
                    //traceFlowSolver.FindAccumulation(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out pJuncSel, out pEdgeSel,out pTotalCost);
                    traceFlowSolver.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum, out juncEIDs, out edgeEIDs, 1, ref segCosts);
                    if (Convert.ToDouble(segCosts[0]) < shortest)
                    {
                        shortest = Convert.ToDouble(segCosts[0]);
                    }
                    string test = "";


                    // if (edgeEIDs != null && edgeEIDs.Count > 0)
                    // {
                    //     // foundSource = true;
                    //     // break;
                    // }

                }
                MessageBox.Show(shortest.ToString());
                //if (!boolCont)
                //{

                //    pStepPro.Hide();
                //    pProDlg.HideDialog();
                //    pStepPro = null;
                //    pProDlg = null;
                //    pProDFact = null;
                //    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                //}
                //pointAlong++;
                //traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                //traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                //netSolver = traceFlowSolver as INetSolver;
                //netSolver.SourceNetwork = gn.Network;

                ////Globals.AddFlagsToTraceSolver(startNetFlag.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                //Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                //Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);

                ////Set the barriers in the network based on the saved valves
                //netElementBarrier = new SelectionSetBarriersClass();
                //foreach (DictionaryEntry entry in hasSourceValveHT)
                //{
                //    eidInfo = entry.Value as IEIDInfo;

                //    //netElementBarrier.Add(valveFC.FeatureClassID, eidInfo.Feature.OID);
                //    netElementBarrier.Add(((IFeatureClass)eidInfo.Feature.Class).FeatureClassID, eidInfo.Feature.OID);
                //}
                //netSolver.SelectionSetBarriers = netElementBarrier;

                //pointAlong++;
                ////Run last trace
                //traceFlowSolver.FindFlowElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);
                ////skipped valves =>  //Hashtable skippedValvesEIDInfoHT = GetEIDInfoListByFC(valveFC.FeatureClassID, juncEIDs, eidHelper);

                ////Select junction features
                //pointAlong++; //51,44

                ////Open identify dialog with selected features
                ////IdentifySelected(map);
                //if (processEvent)
                //{

                //    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("Complete");
                //    pStepPro.Step();
                //    boolCont = pTrkCan.Continue();
                //}


                //if (!boolCont)
                //{

                //    pStepPro.Hide();
                //    pProDlg.HideDialog();
                //    pStepPro = null;
                //    pProDlg = null;
                //    pProDFact = null;
                //    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                //}

                //if (snappedPoint != null)
                //{
                //    snappedPoint.Project(map.SpatialReference);
                //    // traceRes.TracePoint = snappedPoint;

                //}
                //// return "Post1";

                ////Globals.LoadJunctions(ref traceRes, ref gn, ref map, ref juncEIDs, ref meterDSName);
                //// Globals.LoadValves(ref traceRes, ref gn, ref map, ref hasSourceValveHT);

                ////Globals.LoadEdges(ref traceRes, ref gn, ref map, ref edgeEIDs);
                ////((IMxDocument)app.Document).FocusMap.ClearSelection();
                ////Globals.RemoveGraphics(((IMxDocument)app.Document).FocusMap, false);
                //string returnVal = "";
                //returnVal = Globals.SelectJunctions(ref map, ref gn, ref juncEIDs, ref junctionFlag, MeterName, MeterCritField, MeterCritVal, processEvent);
                //if (processEvent)
                //{
                //    if (selectEdges)
                //        Globals.SelectEdges(ref map, ref  gn, ref edgeEIDs);
                //    else
                //        Globals.DrawEdges(ref map, ref  gn, ref edgeEIDs);
                //}
                //returnVal = Globals.SelectValveJunctions(ref map, ref hasSourceValveHT, ref valveFLs, processEvent) + "_" + returnVal;

                //if (processEvent)
                //{
                //    if (pNetAnalysisExt != null)
                //    {
                //        foreach (IFlagDisplay pFgDi in pFlagsDisplay)
                //        {
                //            Globals.AddFlagToGN(ref pNetAnalysisExt, ref gn, pFgDi);

                //            // Globals.AddPointGraphic(map, pFgDi.Geometry as IPoint, false);
                //        }
                //    }
                //    else
                //    {
                //        foreach (IFlagDisplay pFgDi in pFlagsDisplay)
                //        {
                //            //  Globals.AddFlagToGN(ref pNetAnalysisExt, ref gn,  pFgDi);

                //            Globals.AddPointGraphic(map, pFgDi.Geometry as IPoint, false);
                //        }
                //    }
                //    Globals.GetCommand("esriArcMapUI.ZoomToSelectedCommand", app).Execute();

                //}
                string returnVal = "";
                return returnVal;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
                return ex.Message.ToString();

            }
            finally
            {

                barrierIds = null;
                sourceDirectEIDInfoHT = null;
                netFlag1 = null;
                netFlag2 = null;
                junctionFlags = null;
                edgeFlags = null;
                pID = null;

                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;

                if (gnList != null)
                {
                    //   Marshal.ReleaseComObject(gnList);
                }
                if (gn != null)
                {
                    Marshal.ReleaseComObject(gn);
                }
                if (snappedPoint != null)
                {
                    Marshal.ReleaseComObject(snappedPoint);
                }
                if (startNetFlag != null)
                {
                    //   Marshal.ReleaseComObject(startNetFlag);
                }

                if (junctionFlag != null)
                {
                    Marshal.ReleaseComObject(junctionFlag);
                }
                if (edgeFlag != null)
                {
                    // Marshal.ReleaseComObject(edgeFlag);
                }
                if (traceFlowSolver != null)
                {
                    Marshal.ReleaseComObject(traceFlowSolver);
                }
                if (netSolver != null)
                {
                    Marshal.ReleaseComObject(netSolver);
                }
                if (netElementBarriers != null)
                {
                    Marshal.ReleaseComObject(netElementBarriers);
                }
                if (nb != null)
                {
                    Marshal.ReleaseComObject(nb);
                }
                if (juncEIDs != null)
                {
                    Marshal.ReleaseComObject(juncEIDs);
                }
                if (edgeEIDs != null)
                {
                    Marshal.ReleaseComObject(edgeEIDs);
                }
                if (eidInfo != null)
                {
                    Marshal.ReleaseComObject(eidInfo);
                }
                if (valveEIDInfo != null)
                {
                    Marshal.ReleaseComObject(valveEIDInfo);
                }
                if (sourceEIDInfo != null)
                {
                    Marshal.ReleaseComObject(sourceEIDInfo);
                }
                if (vEIDInfo != null)
                {
                    Marshal.ReleaseComObject(vEIDInfo);
                }
                if (userIds != null)
                {
                    // Marshal.ReleaseComObject(userIds);
                }
                if (eidHelper != null)
                {
                    Marshal.ReleaseComObject(eidHelper);
                }
                if (valveEIDInfoHT != null)
                {
                    //  Marshal.ReleaseComObject(valveEIDInfoHT);
                }
                if (sourceEIDInfoHT != null)
                {
                    //  Marshal.ReleaseComObject(sourceEIDInfoHT);
                }
                if (segCosts != null)
                {
                    //  Marshal.ReleaseComObject(segCosts);
                }
                if (netElementBarrier != null)
                {
                    Marshal.ReleaseComObject(netElementBarrier);
                }
                //if (traceRes != null)
                //{
                //    traceRes.Dispose();
                //    // Marshal.ReleaseComObject(traceRes);
                //}

                pNetAnalysisExt = null;
                pFlagsDisplay = null;
                pEdgeFlags = null;
                pJunctionFlags = null;
                pNetFlags = null;
                pFlagDisplay = null;
                //pEdgeFlagsBar = null;
                //pJunctionFlagsBar = null;



                gnList = null;

                gn = null;
                snappedPoint = null;

                startNetFlag = null;

                junctionFlag = null;
                edgeFlag = null;
                traceFlowSolver = null;
                netSolver = null;
                netElementBarriers = null;
                nb = null;
                juncEIDs = null;
                edgeEIDs = null;
                eidInfo = null;
                valveEIDInfo = null;
                sourceEIDInfo = null;
                vEIDInfo = null;
                userIds = null;

                eidHelper = null;
                valveEIDInfoHT = null;
                sourceEIDInfoHT = null;
                segCosts = null;
                netElementBarrier = null;

                //traceRes = null;
            }
            GC.Collect();
            GC.WaitForFullGCComplete(300);

        }



        public static string TraceIsolation(double[] x, double[] y, IApplication app, string sourceFLName, string valveFLName, string operableFieldNameValves, string operableFieldNameSources,
                                  double snapTol, bool processEvent, string[] opValues, string addSQL, bool traceIndeterminate, bool ZeroSourceCont, bool selectEdges, string MeterName,
                                  string MeterCritField, string MeterCritVal, string closedValveQuery, IFeatureLayer mainsFL, out IPolyline mergedLines, out List<int> lineOIDs, bool addResultsAsLayer)
        {

            mergedLines = null;
            lineOIDs = null;
            IMap map = null;

            List<int> valveFCClassIDs = new List<int>();

            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;

            int pointAlong = 0;

            List<IGeometricNetwork> gnList = null;
            int gnIdx;
            IGeometricNetwork gn = null;
            IPoint snappedPoint = null;
            int EID = -1;
            double distanceAlong;
            List<INetFlag> startNetFlag = null;
            List<IFlagDisplay> pFlagsDisplay = null;
            //Find feature classes
            string[] sourceFLs;
            IFeatureClass[] sourceFC = null;
            IFeatureLayer[] sourceFL = null;
            List<int> sourceFCClassIDs = new List<int>();

            string[] strValveFLs;
            IFeatureLayer pTempLay = null;
            List<IFeatureLayer> valveFLs = null;
            List<IFeatureClass> valveFCs = null;
            //IFeatureLayer meterFL = null;
            // string meterDSName;
            IJunctionFlag[] junctionFlag = null;
            IEdgeFlag[] edgeFlag = null;
            ITraceFlowSolverGEN traceFlowSolver = null;
            INetSolver netSolver = null;
            INetElementBarriersGEN netElementBarriers = null;

            INetElementBarriers nb = null;
            IEnumNetEID juncEIDs = null;
            IEnumNetEID edgeEIDs = null;
            IEnumEIDInfo enumEidInfoJunc = null;
            IEnumEIDInfo enumEidInfoEdge = null;

            IEIDInfo eidInfo = null;
            IEIDInfo valveEIDInfo = null;
            IEIDInfo sourceEIDInfo = null;
            IEIDInfo vEIDInfo = null;
            List<int[]> userIds = null;

            IEIDHelper eidHelper = null;
            List<Hashtable> valveEIDInfoHT = null;
            Hashtable sourceEIDInfoHT = null;
            System.Object[] segCosts = null;
            ISelectionSetBarriers netElementBarrier = null;


            List<IEdgeFlag> pEdgeFlags = null;
            List<IJunctionFlag> pJunctionFlags = null;
            //List<IEdgeFlag> pEdgeFlagsBar = null;
            //List<IJunctionFlag> pJunctionFlagsBar = null;
            INetElementBarriers pEdgeElementBarriers = null;
            INetElementBarriers pJunctionElementBarriers = null;
            ISelectionSetBarriers pSelectionSetBarriers = null;
            List<INetFlag> pNetFlags;
            //ITraceResult traceRes = null;
            IFlagDisplay pFlagDisplay = null;
            INetworkAnalysisExt pNetAnalysisExt = null;
            UID pID = null;

            IJunctionFlag[] junctionFlags = null;
            IEdgeFlag[] edgeFlags = null;

            Hashtable noSourceValveHT = null;
            Hashtable hasSourceValveHT = null;
            List<BarClassIDS> barrierIds = null;
            Hashtable sourceDirectEIDInfoHT = null;
            INetFlag netFlag1 = null;
            INetFlag netFlag2 = null;

            try
            {
                map = ((app.Document as IMxDocument).FocusMap);

                bool boolCont = true;
                if (processEvent)
                {
                    // Create a CancelTracker  
                    pTrkCan = new CancelTrackerClass();
                    // Create the ProgressDialog. This automatically displays the dialog  
                    pProDFact = new ProgressDialogFactoryClass();
                    pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                    // Set the properties of the ProgressDialog  
                    pProDlg.CancelEnabled = true;

                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_16a");
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_16a");


                    pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                    // Set the properties of the Step Progressor  
                    pStepPro = (IStepProgressor)pProDlg;

                    pStepPro.MinRange = 0;
                    pStepPro.MaxRange = 20;
                    pStepPro.StepValue = 1;
                    pStepPro.Position = 0;
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");



                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();

                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                }
                if (processEvent)
                {
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_4");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;


                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref map);
                gnIdx = -1;
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_14a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;

                // Create junction or edge flag at start of trace - also returns geometric network, snapped point, and EID of junction

                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                startNetFlag = new List<INetFlag>();// null;// Globals.GetJunctionFlag(x, y, map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID) as INetFlag;
                //if (startNetFlag == null)
                pFlagsDisplay = new List<IFlagDisplay>();
                if (x != null)
                {
                    for (int l = 0; l < x.Length; l++)
                    {
                        startNetFlag.Add(Globals.GetEdgeFlag(x[l], y[l], ref  map, ref gnList, snapTol, ref gnIdx, out snappedPoint, out EID, out distanceAlong, out pFlagDisplay, true) as INetFlag);
                        pFlagsDisplay.Add(pFlagDisplay);

                    }
                }
                pointAlong++;

                //Set network to trace
                if (gnIdx > -1)
                    gn = gnList[gnIdx] as IGeometricNetwork;


                INetElementBarriersGEN netElementBarriersClose;



                pID = new UID();

                pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                if (gn != null)
                {
                    Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref gn);
                }

                gn = pNetAnalysisExt.CurrentNetwork;


                traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(ref pNetAnalysisExt, out pEdgeFlags, out pJunctionFlags, out pEdgeElementBarriers, out pJunctionElementBarriers, out pSelectionSetBarriers) as ITraceFlowSolverGEN;

                strValveFLs = valveFLName.Split('|');


                valveFLs = new List<IFeatureLayer>();//(IFeatureLayer)Globals.FindLayer(map, valveFLName);
                valveFCs = new List<IFeatureClass>();//[strValveFLs.Length];


                //closedValvesbarrierIds = new List<BarClassIDS>();
                IFeatureCursor pCurValBar = null;
                IFeature valBarFeat = null;
                QueryFilter pQFValBar = new QueryFilterClass();
                pQFValBar = new QueryFilterClass();
                if (closedValveQuery != "")
                {
                    pQFValBar.WhereClause = closedValveQuery;
                }
                //INetFlag closedValveBarr;
                List<int> closeVal = new List<int>();
                List<FeatureOIDs> selectBars = new List<FeatureOIDs>();
                FeatureOIDs featID;
                for (int i = 0; i < strValveFLs.Length; i++)
                {
                    bool FCorLayerTemp = true;

                    pTempLay = (IFeatureLayer)Globals.FindLayerFromMapDataset(map, strValveFLs[i], ref FCorLayerTemp, gn.FeatureDataset);
                    if (pTempLay != null)
                    {

                        if (pTempLay.FeatureClass != null)
                        {
                            valveFLs.Add(pTempLay);
                            valveFCs.Add(pTempLay.FeatureClass);
                            valveFCClassIDs.Add(pTempLay.FeatureClass.FeatureClassID);
                            try
                            {
                                if (closedValveQuery != "")
                                {

                                    pCurValBar = pTempLay.FeatureClass.Search(pQFValBar, true);

                                    while ((valBarFeat = pCurValBar.NextFeature()) != null)
                                    {

                                        //Attempt to use selection barriers
                                        //featID = new FeatureOIDs();
                                        //featID.ClassID = pTempLay.FeatureClass.FeatureClassID;
                                        //featID.ID = valBarFeat.OID;

                                        //selectBars.Add(featID);
                                        //pSelectionSetBarriers.Add(pTempLay.FeatureClass.FeatureClassID, valBarFeat.OID);

                                        //To Use a Edge Barries
                                        try
                                        {
                                            IPoint loc = (valBarFeat.ShapeCopy as IPoint);
                                            closeVal.Add(Globals.getEIDAtLocation(ref loc, ref map, ref gn, snapTol));
                                            loc = null;



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }


                                }
                            }
                            catch
                            { }

                        }
                    }

                }
                //To use closed valves as edge barriers
                if (closeVal.Count > 0)
                {
                    INetworkAnalysisExtBarriers pNetworkAnalysisExtBarriers = null;

                    INetElements pNetElements = gn.Network as INetElements;


                    pNetworkAnalysisExtBarriers = (INetworkAnalysisExtBarriers)pNetAnalysisExt;

                    int lngFlagCount = pNetworkAnalysisExtBarriers.EdgeBarrierCount;

                    //only execute this next bit if there are junction flags
                    if (lngFlagCount != 0)
                    {


                        for (int i = 0; i < lngFlagCount; i++)
                        {

                            pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_EdgeBarrier(i);
                            closeVal.Add(pNetElements.GetEID(pFlagDisplay.FeatureClassID, pFlagDisplay.FID, pFlagDisplay.SubID, esriElementType.esriETEdge));


                        }

                    }


                    netElementBarriersClose = new NetElementBarriersClass() as INetElementBarriersGEN;
                    netElementBarriersClose.ElementType = esriElementType.esriETEdge;
                    //netElementBarriersClose.ElementType = esriElementType.esriETJunction;
                    netElementBarriersClose.Network = gn.Network;

                    int[] clo = closeVal.ToArray();

                    netElementBarriersClose.SetBarriersByEID(ref clo);
                    pEdgeElementBarriers = netElementBarriersClose as INetElementBarriers;



                }
                valBarFeat = null;
                pQFValBar = null;
                if (pCurValBar != null)
                    Marshal.ReleaseComObject(pCurValBar);
                pCurValBar = null;


                netSolver = traceFlowSolver as INetSolver;

                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                pNetFlags = new List<INetFlag>();

                if (pEdgeFlags != null)
                {
                    foreach (IEdgeFlag pEdFl in pEdgeFlags)
                    {
                        pNetFlags.Add((INetFlag)pEdFl);

                    }
                }
                if (pJunctionFlags != null)
                {

                    foreach (IJunctionFlag pJcFl in pJunctionFlags)
                    {
                        pNetFlags.Add((INetFlag)pJcFl);
                    }
                }
                if (startNetFlag != null)
                {
                    if (startNetFlag.Count > 0)
                    {
                        foreach (INetFlag pNF in startNetFlag)
                        {
                            if (pNF != null)
                            {
                                pNetFlags.Add((INetFlag)pNF);
                            }
                        }
                    }

                    // pNetFlags.Add((INetFlag)startNetFlag);

                }
                if (pNetFlags.Count == 0)
                {
                    return A4LGSharedFunctions.Localizer.GetString("AddFlagOrClickReturnStatement");

                }

                // Stop if user point was not on a visible network feature, old trace results and selection are cleared
                if (gn == null || pNetFlags.Count == 0)
                {

                    return A4LGSharedFunctions.Localizer.GetString("NotIntersectReturnStatement");
                }
                pointAlong++;
                if (processEvent)
                {


                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                //Find feature classes
                sourceFLs = sourceFLName.Split('|');
                sourceFC = new IFeatureClass[sourceFLs.Length];
                sourceFL = new IFeatureLayer[sourceFLs.Length];

                for (int i = 0; i < sourceFLs.Length; i++)
                {
                    bool FCorLayerTemp = true;
                    sourceFL[i] = (IFeatureLayer)Globals.FindLayerFromMapDataset(map, sourceFLs[i], ref FCorLayerTemp, gn.FeatureDataset);
                    if (sourceFL[i] != null)
                    {
                        sourceFC[i] = sourceFL[i].FeatureClass;
                        sourceFCClassIDs.Add(sourceFL[i].FeatureClass.FeatureClassID);
                    }

                }
                pointAlong++;
                //                IFeatureClass sourceFC = (IFeatureClass)Globals.GetFeatureClassFromGeometricNetwork(sourceFCName, gn, esriFeatureType.esriFTSimpleJunction);
                // IFeatureClass valveFC = (IFeatureClass)Globals.GetFeatureClassFromGeometricNetwork(valveFCName, gn, esriFeatureType.esriFTSimpleJunction);
                //strValveFLs = valveFLName.Split('|');

                //valveFLs = new List<IFeatureLayer>();//(IFeatureLayer)Globals.FindLayer(map, valveFLName);
                //valveFCs = new List<IFeatureClass>();//[strValveFLs.Length];


                //for (int i = 0; i < strValveFLs.Length; i++)
                //{
                //    bool FCorLayerTemp = true;
                //    pTempLay = (IFeatureLayer)Globals.FindLayer(map, strValveFLs[i], ref FCorLayerTemp);
                //    if (pTempLay != null)
                //    {

                //        if (pTempLay.FeatureClass != null)
                //        {
                //            valveFLs.Add(pTempLay);
                //            valveFCs.Add(pTempLay.FeatureClass);
                //            valveFCClassIDs.Add(pTempLay.FeatureClass.FeatureClassID);



                //        }
                //    }

                //}

                //  string strMeterFL = meterFLName;

                //meterFL = (IFeatureLayer)Globals.FindLayer(map, meterFLName);
                //meterDSName = "";
                //if (meterFL != null)
                //{
                //    if (meterFL is IDataset)
                //    {
                //        IDataset pTempDataset = (IDataset)meterFL;
                //        meterDSName = pTempDataset.BrowseName;
                //        if (meterDSName.Contains("."))
                //        {
                //            meterDSName = meterDSName.Substring(meterDSName.LastIndexOf(".") + 1);

                //        }
                //        Marshal.ReleaseComObject(pTempDataset);

                //        pTempDataset = null;
                //    }

                //}
                //meterFL = null;


                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16a");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;
                if (valveFCs == null)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement"); ;
                    }
                    return A4LGSharedFunctions.Localizer.GetString("LambdaReturnStatement") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                }
                pointAlong++;
                if (valveFCs.Count == 0)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                    return A4LGSharedFunctions.Localizer.GetString("LambdaReturnStatement") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                }

                if (sourceFC == null)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + sourceFLs + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                    return A4LGSharedFunctions.Localizer.GetString("LambdaReturnStatement") + sourceFLs + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                }
                pointAlong++;
                if (sourceFC.Length == 0)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + sourceFLs + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                    return A4LGSharedFunctions.Localizer.GetString("LambdaReturnStatement") + sourceFLs + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15e") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15f");
                }

                pointAlong++;

                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16b");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }


                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlags, out edgeFlags);


                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16c");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }


                //Create barriers based on all operable valves
                pointAlong++;
                netElementBarriers = new NetElementBarriersClass() as INetElementBarriersGEN;
                netElementBarriers.ElementType = esriElementType.esriETJunction;
                netElementBarriers.Network = gn.Network;

                userIds = Globals.GetOperableValveOIDs(valveFCs.ToArray(), operableFieldNameValves, opValues, addSQL);
                if (userIds == null)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16d");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                }

                else
                {
                    if (userIds.Count == 0)
                    {
                        if (processEvent)
                        {

                            pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16d");
                            pStepPro.Step();
                            boolCont = pTrkCan.Continue();
                        }


                        if (!boolCont)
                        {

                            pStepPro.Hide();
                            pProDlg.HideDialog();
                            pStepPro = null;
                            pProDlg = null;
                            pProDFact = null;
                            return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                        }
                    }
                    else
                    {
                        try
                        {
                            int idxUser = 0;
                            if (processEvent)
                            {

                                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16e");
                                pStepPro.Step();
                                boolCont = pTrkCan.Continue();
                            }


                            if (!boolCont)
                            {

                                pStepPro.Hide();
                                pProDlg.HideDialog();
                                pStepPro = null;
                                pProDlg = null;
                                pProDFact = null;
                                return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                            }
                            foreach (IFeatureClass valveFC in valveFCs)
                            {
                                int[] usrid = userIds[idxUser];
                                if (usrid.Length > 0)
                                {
                                    netElementBarriers.SetBarriers(valveFC.FeatureClassID, ref usrid);  //error here after sum
                                    nb = netElementBarriers as INetElementBarriers;
                                    netSolver.set_ElementBarriers(esriElementType.esriETJunction, nb);
                                }
                                idxUser++;
                            }
                        }
                        catch (Exception Ex)
                        {
                            if (processEvent)
                            {



                                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_16a") + Ex.Message;
                                pStepPro.Step();
                                boolCont = pTrkCan.Continue();
                                return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                            }


                            if (!boolCont)
                            {

                                pStepPro.Hide();
                                pProDlg.HideDialog();
                                pStepPro = null;
                                pProDlg = null;
                                pProDFact = null;
                                return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                            }
                        }
                    }
                }
                pointAlong++;
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16f");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                try
                {
                    juncEIDs = null;
                    edgeEIDs = null;
                    traceFlowSolver.FindFlowEndElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);
                }
                catch (Exception ex)
                {
                    try
                    {
                        traceFlowSolver.FindFlowEndElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);
                    }
                    catch (Exception ex2)
                    {
                        juncEIDs = null;
                        edgeEIDs = null;
                        if (processEvent)
                        {

                            pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "FindFlowEndElements";
                            pStepPro.Step();
                            boolCont = pTrkCan.Continue();
                        }


                        if (!boolCont)
                        {

                            pStepPro.Hide();
                            pProDlg.HideDialog();
                            pStepPro = null;
                            pProDlg = null;
                            pProDFact = null;
                            return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                        }
                        return A4LGSharedFunctions.Localizer.GetString("DontFindFlowEltReturnStatement");
                    }
                    //MessageBox.Show("Error in the FindFlowEndElements");
                }
                pointAlong++;
                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.ReturnFeatures = true;

                //Save valves which stopped the trace
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16g");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;

                valveEIDInfoHT = new List<Hashtable>();
                int totalreachcount = 0;
                foreach (IFeatureClass valveFC in valveFCs)
                {
                    if (valveFC != null)
                    {
                        Hashtable valveEIDInfoHTtemp = Globals.GetEIDInfoListByFC(valveFC.FeatureClassID, juncEIDs, eidHelper);
                        totalreachcount = totalreachcount + valveEIDInfoHTtemp.Count;
                        valveEIDInfoHT.Add(valveEIDInfoHTtemp);
                    }

                }
                if (totalreachcount == 0)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = (A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16a") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15c") + Environment.NewLine +
                            Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16d") + Environment.NewLine +
                            A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16e"));
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        //return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }

                    //return A4LGSharedFunctions.Localizer.GetString("Error") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16a") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15c") + Environment.NewLine +
                    //        Environment.NewLine +
                    //        A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + valveFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                    //        A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16d") + Environment.NewLine +
                    //        A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16e");
                }
                pointAlong++;
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16h");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }


                traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                netSolver = traceFlowSolver as INetSolver;

                netSolver.SourceNetwork = gn.Network;
                //Globals.AddFlagsToTraceSolver(startNetFlag.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);


                pointAlong++;
                foreach (int sFC in sourceFCClassIDs)
                {
                    netSolver.DisableElementClass(sFC);
                }

                traceFlowSolver.FindFlowEndElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctions, out juncEIDs, out edgeEIDs);
                pointAlong++;
                //Save sources which are reachable
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16i");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                sourceEIDInfoHT = new Hashtable();

                //foreach (IFeatureClass sFC in sourceFC)
                //{
                Globals.GetEIDInfoListByFCWithHT(ref sourceEIDInfoHT, sourceFCClassIDs, operableFieldNameSources, opValues, juncEIDs, eidHelper);

                //}
                pointAlong++;
                if (sourceEIDInfoHT.Count == 0 && totalreachcount > 0)
                {
                    if (processEvent)
                    {


                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("No") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15c") + Environment.NewLine +
                         Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16j");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return null;
                    }
                    return A4LGSharedFunctions.Localizer.GetString("Error") + A4LGSharedFunctions.Localizer.GetString("No") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15c") + Environment.NewLine +
                         Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16j");
                }
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16k");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return null;
                }

                traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                netSolver = traceFlowSolver as INetSolver;
                netSolver.SourceNetwork = gn.Network;
                //Globals.AddFlagsToTraceSolver(startNetFlag.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);

                pointAlong++;
                //Set the barriers in the network based on the saved valves
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16l");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }



                //Run the trace to find directly reachable sources (without passing valve)
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16m");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }

                netElementBarrier = new SelectionSetBarriersClass();


                totalreachcount = 0;
                foreach (Hashtable HTentry in valveEIDInfoHT)
                {
                    foreach (DictionaryEntry entry in HTentry)
                    {
                        eidInfo = entry.Value as IEIDInfo;
                        netElementBarrier.Add(eidInfo.Feature.Class.ObjectClassID, eidInfo.Feature.OID);
                    }
                    //totalreachcount++;
                }

                //attempt to use selection barriers for closed valves
                //foreach (FeatureOIDs featOIDS in selectBars)
                //{
                //    netElementBarrier.Add(featOIDS.ClassID, featOIDS.ID);
                //}
                pointAlong++;
                netSolver.SelectionSetBarriers = netElementBarrier;
                pointAlong++;
                Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);

                traceFlowSolver.FindFlowElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);
                // Hashtable sourceDirectEIDInfoHT = Globals.GetEIDInfoListByFC(sourceFC.FeatureClassID, juncEIDs, eidHelper);
                sourceDirectEIDInfoHT = new Hashtable();


                Globals.GetEIDInfoListByFCWithHT(ref sourceDirectEIDInfoHT, sourceFCClassIDs, operableFieldNameSources, opValues, juncEIDs, eidHelper);





                //Remove directly reachable sources from source array
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16n");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;
                foreach (DictionaryEntry entry in sourceDirectEIDInfoHT)
                {
                    eidInfo = entry.Value as IEIDInfo;
                    sourceEIDInfoHT.Remove(eidInfo.Feature.OID);
                }
                pointAlong++;//21
                if (sourceEIDInfoHT.Count == 0 && totalreachcount > 0)
                {
                    if (processEvent)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("No") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16o") + Environment.NewLine +
                         Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16j");
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();
                    }


                    if (!boolCont)
                    {

                        pStepPro.Hide();
                        pProDlg.HideDialog();
                        pStepPro = null;
                        pProDlg = null;
                        pProDFact = null;
                        return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                    }
                    return A4LGSharedFunctions.Localizer.GetString("Error") + A4LGSharedFunctions.Localizer.GetString("No") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16o") + Environment.NewLine +
                         Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16b") + sourceFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_16c") + Environment.NewLine +
                         A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16j");
                }
                pointAlong++;//22
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16p");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }


                noSourceValveHT = new Hashtable();
                hasSourceValveHT = new Hashtable();

                // IEIDInfo vEIDInfo;
                barrierIds = null;

                bool foundSource;
                // ArrayList barrierArrayList;
                pointAlong++;
                totalreachcount = 0;
                foreach (Hashtable valveHT in valveEIDInfoHT)
                {
                    foreach (DictionaryEntry valveEntry in valveHT)
                    {

                        foundSource = false;
                        valveEIDInfo = valveEntry.Value as IEIDInfo;


                        //Create array of all isolation valves excluding the current one
                        // barrierArrayList = new ArrayList();
                        barrierIds = new List<BarClassIDS>();

                        foreach (Hashtable valveHTBar in valveEIDInfoHT)
                        {
                            BarClassIDS barClID = new BarClassIDS();
                            List<int> tempIntArr = new List<int>();

                            foreach (DictionaryEntry valveEntryBar in valveHTBar)
                            {
                                vEIDInfo = valveEntryBar.Value as IEIDInfo;

                                barClID.ClassID = vEIDInfo.Feature.Class.ObjectClassID;

                                vEIDInfo = valveEntryBar.Value as IEIDInfo;
                                if (vEIDInfo.Feature.OID == valveEIDInfo.Feature.OID && vEIDInfo.Feature.Class.ObjectClassID == valveEIDInfo.Feature.Class.ObjectClassID)
                                {

                                }
                                else
                                {

                                    //  barrierArrayList.Add(vEIDInfo.Feature.OID);
                                    tempIntArr.Add(vEIDInfo.Feature.OID);
                                    //barrierIds.Add(vEIDInfo.Feature.OID);
                                }

                            }
                            barClID.IDs = tempIntArr.ToArray();
                            barrierIds.Add(barClID);
                        }
                        //if (valveHT.Count > 1)
                        //{

                        //    barrierArrayList = new ArrayList();
                        //    barrierIds = new int[valveHT.Count - 1];
                        //    foreach (DictionaryEntry v in valveHT)
                        //    {
                        //        vEIDInfo = v.Value as IEIDInfo;
                        //        if (vEIDInfo.Feature.OID != valveEIDInfo.Feature.OID)
                        //        {
                        //            barrierArrayList.Add(vEIDInfo.Feature.OID);
                        //        }
                        //    }
                        //    barrierArrayList.CopyTo(barrierIds);
                        //}
                        //else
                        //    barrierArrayList = null;

                        pointAlong++;
                        //For each source, attempt to trace

                        foreach (DictionaryEntry sourceEntry in sourceEIDInfoHT)
                        {

                            sourceEIDInfo = sourceEntry.Value as IEIDInfo;

                            //Setup trace to test each valve
                            traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                            traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                            netSolver = traceFlowSolver as INetSolver;
                            netSolver.SourceNetwork = gn.Network;
                            Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);

                            //Set the first junction flag for path finding based this current valve
                            netFlag1 = new JunctionFlagClass();
                            netFlag1.UserClassID = valveEIDInfo.Feature.Class.ObjectClassID;
                            netFlag1.UserID = valveEIDInfo.Feature.OID;
                            netFlag1.UserSubID = 0;
                            netFlag1.Label = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_16a");
                            //AddFlagToTraceSolver(netFlag1, ref traceFlowSolver, out junctionFlag, out edgeFlag);

                            //Set the second (and last) trace flag at this source
                            netFlag2 = new JunctionFlagClass();
                            netFlag2.UserClassID = sourceEIDInfo.Feature.Class.ObjectClassID;
                            netFlag2.UserID = sourceEIDInfo.Feature.OID;
                            netFlag2.UserSubID = 0;
                            netFlag2.Label = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_16b");

                            Globals.AddTwoJunctionFlagsToTraceSolver(ref traceFlowSolver, netFlag1, netFlag2);


                            //Set as isolation valves (except the current one) as barriers
                            if (barrierIds != null && barrierIds.Count > 0)
                            {
                                netElementBarriers = new NetElementBarriersClass() as INetElementBarriersGEN;
                                netElementBarriers.ElementType = esriElementType.esriETJunction;
                                netElementBarriers.Network = gn.Network;
                                bool setBar = false;
                                foreach (BarClassIDS tempBarIDS in barrierIds)
                                {
                                    if (tempBarIDS.IDs.Length > 0)
                                    {
                                        int[] barIDs = tempBarIDS.IDs;

                                        netElementBarriers.SetBarriers(tempBarIDS.ClassID, ref barIDs);
                                        setBar = true;
                                    }
                                }
                                if (setBar)//required, it would produce an error if there where no other barriers
                                {
                                    nb = netElementBarriers as INetElementBarriers;
                                    netSolver.set_ElementBarriers(esriElementType.esriETJunction, nb);
                                }
                            }

                            //Run trace
                            segCosts = new System.Object[1];
                            segCosts[0] = new System.Object(); edgeEIDs = null;
                            traceFlowSolver.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum, out juncEIDs, out edgeEIDs, 1, ref segCosts);
                            if (edgeEIDs != null && edgeEIDs.Count > 0)
                            {
                                foundSource = true;
                                break;
                            }

                        } // End of source loop
                        pointAlong++;//25 -30ish
                        if (foundSource)
                        {
                            hasSourceValveHT.Add(valveEIDInfo.EID, valveEIDInfo);//valveEIDInfo.Feature.Class.ObjectClassID +":" + 
                        }
                        else
                        {
                            noSourceValveHT.Add(valveEIDInfo.EID, valveEIDInfo);
                            //Set the second (and last) trace flag at this source
                            //netFlag2 = new JunctionFlagClass();
                            //netFlag2.UserClassID = valveEIDInfo.Feature.Class.ObjectClassID;
                            //netFlag2.UserID = valveEIDInfo.Feature.OID;
                            //netFlag2.UserSubID = 0;
                            //pNetFlags.Add(netFlag2);
                        }

                    } // End of valve loop
                    //totalreachcount++;
                }
                //Setup last trace with correct valve barriers

                if (hasSourceValveHT.Count == 0)
                {
                    if (ZeroSourceCont)
                        hasSourceValveHT = noSourceValveHT;
                    else
                        return A4LGSharedFunctions.Localizer.GetString("NoWaterSourceIdentifiedReturnStatement");

                }
                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_16q");
                    pStepPro.Step();
                    boolCont = pTrkCan.Continue();
                }


                if (!boolCont)
                {

                    pStepPro.Hide();
                    pProDlg.HideDialog();
                    pStepPro = null;
                    pProDlg = null;
                    pProDFact = null;
                    return A4LGSharedFunctions.Localizer.GetString("CanceledReturnStatement");
                }
                pointAlong++;
                traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;
                netSolver = traceFlowSolver as INetSolver;
                netSolver.SourceNetwork = gn.Network;

                //Globals.AddFlagsToTraceSolver(startNetFlag.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlag, out edgeFlag);
                INetElementBarriers nullTemp = null;
                Globals.AddBarriersToSolver(ref traceFlowSolver, ref pEdgeElementBarriers, ref pJunctionElementBarriers, ref pSelectionSetBarriers);

                //Set the barriers in the network based on the saved valves

                netElementBarrier = new SelectionSetBarriersClass();

                foreach (DictionaryEntry entry in hasSourceValveHT)
                {
                    eidInfo = entry.Value as IEIDInfo;

                    //netElementBarrier.Add(valveFC.FeatureClassID, eidInfo.Feature.OID);
                    netElementBarrier.Add(((IFeatureClass)eidInfo.Feature.Class).FeatureClassID, eidInfo.Feature.OID);
                }
                //attempt to use selection barriers for closed valves
                //foreach (FeatureOIDs featOIDS in selectBars)
                //{
                //    netElementBarrier.Add(featOIDS.ClassID, featOIDS.ID);
                //}
                netSolver.SelectionSetBarriers = netElementBarrier;

                pointAlong++;
                //Run last trace
                traceFlowSolver.FindFlowElements(esriFlowMethod.esriFMConnected, esriFlowElements.esriFEJunctionsAndEdges, out juncEIDs, out edgeEIDs);
                //skipped valves =>  //Hashtable skippedValvesEIDInfoHT = GetEIDInfoListByFC(valveFC.FeatureClassID, juncEIDs, eidHelper);

                //Select junction features
                pointAlong++; //51,44

                //Open identify dialog with selected features
                //IdentifySelected(map);
             
                if (snappedPoint != null)
                {
                    snappedPoint.Project(map.SpatialReference);
                    // traceRes.TracePoint = snappedPoint;

                }
                // return "Post1";

                //Globals.LoadJunctions(ref traceRes, ref gn, ref map, ref juncEIDs, ref meterDSName);
                // Globals.LoadValves(ref traceRes, ref gn, ref map, ref hasSourceValveHT);

                //Globals.LoadEdges(ref traceRes, ref gn, ref map, ref edgeEIDs);
                //((IMxDocument)app.Document).FocusMap.ClearSelection();
                //Globals.RemoveGraphics(((IMxDocument)app.Document).FocusMap, false);

                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;

                //eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = true;
                eidHelper.PartialComplexEdgeGeometry = true;



                if (processEvent)
                {
                    if (pNetAnalysisExt != null)
                    {
                        foreach (IFlagDisplay pFgDi in pFlagsDisplay)
                        {
                            Globals.AddFlagToGN(ref pNetAnalysisExt, ref gn, pFgDi);

                            // Globals.AddPointGraphic(map, pFgDi.Geometry as IPoint, false);
                        }
                    }
                    else
                    {
                        foreach (IFlagDisplay pFgDi in pFlagsDisplay)
                        {
                            //  Globals.AddFlagToGN(ref pNetAnalysisExt, ref gn,  pFgDi);

                            Globals.AddPointGraphic(map, pFgDi.Geometry as IPoint, false);
                        }
                    }
                }

                enumEidInfoJunc = eidHelper.CreateEnumEIDInfo(juncEIDs);
                enumEidInfoEdge = eidHelper.CreateEnumEIDInfo(edgeEIDs);

                int totalCount = hasSourceValveHT.Count;


                totalCount = totalCount + enumEidInfoJunc.Count;
                totalCount = totalCount + enumEidInfoEdge.Count;
                int totalPrompt = ConfigUtil.GetConfigValue("Trace_ResultTotalPrompt", 2000);

                if (totalCount > totalPrompt)
                {
                    DialogResult msgResult = MessageBox.Show(String.Format(A4LGSharedFunctions.Localizer.GetString("TraceResultsCount"), totalCount), A4LGSharedFunctions.Localizer.GetString("Proceed"), MessageBoxButtons.YesNo);
                    if (msgResult == DialogResult.No)
                    {
                        return "";
                    }

                }

                string returnVal = "";
                returnVal = Globals.SelectJunctions(ref map, ref gn, ref juncEIDs, ref junctionFlag, MeterName, MeterCritField, MeterCritVal, processEvent);
                if (processEvent)
                {
                    if (selectEdges)
                        Globals.SelectEdges(ref map, ref  gn, ref edgeEIDs);
                    else
                        Globals.DrawEdges(ref map, ref  gn, ref edgeEIDs);
                }
                mergedLines = Globals.MergeEdges(ref map, ref  gn, ref edgeEIDs, ref mainsFL, out lineOIDs);

                returnVal = Globals.SelectValveJunctions(ref map, ref hasSourceValveHT, ref valveFLs, processEvent) + "_" + returnVal;

                if (processEvent)
                {

                    Globals.GetCommand("esriArcMapUI.ZoomToSelectedCommand", app).Execute();

                }
                if (addResultsAsLayer)
                {


                    Globals.TraceResultsToLayer(ref app, ref  gn, ref enumEidInfoJunc, ref enumEidInfoEdge, ref hasSourceValveHT, ref valveFLs);

                }
                ((IMxDocument)app.Document).UpdateContents();
                return returnVal;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
                return ex.Message.ToString() + "\n" + pointAlong;

            }
            finally
            {

                if (processEvent)
                {

                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("Complete");
                    pStepPro.Step();
                   
                }



                barrierIds = null;
                sourceDirectEIDInfoHT = null;
                netFlag1 = null;
                netFlag2 = null;
                junctionFlags = null;
                edgeFlags = null;
                pID = null;
                noSourceValveHT = null;
                hasSourceValveHT = null;

                if (pProDlg != null)
                {

                    pProDlg.HideDialog();
                }
                pStepPro = null;
                pProDlg = null;
                pProDFact = null;

                if (gnList != null)
                {
                    //   Marshal.ReleaseComObject(gnList);
                }
                if (gn != null)
                {
                    Marshal.ReleaseComObject(gn);
                }
                if (snappedPoint != null)
                {
                    Marshal.ReleaseComObject(snappedPoint);
                }
                if (startNetFlag != null)
                {
                    //   Marshal.ReleaseComObject(startNetFlag);
                }
                if (sourceFC != null)
                {

                    //  Marshal.ReleaseComObject(sourceFC);
                }
                if (sourceFL != null)
                {
                    //  Marshal.ReleaseComObject(sourceFL);
                }
                if (pTempLay != null)
                {
                    Marshal.ReleaseComObject(pTempLay);
                }
                if (valveFLs != null)
                {
                    // Marshal.ReleaseComObject(valveFLs);
                }
                if (valveFCs != null)
                {
                    // Marshal.ReleaseComObject(valveFCs);
                }
                //if (meterFL != null)
                //{
                //    Marshal.ReleaseComObject(meterFL);
                //}
                if (junctionFlag != null)
                {
                    // Marshal.ReleaseComObject(junctionFlag);
                }
                if (edgeFlag != null)
                {
                    // Marshal.ReleaseComObject(edgeFlag);
                }
                if (traceFlowSolver != null)
                {
                    Marshal.ReleaseComObject(traceFlowSolver);
                }
                if (netSolver != null)
                {
                    Marshal.ReleaseComObject(netSolver);
                }
                if (netElementBarriers != null)
                {
                    Marshal.ReleaseComObject(netElementBarriers);
                }
                if (nb != null)
                {
                    Marshal.ReleaseComObject(nb);
                }
                if (juncEIDs != null)
                {
                    Marshal.ReleaseComObject(juncEIDs);
                }
                if (edgeEIDs != null)
                {
                    Marshal.ReleaseComObject(edgeEIDs);
                }
                if (eidInfo != null)
                {
                    Marshal.ReleaseComObject(eidInfo);
                }
                if (valveEIDInfo != null)
                {
                    Marshal.ReleaseComObject(valveEIDInfo);
                }
                if (sourceEIDInfo != null)
                {
                    Marshal.ReleaseComObject(sourceEIDInfo);
                }
                if (enumEidInfoJunc != null)
                {
                    Marshal.ReleaseComObject(enumEidInfoJunc);
                }
                if (enumEidInfoEdge != null)
                {
                    Marshal.ReleaseComObject(enumEidInfoEdge);
                }


                if (vEIDInfo != null)
                {
                    Marshal.ReleaseComObject(vEIDInfo);
                }
                if (userIds != null)
                {
                    // Marshal.ReleaseComObject(userIds);
                }
                if (eidHelper != null)
                {
                    Marshal.ReleaseComObject(eidHelper);
                }
                if (valveEIDInfoHT != null)
                {
                    //  Marshal.ReleaseComObject(valveEIDInfoHT);
                }
                if (sourceEIDInfoHT != null)
                {
                    //  Marshal.ReleaseComObject(sourceEIDInfoHT);
                }
                if (segCosts != null)
                {
                    //  Marshal.ReleaseComObject(segCosts);
                }
                if (netElementBarrier != null)
                {
                    Marshal.ReleaseComObject(netElementBarrier);
                }
                //if (traceRes != null)
                //{
                //    traceRes.Dispose();
                //    // Marshal.ReleaseComObject(traceRes);
                //}

                pNetAnalysisExt = null;
                pFlagsDisplay = null;
                pEdgeFlags = null;
                pJunctionFlags = null;
                pNetFlags = null;
                pFlagDisplay = null;
                //pEdgeFlagsBar = null;
                //pJunctionFlagsBar = null;



                gnList = null;

                gn = null;
                snappedPoint = null;

                startNetFlag = null;

                sourceFC = null;
                sourceFL = null;

                pTempLay = null;
                valveFLs = null;
                valveFCs = null;
                //meterFL = null;

                junctionFlag = null;
                edgeFlag = null;
                traceFlowSolver = null;
                netSolver = null;
                netElementBarriers = null;
                nb = null;
                juncEIDs = null;
                edgeEIDs = null;
                eidInfo = null;
                valveEIDInfo = null;
                sourceEIDInfo = null;
                vEIDInfo = null;
                userIds = null;

                eidHelper = null;
                valveEIDInfoHT = null;
                sourceEIDInfoHT = null;
                segCosts = null;
                netElementBarrier = null;

                //traceRes = null;
            }
            GC.Collect();
            GC.WaitForFullGCComplete(300);

        }

        public static string TracePath(double[] Xs, double[] Ys, string GeoNetName, IApplication app, IMap map, bool traceIndeterminate,
                                   double snapTol, bool selectEdges, out IEnumNetEID juncEIDs, out IEnumNetEID edgeEIDs, out IGeometricNetwork gn)
        {

            gn = null;

            juncEIDs = null;
            edgeEIDs = null;

            List<INetFlag> pNetFlags = null;
            IFlagDisplay pFlagDisplay = null;

            IJunctionFlag[] junctionFlags = null;
            IEdgeFlag[] edgeFlags = null;


            ITraceFlowSolverGEN traceFlowSolver = null;

            List<IEdgeFlag> pEdgeFlags = null;
            List<IJunctionFlag> pJunctionFlags = null;
            INetElementBarriers pEdgeElementBarriers;
            INetElementBarriers pJunctionElementBarriers;
            ISelectionSetBarriers pSelectionSetBarriers;
            List<IGeometricNetwork> gnList = null;
            INetworkAnalysisExt pNetAnalysisExt = null;
            UID pID = null;
            try
            {
                gnList = Globals.GetGeometricNetworks(ref map);
                if (gnList == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_17a"));
                    return "";
                }

                int gnIdx = -1;

                gnIdx = Globals.GetGeometricNetwork(ref gnList, GeoNetName);
                if (gnIdx != -1)
                {
                    gn = (IGeometricNetwork)gnList[gnIdx];
                }


                pNetFlags = new List<INetFlag>();

                //  INetFlag[] netFlags = new INetFlag[Xs.Length];

                for (int i = 0; i < Xs.Length; i++)
                {
                    IPoint snappedPoint = null; int EID = -1;
                    if (gn == null)
                    {

                        pNetFlags.Add(Globals.GetJunctionFlag(Xs[i], Ys[i], ref map, ref gnList, snapTol, ref gnIdx,
                                    out snappedPoint, out EID, out  pFlagDisplay, true) as INetFlag);

                        //Set network to trace
                        if (gnIdx > -1)
                            gn = gnList[gnIdx] as IGeometricNetwork;


                    }
                    else
                    {
                        pNetFlags.Add(Globals.GetJunctionFlagWithGN(Xs[i], Ys[i], ref map, ref gn, ref snapTol, out snappedPoint, out EID, out  pFlagDisplay, true) as INetFlag);

                    }
                }

                if (gn == null || pNetFlags.Count < 1)
                {
                    return (A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_17b"));

                }

                if (app != null)
                {

                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);
                    Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref gn);
                    traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(ref pNetAnalysisExt, out pEdgeFlags, out pJunctionFlags, out pEdgeElementBarriers, out pJunctionElementBarriers, out pSelectionSetBarriers) as ITraceFlowSolverGEN;

                }
                else
                {
                    traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                    INetSolver netSolver = traceFlowSolver as INetSolver;

                    netSolver.SourceNetwork = gn.Network;


                }



                traceFlowSolver.TraceIndeterminateFlow = traceIndeterminate;




                if (pEdgeFlags != null)
                {
                    foreach (IEdgeFlag pEdFl in pEdgeFlags)
                    {
                        pNetFlags.Add((INetFlag)pEdFl);

                    }
                }
                if (pJunctionFlags != null)
                {

                    foreach (IJunctionFlag pJcFl in pJunctionFlags)
                    {
                        pNetFlags.Add((INetFlag)pJcFl);
                    }
                }
                //if (startNetFlag != null)
                //{


                //    pNetFlags.Add((INetFlag)startNetFlag);

                //}

                Globals.AddFlagsToTraceSolver(pNetFlags.ToArray(), ref traceFlowSolver, out junctionFlags, out edgeFlags);
                int intCount = 1;
                System.Object[] segCosts = new System.Object[1];
                segCosts[0] = new System.Object();

                traceFlowSolver.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinMax,
                    out juncEIDs, out edgeEIDs, intCount, ref segCosts);

                //Select junction features
                map.ClearSelection();
                IJunctionFlag[] junctionFlag = null;
                Globals.SelectJunctions(ref map, ref  gn, ref juncEIDs, ref junctionFlag, "", "", "", true);
                if (selectEdges)
                    Globals.SelectEdges(ref map, ref gn, ref edgeEIDs);
                edgeEIDs.Reset();

                //Draw edge graphics
                IEnvelope env = Globals.DrawEdges(ref map, ref gn, ref edgeEIDs);

                return edgeEIDs.Count.ToString();


            }
            catch (Exception Ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_17a") + "\n" + Ex.Message);
                return "";

            }
        }
        public static void TraceIsolationSummary(IApplication app, string sourceFLName, string valveFLName, string operableFieldNameValve, string operableFieldNameSource,
                                double snapTol, bool processEvent, string[] opValues, string addSQL, bool traceIndeterminate, bool ZeroSourceCont,
                                    string mainsFLName, string meterFLName, string metersCritFieldName, string metersCritValue,
                                string traceSum_LayerName, string traceSum_FacilityIDField, string traceSum_DateFieldName, string traceSum_ValveCountFieldName,
                                    string traceSum_MeterCountFieldName, string traceSum_CritMeterCountFieldName, string traceSum_CommentsFieldName,
                                    bool saveEntireLine)
        {
            IFeatureLayer mainsFL = null;

            IFeatureLayer resultsLayer = null;
            IFeatureClass mainFC = null;
            INetworkClass mainsNetwork = null;
            IFeatureClass resultsFC = null;
            IFeatureSelection mainsFS = null;
            IFeature pMainsFeat = null;
            IProgressDialogFactory pProDFact = null;
            IStepProgressor pStepPro = null;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan = null;
            IWorkspaceEdit pWSEdit = null;
            IFeatureCursor pFC = null;
            IFeatureCursor pInsCur = null;
            // IFeatureClassLoad featureClassLoad = null;
            // ISchemaLock schemaLock = null;
            IFeatureBuffer pSumFeatBuf = null;
            IEnumIDs pSelectIDs = null;

            ICurve pCurve = null;
            IPoint pPnt = null;
            IDataset pDS = null;

            int facilityIDFieldPosition;

            int resultsFacilityIDFieldPosition;
            //int resultsSourceIDFieldPosition;
            int resultsDateFieldPosition;
            int resultsValveCountFieldPosition;
            int resultsMeterCountFieldPosition;
            int resultsCritMeterCountFieldPosition;
            int resultsCommentsFieldPosition;


            IGeometryDef pGeometryDefTest = null;
            IFields pFieldsTest = null;
            IField pFieldTest = null;

            bool bZAware;
            bool bMAware;
            ESRI.ArcGIS.Geometry.IZAware zAware;

            try
            {

                bool FCorLayerMains = true;
                mainsFL = (IFeatureLayer)Globals.FindLayer(((IMxDocument)app.Document).FocusMap, mainsFLName, ref FCorLayerMains);
                if (mainsFL == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + mainsFLName + " feature class was not found.\r\n Check the TraceIsolationSummary_Main_FeatureLayer tag in the config");
                    return;
                }
                mainFC = mainsFL.FeatureClass;

                //Determine field position for facility id
                facilityIDFieldPosition = mainFC.Fields.FindField("FACILITYID");
                if (facilityIDFieldPosition == -1)
                    facilityIDFieldPosition = mainFC.Fields.FindField("FACID");
                if (facilityIDFieldPosition == -1)
                    facilityIDFieldPosition = mainFC.Fields.FindField("ASSETID");
                if (facilityIDFieldPosition == -1)
                    facilityIDFieldPosition = mainFC.Fields.FindField("LEGACYID");
                if (facilityIDFieldPosition == -1)
                    facilityIDFieldPosition = mainFC.Fields.FindField(mainFC.OIDFieldName);
                if (facilityIDFieldPosition == -1)
                    return;


                mainsNetwork = mainFC as INetworkClass;
                if ((mainsNetwork == null) || (mainsNetwork.GeometricNetwork == null))
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + mainsFLName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18a"));
                    return;
                }



                mainsFS = (IFeatureSelection)mainsFL;
                int FeatureCount = 0;
                if (mainsFS.SelectionSet.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("No") + mainsFL.Name + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18b"),
                        A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"), MessageBoxButtons.OK);

                    return;

                }
                FeatureCount = mainsFS.SelectionSet.Count;

                bool boolCont = true;
                // Create a CancelTracker  
                pTrkCan = new CancelTrackerClass();
                // Create the ProgressDialog. This automatically displays the dialog  
                pProDFact = new ProgressDialogFactoryClass();
                pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);


                // Set the properties of the ProgressDialog  
                pProDlg.CancelEnabled = true;
                pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_18a");
                pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_18a");
                pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                // Set the properties of the Step Progressor  
                pStepPro = (IStepProgressor)pProDlg;

                pStepPro.MinRange = 1;
                pStepPro.MaxRange = FeatureCount - 1;
                pStepPro.StepValue = 1;
                pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_18a");


                // Get Results feature class and test its schema
                bool FCorLayerResults = true;
                resultsLayer = (IFeatureLayer)Globals.FindLayer(((IMxDocument)app.Document).FocusMap, traceSum_LayerName, ref FCorLayerResults);
                if (resultsLayer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + traceSum_LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18c"));
                    return;
                }
                if (resultsLayer.FeatureClass == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + traceSum_LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18d"));
                    return;
                }
                resultsFC = resultsLayer.FeatureClass;


                pDS = (IDataset)resultsFC;
                pWSEdit = (IWorkspaceEdit)pDS.Workspace;
                pDS = null;
                bool alreadyEditing = pWSEdit.IsBeingEdited();

                //        if (alreadyEditing != true)
                //            pWSEdit.StartEditing(true);

                //        pWSEdit.StartEditOperation(
                if (alreadyEditing == false)
                {

                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_15d") + traceSum_LayerName + " layer must be editable.");
                    return;

                }
                //  resultsFC = resultsLayer.FeatureClass;
                resultsFacilityIDFieldPosition = resultsFC.Fields.FindField(traceSum_FacilityIDField);
                resultsDateFieldPosition = resultsFC.Fields.FindField(traceSum_DateFieldName);
                resultsValveCountFieldPosition = resultsFC.Fields.FindField(traceSum_ValveCountFieldName);
                resultsMeterCountFieldPosition = resultsFC.Fields.FindField(traceSum_MeterCountFieldName);
                resultsCritMeterCountFieldPosition = resultsFC.Fields.FindField(traceSum_CritMeterCountFieldName);
                resultsCommentsFieldPosition = resultsFC.Fields.FindField(traceSum_CommentsFieldName);



                if (resultsFacilityIDFieldPosition == -1)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Field") + traceSum_FacilityIDField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18f"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"));
                    return;
                }
                //if (resultsDateFieldPosition == -1)
                //{
                //    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Field") + traceSum_DateFieldName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18f"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"));
                //    return;
                //}

                //if (resultsValveCountFieldPosition == -1)
                //{
                //    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Field") + traceSum_ValveCountFieldName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18f"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"));
                //    return;
                //}
                //if (resultsMeterCountFieldPosition == -1)
                //{
                //    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Field") + traceSum_MeterCountFieldName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18f"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"));
                //    return;
                //}
                //if (resultsCritMeterCountFieldPosition == -1)
                //{
                //    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Field") + traceSum_CritMeterCountFieldName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_18f"), A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a"));
                //    return;
                //}


                string sShpName = resultsFC.ShapeFieldName;

                pFieldsTest = resultsFC.Fields;
                int lGeomIndex = pFieldsTest.FindField(sShpName);

                pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                pGeometryDefTest = pFieldTest.GeometryDef;

                //Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ;
                bMAware = pGeometryDefTest.HasM;




                string closedValveQuery = ConfigUtil.GetConfigValue("TraceIsolation_Valve_ClosedValveQuery", "");

                pWSEdit.StartEditOperation();


                pInsCur = resultsFC.Insert(true);
                pSumFeatBuf = resultsFC.CreateFeatureBuffer();

                int intValveCount = 0;
                int intMeterCount = 0;
                int intCritMeterCount = 0;
                string comments = "";

                pSelectIDs = mainsFS.SelectionSet.IDs;
                int intCurID = pSelectIDs.Next();
                List<int> processedIDs = new List<int>();

                while (intCurID != -1)
                {
                    intValveCount = 0;
                    intMeterCount = 0;
                    intCritMeterCount = 0;
                    comments = "";
                    try
                    {
                        //if( processedIDs.Contains(intCurID)) {
                        //    pStepPro.Step();

                        //    boolCont = pTrkCan.Continue();

                        //    if (!boolCont)
                        //    {
                        //        pInsCur.Flush();
                        //        pWSEdit.AbortEditOperation();





                        //        return;
                        //    }
                        //    intCurID = pSelectIDs.Next();


                        //    Marshal.ReleaseComObject(pMainsFeat);
                        //    continue;
                        //}
                        // Globals.RemoveTraceGraphics(((IMxDocument)ArcMap.Application.Document).FocusMap, false);
                        // Globals.ClearSelected(ArcMap.Application, false);
                        Globals.ClearGNFlags(app, Globals.GNTypes.Flags);

                        pMainsFeat = mainsFL.FeatureClass.GetFeature(intCurID);
                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_18b") + pStepPro.Position + A4LGSharedFunctions.Localizer.GetString("Of") + FeatureCount + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_18a");


                        pCurve = (ICurve)pMainsFeat.Shape;
                        pPnt = new PointClass();
                        pCurve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, pPnt);

                        IPolyline mergedLines = new PolylineClass();
                        List<int> procoids = new List<int>();

                        string result = GeoNetTools.TraceIsolation(new double[] { pPnt.X }, new double[] { pPnt.Y }, app, sourceFLName, valveFLName, operableFieldNameValve,
                            operableFieldNameSource, snapTol, false, opValues, addSQL, traceIndeterminate, ZeroSourceCont, true, meterFLName, metersCritFieldName,
                            metersCritValue, closedValveQuery, mainsFL, out mergedLines, out procoids, false);
                        string[] resVals = result.Split('_');
                        processedIDs.Add(intCurID);
                        if (resVals.Length == 3)
                        {
                            intValveCount = Convert.ToInt32(resVals[0]);
                            intMeterCount = Convert.ToInt32(resVals[1]);
                            intCritMeterCount = Convert.ToInt32(resVals[2]);
                            comments = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_18a");
                        }
                        else
                        {
                            comments = result;
                        }
                        if (mergedLines != null && procoids != null && saveEntireLine == true)
                        {
                            if (mergedLines.IsEmpty)
                            {
                                comments = "Could not save merged geo: Length is null";
                                pSumFeatBuf.Shape = pMainsFeat.ShapeCopy;
                            }
                            else
                            {
                                processedIDs.AddRange(procoids);
                                try
                                {
                                    if (bZAware)
                                    {

                                        zAware = (ESRI.ArcGIS.Geometry.IZAware)mergedLines;
                                        if (zAware.ZAware == false)
                                        {
                                            zAware.ZAware = true;
                                        }
                                        ESRI.ArcGIS.Geometry.IZ IZ;
                                        IZ = (ESRI.ArcGIS.Geometry.IZ)mergedLines;
                                        IZ.SetConstantZ(0);
                                    }
                                    pSumFeatBuf.Shape = mergedLines;
                                }
                                catch (Exception Ex)
                                {
                                    comments = "Could not save merged geo: " + Ex.Message;
                                    pSumFeatBuf.Shape = pMainsFeat.ShapeCopy;
                                }
                            }
                        }
                        else
                        {
                            pSumFeatBuf.Shape = pMainsFeat.ShapeCopy;

                        }
                        // pStepPro.Message = "Saving Result: " + pStepPro.Position + A4LGSharedFunctions.Localizer.GetString("Of") + FeatureCount + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_18a");


                        if (resultsDateFieldPosition > -1)
                        {
                            pSumFeatBuf.set_Value(resultsDateFieldPosition, DateTime.Now);
                        }
                        pSumFeatBuf.set_Value(resultsFacilityIDFieldPosition, pMainsFeat.get_Value(facilityIDFieldPosition));

                        if (resultsValveCountFieldPosition > -1)
                        {
                            pSumFeatBuf.set_Value(resultsValveCountFieldPosition, intValveCount);
                        }
                        if (resultsMeterCountFieldPosition > -1)
                        {
                            pSumFeatBuf.set_Value(resultsMeterCountFieldPosition, intMeterCount);
                        }
                        if (resultsCritMeterCountFieldPosition > -1)
                        {
                            pSumFeatBuf.set_Value(resultsCritMeterCountFieldPosition, intCritMeterCount);
                        }
                        if (resultsCommentsFieldPosition > -1)
                        {
                            try
                            {
                                if (comments.Length > resultsFC.Fields.get_Field(resultsCommentsFieldPosition).Length)
                                {
                                    comments = comments.Substring(0, resultsFC.Fields.get_Field(resultsCommentsFieldPosition).Length - 2);
                                }
                                pSumFeatBuf.set_Value(resultsCommentsFieldPosition, comments);
                            }
                            catch
                            {
                            }

                        }
                        pStepPro.Step();
                        pInsCur.InsertFeature(pSumFeatBuf);

                        //curIdx = curIdx + 1;

                        // pInsCur.InsertFeature (pFeatBuf);
                        boolCont = pTrkCan.Continue();

                        if (!boolCont)
                        {
                            pInsCur.Flush();
                            pWSEdit.AbortEditOperation();





                            return;
                        }


                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Debug.WriteLine(Ex.Message + A4LGSharedFunctions.Localizer.GetString("Step") + pStepPro.Position);

                        System.Diagnostics.Trace.WriteLine(Ex.Message + A4LGSharedFunctions.Localizer.GetString("Step") + pStepPro.Position);
                        //    MessageBox.Show(Ex.Message);
                    }
                    intCurID = pSelectIDs.Next();


                    Marshal.ReleaseComObject(pMainsFeat);


                }
                try
                {
                    pInsCur.Flush();
                }
                catch
                { }

                pWSEdit.StopEditOperation();


            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_18a") + "\n" + ex.Message);

                if (pWSEdit != null)
                {
                    if (pWSEdit.IsBeingEdited() == true)
                    {
                        pWSEdit.AbortEditOperation();
                        //   pWSEdit.StopEditing(false);
                    }
                }
            }
            finally
            {

                //if (schemaLock != null)
                //    schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                //if (featureClassLoad != null)
                //    featureClassLoad.LoadOnlyMode = false;



                //if (pWSEdit != null)
                //{
                //    if (pWSEdit.IsBeingEdited() == true)
                //    {
                //        pWSEdit.AbortEditOperation();
                //        pWSEdit.StopEditing(false);
                //    }
                //}

                //if (pInsCur != null)
                //    pInsCur.Flush();

                if (pProDlg != null)
                    pProDlg.HideDialog();
                if (pFC != null)
                    Marshal.ReleaseComObject(pFC);
                if (pInsCur != null)
                    Marshal.ReleaseComObject(pInsCur);

                mainsFL = null;

                resultsLayer = null;
                mainFC = null;
                mainsNetwork = null;
                resultsFC = null;
                mainsFS = null;
                pMainsFeat = null;
                pProDFact = null;
                pStepPro = null;
                pProDlg = null;
                pTrkCan = null;
                pWSEdit = null;
                pFC = null;
                pInsCur = null;
                pSumFeatBuf = null;
                pSelectIDs = null;

                pCurve = null;
                pPnt = null;
                pDS = null;

            }
        }

        public static void AddFlagsForSewerProfile(IApplication app, double snapTol, List<ProfileGraphDetails> ProfileGraph)
        {
            IFeatureCursor pFeatCursor = null;
            IMxDocument pMxDoc = null;

            IPoint pTracePoint = null;
            ISpatialFilter pSpatialFilter = null;
            // set the symbol for the flag, red circle
            IMarkerSymbol pMarkerSym = null;
            IRgbColor pRGBColor = null;


            IFeatureLayer pManholeLayer = null;
            IFeatureLayer pMainLayer = null;
            IFeatureLayer pTapLayer = null;
            IGraphicsContainer gc = null;

            IElement pProfileElemFirst = null;
            IElementProperties3 pProfileElemPropFirst = null;

            IElement element = null;
            IElementProperties3 elementProp = null;
            ITopologicalOperator pTopoOp = null;

            IFeature pFeature = null;
            double snapdistnet; // NetworkExt search tolerance
            IPolygon pBuffGeometry = null;
            IMarkerElement markerelem = null;
            IGeometricNetwork gn = null;


            IEnumNetEID juncEIDs = null;
            IEnumNetEID edgeEIDs = null;
            try
            {
                pMxDoc = ((IMxDocument)app.Document);

                pTracePoint = pMxDoc.CurrentLocation;


                pMarkerSym = Globals.FindMarkerSym("Esri.style", "Default", "Circle 2", pMxDoc);
                pRGBColor = Globals.GetColor(255, 0, 0);


                pMarkerSym.Color = pRGBColor;
                pMarkerSym.Size = 11;

                gc = pMxDoc.FocusMap as IGraphicsContainer;

                pProfileElemFirst = null;
                pProfileElemPropFirst = null;



                gc.Reset();
                element = gc.Next();


                while (element != null)
                {
                    elementProp = element as IElementProperties3;
                    if (elementProp.Name.Contains("ProfileGraphFlag"))
                    {
                        if (pProfileElemFirst == null)
                        {
                            pProfileElemFirst = element;
                            pProfileElemPropFirst = elementProp;
                            break;
                        }

                    }
                    element = gc.Next();
                }



                for (int i = 0; i < ProfileGraph.Count; i++)
                {
                    if (pProfileElemFirst != null)
                    {
                        if (!pProfileElemPropFirst.Name.Contains(ProfileGraph[i].Network_Name))
                        {
                            //MessageBox.Show("The network (" + ProfileGraph[i].Network_Name + ") was not found, please update the config to make the name of the layer in the mxd");

                            continue;
                        }

                    }
                    bool FCorLayerManhole = true;
                    pManholeLayer = (IFeatureLayer)Globals.FindLayer(pMxDoc.FocusMap, ProfileGraph[i].Point_LayerName, ref FCorLayerManhole);

                    if (pManholeLayer == null)
                    {
                        // MessageBox.Show("The point layer (" + ProfileGraph[i].Point_LayerName + ") was not found, please update the config to make the name of the layer in the mxd");

                        continue;
                    }
                    bool FCorLayerMain = true;
                    pMainLayer = (IFeatureLayer)Globals.FindLayer(pMxDoc.FocusMap, ProfileGraph[i].Line_LayerName, ref FCorLayerMain);
                    if (pMainLayer == null)
                    {
                        // MessageBox.Show("The Main layer (" + ProfileGraph[i].Line_LayerName + ") was not found, please update the config to make the name of the layer in the mxd");

                        continue;
                    }
                    bool FCorLayerTap = true;
                    if (ProfileGraph[i].PointAlong_LayerName != "")
                    {
                        pTapLayer = (IFeatureLayer)Globals.FindLayer(pMxDoc.FocusMap, ProfileGraph[i].PointAlong_LayerName, ref FCorLayerTap);

                    }
                    else
                        pTapLayer = null;
                    // make sure a manhole was selected
                    // buffer the point by the snap tolerance, this is faster than checking
                    //the flag to see if it is on a manhole


                    pTopoOp = (ITopologicalOperator)pTracePoint;  // QI
                    snapdistnet = Globals.ConvertPixelsToMap(snapTol, pMxDoc.FocusMap);
                    pBuffGeometry = (IPolygon)pTopoOp.Buffer(snapdistnet);

                    // get the feature the user clicked to be sure it's a manhole

                    pSpatialFilter = new SpatialFilterClass();
                    pSpatialFilter.Geometry = pBuffGeometry;
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSpatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                    pFeatCursor = pManholeLayer.Search(pSpatialFilter, true);

                    // there is a layer definition set to show only manholes, cursor
                    // will return nothing if user didn't click on a manhole
                    pFeature = pFeatCursor.NextFeature();

                    if (pFeature != null)
                    {
                        pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pBuffGeometry.Envelope);



                        markerelem = new MarkerElementClass();
                        markerelem.Symbol = pMarkerSym;
                        element = (IElement)markerelem;
                        element.Geometry = pFeature.ShapeCopy;
                        elementProp = element as IElementProperties3;
                        elementProp.Name = "ProfileGraphFlag-" + ProfileGraph[i].Network_Name;

                        elementProp.ReferenceScale = pMxDoc.FocusMap.ReferenceScale;
                        gc.AddElement(element, 0);

                        if (pProfileElemFirst == null)
                            return;
                        else
                        {
                            ESRI.ArcGIS.esriSystem.IStatusBar statusBar = null;
                            ESRI.ArcGIS.esriSystem.IAnimationProgressor animationProgressor = null;
                            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
                            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
                            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
                            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = null;

                            statusBar = app.StatusBar;
                            animationProgressor = statusBar.ProgressAnimation;

                            animationProgressor.Show();
                            animationProgressor.Play(0, -1, -1);


                            statusBar.set_Message(0, A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_19a"));


                            // Create a CancelTracker
                            trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                            progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                            // Set the properties of the Step Progressor
                            System.Int32 int32_hWnd = app.hWnd;
                            stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                            stepProgressor.MinRange = 0;
                            stepProgressor.MaxRange = 3;
                            stepProgressor.StepValue = 1;
                            stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_19a");

                            // Create the ProgressDialog. This automatically displays the dialog
                            progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                            // Set the properties of the ProgressDialog
                            progressDialog2.CancelEnabled = true;
                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDesc_19a");
                            progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsTitle_19a");
                            progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral;

                            // Step. Do your big process here.
                            System.Boolean boolean_Continue = false;
                            boolean_Continue = true;

                            stepProgressor.Step();

                            GeoNetTools.TracePath(new double[] { (pProfileElemFirst.Geometry as IPoint).X, (element.Geometry as IPoint).X },
                                new double[] { (pProfileElemFirst.Geometry as IPoint).Y, (element.Geometry as IPoint).Y },
                                ProfileGraph[i].Network_Name, app, pMxDoc.FocusMap, true, snapTol, true, out juncEIDs, out edgeEIDs, out gn);
                            boolean_Continue = trackCancel.Continue();
                            if (!boolean_Continue)
                            {
                                return;
                            }
                            stepProgressor.Step();
                            stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsProc_19a");
                            if (juncEIDs != null && edgeEIDs != null)
                            {
                                GeoNetTools.ProfileGetRelatedElevData(app, ProfileGraph, gn, edgeEIDs, juncEIDs, i, ref pManholeLayer, ref pMainLayer, ref pTapLayer);
                            }
                            else
                            {


                            }


                            //ProfileFindPath();

                            // unpress the UIToolControl button
                            app.CurrentTool = null;
                            app.RefreshWindow();
                            progressDialog2.HideDialog();
                            progressDialog2 = null;
                            return;
                        }


                    }
                    else//Next Layer in the config
                    {
                    }


                }


                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19a"));



            }
            catch (Exception Ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "AddFlagsForSewerProfile " + Ex.Message);
            }
            finally
            {
                if (pFeatCursor != null)
                {
                    Marshal.ReleaseComObject(pFeatCursor);

                }
                pMxDoc.ActiveView.Refresh();
                pFeatCursor = null;
                pMxDoc = null;

                pTracePoint = null;
                pSpatialFilter = null;
                // set the symbol for the flag, red circle
                pMarkerSym = null;
                pRGBColor = null;


                pManholeLayer = null;
                pMainLayer = null;
                pTapLayer = null;
                gc = null;

                pProfileElemFirst = null;
                pProfileElemPropFirst = null;

                element = null;
                elementProp = null;
                pTopoOp = null;

                pFeature = null;

                pBuffGeometry = null;
                markerelem = null;
                gn = null;


                juncEIDs = null;
                edgeEIDs = null;
            }

        }
        private static void ProfileGetRelatedElevData(IApplication app, List<ProfileGraphDetails> ProfileGraph, IGeometricNetwork pGeometricNet,
                  IEnumNetEID pResultEdges, IEnumNetEID pResultJunctions, int CurrentDetail,
                  ref IFeatureLayer pFLManhole, ref IFeatureLayer pFLMain, ref IFeatureLayer pFLTap)
        {



            List<mainDetails> SewerColMains = null;
            List<manholeDetails> SewerColManholes = null;
            List<tapDetails> SewerColTap = null;

            IEIDHelper pEIDHelperEdges = null;
            IEnumEIDInfo pEnumEIDInfoEdges = null;

            IEIDHelper pEIDHelperJunctions = null;
            IEnumEIDInfo pEnumEIDInfoJunctions = null;

            IPolyline pPolyline = null;
            IPointCollection pPtColl = null;

            IEIDInfo pEIDInfo = null;
            IPoint pNewPt = null;
            ISegmentCollection pSegColl = null;
            IMSegmentation pMSegmentation = null;
            IMAware pMAware = null;

            IPointCollection pPtCollection = null;
            IEnumVertex pEnumVertex;

            IHitTest pHtTest = null;
            IPoint pHitPntOne = null;
            IPoint pHitPntTwo = null;
            IFeature pFeature = null;
            Hashtable pFeatureAdded = null;
            mainDetails mainDet = null;


            IEdgeFeature pEdge = null;

            IPoint pGeoOne = null;
            IPoint pGeoTwo = null;
            IField pFld = null;
            IJunctionFeature pJunc = null;
            tapDetails tapDet = null;
            manholeDetails manDet = null;
            IFeatureLayer pFl = null;
            ISpatialFilter pSpatFilt = null;
            IFeatureCursor pFC = null;

            try
            {



                SewerColMains = new List<mainDetails>();
                SewerColManholes = new List<manholeDetails>();
                SewerColTap = new List<tapDetails>();

                pEIDHelperEdges = new EIDHelper();
                pEIDHelperEdges.GeometricNetwork = pGeometricNet;
                pEIDHelperEdges.ReturnFeatures = true;

                pEIDHelperEdges.ReturnGeometries = true;
                pEIDHelperEdges.PartialComplexEdgeGeometry = true;
                pEnumEIDInfoEdges = pEIDHelperEdges.CreateEnumEIDInfo(pResultEdges);


                pEnumEIDInfoEdges.Reset();  //edges



                pEIDHelperJunctions = new EIDHelperClass();
                pEIDHelperJunctions.GeometricNetwork = pGeometricNet;
                pEIDHelperJunctions.ReturnFeatures = true;
                pEIDHelperJunctions.ReturnGeometries = true;
                pEIDHelperJunctions.PartialComplexEdgeGeometry = true;
                pEnumEIDInfoJunctions = pEIDHelperJunctions.CreateEnumEIDInfo(pResultJunctions);
                pEnumEIDInfoJunctions.Reset();// junctions



                pPolyline = new PolylineClass();
                pPolyline.SpatialReference = (pFLMain as IGeoDataset).SpatialReference;

                pPtColl = (IPointCollection)pPolyline;  //QI



                for (int i = 0; i < pEnumEIDInfoJunctions.Count; i++)
                {

                    pEIDInfo = pEnumEIDInfoJunctions.Next();
                    pNewPt = (IPoint)pEIDInfo.Geometry;

                    pPtColl.AddPoint(pNewPt);




                }

                pSegColl = (ISegmentCollection)pPolyline;


                pPolyline.Densify(50, 0.01);

                pMAware = (IMAware)pPolyline;//'QI
                pMAware.MAware = true;
                pMSegmentation = (IMSegmentation)pPolyline;
                // get the M values, put the distance in m, 0 to Length
                pMSegmentation.SetMsAsDistance(false);

                pPtCollection = (IPointCollection)pPolyline;

                pEnumVertex = pPtCollection.EnumVertices;
                pEnumVertex.Reset();



                pHtTest = pPolyline as IHitTest;

                pHitPntOne = new PointClass();
                pHitPntTwo = new PointClass();
                double pHitDistOne = -1;
                double pHitDistTwo = -1;
                int pHitPrtOne = -1;
                int pHitPrtTwo = -1;
                int pHitSegOne = -1;
                int pHitSegTwo = -1;
                bool pHitSideOne = false;
                bool pHitSideTwo = false;


                pFeatureAdded = new Hashtable();


                pEnumEIDInfoEdges.Reset();  //edges
                int intUpStreamFld = pFLMain.FeatureClass.Fields.FindField(ProfileGraph[CurrentDetail].Line_UpStreamElevationField);
                int intDownStreamFld = pFLMain.FeatureClass.Fields.FindField(ProfileGraph[CurrentDetail].Line_DownStreamElevationField);
                if (intDownStreamFld < 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19c") + ProfileGraph[CurrentDetail].Line_DownStreamElevationField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19b"));
                }
                if (intUpStreamFld < 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19d") + ProfileGraph[CurrentDetail].Line_UpStreamElevationField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19b"));
                }
                for (int i = 0; i < pResultEdges.Count; i++)
                {
                    pEIDInfo = pEnumEIDInfoEdges.Next();
                    pFeature = pEIDInfo.Feature;
                    if (((IDataset)pFeature.Class).Name != ((IDataset)pFLMain.FeatureClass).Name)
                        continue;
                    if (pFeatureAdded.ContainsValue(pFeature.OID))
                        continue;

                    mainDet = new mainDetails();


                    pEdge = (IEdgeFeature)pFeature;

                    pGeoOne = (IPoint)pEdge.FromJunctionFeature.get_OriginalGeometryForJunctionElement(0);
                    pGeoTwo = (IPoint)pEdge.ToJunctionFeature.get_OriginalGeometryForJunctionElement(0);

                    pFeature = ((IFeatureClass)pFeature.Class).GetFeature(pFeature.OID);

                    bool bHitOne = pHtTest.HitTest(pGeoOne, .1,
                                            esriGeometryHitPartType.esriGeometryPartVertex,
                                            pHitPntOne, ref pHitDistOne, ref pHitPrtOne, ref pHitSegOne, ref pHitSideOne);
                    bool bHitTwo = pHtTest.HitTest(pGeoTwo, .1,
                                   esriGeometryHitPartType.esriGeometryPartVertex,
                                   pHitPntTwo, ref pHitDistTwo, ref pHitPrtTwo, ref pHitSegTwo, ref pHitSideTwo);
                    if (bHitOne && bHitTwo)
                    {
                        if (pHitPntOne.M < pHitPntTwo.M)
                        {
                            mainDet.UpM = pHitPntOne.M;
                            mainDet.DownM = pHitPntTwo.M;
                            if (intUpStreamFld > 0)
                            {
                                if (pFeature.get_Value(intUpStreamFld).ToString() != "")
                                {
                                    mainDet.UpElev = Convert.ToDouble(pFeature.get_Value(intUpStreamFld));
                                }
                                else
                                    mainDet.UpElev = -9999;
                            }
                            else
                                mainDet.UpElev = -9999;

                            if (intDownStreamFld > 0)
                            {
                                if (pFeature.get_Value(intDownStreamFld).ToString() != "")
                                {
                                    mainDet.DownElev = Convert.ToDouble(pFeature.get_Value(intDownStreamFld));
                                }
                                else
                                    mainDet.DownElev = -9999;
                            }
                            else
                                mainDet.DownElev = -9999;
                        }
                        else
                        {
                            mainDet.DownM = pHitPntOne.M;
                            mainDet.UpM = pHitPntTwo.M;
                            if (intUpStreamFld > 0)
                            {
                                if (pFeature.get_Value(intUpStreamFld) != null && pFeature.get_Value(intUpStreamFld).ToString() != "")
                                {
                                    mainDet.DownElev = Convert.ToDouble(pFeature.get_Value(intUpStreamFld));
                                }
                                else
                                    mainDet.DownElev = -9999;
                            }
                            else
                                mainDet.DownElev = -9999;

                            if (intDownStreamFld > 0)
                            {
                                if (pFeature.get_Value(intDownStreamFld) != null && pFeature.get_Value(intDownStreamFld).ToString() != "")
                                {
                                    mainDet.UpElev = Convert.ToDouble(pFeature.get_Value(intDownStreamFld));
                                }
                                else
                                    mainDet.UpElev = -9999;
                            }
                            else
                                mainDet.UpElev = -9999;

                        }
                        string label = "";
                        for (int l = 0; l < ProfileGraph[CurrentDetail].Line_Labels.Length; l++)
                        {
                            if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_Labels[l]) > 0)
                            {
                                int fldIdx = pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_Labels[l]);

                                pFld = pFeature.Fields.get_Field(fldIdx);

                                if (pFeature.get_Value(fldIdx) != null)
                                {
                                    if (label == "")
                                    {
                                        label = Globals.GetDomainDisplay(pFeature.get_Value(fldIdx), pFeature, pFld);
                                    }
                                    else
                                    {

                                        label = label + "\r\n" + Globals.GetDomainDisplay(pFeature.get_Value(fldIdx), pFeature, pFld);

                                    }
                                }
                            }
                        }


                        mainDet.Label = label;


                    }



                    if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_IDField) > 0)
                    {
                        if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_IDField)) != null && pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_IDField)).ToString() != "")
                        {
                            mainDet.MainID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Line_IDField)));
                        }
                        else
                            mainDet.MainID = "Unk";
                    }
                    else
                        mainDet.MainID = "Unk";

                    pFeatureAdded.Add(pFeature.OID, pFeature.OID);
                    SewerColMains.Add(mainDet);


                }

                pFeatureAdded = new Hashtable();


                pEnumEIDInfoJunctions.Reset();
                for (int i = 0; i < pEnumEIDInfoJunctions.Count; i++)
                {


                    pEIDInfo = pEnumEIDInfoJunctions.Next();
                    pFeature = pEIDInfo.Feature;
                    if (pFLTap != null)
                    {
                        if (((IDataset)pFeature.Class).Name == ((IDataset)pFLTap.FeatureClass).Name)
                        {
                            pJunc = (IJunctionFeature)pFeature;
                            pFeature = ((IFeatureClass)pFeature.Class).GetFeature(pFeature.OID);
                            pHitPntOne = new PointClass();
                            bool bHit = pHtTest.HitTest(pFeature.Shape as IPoint, .1,
                                                    esriGeometryHitPartType.esriGeometryPartVertex,
                                                    pHitPntOne, ref pHitDistOne, ref pHitPrtOne, ref pHitSegOne, ref pHitSideOne);

                            if (bHit)
                            {
                                tapDet = new tapDetails();

                                tapDet.M = pHitPntOne.M;

                                if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_IDField) > 0)
                                {
                                    if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_IDField)) != null &&
                                        pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_IDField)).ToString() != "")
                                    {
                                        tapDet.tapID = pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_IDField)).ToString();
                                    }
                                    else
                                        tapDet.tapID = "Unk";
                                }
                                else
                                    tapDet.tapID = "Unk";

                                string label = "";
                                for (int l = 0; l < ProfileGraph[CurrentDetail].PointAlong_Labels.Length; l++)
                                {
                                    if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_Labels[l]) > 0)
                                    {
                                        int fldIdx = pFeature.Fields.FindField(ProfileGraph[CurrentDetail].PointAlong_Labels[l]);

                                        pFld = pFeature.Fields.get_Field(fldIdx);

                                        if (pFeature.get_Value(fldIdx) != null)
                                        {
                                            if (label == "")
                                            {
                                                label = Globals.GetDomainDisplay(pFeature.get_Value(fldIdx), pFeature, pFld);
                                            }
                                            else
                                            {

                                                label = label + "\r\n" + Globals.GetDomainDisplay(pFeature.get_Value(fldIdx), pFeature, pFld);

                                            }
                                        }
                                    }
                                }


                                tapDet.tapLabel = label;
                                SewerColTap.Add(tapDet);
                            }

                        }
                    }
                    object val;
                    double elev;
                    if (((IDataset)pFeature.Class).Name == ((IDataset)pFLManhole.FeatureClass).Name)
                    {
                        if (pFeatureAdded.ContainsValue(pFeature.OID))
                            continue;

                        pJunc = (IJunctionFeature)pFeature;
                        pFeature = ((IFeatureClass)pFeature.Class).GetFeature(pFeature.OID);
                        pHitPntOne = new PointClass();
                        bool bHit = pHtTest.HitTest(pFeature.Shape as IPoint, .1,
                                                esriGeometryHitPartType.esriGeometryPartVertex,
                                                pHitPntOne, ref pHitDistOne, ref pHitPrtOne, ref pHitSegOne, ref pHitSideOne);

                        if (bHit)
                        {
                            manDet = new manholeDetails();

                            manDet.M = pHitPntOne.M;

                            if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_TopElevationField) > 0)
                            {
                                if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_TopElevationField)) != null &&
                                    pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_TopElevationField)).ToString() != "")
                                {
                                    val = pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_TopElevationField));

                                    if (Double.TryParse(val.ToString(), out elev))
                                    {
                                        manDet.Top = elev;
                                    }
                                    else
                                    {
                                        manDet.Top = -9999;
                                    }
                                }
                                else
                                    manDet.Top = -9999;
                            }
                            else
                                manDet.Top = -9999;


                            if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_BottomElevationField) > 0)
                            {
                                if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_BottomElevationField)) != null &&
                                    pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_BottomElevationField)).ToString() != "")
                                {
                                    val = pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_BottomElevationField));

                                    if (Double.TryParse(val.ToString(), out elev))
                                    {
                                        manDet.Bottom = elev;
                                        if (ProfileGraph[CurrentDetail].Point_BottomElevationTypeField.ToUpper() == "INVERT")
                                        {

                                            manDet.Bottom = manDet.Top - elev;

                                        }

                                    }
                                    else
                                    {
                                        manDet.Bottom = -9999;
                                    }

                                }
                                else
                                    manDet.Bottom = -9999;
                            }
                            else
                                manDet.Bottom = -9999;


                            //if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertElevationField) > 0)
                            //{
                            //    if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertElevationField)) != null && pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertElevationField)).ToString() != "")
                            //    {
                            //        manDet.InvertElev = (double)pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertElevationField));
                            //    }
                            //    else
                            //        manDet.InvertElev = -9999;
                            //}
                            //else
                            //    manDet.InvertElev = -9999;

                            //if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertField) > 0)
                            //{
                            //    if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertField)) != null && pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertField)).ToString() != "")
                            //    {
                            //        manDet.Invert = (double)pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_InvertField));
                            //    }
                            //    else
                            //        manDet.Invert = -9999;
                            //}
                            //else
                            //    manDet.Invert = -9999;

                            //if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_RimElevationField) > 0)
                            //{
                            //    if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_RimElevationField)) != null && pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_RimElevationField)).ToString() != "")
                            //    {
                            //        manDet.Rim = (double)pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_RimElevationField));
                            //    }
                            //    else
                            //        manDet.Rim = -9999;
                            //}
                            //else
                            //    manDet.Rim = -9999;


                            if (pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_IDField) > 0)
                            {
                                if (pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_IDField)) != null && pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_IDField)).ToString() != "")
                                {
                                    manDet.ManholeID = pFeature.get_Value(pFeature.Fields.FindField(ProfileGraph[CurrentDetail].Point_IDField)).ToString();
                                }
                                else
                                    manDet.ManholeID = "UNK";
                            }
                            else
                                manDet.ManholeID = "UNK";

                            pFeatureAdded.Add(pFeature.OID, pFeature.OID);
                            SewerColManholes.Add(manDet);
                        }
                    }

                }
                if (ProfileGraph[CurrentDetail].Lines_Along != null)
                {
                    if (ProfileGraph[CurrentDetail].Lines_Along.Length > 0)
                    {

                        pSpatFilt = new SpatialFilterClass();
                        pSpatFilt.Geometry = pPolyline;
                        pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;


                        for (int i = 0; i < ProfileGraph[CurrentDetail].Lines_Along.Length; i++)
                        {
                            bool FCorLayerTemp = true;
                            pFl = (IFeatureLayer)Globals.FindLayer(app, ProfileGraph[CurrentDetail].Lines_Along[i].Layer_Name, ref FCorLayerTemp);
                            if (pFl != null)
                            {

                                intUpStreamFld = pFl.FeatureClass.Fields.FindField(ProfileGraph[CurrentDetail].Lines_Along[i].Line_UpStreamElevationField);
                                intDownStreamFld = pFl.FeatureClass.Fields.FindField(ProfileGraph[CurrentDetail].Lines_Along[i].Line_DownStreamElevationField);
                                int intIdFld = pFl.FeatureClass.Fields.FindField(ProfileGraph[CurrentDetail].Lines_Along[i].Line_IDField);
                                if (intIdFld < 0)
                                {
                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19f") + ProfileGraph[CurrentDetail].Lines_Along[i].Line_IDField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19e") + ProfileGraph[CurrentDetail].Lines_Along[i].Layer_Name);
                                }
                                if (intDownStreamFld < 0)
                                {
                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19c") + ProfileGraph[CurrentDetail].Lines_Along[i].Line_DownStreamElevationField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19e") + ProfileGraph[CurrentDetail].Lines_Along[i].Layer_Name);
                                }
                                if (intUpStreamFld < 0)
                                {
                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19d") + ProfileGraph[CurrentDetail].Lines_Along[i].Line_UpStreamElevationField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_19e") + ProfileGraph[CurrentDetail].Lines_Along[i].Layer_Name);
                                }



                                pSpatFilt.GeometryField = pFl.FeatureClass.ShapeFieldName;

                                pFC = pFl.Search(pSpatFilt, true);

                            }
                        }
                    }
                }

                ProfileCreateGraph(app, ProfileGraph, pPolyline, SewerColMains, SewerColManholes, SewerColTap, CurrentDetail);




            }
            catch (Exception Ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ProfileGetRelatedSewerElevData " + Ex.Message);
            }
            finally
            {

                SewerColMains = null;
                SewerColManholes = null;
                SewerColTap = null;

                pEIDHelperEdges = null;
                pEnumEIDInfoEdges = null;

                pEIDHelperJunctions = null;
                if (pEnumEIDInfoJunctions != null)
                    Marshal.ReleaseComObject(pEnumEIDInfoJunctions);

                pEnumEIDInfoJunctions = null;

                pPolyline = null;
                pPtColl = null;

                pEIDInfo = null;
                pNewPt = null;
                pSegColl = null;
                pMSegmentation = null;
                pMAware = null;

                pPtCollection = null;
                pEnumVertex = null;

                pHtTest = null;
                pHitPntOne = null;
                pHitPntTwo = null;
                pFeature = null;
                pFeatureAdded = null;
                mainDet = null;


                pEdge = null;

                pGeoOne = null;
                pGeoTwo = null;
                pFld = null;
                pJunc = null;
                tapDet = null;
                manDet = null;
                pFl = null;
                pSpatFilt = null;
                if (pFC != null)
                    Marshal.ReleaseComObject(pFC);

                pFC = null;


            }

        }
        private static void ProfileCreateGraph(IApplication app, List<ProfileGraphDetails> ProfileGraph, IPolyline pPolyline, List<mainDetails> SewerColMains,
                                        List<manholeDetails> SewerColManholes, List<tapDetails> SewerColTap, int CurrentDetail)
        {
            // profile the sewer line and the surface elevation data
            // pPolyLine is a line composed from the edge results of the network trace
            IWorkspace pWS = null;
            ICursor pCursor = null;
            IMxDocument pMxDoc = null;
            IMap pMap = null;
            IZ pPolyLineZ = null;


            IZAware pZAwareLineZ = null;
            ISurface pSurface = null;
            IRasterLayer pRasterLayer = null;

            //get the elevation layer
            ILayer pRasLay = null;
            IPoint pPtOrigFrom = null;
            IPoint pPtOrigTo = null;
            IStandaloneTableCollection pStandAloneTabColl = null;
            IStandaloneTable pStandAloneTabMainLabel = null;
            ITable pTapTable = null;
            ITable pMainTable = null;
            ITable pManholeTable = null;

            ITable pSurfaceTable = null;

            ITable pMainLabelTable = null;
            ITableFields pTableFieldsMainLabel = null;
            IStandaloneTable pStandAloneTabMain = null;
            ITableFields pTableFieldsMain = null;
            IStandaloneTable pStandAloneTabManhole = null;
            ITableFields pTableFieldsManhole = null;
            IStandaloneTable pStandAloneTabSurface = null;
            ITableFields pTableFieldsSurface = null;
            IStandaloneTable pStandAloneTabTap = null;
            ITableFields pTableFieldsTap = null;
            IRowBuffer pRowBuff = null;
            ICursor pLabelCursor = null;

            ICursor pTapCursor = null;
            ISegment pSegment = null;
            ILine pLine = null;
            IPoint pFromPnt = null;
            IPoint pToPnt = null;
            IPoint pMidPnt = null;
            IDataGraphBase pDataGraphBase = null;
            IDataGraphT pDataGraphT = null;

            IPointCollection pPtCollection = null;
            IEnumVertex pEnumVertex = null;
            IPoint pPt = null;
            ISeriesProperties pAreaSeriesProps = null;
            IColor pColor = null;

            String strXDataFldName = null;
            String strYDataFldName = null;
            IDataSortSeriesProperties pSortFlds = null;
            IPointSeriesProperties pScatterSeriesProps2 = null;
            ISeriesProperties pScatterSeriesProps = null;
            IBarSeriesProperties pManHoleSeries = null;
            ILineSeriesProperties pLineSeriesProps2 = null;

            ISeriesProperties pLineSeriesProps = null;
            ITrackCancel pCancelTracker = null;
            IDataGraphWindow2 pDataGraphWin = null;
            IDataGraphCollection pDataGraphs = null;
            try
            {

                pMxDoc = (IMxDocument)app.Document;
                pMap = pMxDoc.FocusMap;

                // Open the Workspace
                pWS = Globals.CreateInMemoryWorkspace();



                //get the elevation layer
                bool FCorLayerRas = true;
                pRasLay = Globals.FindLayer(pMap, ProfileGraph[CurrentDetail].Elevation_LayerName, ref FCorLayerRas);

                if (pRasLay != null)
                {
                    pRasterLayer = pRasLay as IRasterLayer;
                    // get the surface to interpolate from
                    pSurface = Globals.GetSurface(pRasterLayer);

                    // make the polyline z-aware
                    pZAwareLineZ = (IZAware)pPolyline;
                    pZAwareLineZ.ZAware = true;

                    // work around for InterpolateFromSurface sometimes flipping polyline

                    pPtOrigFrom = pPolyline.FromPoint;
                    pPtOrigTo = pPolyline.ToPoint;
                    pPolyline.Project((pRasterLayer as IGeoDataset).SpatialReference);
                    // add z values to the polyline
                    pPolyLineZ = (IZ)pPolyline;

                    pPolyLineZ.InterpolateFromSurface(pSurface);
                    pPolyline.ReverseOrientation();


                }
                int i;

                pStandAloneTabColl = (IStandaloneTableCollection)pMap;
                for (i = pStandAloneTabColl.StandaloneTableCount - 1; i > 0; i--)
                {
                    if (pStandAloneTabColl.StandaloneTable[i].Name == "Point Table")
                    {
                        pStandAloneTabColl.RemoveStandaloneTable(pStandAloneTabColl.StandaloneTable[i]);
                        continue;
                    }
                    else if (pStandAloneTabColl.StandaloneTable[i].Name == "Surface Table")
                    {
                        pStandAloneTabColl.RemoveStandaloneTable(pStandAloneTabColl.StandaloneTable[i]);
                        continue;
                    }
                    else if (pStandAloneTabColl.StandaloneTable[i].Name == "Line Table")
                    {
                        pStandAloneTabColl.RemoveStandaloneTable(pStandAloneTabColl.StandaloneTable[i]);
                        continue;
                    }
                    else if (pStandAloneTabColl.StandaloneTable[i].Name == "Line Label Table")
                    {
                        pStandAloneTabColl.RemoveStandaloneTable(pStandAloneTabColl.StandaloneTable[i]);
                        continue;
                    }
                    else if (pStandAloneTabColl.StandaloneTable[i].Name == "Points Along Table")
                    {
                        pStandAloneTabColl.RemoveStandaloneTable(pStandAloneTabColl.StandaloneTable[i]);
                        continue;
                    }
                }



                pMainTable = Globals.createTableInMemory("Line Table", createLineFields(), pWS);
                if (pMainTable == null)
                    return;
                pManholeTable = Globals.createTableInMemory("Point Table", createPointFields(), pWS);
                if (pManholeTable == null)
                    return;

                if (pRasterLayer != null)
                {
                    pSurfaceTable = Globals.createTableInMemory("Surface Table", createSurfaceFields(), pWS);
                    if (pSurfaceTable == null)
                        return;
                }

                pMainLabelTable = Globals.createTableInMemory("Line Label Table", createLineLabelFields(), pWS);
                if (pMainLabelTable == null)
                    return;


                pTapTable = Globals.createTableInMemory("Points Along Table", createLineLabelFields(), pWS);
                if (pTapTable == null)
                    return;

                // add the table to the map so it can be edited
                // Create a new standalone table and add it to the collection of the focus map

                pStandAloneTabMainLabel = new StandaloneTableClass();
                pStandAloneTabMainLabel.Table = pMainLabelTable;
                pStandAloneTabColl.AddStandaloneTable(pStandAloneTabMainLabel);
                pMainLabelTable = (ITable)pStandAloneTabMainLabel;// QI, used in data graph
                pTableFieldsMainLabel = (ITableFields)pStandAloneTabMainLabel;


                pStandAloneTabMain = new StandaloneTableClass();
                pStandAloneTabMain.Table = pMainTable;
                pStandAloneTabColl.AddStandaloneTable(pStandAloneTabMain);
                pMainTable = (ITable)pStandAloneTabMain;// QI, used in data graph
                pTableFieldsMain = (ITableFields)pStandAloneTabMain;


                pStandAloneTabManhole = new StandaloneTableClass();
                pStandAloneTabManhole.Table = pManholeTable;
                pStandAloneTabColl.AddStandaloneTable(pStandAloneTabManhole);
                pManholeTable = (ITable)pStandAloneTabManhole;// QI, used in data graph
                pTableFieldsManhole = (ITableFields)pStandAloneTabManhole;

                if (pSurfaceTable != null)
                {
                    pStandAloneTabSurface = new StandaloneTableClass();
                    pStandAloneTabSurface.Table = pSurfaceTable;
                    pStandAloneTabColl.AddStandaloneTable(pStandAloneTabSurface);
                    pSurfaceTable = (ITable)pStandAloneTabSurface;// QI, used in data graph

                    pTableFieldsSurface = (ITableFields)pStandAloneTabSurface;
                }
                if (pTapTable != null)
                {

                    pStandAloneTabTap = new StandaloneTableClass();
                    pStandAloneTabTap.Table = pTapTable;
                    pStandAloneTabColl.AddStandaloneTable(pStandAloneTabTap);
                    pTapTable = (ITable)pStandAloneTabTap;// QI, used in data graph

                    pTableFieldsTap = (ITableFields)pStandAloneTabTap;
                }

                // Refresh the TOC
                pMxDoc.UpdateContents();


                // get an insert cursor for the table


                pCursor = pManholeTable.Insert(true);

                double minChartVal = 0.0;
                double maxChartVal = 0.0;
                int id = 0;
                foreach (manholeDetails manholeDetail in SewerColManholes)
                {
                    //SewerElevCollManholesDetails.Add(new object[] { pPt.M, manRim, manInvElev, manInv, manID });

                    pRowBuff = pManholeTable.CreateRowBuffer();
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("X"), manholeDetail.M);//0
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("TOPELEV"), manholeDetail.Top);//1
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("BOTELEV"), manholeDetail.Bottom);//2
                    //pRowBuff.set_Value(pRowBuff.Fields.FindField("INVERT"), manholeDetail.Invert);//3
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("ID"), manholeDetail.ManholeID);//4

                    if (id == 0)
                    {
                        minChartVal = (double)manholeDetail.Bottom;
                        maxChartVal = (double)manholeDetail.Top;
                        id++;
                    }
                    else
                    {
                        if (minChartVal > (double)manholeDetail.Bottom)
                            minChartVal = (double)manholeDetail.Bottom;
                        if (maxChartVal < (double)manholeDetail.Top)
                            maxChartVal = (double)manholeDetail.Top;
                    }
                    pCursor.InsertRow(pRowBuff);

                }
                // flush any writes
                pCursor.Flush();
                Marshal.ReleaseComObject(pCursor);

                pCursor = pMainTable.Insert(true);
                pLabelCursor = pMainLabelTable.Insert(true);
                pTapCursor = pTapTable.Insert(true);

                foreach (mainDetails mainDetail in SewerColMains)
                {
                    //SewerElevCollManholesDetails.Add(new object[] { pPt.M, manRim, manInvElev, manInv, manID });


                    //pRowBuff.set_Value(pRowBuff.Fields.FindField("FROMM"), mainDetail.UpM);//0
                    //pRowBuff.set_Value(pRowBuff.Fields.FindField("TOM"), mainDetail.DownM);//1
                    //pRowBuff.set_Value(pRowBuff.Fields.FindField("FROMELEV"), mainDetail.UpElev);//2
                    //pRowBuff.set_Value(pRowBuff.Fields.FindField("TOELEV"), mainDetail.DownElev);//3
                    pRowBuff = pMainTable.CreateRowBuffer();
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("ELEVATION"), mainDetail.UpElev);//2
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("MEASURE"), mainDetail.UpM);//3
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("FACILITYID"), mainDetail.MainID);//4
                    pCursor.InsertRow(pRowBuff);

                    pRowBuff = pMainTable.CreateRowBuffer();
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("ELEVATION"), mainDetail.DownElev);//2
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("MEASURE"), mainDetail.DownM);//3
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("FACILITYID"), mainDetail.MainID);//4
                    pCursor.InsertRow(pRowBuff);



                    pLine = new LineClass();
                    pFromPnt = new PointClass();
                    pToPnt = new PointClass();
                    pMidPnt = new PointClass();

                    pFromPnt.Y = mainDetail.UpElev;
                    pFromPnt.X = mainDetail.UpM;

                    pToPnt.Y = mainDetail.DownElev;
                    pToPnt.X = mainDetail.DownM;
                    pLine.PutCoords(pFromPnt, pToPnt);
                    pSegment = pLine as ISegment;

                    pSegment.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, pMidPnt);


                    //   pSegM = (ISegmentM)pSegment;
                    // pSegM.SetMs(

                    pRowBuff = pMainLabelTable.CreateRowBuffer();
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("ELEVATION"), pMidPnt.Y);//2
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("MEASURE"), pMidPnt.X);//3
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("FACILITYID"), mainDetail.MainID);//4
                    pRowBuff.set_Value(pRowBuff.Fields.FindField("LABEL"), mainDetail.Label);//4
                    pLabelCursor.InsertRow(pRowBuff);
                    double slope = (pToPnt.Y - pFromPnt.Y) / (pToPnt.X - pFromPnt.X);

                    foreach (tapDetails tpDet in SewerColTap)
                    {
                        double distDown;
                        if (tpDet.Added == true)
                            continue;
                        if (pFromPnt.X < pToPnt.X)
                        {
                            if (tpDet.M > pFromPnt.X && tpDet.M < pToPnt.X)
                            {
                                distDown = tpDet.M - pFromPnt.X;

                            }
                            else
                                continue;
                        }
                        else
                        {
                            if (tpDet.M > pToPnt.X && tpDet.M < pFromPnt.X)
                            {
                                distDown = tpDet.M - pToPnt.X;

                            }
                            else
                                continue;
                        }
                        {

                            pSegment.QueryPoint(esriSegmentExtension.esriNoExtension, distDown, false, pMidPnt);
                            //if (pMidPnt.X == tpDet.M)
                            //{
                            pRowBuff = pTapTable.CreateRowBuffer();
                            pRowBuff.set_Value(pRowBuff.Fields.FindField("ELEVATION"), pMidPnt.Y);//2
                            pRowBuff.set_Value(pRowBuff.Fields.FindField("MEASURE"), tpDet.M);//3
                            pRowBuff.set_Value(pRowBuff.Fields.FindField("FACILITYID"), tpDet.tapID);//4
                            pRowBuff.set_Value(pRowBuff.Fields.FindField("LABEL"), tpDet.tapLabel);//4
                            //pRowBuff.set_Value(pRowBuff.Fields.FindField("LABEL"), mainDetail.Label);//4
                            pTapCursor.InsertRow(pRowBuff);
                            tpDet.Added = true;
                            //SewerColTap.Remove(tpDet);

                            // }
                        }
                    }




                    //if (minChartVal > (double)manholeDetail.InvertElev)
                    //    minChartVal = (double)manholeDetail.InvertElev;
                    //if (maxChartVal < (double)manholeDetail.Rim)
                    //    maxChartVal = (double)manholeDetail.Rim;



                }
                // flush any writes
                pCursor.Flush();
                Marshal.ReleaseComObject(pCursor);

                pLabelCursor.Flush();
                Marshal.ReleaseComObject(pLabelCursor);

                pTapCursor.Flush();
                Marshal.ReleaseComObject(pTapCursor);




                if (pSurfaceTable != null)
                {
                    pPtCollection = (IPointCollection)pPolyLineZ;

                    pEnumVertex = pPtCollection.EnumVertices;
                    pEnumVertex.Reset();

                    int lPartIndex;
                    int lVertexIndex;
                    pEnumVertex.Next(out pPt, out lPartIndex, out lVertexIndex);
                    pCursor = pSurfaceTable.Insert(true);
                    while (pPt != null)
                    {
                        pRowBuff = pSurfaceTable.CreateRowBuffer();
                        pRowBuff.set_Value(pRowBuff.Fields.FindField("X"), pPt.M);//2
                        pRowBuff.set_Value(pRowBuff.Fields.FindField("Y"), pPt.Z);//3

                        pCursor.InsertRow(pRowBuff);
                        pEnumVertex.Next(out pPt, out lPartIndex, out lVertexIndex);

                    }
                }


                //=============================================================================
                // create the graph from the table


                // create graph
                pDataGraphBase = new DataGraphTClass();
                pDataGraphT = (IDataGraphT)pDataGraphBase;

                // graph, axis and legend titles. Substitute them for different input layer
                pDataGraphT.GeneralProperties.Title = ProfileGraph[CurrentDetail].GraphTitle_Name; // "Sewer Main Profile";
                pDataGraphT.LegendProperties.Title = ProfileGraph[CurrentDetail].Legend_Name; //"Profile Legend";



                //   IDataGraphTAxisProperties pHort = pDataGraphT.AxisProperties[0];

                pDataGraphT.AxisProperties[0].Title = ProfileGraph[CurrentDetail].LeftAxis_Name;
                pDataGraphT.AxisProperties[0].AutomaticMaximum = true;
                pDataGraphT.AxisProperties[0].AutomaticMinimum = true;
                pDataGraphT.AxisProperties[0].Minimum = minChartVal - 5;
                pDataGraphT.AxisProperties[0].Maximum = maxChartVal + 5;
                pDataGraphT.AxisProperties[0].InitDefaults();

                //pDataGraphT.AxisProperties[1].AutomaticMaximum = true;
                //pDataGraphT.AxisProperties[1].AutomaticMinimum = false;
                //pDataGraphT.AxisProperties[1].Minimum = minChartVal - 5;
                ////pDataGraphT.AxisProperties[1].Title = "Manholes";
                //pDataGraphT.AxisProperties[1].Visible = true;

                pDataGraphT.AxisProperties[3].Visible = true;
                pDataGraphT.AxisProperties[3].Title = ProfileGraph[CurrentDetail].TopAxis_Name;
                pDataGraphT.AxisProperties[2].Title = ProfileGraph[CurrentDetail].BottomAxis_Name; // "Date";
                pDataGraphT.AxisProperties[2].ValueFormat = "0";
                pDataGraphT.AxisProperties[2].Minimum = 0;
                pDataGraphT.AxisProperties[2].AutomaticMinimum = false;
                pDataGraphT.AxisProperties[2].ValueFormat = "#,##0.###";
                pDataGraphBase.Name = ProfileGraph[CurrentDetail].Graph_Name; ;

                // & strTableName  layerName;
                //IDataGraphTGeneralProperties pGenProp = pDataGraphT.GeneralProperties;
                //pGenProp.Show3D = true;


                // put the legend below the graph
                //pDataGraphT.LegendProperties.Alignment = esriDataGraphTLegendBottom

                // use only for series-specific properties
                //IPointSeriesProperties pPtSeries;
                //IAreaSeriesProperties pAreaSeries;
                //ILineSeriesProperties pLineSeries;

                //-------------------------------------------------------------------------------
                // area series - ground elevation

                int idx = 0;
                if (pSurfaceTable != null)
                {
                    // create the area graph for the ground elevation
                    pAreaSeriesProps = pDataGraphT.AddSeries("area:vertical"); //("scatter_plot")  '("line:vertical")
                    pAreaSeriesProps.SourceData = pSurfaceTable;// pLayer
                    pAreaSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                    pColor = Globals.GetColor(137, 112, 68);

                    pAreaSeriesProps.CustomColor = pColor.RGB; // pSymbol.Color.RGB

                    // get the fields to graph - ground elevation
                    strXDataFldName = "X"; //pTable.Fields.Field(i).Name ' "Dist_to_Rds"
                    strYDataFldName = "Y"; // pTable.Fields.Field(i).Name ' "Dist_to_Rds"

                    //  pSeriesProps.whereClause = whereClause
                    pAreaSeriesProps.InLegend = true; //show legend   ' false = don't show legend
                    pAreaSeriesProps.Name = ProfileGraph[CurrentDetail].Elevation_LayerName;//A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_1");
                    //pSeriesProps.LabelField = "OBJECTID";
                    pAreaSeriesProps.ValueFormat = "0 ";
                    pAreaSeriesProps.SetField(0, strXDataFldName); // timefldName
                    pAreaSeriesProps.SetField(1, strYDataFldName);

                    // sort on the X value
                    pSortFlds = (IDataSortSeriesProperties)pAreaSeriesProps;

                    pSortFlds.AddSortingField(strXDataFldName, true, ref idx);
                }
                //-------------------------------------------------------------------------------


                //------Manhole Locations


                // create the area graph for the ground elevation
                pAreaSeriesProps = pDataGraphT.AddSeries("bar:minmax"); //("scatter_plot")  '("line:vertical")


                pManHoleSeries = (IBarSeriesProperties)pAreaSeriesProps;
                pManHoleSeries.BarStyle = esriBarStyle.esriCylinderBar;
                pManHoleSeries.BarSize = 5;
                pAreaSeriesProps.SourceData = pManholeTable;// pLayer
                pAreaSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                pColor = Globals.GetColor(255, 255, 68);

                pAreaSeriesProps.CustomColor = pColor.RGB; // pSymbol.Color.RGB
                pAreaSeriesProps.InLegend = true; //show legend   ' false = don't show legend


                pAreaSeriesProps.Name = ProfileGraph[CurrentDetail].Point_LayerName;//"Point Locations";

                pAreaSeriesProps.HorizontalAxis = 3;
                pAreaSeriesProps.LabelField = "ID";

                pAreaSeriesProps.SetField(0, "X");
                pAreaSeriesProps.SetField(1, "BOTELEV");
                pAreaSeriesProps.SetField(2, "TOPELEV");

                // sort on the X value

                //pSortFlds = (IDataSortSeriesProperties)pAreaSeriesProps;
                //idx = 0;
                //pSortFlds.AddSortingField("X", true, ref idx);

                //----


                // line series - sewer line

                pColor = Globals.GetColor(76, 230, 0);// green

                pLineSeriesProps = pDataGraphT.AddSeries("line:vertical"); //("area:vertical") '("scatter_plot")  '("line:vertical")

                pLineSeriesProps.SourceData = pMainTable; // pLayer
                pLineSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                pLineSeriesProps.PenProperties.Width = 3;
                pLineSeriesProps.CustomColor = pColor.RGB;  // pSymbol.Color.RGB

                // don't have any symbols on the line, just solid
                pLineSeriesProps2 = (ILineSeriesProperties)pLineSeriesProps;// 'QI
                pLineSeriesProps2.SymbolProperties.Style = esriDataGraphTSymbolType.esriDataGraphTSymbolNothing;

                // get the fields to graph 
                strXDataFldName = "MEASURE";
                strYDataFldName = "ELEVATION";


                pLineSeriesProps.InLegend = true; // show legend   ' false = don't show legend
                pLineSeriesProps.Name = ProfileGraph[CurrentDetail].Line_LayerName;//"Line Profile";// pMainTable.Fields.get_Field(pMainTable.Fields.FindField("ELEVATION")).AliasName;
                //pSeriesProps.LabelField = "OBJECTID"
                pLineSeriesProps.ValueFormat = "0 ";
                pLineSeriesProps.SetField(0, strXDataFldName);// timefldName
                pLineSeriesProps.SetField(1, strYDataFldName);

                // sort on the X value
                pSortFlds = (IDataSortSeriesProperties)pLineSeriesProps;
                pSortFlds.AddSortingField(strXDataFldName, true, ref idx);

                //----------  end line series


                pScatterSeriesProps = pDataGraphT.AddSeries("scatter_plot"); //("area:vertical") '("scatter_plot")  '("line:vertical")

                pScatterSeriesProps.SourceData = pMainLabelTable; // pLayer
                pScatterSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                pScatterSeriesProps.PenProperties.Width = 3;
                pScatterSeriesProps.CustomColor = pColor.RGB;  // pSymbol.Color.RGB

                // don't have any symbols on the line, just solid

                pScatterSeriesProps2 = (IPointSeriesProperties)pScatterSeriesProps;// 'QI

                pScatterSeriesProps2.SymbolProperties.Style = esriDataGraphTSymbolType.esriDataGraphTSymbolCircle;
                pScatterSeriesProps2.SymbolProperties.Visible = false;

                pScatterSeriesProps2.SymbolProperties.Color = pColor.RGB;
                pScatterSeriesProps2.SymbolProperties.BorderProperties.Color = pColor.RGB;
                pScatterSeriesProps2.SymbolProperties.BorderProperties.Style = esriDataGraphTPenType.esriDataGraphTPenClear;

                // get the fields to graph 
                strXDataFldName = "MEASURE";
                strYDataFldName = "ELEVATION";


                pScatterSeriesProps.InLegend = false; // show legend   ' false = don't show legend
                pScatterSeriesProps.Name = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_14");
                //pSeriesProps.LabelField = "OBJECTID"
                pScatterSeriesProps.ValueFormat = " ";
                pScatterSeriesProps.HorizontalAxis = 3;
                pScatterSeriesProps.LabelField = "LABEL";
                pScatterSeriesProps.Marks = true;

                pScatterSeriesProps.SetField(0, strXDataFldName);// timefldName
                pScatterSeriesProps.SetField(1, strYDataFldName);
                //pScatterSeriesProps.SetField(2, "LABEL");
                // sort on the X value
                //  pSortFlds = (IDataSortSeriesProperties)pScatterSeriesProps;
                // pSortFlds.AddSortingField(strXDataFldName, true, ref idx);


                //ISeriesProperties pScatterSeriesProps;

                pScatterSeriesProps = pDataGraphT.AddSeries("scatter_plot"); //("area:vertical") '("scatter_plot")  '("line:vertical")

                pScatterSeriesProps.SourceData = pTapTable; // pLayer

                // get the fields to graph 
                strXDataFldName = "MEASURE";
                strYDataFldName = "ELEVATION";


                pScatterSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                pScatterSeriesProps.PenProperties.Width = 3;
                pScatterSeriesProps.HorizontalAxis = 5;
                pScatterSeriesProps.LabelField = "LABEL";

                if (ProfileGraph[CurrentDetail].PointAlong_ShowLabels.ToUpper() == "TRUE" && ProfileGraph[CurrentDetail].PointAlong_Labels.Length > 0)
                {

                    pScatterSeriesProps.Marks = true;
                }


                pScatterSeriesProps.InLegend = true; // show legend   ' false = don't show legend
                pScatterSeriesProps.Name = ProfileGraph[CurrentDetail].PointAlong_LayerName;//"Points Along";
                //pSeriesProps.LabelField = "OBJECTID"
                pScatterSeriesProps.ValueFormat = " ";

                pScatterSeriesProps.SetField(0, strXDataFldName);// timefldName
                pScatterSeriesProps.SetField(1, strYDataFldName);


                pColor = Globals.GetColor(255, 0, 0);// green

                pScatterSeriesProps.CustomColor = pColor.RGB;  // pSymbol.Color.RGB

                // don't have any symbols on the line, just solid
                //IPointSeriesProperties pScatterSeriesProps2;
                pScatterSeriesProps2 = (IPointSeriesProperties)pScatterSeriesProps;// 'QI

                pScatterSeriesProps2.SymbolProperties.Style = esriDataGraphTSymbolType.esriDataGraphTSymbolStar;
                pScatterSeriesProps2.SymbolProperties.Visible = true;

                pScatterSeriesProps2.SymbolProperties.Color = pColor.RGB;
                pScatterSeriesProps2.SymbolProperties.BorderProperties.Color = pColor.RGB;
                pScatterSeriesProps2.SymbolProperties.BorderProperties.Style = esriDataGraphTPenType.esriDataGraphTPenSolid;


                pDataGraphBase.UseSelectedSet = false;

                pCancelTracker = new CancelTrackerClass();
                pDataGraphT.Update(pCancelTracker);


                // create data graph window

                pDataGraphWin = new DataGraphWindowClass();
                pDataGraphWin.DataGraphBase = pDataGraphBase;
                pDataGraphWin.Application = app;
                // size and position the window
                pDataGraphWin.PutPosition(0, 0, 900, 250);

                // add the graph to the project

                pDataGraphs = (IDataGraphCollection)pMxDoc; //QI
                pDataGraphs.AddDataGraph(pDataGraphBase);
                //IDataGraphT ptmp = (IDataGraphT)pDataGraphs.DataGraph[1];

                //string fld = ptmp.SeriesProperties[5].GetField(0);
                //fld = ptmp.SeriesProperties[5].GetField(1);
                //fld = ptmp.SeriesProperties[5].GetField(2);
                //fld = ptmp.SeriesProperties[5].LabelField;
                //fld = ptmp.SeriesProperties[5].Marks.ToString();
                //   fld = ptmp.SeriesProperties[5].HorizontalAxis;


                pDataGraphT.AxisProperties[0].AutomaticMaximum = true;
                pDataGraphT.AxisProperties[0].AutomaticMinimum = true;
                // pDataGraphT.AxisProperties[0].Minimum = minChartVal - 5;
                //pDataGraphT.AxisProperties[0].Maximum = maxChartVal + 5;
                pDataGraphT.AxisProperties[0].InitDefaults();


                // show the graph
                pDataGraphWin.Show(true);









                //// get an insert cursor for the table
                //pCursor = null;
                //pRowBuff = null;
                //pCursor = pProfTable.Insert(true);

                //// populate the table
                //IPoint pPt;
                //int lPartIndex;
                //int lVertexIndex;
                //IEnumVertex pEnumVertex;

                //pPtCollection = (IPointCollection)pPolyline;
                //pEnumVertex = pPtCollection.EnumVertices;
                //pEnumVertex.Reset();

                //// add the vertex xyz to the new table
                //i = 0;
                //pEnumVertex.Next(out pPt, out lPartIndex, out lVertexIndex);

                //while (pPt != null)
                //{

                //    pRowBuff = pProfTable.CreateRowBuffer();
                //    pRowBuff.set_Value(pRowBuff.Fields.FindField("X"), pPt.X);
                //    pRowBuff.set_Value(pRowBuff.Fields.FindField("Y"), pPt.Y);
                //    pRowBuff.set_Value(pRowBuff.Fields.FindField("Z"), pPt.Z);
                //    pRowBuff.set_Value(pRowBuff.Fields.FindField("M"), pPt.M);

                //    // keep -99999 as a flag, data will be calculated later,
                //    // right now a graph can't ignore null values
                //    if (SewerElevCollFroms[i] == null || SewerElevCollFroms[i].ToString() == "")
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("UPELEV"), -99999);
                //        if (MessageBox.Show("There is a null value in the Upstream Elevations, Continue?", "Missing Data", MessageBoxButtons.YesNo) == DialogResult.No)
                //        {
                //            return;

                //        }
                //    }
                //    else
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("UPELEV"), SewerElevCollFroms[i]);
                //    }

                //    if (SewerElevCollTos[i] == null || SewerElevCollTos[i].ToString() == "")
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("DOWNELEV"), -99999);
                //        if (MessageBox.Show("There is a null value in the Downstream Elevations, Continue?", "Missing Data", MessageBoxButtons.YesNo) == DialogResult.No)
                //        {
                //            return;

                //        }
                //    }
                //    else
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("DOWNELEV"), SewerElevCollTos[i]);
                //    }

                //    if (SewerElevCollRim[i] == null || SewerElevCollRim[i].ToString() == "")
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("RIMELEV"), -99999);
                //        if (MessageBox.Show("There is a null value in the Rim Elevation, Continue?", "Missing Data", MessageBoxButtons.YesNo) == DialogResult.No)
                //        {
                //            return;

                //        }
                //    }

                //    else
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("RIMELEV"), SewerElevCollRim[i]);
                //    }

                //    if (SewerElevCollInvertElev[i] == null || SewerElevCollInvertElev[i].ToString() == "")
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("INVERTELEV"), -99999);
                //        if (MessageBox.Show("There is a null value in the Invert Elevation, Continue?", "Missing Data", MessageBoxButtons.YesNo) == DialogResult.No)
                //        {
                //            return;

                //        }

                //    }
                //    else
                //    {
                //        pRowBuff.set_Value(pRowBuff.Fields.FindField("INVERTELEV"), SewerElevCollInvertElev[i]);
                //    }
                //    // pRowBuff.set_Value(pRowBuff.Fields.FindField("UPELEV"), SewerElevCollFroms[i]);
                //    //pRowBuff.set_Value(pRowBuff.Fields.FindField("DOWNELEV"), SewerElevCollTos[i]);
                //    // pRowBuff.set_Value(pRowBuff.Fields.FindField("RIMELEV"), SewerElevCollRim[i]);
                //    // pRowBuff.set_Value(pRowBuff.Fields.FindField("INVERTELEV"), SewerElevCollInvertElev[i]);

                //    /** use this code if graph can ignore null data
                //    '    ' skip records with -99999 as sewer elev
                //    '  If SewerElevColl(i) <> -99999 Then
                //    '    pRowBuff.Value(pRowBuff.Fields.FindField("SewerElev")) = SewerElevColl(i)
                //    '  Else
                //    '    pRowBuff.Value(pRowBuff.Fields.FindField("SewerElev")) = Null
                //    '  End If*/
                //    pCursor.InsertRow(pRowBuff);
                //    pEnumVertex.Next(out pPt, out lPartIndex, out lVertexIndex);
                //    i++;
                //}
                //// flush any writes
                //pCursor.Flush();
                //Marshal.ReleaseComObject(pCursor);

                ///*
                //  ' Calculate a value for an intermediate point between 2 manholes
                //  ' needed because graph requires a value for every record or it gives it a zero.
                //  ' Sewer lines have values at each manhole but a sewerline is composed of many
                //  ' lines - where the laterals break it.
                //  ' ** this may change if they modify the software to ignore null values in a graph
                //  */



                //// make a cursor of just the records that have a valid sewer elev
                //// needed to get the deltaSewerElev of the sewer elev data


                ////*********
                //string currentField = "UPELEV";
                //for (int k = 0; k < 4; k++)
                //{
                //    switch (k)
                //    {
                //        case 0:
                //            currentField = "UPELEV";
                //            break;
                //        case 1:
                //            currentField = "DOWNELEV";
                //            break;
                //        case 2:
                //            currentField = "RIMELEV";
                //            break;
                //        case 3:
                //            currentField = "INVERTELEV";
                //            break;
                //    }
                //    double Mmin = 0.0;
                //    double Mmax = 0.0;
                //    double minSewerElev = 0.0;
                //    double maxSewerElev = 0.0;
                //    double deltaSewerElev = 0.0;
                //    double deltaM = 0.0;
                //    double newZ = 0.0;
                //    double m = 0.0;
                //    double sewerelev = 0.0;
                //    int j;
                //    IRow pRowSewerElev;

                //    ICursor pCursorSewerElev;
                //    IQueryFilter pQueryFilter;
                //    pQueryFilter = new QueryFilterClass();
                //    pQueryFilter.WhereClause = currentField + " <> -99999";
                //    pCursorSewerElev = pProfTable.Search(pQueryFilter, false);

                //    // recreate the cursor as an update cursor
                //    pCursor = null;
                //    pCursor = pProfTable.Update(null, false);
                //    pRowBuff = pCursor.NextRow();

                //    j = 0;
                //    deltaM = 0;
                //    while (pRowBuff != null)
                //    {
                //        // for the intermediate records, SewerElev will have a value of -99999,
                //        // update them with a calculated value
                //        if ((double)(pRowBuff.get_Value(pRowBuff.Fields.FindField(currentField))) == -99999)
                //        {
                //            m = (double)pRowBuff.get_Value(pRowBuff.Fields.FindField("M"));
                //            newZ = (((m - Mmin) / deltaM) * deltaSewerElev) + sewerelev;
                //            pRowBuff.set_Value(pRowBuff.Fields.FindField(currentField), newZ);
                //            pCursor.UpdateRow(pRowBuff as IRow);
                //        }
                //        else
                //        {
                //            //valid sewer elev record
                //            // calculate the delta sewer elev
                //            if (j == 0)
                //            {
                //                // get the min and max sewer elev values
                //                // get the man and max M value, this is used in the ratio calculation,
                //                //  I can't use the whole line length as the M because the slope of the
                //                //  sewer pipe can change at each manhole so the calculation has to be
                //                //  from manhole to manhole, not the whole line
                //                try
                //                {
                //                    pRowSewerElev = pCursorSewerElev.NextRow();
                //                    minSewerElev = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField(currentField));
                //                    //string tmp = pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField("M")).ToString();
                //                    //if (tmp == "")
                //                    //    Mmin = 0.0;
                //                    //else
                //                    //    Mmin = Convert.ToDouble(tmp);
                //                    Mmin = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField("M"));
                //                    pRowSewerElev = pCursorSewerElev.NextRow();
                //                    maxSewerElev = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField(currentField));
                //                    Mmax = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField("M"));
                //                }
                //                catch (Exception Ex)
                //                {
                //                    MessageBox.Show(Ex.Message);

                //                }

                //            }
                //            else
                //            {
                //                pRowSewerElev = pCursorSewerElev.NextRow();
                //                if (pRowSewerElev == null)
                //                {
                //                    break;
                //                }
                //                minSewerElev = maxSewerElev;
                //                Mmin = Mmax;
                //                maxSewerElev = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField(currentField));
                //                Mmax = (double)pRowSewerElev.get_Value(pRowSewerElev.Fields.FindField("M"));
                //            }
                //            deltaSewerElev = maxSewerElev - minSewerElev;
                //            deltaM = Mmax - Mmin;

                //            // this value is the base value that the calc'd values need as a base
                //            sewerelev = minSewerElev;//pRowBuff.Value(pRowBuff.Fields.FindField("SewerElev"))
                //            j++;
                //        }
                //        pRowBuff = (IRowBuffer)pCursor.NextRow();
                //    }
                //    pCursor.Flush();
                //    Marshal.ReleaseComObject(pCursor);
                //    Marshal.ReleaseComObject(pCursorSewerElev);

                //}


                ////=============================================================================
                //// create the graph from the table

                //IDataGraphBase pDataGraphBase;
                //IDataGraphT pDataGraphT;

                //// create graph
                //pDataGraphBase = new DataGraphTClass();
                //pDataGraphT = (IDataGraphT)pDataGraphBase;

                //// graph, axis and legend titles. Substitute them for different input layer
                //pDataGraphT.GeneralProperties.Title = "Sewer Main Profile";
                //pDataGraphT.LegendProperties.Title = "Profile Legend";



                ////   IDataGraphTAxisProperties pHort = pDataGraphT.AxisProperties[0];

                //pDataGraphT.AxisProperties[0].Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_13");
                //pDataGraphT.AxisProperties[0].AutomaticMaximum = true;
                //pDataGraphT.AxisProperties[0].AutomaticMinimum = true;
                ////pDataGraphT.AxisProperties[0].Minimum = minChartVal - 5;
                ////pDataGraphT.AxisProperties[0].Maximum= maxChartVal + 5;

                ////pDataGraphT.AxisProperties[1].AutomaticMaximum = true;
                ////pDataGraphT.AxisProperties[1].AutomaticMinimum = false;
                ////pDataGraphT.AxisProperties[1].Minimum = minChartVal - 5;
                //////pDataGraphT.AxisProperties[1].Title = "Manholes";
                ////pDataGraphT.AxisProperties[1].Visible = true;

                //pDataGraphT.AxisProperties[3].Visible = true;

                //pDataGraphT.AxisProperties[3].Title = "Manholes";


                //pDataGraphT.AxisProperties[2].Title = "Length (feet)"; // "Date";

                //pDataGraphT.AxisProperties[2].ValueFormat = "0";
                //pDataGraphT.AxisProperties[2].Minimum = 0;
                //pDataGraphT.AxisProperties[2].AutomaticMinimum = false;
                //pDataGraphT.AxisProperties[2].ValueFormat = "#,##0.###";
                //pDataGraphBase.Name = "Sewer Main Profile Graph"; // & strTableName  layerName;
                ////IDataGraphTGeneralProperties pGenProp = pDataGraphT.GeneralProperties;
                ////pGenProp.Show3D = true;


                //// put the legend below the graph
                ////pDataGraphT.LegendProperties.Alignment = esriDataGraphTLegendBottom

                //// use only for series-specific properties
                ////IPointSeriesProperties pPtSeries;
                ////IAreaSeriesProperties pAreaSeries;
                ////ILineSeriesProperties pLineSeries;

                ////-------------------------------------------------------------------------------
                //// area series - ground elevation
                //ISeriesProperties pAreaSeriesProps;
                //IColor pColor;

                //// create the area graph for the ground elevation
                //pAreaSeriesProps = pDataGraphT.AddSeries("area:vertical"); //("scatter_plot")  '("line:vertical")
                //pAreaSeriesProps.SourceData = pProfTable;// pLayer
                //pAreaSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                //pColor = Globals.GetColor(137, 112, 68);

                //pAreaSeriesProps.CustomColor = pColor.RGB; // pSymbol.Color.RGB

                //String strXDataFldName;
                //String strYDataFldName;

                //// get the fields to graph - ground elevation
                //strXDataFldName = "M"; //pTable.Fields.Field(i).Name ' "Dist_to_Rds"
                //strYDataFldName = "Z"; // pTable.Fields.Field(i).Name ' "Dist_to_Rds"
                ////timefldName = "TSDateTime"   ' substitute data/time field name for different dataset
                ////gageIDFldName = "Name"         ' substitute gage ID field name for different dataset

                ////  pSeriesProps.whereClause = whereClause
                //pAreaSeriesProps.InLegend = true; //show legend   ' false = don't show legend
                //pAreaSeriesProps.Name = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsAlias_1");
                ////pSeriesProps.LabelField = "OBJECTID";
                //pAreaSeriesProps.ValueFormat = "0 ";
                //pAreaSeriesProps.SetField(0, strXDataFldName); // timefldName
                //pAreaSeriesProps.SetField(1, strYDataFldName);

                //// sort on the X value
                //IDataSortSeriesProperties pSortFlds;
                //pSortFlds = (IDataSortSeriesProperties)pAreaSeriesProps;
                //int idx = 0;
                //pSortFlds.AddSortingField(strXDataFldName, true, ref idx);

                ////-------------------------------------------------------------------------------


                ////------Manhole Locations

                //IBarSeriesProperties pManHoleSeries;

                //// create the area graph for the ground elevation
                //pAreaSeriesProps = pDataGraphT.AddSeries("bar:minmax"); //("scatter_plot")  '("line:vertical")


                //pManHoleSeries = (IBarSeriesProperties)pAreaSeriesProps;
                //pManHoleSeries.BarStyle = esriBarStyle.esriCylinderBar;
                //pManHoleSeries.BarSize = 5;
                //pAreaSeriesProps.SourceData = pManTable;// pLayer
                //pAreaSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                //pColor = Globals.GetColor(255, 255, 68);

                //pAreaSeriesProps.CustomColor = pColor.RGB; // pSymbol.Color.RGB
                //pAreaSeriesProps.InLegend = true; //show legend   ' false = don't show legend

                //pAreaSeriesProps.Name = "Manhole Locations";

                //pAreaSeriesProps.HorizontalAxis = 3;
                //pAreaSeriesProps.LabelField = "ID";

                //pAreaSeriesProps.SetField(0, "X");
                //pAreaSeriesProps.SetField(1, "INVERTELEV");
                //pAreaSeriesProps.SetField(2, "RIMELEV");

                //// sort on the X value

                ////pSortFlds = (IDataSortSeriesProperties)pAreaSeriesProps;
                ////idx = 0;
                ////pSortFlds.AddSortingField("X", true, ref idx);

                ////----


                //// line series - sewer line
                //currentField = "UPELEV";
                //int currentFieldWidth = 1;
                //for (int k = 3; k >= 0; k--)
                //{

                //    switch (k)
                //    {
                //        case 0:
                //            currentField = "UPELEV";
                //            pColor = Globals.GetColor(76, 230, 0);// green
                //            currentFieldWidth = 3;
                //            break;
                //        case 1:
                //            currentField = "DOWNELEV";
                //            pColor = Globals.GetColor(76, 0, 255);
                //            currentFieldWidth = 3;

                //            break;
                //        case 2:
                //            currentField = "RIMELEV";
                //            pColor = Globals.GetColor(76, 230, 255);
                //            currentFieldWidth = 3;
                //            continue;
                //        //break;
                //        case 3:
                //            currentField = "INVERTELEV";
                //            pColor = Globals.GetColor(255, 230, 0);
                //            currentFieldWidth = 3;
                //            break;
                //    }

                //    ISeriesProperties pLineSeriesProps;

                //    pLineSeriesProps = pDataGraphT.AddSeries("line:vertical"); //("area:vertical") '("scatter_plot")  '("line:vertical")

                //    pLineSeriesProps.SourceData = pProfTable; // pLayer
                //    pLineSeriesProps.ColorType = esriGraphColorType.esriGraphColorCustomAll;
                //    pLineSeriesProps.PenProperties.Width = currentFieldWidth;
                //    pLineSeriesProps.CustomColor = pColor.RGB;  // pSymbol.Color.RGB

                //    // don't have any symbols on the line, just solid
                //    ILineSeriesProperties pLineSeriesProps2;
                //    pLineSeriesProps2 = (ILineSeriesProperties)pLineSeriesProps;// 'QI
                //    pLineSeriesProps2.SymbolProperties.Style = esriDataGraphTSymbolType.esriDataGraphTSymbolNothing;

                //    // get the fields to graph 
                //    strXDataFldName = "M";
                //    strYDataFldName = currentField;


                //    pLineSeriesProps.InLegend = true; // show legend   ' false = don't show legend
                //    pLineSeriesProps.Name = pProfTable.Fields.get_Field(pProfTable.Fields.FindField(currentField)).AliasName;
                //    //pSeriesProps.LabelField = "OBJECTID"
                //    pLineSeriesProps.ValueFormat = "0 ";
                //    pLineSeriesProps.SetField(0, strXDataFldName);// timefldName
                //    pLineSeriesProps.SetField(1, strYDataFldName);

                //    // sort on the X value
                //    pSortFlds = (IDataSortSeriesProperties)pLineSeriesProps;
                //    pSortFlds.AddSortingField(strXDataFldName, true, ref idx);

                //    //----------  end line series

                //}



                //pDataGraphBase.UseSelectedSet = false;

                //ITrackCancel pCancelTracker;
                //pCancelTracker = new CancelTrackerClass();
                //pDataGraphT.Update(pCancelTracker);
                ////pDataGraphT.get_AxisProperties(0).AutomaticMaximum = true;
                ////pDataGraphT.get_AxisProperties(0).AutomaticMinimum= true;
                ////pDataGraphT.get_AxisProperties(1).AutomaticMaximum = true;
                ////pDataGraphT.get_AxisProperties(1).AutomaticMinimum = true;
                ////pDataGraphT.get_AxisProperties(2).AutomaticMaximum = true;
                ////pDataGraphT.get_AxisProperties(2).AutomaticMinimum = true;
                ////pDataGraphT.get_AxisProperties(3).AutomaticMaximum = true;
                ////pDataGraphT.get_AxisProperties(3).AutomaticMinimum = true;


                //// create data graph window
                //IDataGraphWindow2 pDataGraphWin;
                //pDataGraphWin = new DataGraphWindowClass();
                //pDataGraphWin.DataGraphBase = pDataGraphBase;
                //pDataGraphWin.Application = app;
                //// size and position the window
                //pDataGraphWin.PutPosition(0, 0, 900, 250);

                //// add the graph to the project
                //IDataGraphCollection pDataGraphs;
                //pDataGraphs = (IDataGraphCollection)pMxDoc; //QI
                //pDataGraphs.AddDataGraph(pDataGraphBase);
                ////IDataGraphT ptmp = (IDataGraphT)pDataGraphs.DataGraph[1];

                ////string fld = ptmp.SeriesProperties[5].GetField(0);
                ////fld = ptmp.SeriesProperties[5].GetField(1);
                ////fld = ptmp.SeriesProperties[5].GetField(2);
                ////fld = ptmp.SeriesProperties[5].LabelField;
                ////fld = ptmp.SeriesProperties[5].Marks.ToString();
                ////   fld = ptmp.SeriesProperties[5].HorizontalAxis;




                //// show the graph
                //pDataGraphWin.Show(true);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ProfileCreateGraph " + Ex.Message);
            }
            finally
            {

                pWS = null;
                pCursor = null;
                pMxDoc = null;
                pMap = null;
                pPolyLineZ = null;


                pZAwareLineZ = null;
                pSurface = null;
                pRasterLayer = null;


                pRasLay = null;
                pPtOrigFrom = null;
                pPtOrigTo = null;
                pStandAloneTabColl = null;
                pStandAloneTabMainLabel = null;
                pTapTable = null;
                pMainTable = null;
                pManholeTable = null;

                pSurfaceTable = null;

                pMainLabelTable = null;
                pTableFieldsMainLabel = null;
                pStandAloneTabMain = null;
                pTableFieldsMain = null;
                pStandAloneTabManhole = null;
                pTableFieldsManhole = null;
                pStandAloneTabSurface = null;
                pTableFieldsSurface = null;
                pStandAloneTabTap = null;
                pTableFieldsTap = null;
                pRowBuff = null;
                pLabelCursor = null;

                pTapCursor = null;
                pSegment = null;
                pLine = null;
                pFromPnt = null;
                pToPnt = null;
                pMidPnt = null;
                pDataGraphBase = null;
                pDataGraphT = null;

                pPtCollection = null;
                pEnumVertex = null;
                pPt = null;
                pAreaSeriesProps = null;
                pColor = null;


                pSortFlds = null;
                pScatterSeriesProps2 = null;
                pScatterSeriesProps = null;
                pManHoleSeries = null;
                pLineSeriesProps2 = null;

                pLineSeriesProps = null;
                pCancelTracker = null;
                pDataGraphWin = null;
                pDataGraphs = null;
            }

        }


        public static void CalculateFlowAccum(List<FlowLayerDetails> sumFlowAcc, IApplication app)
        {

            IMap pMap = null;


            IFeatureLayer pFLayer = null;
            IFeatureSelection pFSel = null;

            IFeatureCursor pFCursor = null;
            ICursor pCursor = null;
            IFeature pFeature = null;

            IEditor pEditor = null;


            string retAcc = "";
            int lSumFieldLoc;

            IProgressDialogFactory pProDFact;
            IStepProgressor pStepPro;
            IProgressDialog2 pProDlg = null;
            ITrackCancel pTrkCan;





            try
            {
                if (app == null)
                    return;
                pMap = ((app.Document as IMxDocument).FocusMap);
                if (pMap == null)
                    return;

                pEditor = Globals.getEditor(app);
                if (pEditor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"));
                    return;
                }
                bool boolCont = true;
                // Create a CancelTracker  
                pTrkCan = new CancelTrackerClass();
                // Create the ProgressDialog. This automatically displays the dialog  
                pProDFact = new ProgressDialogFactoryClass();
                pProDlg = (IProgressDialog2)pProDFact.Create(pTrkCan, 0);
                pProDlg.CancelEnabled = true;

                pProDlg.Animation = esriProgressAnimationTypes.esriProgressGlobe;

                // Set the properties of the Step Progressor  
                pStepPro = (IStepProgressor)pProDlg;

                foreach (FlowLayerDetails sumAcc in sumFlowAcc)
                {
                    bool boolFoundAsLayer = true;

                    pFLayer = Globals.FindLayer(app, sumAcc.LayerName, ref boolFoundAsLayer) as IFeatureLayer;
                    if (pFLayer == null)
                    {
                        //MessageBox.Show(sumAcc.LayerName + " feature layer not found.\nAny selected features in this layer will be analyzed for acculmuation.");
                        continue;
                    }

                    if (pFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(sumAcc.LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20a"));
                        continue;
                    }

                    if (Globals.IsEditable(ref pFLayer, ref pEditor) == false)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20b") + sumAcc.LayerName + ".");
                    }

                    pFSel = pFLayer as IFeatureSelection;

                    // Verify that the layer has some features selected
                    if (pFSel.SelectionSet.Count < 1)
                    {
                        //MessageBox.Show(sumAcc.LayerName + " layer must have some features selected.");
                        continue;
                    }


                    lSumFieldLoc = pFLayer.FeatureClass.Fields.FindField(sumAcc.SumFlowField);
                    if (lSumFieldLoc == -1)
                    {
                        MessageBox.Show(sumAcc.LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20d") + sumAcc.SumFlowField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20e"));
                        return;
                    }

                    // Step through each selected feature in the selection set
                    pFSel.SelectionSet.Search(null, false, out pCursor);

                    pFCursor = pCursor as IFeatureCursor;



                    // Set the properties of the ProgressDialog  
                    pProDlg.Description = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_20a") + sumAcc.LayerName;
                    pProDlg.Title = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_20a") + sumAcc.LayerName;


                    pStepPro.MinRange = 0;
                    pStepPro.MaxRange = pFSel.SelectionSet.Count;
                    pStepPro.StepValue = 1;
                    pStepPro.Position = 0;
                    pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_20a") + sumAcc.LayerName + " 0" + A4LGSharedFunctions.Localizer.GetString("OutOf") + pFSel.SelectionSet.Count;


                    try
                    {
                        pEditor.StartOperation();
                    }
                    catch
                    {
                        pEditor.AbortOperation();
                        pEditor.StartOperation();
                    }


                    int cnt = 1;
                    while ((pFeature = pFCursor.NextFeature()) != null)
                    {

                        pStepPro.Message = A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_20a") + sumAcc.LayerName + cnt + A4LGSharedFunctions.Localizer.GetString("OutOf") + pFSel.SelectionSet.Count;
                        pStepPro.Step();
                        boolCont = pTrkCan.Continue();

                        if (!boolCont)
                        {

                            pStepPro.Hide();
                            pProDlg.HideDialog();
                            pStepPro = null;
                            pProDlg = null;
                            pProDFact = null;
                            return;
                        }

                        retAcc = Globals.ReturnAccumulation(ref app, ref pFeature, sumAcc.WeightName, sumAcc.FlowDirection);
                        if (Globals.IsNumeric(retAcc))
                        {
                            pFeature.set_Value(lSumFieldLoc, retAcc);
                            pFeature.Store();
                        }
                        else
                        {

                        }

                        cnt++;
                    }

                    pEditor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_20a") + sumAcc.LayerName);



                }


                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsDone_20b"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_20a") + "\n" + ex.Message);
            }
            finally
            {

                //pStepPro.Hide();
                if (pProDlg != null)
                    pProDlg.HideDialog();
                pProDFact = null;
                pStepPro = null;
                pProDlg = null;
                pTrkCan = null;



                pMap = null;
                pFLayer = null;

                pFSel = null;
                if (pFCursor != null)
                    Marshal.ReleaseComObject(pFCursor);
                pFCursor = null;
                if (pCursor != null)
                    Marshal.ReleaseComObject(pCursor);
                pCursor = null;
                pFeature = null;
                pEditor = null;

            }

        }

        public static void CalculateFlowAccumAtLocation(List<FlowLayerDetails> sumFlowAcc, IApplication app, double snapTol)
        {

            IMap pMap = null;


            IFeatureLayer pFLayer = null;

            IFeatureCursor pFCursor = null;
            IFeature pFeature = null;

            IPointToEID pPointToEid = null;

            string retAcc = "";
            int lSumFieldLoc;

            int lEID;
            IPoint pSnappedPoint = null;
            INetElements pNetElements = null;
            try
            {
                if (app == null)
                    return;
                pMap = ((app.Document as IMxDocument).FocusMap);
                if (pMap == null)
                    return;
                if (sumFlowAcc == null)
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20c"));
                if (sumFlowAcc.Count == 0)
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20c"));
                bool flowCalced = false;
                foreach (FlowLayerDetails sumAcc in sumFlowAcc)
                {
                    if (flowCalced == true)
                        break;

                    bool boolFoundAsLayer = true;

                    pFLayer = Globals.FindLayer(app, sumAcc.LayerName, ref boolFoundAsLayer) as IFeatureLayer;
                    if (pFLayer == null)
                    {
                        //MessageBox.Show(sumAcc.LayerName + " feature layer not found.\nAny selected features in this layer will be analyzed for acculmuation.");
                        continue;
                    }

                    if (pFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(sumAcc.LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20a"));
                        continue;
                    }


                    lSumFieldLoc = pFLayer.FeatureClass.Fields.FindField(sumAcc.SumFlowField);
                    if (lSumFieldLoc == -1)
                    {
                        MessageBox.Show(sumAcc.LayerName + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20b") + sumAcc.SumFlowField + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20e"));
                        return;
                    }





                    lEID = -1;
                    //Convert pixcel search distance to real world (map units)
                    //dRWSearchTolerance = c_iSearchTolerance;

                    //Find the closest network junction element to the user specified point

                    pPointToEid = new PointToEIDClass();
                    pPointToEid.GeometricNetwork = ((INetworkClass)pFLayer.FeatureClass).GeometricNetwork;
                    pPointToEid.SourceMap = pMap;
                    pPointToEid.SnapTolerance = snapTol;
                    bool bTest = false;
                    if (pFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline || pFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryLine)
                    {
                        double dblPercAlong = 0;

                        pPointToEid.GetNearestEdge((app.Document as IMxDocument).CurrentLocation, out lEID, out pSnappedPoint, out dblPercAlong);


                        if (pSnappedPoint == null)
                            continue;
                        if (lEID == 0)
                            continue;
                        //Get the complete id for the network element
                        pNetElements = (((INetworkClass)pFLayer.FeatureClass).GeometricNetwork.Network) as INetElements;



                        bTest = pNetElements.IsValidElement(lEID, esriElementType.esriETEdge);
                        pFeature = Globals.GetFeatureByEID(lEID, ((INetworkClass)pFLayer.FeatureClass).GeometricNetwork, pMap, esriElementType.esriETEdge);
                    }
                    else if (pFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        pPointToEid.GetNearestJunction((app.Document as IMxDocument).CurrentLocation, out lEID, out pSnappedPoint);


                        if (pSnappedPoint == null)
                            continue;
                        if (lEID == 0)
                            continue;
                        //Get the complete id for the network element
                        pNetElements = (((INetworkClass)pFLayer.FeatureClass).GeometricNetwork.Network) as INetElements;



                        bTest = pNetElements.IsValidElement(lEID, esriElementType.esriETJunction);
                        pFeature = Globals.GetFeatureByEID(lEID, ((INetworkClass)pFLayer.FeatureClass).GeometricNetwork, pMap, esriElementType.esriETJunction);
                    }
                    if (bTest && pFeature != null)
                    {


                        retAcc = Globals.ReturnAccumulation(ref app, ref pFeature, sumAcc.WeightName, sumAcc.FlowDirection);
                        if (Globals.IsNumeric(retAcc))
                        {
                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsMess_20b") + pFeature.OID + A4LGSharedFunctions.Localizer.GetString("In") + pFLayer.Name + A4LGSharedFunctions.Localizer.GetString("Is") + retAcc + ".");
                            //pFeature.set_Value(lSumFieldLoc, retAcc);
                            //pFeature.Store();
                        }
                    }
                    flowCalced = true;


                }

                if (flowCalced == false)
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeoNetToolsError_20f"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("GeoNetToolsLbl_20a") + "\n" + ex.Message);
            }
            finally
            {


                pMap = null;
                pFLayer = null;
                if (pFCursor != null)
                    Marshal.ReleaseComObject(pFCursor);
                pFCursor = null;
                pFeature = null;

                pSnappedPoint = null;
                pNetElements = null;
            }

        }

    }
}
