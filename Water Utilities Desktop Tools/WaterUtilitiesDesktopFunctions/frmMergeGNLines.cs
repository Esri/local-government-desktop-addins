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
using System.Collections;
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
    public partial class frmMergeGNLines : Form
    {
        private string  ConcatDelim  = ",";

        private static IApplication _app;
        private static IEditor m_editor;
        private static Hashtable m_colFeatures;
        private static IFeatureLayer m_FeatLay;
        private static List<MergeSplitGeoNetFeatures> m_Config = null;

        public frmMergeGNLines(IApplication app, IEditor editor)
        {

            InitializeComponent();
            _app = app;
            m_editor = editor;
            m_Config = ConfigUtil.GetMergeSplitConfig();


        }
        public bool loadDialog()
        {

            IMxDocument mxdoc = null;
            ILayer layer = null;
            IFeatureLayer fLayer = null;
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            INetworkClass netFC = null;
            IEnumIDs pEnumIDs = null;
            try
            {
                lstMergeFeatures.Items.Clear();
                mxdoc = _app.Document as IMxDocument;
                layer = mxdoc.SelectedLayer as ILayer;
                if (layer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_6") + Environment.NewLine + A4LGSharedFunctions.Localizer.GetString("TOC_5"));
                    return false;
                }

                //Verify that it is a feature layer
                fLayer = layer as IFeatureLayer;
                if (fLayer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_1"));
                    return false;
                }

                //Get the Feature layer and feature class

                fc = fLayer.FeatureClass;
                fSel = fLayer as IFeatureSelection;

                //Verify that it is a line layer
                if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_2"));
                    return false;
                }
                //Verify that the layer is part of a geometric network if using the establish flow by AnicillaryRole
                //IGeometricNetwork gnet = default(IGeometricNetwork);
                //IUtilityNetwork unet = default(IUtilityNetwork);
                //INetElements netelems = default(INetElements);

                netFC = fc as INetworkClass;
                if (netFC == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_3"));
                    return false;
                }


                //IEnumFeature pEnumFeature;
                //IFeature pFeature;

                pEnumIDs = fSel.SelectionSet.IDs;


                //pEnumFeature = m_editor.EditSelection;
                //pEnumFeature.Reset();

                m_colFeatures = new Hashtable();
                int pFeatID = pEnumIDs.Next();
                while (pFeatID != -1)
                {
                    m_colFeatures.Add(pFeatID.ToString(), fc.GetFeature(pFeatID));
                    lstMergeFeatures.Items.Add(pFeatID.ToString());
                    pFeatID = pEnumIDs.Next();
                }
                //pFeature = pEnumFeature.Next();
                //while (pFeature != null)
                //{
                //    if (pFeature.Class.CLSID.Value.ToString() == fc.CLSID.Value.ToString())
                //    {
                //        m_colFeatures.Add(pFeature.OID.ToString(), pFeature);
                //        lstMergeFeatures.Items.Add(pFeature.OID.ToString());

                //    }

                //    pFeature = pEnumFeature.Next();

                //}
                if (m_colFeatures.Count == 0)
                    return false;
                if (m_colFeatures.Count == 1)
                    return false;

                m_FeatLay = fLayer;
                if (m_Config[0].MergeSplitElev.ToUpper() == "TRUE")
                    chkMergeElevationData.Checked = true;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorOn") + A4LGSharedFunctions.Localizer.GetString("MergeOprt_1") + "\r\n" + ex.Message);
                return false;
            }
            finally
            {

                mxdoc = null;
                layer = null;
                fLayer = null;
                fc = null;
                fSel = null;
                netFC = null;
                pEnumIDs = null;
            }

        }
        private void lstMergeFeatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            IFeature pFeature = null;
            IMxDocument mxdoc = null;
            try
            {
                if (lstMergeFeatures.Items.Count == 0 || lstMergeFeatures.SelectedItems.Count == 0)
                {
                    btnOK.Enabled = false;
                    return;
                }
                else
                    btnOK.Enabled = true;



                string strOID = "";


                //for (int i = 0; i < lstMergeFeatures.Items.Count; i++)
                //{
                //    if (lstMergeFeatures.SelectedItems[0] == lstMergeFeatures.Items[i])
                //    {
                //        strOID = lstMergeFeatures.Items[i].ToString();
                //        break;
                //    }

                //}
                strOID = lstMergeFeatures.SelectedItems[0].Text.ToString();

                if (strOID == "")
                    return;
                pFeature = (IFeature)m_colFeatures[strOID];

                mxdoc = (IMxDocument)_app.Document;
                Globals.FlashGeometry(pFeature.ShapeCopy, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorOn") + A4LGSharedFunctions.Localizer.GetString("MergeOprt_2") + "\r\n" + ex.Message);
            }
            finally
            {
                pFeature = null;
                mxdoc = null;
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            IFeature pAttributeFeature = default(IFeature);
            ISubtypes pSubtypes = default(ISubtypes);
            ArrayList colAttributes = null;
            IFeature pCurFeature = default(IFeature);

            IFeature pNewFeature = default(IFeature);


            IGeometry pCurGeom = default(IGeometry);
            IGeometry pTmpGeom = default(IGeometry);
            ITopologicalOperator5 pTopoOperator = default(ITopologicalOperator5);
            IRowSubtypes pOutRSType = default(IRowSubtypes);
            IFields pFlds = default(IFields);
            IField pFld = default(IField);
            IDomain pDomain = default(IDomain);
            IGeometryCollection pGeomColl = default(IGeometryCollection);

            IInvalidArea pRefresh = null;
            IComplexEdgeFeature pCEF = null;
            IMap pMap = null;

            try
            {

                long lGTotalVal = 0;


                int lSubTypeCode = 0;
                string strOID = null;
                int intOID = -1;

                int i, j;

                //Screen.MousePointer = vbHourglass

                //The Next button doesn't get enabled until at least 1 FC is selected, but just in case...
                if (lstMergeFeatures.Items.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("SlctOneFtr"));
                    return;
                }

                //For Next loop which iterates through the array populating colSelClasses
                //This is for when I implement selecting multiple feature classes from the listbox....
                for (i = 0; i <= lstMergeFeatures.Items.Count - 1; i++)
                {
                    if (lstMergeFeatures.Items[i].Selected)
                    {
                        strOID = lstMergeFeatures.Items[i].Text;
                        intOID = Convert.ToInt32(strOID);
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (strOID == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("SlctMainFtr"));
                    return;

                }

                //Get the field values of the selected feature for use later
                pAttributeFeature = (IFeature)m_colFeatures[strOID];



                colAttributes = new ArrayList();
                for (i = 0; i < pAttributeFeature.Fields.FieldCount; i++)
                {
                    colAttributes.Add(pAttributeFeature.get_Value(i).ToString());

                }

                pSubtypes = (ISubtypes)m_FeatLay.FeatureClass;
                if (pSubtypes.HasSubtype)
                {
                    lSubTypeCode = (int)pAttributeFeature.get_Value(pSubtypes.SubtypeFieldIndex);

                }
                else
                {
                    lSubTypeCode = 0;
                }

                //If the features being merged and the target layer are the same FC and if that FC has subtypes, get the subtype code of the selected feature or target layer
                //If not, get the default
                //pSubtypes = m_pFC;
                //if pSubtypes.HasSubtype Then
                //  lSubTypeCode = m_lSubtype
                //End If

                //start edit operation
                m_editor.StartOperation();

                //pEnumFeature = m_pEditor.EditSelection
                //pEnumFeature.Reset

                //create a new feature to be the merge feature
                //pNFC = m_pFC                             'QI
                //Create the new feature
                pNewFeature = m_FeatLay.FeatureClass.CreateFeature();


                //create the new geometry.
                //initialize the default values for the new feature
                pOutRSType = (IRowSubtypes)pNewFeature;                //Set the RowSubtypes to the NewFeature
                //if (lSubTypeCode <> 0 ) 
                pOutRSType.SubtypeCode = lSubTypeCode;     //If there's a subtype code, set it
                //End If
                pOutRSType.InitDefaultValues();                //Init the Default values for the feature


                pFlds = m_FeatLay.FeatureClass.Fields;
                //Loop until we've gone through all the selected features (pCurFeature)
                i = 0;
                IList<MergeSplitFlds> pFldsNames = new List<MergeSplitFlds>();
                if (m_Config != null)
                {
                    if (m_Config.Count > 0)
                    {
                        if (m_Config[0] != null)
                        {
                            if (m_Config[0].Fields != null)
                            {
                                foreach (A4LGSharedFunctions.Field FldNam in m_Config[0].Fields)
                                {
                                    int idx = Globals.GetFieldIndex(pFlds, FldNam.Name);
                                    if (idx > -1)
                                    {
                                        try
                                        {

                                            pFldsNames.Add(new MergeSplitFlds(FldNam.Name, idx, "", FldNam.MergeRule, FldNam.SplitRule));
                                        }
                                        catch { }
                                    }

                                }
                            }
                        }
                    }
                }
                foreach (string ky in m_colFeatures.Keys)
                {

                    //}

                    //for (i = 0; i < m_colFeatures.Count; i++)
                    //{

                    pCurFeature = (IFeature)m_colFeatures[ky];
                    if (pFldsNames != null)
                    {
                        foreach (MergeSplitFlds FldNam in pFldsNames)
                        {

                            string testVal = pCurFeature.get_Value(FldNam.FieldIndex).ToString();
                            //if (Globals.IsNumeric(testVal))
                            //{
                            //    FldNam.Value = FldNam.Value + Convert.ToDouble(testVal);
                            //}

                            switch (FldNam.MergeType.ToUpper())
                            {
                                case "MAX":
                                    if (FldNam.Value == "")
                                    {
                                        FldNam.Value = testVal;
                                    }
                                    else
                                    {
                                        if (Convert.ToDouble(FldNam.Value) < Convert.ToDouble(testVal))
                                        {
                                            FldNam.Value = testVal;

                                        }
                                    }

                                    break;
                                case "MIN":
                                    if (FldNam.Value == "")
                                    {
                                        FldNam.Value = testVal;
                                    }
                                    else
                                    {
                                        if (Convert.ToDouble(FldNam.Value) > Convert.ToDouble(testVal))
                                        {
                                            FldNam.Value = testVal;

                                        }
                                    }
                                    break;
                                case "SUM":
                                    if (FldNam.Value == "")
                                    {
                                        FldNam.Value = testVal;
                                    }
                                    else
                                    {
                                        FldNam.Value += Convert.ToDouble(FldNam.Value) + Convert.ToDouble(testVal);
                                    }

                                    break;
                                case "AVERAGE":
                                    if (FldNam.Value == "")
                                    {
                                        FldNam.Value = testVal;
                                    }
                                    else
                                    {
                                        FldNam.Value += Convert.ToDouble(FldNam.Value) + Convert.ToDouble(testVal);
                                    }
                                    FldNam.AvgCount++;
                                    break;

                                case "CONCAT":
                                    if (FldNam.Value.Contains(testVal.ToString() + ConcatDelim))
                                                                                        {
                                                                                        }
                                    else if (FldNam.Value.Contains(ConcatDelim + testVal.ToString()))
                                     {
                                     }
                                     else
                                     {

                                         if (FldNam.Value == "")
                                         {
                                             FldNam.Value =testVal.ToString();
                                         }
                                         else
                                         {
                                             FldNam.Value += ConcatDelim + testVal.ToString();
                                         }

                                     }

                                    break;
                                default:

                                    break;
                            }

                        }
                    }
                    //get the geometry of the current feature, if it's the first feature, set it to pTmpGeom
                    //Otherwise, pTmpGeom is already set so Union the Geom of this feature with pTmpGeom
                    //And set that equal to the new pTmpGeom......
                    pCurGeom = pCurFeature.ShapeCopy;
                    if (i == 0)
                    {
                        pTmpGeom = pCurGeom;
                        pTopoOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator5)pTmpGeom;
                        pTopoOperator.IsKnownSimple_2 = false; //051710
                        pTopoOperator.Simplify();// 051710

                    }
                    else
                    {


                        //Simplify the just obtained OtherPolyLine
                        pTopoOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator5)pCurGeom;//051710
                        pTopoOperator.IsKnownSimple_2 = false;
                        pTopoOperator.Simplify();

                        //Reset the pTopoOp to pNewPolyLine in prep for the union
                        pTopoOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator5)pTmpGeom;
                        pTmpGeom = pTopoOperator.Union(pCurGeom);

                        //Simplify the resulting New Line
                        pTopoOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator5)pTmpGeom;
                        pTopoOperator.IsKnownSimple_2 = false;
                        pTopoOperator.Simplify();
                        //pTopoOperator = (ITopologicalOperator5)pTmpGeom;
                        //pTopoOperator.Simplify();
                        //pOutputGeometry = pTopoOperator.Union(pCurGeom);
                        //pTopoOperator.Simplify();
                        //pTmpGeom = pOutputGeometry;

                        //pTopoOperator.SimplifyAsFeature();
                        //pOutputGeometry = pTopoOperator.UnionEx(pCurGeom,true);
                        //pTopoOperator.SimplifyAsFeature();
                        //pTmpGeom = pOutputGeometry;


                        //pTopoOperator.Simplify();
                        //pOutputGeometry = pTopoOperator.Union(pCurGeom);
                        //pTmpGeom = pOutputGeometry;

                    }







                    //now go through each field, if it has a domain associated with it, then evaluate the merge policy...
                    //If not domain, then grab the value from the selected feature

                    for (j = 0; j < pFlds.FieldCount; j++)
                    {
                        pFld = pFlds.get_Field(j);
                        pDomain = pSubtypes.get_Domain(lSubTypeCode, pFld.Name);
                        if (pDomain != null && pFld.DefaultValue != null)
                        {
                            if (pDomain.MergePolicy == esriMergePolicyType.esriMPTSumValues)
                            {
                                //if (lCount == 1)
                                //    pNewFeature.set_Value(j, pCurFeature.get_Value(j));
                                //else
                                if (pNewFeature.get_Value(j) != null && pCurFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()) && Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (double)pNewFeature.get_Value(j) + (double)pCurFeature.get_Value(j));
                                    }

                                    else if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (double)pNewFeature.get_Value(j));
                                    }
                                    else if (Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (double)pCurFeature.get_Value(j));
                                    }

                                }
                                else if (pNewFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (double)pNewFeature.get_Value(j));
                                    }

                                }
                                else if (pCurFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (double)pCurFeature.get_Value(j));
                                    }

                                }
                                else
                                { }


                            }
                            else if (pDomain.MergePolicy == esriMergePolicyType.esriMPTAreaWeighted)
                            {

                                if (pNewFeature.get_Value(j) != null && pCurFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()) && Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (Double)pNewFeature.get_Value(j) + ((Double)pCurFeature.get_Value(j) * (Globals.GetGeometryLength(pCurFeature) / lGTotalVal)));
                                    }

                                    else if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (Double)pNewFeature.get_Value(j) * (Globals.GetGeometryLength(pCurFeature) / lGTotalVal));
                                    }
                                    else if (Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (Double)pCurFeature.get_Value(j) * (Globals.GetGeometryLength(pCurFeature) / lGTotalVal));
                                    }

                                }
                                else if (pNewFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pNewFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (Double)pNewFeature.get_Value(j) * (Globals.GetGeometryLength(pCurFeature) / lGTotalVal));
                                    }

                                }
                                else if (pCurFeature.get_Value(j) != null)
                                {
                                    if (Globals.IsNumeric(pCurFeature.get_Value(j).ToString()))
                                    {
                                        pNewFeature.set_Value(j, (Double)pCurFeature.get_Value(j) * (Globals.GetGeometryLength(pCurFeature) / lGTotalVal));
                                    }

                                }
                                else
                                { }


                            }
                            else if (pCurFeature.OID == pAttributeFeature.OID)
                            {
                                try
                                {
                                    pNewFeature.set_Value(j, pCurFeature.get_Value(j));
                                }
                                catch
                                { }
                            }

                        }
                        else
                            if (pCurFeature.OID == intOID)
                            {
                                //Set the field values from the selected feature; ignore Subtype, non-editable and Shape field

                                if (pFld.Editable == true && pSubtypes.SubtypeFieldIndex != j && m_FeatLay.FeatureClass.ShapeFieldName.ToUpper() != pFld.Name.ToUpper())
                                {
                                    //if (colAttributes[j] != null)
                                    //{
                                    try
                                    {
                                        pNewFeature.set_Value(j, colAttributes[j]);
                                    }
                                    catch { }

                                    // }

                                }
                            }

                    }
                    pCurFeature.Delete(); //delete the feature
                    i++;

                }
                //Check if the merged geometry is multi-part. If so, raise an error and abort
                //Multipart geometries are not supported in the geometric network.
                pGeomColl = (IGeometryCollection)pTmpGeom;

                if (pGeomColl.GeometryCount > 1)
                {
                    m_editor.AbortOperation();
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("SlctOneFtr") , A4LGSharedFunctions.Localizer.GetString("ErrorOn") + A4LGSharedFunctions.Localizer.GetString("MergeOprt_4"));
                    this.Close();
                    return;
                }
                pNewFeature.Shape = pTmpGeom;
                pNewFeature.Store();



                if (m_FeatLay.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge)
                {
                    pCEF = (IComplexEdgeFeature)pNewFeature;
                    pCEF.ConnectAtIntermediateVertices();
                }

                if (chkMergeElevationData.Checked)
                {
                    if (pFldsNames != null)
                    {
                        foreach (MergeSplitFlds FldNam in pFldsNames)
                        {

                            if (FldNam.MergeType.ToUpper() == "Average".ToUpper())
                            {
                                FldNam.Value = (Convert.ToDouble(FldNam.Value) / FldNam.AvgCount).ToString();
                            }
                            try
                            {
                                pNewFeature.set_Value(FldNam.FieldIndex, FldNam.Value);
                            }
                            catch
                            {
                            }
                        }
                    }

                }
                //finish edit operation
                m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("MergeOprt_4"));

                //refresh features

                pRefresh = new InvalidArea();
                pRefresh.Display = m_editor.Display;
                pRefresh.Add(pNewFeature);
                pRefresh.Invalidate(-2);

                //    'select new feature

                pMap = m_editor.Map;
                pMap.ClearSelection();
                pMap.SelectFeature((ILayer)m_FeatLay, pNewFeature);

                this.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("Key cannot be null"))
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorOn")  + A4LGSharedFunctions.Localizer.GetString("MergeOprt_5") + "\r\n" + A4LGSharedFunctions.Localizer.GetString("MergeOprt_6"));
                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorOn")  + A4LGSharedFunctions.Localizer.GetString("MergeOprt_5") + "\r\n" + ex.Message);
                }

                try
                {
                    //finish edit operation
                    m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("MergeOprt_4"));

                }
                catch
                { }
                this.Close();
            }
            finally
            {
                pAttributeFeature = null;
                pSubtypes = null;
                colAttributes = null;
                pCurFeature = null;

                pNewFeature = null;


                pCurGeom = null;
                pTmpGeom = null;
                pTopoOperator = null;
                pOutRSType = null;
                pFlds = null;
                pFld = null;
                pDomain = null;
                pGeomColl = null;

                pRefresh = null;
                pCEF = null;
                pMap = null;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorOn")  + A4LGSharedFunctions.Localizer.GetString("MergeOprt_5") + "\r\n" + ex.Message);
            }

        }

        private void frmMergeGNLines_Load(object sender, EventArgs e)
        {

        }




    }
}
