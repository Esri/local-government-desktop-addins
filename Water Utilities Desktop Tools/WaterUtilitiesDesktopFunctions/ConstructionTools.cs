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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

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
    public class pointAlongSettings
    {

        public IFeatureLayer PointAlongLayer { get; set; }
        public IFeatureLayer PolygonIntersectLayer { get; set; }
        public string PolygonIntersectSide { get; set; }
        public IEditTemplate PointAlongEditTemplate { get; set; }
        public double PointAlongDistance { get; set; }
        public bool DistanceIsPercent { get; set; }
        public bool FoundAsLayer { get; set; }

    }
    public static class CreateLineWithEndPoints
    {

        public static bool CreatePoints(IApplication app, List<ConstructLineWithPointsDetails> linesWithPointsDetails, IPolyline line, IFeatureLayer SourceLayer, bool bUseTemplate, out List<IFeature> pLstFeat)
        {
            pLstFeat = null;

            IFeatureLayer pStartPointLayer = null;
            IFeatureLayer pAlongPointLayer = null;
            IFeatureLayer pEndPointLayer = null;
            IEditor editor = null;
            ICurve pCur = null;// as ICurve; 
            //List<string> strTemplateNames = null;
            IEditTemplate pEditTempStart = null;
            IEditTemplate pEditTempAlong = null;
            IEditTemplate pEditTempEnd = null;
            IFeature pPntFeat = null;
            IEnumVertex pEnumVx = null;
            IPoint ppnt = null;
            try
            {
                editor = Globals.getEditor(app);
                if (linesWithPointsDetails == null) return false;
                if (linesWithPointsDetails.Count == 0) return false;
                //   MessageBox.Show(Control.ModifierKeys.ToString());
                pLstFeat = new List<IFeature>();
                foreach (ConstructLineWithPointsDetails pDet in linesWithPointsDetails)
                {
                    if (pDet.Line_LayerName != SourceLayer.Name)
                        continue;
                    bool FCorLayerStart = true;
                    bool FCorLayerAlong = true;
                    bool FCorLayerEnd = true;

                    pStartPointLayer = Globals.FindLayer(app, pDet.Point_Start_LayerName, ref FCorLayerStart) as IFeatureLayer;
                    pAlongPointLayer = Globals.FindLayer(app, pDet.Point_Along_LayerName, ref FCorLayerAlong) as IFeatureLayer;
                    pEndPointLayer = Globals.FindLayer(app, pDet.Point_End_LayerName, ref FCorLayerEnd) as IFeatureLayer;
                    if (pStartPointLayer == null)
                        continue;
                    else if (pStartPointLayer.FeatureClass == null)
                        continue;
                    //if (pAlongPointLayer == null)
                    //    continue;
                    //else if (pAlongPointLayer.FeatureClass == null)
                    //    continue;
                    if (pEndPointLayer == null)
                        continue;
                    else if (pEndPointLayer.FeatureClass == null)
                        continue;
                    //if (pDet.lineLayerName == SourceLayer.Name)


                    if (!Globals.IsEditable(ref pStartPointLayer, ref editor))
                        return false;
                    if (pAlongPointLayer != null)
                    {
                        if (!Globals.IsEditable(ref pAlongPointLayer, ref editor))
                            return false;
                    }
                    if (!Globals.IsEditable(ref pEndPointLayer, ref editor))
                        return false;
                    //IFeatureLayer pPointLay = Globals.FindLayer(app, pDet.pointLayerName) as IFeatureLayer;

                    pCur = line;// as ICurve; 

                    if (bUseTemplate)
                    {
                        //pEditTempStart = Globals.PromptAndGetEditTemplate(app, pStartPointLayer, pDet.Point_Start_EditTemplate, "Template for Start Layer: " + pStartPointLayer.Name);
                        //pEditTempAlong = Globals.PromptAndGetEditTemplate(app, pAlongPointLayer, pDet.Point_Along_EditTemplate, "Template for Point Along Layer: " + pAlongPointLayer.Name);

                        //pEditTempEnd = Globals.PromptAndGetEditTemplate(app, pEndPointLayer, pDet.Point_End_EditTemplate, "Template for End Layer: " + pEndPointLayer.Name);

                        pEditTempStart = Globals.PromptAndGetEditTemplateGraphic(pStartPointLayer, pDet.Point_Start_EditTemplate);
                        if (pAlongPointLayer != null)
                        {
                            pEditTempAlong = Globals.PromptAndGetEditTemplateGraphic(pAlongPointLayer, pDet.Point_Along_EditTemplate);

                        }
                        pEditTempEnd = Globals.PromptAndGetEditTemplateGraphic(pEndPointLayer, pDet.Point_End_EditTemplate);


                    }
                    else
                    {
                        //pEditTempStart = Globals.PromptAndGetEditTemplate(app, pStartPointLayer, "", "Template for Start Layer: " + pStartPointLayer.Name);
                        //pEditTempAlong = Globals.PromptAndGetEditTemplate(app, pAlongPointLayer, "", "Template for Point Along Layer: " + pAlongPointLayer.Name);

                        //pEditTempEnd = Globals.PromptAndGetEditTemplate(app, pEndPointLayer, "", "Template for End Layer: " + pEndPointLayer.Name);

                        pEditTempStart = Globals.PromptAndGetEditTemplateGraphic(pStartPointLayer, "");
                        if (pAlongPointLayer != null)
                        {
                            pEditTempAlong = Globals.PromptAndGetEditTemplateGraphic(pAlongPointLayer, "");
                        }
                        pEditTempEnd = Globals.PromptAndGetEditTemplateGraphic(pEndPointLayer, "");


                    }

                    if (pDet.PointAtVertices.ToUpper() == "TRUE")
                    {
                        ESRI.ArcGIS.Geometry.IPointCollection4 pPointColl;
                        pPointColl = (IPointCollection4)line;
                        pEnumVx = pPointColl.EnumVertices;
                        pEnumVx.Reset();

                        int partIdx;
                        int verIdx;
                        pEnumVx.Next(out ppnt, out partIdx, out verIdx);
                        int pntIdx = 0;
                        while (ppnt != null)
                        {
                            if (pntIdx == 0)
                            {
                                if (pEditTempStart == null)
                                {
                                    pPntFeat = Globals.CreateFeature(pCur.FromPoint, pStartPointLayer, editor, app, true, false, true);
                                    //editor.Map.SelectFeature(pStartPointLayer, pPntFeat);
                                    //  pPntFeat.Store();
                                }
                                else
                                {
                                    pPntFeat = Globals.CreateFeature(pCur.FromPoint, pEditTempStart, editor, app, true, false, true);
                                    //editor.Map.SelectFeature(pEditTempStart.Layer, pPntFeat);
                                    //pPntFeat.Store();
                                }

                            }
                            else if (pntIdx == pPointColl.PointCount - 1)
                            {

                                if (pEditTempEnd == null)
                                {

                                    pPntFeat = Globals.CreateFeature(pCur.ToPoint, pEndPointLayer, editor, app, true, false, true);
                                    //editor.Map.SelectFeature(pEndPointLayer, pPntFeat);
                                    // pPntFeat.Store();
                                }
                                else
                                {
                                    pPntFeat = Globals.CreateFeature(pCur.ToPoint, pEditTempEnd, editor, app, true, false, true);
                                    //editor.Map.SelectFeature(pEditTempEnd.Layer, pPntFeat);
                                    //   pPntFeat.Store();
                                }
                            }
                            else
                            {
                                if (pAlongPointLayer != null)
                                {
                                    if (pEditTempAlong != null)
                                    {
                                        pPntFeat = Globals.CreateFeature(ppnt, pEditTempAlong, editor, app, true, false, true);
                                        //editor.Map.SelectFeature(pEditTempStart.Layer, pPntFeat);
                                    }
                                    else
                                    {
                                        pPntFeat = Globals.CreateFeature(ppnt, pAlongPointLayer, editor, app, true, false, true);
                                        //editor.Map.SelectFeature(pEditTempStart.Layer, pPntFeat);
                                    }
                                }
                            }

                            if (pPntFeat != null)
                            {
                                pLstFeat.Add(pPntFeat);
                                //pPntFeat.Store();
                            }

                            pntIdx = pntIdx + 1;

                            pEnumVx.Next(out ppnt, out partIdx, out verIdx);
                        }


                    }
                    else
                    {

                        if (pEditTempStart == null)
                        {
                            pPntFeat = Globals.CreateFeature(pCur.FromPoint, pStartPointLayer, editor, app, true, false, true);
                            if (pPntFeat != null)
                            {
                                pLstFeat.Add(pPntFeat);
                                // pPntFeat.Store();
                            }
                        }
                        else
                        {
                            pPntFeat = Globals.CreateFeature(pCur.FromPoint, pEditTempStart, editor, app, true, false, true);
                            if (pPntFeat != null)
                            {
                                pLstFeat.Add(pPntFeat);
                                // pPntFeat.Store();
                            }
                        }

                        if (pEditTempEnd == null)
                        {
                            pPntFeat = Globals.CreateFeature(pCur.ToPoint, pEndPointLayer, editor, app, true, false, true);
                            if (pPntFeat != null)
                            {
                                pLstFeat.Add(pPntFeat);
                                // pPntFeat.Store();
                            }
                        }
                        else
                        {
                            pPntFeat = Globals.CreateFeature(pCur.ToPoint, pEditTempEnd, editor, app, true, false, true);
                            if (pPntFeat != null)
                            {
                                pLstFeat.Add(pPntFeat);
                                // pPntFeat.Store();
                            }
                        }



                    }
                    return Convert.ToBoolean(pDet.TwoPointLines);


                }
                return false;


            }
            catch (Exception ex)
            {

                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("Error") + ex.Message);
                return false;
            }
            finally
            {
                pStartPointLayer = null;
                pAlongPointLayer = null;
                pEndPointLayer = null;
                editor = null;
                pCur = null;// as ICurve; 

                pEditTempStart = null;
                pEditTempAlong = null;
                pEditTempEnd = null;
                pPntFeat = null;
                pEnumVx = null;
                ppnt = null;
            }

        }
    }

    public static class ConnectClosest
    {

        private static string _caption = A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_1");
        public static returnFeatArray ConnectClosestFeatureAtPoint(IApplication app, List<ConnectClosestDetails> connectClosestLayers, IPoint location, string LayerName, bool logOperation, Keys mod)
        {

            bool bSelectedOnly;
            bool bUseTemplate;
            if (mod == Keys.Shift)
            {

                bSelectedOnly = true;
                bUseTemplate = true;

            }
            else if (mod == (Keys.Control | Keys.Shift))
            {
                bSelectedOnly = true;
                bUseTemplate = false;


            }
            else if (mod == Keys.Control)
            {
                bSelectedOnly = false;
                bUseTemplate = false;

            }
            else
            {


                bSelectedOnly = false;
                bUseTemplate = true;
            }
            List<IFeature> pRetFeature = null;
            IEditor editor = null;
            IMouseCursor appCursor = null;
            IMxDocument mxdoc = null;
            IMap map = null;
            IFeatureLayer pTargetLayer = null;
            IEditLayers eLayers = null;
            IFeatureLayer pointFLayer = null;
            IFeatureLayer connectLineFLayer = null;
            IEditTemplate pEditTemp = null;
            IGeometry pNearestFeature = null;
            IPolyline pNewPoly = null;
            IFeature pLine = null;
            returnFeatArray retVal = new returnFeatArray();
            try
            {
                pRetFeature = new List<IFeature>();
                //Get edit session
                editor = Globals.getEditor(app);

                if (editor == null)
                    return null;

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), _caption);
                    //editor = null;

                    return null;
                }

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);

                app = editor.Parent;
                mxdoc = (IMxDocument)app.Document;
                map = editor.Map;



                //Find required layers
                if (connectClosestLayers == null)
                    return null;
                if (connectClosestLayers.Count == 0)
                    return null;


                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = app.hWnd;

                if (logOperation)
                {
                    try
                    {
                        editor.StartOperation();
                    }
                    catch
                    {
                        logOperation = false;
                    }

                }
                pTargetLayer = null;
                bool FCorLayerTarget = true;
                if (LayerName != "")
                {
                    pTargetLayer = Globals.FindLayer(map, LayerName, ref FCorLayerTarget) as IFeatureLayer;

                }

                for (int k = 0; k < connectClosestLayers.Count; k++)
                {
                    if (pTargetLayer.Name.ToString() != (connectClosestLayers[k] as ConnectClosestDetails).Point_Layer)
                    { continue; }

                    // int currentLayerSub;
                    bool FCorLayerPoint = true;
                    bool FCorLayerConnect = true;
                    pointFLayer = Globals.FindLayer(map, (connectClosestLayers[k] as ConnectClosestDetails).Point_Layer, ref FCorLayerPoint) as IFeatureLayer;
                    connectLineFLayer = Globals.FindLayer(map, (connectClosestLayers[k] as ConnectClosestDetails).Line_Layer, ref FCorLayerConnect) as IFeatureLayer;


                    //Report any problems before exiting
                    if (pointFLayer == null)
                    {
                        //MessageBox.Show("Layer representing connection points was not found.  Configuration indicated feature class name: '" + _pointLayerName + "'.", this._caption);
                        //return;
                        continue;
                    }
                    if (connectLineFLayer == null)
                    {
                        //MessageBox.Show("Layer representing connect was not found.  Configuration indicated feature class name: '" + _connectLineLayerName + "'.", this._caption);
                        //return;
                        continue;
                    }
                    if (pTargetLayer != null)
                    {
                        if (pTargetLayer.FeatureClass.CLSID.Value.ToString() != pointFLayer.FeatureClass.CLSID.Value.ToString())

                        { continue; }

                    }


                    //Confirm that target layer is editable and is a line layer
                    eLayers = (IEditLayers)editor;
                    if (!(eLayers.IsEditable(connectLineFLayer)) || (connectLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline))
                        continue;

                    //Confirm the other layers are the correct shape type
                    if (pointFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                        continue;



                    try
                    {



                        if (bUseTemplate)
                        {
                            //pEditTemp = Globals.PromptAndGetEditTemplate(app, connectLineFLayer, (connectClosestLayers[k] as ConnectClosestDetails).Line_EditTemplate);
                            pEditTemp = Globals.PromptAndGetEditTemplateGraphic(connectLineFLayer, (connectClosestLayers[k] as ConnectClosestDetails).Line_EditTemplate);
                        }
                        else
                        {
                            //pEditTemp = Globals.PromptAndGetEditTemplate(app, connectLineFLayer, "");
                            pEditTemp = Globals.PromptAndGetEditTemplateGraphic(connectLineFLayer, "");
                        }

                        pNearestFeature = Globals.GetClosestFeatureIgnoreExistingLineFeature((connectClosestLayers[k] as ConnectClosestDetails).Search_Threshold,
                                                                        location, pointFLayer, connectLineFLayer, bSelectedOnly);

                        if (pNearestFeature == null)
                            break;

                        pNewPoly = new PolylineClass();
                        pNewPoly.FromPoint = pNearestFeature as IPoint;
                        pNewPoly.ToPoint = location as IPoint;


                        if (pEditTemp == null)
                        {
                            pLine = Globals.CreateFeature(pNewPoly, connectLineFLayer, editor, app, false, false, true);
                        }
                        else
                        {
                            pLine = Globals.CreateFeature(pNewPoly, pEditTemp, editor, app, false, false, true);
                        }
                        pLine.Store();
                        pRetFeature.Add(pLine);

                        if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow != null)
                        {
                            if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "DIGITIZED")
                            {
                                retVal.Options = "DIGITIZED";

                            }
                            else if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "ROLE")
                            {
                                retVal.Options = "ROLE";

                            }
                            else if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "Ancillary".ToUpper())
                            {
                                retVal.Options = "Ancillary".ToUpper();

                            }
                            else
                            {
                            }

                        }


                    }

                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ConnectClosestFeatureAtPoint\n" + ex.Message, ex.Source);

                    }


                    if (logOperation)
                    {
                        try
                        {
                            // Stop the edit operation 
                            editor.StopOperation(_caption);

                        }
                        catch
                        {
                            logOperation = false;
                        }

                    }
                }
                (map as IActiveView).Refresh();
                retVal.Features = pRetFeature;
                return retVal;
            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ConnectClosestFeatureAtPoint\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_1"));
                return null;
            }
            finally
            {
                pRetFeature = null;
                editor = null;
                appCursor = null;
                mxdoc = null;
                map = null;
                pTargetLayer = null;
                eLayers = null;
                pointFLayer = null;
                connectLineFLayer = null;
                pEditTemp = null;
                pNearestFeature = null;
                pNewPoly = null;
                pLine = null;
            }
        }

        public static returnFeatArray ConnectClosestFeature(IApplication app, List<ConnectClosestDetails> connectClosestLayers, bool logOperation, bool suppressDialog, string LayerName)
        {
            bool bUseTemplate;
            bool bSelectedOnly;
            if (Control.ModifierKeys == Keys.Shift)
            {

                bSelectedOnly = true;
                bUseTemplate = true;

            }
            else if (Control.ModifierKeys == (Keys.Control | Keys.Shift))
            {
                bSelectedOnly = true;
                bUseTemplate = false;


            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                bSelectedOnly = false;
                bUseTemplate = false;

            }
            else
            {


                bSelectedOnly = false;
                bUseTemplate = true;
            }

            //ProgressBar
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;

            List<IFeature> pRetFeature = new List<IFeature>();
            IEditor editor = null;
            IMouseCursor appCursor = null;
            IMxDocument mxdoc = null;
            IMap map = null;
            IFeatureLayer pTargetLayer = null;
            IFeatureLayer pointFLayer = null;
            IFeatureLayer connectLineFLayer = null;
            ICursor pointCursor = null;
            IFeature pointFeature = null;

            IEditTemplate pEditTemp = null;
            IFeatureSelection pointFeatureSelection = null;
            IGeometry pNearestFeature = null;
            ISelectionSet2 sel = null;
            IPolyline pNewPoly = null;
            IFeature pLine = null;
            IEditLayers eLayers = null;
            returnFeatArray retVal = new returnFeatArray();
            try
            {
                //Get edit session
                trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();
                editor = Globals.getEditor(app);
                if (editor == null)
                    return null;

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), _caption);
                    //_editor = null;

                    return null;
                }

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);

                mxdoc = (IMxDocument)app.Document;
                map = mxdoc.FocusMap;



                //Find required layers
                if (connectClosestLayers == null)
                    return null;
                if (connectClosestLayers.Count == 0)
                    return null;

                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = app.hWnd;
                if (suppressDialog == false)
                {
                    progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
                    stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);

                    stepProgressor.StepValue = 1;
                    stepProgressor.Message = _caption;
                    // Create the ProgressDialog. This automatically displays the dialog
                    progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                    // Set the properties of the ProgressDialog
                    progressDialog.CancelEnabled = true;
                    progressDialog.Title = _caption;
                    progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;
                    progressDialog.ShowDialog();

                }
                // Create an edit operation enabling undo/redo


                bool FCorLayerTarget = true;
                if (LayerName != "")
                {
                    pTargetLayer = Globals.FindLayer(map, LayerName, ref  FCorLayerTarget) as IFeatureLayer;

                }

                for (int k = 0; k < connectClosestLayers.Count; k++)
                {
                    bool FCorLayerPoint = true;
                    bool FCorLayerConnect = true;
                    //int currentLayerSub;
                    pointFLayer = Globals.FindLayer(map, (connectClosestLayers[k] as ConnectClosestDetails).Point_Layer, ref FCorLayerPoint) as IFeatureLayer;
                    connectLineFLayer = Globals.FindLayer(map, (connectClosestLayers[k] as ConnectClosestDetails).Line_Layer, ref FCorLayerConnect) as IFeatureLayer;


                    //Report any problems before exiting
                    if (pointFLayer == null)
                    {
                        //MessageBox.Show("Layer representing connection points was not found.  Configuration indicated feature class name: '" + _pointLayerName + "'.", _caption);
                        //return;
                        continue;
                    }
                    if (connectLineFLayer == null)
                    {
                        //MessageBox.Show("Layer representing connect was not found.  Configuration indicated feature class name: '" + _connectLineLayerName + "'.", _caption);
                        //return;
                        continue;
                    }
                    if (pTargetLayer != null)
                    {
                        if (pTargetLayer.FeatureClass.CLSID.Value.ToString() != pointFLayer.FeatureClass.CLSID.Value.ToString())

                        { continue; }
                    }

                    //Verify that some points are selected

                    pointFeatureSelection = (IFeatureSelection)pointFLayer;
                    if (pointFeatureSelection.SelectionSet.Count == 0)
                        continue;

                    //Confirm that target layer is editable and is a line layer
                    eLayers = (IEditLayers)editor;
                    if (!(eLayers.IsEditable(connectLineFLayer)) || (connectLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline))
                        continue;

                    //Confirm the other layers are the correct shape type
                    if (pointFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                        continue;
                    if (suppressDialog == false)
                    {
                        stepProgressor.MinRange = 0;
                        stepProgressor.MaxRange = connectClosestLayers.Count;
                        progressDialog.Title = (connectClosestLayers[k] as ConnectClosestDetails).Line_Layer;
                        stepProgressor.Message = (connectClosestLayers[k] as ConnectClosestDetails).Line_Layer;
                        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ConnectAsset") + "1" + A4LGSharedFunctions.Localizer.GetString("Of") + pointFeatureSelection.SelectionSet.Count + ".";
                    }

                    int total = pointFeatureSelection.SelectionSet.Count;
                    int i = 0;


                    try
                    {

                        if (logOperation)
                        {
                            try
                            {
                                editor.StartOperation();
                            }
                            catch
                            {
                                logOperation = false;
                            }

                        }


                        if (bUseTemplate)
                        {
                            pEditTemp = Globals.PromptAndGetEditTemplateGraphic(connectLineFLayer, connectClosestLayers[k].Line_EditTemplate);
                            //pEditTemp = Globals.PromptAndGetEditTemplate(app, connectLineFLayer, connectClosestLayers[k].Line_EditTemplate);
                        }
                        else
                        {
                            //pEditTemp = Globals.PromptAndGetEditTemplate(app, connectLineFLayer, "");
                            pEditTemp = Globals.PromptAndGetEditTemplateGraphic(connectLineFLayer, "");
                        }

                        sel = pointFeatureSelection.SelectionSet as ISelectionSet2;
                        //sel.Update(null, false, out pointCursor);
                        sel.Search(null, false, out pointCursor);
                        while ((pointFeature = (IFeature)pointCursor.NextRow()) != null)
                        {
                            i += 1;
                            if (suppressDialog == false)
                            {
                                //Update progress bar
                                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("ConnectAsset") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + "." + Environment.NewLine +
                                  A4LGSharedFunctions.Localizer.GetString("CurrentOID") + pointFeature.OID;
                                stepProgressor.Step();
                            }
                            ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                            statusBar.set_Message(0, i.ToString());

                            //Check if the cancel button was pressed. If so, stop process
                            bool boolean_Continue = trackCancel.Continue();
                            if (!boolean_Continue)
                            {
                                break;
                            }

                            //IFeature pNearestFeature = 
                            pNearestFeature = Globals.GetClosestFeatureIgnoreExistingLineFeature((connectClosestLayers[k] as ConnectClosestDetails).Search_Threshold,
                                                                             pointFeature.ShapeCopy, pointFLayer, connectLineFLayer, bSelectedOnly);

                            if (pNearestFeature == null)
                                break;

                            pNewPoly = new PolylineClass();
                            pNewPoly.FromPoint = pNearestFeature as IPoint;
                            pNewPoly.ToPoint = pointFeature.ShapeCopy as IPoint;



                            if (pEditTemp == null)
                            {
                                pLine = Globals.CreateFeature(pNewPoly, connectLineFLayer, editor, app, false, false, true);
                            }
                            else
                            {
                                pLine = Globals.CreateFeature(pNewPoly, pEditTemp, editor, app, false, false, true);
                            }
                            pLine.Store();
                            pRetFeature.Add(pLine);


                        }
                        if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow != null)
                        {
                            if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "DIGITIZED")
                            {
                                retVal.Options = "DIGITIZED";


                            }
                            else if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "ROLE")
                            {
                                retVal.Options = "ROLE";

                            }
                            else if ((connectClosestLayers[k] as ConnectClosestDetails).Reset_Flow.ToUpper() == "Ancillary".ToUpper())
                            {
                                retVal.Options = "ANCILLARY";

                            }
                            else
                            {
                            }

                        }

                        if (logOperation)
                        {
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation((connectClosestLayers[k] as ConnectClosestDetails).Point_Layer);

                            }
                            catch
                            {
                                logOperation = false;
                            }

                        }

                    }

                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ConnectClosestFeature\n" + ex.Message, ex.Source);

                        // Cleanup
                        if (progressDialog != null)
                            progressDialog.HideDialog();
                        return null;
                    }



                }
                (map as IActiveView).Refresh();
                retVal.Features = pRetFeature;
                return retVal;
            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "ConnectClosestFeature\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_1"));
                return null;
            }
            finally
            {// Cleanup
                if (progressDialog != null)
                    progressDialog.HideDialog();
                if (pointCursor != null)
                    Marshal.ReleaseComObject(pointCursor);
                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog = null;
                trackCancel = null;

                pRetFeature = null;
                editor = null;
                appCursor = null;
                mxdoc = null;
                map = null;
                pTargetLayer = null;
                pointFLayer = null;
                connectLineFLayer = null;
                pointCursor = null;
                pointFeature = null;

                pEditTemp = null;
                pointFeatureSelection = null;
                pNearestFeature = null;
                sel = null;
                pNewPoly = null;
                pLine = null;
                eLayers = null;


            }
        }

    }

    public static class AddLateralsLinesCmds
    {
        private static string _caption = A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2");


        public static string AddLaterals(IApplication app, List<AddLateralDetails> addLateralsDetails, IFeature inFeatures, bool logOperation, bool suppressDialog, bool store, bool ForceSourcePointConnection) {
            return AddLaterals(app, addLateralsDetails, inFeatures, logOperation, suppressDialog, store, ForceSourcePointConnection, null);

        }

        public static string AddLaterals(IApplication app, List<AddLateralDetails> addLateralsDetails, IFeature inFeatures, bool logOperation, bool suppressDialog, bool store, bool ForceSourcePointConnection, IFeatureLayer pEditLayer)
        {
            ICommandItem pCmdItem;
            string resetFlow = "";
            bool useDefaultTemplate;
            List<IFeature> ComplFeat = new List<IFeature>();
            IMap map = null;
            IEditor editor = null;
            IMouseCursor appCursor = null;
            IMxDocument mxdoc = null;
            IFeatureLayer pointFLayer = null;
            IFeatureLayer matchLineFLayer = null;
            IFeatureLayer targetLineFLayer = null;
            IEditLayers eLayers = null;
            ISelectionSet2 pointSelSet = null;
            IFeatureSelection pointFeatureSelection = null;
            IEditTemplate pLateralLineEditTemp = null;
            List<pointAlongSettings> pointAlongLayers = null;
            pointAlongSettings pointAlongLayer = null;
            ICursor pointCursor = null;
            IFeature pointFeature = null;

            //ProgressBar
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;


            try
            {

                if (Control.ModifierKeys == Keys.Control)
                {
                    useDefaultTemplate = false;
                }
                else
                {

                    useDefaultTemplate = true;
                }
                bool boolSelectedEdges = false;
                if (Control.ModifierKeys == Keys.Shift)
                {
                    boolSelectedEdges = true;
                }
                else
                {

                    boolSelectedEdges = false;
                }
                //Get edit session
                bool LatCreated = false;

                editor = Globals.getEditor(app);
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), _caption);
                    editor = null;

                    return "";
                }

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);


                mxdoc = (IMxDocument)app.Document;
                map = editor.Map;
   
                for (int k = 0; k < addLateralsDetails.Count; k++)
                {
                    bool FCorLayerPoint = true;
                    if (pEditLayer != null)
                    {
                        if (pEditLayer.Name == addLateralsDetails[k].Point_LayerName)
                        {
                            pointFLayer = pEditLayer;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else { 
                        pointFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].Point_LayerName, ref FCorLayerPoint);
                    }
                    if (inFeatures != null)
                    {
                        if (pointFLayer == null)
                            continue;
                        if (pointFLayer.FeatureClass == null)
                            continue;

                        if (inFeatures.Class.CLSID.ToString() != pointFLayer.FeatureClass.CLSID.ToString())
                            continue;
                        if (inFeatures.Class.ObjectClassID.ToString() != pointFLayer.FeatureClass.ObjectClassID.ToString())
                            continue;
                        if (inFeatures.Class.AliasName.ToString() != pointFLayer.FeatureClass.AliasName.ToString())
                            continue;

                    }
                    //Report any problems before exiting
                    if (pointFLayer == null)
                    {
                        continue;
                    }

                    bool FCorLayerMatch = true;
                    bool FCorLayerTarget = true;

                    matchLineFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].MainLine_LayerName, ref FCorLayerMatch);
                    targetLineFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].LateralLine_LayerName, ref FCorLayerTarget);

                    // IFeatureLayerDefinition2 pFeatLayerdef = matchLineFLayer as IFeatureLayerDefinition2;

                    if (matchLineFLayer == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_1") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (matchLineFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_1") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (matchLineFLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_2") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_3") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_3") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_4") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }


                    //Confirm the other layers are the correct shape type
                    if (pointFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint || matchLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        return "";


                    //Confirm that target layer is editable and is a line layer
                    eLayers = (IEditLayers)editor;
                    if (!(eLayers.IsEditable(targetLineFLayer)) || (targetLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline))
                        return "";

                    //Confirm that the two line layers are different Feature classes
                    if ((matchLineFLayer.FeatureClass.CLSID == targetLineFLayer.FeatureClass.CLSID) && (matchLineFLayer.FeatureClass.AliasName == targetLineFLayer.FeatureClass.AliasName))
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_1") , A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_2") );
                        return "";
                    }


                    //Verify that some points are selected
                    pointFeatureSelection = (IFeatureSelection)pointFLayer;

                    if (pointFeatureSelection.SelectionSet.Count == 0)
                        continue;

                    pointSelSet = pointFeatureSelection.SelectionSet as ISelectionSet2;





                    if (useDefaultTemplate)
                    {
                        //pLateralLineEditTemp = Globals.PromptAndGetEditTemplate(app, targetLineFLayer, addLateralsDetails[k].LateralLine_EditTemplate);
                        pLateralLineEditTemp = Globals.PromptAndGetEditTemplateGraphic(targetLineFLayer, addLateralsDetails[k].LateralLine_EditTemplate);
                    }
                    else
                    {
                        pLateralLineEditTemp = Globals.PromptAndGetEditTemplateGraphic(targetLineFLayer, "");
                        //pLateralLineEditTemp = Globals.PromptAndGetEditTemplate(app, targetLineFLayer, "");
                    }







                    if (addLateralsDetails[k].PointAlong != null)
                    {


                        if (addLateralsDetails[k].PointAlong.Length > 0)
                        {
                            pointAlongLayers = new List<pointAlongSettings>();


                            // IEditTemplate pPointAlongEditTemp;
                            for (int j = 0; j < addLateralsDetails[k].PointAlong.Length; j++)
                            {
                                pointAlongLayer = new pointAlongSettings();
                                bool FCorLayerPointsAlong = true;
                                pointAlongLayer.PointAlongLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].PointAlong[j].LayerName, ref FCorLayerPointsAlong);
                                if (pointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";

                                }
                                else if (pointAlongLayer.PointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";


                                }
                                else if (pointAlongLayer.PointAlongLayer.FeatureClass == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_2") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";
                                }
                                else if (pointAlongLayer.PointAlongLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint)
                                {
                                    MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_3") , A4LGSharedFunctions.Localizer.GetString("Warning") );

                                    return "";
                                }

                                pointAlongLayer.PolygonIntersectSide = addLateralsDetails[k].PointAlong[j].PolygonOffsetSide;
                                if (pointAlongLayer.PolygonIntersectSide == null)
                                {
                                    pointAlongLayer.PolygonIntersectSide = "TO";

                                }
                                bool FCorLayerTemp = true;
                                pointAlongLayer.PolygonIntersectLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].PointAlong[j].PolygonOffsetLayerName, ref FCorLayerTemp);
                                pointAlongLayer.FoundAsLayer = FCorLayerTemp;
                                if (pointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";

                                }
                                else if (pointAlongLayer.PolygonIntersectLayer != null)
                                {

                                    if (pointAlongLayer.PolygonIntersectLayer.FeatureClass != null)
                                    {

                                        //Confirm that target layer is editable and is a line layer
                                        if (pointAlongLayer.PolygonIntersectLayer != null)
                                        {
                                            if (pointAlongLayer.PolygonIntersectLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                                            {
                                                MessageBox.Show(addLateralsDetails[k].PointAlong[j].PolygonOffsetLayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_3"));


                                                return "";
                                            }

                                        }
                                    }
                                }
                                //Confirm that target layer is editable and is a line layer
                                if (pointAlongLayer.PointAlongLayer != null)
                                {
                                    if (!(eLayers.IsEditable(pointAlongLayer.PointAlongLayer)) || (pointAlongLayer.PointAlongLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint))
                                    {
                                        MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_4"));


                                        return "";
                                    }
                                    if (useDefaultTemplate)
                                    {
                                        pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplateGraphic(pointAlongLayer.PointAlongLayer, addLateralsDetails[k].PointAlong[j].EditTemplate);
                                        //pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplate(app, pointAlongLayer.PointAlongLayer, addLateralsDetails[k].PointAlong[j].EditTemplate);
                                    }
                                    else
                                    {
                                        pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplateGraphic(pointAlongLayer.PointAlongLayer, "");
                                        //pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplate(app, pointAlongLayer.PointAlongLayer, "");
                                    }

                                }


                                //if (addLateralsDetails[k].PointAlong[j].Distance < 0)
                                //    pointAlongLayer.PointAlongDistance = 0;
                                //else
                                pointAlongLayer.PointAlongDistance = (double)addLateralsDetails[k].PointAlong[j].Distance;


                                //if (addLateralsDetails[k].PointAlong[j].DistanceIsPercent != null)
                                pointAlongLayer.DistanceIsPercent = (bool)addLateralsDetails[k].PointAlong[j].DistanceIsPercent;
                                //else
                                //  pointAlongLayer.DistanceIsPercent =false;







                                pointAlongLayers.Add(pointAlongLayer);



                                //Verify subtype is valid for target point 
                                //if (targetPointFLayer != null)
                                //{
                                //    ISubtypes targetPointSubtypes = targetPointFLayer[j].FeatureClass as ISubtypes;
                                //    //string targetPointSubtypeName = targetPointSubtypes.get_SubtypeName(_targetPointSubtype);
                                //    if ((targetPointSubtypes == null) || (!targetPointSubtypes.HasSubtype))// || (String.IsNullOrEmpty(targetPointSubtypeName)))
                                //        addLateralsDetails[k].PointAlong[j].Subtype = -1;
                                //    else
                                //    {
                                //        try
                                //        {
                                //            string SubVal = targetPointSubtypes.get_SubtypeName(addLateralsDetails[k].PointAlong[j].Subtype);
                                //            //  addLateralsDetails[k].PointAlong[j].Subtype = SubVal
                                //            //targetPointSubtype[k] = addLateralsDetails[k].PointAlong[j].Subtype;

                                //        }
                                //        catch
                                //        {
                                //            addLateralsDetails[k].PointAlong[j].Subtype = targetPointSubtypes.DefaultSubtypeCode;
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    addLateralsDetails[k].PointAlong[j].Subtype = -1;
                                //}
                                //addLateralsDetails[k].PointAlong[j].Distance
                                //    addLateralsDetails[k].PointAlong[j].DistanceIsPercent
                                //        addLateralsDetails[k].PointAlong[j].FieldToPopulate
                                //        addLateralsDetails[k].PointAlong[j].ValueToPopulate
                            }
                        }
                    }
                    //****************************************

                    int total;

                    total = pointSelSet.Count;

                    int i = 0;

                    // Create a CancelTracker
                    trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                    // Set the properties of the Step Progressor
                    System.Int32 int32_hWnd = app.hWnd;
                    if (suppressDialog == false)
                    {
                        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
                        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);

                        stepProgressor.MinRange = 0;
                        stepProgressor.MaxRange = total;
                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = _caption;
                        // Create the ProgressDialog. This automatically displays the dialog
                        progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog.CancelEnabled = true;
                        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("AddLine") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
                        progressDialog.Title = _caption;
                        progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;
                        progressDialog.ShowDialog();

                    }
                    // Create an edit operation enabling undo/redo

                    if (logOperation)
                    {
                        try
                        {
                            editor.StartOperation();
                        }
                        catch
                        {
                            logOperation = false;
                        }

                    }

                    IPoint fromPoint = null;
                    IPoint selPt1 = null;
                    IPoint selPt2 = null;
                    ILine distanceLine = null;
                    object Missing = null;
                    IEnvelope env = null;
                    IEnumIDs selIds = null;
                    IFeature pointFeature2 = null;
                    List<int> completedOIDArrayList = null;
                    ITopologicalOperator topoOp = null;
                    IPolygon poly = null;
                    ISpatialFilter sFilter = null;
                    IFeatureCursor lineCursor = null;
                    INetworkFeature pNF = null;
                    IFeature testPointFeature = null;

                    int featOID1, featOID2, nearbyCount;

                    try
                    {

                        // ISelectionSet2 sel = pointSelSet as ISelectionSet2;
                        pointSelSet.Update(null, false, out pointCursor);
                        completedOIDArrayList = new List<int>();

                        while ((pointFeature = (IFeature)pointCursor.NextRow()) != null)
                        {
                            try
                            {
                                //if (inFeatures != null)
                                //{
                                //    if (pointFeature.OID != inFeatures.OID)
                                //    {
                                //        continue;

                                //    }
                                //}
                                i += 1;
                                if (suppressDialog == false)
                                {
                                    //Update progress bar
                                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("AddLine") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + "." + Environment.NewLine +
                                      A4LGSharedFunctions.Localizer.GetString("CurrentOID") + pointFeature.OID;
                                    stepProgressor.Step();
                                }
                                ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                                statusBar.set_Message(0, i.ToString());

                                //Check if the cancel button was pressed. If so, stop process
                                bool boolean_Continue = trackCancel.Continue();
                                if (!boolean_Continue)
                                {
                                    break;
                                }

                                if (!ComplFeat.Contains(pointFeature))
                                {
                                    //Get the "from" point for new line (start from selected point)
                                    fromPoint = pointFeature.ShapeCopy as IPoint;



                                    //Create new feature(s)

                                    env = new EnvelopeClass();

                                    //Dual Laterals When Two Selected
                                    if (total == 2 && addLateralsDetails[k].Dual_When_Two_Selected)
                                    {
                                        if (suppressDialog == false)
                                        {
                                            //Update progress bar
                                            progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("AddLine") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + "." + Environment.NewLine +
                                              A4LGSharedFunctions.Localizer.GetString("CurrentOID") + pointFeature.OID;
                                            stepProgressor.Step();
                                        }
                                        //Obtain both starting points
                                        selIds = pointSelSet.IDs;
                                        selIds.Reset();
                                        featOID1 = selIds.Next();
                                        featOID2 = selIds.Next();
                                        pointFeature2 = pointFLayer.FeatureClass.GetFeature(featOID2);
                                        selPt1 = pointFeature.ShapeCopy as IPoint;
                                        selPt2 = pointFeature2.ShapeCopy as IPoint;

                                        //Measure distance
                                        distanceLine = new LineClass();
                                        distanceLine.PutCoords(selPt1, selPt2);
                                        if (distanceLine.Length <= addLateralsDetails[k].Dual_Max_Distance_When_Two_Selected)
                                        {
                                            LatCreated = CreateDual(ref app, ref editor, pointFeature, pointFeature2, distanceLine, matchLineFLayer,
                                                              targetLineFLayer, pLateralLineEditTemp, pointAlongLayers,
                                                              addLateralsDetails[k].DeleteExistingLines,
                                                              addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].Dual_Option_Make_Square,
                                                              addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                              addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature2);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature);
                                            //CreateDualOld(pointFeature, pointFeature2, distanceLine, matchLineFLayer,
                                            //                lineFeature, targetLineFLayer, targetPointFLayer, addLateralsDetails[k].DeleteExistingLines,
                                            //                 addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].Dual_Option_Make_Square,
                                            //                 addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                            //                 addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                            //                 addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                            //                 addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                            //                 addLateralsDetails[k].PointAlong);



                                            //_targetPointDistance, _targetPointDistanceIsPercent, _targetPointSubtype, _targetPointValue, _targetPointFieldName);
                                            break;
                                        }
                                        //Create two single laterals if the duals are not created
                                        else
                                        {
                                            LatCreated = CreateSingle(ref app, ref editor, pointFeature, matchLineFLayer, targetLineFLayer, pLateralLineEditTemp, pointAlongLayers,
                                                             addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                                             addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                             addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature);

                                            LatCreated = CreateSingle(ref app, ref editor, pointFeature2, matchLineFLayer, targetLineFLayer, pLateralLineEditTemp, pointAlongLayers,
                                                            addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                                            addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                            addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature2);

                                            //CreateSingleOld(pointFeature, matchLineFLayer, lineFeature, targetLineFLayer, targetPointFLayer,
                                            //                addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                            //                addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                            //                addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                            //                addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                            //                addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                            //                addLateralsDetails[k].PointAlong);

                                            //CreateSingleOld(pointFeature2, matchLineFLayer, lineFeature, targetLineFLayer, targetPointFLayer,
                                            //                addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                            //                addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                            //                addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                            //                addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                            //                addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                            //                addLateralsDetails[k].PointAlong);
                                            break;
                                        }
                                    }

                                    //Dual Laterals when Nearby
                                    else if ((total != 1) & addLateralsDetails[k].Dual_When_Nearby)
                                    {

                                        //Check that this feature has not already been completed
                                        if (completedOIDArrayList.Contains(pointFeature.OID))
                                            continue;

                                        selPt1 = pointFeature.ShapeCopy as IPoint;
                                        nearbyCount = 0;
                                        pointFeature2 = null;

                                        //Determine if extactly one other point is within the specified max distance
                                        topoOp = selPt1 as ITopologicalOperator;
                                        poly = topoOp.Buffer(addLateralsDetails[k].Dual_Max_Distance_When_Nearby / 2) as IPolygon;
                                        sFilter = new SpatialFilterClass();
                                        sFilter.Geometry = poly;
                                        sFilter.GeometryField = pointFLayer.FeatureClass.ShapeFieldName;
                                        sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                                        if (addLateralsDetails[k].SearchOnLayer)
                                            lineCursor = pointFLayer.Search(sFilter, false);
                                        else
                                            lineCursor = pointFLayer.FeatureClass.Search(sFilter, false);

                                        while ((testPointFeature = lineCursor.NextFeature()) != null)
                                        {
                                            if (testPointFeature.OID != pointFeature.OID)
                                            {
                                                //Check that this nearby feature has not already been completed
                                                if (!completedOIDArrayList.Contains(pointFeature.OID))
                                                {
                                                    pointFeature2 = testPointFeature;
                                                    nearbyCount += 1;
                                                }
                                            }
                                            if (nearbyCount > 1)
                                                break;
                                        }

                                        if (nearbyCount == 1)
                                        {
                                            selPt2 = pointFeature2.ShapeCopy as IPoint;

                                            //Measure distance
                                            distanceLine = new LineClass();
                                            distanceLine.PutCoords(selPt1, selPt2);
                                            LatCreated = CreateDual(ref app, ref editor, pointFeature, pointFeature2, distanceLine, matchLineFLayer,
                                                              targetLineFLayer, pLateralLineEditTemp, pointAlongLayers, addLateralsDetails[k].DeleteExistingLines,
                                                              addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].Dual_Option_Make_Square,
                                                              addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                              addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature2);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature);
                                            //CreateDualOld(pointFeature, pointFeature2, distanceLine, matchLineFLayer,
                                            //               lineFeature, targetLineFLayer, targetPointFLayer, addLateralsDetails[k].DeleteExistingLines,
                                            //                addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].Dual_Option_Make_Square,
                                            //                addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                            //                addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                            //                addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                            //                addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                            //                addLateralsDetails[k].PointAlong);
                                            //Add 2nd OID to completed list
                                            completedOIDArrayList.Add(pointFeature2.OID);
                                        }

                                        //Create a single lateral if 1 nearby not found
                                        else
                                        {
                                            LatCreated = CreateSingle(ref app, ref editor, pointFeature, matchLineFLayer, targetLineFLayer, pLateralLineEditTemp, pointAlongLayers,
                                                addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                                            addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                            addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                            if (LatCreated)
                                                ComplFeat.Add(pointFeature);
                                            //CreateSingleOld(pointFeature, matchLineFLayer, lineFeature, targetLineFLayer, targetPointFLayer,
                                            //    addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                            //                addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                            //                addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                            //                addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                            //                addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                            //                addLateralsDetails[k].PointAlong);
                                        }
                                    }
                                    //Single Laterals
                                    else
                                    {
                                        LatCreated = CreateSingle(ref app, ref editor, pointFeature, matchLineFLayer, targetLineFLayer, pLateralLineEditTemp, pointAlongLayers,
                                              addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                                             addLateralsDetails[k].FromToFields, addLateralsDetails[k].Hook_DoglegDistance,
                                                            addLateralsDetails[k].Hook_DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete, store, addLateralsDetails[k].SearchOnLayer, addLateralsDetails[k].SearchDistance, addLateralsDetails[k].Hook_Angle, boolSelectedEdges);
                                        if (LatCreated)
                                            ComplFeat.Add(pointFeature);
                                        //CreateSingleOld(pointFeature, matchLineFLayer, lineFeature, targetLineFLayer, targetPointFLayer,
                                        //     addLateralsDetails[k].LateralLine_StartAtMain, addLateralsDetails[k].DeleteExistingLines,
                                        //                   addLateralsDetails[k].Point_FieldToCalcFromMain, addLateralsDetails[k].Main_FieldToCalcForPoint,
                                        //                   addLateralsDetails[k].Point_PrefixForMainValue, addLateralsDetails[k].LateralLine_ValueToPopulate,
                                        //                   addLateralsDetails[k].LateralLine_FieldToPopulate, targetLineSubValue,
                                        //                   addLateralsDetails[k].Hook_DoglegDistance, addLateralsDetails[k].DistanceIsPercent, addLateralsDetails[k].TolerenceForDelete,
                                        //                   addLateralsDetails[k].PointAlong);
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);

                            }
                            finally
                            {

                            }
                            //   addLateralsDetails[k].InitDefaults();
                            if (addLateralsDetails[k].Reset_Flow != null)
                            {
                                resetFlow = addLateralsDetails[k].Reset_Flow;
                               

                                if (resetFlow.ToUpper() == "DIGITIZED")
                                {
                                    pCmdItem = Globals.GetCommand("A4WaterUtilities_EstablishFlowDigitized", app);
                                    if (pCmdItem != null)
                                    {
                                        pCmdItem.Execute();
                                    }
                                    else
                                    {
                                        pCmdItem = Globals.GetCommand("A4GasUtilities_EstablishFlowDigitized", app);
                                        if (pCmdItem != null)
                                        {
                                            pCmdItem.Execute();
                                        }
                                    }

                                }
                                else if (resetFlow.ToUpper() == "ROLE")
                                {
                                    pCmdItem = Globals.GetCommand("A4WaterUtilities_EstablishFlowDigitized", app);
                                    if (pCmdItem != null)
                                    {
                                        pCmdItem.Execute();
                                    }
                                    else
                                    {
                                        pCmdItem = Globals.GetCommand("A4GasUtilities_EstablishFlowDigitized", app);
                                        if (pCmdItem != null)
                                        {
                                            pCmdItem.Execute();
                                        }
                                    }
                                }
                                else if (resetFlow.ToUpper() == "Ancillary".ToUpper())
                                {
                                    pCmdItem = Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", app);
                                    if (pCmdItem != null)
                                    {
                                        pCmdItem.Execute();
                                    }
                                    else
                                    {
                                        pCmdItem = Globals.GetCommand("A4GasUtilities_EstablishFlowAncillary", app);
                                        if (pCmdItem != null)
                                        {
                                            pCmdItem.Execute();
                                        }
                                    }
                                }
                                else
                                {
                                }
                                }

                        }

                        if (ForceSourcePointConnection)
                        {
                            foreach (IFeature sourcePnt in ComplFeat)
                            {
                                if (sourcePnt != null)
                                {
                                    if (sourcePnt.Shape.IsEmpty != true)
                                    {
                                        if (sourcePnt is INetworkFeature)
                                        {
                                            pNF = (INetworkFeature)sourcePnt;
                                            try
                                            {
                                                pNF.Connect();
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                }
                            }


                        }



                    }
                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);


                    }
                    finally
                    {
                        // Cleanup
                        if (progressDialog != null)
                            progressDialog.HideDialog();
                        if (lineCursor != null)
                            Marshal.ReleaseComObject(lineCursor);

                        pNF = null;
                        fromPoint = null;
                        selPt1 = null;
                        selPt2 = null;
                        distanceLine = null;
                        Missing = null;
                        env = null;
                        selIds = null;
                        pointFeature2 = null;
                        completedOIDArrayList = null;
                        topoOp = null;
                        poly = null;
                        sFilter = null;
                        lineCursor = null;
                        testPointFeature = null;
                    }



                    if (logOperation)
                    {
                        try
                        {
                            // Stop the edit operation 
                            editor.StopOperation(_caption);

                        }
                        catch
                        {
                            logOperation = false;
                        }

                    }


                    //88
                }




                return resetFlow;

            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);
                return "";
            }
            finally
            {
                ComplFeat.Clear();
                if (map != null)
                {
                    (map as IActiveView).Refresh();
                }
                if (progressDialog != null)
                    progressDialog.HideDialog();

                ComplFeat = null;
                map = null;
                editor = null;
                appCursor = null;
                mxdoc = null;
                pointFLayer = null;
                matchLineFLayer = null;
                targetLineFLayer = null;
                eLayers = null;
                pointSelSet = null;
                pointFeatureSelection = null;
                pLateralLineEditTemp = null;
                pointAlongLayers = null;
                pointAlongLayer = null;
                pointCursor = null;
                pointFeature = null;

                //ProgressBar
                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog = null;
                // Create a CancelTracker
                trackCancel = null;


            }
        }

        #region Helper methods


        private static bool CreateSingle(ref IApplication app, ref  IEditor editor, IFeature pointFeature, IFeatureLayer matchLineFLayer,
                                                  IFeatureLayer targetLineFLayer, IEditTemplate targetLineEditTemplate,
                                                  List<pointAlongSettings> pointAlongLayers,
                                                  bool startAtMain, bool deleteExistingLines, FromToField[] fromToPairs, double doglegDistance,
                                                  bool DistAsPercent, double tolerenceForDelete, bool store, bool SearchOnLayer, int searchDistance, double angle, bool checkSelection)
        {

            IFeature lineFeature = null;
            IPoint thisPoint = null;
            IPoint turnPoint = null;
            IPoint toPoint = null;
            ICurve mainCurve = null;
            IPolyline polyline = null;
            IFeature pFeat = null;
            try
            {
                // Get closest main to point1

                thisPoint = (IPoint)pointFeature.ShapeCopy;

                lineFeature = Globals.GetClosestFeature(thisPoint, matchLineFLayer, Convert.ToDouble(searchDistance), SearchOnLayer, checkSelection);
                //    (_app.Document as IMxDocument).FocusMap.ClearSelection();

                if (lineFeature != null)
                {
                    //Delete any existing lateral lines at this location
                    if (deleteExistingLines)
                    {
                        DeleteExistingFeatures(pointFeature, targetLineFLayer, pointAlongLayers, tolerenceForDelete);
                    }


                    //Determine To and Turn Points
                    mainCurve = lineFeature.ShapeCopy as ICurve;
                    CreateToAndTurnPoints(mainCurve, thisPoint, out toPoint, out turnPoint, doglegDistance, DistAsPercent);

                    //Create the new line
                    polyline = Globals.CreatePolylineFromPointsNewTurn(thisPoint, turnPoint, toPoint, ref matchLineFLayer, ref lineFeature, SearchOnLayer, angle, editor.Map.SpatialReference);
                    //If requested, store pipe id in the point
                    StorePipeInfoPointFeature(lineFeature, pointFeature, fromToPairs, store);

                    if (polyline.Length > 0)
                    {
                        if (startAtMain)
                            polyline.ReverseOrientation();


                        if (targetLineEditTemplate != null)
                        {
                            pFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                        }
                        else
                        {
                            pFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                        }
                        if (pFeat == null)
                        {
                            editor.AbortOperation();
                            return false;

                        }
                        //Globals.SetFlowDirection(pFeat, targetLineFLayer);
                        try
                        {
                            if (pFeat != null)
                            {
                                Globals.ValidateFeature(pFeat);
                                pFeat.Store();
                            }
                            //if (pFeat is INetworkFeature)
                            //{
                            //    INetworkFeature pNF = (INetworkFeature)pFeat;

                            //    pNF.Connect();
                            //}
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show("The Feature could not be stored, this is typically because the layer has Z and the geometric network was not created to honors, you need to drop the network and recreate it with Z's enabled\n" + ex.Message);
                        }

                        //Old Way
                        //IFeature pFeat = CreateLineFeature(targetLineFLayer, newPolyLine, targetLineValue, targetLineFieldName, targetLineSubtype);

                        //Optionally, create new point along line
                        // int idx = 0;
                        if (pointAlongLayers != null)
                        {
                            foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                            {
                                if (pPointAlongLayer.PolygonIntersectLayer != null)
                                    Globals.AddPointAlongLineWithIntersect(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate, pPointAlongLayer.PolygonIntersectLayer, pPointAlongLayer.PolygonIntersectSide);

                                else
                                    Globals.AddPointAlongLine(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate);
                                //   idx++;
                            }
                        }

                        //Globals.SetFlowDirection(pFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                        //try
                        //{
                        //    if (pFeat != null)
                        //    {
                        //      //  Globals.ValidateFeature(pFeat);
                        //        pFeat.Store();
                        //    }
                        //    //if (pFeat is INetworkFeature)
                        //    //{
                        //    //    INetworkFeature pNF = (INetworkFeature)pFeat;

                        //    //    pNF.Connect();
                        //    //}
                        //}
                        //catch (Exception ex)
                        //{
                        //    // MessageBox.Show("The Feature could not be stored, this is typically because the layer has Z and the geometric network was not created to honors, you need to drop the network and recreate it with Z's enabled\n" + ex.Message);
                        //}
                        return true;
                        //AddPointAlongLine(newPolyLine as ICurve, targetPointFLayer, targetPointDistance, targetPointDistanceIsPercent, targetPointSubtype, targetPointValue, targetPointFieldName);
                    }
                    else
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_6"));
                        return false;

                    }
                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_5a") + matchLineFLayer.Name + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_5b") + searchDistance + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_5c"));

                    return false;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_3") + "\n" + ex.Message, ex.Source);
                return false;
            }

            finally
            {
                lineFeature = null;
                thisPoint = null;
                turnPoint = null;
                toPoint = null;
                mainCurve = null;
                polyline = null;
                pFeat = null;
            }
        }
        private static bool CreateDual(ref IApplication app, ref  IEditor editor, IFeature pointFeature, IFeature pointFeature2, ILine distanceLine,
                                                          IFeatureLayer matchLineFLayer,
                                                          IFeatureLayer targetLineFLayer, IEditTemplate targetLineEditTemplate,
                                                          List<pointAlongSettings> pointAlongLayers,
                                                          bool deleteExistingLines, bool startAtMain, bool squareDualLines,
                                                         FromToField[] fromToPairs, double doglegDistance, bool DistAsPercent, double tolerenceForDelete, bool store, bool SearchOnLayer, int searchDistance, double angle, bool checkSelection)
        {

            IPoint point = null;
            IPoint point2 = null;
            IPoint turnPoint = null;
            IPoint toPoint = null;
            IPoint joinPoint = null;
            IPoint sqPoint1 = null;
            IPoint sqPoint2 = null;
            IPoint midPoint = null;
            IFeature lineFeature = null;
            IPolyline polyline = null;
            IFeature pLineFeat = null;
            ICurve mainCurve = null;
            try
            {
                point = (IPoint)pointFeature.Shape;
                point2 = (IPoint)pointFeature2.Shape;


                //Determine mid point
                midPoint = new PointClass();
                distanceLine.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, midPoint);

                // Get closest main to midpoint (if a single main was not selected)

                lineFeature = Globals.GetClosestFeature(midPoint, matchLineFLayer, Convert.ToDouble(searchDistance), SearchOnLayer, checkSelection);


                if (lineFeature != null)
                {
                    //Delete any existing lateral lines at these locations
                    if (deleteExistingLines)
                    {
                        DeleteExistingFeatures(pointFeature, targetLineFLayer, pointAlongLayers, tolerenceForDelete);
                        DeleteExistingFeatures(pointFeature2, targetLineFLayer, pointAlongLayers, tolerenceForDelete);
                    }



                    //Determine To and Turn Points (and possible square points)
                    mainCurve = lineFeature.ShapeCopy as ICurve;
                    CreateToAndTurnPointsDual(mainCurve, midPoint, distanceLine, out toPoint, out turnPoint,
                                                            out joinPoint, out  sqPoint1, out  sqPoint2, doglegDistance, DistAsPercent,
                                                            squareDualLines, tolerenceForDelete);

                    //Create the new base line (possibly hooked)

                    polyline = Globals.CreatePolylineFromPointsNewTurn(joinPoint, turnPoint, toPoint, ref matchLineFLayer, ref lineFeature, SearchOnLayer, angle,editor.Map.SpatialReference);


                    //If requested, store pipe id in the point
                    StorePipeInfoPointFeature(lineFeature, pointFeature, fromToPairs, store);
                    StorePipeInfoPointFeature(lineFeature, pointFeature2, fromToPairs, store);

                    if (polyline.Length != 0.0)
                    {
                        if (startAtMain)
                            polyline.ReverseOrientation();


                        if (targetLineEditTemplate != null)
                        {
                            pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                        }
                        else
                        {
                            pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                        }

                        pLineFeat.Store();

                        // Globals.SetFlowDirection(pLineFeat, targetLineFLayer, ((IMxDocument)app.Document).FocusMap);
                        if (pointAlongLayers != null)
                        {
                            foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                            {
                                if (pPointAlongLayer.PolygonIntersectLayer != null)
                                    Globals.AddPointAlongLineWithIntersect(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate, pPointAlongLayer.PolygonIntersectLayer, pPointAlongLayer.PolygonIntersectSide);

                                else
                                    Globals.AddPointAlongLine(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate);

                            }
                        }

                        if (squareDualLines)
                        {
                            //Create Arm 1
                            polyline = Globals.CreatePolylineFromPoints(point, sqPoint1, joinPoint);
                            if (startAtMain)
                                polyline.ReverseOrientation();

                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }
                            // Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            //Create Arm 2
                            polyline = Globals.CreatePolylineFromPoints(point2, sqPoint2, joinPoint);
                            if (startAtMain)
                                polyline.ReverseOrientation();
                            pLineFeat.Store();


                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }

                            // Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            pLineFeat.Store();


                            return true;
                        }
                        else
                        {

                            //Create Arm 1
                            polyline = Globals.CreatePolylineFromPoints(point, joinPoint);
                            if (startAtMain)
                                polyline.ReverseOrientation();

                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }
                            // Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            //Create Arm 2
                            polyline = Globals.CreatePolylineFromPoints(point2, joinPoint);
                            if (startAtMain)
                                polyline.ReverseOrientation();
                            pLineFeat.Store();


                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }
                            //idx = 0;
                            //  Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            pLineFeat.Store();


                            return true;

                        }
                    }
                    else
                    {
                        //Create branch 1
                        polyline = Globals.CreatePolylineFromPoints(point, joinPoint);
                        if (polyline.Length != 0.0)
                        {
                            if (startAtMain)
                                polyline.ReverseOrientation();


                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }

                            //  Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            pLineFeat.Store();


                            if (pointAlongLayers != null)
                            {
                                foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                                {
                                    Globals.AddPointAlongLine(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate);
                                    //   idx++;
                                }
                            }


                            //Create branch 2
                            polyline = Globals.CreatePolylineFromPoints(point2, joinPoint);
                            if (startAtMain)
                                polyline.ReverseOrientation();


                            if (targetLineEditTemplate != null)
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
                            }
                            else
                            {
                                pLineFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
                            }

                            //  Globals.SetFlowDirection(pLineFeat, targetLineFLayer,((IMxDocument)app.Document).FocusMap);
                            pLineFeat.Store();

                            if (pointAlongLayers != null)
                            {
                                foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                                {
                                    Globals.AddPointAlongLine(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate);
                                    //   idx++;
                                }
                            }


                            return true;
                        }
                        else
                        {
                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_6"));
                            return false;

                        }
                    }
                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_7"));
                    return false;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_4") + "\n" + ex.Message, ex.Source);
                return false;
            }

            finally
            {
                point = null;
                point2 = null;
                turnPoint = null;
                toPoint = null;
                joinPoint = null;
                sqPoint1 = null;
                sqPoint2 = null;
                midPoint = null;
                lineFeature = null;
                polyline = null;
                pLineFeat = null;
                mainCurve = null;
            }
        }

        private static void StorePipeInfoPointFeature(IFeature lineFeature, IFeature pointFeature, FromToField[] fromToPairs, bool store)
        {
            try
            {


                if (fromToPairs == null)
                {

                    return;

                }

                if (fromToPairs.Length == 0)
                {

                    return;

                }
                if (pointFeature == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_8"));
                    return;

                }
                foreach (FromToField frmTo in fromToPairs)
                {
                    int pointFieldToCalc = pointFeature.Fields.FindField(frmTo.TargetField);
                    int matchFieldForCalc = lineFeature.Fields.FindField(frmTo.SourceField);
                    if (pointFieldToCalc > -1 && matchFieldForCalc > -1)
                    {
                        if (pointFeature.Fields.get_Field(pointFieldToCalc).Type == esriFieldType.esriFieldTypeString)
                            pointFeature.set_Value(pointFieldToCalc, frmTo.Prefix + lineFeature.get_Value(matchFieldForCalc).ToString());
                        else
                            pointFeature.set_Value(pointFieldToCalc, lineFeature.get_Value(matchFieldForCalc));

                    }

                }
                if (store)
                {
                    pointFeature.Store();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_9") + ex.Message);

            }
        }
        private static void CreateToAndTurnPoints(ICurve mainCurve, IPoint joinPoint, out IPoint toPoint, out IPoint turnPoint, double doglegDistance, bool distAsPercent)
        {

            IPoint closestPointOnMain = null;
            IProximityOperator lineProxOp = null;
            ILine tempLine = null;
            ICurve pCur = null;
            IPoint tempPt2 = null;
            IConstructPoint constTempPt2 = null;
            toPoint = null;
            turnPoint = null;
            try
            {

                closestPointOnMain = new PointClass();
                lineProxOp = mainCurve as IProximityOperator;
                closestPointOnMain = lineProxOp.ReturnNearestPoint(joinPoint, esriSegmentExtension.esriNoExtension);


                //Create temp line to main
                tempLine = new LineClass();
                tempLine.FromPoint = closestPointOnMain;
                tempLine.ToPoint = joinPoint;
                toPoint = closestPointOnMain;

                double dist;
                if (distAsPercent)
                {
                    pCur = (ICurve)tempLine;

                    dist = pCur.Length * (doglegDistance / 100);
                }
                else
                {
                    if (tempLine.Length < doglegDistance)
                    {
                        dist = 0;
                    }
                    else
                    {//Adjust distance if needed
                        dist = doglegDistance;
                    }

                }


                //Create the "to" point for new line - tap location on main
                //IPoint tempPt1 = new PointClass();
                //IConstructPoint constTempPt1 = (IConstructPoint)tempPt1;
                //constTempPt1.ConstructAlong(tempLine, esriSegmentExtension.esriNoExtension, dist, false);
                //  toPoint = (IPoint)constTempPt1;

                //Create dogleg (turn) point for new line if needed
                if (dist > 0)
                {
                    tempPt2 = new PointClass();
                    constTempPt2 = (IConstructPoint)tempPt2;
                    constTempPt2.ConstructAlong(tempLine, esriSegmentExtension.esriNoExtension, dist, false);
                    turnPoint = (IPoint)constTempPt2;
                }
                else
                    turnPoint = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "CreateToAndTurnPoints: " + ex.Message);
            }
            finally
            {
                closestPointOnMain = null;
                lineProxOp = null;
                tempLine = null;
                pCur = null;
                tempPt2 = null;
                constTempPt2 = null;
            }
        }
        private static void CreateToAndTurnPointsDual(ICurve mainCurve, IPoint midPoint, ILine distanceLine, out IPoint toPoint, out IPoint turnPoint, out IPoint joinPoint, out IPoint sqPoint1, out IPoint sqPoint2, double doglegDistance, bool distAsPercent, bool squareDualLines, double tolerenceForDelete)
        {

            sqPoint1 = new PointClass();
            sqPoint2 = new PointClass();
            toPoint = null;
            turnPoint = null;
            joinPoint = null;

            IPoint closestPointOnMain = null;
            IProximityOperator lineProxOp = null;
            ILine tempLine = null;
            ICurve pCur = null;
            IPoint tempPt2 = null;
            IConstructPoint constTempPt2 = null;
            ILine verticalLine = null;
            IConstructPoint conSqPoint1 = null;
            IConstructPoint conSqPoint2 = null;


            try
            {
                closestPointOnMain = new PointClass();
                lineProxOp = mainCurve as IProximityOperator;
                closestPointOnMain = lineProxOp.ReturnNearestPoint(midPoint, esriSegmentExtension.esriNoExtension);


                //Create temp line to main
                tempLine = new LineClass();
                tempLine.FromPoint = closestPointOnMain;
                tempLine.ToPoint = midPoint;
                toPoint = closestPointOnMain;

                double dist;
                if (distAsPercent)
                {
                    pCur = (ICurve)tempLine;

                    dist = pCur.Length * (doglegDistance / 100);
                }
                else
                {
                    if (tempLine.Length < doglegDistance)
                    {
                        dist = 0;
                    }
                    else
                    {//Adjust distance if needed
                        dist = doglegDistance;
                    }

                }


                //Create the "to" point for new line - tap location on main
                //IPoint tempPt1 = new PointClass();
                //IConstructPoint constTempPt1 = (IConstructPoint)tempPt1;
                //constTempPt1.ConstructAlong(tempLine, esriSegmentExtension.esriNoExtension, dist, false);
                //  toPoint = (IPoint)constTempPt1;

                //Create dogleg (turn) point for new line if needed
                if (dist > 0)
                {
                    tempPt2 = new PointClass();
                    constTempPt2 = (IConstructPoint)tempPt2;
                    constTempPt2.ConstructAlong(tempLine, esriSegmentExtension.esriNoExtension, dist, false);
                    turnPoint = (IPoint)constTempPt2;
                }
                else
                    turnPoint = null;



                //Determine Join Point
                joinPoint = new PointClass();
                verticalLine = new LineClass() as ILine;
                verticalLine.FromPoint = midPoint;
                if (turnPoint != null)
                    verticalLine.ToPoint = turnPoint;
                else
                    verticalLine.ToPoint = closestPointOnMain;
                if (verticalLine.Length > distanceLine.Length)
                    dist = distanceLine.Length / 2;
                else if (verticalLine.Length > 10)
                    dist = verticalLine.Length / 2;
                else
                    dist = verticalLine.Length / 2;
                //return;
                verticalLine.QueryPoint(esriSegmentExtension.esriNoExtension, dist, false, joinPoint);

                if (squareDualLines)
                {
                    //Create squared turn points
                    conSqPoint1 = sqPoint1 as IConstructPoint;
                    conSqPoint2 = sqPoint2 as IConstructPoint;

                    conSqPoint1.ConstructAngleIntersection(distanceLine.FromPoint, verticalLine.Angle, joinPoint, verticalLine.Angle + (Math.PI / 2));
                    conSqPoint2.ConstructAngleIntersection(distanceLine.ToPoint, verticalLine.Angle, joinPoint, verticalLine.Angle + (Math.PI / 2));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateToAndTurnPointsDual\r\n" + ex.Message);

            }
            finally
            {
                closestPointOnMain = null;
                lineProxOp = null;
                tempLine = null;
                pCur = null;
                tempPt2 = null;
                constTempPt2 = null;
                verticalLine = null;
                conSqPoint1 = null;
                conSqPoint2 = null;

            }

        }
        private static void DeleteFeaturesAtPoint(IFeatureLayer targetLineFLayer, IPoint point, IFeatureLayer targetPointFLayer, int targetLineSubtype, double tolerenceForDelete, bool searchOnLayer)
        {
            ITopologicalOperator topoOp = null;
            IPolygon poly = null;
            ISpatialFilter sFilter = null;
            INetworkClass netClass = null;
            IFeatureClass orphanFC = null;
            INetworkFeature netFeature = null;
            IFeatureCursor fCursor = null;
            IFeature feature = null;
            IPolyline line = null;
            IEdgeFeature edgeFeature = null;
            ISimpleJunctionFeature toJunctionFeature = null;
            ISimpleJunctionFeature fromJunctionFeature = null;
            ISubtypes subt = null;
            try
            {

                topoOp = point as ITopologicalOperator;
                poly = topoOp.Buffer(tolerenceForDelete) as IPolygon;
                sFilter = new SpatialFilterClass();
                sFilter.Geometry = poly;
                sFilter.GeometryField = targetLineFLayer.FeatureClass.ShapeFieldName;
                sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                netClass = targetLineFLayer.FeatureClass as INetworkClass;
                orphanFC = netClass.GeometricNetwork.OrphanJunctionFeatureClass;

                if (searchOnLayer)
                    fCursor = targetLineFLayer.Search(sFilter, false);
                else
                    fCursor = targetLineFLayer.FeatureClass.Search(sFilter, false);


                while ((feature = fCursor.NextFeature()) != null)
                {
                    //Find connecting junctions
                    netFeature = feature as INetworkFeature;
                    if (netFeature != null)
                    {
                        edgeFeature = netFeature as IEdgeFeature;
                        if (edgeFeature != null)
                        {
                            toJunctionFeature = edgeFeature.ToJunctionFeature as ISimpleJunctionFeature;
                            fromJunctionFeature = edgeFeature.ToJunctionFeature as ISimpleJunctionFeature;
                        }
                    }

                    //If subtypes are specified for the new lateral lines, delete only existing laterals with that subtype
                    subt = feature as ISubtypes;
                    line = feature.ShapeCopy as IPolyline;
                    if (targetLineSubtype > -1 && subt != null)
                    {
                        int? thisSubtype = feature.get_Value(subt.SubtypeFieldIndex) as int?;
                        if (thisSubtype != null && thisSubtype == targetLineSubtype)
                        {
                            feature.Delete();

                            DeleteTargetPoints(targetPointFLayer.FeatureClass, line, tolerenceForDelete);
                            DeleteExisitingJunction(toJunctionFeature, targetLineFLayer);
                            DeleteOrphanJunctions(orphanFC, line, tolerenceForDelete);
                        }
                    }

                    //Otherwise, just delete each feature
                    else
                    {
                        feature.Delete();
                        DeleteTargetPoints(targetPointFLayer.FeatureClass, line, tolerenceForDelete);
                        DeleteExisitingJunction(toJunctionFeature, targetLineFLayer);
                        DeleteOrphanJunctions(orphanFC, line, tolerenceForDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DeleteFeatureAtPoint\r\n" + ex.Message);

            }
            finally
            {

                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);
                topoOp = null;
                poly = null;
                sFilter = null;
                netClass = null;
                orphanFC = null;
                netFeature = null;
                fCursor = null;
                feature = null;
                line = null;
                edgeFeature = null;
                toJunctionFeature = null;
                fromJunctionFeature = null;
                subt = null;
            }

        }
        private static void DeleteExistingFeatures(IFeature pointFeature, IFeatureLayer targetLineFLayer, List<pointAlongSettings> pointAlongLayers, double tolerenceForDelete)
        {
            ISimpleJunctionFeature junc = null;
            IEdgeFeature edge = null;
            IFeature edgeFeature = null;
            ISimpleJunctionFeature oppositeJunctionFeature = null;

            INetworkClass netClass = null;
            IFeatureClass orphanFC = null;
            IPolyline line = null;

            try
            {
                //Halt delete if the selected point feature is not a simple junction feature or has no connected edges
                junc = pointFeature as ISimpleJunctionFeature;
                if (junc == null)
                    return;
                if (junc.EID == 0)
                    return;
                if (junc.EdgeFeatureCount == 0)
                    return;

                if (junc.EdgeFeatureCount != 1)
                    return;


                bool flowAway = true;
                netClass = targetLineFLayer.FeatureClass as INetworkClass;
                orphanFC = netClass.GeometricNetwork.OrphanJunctionFeatureClass;


                //Return the connected edge (with correct subtype if used)
                //Return null if more than one found
                edge = junc.get_EdgeFeature(0);

                if (edge != null)
                {
                    edgeFeature = edge as IFeature;
                    if (flowAway)
                        oppositeJunctionFeature = edge.ToJunctionFeature as ISimpleJunctionFeature;
                    else
                        oppositeJunctionFeature = edge.FromJunctionFeature as ISimpleJunctionFeature;

                    line = edgeFeature.ShapeCopy as IPolyline;
                    edgeFeature.Delete();
                    foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                    {
                        if (pPointAlongLayer.PointAlongLayer != null && pPointAlongLayer.PointAlongLayer.Valid)
                            DeleteTargetPoints(pPointAlongLayer.PointAlongLayer.FeatureClass, line, tolerenceForDelete);
                    }

                    DeleteExisitingJunction(oppositeJunctionFeature, targetLineFLayer);
                    DeleteOrphanJunctions(orphanFC, line, tolerenceForDelete);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "DeleteExistingFeatures\r\n" + ex.Message);

            }
            finally
            {
                junc = null;
                edge = null;
                edgeFeature = null;
                oppositeJunctionFeature = null;

                netClass = null;
                orphanFC = null;
                line = null;

            }
        }
        private static void DeleteExisitingJunction(ISimpleJunctionFeature juncFeature, IFeatureLayer targetLineFLayer)
        {
            IFeature thisJuction = null;
            IFeature edgeFeat = null;
            IEdgeFeature edge = null;
            IFeature toJuncFeat = null;
            ISimpleJunctionFeature toJunc = null;

            try
            {
                if (juncFeature != null)
                {
                    if (juncFeature.EID > 0)
                    {
                        thisJuction = juncFeature as IFeature;

                        //If this junction connects to one lateral line delete the junction and the line
                        if (juncFeature.EdgeFeatureCount == 1)
                        {
                            edgeFeat = juncFeature.get_EdgeFeature(0) as IFeature;
                            edge = edgeFeat as IEdgeFeature;
                            toJuncFeat = edge.ToJunctionFeature as IFeature;
                            toJunc = toJuncFeat as ISimpleJunctionFeature;

                            if (edgeFeat.Class.ObjectClassID == targetLineFLayer.FeatureClass.FeatureClassID)
                                edgeFeat.Delete();

                            if (DeleteNeeded(toJunc))
                                toJuncFeat.Delete();
                        }

                        //If this junction connects two edges of the same line, delete the junction
                        if (DeleteNeeded(juncFeature))
                            thisJuction.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "DeleteExisitingJunction: " + ex.Message);
            }
            finally
            {
                thisJuction = null;
                edgeFeat = null;
                edge = null;
                toJuncFeat = null;
                toJunc = null;
            }

        }
        private static bool DeleteNeeded(ISimpleJunctionFeature juncFeature)
        {
            IFeature feat1 = null;
            IFeature feat2 = null;
            try
            {
                if (juncFeature.EdgeFeatureCount == 2)
                {
                    feat1 = juncFeature.get_EdgeFeature(0) as IFeature;
                    feat2 = juncFeature.get_EdgeFeature(1) as IFeature;
                    if (feat1.OID == feat2.OID && feat1.Class.ObjectClassID == feat2.Class.ObjectClassID)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "DeleteNeeded: " + ex.Message);
                return false;

            }
            finally
            {
                feat1 = null;
                feat2 = null;

            }

        }
        private static void DeleteTargetPoints(IFeatureClass targetFC, IPolyline line, double tolerenceForDelete)
        {
            ITopologicalOperator topoOpLine = null;
            IPolygon poly2 = null;
            ISpatialFilter sFilter2 = null;
            IFeatureCursor fCursor2 = null;
            IFeature feature2 = null;
            try
            {
                if (targetFC != null && line != null)
                {
                    //Buffer the line 
                    topoOpLine = line as ITopologicalOperator;
                    poly2 = topoOpLine.Buffer(tolerenceForDelete) as IPolygon;
                    sFilter2 = new SpatialFilterClass();
                    sFilter2.Geometry = poly2;
                    sFilter2.GeometryField = targetFC.ShapeFieldName;
                    sFilter2.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    fCursor2 = targetFC.Search(sFilter2, false);

                    while ((feature2 = fCursor2.NextFeature()) != null)
                    {
                        feature2.Delete();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "DeleteTargetPoints: " + ex.Message);

            }
            finally
            {
                if (fCursor2 != null)
                    Marshal.ReleaseComObject(fCursor2);
                topoOpLine = null;
                poly2 = null;
                sFilter2 = null;
                fCursor2 = null;
                feature2 = null;
            }
        }
        private static void DeleteOrphanJunctions(IFeatureClass orphanFC, ICurve curve, double tolerenceForDelete)
        {
            ITopologicalOperator topoOpLine = null;
            IPolygon poly2 = null;
            ISpatialFilter sFilter2 = null;
            IFeatureCursor fCursor2 = null;
            IFeature feature2 = null;
            ISimpleJunctionFeature juncfeat = null;
            IRow row1 = null;
            IRow row2 = null;
            try
            {
                if (orphanFC != null && curve != null)
                {
                    //Buffer the line 
                    topoOpLine = curve as ITopologicalOperator;
                    poly2 = topoOpLine.Buffer(tolerenceForDelete) as IPolygon;
                    sFilter2 = new SpatialFilterClass();
                    sFilter2.Geometry = poly2;
                    sFilter2.GeometryField = orphanFC.ShapeFieldName;
                    sFilter2.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    fCursor2 = orphanFC.Search(sFilter2, false);

                    while ((feature2 = fCursor2.NextFeature()) != null)
                    {
                        juncfeat = feature2 as ISimpleJunctionFeature;
                        if (juncfeat != null)
                        {
                            if (juncfeat.EdgeFeatureCount == 0)
                                feature2.Delete();
                            else if (juncfeat.EdgeFeatureCount == 2)
                            {
                                row1 = juncfeat.get_EdgeFeature(0) as IRow;
                                row2 = juncfeat.get_EdgeFeature(1) as IRow;
                                if (row1.OID == row2.OID)
                                    feature2.Delete();
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "DeleteOrphanJunctions: " + ex.Message);
            }
            finally
            {
                if (fCursor2 != null)
                    Marshal.ReleaseComObject(fCursor2);
                topoOpLine = null;
                poly2 = null;
                sFilter2 = null;
                fCursor2 = null;
                feature2 = null;
                juncfeat = null;
                row1 = null;
                row2 = null;

            }
        }

        #endregion
    }

    public static class AddLateralsFromPoint
    {

        public static string createTapPoints(IApplication app, List<TapPointDetails> pointDetailsList)
        {
            IWorkspace pWS = null;
            IMap map = null;
            ITable pTable = null;
            IFields pFields = null;
            IFeatureClass pPointFC = null;
            IFeatureLayer pFT = null;
            ICursor pCurs = null;
            IRow pRow = null;
            IFeature pFeat = null;
            IFeatureLayer pMains = null;
            IQueryFilter pQFilt = null;
            IFeatureCursor pMainsCurs = null;
            IPoint pPntOnLine = null;
            IFeatureBuffer pFBuf = null;
            IFeatureCursor pntInsertCurs = null;

            try
            {
                foreach (TapPointDetails pointDetails in pointDetailsList)
                {


                    bool bFndType = false;

                    map = (app.Document as IMxDocument).FocusMap;

                    pMains = (IFeatureLayer)(Globals.FindLayer(app, pointDetails.MainLayerName, ref  bFndType));
                    if (pMains == null)
                        return "";

                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }

                    pTable = Globals.FindTable(app, pointDetails.TapPointTableName);
                    int idxPntMainIDFld = Globals.GetFieldIndex(pTable.Fields, pointDetails.MainIDField);
                    int idxPntDistFld = Globals.GetFieldIndex(pTable.Fields, pointDetails.DistanceField);
                    int idxPntDirFld = Globals.GetFieldIndex(pTable.Fields, pointDetails.DistanceField);
                    int idxMainLayerIDFld = Globals.GetFieldIndex(pTable.Fields, pointDetails.MainLayerIDField);

                    if (idxPntDistFld == -1 || idxPntMainIDFld == -1 || idxMainLayerIDFld == -1 || idxPntDirFld == -1)
                        return "";

                    pFields = Globals.createFeatureClassFieldsFromTableFields(pTable.Fields, map.SpatialReference, esriGeometryType.esriGeometryPoint);
                    pPointFC = Globals.createFeatureClassInMemory(pointDetails.TapPointResultLayerName, pFields, pWS, esriFeatureType.esriFTSimple);

                    pCurs = pTable.Search(null, true);


                    pntInsertCurs = pPointFC.Insert(true);

                    string strDist, strDir;
                    bool rev = false;
                    while ((pRow = pCurs.NextRow()) != null)
                    {
                        pQFilt = Globals.createQueryFilter();
                        if (pMains.FeatureClass.Fields.get_Field(idxMainLayerIDFld).Type == esriFieldType.esriFieldTypeString)
                        {
                            pQFilt.WhereClause = pointDetails.MainLayerIDField + " = '" + pRow.get_Value(idxPntMainIDFld) + "'";
                        }
                        else
                        {
                            pQFilt.WhereClause = pointDetails.MainLayerIDField + " = " + pRow.get_Value(idxPntMainIDFld) + "";
                        }
                        pMainsCurs = pMains.Search(pQFilt, true);

                        while ((pFeat = pMainsCurs.NextFeature()) != null)
                        {
                            strDist = pRow.get_Value(idxPntDistFld).ToString();
                            strDir = pRow.get_Value(idxPntDistFld).ToString();
                            if (Globals.IsNumeric(strDist))
                            {

                                if (pRow.get_Value(idxPntDirFld).ToString() == pointDetails.LookingUpstreamValue)
                                {
                                    rev = true;
                                }
                                else
                                    rev = false;

                                pPntOnLine = Globals.CreatePointFromDistanceOnLine(Convert.ToDouble(strDist), (IPolyline)pFeat.Shape, rev);
                                if (pPntOnLine != null)
                                {
                                    pFBuf = pPointFC.CreateFeatureBuffer();
                                    pFeat = (IFeature)pFBuf;
                                    pFeat.Shape = pPntOnLine;
                                    for (int i = 0; i < pTable.Fields.FieldCount - 1; i++)
                                    {
                                        int fldCopyIDX = Globals.GetFieldIndex(pPointFC.Fields, pTable.Fields.get_Field(i).Name);
                                        if (fldCopyIDX > -1)
                                        {
                                            try
                                            {
                                                pFeat.set_Value(fldCopyIDX, pRow.get_Value(i));
                                            }
                                            catch
                                            { }

                                        }
                                    }
                                    pntInsertCurs.InsertFeature(pFBuf);

                                }
                            }
                        }

                    }



                    pFT = new FeatureLayerClass();
                    pFT.FeatureClass = pPointFC;
                    pFT.Name = pointDetails.TapPointResultLayerName;

                    map.AddLayer(pFT);
                    return "";
                }
                return "";
            }
            catch
            { 
                return ""; 
            }
            finally
            {
            }

        }
        private static string _caption = A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_6");

        public static string AddLateralsFromMainPoint(IApplication app, List<AddLateralFromMainPointDetails> addLateralsDetails, IFeature inFeatures, bool logOperation, bool suppressDialog, bool store)
        {

            string resetFlow = "";
            bool useDefaultTemplate;
            List<IFeature> ComplFeat = new List<IFeature>();
            IMap map = null;
            IEditor editor = null;
            IMouseCursor appCursor = null;
            IMxDocument mxdoc = null;
            IFeatureLayer pointFLayer = null;
            IFeatureLayer matchLineFLayer = null;
            IFeatureLayer targetLineFLayer = null;
            IEditLayers eLayers = null;
            ISelectionSet2 pointSelSet = null;
            IFeatureSelection pointFeatureSelection = null;
            IEditTemplate pLateralLineEditTemp = null;
            List<pointAlongSettings> pointAlongLayers = null;
            pointAlongSettings pointAlongLayer = null;
            ICursor pointCursor = null;
            IFeature pointFeature = null;

            //ProgressBar
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;


            try
            {

                if (Control.ModifierKeys == Keys.Control)
                {
                    useDefaultTemplate = false;
                }
                else
                {

                    useDefaultTemplate = true;
                }
                bool boolSelectedEdges = false;
                if (Control.ModifierKeys == Keys.Shift)
                {
                    boolSelectedEdges = true;
                }
                else
                {

                    boolSelectedEdges = false;
                }
                //Get edit session
                bool LatCreated = false;

                editor = Globals.getEditor(app);
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), _caption);
                    editor = null;

                    return "";
                }

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);


                mxdoc = (IMxDocument)app.Document;
                map = editor.Map;

                for (int k = 0; k < addLateralsDetails.Count; k++)
                {
                    bool FCorLayerPoint = true;
                    pointFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].Point_LayerName, ref FCorLayerPoint);
                    if (inFeatures != null)
                    {
                        if (pointFLayer == null)
                            continue;
                        if (pointFLayer.FeatureClass == null)
                            continue;

                        if (inFeatures.Class.CLSID.ToString() != pointFLayer.FeatureClass.CLSID.ToString())
                            continue;

                    }
                    //Report any problems before exiting
                    if (pointFLayer == null)
                    {
                        continue;
                    }

                    bool FCorLayerMatch = true;
                    bool FCorLayerTarget = true;

                    matchLineFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].MainLine_LayerName, ref FCorLayerMatch);
                    targetLineFLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].LateralLine_LayerName, ref FCorLayerTarget);

                    // IFeatureLayerDefinition2 pFeatLayerdef = matchLineFLayer as IFeatureLayerDefinition2;

                    if (matchLineFLayer == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_1") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (matchLineFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_1") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (matchLineFLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_2") + "'" + addLateralsDetails[k].MainLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_3") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer.FeatureClass == null)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_3") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }
                    if (targetLineFLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsMess_4") + "'" + addLateralsDetails[k].LateralLine_LayerName + "'.", _caption);
                        return "";
                    }


                    //Confirm the other layers are the correct shape type
                    if (pointFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint || matchLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        return "";


                    //Confirm that target layer is editable and is a line layer
                    eLayers = (IEditLayers)editor;
                    if (!(eLayers.IsEditable(targetLineFLayer)) || (targetLineFLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline))
                        return "";

                    //Confirm that the two line layers are different Feature classes
                    if ((matchLineFLayer.FeatureClass.CLSID == targetLineFLayer.FeatureClass.CLSID) && (matchLineFLayer.FeatureClass.AliasName == targetLineFLayer.FeatureClass.AliasName))
                    {
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_1") , A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_2") );
                        return "";
                    }


                    //Verify that some points are selected
                    pointFeatureSelection = (IFeatureSelection)pointFLayer;

                    if (pointFeatureSelection.SelectionSet.Count == 0)
                        continue;

                    pointSelSet = pointFeatureSelection.SelectionSet as ISelectionSet2;





                    if (useDefaultTemplate)
                    {
                        //pLateralLineEditTemp = Globals.PromptAndGetEditTemplate(app, targetLineFLayer, addLateralsDetails[k].LateralLine_EditTemplate);
                        pLateralLineEditTemp = Globals.PromptAndGetEditTemplateGraphic(targetLineFLayer, addLateralsDetails[k].LateralLine_EditTemplate);
                    }
                    else
                    {
                        pLateralLineEditTemp = Globals.PromptAndGetEditTemplateGraphic(targetLineFLayer, "");
                        //pLateralLineEditTemp = Globals.PromptAndGetEditTemplate(app, targetLineFLayer, "");
                    }







                    if (addLateralsDetails[k].PointAlong != null)
                    {


                        if (addLateralsDetails[k].PointAlong.Length > 0)
                        {
                            pointAlongLayers = new List<pointAlongSettings>();


                            // IEditTemplate pPointAlongEditTemp;
                            for (int j = 0; j < addLateralsDetails[k].PointAlong.Length; j++)
                            {
                                pointAlongLayer = new pointAlongSettings();
                                bool FCorLayerPointsAlong = true;
                                pointAlongLayer.PointAlongLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].PointAlong[j].LayerName, ref FCorLayerPointsAlong);
                                if (pointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";

                                }
                                else if (pointAlongLayer.PointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";


                                }
                                else if (pointAlongLayer.PointAlongLayer.FeatureClass == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_2") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";
                                }
                                else if (pointAlongLayer.PointAlongLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint)
                                {
                                    MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_3") , A4LGSharedFunctions.Localizer.GetString("Warning") );

                                    return "";
                                }

                                pointAlongLayer.PolygonIntersectSide = addLateralsDetails[k].PointAlong[j].PolygonOffsetSide;
                                if (pointAlongLayer.PolygonIntersectSide == null)
                                {
                                    pointAlongLayer.PolygonIntersectSide = "TO";

                                }
                                bool FCorLayerTemp = true;
                                pointAlongLayer.PolygonIntersectLayer = (IFeatureLayer)Globals.FindLayer(map, addLateralsDetails[k].PointAlong[j].PolygonOffsetLayerName, ref FCorLayerTemp);
                                pointAlongLayer.FoundAsLayer = FCorLayerTemp;
                                if (pointAlongLayer == null)
                                {
                                    if (MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsAsk_1") , A4LGSharedFunctions.Localizer.GetString("Warning") , MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                        return "";

                                }
                                else if (pointAlongLayer.PolygonIntersectLayer != null)
                                {

                                    if (pointAlongLayer.PolygonIntersectLayer.FeatureClass != null)
                                    {

                                        //Confirm that target layer is editable and is a line layer
                                        if (pointAlongLayer.PolygonIntersectLayer != null)
                                        {
                                            if (pointAlongLayer.PolygonIntersectLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                                            {
                                                MessageBox.Show(addLateralsDetails[k].PointAlong[j].PolygonOffsetLayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_3"));


                                                return "";
                                            }

                                        }
                                    }
                                }
                                //Confirm that target layer is editable and is a line layer
                                if (pointAlongLayer.PointAlongLayer != null)
                                {
                                    if (!(eLayers.IsEditable(pointAlongLayer.PointAlongLayer)) || (pointAlongLayer.PointAlongLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint))
                                    {
                                        MessageBox.Show(addLateralsDetails[k].PointAlong[j].LayerName + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsError_4"));


                                        return "";
                                    }
                                    if (useDefaultTemplate)
                                    {
                                        pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplateGraphic(pointAlongLayer.PointAlongLayer, addLateralsDetails[k].PointAlong[j].EditTemplate);
                                        //pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplate(app, pointAlongLayer.PointAlongLayer, addLateralsDetails[k].PointAlong[j].EditTemplate);
                                    }
                                    else
                                    {
                                        pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplateGraphic(pointAlongLayer.PointAlongLayer, "");
                                        //pointAlongLayer.PointAlongEditTemplate = Globals.PromptAndGetEditTemplate(app, pointAlongLayer.PointAlongLayer, "");
                                    }

                                }


                                //if (addLateralsDetails[k].PointAlong[j].Distance < 0)
                                //    pointAlongLayer.PointAlongDistance = 0;
                                //else
                                pointAlongLayer.PointAlongDistance = (double)addLateralsDetails[k].PointAlong[j].Distance;


                                //if (addLateralsDetails[k].PointAlong[j].DistanceIsPercent != null)
                                pointAlongLayer.DistanceIsPercent = (bool)addLateralsDetails[k].PointAlong[j].DistanceIsPercent;
                                //else
                                //  pointAlongLayer.DistanceIsPercent =false;







                                pointAlongLayers.Add(pointAlongLayer);




                            }
                        }
                    }
                    //****************************************

                    int total;

                    total = pointSelSet.Count;

                    int i = 0;

                    // Create a CancelTracker
                    trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                    // Set the properties of the Step Progressor
                    System.Int32 int32_hWnd = app.hWnd;
                    if (suppressDialog == false)
                    {
                        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
                        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);

                        stepProgressor.MinRange = 0;
                        stepProgressor.MaxRange = total;
                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = _caption;
                        // Create the ProgressDialog. This automatically displays the dialog
                        progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog.CancelEnabled = true;
                        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("AddLine") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
                        progressDialog.Title = _caption;
                        progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;
                        progressDialog.ShowDialog();

                    }
                    // Create an edit operation enabling undo/redo

                    if (logOperation)
                    {
                        try
                        {
                            editor.StartOperation();
                        }
                        catch
                        {
                            logOperation = false;
                        }

                    }

                    //IPoint fromPoint = null;
                    //IPoint selPt1 = null;
                    //IPoint selPt2 = null;
                    //ILine distanceLine = null;
                    //object Missing = null;
                    //IEnvelope env = null;
                    //IEnumIDs selIds = null;
                    //IFeature pointFeature2 = null;
                    List<int> completedOIDArrayList = null;
                    //ITopologicalOperator topoOp = null;
                    //IPolygon poly = null;
                    //ISpatialFilter sFilter = null;
                    IFeatureCursor lineCursor = null;
                    INetworkFeature pNF = null;
                    IFeature testPointFeature = null;

                    int featOID1, featOID2, nearbyCount;

                    try
                    {

                        // ISelectionSet2 sel = pointSelSet as ISelectionSet2;
                        pointSelSet.Update(null, false, out pointCursor);
                        completedOIDArrayList = new List<int>();

                        while ((pointFeature = (IFeature)pointCursor.NextRow()) != null)
                        {
                            try
                            {

                                i += 1;
                                if (suppressDialog == false)
                                {
                                    //Update progress bar
                                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("AddLine") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + "." + Environment.NewLine +
                                      A4LGSharedFunctions.Localizer.GetString("CurrentOID") + pointFeature.OID;
                                    stepProgressor.Step();
                                }
                                ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                                statusBar.set_Message(0, i.ToString());

                                //Check if the cancel button was pressed. If so, stop process
                                bool boolean_Continue = trackCancel.Continue();
                                if (!boolean_Continue)
                                {
                                    break;
                                }

                                CreateLateralFromMainPoint(ref app, ref editor, pointFeature, matchLineFLayer, targetLineFLayer, pLateralLineEditTemp, pointAlongLayers, addLateralsDetails[k].LateralLine_StartAtMain,
                                    addLateralsDetails[k].FromToFields, addLateralsDetails[k].LateralLine_AngleDetails, addLateralsDetails[k].SearchOnLayer, boolSelectedEdges);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);

                            }
                            finally
                            {

                            }
                            //   addLateralsDetails[k].InitDefaults();
                            if (addLateralsDetails[k].Reset_Flow != null)
                            {
                                resetFlow = addLateralsDetails[k].Reset_Flow;

                            }
                        }

                        //if (ForceSourcePointConnection)
                        //{
                        //    foreach (IFeature sourcePnt in ComplFeat)
                        //    {
                        //        if (sourcePnt != null)
                        //        {
                        //            if (sourcePnt.Shape.IsEmpty != true)
                        //            {
                        //                if (sourcePnt is INetworkFeature)
                        //                {
                        //                    pNF = (INetworkFeature)sourcePnt;
                        //                    try
                        //                    {
                        //                        pNF.Connect();
                        //                    }
                        //                    catch
                        //                    { }
                        //                }
                        //            }
                        //        }
                        //    }


                        // }



                    }
                    catch (Exception ex)
                    {
                        editor.AbortOperation();
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);


                    }
                    finally
                    {
                        // Cleanup
                        if (progressDialog != null)
                            progressDialog.HideDialog();
                        if (lineCursor != null)
                            Marshal.ReleaseComObject(lineCursor);

                        pNF = null;
                        //fromPoint = null;
                        //selPt1 = null;
                        //selPt2 = null;
                        //distanceLine = null;
                        //Missing = null;
                        //env = null;
                        //selIds = null;
                        //pointFeature2 = null;
                        completedOIDArrayList = null;
                        //topoOp = null;
                        //poly = null;
                        //sFilter = null;
                        lineCursor = null;
                        testPointFeature = null;
                    }



                    if (logOperation)
                    {
                        try
                        {
                            // Stop the edit operation 
                            editor.StopOperation(_caption);

                        }
                        catch
                        {
                            logOperation = false;
                        }

                    }


                    //88
                }




                return resetFlow;

            }

            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ConstructionToolsLbl_2") + "\n" + ex.Message, ex.Source);
                return "";
            }
            finally
            {
                ComplFeat.Clear();
                if (map != null)
                {
                    (map as IActiveView).Refresh();
                }
                if (progressDialog != null)
                    progressDialog.HideDialog();

                ComplFeat = null;
                map = null;
                editor = null;
                appCursor = null;
                mxdoc = null;
                pointFLayer = null;
                matchLineFLayer = null;
                targetLineFLayer = null;
                eLayers = null;
                pointSelSet = null;
                pointFeatureSelection = null;
                pLateralLineEditTemp = null;
                pointAlongLayers = null;
                pointAlongLayer = null;
                pointCursor = null;
                pointFeature = null;

                //ProgressBar
                progressDialogFactory = null;
                stepProgressor = null;
                progressDialog = null;
                // Create a CancelTracker
                trackCancel = null;


            }


        }

        private static bool CreateLateralFromMainPoint(ref IApplication app, ref  IEditor editor, IFeature pointFeature,
                                                     IFeatureLayer mainLineFLayer, IFeatureLayer targetLineFLayer, IEditTemplate targetLineEditTemplate,
                                                     List<pointAlongSettings> pointAlongLayers, bool startAtMain, FromToField[] fromToPairs, LateralLine_AngleDetails latDet, bool SearchOnLayer, bool CheckSelection)
        {

            List<IFeature> pointsAlong = new List<IFeature>();

            //IGeometry pGeometry;
            IPolyline polyline;

            //IPoint pToPoint;
            //IPoint pFromPoint;
            //IConstructPoint pConstructPoint;

            UID pId = new UID();

            IFeature pFeat;
            // double pi;
            double dblDegrees;
            double dblAngleRad;
            double dblLateralLength;


            //Find LineAngle field in the layer
            //iLayerLineAngleFieldPos = pFC.FindField(c_sLineAngleFieldName)
            //If iLayerLineAngleFieldPos < 0 Then
            //  MsgBox c_sLineAngleFieldName & " was not found in the highlighted layer.", vbCritical, c_sTitle
            //  Exit Sub
            //End If


            if (targetLineFLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
            { //MsgBox "Edit target must be a polyline layer (i.e. laterals).", vbCritical, c_sTitle
                return false;
            }



            dblDegrees = 90;
            if (!Globals.IsNumeric(latDet.AngleField))
            {
                int fldIdx = Globals.GetFieldIndex(pointFeature.Class.Fields, latDet.AngleField);

                if (fldIdx > -1)
                {

                    string temp = pointFeature.get_Value(fldIdx).ToString();
                    if (Globals.IsNumeric(temp))
                    {
                        Double.TryParse(temp, out dblDegrees);
                    }
                }

            }
            else
            {
                Double.TryParse(latDet.AngleField, out dblDegrees);
            }


            dblLateralLength = 10;
            if (!Globals.IsNumeric(latDet.LengthField))
            {
                int fldIdx = Globals.GetFieldIndex(pointFeature.Class.Fields, latDet.LengthField);

                if (fldIdx > -1)
                {

                    string temp = pointFeature.get_Value(fldIdx).ToString();
                    if (Globals.IsNumeric(temp))
                    {
                        Double.TryParse(temp, out dblLateralLength);
                    }
                }

            }
            else
            {
                Double.TryParse(latDet.LengthField, out dblLateralLength);
            }

            if (latDet.AngleType.ToUpper() == "CLOCK")
            {
                dblAngleRad = Globals.ConvertDegToRads(Globals.ConvertClockPositionToDegrees(dblDegrees));
            }
            else if (latDet.AngleType.ToUpper() == "RADIANS")
                dblAngleRad = Globals.ConvertDegToRads(dblDegrees);
            else
                dblAngleRad = dblDegrees;


            string dirVal = latDet.DirectionField;
            if (dirVal != "")
            {
                int fldIdx = Globals.GetFieldIndex(pointFeature.Class.Fields, dirVal);

                if (fldIdx > -1)
                {

                    dirVal = pointFeature.get_Value(fldIdx).ToString();




                }

            }
            else
            {
                dirVal = "";
            }

            if (dirVal.ToUpper() == latDet.LookingUpstreamValue.ToUpper())
            {
                dblAngleRad = Globals.ConvertDegToRads(Globals.ConvertRadsToDegrees(dblAngleRad) + 180);
            }
            else if (dirVal == latDet.LookingDownstreamValue)
            {

            }
            else
            {

            }

            if (latDet.OnlyPerp.ToUpper() == "TRUE")
            {
                double val = Globals.ConvertRadsToDegrees(dblAngleRad);
                if (val >= 0.0 && val <= 180.0)
                {
                    dblAngleRad = Globals.ConvertDegToRads(90);
                }
                else
                {
                    dblAngleRad = Globals.ConvertDegToRads(270);

                }
            }
            polyline = Globals.CreateAngledLineFromLocationOnLine((IPoint)pointFeature.Shape, mainLineFLayer, SearchOnLayer, dblAngleRad, dblLateralLength, latDet.AddAngleToLineAngle, startAtMain, CheckSelection);
            //Add that line to Laterals lateral
            if (targetLineEditTemplate != null)
            {
                pFeat = Globals.CreateFeature(polyline as IGeometry, targetLineEditTemplate, editor, app, false, false, true);
            }
            else
            {
                pFeat = Globals.CreateFeature(polyline as IGeometry, targetLineFLayer, editor, app, false, false, true);
            }
            pFeat.Store();
            if (pointAlongLayers != null)
            {
                foreach (pointAlongSettings pPointAlongLayer in pointAlongLayers)
                {
                    if (pPointAlongLayer.PolygonIntersectLayer != null)
                        pointsAlong.Add(Globals.AddPointAlongLineWithIntersect(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate, pPointAlongLayer.PolygonIntersectLayer, pPointAlongLayer.PolygonIntersectSide));

                    else
                        pointsAlong.Add(Globals.AddPointAlongLine(ref app, ref editor, polyline as ICurve, pPointAlongLayer.PointAlongLayer, pPointAlongLayer.PointAlongDistance, pPointAlongLayer.DistanceIsPercent, pPointAlongLayer.PointAlongEditTemplate));


                    //   idx++;
                }
            }







            if (pFeat is INetworkFeature)
            {
                INetworkFeature pNF = (INetworkFeature)pFeat;
                pNF.CreateNetworkElements();
                //pNF.Connect();


            }

            return true;



        }
    }




    public class returnFeatArray
    {

        public List<IFeature> Features
        {
            get;
            set;
        }
        public string Options
        {
            get;
            set;
        }


    }
}
