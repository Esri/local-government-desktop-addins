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
using System.Collections;
using System.Collections.Generic;

using System.Drawing;
using System.Runtime.InteropServices;

using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    //public class LayersAndDetails
    //{
    //    private string m_Layer;
    //    private string m_QueryString;
    //    private double m_ZoomScale;
    //    public LayersAndDetails(string Layer, string QueryString, string ZoomScale)
    //    {
    //        m_Layer = Layer;
    //        m_QueryString = QueryString;
    //        if (Globals.IsDouble(ZoomScale))
    //        {
    //            m_ZoomScale = Convert.ToDouble( ZoomScale);
    //        }

    //    }
    //    public string QueryAndZoom
    //    {
    //        get { return m_QueryString + "," + m_ZoomScale; }
    //    }
    //    public double ZoomScale
    //    {
    //        get { return m_ZoomScale; }
    //        set { m_ZoomScale = value; }
    //    }
    //    public string Layer
    //    {
    //        get { return m_Layer; }
    //        set { m_Layer = value; }
    //    }
    //    public string QueryString
    //    {
    //        get { return m_QueryString; }
    //        set { m_QueryString = value; }
    //    }



    //}
    public class DataTools
    {

        public static ISpatialReferenceFactory spatRefFact = new SpatialReferenceEnvironmentClass();
       
        public static IGeographicCoordinateSystem srWGS84 = spatRefFact.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
       
        private static IApplication _app;
        private Excel.Application ExcelApp;
        private Excel.Workbook objBook;
        private Excel.Worksheet objSheet;

        private string messageBoxHeader = A4LGSharedFunctions.Localizer.GetString("DataToolsLbl1");

        public DataTools(IApplication app)
        {
            _app = app;
            //    _editor = Globals.getEditor(_app);

        }
        public void ExportSelectedRecordsToExcel()
        {
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            IMxDocument mxdoc = null;
            IMap map = null;
            IStandaloneTableCollection standTabColl = null;
            IStandaloneTable standTable = null;
            ITableSelection tableSel = null;
            IEnumFeature enumFeat = null;
            IFeature feat = null;
            IFeatureSelection featSel = null;
            ITable openTable = null;
            object missing = null;
            //object fileName = null;
            object newTemplate = null;
            object docType = null;
            object isVisible = null;


            UID geoFeatureLayerID = null;

            IEnumLayer enumLayer = null;
            IFeatureLayer featlayer = null;
            try
            {


                mxdoc = (IMxDocument)_app.Document;
                map = mxdoc.FocusMap;

                long standTableCnt;
                int i = 0;
                bool selectionInTable = false;


                missing = System.Reflection.Missing.Value;
                // fileName = "normal.dot";
                newTemplate = false;
                docType = 0;
                isVisible = true;

                //Get enumeration of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(geoFeatureLayerID, true);

                enumLayer.Reset();

                standTabColl = (IStandaloneTableCollection)map;
                standTableCnt = standTabColl.StandaloneTableCount;

                enumFeat = (IEnumFeature)map.FeatureSelection;
                enumFeat.Reset();
                feat = enumFeat.Next();



                if (standTableCnt > 0)
                {
                    for (int j = 0; j < standTableCnt; j++)
                    {
                        standTable = standTabColl.get_StandaloneTable(j);
                        if (standTable.Valid)
                        {
                            openTable = (ITable)standTable;
                            tableSel = (ITableSelection)openTable;
                            if (tableSel.SelectionSet.Count > 0)
                            {
                                selectionInTable = true;
                                break;
                            }
                        }
                    }
                }

                if ((selectionInTable == false) && (feat == null))
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("DataToolsMess_1") + Environment.NewLine + A4LGSharedFunctions.Localizer.GetString("DataToolsMess_2"), messageBoxHeader);
                    return;
                }
                // Create a CancelTracker
                ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = _app.hWnd;

                progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);



                stepProgressor.MinRange = 0;
                stepProgressor.MaxRange = mxdoc.FocusMap.SelectionCount;
                stepProgressor.StepValue = 1;
                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("ExportXL");
                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = true;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ExportAsset") + "1" + A4LGSharedFunctions.Localizer.GetString("Of") + mxdoc.FocusMap.SelectionCount.ToString() + ".";
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("ExportXL");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;
                progressDialog.ShowDialog();

                if ((feat != null) && (selectionInTable == true))
                {
                    ExcelApp = new Excel.ApplicationClass();


                    //Delete default worksheets
                    ExcelApp.DisplayAlerts = false;
                    objBook = ExcelApp.Workbooks.Add(missing);
                    //objBook = ExcelApp.Workbooks.get_Item(1);
                    for (int m = objBook.Sheets.Count; m > 1; m--)
                    {

                        objSheet = (Excel.Worksheet)objBook.Sheets.get_Item(m);
                        objSheet.Delete();

                    }

                    featlayer = (IFeatureLayer)enumLayer.Next();
                    while (featlayer != null)
                    {
                        if ((featlayer.Valid) && (featlayer.Selectable))
                        {
                            featSel = (IFeatureSelection)featlayer;
                            if (featSel.SelectionSet.Count > 0)
                            {
                                if (ExportLayer(objBook, mxdoc, featlayer, ref progressDialog, ref stepProgressor, ref trackCancel) == false)
                                {
                                    return;
                                }
                            }
                        }
                        featlayer = (IFeatureLayer)enumLayer.Next();
                    }

                    for (i = 0; i < standTableCnt; i++)
                    {
                        standTable = standTabColl.get_StandaloneTable(i);
                        if (standTable.Valid)
                        {
                            openTable = (ITable)standTable;
                            tableSel = (ITableSelection)openTable;
                            if (tableSel.SelectionSet.Count > 0)
                            {
                                if (ExportTable(objBook, mxdoc, standTable, ref progressDialog, ref stepProgressor,ref trackCancel) == false)
                                {
                                return;
                                }
                            }
                        }
                    }
                }
                else if ((feat != null) && (selectionInTable == false))
                {

                    ExcelApp = new Excel.ApplicationClass();

                    //Delete default worksheets
                    ExcelApp.DisplayAlerts = false;
                    objBook = ExcelApp.Workbooks.Add(missing);
                    //objBook = ExcelApp.Workbooks.get_Item(1);
                    for (int m = objBook.Sheets.Count; m > 1; m--)
                    {

                        objSheet = (Excel.Worksheet)objBook.Sheets.get_Item(m);
                        objSheet.Delete();

                    }


                    ILayer pTLay = enumLayer.Next();
                    while (pTLay != null)
                    {
                        if (pTLay is IFeatureLayer)
                        {
                            featlayer = (IFeatureLayer)pTLay;
                            if ((featlayer.Valid) )
                            {
                                featSel = (IFeatureSelection)featlayer;
                                if (featSel.SelectionSet.Count > 0)
                                {
                                    if (ExportLayer(objBook, mxdoc, featlayer, ref progressDialog, ref stepProgressor, ref trackCancel) == false)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        pTLay = enumLayer.Next();

                    }
                    pTLay = null;

                }
                else if ((feat == null) && (selectionInTable == true))
                {
                    ExcelApp = new Excel.ApplicationClass();

                    //Delete default worksheets
                    ExcelApp.DisplayAlerts = false;
                    objBook = ExcelApp.Workbooks.Add(missing);
                    //objBook = ExcelApp.Workbooks.get_Item(1);
                    for (int m = objBook.Sheets.Count; m > 1; m--)
                    {

                        objSheet = (Excel.Worksheet)objBook.Sheets.get_Item(m);
                        objSheet.Delete();

                    }
                    for (i = 0; i < standTableCnt; i++)
                    {
                        standTable = standTabColl.get_StandaloneTable(i);
                        if (standTable.Valid)
                        {
                            openTable = (ITable)standTable;
                            tableSel = (ITableSelection)openTable;
                            if (tableSel.SelectionSet.Count > 0)
                            {
                                if (ExportTable(objBook, mxdoc, standTable, ref progressDialog, ref stepProgressor, ref trackCancel) == false)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }


                if (objBook.Sheets.Count > 1)
                {
                    objSheet = (Excel.Worksheet)objBook.Sheets.get_Item(1);
                    objSheet.Delete();
                }
                //objSheet = (Excel.Worksheet)objBook.Sheets["Sheet2"];
                //objSheet.Delete();
                //objSheet = (Excel.Worksheet)objBook.Sheets["Sheet3"];
                //objSheet.Delete();
                ExcelApp.DisplayAlerts = true;

                //Make the first sheet active
                objSheet = (Excel.Worksheet)objBook.Sheets.get_Item(1) as Excel.Worksheet;
                (objSheet as Microsoft.Office.Interop.Excel._Worksheet).Activate();

                //Make Excel visible
                ExcelApp.Visible = true;
                return;
            }
            catch (Exception ex)
            {
                if (ExcelApp != null)
                    ExcelApp = null;

                //ExcelApp.Visible = true;
                MessageBox.Show("ExportSelectedRecordsToExcel\n" + ex.Message, ex.Source);
                return;
            }
            finally
            {
                if (progressDialog != null)
                    progressDialog.HideDialog();

                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog = null;
                mxdoc = null;
                map = null;
                standTabColl = null;
                standTable = null;
                tableSel = null;
                enumFeat = null;
                feat = null;
                featSel = null;
                openTable = null;
                missing = null;
                //fileName = null;
                newTemplate = null;
                docType = null;
                isVisible = null;


                geoFeatureLayerID = null;

                enumLayer = null;
                featlayer = null;
            }
        }
        private bool ExportLayer(Excel.Workbook ExcelWbk, IMxDocument MxDoc, IFeatureLayer FLayer, ref IProgressDialog2 progressDialog, ref IStepProgressor stepProgressor, ref ITrackCancel trackCancel)
        {


            IFeatureClass FC = null;
            ISubtypes Subtypes = null;

            ITableProperties TableProperties = null;
            IEnumTableProperties EnumTableProperties = null;
            ITableProperty3 TableProperty = null;
            ITableCharacteristics TableCharacteristics = null;
            IFeatureSelection FeatSel = null;
            IDisplayTable DisplayTable = null;
            ITable Table = null;
            ILayerFields LayerFields = null;
            ITableFields TableFields = null;
            ILayer LayerTest = null;

            ICursor Cursor = null;
            IFeatureCursor FCursor = null;
            IField CurField = null;
            IDomain Domain = null;
            ICodedValueDomain CodedValueDomain = null;
            IFeature Feat = null;
            List<IPoint> pGeo = null;

            object missing = null;
            object sheet = null;
            Excel.Worksheet ExcelSheet = null;
            Excel.Range ExcelRange = null;
            Microsoft.Office.Interop.Excel.Style style = null;

            try
            {

                int Col = 0;
                int Row = 0;
                int i;
                int j;
                bool UseDescriptions;
                int subtype;
                string SheetName;

                missing = System.Reflection.Missing.Value;
                sheet = ExcelWbk.Sheets[ExcelWbk.Sheets.Count];

                //Add new Excel worksheet
                ExcelSheet = (Excel.Worksheet)ExcelWbk.Sheets.Add(missing, sheet, missing, missing);
                SheetName = FLayer.Name;
                if (SheetName.Length > 30)
                {
                    SheetName = SheetName.Substring(0, 30);
                }
                ExcelSheet.Name = SheetName;

                //style = ExcelWbk.Styles.Add("Style1");
                //style.NumberFormat = "@";

                LayerTest = (ILayer)FLayer;

                //Get Subtype info
                FC = FLayer.FeatureClass;
                Subtypes = FC as ISubtypes;
                //Determine whether to use descriptions or codes for domains and subtypes
                UseDescriptions = true;
                TableProperties = MxDoc.TableProperties;
                EnumTableProperties = (IEnumTableProperties)TableProperties;
                EnumTableProperties.Reset();
                TableProperty = (ITableProperty3)EnumTableProperties.Next();
                while (TableProperty != null && TableProperty.Layer != null)  //Fixed
                {
                    if (TableProperty.Layer.Equals(LayerTest))
                    {
                        TableCharacteristics = (ITableCharacteristics)TableProperty;
                        UseDescriptions = TableCharacteristics.ShowCodedValueDomainDescriptions;
                    }
                    TableProperty = (ITableProperty3)EnumTableProperties.Next();
                }
                FeatSel = (IFeatureSelection)FLayer;

                DisplayTable = (IDisplayTable)FLayer;
                Table = (ITable)DisplayTable.DisplayTable;

                //Get TableFields so later we can determine whether that field is visible
                LayerFields = (ILayerFields)FLayer;
                TableFields = (ITableFields)FLayer;

                //loop through each field and write column headings
                Row = 1;
                for (j = 0; j < TableFields.FieldCount; j++)
                {
                    CurField = TableFields.get_Field(j);

                    //skip blob and geometry fields
                    if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                    {
                        Col += 1;
                        //Write field alias name as Excel column header
                        ExcelSheet.Cells[Row, Col] = TableFields.get_FieldInfo(j).Alias;
                        if (CurField.Type == esriFieldType.esriFieldTypeString)
                            ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[65535, Col]).EntireColumn.NumberFormat = "@";

                    }
                }
                Col += 1;
                ExcelSheet.Cells[Row, Col] = "X_COORD";
                Col += 1;
                ExcelSheet.Cells[Row, Col] = "Y_COORD";
                Col += 1;
                ExcelSheet.Cells[Row, Col] = "Lat";
                Col += 1;
                ExcelSheet.Cells[Row, Col] = "Long";
                IField pFieldTest = FLayer.FeatureClass.Fields.get_Field(FLayer.FeatureClass.Fields.FindField(FLayer.FeatureClass.ShapeFieldName));
                IGeometryDef pGeometryDefTest = null;
                pGeometryDefTest = pFieldTest.GeometryDef;
                bool bZAware = false;
                bool bMAware = false;
                //Determine if M or Z aware
                if (pGeometryDefTest.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    bZAware = pGeometryDefTest.HasZ;
                    bMAware = pGeometryDefTest.HasM;
                }
                if (bZAware)
                {
                    Col += 1;
                    ExcelSheet.Cells[Row, Col] = "Z_COORD";
                }
                pFieldTest = null;
                pGeometryDefTest = null;
                //Get all selected records for this table (use IDisplayTable to get any joined data)
                DisplayTable.DisplaySelectionSet.Search(null, true, out Cursor);
                FCursor = (IFeatureCursor)Cursor;

                //subtype = Subtypes.DefaultSubtypeCode;

                //For each selected record

                Feat = FCursor.NextFeature();
                //  stepProgressor.Step();
                // progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ExportAsset") + stepProgressor.Position + A4LGSharedFunctions.Localizer.GetString("Of") + MxDoc.FocusMap.SelectionCount.ToString() + ".";

                while (Feat != null)
                {
                    Row += 1;

                    if (Subtypes != null && Subtypes.HasSubtype == true &&
                       (Feat.get_Value(Subtypes.SubtypeFieldIndex) != null))
                    {
                        subtype = Convert.ToInt32(Feat.get_Value(Subtypes.SubtypeFieldIndex));
                    }
                    else
                    {
                        subtype = -99999;
                    }


                    //For each column
                    Col = 0;
                    for (j = 0; j < TableFields.FieldCount; j++)
                    {
                        CurField = TableFields.get_Field(j);

                        //skip blob and geometry fields in data also
                        if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                        {
                            Col += 1;
                            ExcelSheet.Cells[Row, Col] = Feat.get_Value(j);

                            if (UseDescriptions == true && subtype == -99999)
                            {
                                Domain = CurField.Domain;
                                if (Domain != null)
                                {
                                    if (Domain.Type == esriDomainType.esriDTCodedValue)
                                    {
                                        CodedValueDomain = (ICodedValueDomain)CurField.Domain;
                                        for (i = 0; i < CodedValueDomain.CodeCount; i++)
                                        {
                                            if ((CodedValueDomain.get_Value(i)).ToString() == (Feat.get_Value(j)).ToString())
                                            {
                                                //System.Diagnostics.Debug.Print(CodedValueDomain.get_Name(0).ToString());
                                                ExcelSheet.Cells[Row, Col] = CodedValueDomain.get_Name(i);
                                                i = CodedValueDomain.CodeCount;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (UseDescriptions == true && subtype != -99999)
                            {
                                if (Subtypes.SubtypeFieldIndex == j)
                                {
                                    ExcelSheet.Cells[Row, Col] = Subtypes.get_SubtypeName(subtype);
                                }
                                else
                                {
                                    Domain = Subtypes.get_Domain(subtype, CurField.Name);
                                    if ((Domain != null) && (Domain.Type == esriDomainType.esriDTCodedValue))
                                    {
                                        CodedValueDomain = (ICodedValueDomain)Domain;
                                        for (i = 0; i < CodedValueDomain.CodeCount; i++)
                                        {
                                            if ((CodedValueDomain.get_Value(i)).ToString() == (Feat.get_Value(j)).ToString())
                                            {
                                                //System.Diagnostics.Debug.Print(CodedValueDomain.get_Name(0).ToString());
                                                ExcelSheet.Cells[Row, Col] = CodedValueDomain.get_Name(i);
                                                i = CodedValueDomain.CodeCount;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Feat.Shape != null)
                    {
                        if (Feat.Shape.IsEmpty == false)
                        {
                            pGeo = Globals.GetGeomCenter(Feat.Shape);
                            if (pGeo != null)
                            {

                                if (pGeo.Count > 0)
                                {
                                    if (pGeo[0].X != null)
                                        ExcelSheet.Cells[Row, Col + 1] = pGeo[0].X;
                                    if (pGeo[0].Y != null)
                                        ExcelSheet.Cells[Row, Col + 2] = pGeo[0].Y;
                                    if (pGeo[0] != null)
                                    {
                                        pGeo[0].Project(srWGS84);

                                        ExcelSheet.Cells[Row, Col + 3] = pGeo[0].Y;
                                        ExcelSheet.Cells[Row, Col + 4] = pGeo[0].X;
                                    }
                                    if (bZAware)
                                    {

                                        ExcelSheet.Cells[Row, Col + 5] = pGeo[0].Z;
                                    }
                                }
                            }
                        }
                    }

                    stepProgressor.Step();
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ExportAsset") + stepProgressor.Position + A4LGSharedFunctions.Localizer.GetString("Of") + MxDoc.FocusMap.SelectionCount.ToString() + ".";
                    if (!trackCancel.Continue())
                    {
                        return false;
                    }


                    Feat = FCursor.NextFeature();
                }

                //Hide Columns
                Col = 0;
                for (j = 0; j < TableFields.FieldCount; j++)
                {
                    CurField = TableFields.get_Field(j);

                    //skip blob and geometry fields in data also
                    if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                    {
                        Col += 1;
                        //Autofit
                        ExcelRange = ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[Row, Col]);
                        ExcelRange.EntireColumn.AutoFit();

                        //Hide column if invisible in ArcMap
                        if (TableFields.get_FieldInfo(j).Visible == false)
                        {
                            ExcelRange = ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[Row, Col]);
                            ExcelRange.EntireColumn.Hidden = true;
                        }
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ExportXLError"));
                return true;
            }
            finally
            {
                FC = null;
                Subtypes = null;

                TableProperties = null;
                if (EnumTableProperties != null)
                    Marshal.ReleaseComObject(EnumTableProperties);

                EnumTableProperties = null;
                TableProperty = null;
                TableCharacteristics = null;
                FeatSel = null;
                DisplayTable = null;
                Table = null;
                LayerFields = null;
                TableFields = null;
                LayerTest = null;
                if (Cursor != null)
                    Marshal.ReleaseComObject(Cursor);
                Cursor = null;

                if (FCursor != null)
                    Marshal.ReleaseComObject(FCursor);
                FCursor = null;
                CurField = null;
                Domain = null;
                CodedValueDomain = null;
                Feat = null;
                pGeo = null;

                missing = null;
                sheet = null;
                ExcelSheet = null;
                ExcelRange = null;
                style = null;

            }
        }
        private bool ExportTable(Excel.Workbook ExcelWbk, IMxDocument MxDoc, IStandaloneTable StandTable, ref IProgressDialog2 progressDialog, ref IStepProgressor stepProgressor,ref ITrackCancel trackCancel)
        {
            ITableProperties TableProperties = null;
            IEnumTableProperties EnumTableProperties = null;
            ITableProperty3 TableProperty = null;
            ITableCharacteristics TableCharacteristics = null;
            ITableSelection TabSel = null;
            IDisplayTable DisplayTable = null;
            ITable Table = null;
            IRow TabRow = null;
            IObjectClass ObjectClass = null;
            ISubtypes Subtypes = null;
            ITableFields TableFields = null;

            ICursor Cursor = null;
            IField CurField = null;
            IDomain Domain = null;
            ICodedValueDomain CodedValueDomain = null;

            object missing = null;
            object sheet = null;
            Microsoft.Office.Interop.Excel.Style style = null;


            try
            {
                bool UseDescriptions;
                int subtype;
                string SheetName;



                int Col = 0;
                int Row = 0;
                int i;
                int j;



                missing = System.Reflection.Missing.Value;

                sheet = ExcelWbk.Sheets[ExcelWbk.Sheets.Count];
                Excel.Worksheet ExcelSheet;
                Excel.Range ExcelRange;

                //Add new Excel worksheet
                ExcelSheet = (Excel.Worksheet)ExcelWbk.Sheets.Add(missing, sheet, missing, missing);
                //style = ExcelWbk.Styles.Add("Style1");
                //style.NumberFormat = "@";
                //style.Font.Name = "Arial";
                //style.Font.Bold = True
                //style.Font.Size = 12;
                //style.Interior.Pattern = Microsoft.Office.Interop.Excel.XlPattern.xlPattern Solid
                //style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlighLe ft




                SheetName = StandTable.Name;
                if (SheetName.Length > 30)
                {
                    SheetName = SheetName.Substring(0, 30);
                }
                ExcelSheet.Name = SheetName;


                //Determine whether to use descriptions or codes for domains
                UseDescriptions = true;
                TableProperties = MxDoc.TableProperties;
                EnumTableProperties = (IEnumTableProperties)TableProperties;
                EnumTableProperties.Reset();
                TableProperty = (ITableProperty3)EnumTableProperties.Next();

                while (TableProperty != null)
                {
                    if (TableProperty.StandaloneTable != null)
                    {
                        if (TableProperty.StandaloneTable.Equals(StandTable))
                        {
                            TableCharacteristics = (ITableCharacteristics)TableProperty;
                            UseDescriptions = TableCharacteristics.ShowCodedValueDomainDescriptions;
                            break;
                        }
                    }
                    TableProperty = (ITableProperty3)EnumTableProperties.Next();
                }
                TabSel = (ITableSelection)StandTable;

                DisplayTable = (IDisplayTable)StandTable;
                Table = (ITable)DisplayTable.DisplayTable;

                //Get subtype info
                ObjectClass = Table as IObjectClass;
                Subtypes = ObjectClass as ISubtypes;

                //Get TableFields so later we can determine whether that field is visible            
                TableFields = (ITableFields)StandTable;


                //loop through each field and write column headings
                Row = 1;
                for (j = 0; j < TableFields.FieldCount; j++)
                {
                    CurField = TableFields.get_Field(j);

                    //skip blob and geometry fields
                    if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                    {
                        Col += 1;
                        //Write field alias name as Excel column header
                        ExcelSheet.Cells[Row, Col] = TableFields.get_FieldInfo(j).Alias;
                        if (CurField.Type == esriFieldType.esriFieldTypeString)
                            ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[65535, Col]).EntireColumn.NumberFormat = "@";

                    }
                }

                //Get all selected records for this table (use IDisplayTable to get any joined data)
                DisplayTable.DisplaySelectionSet.Search(null, true, out Cursor);

                //subtype = Subtypes.DefaultSubtypeCode;

                //For each selected record

                TabRow = Cursor.NextRow();
                //stepProgressor.Step();
                //     progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ExportAsset") + stepProgressor.Position + A4LGSharedFunctions.Localizer.GetString("Of") + MxDoc.FocusMap.SelectionCount.ToString() + ".";

                while (TabRow != null)
                {
                    Row += 1;

                    if (Subtypes != null && Subtypes.HasSubtype == true &&
                       (TabRow.get_Value(Subtypes.SubtypeFieldIndex) != null))
                    {
                        subtype = Convert.ToInt32(TabRow.get_Value(Subtypes.SubtypeFieldIndex));
                    }
                    else
                    {
                        subtype = -99999;
                    }


                    //For each column
                    Col = 0;
                    for (j = 0; j < TableFields.FieldCount; j++)
                    {
                        CurField = TableFields.get_Field(j);

                        //skip blob and geometry fields in data also
                        if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                        {
                            Col += 1;
                            ExcelSheet.Cells[Row, Col] = TabRow.get_Value(j);

                            if (UseDescriptions == true && subtype == -99999)
                            {
                                Domain = CurField.Domain;
                                if (Domain != null)
                                {
                                    if (Domain.Type == esriDomainType.esriDTCodedValue)
                                    {
                                        CodedValueDomain = (ICodedValueDomain)CurField.Domain;
                                        for (i = 0; i < CodedValueDomain.CodeCount; i++)
                                        {
                                            if ((CodedValueDomain.get_Value(i)).ToString() == (TabRow.get_Value(j)).ToString())
                                            {
                                                //System.Diagnostics.Debug.Print(CodedValueDomain.get_Name(0).ToString());
                                                ExcelSheet.Cells[Row, Col] = CodedValueDomain.get_Name(i);
                                                i = CodedValueDomain.CodeCount;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (UseDescriptions == true && subtype != -99999)
                            {
                                if (Subtypes.SubtypeFieldIndex == j)
                                {
                                    ExcelSheet.Cells[Row, Col] = Subtypes.get_SubtypeName(subtype);
                                }
                                else
                                {
                                    Domain = Subtypes.get_Domain(subtype, CurField.Name);
                                    if ((Domain != null) && (Domain.Type == esriDomainType.esriDTCodedValue))
                                    {
                                        CodedValueDomain = (ICodedValueDomain)Domain;
                                        for (i = 0; i < CodedValueDomain.CodeCount; i++)
                                        {
                                            if ((CodedValueDomain.get_Value(i)).ToString() == (TabRow.get_Value(j)).ToString())
                                            {
                                                //System.Diagnostics.Debug.Print(CodedValueDomain.get_Name(0).ToString());
                                                ExcelSheet.Cells[Row, Col] = CodedValueDomain.get_Name(i);
                                                i = CodedValueDomain.CodeCount;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    stepProgressor.Step();
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ExportAsset") + stepProgressor.Position + A4LGSharedFunctions.Localizer.GetString("Of") + MxDoc.FocusMap.SelectionCount.ToString() + ".";
                    if (!trackCancel.Continue())
                    {
                        return false;
                    }
                    TabRow = Cursor.NextRow();
                }

                //Hide Columns
                Col = 0;
                for (j = 0; j < TableFields.FieldCount; j++)
                {
                    CurField = TableFields.get_Field(j);

                    //skip blob and geometry fields in data also
                    if ((CurField.Type != esriFieldType.esriFieldTypeBlob) && (CurField.Type != esriFieldType.esriFieldTypeGeometry))
                    {
                        Col += 1;
                        //Autofit
                        ExcelRange = ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[Row, Col]);
                        ExcelRange.EntireColumn.AutoFit();

                        //Hide column if invisible in ArcMap
                        if (TableFields.get_FieldInfo(j).Visible == false)
                        {
                            ExcelRange = ExcelSheet.get_Range(ExcelSheet.Cells[1, Col], ExcelSheet.Cells[Row, Col]);
                            ExcelRange.EntireColumn.Hidden = true;
                        }
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ExportXLError"));
                return true;
            }
            finally
            {
                TableProperties = null;
                EnumTableProperties = null;
                TableProperty = null;
                TableCharacteristics = null;
                TabSel = null;
                DisplayTable = null;
                Table = null;
                TabRow = null;
                ObjectClass = null;
                Subtypes = null;
                TableFields = null;


                if (Cursor != null)
                    Marshal.ReleaseComObject(Cursor);
                Cursor = null;
                CurField = null;
                Domain = null;
                CodedValueDomain = null;

                missing = null;
                sheet = null;
                style = null;


            }
        }



        public void IdentifySelected()
        {
            try
            {


                if (((IMxDocument)_app.Document).FocusMap.SelectionCount > 0)

                    //Globals.IdentifySelectedDockable(_app);
                    Globals.IdentifySelected(((IMxDocument)_app.Document).FocusMap);
                else
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("DataToolsMess_1") + Environment.NewLine + A4LGSharedFunctions.Localizer.GetString("DataToolsMess_2"), messageBoxHeader);

            }
            catch (Exception ex)
            {
                MessageBox.Show("IdentifySelected\n" + ex.ToString(), messageBoxHeader);
            }
        }
    }
}

