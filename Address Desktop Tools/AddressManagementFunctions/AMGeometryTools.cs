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
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using A4LGSharedFunctions;
using System.Runtime.InteropServices;



namespace A4LGAddressManagement
{
    public static class AMGeometryTools
    {
        public class intersectionRecords : IEquatable<intersectionRecords>
        {

            public intersectionRecords(double x, double y)
            {
                this.x = x;
                this.y = y;
                
            }
            public double x { get; set; }
            public double y { get; set; }


            public bool Equals(intersectionRecords other)
            {
                if (this.x == other.x && this.y == other.y)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        private static IFields createIntersectionFields(ISpatialReference pSpatRef)
        {

            IFields pFields;


            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription;
            ESRI.ArcGIS.Geodatabase.IFieldsEdit pFieldsEdit;
            ESRI.ArcGIS.Geodatabase.IField pField;
            ESRI.ArcGIS.Geodatabase.IFieldEdit pFieldEdit;
            IGeometryDefEdit geomDefEdit;
            try
            {
                objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();

                pFields = objectClassDescription.RequiredFields;
                pFieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)pFields; // Explicit Cast


                pField = pFields.get_Field(pFields.FindField("Shape"));
                pFieldEdit = (IFieldEdit)pField;
                geomDefEdit = (IGeometryDefEdit)pField.GeometryDef;
                geomDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                geomDefEdit.SpatialReference_2 = pSpatRef;

                pField = new FieldClass();
                pFieldEdit = (IFieldEdit)pField;
                pFieldEdit.Name_2 = "PRIMSTREET";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                pFieldEdit.Length_2 = 120;
                pFieldEdit.AliasName_2 = "Primary Street";
                pFieldEdit.IsNullable_2 = true;
                pFieldsEdit.AddField(pField);


                pField = new FieldClass();
                pFieldEdit = (IFieldEdit)pField;
                pFieldEdit.Name_2 = "SECSTREET";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                pFieldEdit.Length_2 = 120;
                pFieldEdit.AliasName_2 = "Secondary Street";
                pFieldEdit.IsNullable_2 = true;
                pFieldsEdit.AddField(pField);






                //IGeometryDef pGeoDef;
                //IGeometryDefEdit pGeoDefEdit;
                //pGeoDef = new GeometryDefClass();

                //pGeoDefEdit = pGeoDef as IGeometryDefEdit;



                ////pGeoDefEdit.AvgNumPoints_2 = 5;
                //pGeoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                //pGeoDefEdit.GridCount_2 = 1;
                //pGeoDefEdit.GridSize_2[0] = 200;
                //pGeoDefEdit.HasM_2 = false;
                //pGeoDefEdit.HasZ_2 = false;


                //pField = new FieldClass();
                //pFieldEdit = (IFieldEdit)pField;

                //pFieldEdit.Name_2 = "SHAPE";
                //pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                //pFieldEdit.GeometryDef_2 = pGeoDef;
                //pFieldEdit.AliasName_2 = "Shape";
                //pFieldEdit.IsNullable_2 = true;
                //pFieldEdit.Required_2 = true;

                //pFieldsEdit.set_Field(1, pField);


                return pFields;
            }
            catch
            {
                return null;
            }
        }

        public static void CreateIntersectionPoints(IApplication app, List<AddressCenterlineDetails> pAddressDet, bool FlipRanges)
        {


            IProgressDialog2 progressDialog = default(IProgressDialog2);
            IProgressDialogFactory progressDialogFactory = null;
            ITrackCancel trackCancel = null;
            Int32 int32_hWnd;
            IStepProgressor stepProgressor = null;





            //IEditor editor = null;
            IMap map = null;
            IList<intersectionRecords> strIntCoords;



            IMxDocument mxdoc = null;
            List<ILayer> layers = null;
            //Get the Feature layer and feature class
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            IFeatureLayer fLayer = null;
            IEnumIDs enumIds = null;

            IFeature featSelectedLine = null;
            IFeature featIntersectLine = null;
            IFeature newPntFeat = null;
            ISpatialFilter pSpatFilt = null;
            IFeatureCursor pFeatCurs = null;
            IPoint pPnt = null;
            IWorkspace pWS = null;
            IFeatureClass pIntPoint = null;
            IFeatureLayer pFT = null;
            int StNameidx;
            bool boolean_Continue = true;
            try
            {

                //editor = Globals.getEditor(app);
                //if (editor == null)
                //{
                //    return;
                //}

                //if (editor.EditState != esriEditState.esriStateEditing)
                //{
                //    MessageBox.Show("Must be editing.", "CreateIntersectionPoints");
                //    return;
                //}

                //// Verify that there are layers in the table on contents
                //map = editor.Map;

                map = (app.Document as IMxDocument).FocusMap;
                if (map.LayerCount < 1)
                {
                    MessageBox.Show("Must have at least one layer in your map.");
                    return;
                }

                if (pAddressDet.Count == 0)
                {
                    MessageBox.Show("The config file or config information is missing");
                    return;
                }

                //Get highlighted layer in the TOC
                mxdoc = app.Document as IMxDocument;
                //bool foundAsFC = false;
                layers = Globals.FindLayersByClassName(app, pAddressDet[0].FeatureClassName);
                // Maintain OId list
                if (layers == null)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layers were not found.");
                    return;
                }
                if (layers.Count == 0)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layers were not found.");
                    return;
                }
                // Create an edit operation enabling undo/redo
                //if (editor.EditState != esriEditState.esriStateEditing)
                //{
                //    MessageBox.Show("Must be editing.", "Address CreateIntersectionPoints");
                //    return;
                //}

                //try
                //{
                //    editor.StartOperation();
                //}
                //catch
                //{
                //    editor.AbortOperation();
                //    editor.StartOperation();
                //}

                //ProgressBar
                progressDialogFactory = new ProgressDialogFactoryClass();

                // Create a CancelTracker
                trackCancel = new CancelTrackerClass();

                // Set the properties of the Step Progressor
                int32_hWnd = app.hWnd;
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                stepProgressor.MinRange = 0;

                stepProgressor.StepValue = 1;
                stepProgressor.Message = "";
                stepProgressor.Hide();

                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = false;
                progressDialog.Description = "Creating Intersection Points";
                progressDialog.Title = "Creating Intersection Points";
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral;



                List<int> oidProcessed = new List<int>();
                strIntCoords = new List<intersectionRecords>();
                foreach (ILayer layer in layers)
                {
                    //Verify that it is a feature layer
                    fLayer = layer as IFeatureLayer;
                    if (fLayer == null)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layer was not found.");
                        return;
                    }
                    if (fLayer.FeatureClass == null)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " data path is not set.");
                        return;
                    }
                    //Get the Feature layer and feature class
                    fc = fLayer.FeatureClass;
                    fSel = fLayer as IFeatureSelection;

                    //Verify that it is a line layer
                    if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " is not a Line Layer.");
                        continue;
                    }


                    //Verify it has some features selected
                    if (fSel.SelectionSet.Count < 1)
                    {
                        //  MessageBox.Show("No features selected in the " + pAddressDet[0].FeatureClassName + " Layer.");
                        continue;
                    }
                    stepProgressor.MaxRange = fSel.SelectionSet.Count;
                    boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        return;
                    }
                    stepProgressor.Step();
                    stepProgressor.Message = "Finding Street Index Field";
                    StNameidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].FullName);
                    if (StNameidx == -1)
                    {
                        StNameidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].FullName);
                    }

                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }

                    if (pWS == null)
                    {
                        return;

                    }
                    pIntPoint = Globals.createFeatureClassInMemory("Intersections", createIntersectionFields((fLayer as IGeoDataset).SpatialReference), pWS, esriFeatureType.esriFTSimple);
                    pFT = new FeatureLayerClass();
                    pFT.FeatureClass = pIntPoint;
                    pFT.Name = "Intersection Points";

                    map.AddLayer(pFT);

                    try
                    {
                        enumIds = fSel.SelectionSet.IDs;
                        enumIds.Reset();

                        for (int id = enumIds.Next(); id > -1; id = enumIds.Next())
                        {

                            stepProgressor.Message = "Creating Intersection Point for: " + id;


                            if (!oidProcessed.Contains(id))
                            {
                                featSelectedLine = fc.GetFeature(id);
                                pSpatFilt = Globals.createSpatialFilter(fLayer, featSelectedLine.Shape, false, map.SpatialReference);

                                pFeatCurs = fLayer.FeatureClass.Search(pSpatFilt, true);
                                while ((featIntersectLine = pFeatCurs.NextFeature()) != null)
                                {
                                    if (featIntersectLine.OID != featSelectedLine.OID)
                                    {
                                        if (featIntersectLine.get_Value(StNameidx).ToString().Trim() != featSelectedLine.get_Value(StNameidx).ToString().Trim())
                                        {

                                            pPnt = Globals.GetIntersection(featSelectedLine.Shape, featIntersectLine.Shape as IPolyline) as IPoint;
                                            if (pPnt != null)
                                            {
                                                if (!pPnt.IsEmpty)
                                                {
                                                    if (!strIntCoords.Contains(new intersectionRecords(pPnt.X ,pPnt.Y )))
                                                    {
                                                        newPntFeat = pIntPoint.CreateFeature();
                                                        newPntFeat.Shape = pPnt;
                                                        newPntFeat.set_Value(pIntPoint.Fields.FindField("PRIMSTREET"), featSelectedLine.get_Value(StNameidx));
                                                        newPntFeat.set_Value(pIntPoint.Fields.FindField("SECSTREET"), featIntersectLine.get_Value(StNameidx));
                                                        newPntFeat.Store();
                                                        strIntCoords.Add(new intersectionRecords(pPnt.X, pPnt.Y));

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //oidProcessed.Add(feat.OID);

                                //feat.Store();
                            }
                            stepProgressor.Step();
                        }
                    }
                    catch (Exception ex)
                    {
                        // editor.AbortOperation();
                        MessageBox.Show("CreateIntersectionPoints\n" + ex.Message, ex.Source);
                        break;
                    }
                    finally
                    {

                        enumIds = null;

                        enumIds = null;

                        featSelectedLine = null;
                        featIntersectLine = null;
                        newPntFeat = null;
                        pSpatFilt = null;
                        if (pFeatCurs != null)
                        {
                            Marshal.ReleaseComObject(pFeatCurs);

                        }
                        pFeatCurs = null;
                        pPnt = null;


                    }
                    // Stop the edit operation 
                }
                //editor.StopOperation("Address CreateIntersectionPoints");
                // MessageBox.Show("Create Intersection Points");

                mxdoc.ActiveView.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateIntersectionPoints\n" + ex.Message, "CreateIntersectionPoints");
                return;
            }
            finally
            {

                if (progressDialog != null)
                    progressDialog.HideDialog();
                progressDialog = null;
                progressDialogFactory = null;
                trackCancel = null;

                stepProgressor = null;
                stepProgressor = null;

                map = null;
                mxdoc = null;
                layers = null;

                fc = null;
                fSel = null;
                fLayer = null;
                if (enumIds != null)
                {
                    Marshal.ReleaseComObject(enumIds);

                }
                enumIds = null;

                featSelectedLine = null;
                featIntersectLine = null;
                newPntFeat = null;
                pSpatFilt = null;
                if (pFeatCurs != null)
                {
                    Marshal.ReleaseComObject(pFeatCurs);

                }
                pFeatCurs = null;
                pPnt = null;
                pWS = null;
                pIntPoint = null;
                pFT = null;


            }

        }


        public static void FlipLines(IApplication app, List<AddressCenterlineDetails> pAddressDet, bool FlipRanges)
        {
            IEditor editor = null;
            IMap map = null;


            IMxDocument mxdoc = null;
            List<ILayer> layers = null;
            //Get the Feature layer and feature class
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            IFeatureLayer fLayer = null;

            bool bRoadFlipped = false;
            int LFidx, LTidx, RFidx, RTidx;

            try
            {

                editor = Globals.getEditor(app);
                if (editor == null)
                {
                    return;
                }

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show("Must be editing.", "Address Flip Lines");
                    return;
                }

                // Verify that there are layers in the table on contents
                map = editor.Map;
                if (map.LayerCount < 1)
                {
                    MessageBox.Show("Must have at least one layer in your map.");
                    return;
                }

                if (pAddressDet.Count == 0)
                {
                    MessageBox.Show("The config file or config information is missing");
                    return;
                }

                //Get highlighted layer in the TOC
                mxdoc = app.Document as IMxDocument;
                // bool foundAsFC = false;
                layers = Globals.FindLayersByClassName(app, pAddressDet[0].FeatureClassName);
                // Maintain OId list
                if (layers == null)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layers were not found.");
                    return;
                }
                if (layers.Count == 0)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layers were not found.");
                    return;
                }
                // Create an edit operation enabling undo/redo
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show("Must be editing.", "Address FlipLines");
                    return;
                }

                try
                {
                    editor.StartOperation();
                }
                catch
                {
                    editor.AbortOperation();
                    editor.StartOperation();
                }
                List<int> oidProcessed = new List<int>();

                foreach (ILayer layer in layers)
                {
                    //Verify that it is a feature layer
                    fLayer = layer as IFeatureLayer;
                    if (fLayer == null)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " layer was not found.");
                        return;
                    }
                    if (fLayer.FeatureClass == null)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " data path is not set.");
                        return;
                    }
                    //Get the Feature layer and feature class
                    fc = fLayer.FeatureClass;
                    fSel = fLayer as IFeatureSelection;

                    //Verify that it is a line layer
                    if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " is not a Line Layer.");
                        continue;
                    }


                    //Verify it has some features selected
                    if (fSel.SelectionSet.Count < 1)
                    {
                        //MessageBox.Show("No features selected in the " + pAddressDet[0].FeatureClassName + " Layer.");
                        continue;
                    }

                    //LFidx, LTidx, RFidx, RTidx;
                    LFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftFrom);
                    if (LFidx == -1)
                    {
                        LFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftFrom);
                    }

                    LTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftTo);
                    if (LTidx == -1)
                    {
                        LTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftTo);
                    }

                    RFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightFrom);
                    if (RFidx == -1)
                    {
                        RFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightFrom);
                    }

                    RTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightTo);
                    if (RTidx == -1)
                    {
                        RTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightTo);
                    }

                    if (LFidx == -1 || LTidx == -1 || RFidx == -1 || RTidx == -1)
                    {
                        MessageBox.Show("The Address range fields were not found", "Address FlipLines");
                        continue;
                    }


                    IEnumIDs enumIds = null;

                    IFeature feat = null;
                    ICurve curve = null;


                    try
                    {
                        enumIds = fSel.SelectionSet.IDs;
                        enumIds.Reset();

                        for (int id = enumIds.Next(); id > -1; id = enumIds.Next())
                        {
                            if (!oidProcessed.Contains(id))
                            {
                                feat = fc.GetFeature(id);
                                curve = feat.ShapeCopy as ICurve;
                                curve.ReverseOrientation();
                                feat.Shape = curve;
                                if (FlipRanges)
                                {
                                    string TempVal = feat.get_Value(LFidx).ToString();
                                    feat.set_Value(LFidx, feat.get_Value(RFidx));
                                    feat.set_Value(RFidx, TempVal);

                                    TempVal = feat.get_Value(LTidx).ToString();
                                    feat.set_Value(LTidx, feat.get_Value(RTidx));
                                    feat.set_Value(RTidx, TempVal);
                                }
                                oidProcessed.Add(feat.OID);
                                bRoadFlipped = true;
                                feat.Store();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show("FlipLines\n" + ex.Message, ex.Source);
                        break;
                    }
                    finally
                    {

                        enumIds = null;

                        feat = null;
                        curve = null;

                    }
                    // Stop the edit operation 
                }
                
                
                if (bRoadFlipped )
                {
                    editor.StopOperation("Address Flip Lines");
                    MessageBox.Show("Address lines flipped");

                    mxdoc.ActiveView.Refresh();
                    


                }else
                {
                    MessageBox.Show("No Features Selected, please select some features and try again");
                    editor.AbortOperation();
                
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FlipLines\n" + ex.Message, "FlipLines");
                return;
            }
            finally
            {
                editor = null;
                map = null;


                mxdoc = null;
                layers = null;

                fc = null;
                fSel = null;
                fLayer = null;


            }

        }
        public static AddressReturnInfo AddPointWithRef(IApplication app, IPoint pPnt, List<CreatePointWithReferenceDetails> createPointDetails,
          ILayer targetLayer, ref int idxConfig)
        {
            //ISpatialFilter sFilter= null;
            //IFeatureCursor pFeatCur = null;

           // IFeatureLayer lineLayer = null;
            IFeatureLayer pointLayer = null;

            IFeature pPointFeat = null;
            //IFeature pLineFeat = null;

            IEditor pEditor = null;
           // IPoint pSnapedPoint = null;
           // double dAlong = -1;
            bool bStoreEdit = false;
            try
            {
                pEditor = Globals.getEditor(app);
                idxConfig = -1;
                string ClassName = Globals.getClassName(targetLayer);
                for (int i = 0; i < createPointDetails.Count; i++)
                {
                    CreatePointWithReferenceDetails createPointDet = createPointDetails[i];
                    if (createPointDet.LayerName == ClassName || createPointDet.LayerName == targetLayer.Name)
                    {
                        idxConfig = i;
                        bool pointFndAsFL = true;
                        pointLayer = Globals.FindLayer(app, createPointDet.ReferencePointLayerName, ref pointFndAsFL) as IFeatureLayer;
                        if (pointLayer == null)
                            continue;

                        if (Globals.IsEditable(ref pointLayer, ref pEditor) == false)
                        {
                            continue;
                        }

                        pPointFeat = Globals.GetClosestFeature(pPnt, pointLayer, 0, true, false);
                        if (pPointFeat == null)
                        {

                            if (createPointDet.ReferencePointEditTemplate.Trim() != "")
                            {
                                IEditTemplate pEditTemp;
                                //pEditTemp = Globals.PromptAndGetEditTemplate(app, pointLayer, createPointDet.ReferencePointEditTemplate.Trim());
                                pEditTemp = Globals.PromptAndGetEditTemplateGraphic(pointLayer, createPointDet.ReferencePointEditTemplate.Trim());
                                pPointFeat = Globals.CreateFeature(pPnt, pEditTemp, pEditor, app, false, false, true);
                                pEditTemp = null;
                            }
                            else
                            {
                                pPointFeat = Globals.CreateFeature(pPnt, pointLayer, pEditor, app, false, false, true);

                            }

                            bStoreEdit = true;


                        }

                        AddressInfo addInfo = Globals.GetAddressInfo(app, pPnt, createPointDet.AddressCenterlineDetails.FeatureClassName, createPointDet.AddressCenterlineDetails.FullName,
                            createPointDet.AddressCenterlineDetails.LeftTo, createPointDet.AddressCenterlineDetails.RightTo,
                            createPointDet.AddressCenterlineDetails.LeftFrom, createPointDet.AddressCenterlineDetails.RightFrom, createPointDet.AddressCenterlineDetails.IDField, false, 2);

                        if (addInfo == null)
                            continue;


                        if (bStoreEdit)
                        {
                            pPointFeat.Store();

                        }
                        ((IMxDocument)app.Document).FocusMap.SelectFeature(pointLayer, pPointFeat);

                        int intAddPntKeyFld = Globals.GetFieldIndex(pointLayer, createPointDet.ReferencePointIDField);
                        string addressPntKey = "";
                        if (intAddPntKeyFld != -1)
                        {
                            if (pPointFeat.get_Value(intAddPntKeyFld) != null)
                            {
                                addressPntKey = pPointFeat.get_Value(intAddPntKeyFld).ToString();
                            }
                        }
                        AddressReturnInfo addRet = new AddressReturnInfo(addInfo, addressPntKey);





                        //retAddNumLeft = Convert.ToInt32(Math.Round(retAddNumLeft, 0)); 
                        //retAddNumRight = Convert.ToInt32(Math.Round(retAddNumRight, 0));
                        //return new AddressReturnInfo(Convert.ToInt32(retAddNumLeft), Convert.ToInt32(retAddNumRight), roadName, pLineFeat.Shape, addressPntKey);
                        return addRet;


                    }
                }

                return null;
            }
            catch 
            {
                return null;
            }
            finally
            {
                //sFilter = null;
                //pFeatCur = null;

                //lineLayer = null;
                pointLayer = null;

                pPointFeat = null;
                //pLineFeat = null;

                pEditor = null;
                //pSnapedPoint = null;
            }

        }
        //public static void SplitAndProrate(IApplication app, IFeature pNewFeat, IFeatureLayer newLayer, List<AddressCenterlineDetails> pAddressDet)
        //{


        //    IEditor editor = null;
        //    IMap map = null;


        //    IMxDocument mxdoc = null;
        //    ILayer layer = null;
        //    //Get the Feature layer and feature class
        //    IFeatureClass fc = null;
        //    IFeatureSelection fSel = null;
        //    IFeatureLayer fLayer = null;
        //    IFeature pFeat = null;
        //    IPoint pIntPoint = null;
        //    IFeatureEdit2 featureEdit = null;
        //    ISet pSet = null;
        //    IPolyline pPolyLine = null;
        //    IFeature pSplitFeat = null;
        //    int RoadNameidx, LFidx, LTidx, RFidx, RTidx;
        //    //RI.ArcGIS.Framework.ICommandItem pCmd = null;

        //    List<IPoint> pIntPoints = null;
        //    List<IFeature> pNewFeats = null;
        //    IFeatureEdit2 pSourceFeat = null;

        //    try
        //    {

        //        editor = Globals.getEditor(app);
        //        if (editor == null)
        //        {
        //            return;
        //        }



        //        // Verify that there are layers in the table on contents
        //        map = editor.Map;

        //        if (pAddressDet.Count == 0)
        //        {
        //            MessageBox.Show("The config file or config information is missing");
        //            return;
        //        }

        //        //Get highlighted layer in the TOC
        //        mxdoc = app.Document as IMxDocument;
        //        bool foundAsFC = false;
        //        layer = Globals.FindLayer(app, pAddressDet[0].FeatureClassName, ref foundAsFC);

        //        if (layer == null)
        //        {
        //            MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " Featureclass was not found.");
        //            return;
        //        }

        //        //Verify that it is a feature layer
        //        fLayer = layer as IFeatureLayer;
        //        if (fLayer == null)
        //        {
        //            MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " Featureclass was not found.");
        //            return;
        //        }
        //        if (fLayer.FeatureClass == null)
        //        {
        //            MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " data path is not set.");
        //            return;
        //        }
        //        //Get the Feature layer and feature class
        //        fc = fLayer.FeatureClass;
        //        fSel = fLayer as IFeatureSelection;

        //        //Verify that it is a line layer
        //        if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
        //        {
        //            MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " is not a Line Layer.");
        //            return;
        //        }
        //        //LFidx, LTidx, RFidx, RTidx;
        //        RoadNameidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].FullName);
        //        if (RoadNameidx == -1)
        //        {
        //            RoadNameidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].FullName);
        //        }


        //        //LFidx, LTidx, RFidx, RTidx;
        //        LFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftFrom);
        //        if (LFidx == -1)
        //        {
        //            LFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftFrom);
        //        }

        //        LTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftTo);
        //        if (LTidx == -1)
        //        {
        //            LTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftTo);
        //        }

        //        RFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightFrom);
        //        if (RFidx == -1)
        //        {
        //            RFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightFrom);
        //        }

        //        RTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightTo);
        //        if (RTidx == -1)
        //        {
        //            RTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightTo);
        //        }

        //        if (LFidx == -1 || LTidx == -1 || RFidx == -1 || RTidx == -1)
        //        {
        //            MessageBox.Show("The Address range fields were not found", "Address FlipLines");
        //            return;
        //        }


        //        List<int> intOIDs = Globals.GetIntersectingFeaturesOIDs(pNewFeat.ShapeCopy, fLayer, false, pNewFeat.OID);
        //        if (intOIDs == null)
        //            return;

        //        if (intOIDs.Count == 0)
        //            return;

        //        //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
        //        //if (pCmd != null)
        //        //{
        //        //    pCmd.Execute();
        //        //}




        //        pIntPoints = new List<IPoint>();

        //        foreach (int i in intOIDs)
        //        {
        //            pFeat = fLayer.FeatureClass.GetFeature(i);

        //            pIntPoint = Globals.GetIntersection(pNewFeat.ShapeCopy, pFeat.ShapeCopy as IPolyline) as IPoint;




        //            if (pIntPoint != null)
        //            {
        //                pIntPoints.Add(pIntPoint);

        //                featureEdit = pFeat as IFeatureEdit2;
        //                try
        //                {
        //                    pPolyLine = pFeat.Shape as IPolyline;
        //                    if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
        //                    { }
        //                    else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
        //                    { }
        //                    else
        //                    {
        //                        AddressInfo addInfo = Globals.GetAddressInfo(app, pIntPoint, pFeat, RoadNameidx, LTidx, RTidx, LFidx, RFidx, false, 5);
        //                        string LFVal, LTVal, RTVal, RFVal;
        //                        LTVal = pFeat.get_Value(LTidx).ToString();
        //                        RTVal = pFeat.get_Value(RTidx).ToString();
        //                        LFVal = pFeat.get_Value(LFidx).ToString();
        //                        RFVal = pFeat.get_Value(RFidx).ToString();

        //                        try
        //                        {
        //                            pSet = featureEdit.SplitWithUpdate(pIntPoint);
        //                            if (pSet.Count > 0)
        //                            {

        //                                pPolyLine = pFeat.Shape as IPolyline;
        //                                if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
        //                                {

        //                                    pFeat.set_Value(LTidx, addInfo.LeftAddress);
        //                                    pFeat.set_Value(RTidx, addInfo.RightAddress);
        //                                    pFeat.Store();
        //                                }
        //                                else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
        //                                {
        //                                    pFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
        //                                    pFeat.set_Value(RFidx, addInfo.RightAddress + 2);
        //                                    pFeat.Store();
        //                                }
        //                                else
        //                                {

        //                                }

        //                                pSplitFeat = pSet.Next() as IFeature;

        //                                while (pSplitFeat != null)
        //                                {

        //                                    pPolyLine = pSplitFeat.Shape as IPolyline;
        //                                    if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
        //                                    {

        //                                        pSplitFeat.set_Value(LTidx, addInfo.LeftAddress);
        //                                        pSplitFeat.set_Value(RTidx, addInfo.RightAddress);
        //                                        pSplitFeat.Store();
        //                                    }
        //                                    else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
        //                                    {
        //                                        pSplitFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
        //                                        pSplitFeat.set_Value(RFidx, addInfo.RightAddress + 2);
        //                                        pSplitFeat.Store();
        //                                    }
        //                                    else
        //                                    {

        //                                    }
        //                                    pSplitFeat = pSet.Next() as IFeature;

        //                                }
        //                            }
        //                        }
        //                        catch
        //                        { }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show("SplitAndProrate\n" + ex.Message, "SplitAndProrate");
        //                }

        //            }
        //            pFeat = null;

        //        }

        //        //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
        //        //if (pCmd != null)
        //        //{
        //        //    pCmd.Execute();
        //        //}

        //        pNewFeats = new List<IFeature>();
        //        pNewFeats.Add(pNewFeat);


        //        //bool lineSplit = false;
        //        foreach (IPoint pTmpPnt in pIntPoints)
        //        {
        //            // lineSplit = false;
        //            foreach (IFeature pTmpFeat in pNewFeats)
        //            {
        //                bool Rside = false;
        //                if (Globals.GetPointOnLine(pTmpPnt, pTmpFeat.Shape as IPolyline, .1, out  Rside) != null)
        //                {
        //                    if ((pTmpFeat.Shape as IPolyline).ToPoint.X == pTmpPnt.X && (pTmpFeat.Shape as IPolyline).ToPoint.Y == pTmpPnt.Y)
        //                    { }
        //                    else if ((pTmpFeat.Shape as IPolyline).FromPoint.X == pTmpPnt.X && (pTmpFeat.Shape as IPolyline).FromPoint.Y == pTmpPnt.Y)
        //                    { }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            pSet = (pSourceFeat = (pTmpFeat as IFeatureEdit2)).SplitWithUpdate(pTmpPnt);
        //                            // lineSplit = true;
        //                            if (pSet.Count > 0)
        //                            {

        //                                pSplitFeat = pSet.Next() as IFeature;
        //                                while (pSplitFeat != null)
        //                                {

        //                                    (app.Document as IMxDocument).FocusMap.SelectFeature(newLayer, pSplitFeat);
        //                                    pNewFeats.Add(pSplitFeat);
        //                                    pSplitFeat = pSet.Next() as IFeature;
        //                                }
        //                            }
        //                            break;
        //                        }
        //                        catch { }

        //                    }
        //                }

        //            }
        //            //if (lineSplit == true);

        //        }
        //        //List<IFeature> pNewFeats = null ;
        //        //IFeatureEdit2 pSourceFeat = null;
        //        //ISet pSourceSet = null;
        //        //pNewFeats = new List<IFeature>();

        //        //pNewFeats.Add(pNewFeat);
        //        //try
        //        //{
        //        //    for (int k = 0; k < pNewFeats.Count; k++)
        //        //    {
        //        //        pSourceFeat = pNewFeats[k] as IFeatureEdit2;
        //        //        pSourceSet = pSourceFeat.SplitWithUpdate(pIntPoint);
        //        //        if (pSourceSet.Count > 0)
        //        //        {

        //        //        }
        //        //    }


        //        //}
        //        //catch
        //        //{
        //        //}

        //        mxdoc.ActiveView.Refresh();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("SplitAndProrate\n" + ex.Message, "SplitAndProrate");
        //        return;
        //    }
        //    finally
        //    {
        //        //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
        //        //if (pCmd != null)
        //        //{
        //        //    pCmd.Execute();
        //        //}
        //        //pCmd = null;
        //        editor = null;
        //        map = null;


        //        mxdoc = null;
        //        layer = null;

        //        fc = null;
        //        fSel = null;
        //        fLayer = null;

        //        pFeat = null;
        //        pIntPoint = null;
        //        featureEdit = null;
        //        pSet = null;
        //        pPolyLine = null;
        //        pSplitFeat = null;
        //        pIntPoints = null;
        //        pNewFeats = null;
        //        pSourceFeat = null;

        //    }

        //}
        public static void SplitAndProrate(IApplication app, IFeature pNewFeat, IFeatureLayer newLayer, List<AddressCenterlineDetails> pAddressDet)
        {


            IEditor editor = null;
            IMap map = null;


            IMxDocument mxdoc = null;
            ILayer layer = null;
            //Get the Feature layer and feature class
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            IFeatureLayer fLayer = null;
            IFeature pFeat = null;
            IPoint pIntPoint = null;
            IFeatureEdit2 featureEdit = null;
            ISet pSet = null;
            IPolyline pPolyLine = null;
            IFeature pSplitFeat = null;
            int RoadNameidx, LFidx, LTidx, RFidx, RTidx;
            //ESRI.ArcGIS.Framework.ICommandItem pCmd = null;

            List<IPoint> pIntPoints = null;
            List<IFeature> pNewFeats = null;
            IFeatureEdit2 pSourceFeat = null;
            string locationForDebug = "Start";

            try
            {
                locationForDebug = "Getting Editing";
                editor = Globals.getEditor(app);
                if (editor == null)
                {
                    return;
                }
                locationForDebug = "Editor Found";


                // Verify that there are layers in the table on contents
                map = editor.Map;
                locationForDebug = "Checking Address config Detailts";
                if (pAddressDet.Count == 0)
                {
                    MessageBox.Show("The config file or config information is missing");
                    return;
                }

                //Get highlighted layer in the TOC
                mxdoc = app.Document as IMxDocument;
                bool foundAsFC = false;
                locationForDebug = "Finding Layer";
                layer = Globals.FindLayer(app, pAddressDet[0].FeatureClassName, ref foundAsFC);

                if (layer == null)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " Featureclass was not found.");
                    return;
                }
                locationForDebug = layer.Name;

                //Verify that it is a feature layer

                fLayer = layer as IFeatureLayer;
                if (fLayer == null)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " Featureclass was not found.");
                    return;
                }
                locationForDebug = fLayer.Name;

                if (fLayer.FeatureClass == null)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " data path is not set.");
                    return;
                }
                //Get the Feature layer and feature class
                fc = fLayer.FeatureClass;
                locationForDebug = fc.AliasName;

                fSel = fLayer as IFeatureSelection;

                //Verify that it is a line layer
                if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                {
                    MessageBox.Show("The " + pAddressDet[0].FeatureClassName + " is not a Line Layer.");
                    return;
                }
                locationForDebug = fc.ShapeType.ToString();

                //LFidx, LTidx, RFidx, RTidx;
                RoadNameidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].FullName);
                if (RoadNameidx == -1)
                {
                    RoadNameidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].FullName);
                }
                locationForDebug = "RoadNameIndex" + RoadNameidx;


                //LFidx, LTidx, RFidx, RTidx;
                LFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftFrom);
                if (LFidx == -1)
                {
                    LFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftFrom);
                }
                locationForDebug = "RoadNameIndex" + LFidx;

                LTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].LeftTo);
                if (LTidx == -1)
                {
                    LTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].LeftTo);
                }
                locationForDebug = "RoadNameIndex" + LTidx;

                RFidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightFrom);
                if (RFidx == -1)
                {
                    RFidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightFrom);
                }
                locationForDebug = "RoadNameIndex" + RFidx;

                RTidx = fLayer.FeatureClass.Fields.FindField(pAddressDet[0].RightTo);
                if (RTidx == -1)
                {
                    RTidx = fLayer.FeatureClass.Fields.FindFieldByAliasName(pAddressDet[0].RightTo);
                }
                locationForDebug = "RoadNameIndex" + RTidx;

                if (LFidx == -1 || LTidx == -1 || RFidx == -1 || RTidx == -1)
                {
                    MessageBox.Show("The Address range fields were not found", "Address FlipLines");
                    return;
                }

                locationForDebug = "About to search for OIDS" + pNewFeat.OID;

                List<int> intOIDs = Globals.GetIntersectingFeaturesOIDs(pNewFeat.ShapeCopy, fLayer, false, pNewFeat.OID, map.SpatialReference);
                if (intOIDs == null)
                    return;
                if (intOIDs.Count == 0)
                    return;
                locationForDebug = intOIDs.Count + " found";

                //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
                //if (pCmd != null)
                //{
                //    pCmd.Execute();
                //}




                pIntPoints = new List<IPoint>();

                foreach (int i in intOIDs)
                {
                    locationForDebug = i + " OID";

                    pFeat = fLayer.FeatureClass.GetFeature(i);

                    pIntPoint = Globals.GetIntersection(pNewFeat.ShapeCopy, pFeat.ShapeCopy as IPolyline) as IPoint;
                    locationForDebug = "Point found";




                    if (pIntPoint != null)
                    {
                        locationForDebug = pIntPoint.X + " " + pIntPoint.Y;


                        pIntPoints.Add(pIntPoint);

                        featureEdit = pFeat as IFeatureEdit2;
                        try
                        {
                            pPolyLine = pFeat.Shape as IPolyline;
                            if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
                            { }
                            else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
                            { }
                            else
                            {
                                AddressInfo addInfo = Globals.GetAddressInfo(app, pIntPoint, pFeat, RoadNameidx, LTidx, RTidx, LFidx, RFidx, -1, false, 5);
                                if (addInfo == null)
                                {
                                    locationForDebug = "Null AddInfo";
                                }
                                else
                                {
                                    locationForDebug = addInfo.StreetName;

                                    string LFVal, LTVal, RTVal, RFVal;
                                    LTVal = pFeat.get_Value(LTidx).ToString();
                                    RTVal = pFeat.get_Value(RTidx).ToString();
                                    LFVal = pFeat.get_Value(LFidx).ToString();
                                    RFVal = pFeat.get_Value(RFidx).ToString();

                                    try
                                    {
                                        locationForDebug = "About to split";

                                        pSet = featureEdit.SplitWithUpdate(pIntPoint);
                                        locationForDebug = "Result split " + pSet.Count;

                                        if (pSet.Count > 0)
                                        {
                                            locationForDebug = "About to set value1";

                                            pPolyLine = pFeat.Shape as IPolyline;
                                            if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
                                            {

                                                pFeat.set_Value(LTidx, addInfo.LeftAddress);
                                                pFeat.set_Value(RTidx, addInfo.RightAddress);
                                                pFeat.Store();
                                                locationForDebug = "value Set";

                                            }
                                            else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
                                            {
                                                pFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
                                                pFeat.set_Value(RFidx, addInfo.RightAddress + 2);
                                                pFeat.Store();
                                                locationForDebug = "value Set";
                                            }
                                            else
                                            {
                                                double retDis  = Globals.GetDistanceBetweenPoints(pPolyLine.FromPoint, pIntPoint);
                                                if (retDis  !=-99999.9)
                                                {
                                                    if (Globals.ConvertSpatRefToFeet(retDis, pPolyLine.SpatialReference, app) < .01)
                                                    {
                                                        pFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
                                                        pFeat.set_Value(RFidx, addInfo.RightAddress + 2);
                                                        pFeat.Store();
                                                        locationForDebug = "value Set";
                                                    }
                                                    else
                                                    {
                                                        retDis = Globals.GetDistanceBetweenPoints(pPolyLine.ToPoint, pIntPoint);
                                                        if (retDis != -99999.9)
                                                        {
                                                            if (Globals.ConvertSpatRefToFeet(retDis, pPolyLine.SpatialReference, app) < .01)
                                                            {
                                                                pFeat.set_Value(LTidx, addInfo.LeftAddress);
                                                                pFeat.set_Value(RTidx, addInfo.RightAddress);
                                                                pFeat.Store();
                                                                locationForDebug = "value Set";

                                                            }
                                                            else
                                                            {

                                                            }

                                                        }
                                                        else
                                                        { }
                                                    }

                                                }
                                                else
                                                {}

                                            }

                                            pSplitFeat = pSet.Next() as IFeature;

                                            while (pSplitFeat != null)
                                            {
                                                locationForDebug = "About to set value2";

                                                pPolyLine = pSplitFeat.Shape as IPolyline;
                                                if (pPolyLine.ToPoint.X == pIntPoint.X && pPolyLine.ToPoint.Y == pIntPoint.Y)
                                                {

                                                    pSplitFeat.set_Value(LTidx, addInfo.LeftAddress);
                                                    pSplitFeat.set_Value(RTidx, addInfo.RightAddress);
                                                    pSplitFeat.Store();
                                                    locationForDebug = "value Set";
                                                }
                                                else if (pPolyLine.FromPoint.X == pIntPoint.X && pPolyLine.FromPoint.Y == pIntPoint.Y)
                                                {
                                                    pSplitFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
                                                    pSplitFeat.set_Value(RFidx, addInfo.RightAddress + 2);
                                                    pSplitFeat.Store();
                                                    locationForDebug = "value Set";
                                                }
                                                else
                                                {
                                                    double retDis = Globals.GetDistanceBetweenPoints(pPolyLine.FromPoint, pIntPoint);
                                                    if (retDis != -99999.9)
                                                    {
                                                        if (Globals.ConvertSpatRefToFeet(retDis, pPolyLine.SpatialReference, app) < .01)
                                                        {
                                                            pSplitFeat.set_Value(LFidx, addInfo.LeftAddress + 2);
                                                            pSplitFeat.set_Value(RFidx, addInfo.RightAddress + 2);
                                                            pSplitFeat.Store();
                                                            locationForDebug = "value Set";
                                                        }
                                                        else
                                                        {
                                                            retDis = Globals.GetDistanceBetweenPoints(pPolyLine.ToPoint, pIntPoint);
                                                            if (retDis != -99999.9)
                                                            {
                                                                if (Globals.ConvertSpatRefToFeet(retDis, pPolyLine.SpatialReference, app) < .01)
                                                                {
                                                                    pSplitFeat.set_Value(LTidx, addInfo.LeftAddress);
                                                                    pSplitFeat.set_Value(RTidx, addInfo.RightAddress);
                                                                    pSplitFeat.Store();
                                                                    locationForDebug = "value Set";

                                                                }
                                                                else
                                                                {

                                                                }

                                                            }
                                                            else
                                                            { }
                                                        }

                                                    }
                                                    else
                                                    { }

                                                }
                                                pSplitFeat = pSet.Next() as IFeature;

                                            }
                                        }
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("SplitAndProrate\n" + ex.Message, "SplitAndProrate");
                        }

                    }
                    pFeat = null;

                }

                //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                //if (pCmd != null)
                //{
                //    pCmd.Execute();
                //}
                locationForDebug = "Creating New Feat List";

                pNewFeats = new List<IFeature>();
                pNewFeats.Add(pNewFeat);
                locationForDebug = "Feature Added";


                //bool lineSplit = false;
                locationForDebug = "Looping through IntPoints";

                foreach (IPoint pTmpPnt in pIntPoints)
                {
                    // lineSplit = false;
                    foreach (IFeature pTmpFeat in pNewFeats)
                    {
                        locationForDebug = "Getting point on line";

                        bool Rside = false;
                        if (Globals.GetPointOnLine(pTmpPnt, pTmpFeat.Shape as IPolyline, Globals.GetXYTolerance(fLayer), out  Rside) != null)
                        {
                            locationForDebug = "Checking Result Geo";

                            if ((pTmpFeat.Shape as IPolyline).ToPoint.X == pTmpPnt.X && (pTmpFeat.Shape as IPolyline).ToPoint.Y == pTmpPnt.Y)
                            { }
                            else if ((pTmpFeat.Shape as IPolyline).FromPoint.X == pTmpPnt.X && (pTmpFeat.Shape as IPolyline).FromPoint.Y == pTmpPnt.Y)
                            { }
                            else
                            {
                                try
                                {
                                    locationForDebug = "About to SplitWithUpdate - 123";
                                    pSet = (pSourceFeat = (pTmpFeat as IFeatureEdit2)).SplitWithUpdate(pTmpPnt);
                                    // lineSplit = true;
                                    locationForDebug = pSet.Count + ":Set Count  123";

                                    if (pSet.Count > 0)
                                    {

                                        pSplitFeat = pSet.Next() as IFeature;
                                        while (pSplitFeat != null)
                                        {
                                            locationForDebug = "About to Select";

                                            (app.Document as IMxDocument).FocusMap.SelectFeature(newLayer, pSplitFeat);
                                            pNewFeats.Add(pSplitFeat);
                                            pSplitFeat = pSet.Next() as IFeature;
                                        }
                                    }
                                    break;
                                }
                                catch { }

                            }
                        }

                    }
                    //if (lineSplit == true);

                }
                //List<IFeature> pNewFeats = null ;
                //IFeatureEdit2 pSourceFeat = null;
                //ISet pSourceSet = null;
                //pNewFeats = new List<IFeature>();

                //pNewFeats.Add(pNewFeat);
                //try
                //{
                //    for (int k = 0; k < pNewFeats.Count; k++)
                //    {
                //        pSourceFeat = pNewFeats[k] as IFeatureEdit2;
                //        pSourceSet = pSourceFeat.SplitWithUpdate(pIntPoint);
                //        if (pSourceSet.Count > 0)
                //        {

                //        }
                //    }


                //}
                //catch
                //{
                //}

                mxdoc.ActiveView.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show("SplitAndProrate\n" + ex.Message, "SplitAndProrate:" + locationForDebug);
                return;
            }
            finally
            {
                //pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                //if (pCmd != null)
                //{
                //    pCmd.Execute();
                //}
                // pCmd = null;
                editor = null;
                map = null;


                mxdoc = null;
                layer = null;

                fc = null;
                fSel = null;
                fLayer = null;

                pFeat = null;
                pIntPoint = null;
                featureEdit = null;
                pSet = null;
                pPolyLine = null;
                pSplitFeat = null;
                pIntPoints = null;
                pNewFeats = null;
                pSourceFeat = null;

            }

        }


    }
}
