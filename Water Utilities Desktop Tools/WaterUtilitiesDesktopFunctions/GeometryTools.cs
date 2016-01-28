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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

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


namespace A4WaterUtilities
{
    public static class GeometryTools
    {
        public enum JumpTypes { Over, Under }
        public enum FlipTypes { FlipLines, FlipLinesToMatchFlow }



        public static void SetMeasures(IApplication app)
        {
            IEditor editor = null;
            ICursor pCursor = null;
            IFeatureCursor pFCursor = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            IMap map = null;
            IMxDocument mxdoc = null;
            ILayer layer = null;
            IFeatureLayer fLayer = null;
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;


            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;

            IFeature pFeature = null;
            IMSegmentation pMSeg = null;
            ICurve pCurve = null;
            try
            {
                editor = Globals.getEditor(app);
                if (editor == null)
                {
                    return;
                }

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_1"));
                    return;
                }
                mxdoc = app.Document as IMxDocument;
                layer = mxdoc.SelectedLayer as ILayer;
                // Verify that there are layers in the table on contents
                map = mxdoc.FocusMap;
                if (map.LayerCount < 1)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_1"));
                    return;
                }



                if (layer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_6") + Environment.NewLine +
                                     A4LGSharedFunctions.Localizer.GetString("Measure_1"));
                    return;
                }

                //Verify that it is a feature layer
                fLayer = layer as IFeatureLayer;
                if (fLayer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_1"));
                    return;
                }

                //Get the Feature layer and feature class
                fc = fLayer.FeatureClass;
                if (fc == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_2"));
                    return;

                }
                if (fc.Fields.get_Field(fc.Fields.FindField(fc.ShapeFieldName)).GeometryDef.HasM == false)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_3"));
                    return;

                }
                fSel = fLayer as IFeatureSelection;

                //Verify that it is a line layer
                if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_2"));
                    return;
                }
                //Verify Layer Selection
                if (fSel.SelectionSet.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_4"));
                    return;
                }

                bool WithDig = true;
                if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsAsk_1"), A4LGSharedFunctions.Localizer.GetString("CalibrateLn"), MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    WithDig = false;
                }

                //Start edit operation (for undo)
                editor.StartOperation();
                //ProgressBar
                progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                // Create a CancelTracker
                trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = app.hWnd;
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                stepProgressor.MinRange = 0;
                stepProgressor.MaxRange = fSel.SelectionSet.Count;
                stepProgressor.StepValue = 1;
                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_1") + A4LGSharedFunctions.Localizer.GetString("ForLn") + 1 + A4LGSharedFunctions.Localizer.GetString("Of") + fSel.SelectionSet.Count.ToString() + ".";

                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = true;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_1");
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_1") + A4LGSharedFunctions.Localizer.GetString("ForSltedLn");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;
                //Step through each selected line in the highlighted layer
                fSel.SelectionSet.Search(null, false, out pCursor);
                pFCursor = (IFeatureCursor)pCursor;


                pFeature = pFCursor.NextFeature();
                int intCount = 1;
                //'Loop through each selected feature in the highlighted layer

                while ((pFeature != null))
                {

                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_1") + A4LGSharedFunctions.Localizer.GetString("ForLn") + intCount + A4LGSharedFunctions.Localizer.GetString("Of") + fSel.SelectionSet.Count.ToString() + ".";
                    if (pFeature.Shape is IMSegmentation)
                    {
                        try
                        {
                            if (pFeature.Shape != null)
                            {
                                pMSeg = (IMSegmentation)pFeature.ShapeCopy;
                                pCurve = (ICurve)pFeature.ShapeCopy;
                                if (WithDig)
                                {
                                    pMSeg.SetAndInterpolateMsBetween(0, pCurve.Length);
                                }
                                else
                                    pMSeg.SetAndInterpolateMsBetween(pCurve.Length, 0);

                                pFeature.Shape = (IGeometry)pMSeg;
                                pFeature.Store();
                            }
                            else
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("OID") + pFeature.OID + A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_5"));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_1") + "\n" + ex.Message + A4LGSharedFunctions.Localizer.GetString("OID") + pFeature.OID);
                        }
                    }

                    Marshal.ReleaseComObject(pFeature);
                    intCount++;
                    pFeature = pFCursor.NextFeature();
                    stepProgressor.Step();

                }

                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_2"));
            }
            catch (Exception ex)
            {
                editor.AbortOperation();
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_1") + "\n" + ex.Message, ex.Source);
                return;
            }
            finally
            {
                if (progressDialog != null)
                {
                    progressDialog.HideDialog();
                }
                if (pCursor != null)
                    Marshal.ReleaseComObject(pCursor);
                if (pFCursor != null)
                    Marshal.ReleaseComObject(pFCursor);

                editor = null;
                pCursor = null;
                pFCursor = null;
                stepProgressor = null;
                progressDialog = null;
                map = null;
                mxdoc = null;
                layer = null;
                fLayer = null;
                fc = null;
                fSel = null;
                progressDialogFactory = null;
                trackCancel = null;

                pFeature = null;
                pMSeg = null;
                pCurve = null;
            }
        }
        public static void AddRotate(IApplication app, string AddRotateSuspendAA, double addSpinAngle)
        {
            ESRI.ArcGIS.Framework.ICommandItem pCmd = null;
            IEditor editor = null;
            IEditLayers eLayers = null;
            IMap map = null;
            UID geoFeatureLayerID = null;
            IEnumLayer enumLayer = null;
            IFeatureLayer fLayer = null;
            IGeoFeatureLayer geoFLayer = null;
            IRotationRenderer rRenderer = null;
            IFeatureSelection fSel = null;
            ILayer layer = null;
            string rotationFieldName;
            int rotationFieldPos;
            int count;
            double angle;
            IActiveView activeView = null;
            IInvalidArea invalid = null;
            IEnvelope ext = null;
            int i = 0;
            ICursor pointCursor = null;
            IFeature pointFeature = null;
            ISelectionSet2 sel = null;
            //ProgressBar
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;

            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            try
            {

                if (AddRotateSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                }

                int maxIndividualRefresh = 100;
                editor = Globals.getEditor(app);
                map = editor.Map;

                //Get list of editable layers
                // IEditor editor = _editor;
                eLayers = (IEditLayers)editor;
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"));
                    return;
                }


                //Get list of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                if (map.SelectionCount > 0)
                {
                    int total = map.SelectionCount;
                    progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                    // Create a CancelTracker
                    trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                    // Set the properties of the Step Progressor
                    System.Int32 int32_hWnd = app.hWnd;
                    stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = total;
                    stepProgressor.StepValue = 1;
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_1");

                    // Create the ProgressDialog. This automatically displays the dialog

                    progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                    // Set the properties of the ProgressDialog
                    progressDialog.CancelEnabled = true;
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_3") + total.ToString() + ".";
                    progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_2");
                    progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

                    // Prep rotation calculator
                    // RotationCalculator rc = new RotationCalculator(_app);

                    //Prep screen refresh for selected features
                    activeView = map as IActiveView;
                    invalid = new InvalidAreaClass();
                    invalid.Display = editor.Display;



                    // Step through each geofeature layer in the map
                    enumLayer.Reset();
                    layer = enumLayer.Next();
                    bool test = false;
                    while (!(layer == null))
                    {
                        // Verify that this is a valid, visible point layer and that this layer is editable
                        fLayer = (IFeatureLayer)layer;
                        if (fLayer.Valid && fLayer.Visible && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) &&
                            eLayers.IsEditable(fLayer))
                        {
                            // Verify that a selection  for this layer
                            fSel = (IFeatureSelection)fLayer;
                            count = fSel.SelectionSet.Count;

                            if (count > 0)
                            {
                                // Verify that symbol rotation has been setup for this layer
                                geoFLayer = (IGeoFeatureLayer)fLayer;
                                if (!(geoFLayer.Renderer is IRotationRenderer))
                                {
                                    layer = enumLayer.Next();
                                    continue;
                                }


                                // Verify that the rotation field has been specified and that the field exists
                                rRenderer = (IRotationRenderer)geoFLayer.Renderer;
                                rotationFieldName = rRenderer.RotationField;
                                rotationFieldPos = fLayer.FeatureClass.FindField(rotationFieldName);
                                if (rotationFieldPos == -1)
                                {
                                    layer = enumLayer.Next();
                                    continue;
                                }
                                test = true;

                                //rc.RotationType = rRenderer.RotationType;
                                //rc.SpinAngle = m_spinAngle;
                                //rc.DiameterFieldName = m_diameterFieldName;
                                //rc.UseDiameter = true;

                                // Create an edit operation enabling undo/redo
                                editor.StartOperation();
                                try
                                {
                                    //Get a cursor for selected features

                                    sel = fSel.SelectionSet as ISelectionSet2;
                                    sel.Update(null, false, out pointCursor);

                                    while ((pointFeature = (IFeature)pointCursor.NextRow()) != null)
                                    {
                                        if (map.SelectionCount <= maxIndividualRefresh)
                                        {
                                            //Prepare to redraw area around feature
                                            ext = pointFeature.Extent;
                                            ext.Expand(2, 2, true);
                                            invalid.Add(ext);
                                        }
                                        //Use Rotation Calculator
                                        // angle = rc.GetRotationUsingConnectedEdges(pointFeature);

                                        //Set rotation value in feature
                                        if (pointFeature.get_Value(rotationFieldPos) == null)
                                        {
                                            angle = 0;
                                        }
                                        else if (pointFeature.get_Value(rotationFieldPos) == DBNull.Value)
                                        {
                                            angle = 0;
                                        }
                                        else
                                        {
                                             Double.TryParse( pointFeature.get_Value(rotationFieldPos).ToString(),out angle);
                                        }
                                        angle = angle + addSpinAngle;
                                        if (angle > 360) angle -= 360;
                                        if (angle < 0) angle += 360;
                                        pointFeature.set_Value(rotationFieldPos, angle);
                                        pointCursor.UpdateRow(pointFeature as IRow);

                                        //Update progress bar
                                        i += 1;
                                        stepProgressor.Step();
                                        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_4") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";

                                        //Check if the cancel button was pressed. If so, stop process
                                        if (!trackCancel.Continue())
                                        {
                                            break;
                                        }

                                    }

                                    if (pointCursor != null)
                                    {
                                        Marshal.ReleaseComObject(pointCursor);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    editor.AbortOperation();
                                    progressDialog.HideDialog();
                                    MessageBox.Show("AddRotate\n" + ex.Message, ex.Source);
                                    return;
                                }

                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5"));

                            }

                        }

                        layer = enumLayer.Next();

                    }

                    progressDialog.HideDialog();

                    //Alert the user know if no work was performed
                    if (!(test))
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_6"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_3"));

                    //Redraw invalid areas or entire map
                    if (i > 0)
                    {
                        if (map.SelectionCount < maxIndividualRefresh)
                            invalid.Invalidate((short)esriScreenCache.esriAllScreenCaches);
                        else
                            activeView.Refresh();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5"));
                return;
            }
            finally
            {
                if (AddRotateSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                    pCmd = null;
                }
                if (pointCursor != null)
                {
                    Marshal.ReleaseComObject(pointCursor);
                }
                pCmd = null;
                editor = null;
                eLayers = null;
                map = null;
                geoFeatureLayerID = null;
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
                enumLayer = null;
                fLayer = null;
                geoFLayer = null;
                rRenderer = null;
                fSel = null;
                layer = null;

                activeView = null;
                invalid = null;
                ext = null;

                pointCursor = null;
                pointFeature = null;
                sel = null;
                progressDialogFactory = null;

                trackCancel = null;
                stepProgressor = null;
                progressDialog = null;

            }
        }
        public static void RotateSelected(IApplication app, double spinAngle, string diameterFieldName)
        {
            IEditor editor = null;
            IEditLayers eLayers = null;
            IMap map = null;
            UID geoFeatureLayerID = null;
            IEnumLayer enumLayer = null;
            IFeatureLayer fLayer = null;
            IGeoFeatureLayer geoFLayer = null;
            IRotationRenderer rRenderer = null;
            IFeatureSelection fSel = null;
            ILayer layer = null;
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            RotationCalculator rc = null;
            IActiveView activeView = null;
            IInvalidArea invalid = null;

            IEnvelope ext = null;
            ICursor pointCursor = null;
            IFeature pointFeature = null;
            ISelectionSet2 sel = null;


            try
            {

                int maxIndividualRefresh = 100;

                //Get list of editable layers
                 editor = Globals.getEditor(app);
                eLayers = (IEditLayers)editor;
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"));
                    return;
                }

                map = editor.Map;


                //Get list of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                if (map.SelectionCount > 0)
                {

                    string rotationFieldName;
                    int rotationFieldPos;
                    int count;
                    Nullable<double> angle;
                    int total = map.SelectionCount;
                    int i = 0;

                    //ProgressBar
                    progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                    // Create a CancelTracker
                    trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                    // Set the properties of the Step Progressor
                    System.Int32 int32_hWnd = app.hWnd;
                    stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = total;
                    stepProgressor.StepValue = 1;
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_1");

                    // Create the ProgressDialog. This automatically displays the dialog
                    progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                    // Set the properties of the ProgressDialog
                    progressDialog.CancelEnabled = true;
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_3") + total.ToString() + ".";
                    progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_2");
                    progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

                    // Prep rotation calculator
                    rc = new RotationCalculator(app);

                    //Prep screen refresh for selected features
                    activeView = map as IActiveView;
                    invalid = new InvalidAreaClass();
                    invalid.Display = editor.Display;


                    // Step through each geofeature layer in the map
                    enumLayer.Reset();
                    layer = enumLayer.Next();
                    bool test = false;
                    while (!(layer == null))
                    {
                        // Verify that this is a valid, visible point layer and that this layer is editable
                        fLayer = (IFeatureLayer)layer;
                        if (fLayer.Valid && fLayer.Visible && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) &&
                            eLayers.IsEditable(fLayer))
                        {
                            // Verify that a selection  for this layer
                            fSel = (IFeatureSelection)fLayer;
                            count = fSel.SelectionSet.Count;

                            if (count > 0)
                            {
                                // Verify that symbol rotation has been setup for this layer
                                geoFLayer = (IGeoFeatureLayer)fLayer;
                                if (!(geoFLayer.Renderer is IRotationRenderer))
                                {
                                    layer = enumLayer.Next();
                                    continue;
                                }


                                // Verify that the rotation field has been specified and that the field exists
                                rRenderer = (IRotationRenderer)geoFLayer.Renderer;
                                rotationFieldName = rRenderer.RotationField;
                                rotationFieldPos = fLayer.FeatureClass.FindField(rotationFieldName);
                                if (rotationFieldPos == -1)
                                {
                                    layer = enumLayer.Next();
                                    continue;
                                }
                                test = true;

                                rc.RotationType = rRenderer.RotationType;
                                rc.SpinAngle = spinAngle;
                                rc.DiameterFieldName = diameterFieldName;
                                rc.UseDiameter = true;

                                // Create an edit operation enabling undo/redo
                                editor.StartOperation();
                                try
                                {
                                    //Get a cursor for selected features
                                    sel = fSel.SelectionSet as ISelectionSet2;
                                    sel.Update(null, false, out pointCursor);

                                    while ((pointFeature = (IFeature)pointCursor.NextRow()) != null)
                                    {
                                        if (map.SelectionCount <= maxIndividualRefresh)
                                        {
                                            //Prepare to redraw area around feature
                                            ext = pointFeature.Extent;
                                            ext.Expand(2, 2, true);
                                            invalid.Add(ext);
                                        }
                                        //Use Rotation Calculator
                                        angle = rc.GetRotationUsingConnectedEdges(pointFeature);

                                        //Set rotation value in feature
                                        pointFeature.set_Value(rotationFieldPos, angle);
                                        pointCursor.UpdateRow(pointFeature as IRow);

                                        //Update progress bar
                                        i += 1;
                                        stepProgressor.Step();
                                        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_6") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";

                                        //Check if the cancel button was pressed. If so, stop process
                                        if (!trackCancel.Continue())
                                        {
                                            break;
                                        }

                                    }
                                    if (pointCursor != null)
                                    {
                                        Marshal.ReleaseComObject(pointCursor);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    editor.AbortOperation();
                                    progressDialog.HideDialog();
                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5") + "\n" + ex.Message, ex.Source);
                                    return;
                                }

                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5"));

                            }

                        }

                        layer = enumLayer.Next();

                    }

                    progressDialog.HideDialog();

                    //Alert the user know if no work was performed
                    if (!(test))
                        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_6"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5"));

                    //Redraw invalid areas or entire map
                    if (i > 0)
                    {
                        if (map.SelectionCount < maxIndividualRefresh)
                            invalid.Invalidate((short)esriScreenCache.esriAllScreenCaches);
                        else
                            activeView.Refresh();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_5"));
                return;
            }
            finally
            {
                editor = null;
                eLayers = null;
                map = null;
                geoFeatureLayerID = null;
                enumLayer = null;
                fLayer = null;
                geoFLayer = null;
                rRenderer = null;
                fSel = null;
                layer = null;
                progressDialogFactory = null;
                trackCancel = null;
                stepProgressor = null;
                progressDialog = null;
                rc = null;
                activeView = null;
                invalid = null;

                ext = null;
                pointCursor = null;
                pointFeature = null;
                sel = null;

            }
        }
        //public static void SplitLinesAtClick(IApplication app, string SplitSuspendAA, double SplitAtLocationSnap, double SkipDistance, IPoint SplitPoint, bool onlySelectedFeatures, bool ignoreTolerence, bool logEditOperation)
        //{

        //    ESRI.ArcGIS.Framework.ICommandItem pCmd = null;
        //    ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
        //    ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
        //    ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
        //    ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
        //    IMxDocument mxdoc = null;
        //    IEditor editor = null;
        //    IEditLayers eLayers = null;
        //    IMap map = null;
        //    IFeatureLayer fLayer = null;
        //    IFeatureSelection fSel = null;
        //    UID geoFeatureLayerID = null;
        //    IEnumLayer enumLayer = null;
        //    ILayer layer = null;
        //    List<IFeatureLayer> lineLayers = null;
        //    IHitTest hitTest = null;
        //    ISpatialFilter pSpatFilt = null;
        //    ICursor lineCursor = null;
        //    IFeatureCursor lineFCursor = null;
        //    ITopologicalOperator topoOp = null;

        //    IPolygon poly = null;
        //    IFeatureSelection lineSel = null;
        //    IFeature lineFeature = null;
        //    IFeatureEdit2 featureEdit = null;

        //    IPoint pHitPnt = null;
        //    ITopologicalOperator topoOpEndStart = null;
        //    IPolygon polyEndStart = null;
        //    IRelationalOperator relOp = null;
        //    ICurve curve = null;
        //    List<MergeSplitGeoNetFeatures> m_Config = null;
           
        //    ISet pSet = null;
        //    IFeature pSplitResFeat = null;
           
              
        //    try
        //    {
        //        m_Config = ConfigUtil.GetMergeSplitConfig();
              
        //        if (SplitSuspendAA.ToUpper() == "TRUE")
        //        {
        //            pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
        //            if (pCmd != null)
        //            {
        //                pCmd.Execute();
        //            }
        //        }
        //        mxdoc = (IMxDocument)app.Document;
        //        editor = Globals.getEditor(app);

        //        if (editor.EditState != esriEditState.esriStateEditing)
        //        {
        //            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_4"));
        //            return;
        //        }

        //        //Get enumeration of editable layers
        //        eLayers = (IEditLayers)editor;

        //        map = editor.Map;

        //        int i = 0;

        //        //Get enumeration of feature layers
        //        geoFeatureLayerID = new UIDClass();
        //        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        //        enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);


        //        layer = enumLayer.Next();

        //        // Create list of visible, editable line layers
        //        lineLayers = new List<IFeatureLayer>();
        //        enumLayer.Reset();
        //        layer = enumLayer.Next();
        //        while (!(layer == null))
        //        {
        //            fLayer = (IFeatureLayer)layer;
        //            if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
        //               && (fLayer.Visible))
        //            {
        //                if (eLayers.IsEditable(fLayer))
        //                {
        //                    if (onlySelectedFeatures)
        //                    {
        //                        fSel = (IFeatureSelection)fLayer;
        //                        if (fSel.SelectionSet.Count > 0)
        //                            lineLayers.Add(fLayer);
        //                    }
        //                    else
        //                    {
        //                        lineLayers.Add(fLayer);
        //                    }
        //                }
        //            }
        //            layer = enumLayer.Next();
        //        }

        //        //ProgressBar
        //        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

        //        // Create a CancelTracker
        //        trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

        //        // Set the properties of the Step Progressor
        //        System.Int32 int32_hWnd = app.hWnd;
        //        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
        //        stepProgressor.MinRange = 1;
        //        stepProgressor.MaxRange = 1;
        //        stepProgressor.StepValue = 1;
        //        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_7");

        //        // Create the ProgressDialog. This automatically displays the dialog
        //        progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

        //        // Set the properties of the ProgressDialog
        //        progressDialog.CancelEnabled = true;
        //        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_8");
        //        progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_7");
        //        progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

        //        //Create an edit operation enabling undo/redo
        //        if (logEditOperation)
        //            editor.StartOperation();

        //        try
        //        {

        //            topoOp = SplitPoint as ITopologicalOperator;
        //            if (ignoreTolerence)
        //            {
        //                poly = topoOp.Buffer(Globals.ConvertFeetToMapUnits(1, app)) as IPolygon;
        //            }
        //            else
        //            {
        //                poly = topoOp.Buffer(Globals.ConvertFeetToMapUnits(SplitAtLocationSnap, app)) as IPolygon;
        //            }
        //            foreach (IFeatureLayer lineLayer in lineLayers)
        //            {

        //                if (onlySelectedFeatures)
        //                {
        //                    lineSel = lineLayer as IFeatureSelection;
        //                    lineSel.SelectionSet.Search(null, false, out lineCursor);
        //                    lineFCursor = lineCursor as IFeatureCursor;

        //                }
        //                else
        //                {
        //                    pSpatFilt = new SpatialFilter();
        //                    pSpatFilt.GeometryField = lineLayer.FeatureClass.ShapeFieldName;
        //                    pSpatFilt.Geometry = poly;
        //                    pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //                    lineFCursor = lineLayer.Search(pSpatFilt, false);
        //                    //lineFCursor = lineCursor as IFeatureCursor;
        //                }

        //                IList<MergeSplitFlds> pFldsNames = new List<MergeSplitFlds>();

        //                if (m_Config.Count > 0)
        //                {
        //                    foreach (A4LGSharedFunctions.Field FldNam in m_Config[0].Fields)
        //                    {
        //                        int idx = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, FldNam.Name);
        //                        if (idx > -1)
        //                            pFldsNames.Add(new MergeSplitFlds(FldNam.Name, idx, "", FldNam.MergeRule, FldNam.SplitRule));


        //                    }

        //                }

        //                lineFeature = lineFCursor.NextFeature();
        //                while (!(lineFeature == null))
        //                {
        //                    featureEdit = lineFeature as IFeatureEdit2;
        //                    hitTest = lineFeature.ShapeCopy as IHitTest;
        //                    pHitPnt = new PointClass();
        //                    double pHitDist = -1;
        //                    int pHitPrt = -1;
        //                    int pHitSeg = -1;
        //                    bool pHitSide = false;
        //                    bool hit = hitTest.HitTest(SplitPoint, SplitAtLocationSnap, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPnt, pHitDist, pHitPrt, pHitSeg, pHitSide);

        //                    if (hit)
        //                    {
        //                        if (ignoreTolerence == true && (pHitDist == 0.0 || pHitDist == -1.0))
        //                        {

        //                            //Split feature
        //                            topoOpEndStart = pHitPnt as ITopologicalOperator;
        //                            polyEndStart = topoOpEndStart.Buffer(SkipDistance) as IPolygon;
        //                            relOp = polyEndStart as IRelationalOperator;
        //                            curve = lineFeature.Shape as ICurve;
        //                            if (!(relOp.Contains(curve.FromPoint)) &&
        //                                !(relOp.Contains(curve.ToPoint)))
        //                            {
        //                                Globals.FlashGeometry(pHitPnt, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);



                                      
        //                                double dblHighVal = 0;
        //                                double dblLowVal = 0;
        //                                int intHighIdx = -1;
        //                                int intLowIdx = -1;
        //                                foreach (MergeSplitFlds FldNam in pFldsNames)
        //                                {
        //                                    FldNam.Value = lineFeature.get_Value(FldNam.FieldIndex).ToString();
        //                                    if (FldNam.SplitType.ToUpper() == "MAX")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblHighVal = Convert.ToDouble(FldNam.Value);
        //                                                intHighIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }
        //                                    else if (FldNam.SplitType.ToUpper() == "MIN")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblLowVal = Convert.ToDouble(FldNam.Value);
        //                                                intLowIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }


        //                                }
        //                                if (intHighIdx > -1 && intLowIdx > -1)
        //                                {
        //                                    double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

        //                                    double splitDist = Globals.PointDistanceOnLine(pHitPnt, lineFeature.Shape as IPolyline, 2, out pHitPnt);
        //                                    double percentSplit = splitDist / len;
        //                                    double dblMidVal;
        //                                    if (m_Config[0].SplitFormatString == "")
        //                                    {
        //                                        dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
        //                                    }
        //                                    else
        //                                    {
        //                                        dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

        //                                    }


        //                                    //Split feature
        //                                    pSet = featureEdit.SplitWithUpdate(pHitPnt);

        //                                    if (pSet.Count == 1)
        //                                    {
        //                                        while ((pSplitResFeat = pSet.Next() as IFeature) != null)
        //                                        {
        //                                            if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intHighIdx, dblMidVal);
        //                                            }
        //                                            else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intLowIdx, dblMidVal);

        //                                            }
        //                                        }

        //                                    }
        //                                    if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intHighIdx, dblMidVal);
        //                                    }
        //                                    else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intLowIdx, dblMidVal);

        //                                    }
        //                                }
        //                                else
        //                                    featureEdit.SplitWithUpdate(pHitPnt);

        //                                //mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, hitTest, mxdoc.ActiveView.Extent);
        //                                mxdoc.ActiveView.Refresh();

        //                                //mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, curve, mxdoc.ActiveView.Extent);
        //                            }

        //                            topoOpEndStart = null;
        //                            polyEndStart = null;
        //                            relOp = null;
        //                            curve = null;


        //                        }
        //                        else if (ignoreTolerence == false)
        //                        {

        //                            //Split feature
        //                            topoOpEndStart = pHitPnt as ITopologicalOperator;
        //                            polyEndStart = topoOpEndStart.Buffer(SkipDistance) as IPolygon;
        //                            relOp = polyEndStart as IRelationalOperator;
        //                            curve = lineFeature.ShapeCopy as ICurve;
        //                            if (!(relOp.Contains(curve.FromPoint)) &&
        //                                !(relOp.Contains(curve.ToPoint)))
        //                            {
        //                                //Split feature
        //                                Globals.FlashGeometry(pHitPnt, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);


        //                                double dblHighVal = 0;
        //                                double dblLowVal = 0;
        //                                int intHighIdx = -1;
        //                                int intLowIdx = -1;
        //                                foreach (MergeSplitFlds FldNam in pFldsNames)
        //                                {
        //                                    FldNam.Value = lineFeature.get_Value(FldNam.FieldIndex).ToString();
        //                                    if (FldNam.SplitType.ToUpper() == "MAX")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblHighVal = Convert.ToDouble(FldNam.Value);
        //                                                intHighIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }
        //                                    else if (FldNam.SplitType.ToUpper() == "MIN")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblLowVal = Convert.ToDouble(FldNam.Value);
        //                                                intLowIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }


        //                                }
        //                                if (intHighIdx > -1 && intLowIdx > -1)
        //                                {
        //                                    double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

        //                                    double splitDist = Globals.PointDistanceOnLine(pHitPnt, lineFeature.Shape as IPolyline, 2, out pHitPnt);
        //                                    double percentSplit = splitDist / len;
        //                                    double dblMidVal;
        //                                    if (m_Config[0].SplitFormatString == "")
        //                                    {
        //                                        dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
        //                                    }
        //                                    else
        //                                    {
        //                                        dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

        //                                    }


        //                                    //Split feature
        //                                    pSet = featureEdit.SplitWithUpdate(pHitPnt);

        //                                    if (pSet.Count == 1)
        //                                    {
        //                                        while ((pSplitResFeat = pSet.Next() as IFeature) != null)
        //                                        {
        //                                            if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intHighIdx, dblMidVal);
        //                                            }
        //                                            else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intLowIdx, dblMidVal);

        //                                            }
        //                                        }

        //                                    }
        //                                    if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intHighIdx, dblMidVal);
        //                                    }
        //                                    else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intLowIdx, dblMidVal);

        //                                    }
        //                                }
        //                                else
        //                                    featureEdit.SplitWithUpdate(pHitPnt);
        //                            }

        //                            topoOpEndStart = null;
        //                            polyEndStart = null;
        //                            relOp = null;
        //                            curve = null;

        //                        }
        //                    }
        //                    if (lineFeature != null)
        //                    {
        //                        System.Runtime.InteropServices.Marshal.ReleaseComObject(lineFeature);
        //                    }
        //                    lineFeature = lineFCursor.NextFeature();
        //                }
        //                if (lineCursor != null)
        //                {
        //                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);
        //                }
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(lineFCursor);

        //            }

        //            //Update progress bar
        //            i += 1;
        //            progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_8");
        //            ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
        //            statusBar.set_Message(0, i.ToString());
        //        }
        //        catch (Exception ex)
        //        {

        //            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1"));
        //            if (logEditOperation)
        //                editor.AbortOperation();


        //            return;
        //        }
        //        finally
        //        {
        //            mxdoc.ActiveView.Refresh();

        //            progressDialog.HideDialog();

        //        }



        //        //Stop the edit operation 
        //        if (logEditOperation)
        //            editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9"));

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1"));
        //        return;
        //    }
        //    finally
        //    {

        //        if (progressDialog != null)
        //            progressDialog.HideDialog();
        //        if (SplitSuspendAA.ToUpper() == "TRUE")
        //        {
        //            pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
        //            if (pCmd != null)
        //            {
        //                pCmd.Execute();
        //            }
        //        }
        //        pCmd = null;
        //        progressDialog = null;
        //        progressDialogFactory = null;
        //        trackCancel = null;
        //        stepProgressor = null;
        //        mxdoc = null;
        //        editor = null;
        //        eLayers = null;
        //        map = null;
        //        fLayer = null;
        //        fSel = null;
        //        geoFeatureLayerID = null;
        //        enumLayer = null;
        //        layer = null;
        //        lineLayers = null;
        //        hitTest = null;
        //        pSpatFilt = null;
        //        lineCursor = null;
        //        lineFCursor = null;
        //        topoOp = null;

        //        poly = null;
        //        lineSel = null;
        //        lineFeature = null;
        //        featureEdit = null;

        //        pHitPnt = null;
        //        topoOpEndStart = null;
        //        polyEndStart = null;
        //        relOp = null;
        //        curve = null;



        //    }
        //}
        //public static void SplitLines(IApplication app, string SplitSuspendAA, double SplitAtLocationSnap, double SkipDistance)
        //{



        //    IProgressDialogFactory progressDialogFactory = null;
        //    ITrackCancel trackCancel = null;
        //    IStepProgressor stepProgressor = null;

        //    IProgressDialog2 progressDialog = null;

        //    ICommandItem pCmd = null;
        //    IEditLayers eLayers = null;
        //    IEditor editor = null;
        //    IMxDocument mxdoc = null;
        //    IMap map = null;

        //    IFeatureLayer fLayer = null;
        //    IFeatureSelection fSel = null;

        //    UID geoFeatureLayerID = null;

        //    IEnumLayer enumLayer = null;


        //    List<IFeatureLayer> pointLayers = null;

        //    ILayer layer = null;
        //    List<IFeatureLayer> lineLayers;
        //    List<MergeSplitGeoNetFeatures> m_Config = null;
        //    IPoint pSplitPnt = null;
        //    ISet pSet = null;
        //    IFeature pSplitResFeat = null;
        //    try
        //    {
        //        m_Config = ConfigUtil.GetMergeSplitConfig();
              
               
        //        if (SplitSuspendAA.ToUpper() == "TRUE")
        //        {
        //            pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
        //            if (pCmd != null)
        //            {
        //                pCmd.Execute();
        //            }
        //        }
        //        editor = Globals.getEditor(app);
        //        if (editor.EditState != esriEditState.esriStateEditing)
        //        {
        //            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
        //            return;
        //        }

        //        //Get enumeration of editable layers
        //        eLayers = (IEditLayers)editor;


        //        mxdoc = (IMxDocument)app.Document;
        //        map = editor.Map;

        //        int i = 0;
        //        int total = 0;

        //        //Get enumeration of feature layers
        //        geoFeatureLayerID = new UIDClass();
        //        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        //        enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

        //        // Create list of visible point layers with selected feature(s)
        //        pointLayers = new List<IFeatureLayer>();
        //        lineLayers = new List<IFeatureLayer>();
        //        enumLayer.Reset();
        //        layer = enumLayer.Next();
        //        while (layer != null)
        //        {
        //            fLayer = (IFeatureLayer)layer;
        //            if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
        //               && (fLayer.Visible))
        //            {
        //                fSel = fLayer as IFeatureSelection;
        //                if (fSel.SelectionSet.Count > 0)
        //                {
        //                    total += fSel.SelectionSet.Count;
        //                    pointLayers.Add(fLayer);
        //                }
        //            }
        //            else if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
        //                && (fLayer.Visible))
        //            {
        //                if (eLayers.IsEditable(fLayer))
        //                    lineLayers.Add(fLayer);
        //            }
        //            layer = enumLayer.Next();
        //        }


        //        //ProgressBar
        //        progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

        //        // Create a CancelTracker
        //        trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

        //        // Set the properties of the Step Progressor
        //        System.Int32 int32_hWnd = app.hWnd;
        //        stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
        //        stepProgressor.MinRange = 0;
        //        stepProgressor.MaxRange = total;
        //        stepProgressor.StepValue = 1;
        //        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_10");

        //        // Create the ProgressDialog. This automatically displays the dialog
        //        progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor;

        //        // Set the properties of the ProgressDialog
        //        progressDialog.CancelEnabled = true;
        //        progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_11") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
        //        progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9");
        //        progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

        //        //Create an edit operation enabling undo/redo
        //        editor.StartOperation();
        //        ICursor pointCursor = null;
        //        IFeatureSelection pointSel = null;
        //        IFeature pointFeature = null;
        //        ISpatialFilter sFilter = null;
        //        IFeatureSelection lineSel = null;
        //        IFeatureCursor lineCursor = null;
        //        IFeature lineFeature = null;
        //        IFeatureEdit2 featureEdit = null;

        //        //Determine if point is at end of this line (if so skip)
        //        ITopologicalOperator topoOp = null;
        //        IPolygon poly = null;
        //        IRelationalOperator relOp = null;
        //        ICurve curve = null;


               
                
        //        try
        //        {
        //            // step through all points and split the lines that intersect them

        //            foreach (IFeatureLayer pointLayer in pointLayers)
        //            {
        //                pointSel = pointLayer as IFeatureSelection;
        //                pointSel.SelectionSet.Search(null, false, out pointCursor);
        //                pointFeature = pointCursor.NextRow() as IFeature;
        //                while (!(pointFeature == null))
        //                {
        //                    foreach (IFeatureLayer lineLayer in lineLayers)
        //                    {
        //                        sFilter = new SpatialFilterClass();
        //                        sFilter.Geometry = pointFeature.ShapeCopy;
        //                        sFilter.GeometryField = lineLayer.FeatureClass.ShapeFieldName;
        //                        sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

        //                        lineSel = lineLayer as IFeatureSelection;
        //                        lineCursor = lineLayer.Search(sFilter, false);
        //                        lineFeature = lineCursor.NextFeature();

        //                        IList<MergeSplitFlds> pFldsNames = new List<MergeSplitFlds>();

        //                        if (m_Config.Count > 0)
        //                        {
        //                            foreach (A4LGSharedFunctions.Field FldNam in m_Config[0].Fields)
        //                            {
        //                                int idx = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, FldNam.Name);
        //                                if (idx > -1)
        //                                    pFldsNames.Add(new MergeSplitFlds(FldNam.Name, idx, "", FldNam.MergeRule,FldNam.SplitRule));


        //                            }

        //                        }

        //                        while (!(lineFeature == null))
        //                        {
        //                            featureEdit = lineFeature as IFeatureEdit2;

        //                            //Determine if point is at end of this line (if so skip)
        //                            topoOp = pointFeature.Shape as ITopologicalOperator;
        //                            poly = topoOp.Buffer(SkipDistance) as IPolygon;
        //                            relOp = poly as IRelationalOperator;
        //                            curve = lineFeature.Shape as ICurve;
        //                            if (!(relOp.Contains(curve.FromPoint)) &
        //                                !(relOp.Contains(curve.ToPoint)))
        //                            {
        //                                 pSplitPnt = pointFeature.ShapeCopy as IPoint;
        //                                 double dblHighVal = 0;
        //                                 double dblLowVal = 0;
        //                                 int intHighIdx = -1;
        //                                 int intLowIdx = -1;
        //                                foreach (MergeSplitFlds FldNam in pFldsNames)
        //                                {
        //                                    FldNam.Value= lineFeature.get_Value(FldNam.FieldIndex).ToString();
        //                                    if (FldNam.SplitType.ToUpper() == "MAX")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblHighVal = Convert.ToDouble(FldNam.Value);
        //                                                intHighIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }
        //                                    else if (FldNam.SplitType.ToUpper() == "MIN")
        //                                    {
        //                                        if (FldNam.Value != null)
        //                                        {
        //                                            if (FldNam.Value != "")
        //                                            {

        //                                                dblLowVal = Convert.ToDouble(FldNam.Value);
        //                                                intLowIdx = FldNam.FieldIndex;
        //                                            }
        //                                        }
        //                                    }

                                          
        //                                }
        //                                if (intHighIdx > -1 && intLowIdx > -1)
        //                                {
        //                                    double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

        //                                    double splitDist = Globals.PointDistanceOnLine(pSplitPnt, lineFeature.Shape as IPolyline, 2, out pSplitPnt);
        //                                    double percentSplit = splitDist / len;
        //                                    double dblMidVal;
        //                                    if (m_Config[0].SplitFormatString == "")
        //                                    {
        //                                        dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
        //                                    }
        //                                    else
        //                                    {
        //                                        dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

        //                                    }
                                            

        //                                    //Split feature
        //                                    pSet = featureEdit.SplitWithUpdate(pSplitPnt);

        //                                    if (pSet.Count == 1)
        //                                    {
        //                                        while ((pSplitResFeat = pSet.Next() as IFeature) != null)
        //                                        {
        //                                            if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pSplitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pSplitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intHighIdx, dblMidVal);
        //                                            }
        //                                            else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pSplitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pSplitPnt.Y)
        //                                            {
        //                                                pSplitResFeat.set_Value(intLowIdx, dblMidVal);

        //                                            }
        //                                        }

        //                                    }
        //                                    if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pSplitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pSplitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intHighIdx, dblMidVal);
        //                                    }
        //                                    else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pSplitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pSplitPnt.Y)
        //                                    {
        //                                        lineFeature.set_Value(intLowIdx, dblMidVal);

        //                                    }
        //                                }else
        //                                    pSet = featureEdit.SplitWithUpdate(pSplitPnt);


        //                                pSplitPnt = null;
        //                            }

        //                            lineFeature = lineCursor.NextFeature();
        //                        }
        //                        System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);

        //                    }

        //                    //Update progress bar
        //                    i += 1;
        //                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_11") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
        //                    stepProgressor.Step();
        //                    ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
        //                    statusBar.set_Message(0, i.ToString());

        //                    //Check if the cancel button was pressed. If so, stop process
        //                    if (!trackCancel.Continue())
        //                    {
        //                        break;
        //                    }
        //                    pointFeature = (IFeature)pointCursor.NextRow();
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            editor.AbortOperation();
        //            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
        //            return;
        //        }
        //        finally
        //        {
        //            if (pointCursor != null)
        //            {
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(pointCursor);
        //            }
        //            pointCursor = null;
        //            pointSel = null;
        //            pointFeature = null;
        //            sFilter = null;
        //            lineSel = null;
        //            lineCursor = null;
        //            lineFeature = null;
        //            featureEdit = null;

        //            topoOp = null;
        //            poly = null;
        //            relOp = null;
        //            curve = null;
        //        }
        //        progressDialog.HideDialog();


        //        //Stop the edit operation 
        //        editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9"));

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
        //        return;
        //    }
        //    finally
        //    {

        //        pSet = null;
        //        pSplitPnt = null;
        //        if (SplitSuspendAA.ToUpper() == "TRUE")
        //        {
        //            pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
        //            if (pCmd != null)
        //            {
        //                pCmd.Execute();
        //            }
        //            pCmd = null;

        //        }
        //        if (progressDialog != null)
        //            progressDialog.HideDialog();
        //        progressDialogFactory = null;
        //        trackCancel = null;
        //        stepProgressor = null;

        //        progressDialog = null;

        //        pCmd = null;
        //        eLayers = null;
        //        editor = null;
        //        mxdoc = null;
        //        map = null;

        //        fLayer = null;
        //        fSel = null;

        //        geoFeatureLayerID = null;

        //        enumLayer = null;


        //        pointLayers = null;

        //        layer = null;
        //        lineLayers = null;
        //        pSplitResFeat = null;
        //    }
        //}
        public static void SplitLinesAtClick(IApplication app, string SplitSuspendAA, double SplitAtLocationSnap, double SkipDistance, IPoint SplitPoint, bool onlySelectedFeatures, bool ignoreTolerence, bool logEditOperation)
        {

            ESRI.ArcGIS.Framework.ICommandItem pCmd = null;
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog = null;
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = null;
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = null;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = null;
            IMxDocument mxdoc = null;
            IEditor editor = null;
            IEditLayers eLayers = null;
            IMap map = null;
            IFeatureLayer fLayer = null;
            IFeatureSelection fSel = null;
            UID geoFeatureLayerID = null;
            IEnumLayer enumLayer = null;
            ILayer layer = null;
            List<IFeatureLayer> lineLayers = null;

            ISpatialFilter pSpatFilt = null;
            ICursor lineCursor = null;
            IFeatureCursor lineFCursor = null;
            ITopologicalOperator topoOp = null;

            IPolygon poly = null;
            IFeatureSelection lineSel = null;
            IFeature lineFeature = null;


            List<MergeSplitGeoNetFeatures> m_Config = null;


            //IHitTest hitTest = null;
            //IFeatureEdit2 featureEdit = null;

            //IPoint pHitPnt = null;
            //ITopologicalOperator topoOpEndStart = null;
            //IPolygon polyEndStart = null;
            //IRelationalOperator relOp = null;
            //ICurve curve = null;
            //ISet pSet = null;
            //IFeature pSplitResFeat = null;


            try
            {
                m_Config = ConfigUtil.GetMergeSplitConfig();

                if (SplitSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                }
                mxdoc = (IMxDocument)app.Document;
                editor = Globals.getEditor(app);

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_4"));
                    return;
                }

                //Get enumeration of editable layers
                eLayers = (IEditLayers)editor;

                map = editor.Map;

                int i = 0;

                //Get enumeration of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);


                layer = enumLayer.Next();

                // Create list of visible, editable line layers
                lineLayers = new List<IFeatureLayer>();
                enumLayer.Reset();
                layer = enumLayer.Next();
                while (!(layer == null))
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                       && (fLayer.Visible))
                    {
                        if (eLayers.IsEditable(fLayer))
                        {
                            if (onlySelectedFeatures)
                            {
                                fSel = (IFeatureSelection)fLayer;
                                if (fSel.SelectionSet.Count > 0)
                                    lineLayers.Add(fLayer);
                            }
                            else
                            {
                                lineLayers.Add(fLayer);
                            }
                        }
                    }
                    layer = enumLayer.Next();
                }

                //ProgressBar
                progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                // Create a CancelTracker
                trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = app.hWnd;
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                stepProgressor.MinRange = 1;
                stepProgressor.MaxRange = 1;
                stepProgressor.StepValue = 1;
                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_7");

                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = true;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_8");
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_7");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

                //Create an edit operation enabling undo/redo
                if (logEditOperation)
                    editor.StartOperation();

                try
                {

                    topoOp = SplitPoint as ITopologicalOperator;
                    if (ignoreTolerence)
                    {
                        poly = topoOp.Buffer(Globals.ConvertFeetToMapUnits(.1, app)) as IPolygon;
                    }
                    else
                    {
                        poly = topoOp.Buffer(Globals.ConvertFeetToMapUnits(SplitAtLocationSnap, app)) as IPolygon;
                    }
                    foreach (IFeatureLayer lineLayer in lineLayers)
                    {

                        if (onlySelectedFeatures)
                        {
                            lineSel = lineLayer as IFeatureSelection;
                            lineSel.SelectionSet.Search(null, false, out lineCursor);
                            lineFCursor = lineCursor as IFeatureCursor;

                        }
                        else
                        {
                            pSpatFilt = new SpatialFilter();
                            pSpatFilt.GeometryField = lineLayer.FeatureClass.ShapeFieldName;
                            pSpatFilt.Geometry = poly;
                            pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            lineFCursor = lineLayer.Search(pSpatFilt, false);
                            //lineFCursor = lineCursor as IFeatureCursor;
                        }

                        IList<MergeSplitFlds> pFldsNames = new List<MergeSplitFlds>();
                        string strFormValu = "{0:0.##}";
                        if (m_Config.Count > 0)
                        {
                            foreach (A4LGSharedFunctions.Field FldNam in m_Config[0].Fields)
                            {
                                int idx = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, FldNam.Name);
                                if (idx > -1)
                                    pFldsNames.Add(new MergeSplitFlds(FldNam.Name, idx, "", FldNam.MergeRule, FldNam.SplitRule));


                            }
                            strFormValu = m_Config[0].SplitFormatString;
                        }


                        lineFeature = lineFCursor.NextFeature();
                        while (!(lineFeature == null))
                        {
                            Globals.splitLineWithPoint(lineFeature, SplitPoint, SplitAtLocationSnap, pFldsNames, strFormValu,app);
                            //featureEdit = lineFeature as IFeatureEdit2;
                            //hitTest = lineFeature.ShapeCopy as IHitTest;
                            //pHitPnt = new PointClass();
                            //double pHitDist = -1;
                            //int pHitPrt = -1;
                            //int pHitSeg = -1;
                            //bool pHitSide = false;
                            //bool hit = hitTest.HitTest(SplitPoint, SplitAtLocationSnap, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPnt, pHitDist, pHitPrt, pHitSeg, pHitSide);

                            //if (hit)
                            //{
                            //    if (ignoreTolerence == true && (pHitDist == 0.0 || pHitDist == -1.0))
                            //    {

                            //        //Split feature
                            //        topoOpEndStart = pHitPnt as ITopologicalOperator;
                            //        polyEndStart = topoOpEndStart.Buffer(SkipDistance) as IPolygon;
                            //        relOp = polyEndStart as IRelationalOperator;
                            //        curve = lineFeature.Shape as ICurve;
                            //        if (!(relOp.Contains(curve.FromPoint)) &&
                            //            !(relOp.Contains(curve.ToPoint)))
                            //        {
                            //            Globals.FlashGeometry(pHitPnt, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);




                            //            double dblHighVal = 0;
                            //            double dblLowVal = 0;
                            //            int intHighIdx = -1;
                            //            int intLowIdx = -1;
                            //            foreach (MergeSplitFlds FldNam in pFldsNames)
                            //            {
                            //                FldNam.Value = lineFeature.get_Value(FldNam.FieldIndex).ToString();
                            //                if (FldNam.SplitType.ToUpper() == "MAX")
                            //                {
                            //                    if (FldNam.Value != null)
                            //                    {
                            //                        if (FldNam.Value != "")
                            //                        {

                            //                            dblHighVal = Convert.ToDouble(FldNam.Value);
                            //                            intHighIdx = FldNam.FieldIndex;
                            //                        }
                            //                    }
                            //                }
                            //                else if (FldNam.SplitType.ToUpper() == "MIN")
                            //                {
                            //                    if (FldNam.Value != null)
                            //                    {
                            //                        if (FldNam.Value != "")
                            //                        {

                            //                            dblLowVal = Convert.ToDouble(FldNam.Value);
                            //                            intLowIdx = FldNam.FieldIndex;
                            //                        }
                            //                    }
                            //                }


                            //            }
                            //            if (intHighIdx > -1 && intLowIdx > -1)
                            //            {
                            //                double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

                            //                double splitDist = Globals.PointDistanceOnLine(pHitPnt, lineFeature.Shape as IPolyline, 2, out pHitPnt);
                            //                double percentSplit = splitDist / len;
                            //                double dblMidVal;
                            //                if (m_Config[0].SplitFormatString == "")
                            //                {
                            //                    dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
                            //                }
                            //                else
                            //                {
                            //                    dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

                            //                }


                            //                //Split feature
                            //                pSet = featureEdit.SplitWithUpdate(pHitPnt);

                            //                if (pSet.Count == 1)
                            //                {
                            //                    while ((pSplitResFeat = pSet.Next() as IFeature) != null)
                            //                    {
                            //                        if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            //                        {
                            //                            pSplitResFeat.set_Value(intHighIdx, dblMidVal);
                            //                        }
                            //                        else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            //                        {
                            //                            pSplitResFeat.set_Value(intLowIdx, dblMidVal);

                            //                        }
                            //                    }

                            //                }
                            //                if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            //                {
                            //                    lineFeature.set_Value(intHighIdx, dblMidVal);
                            //                }
                            //                else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            //                {
                            //                    lineFeature.set_Value(intLowIdx, dblMidVal);

                            //                }
                            //            }
                            //            else
                            //                featureEdit.SplitWithUpdate(pHitPnt);

                            //            //mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, hitTest, mxdoc.ActiveView.Extent);
                            //            mxdoc.ActiveView.Refresh();

                            //            //mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, curve, mxdoc.ActiveView.Extent);
                            //        }

                            //        topoOpEndStart = null;
                            //        polyEndStart = null;
                            //        relOp = null;
                            //        curve = null;


                            //    }
                            //    else if (ignoreTolerence == false)
                            //    {

                            //        //Split feature
                            //        topoOpEndStart = pHitPnt as ITopologicalOperator;
                            //        polyEndStart = topoOpEndStart.Buffer(SkipDistance) as IPolygon;
                            //        relOp = polyEndStart as IRelationalOperator;
                            //        curve = lineFeature.ShapeCopy as ICurve;
                            //        if (!(relOp.Contains(curve.FromPoint)) &&
                            //            !(relOp.Contains(curve.ToPoint)))
                            //        {
                            //            //Split feature
                            //            Globals.FlashGeometry(pHitPnt, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);


                            //            double dblHighVal = 0;
                            //            double dblLowVal = 0;
                            //            int intHighIdx = -1;
                            //            int intLowIdx = -1;
                            //            foreach (MergeSplitFlds FldNam in pFldsNames)
                            //            {
                            //                FldNam.Value = lineFeature.get_Value(FldNam.FieldIndex).ToString();
                            //                if (FldNam.SplitType.ToUpper() == "MAX")
                            //                {
                            //                    if (FldNam.Value != null)
                            //                    {
                            //                        if (FldNam.Value != "")
                            //                        {

                            //                            dblHighVal = Convert.ToDouble(FldNam.Value);
                            //                            intHighIdx = FldNam.FieldIndex;
                            //                        }
                            //                    }
                            //                }
                            //                else if (FldNam.SplitType.ToUpper() == "MIN")
                            //                {
                            //                    if (FldNam.Value != null)
                            //                    {
                            //                        if (FldNam.Value != "")
                            //                        {

                            //                            dblLowVal = Convert.ToDouble(FldNam.Value);
                            //                            intLowIdx = FldNam.FieldIndex;
                            //                        }
                            //                    }
                            //                }


                            //            }
                            //            if (intHighIdx > -1 && intLowIdx > -1)
                            //            {
                            //                double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

                            //                double splitDist = Globals.PointDistanceOnLine(pHitPnt, lineFeature.Shape as IPolyline, 2, out pHitPnt);
                            //                double percentSplit = splitDist / len;
                            //                double dblMidVal;
                            //                if (m_Config[0].SplitFormatString == "")
                            //                {
                            //                    dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
                            //                }
                            //                else
                            //                {
                            //                    dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

                            //                }


                            //                //Split feature
                            //                pSet = featureEdit.SplitWithUpdate(pHitPnt);

                            //                if (pSet.Count == 1)
                            //                {
                            //                    while ((pSplitResFeat = pSet.Next() as IFeature) != null)
                            //                    {
                            //                        if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            //                        {
                            //                            pSplitResFeat.set_Value(intHighIdx, dblMidVal);
                            //                        }
                            //                        else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            //                        {
                            //                            pSplitResFeat.set_Value(intLowIdx, dblMidVal);

                            //                        }
                            //                    }

                            //                }
                            //                if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            //                {
                            //                    lineFeature.set_Value(intHighIdx, dblMidVal);
                            //                }
                            //                else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            //                {
                            //                    lineFeature.set_Value(intLowIdx, dblMidVal);

                            //                }
                            //            }
                            //            else
                            //                featureEdit.SplitWithUpdate(pHitPnt);
                            //        }

                            //        topoOpEndStart = null;
                            //        polyEndStart = null;
                            //        relOp = null;
                            //        curve = null;

                            //    }
                            //}
                            if (lineFeature != null)
                            {
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(lineFeature);
                            }
                            lineFeature = lineFCursor.NextFeature();
                        }
                        if (lineCursor != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(lineFCursor);

                    }

                    //Update progress bar
                    i += 1;
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_8");
                    ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                    statusBar.set_Message(0, i.ToString());
                }
                catch (Exception ex)
                {

                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1"));
                    if (logEditOperation)
                        editor.AbortOperation();


                    return;
                }
                finally
                {
                    progressDialog.HideDialog();

                }



                //Stop the edit operation 
                if (logEditOperation)
                    editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsRealName_1"));
                return;
            }
            finally
            {

                if (progressDialog != null)
                    progressDialog.HideDialog();
                if (SplitSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                }
                pCmd = null;
                progressDialog = null;
                progressDialogFactory = null;
                trackCancel = null;
                stepProgressor = null;
                mxdoc = null;
                editor = null;
                eLayers = null;
                map = null;
                fLayer = null;
                fSel = null;
                geoFeatureLayerID = null;
                enumLayer = null;
                layer = null;
                lineLayers = null;

                pSpatFilt = null;
                lineCursor = null;
                lineFCursor = null;
                topoOp = null;

                poly = null;
                lineSel = null;
                lineFeature = null;
                //featureEdit = null;
                //hitTest = null;
                //pHitPnt = null;
                //topoOpEndStart = null;
                //polyEndStart = null;
                //relOp = null;
                //curve = null;



            }
        }
        public static void SplitLines(IApplication app, string SplitSuspendAA, double SplitAtLocationSnap, double SkipDistance)
        {



            IProgressDialogFactory progressDialogFactory = null;
            ITrackCancel trackCancel = null;
            IStepProgressor stepProgressor = null;

            IProgressDialog2 progressDialog = null;

            ICommandItem pCmd = null;
            IEditLayers eLayers = null;
            IEditor editor = null;
            IMxDocument mxdoc = null;
            IMap map = null;

            IFeatureLayer fLayer = null;
            IFeatureSelection fSel = null;

            UID geoFeatureLayerID = null;

            IEnumLayer enumLayer = null;


            List<IFeatureLayer> pointLayers = null;

            ILayer layer = null;
            List<IFeatureLayer> lineLayers;
            List<MergeSplitGeoNetFeatures> m_Config = null;
            IPoint pSplitPnt = null;
            ISet pSet = null;
            IFeature pSplitResFeat = null;
            try
            {
                m_Config = ConfigUtil.GetMergeSplitConfig();


                if (SplitSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOffCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                }
                editor = Globals.getEditor(app);
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
                    return;
                }

                //Get enumeration of editable layers
                eLayers = (IEditLayers)editor;


                mxdoc = (IMxDocument)app.Document;
                map = editor.Map;

                int i = 0;
                int total = 0;

                //Get enumeration of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                // Create list of visible point layers with selected feature(s)
                pointLayers = new List<IFeatureLayer>();
                lineLayers = new List<IFeatureLayer>();
                enumLayer.Reset();
                layer = enumLayer.Next();
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                       && (fLayer.Visible))
                    {
                        fSel = fLayer as IFeatureSelection;
                        if (fSel.SelectionSet.Count > 0)
                        {
                            total += fSel.SelectionSet.Count;
                            pointLayers.Add(fLayer);
                        }
                    }
                    else if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        && (fLayer.Visible))
                    {
                        if (eLayers.IsEditable(fLayer))
                            lineLayers.Add(fLayer);
                    }
                    layer = enumLayer.Next();
                }


                //ProgressBar
                progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                // Create a CancelTracker
                trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                // Set the properties of the Step Progressor
                System.Int32 int32_hWnd = app.hWnd;
                stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                stepProgressor.MinRange = 0;
                stepProgressor.MaxRange = total;
                stepProgressor.StepValue = 1;
                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_10");

                // Create the ProgressDialog. This automatically displays the dialog
                progressDialog = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor;

                // Set the properties of the ProgressDialog
                progressDialog.CancelEnabled = true;
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_11") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriDownloadFile;

                //Create an edit operation enabling undo/redo
                editor.StartOperation();
                ICursor pointCursor = null;
                IFeatureSelection pointSel = null;
                IFeature pointFeature = null;
                ISpatialFilter sFilter = null;
                IFeatureSelection lineSel = null;
                IFeatureCursor lineCursor = null;
                IFeature lineFeature = null;
                //IFeatureEdit2 featureEdit = null;

                //Determine if point is at end of this line (if so skip)
                //ITopologicalOperator topoOp = null;
                //IPolygon poly = null;
                //IRelationalOperator relOp = null;
                //ICurve curve = null;




                try
                {
                    // step through all points and split the lines that intersect them

                    foreach (IFeatureLayer pointLayer in pointLayers)
                    {
                        pointSel = pointLayer as IFeatureSelection;
                        pointSel.SelectionSet.Search(null, false, out pointCursor);
                        pointFeature = pointCursor.NextRow() as IFeature;
                        while (!(pointFeature == null))
                        {
                            foreach (IFeatureLayer lineLayer in lineLayers)
                            {
                                sFilter = new SpatialFilterClass();
                                sFilter.Geometry = pointFeature.ShapeCopy;
                                sFilter.GeometryField = lineLayer.FeatureClass.ShapeFieldName;
                                sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                                lineSel = lineLayer as IFeatureSelection;
                                lineCursor = lineLayer.Search(sFilter, false);
                                lineFeature = lineCursor.NextFeature();

                                IList<MergeSplitFlds> pFldsNames = new List<MergeSplitFlds>();

                                string strFormValu = "{0:0.##}";
                                if (m_Config.Count > 0)
                                {
                                    foreach (A4LGSharedFunctions.Field FldNam in m_Config[0].Fields)
                                    {
                                        int idx = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, FldNam.Name);
                                        if (idx > -1)
                                            pFldsNames.Add(new MergeSplitFlds(FldNam.Name, idx, "", FldNam.MergeRule, FldNam.SplitRule));


                                    }
                                    strFormValu = m_Config[0].SplitFormatString;
                                }

                                while (!(lineFeature == null))
                                {
                                    Globals.splitLineWithPoint(lineFeature, pointFeature.Shape as IPoint, SplitAtLocationSnap, pFldsNames, strFormValu,app);

                                    //featureEdit = lineFeature as IFeatureEdit2;

                                    ////Determine if point is at end of this line (if so skip)
                                    //topoOp = pointFeature.Shape as ITopologicalOperator;
                                    //poly = topoOp.Buffer(SkipDistance) as IPolygon;
                                    //relOp = poly as IRelationalOperator;
                                    //curve = lineFeature.Shape as ICurve;
                                    //if (!(relOp.Contains(curve.FromPoint)) &
                                    //    !(relOp.Contains(curve.ToPoint)))
                                    //{
                                    //     pSplitPnt = pointFeature.ShapeCopy as IPoint;
                                    //     double dblHighVal = 0;
                                    //     double dblLowVal = 0;
                                    //     int intHighIdx = -1;
                                    //     int intLowIdx = -1;
                                    //    foreach (MergeSplitFlds FldNam in pFldsNames)
                                    //    {
                                    //        FldNam.Value= lineFeature.get_Value(FldNam.FieldIndex).ToString();
                                    //        if (FldNam.SplitType.ToUpper() == "MAX")
                                    //        {
                                    //            if (FldNam.Value != null)
                                    //            {
                                    //                if (FldNam.Value != "")
                                    //                {

                                    //                    dblHighVal = Convert.ToDouble(FldNam.Value);
                                    //                    intHighIdx = FldNam.FieldIndex;
                                    //                }
                                    //            }
                                    //        }
                                    //        else if (FldNam.SplitType.ToUpper() == "MIN")
                                    //        {
                                    //            if (FldNam.Value != null)
                                    //            {
                                    //                if (FldNam.Value != "")
                                    //                {

                                    //                    dblLowVal = Convert.ToDouble(FldNam.Value);
                                    //                    intLowIdx = FldNam.FieldIndex;
                                    //                }
                                    //            }
                                    //        }


                                    //    }
                                    //    if (intHighIdx > -1 && intLowIdx > -1)
                                    //    {
                                    //        double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

                                    //        double splitDist = Globals.PointDistanceOnLine(pSplitPnt, lineFeature.Shape as IPolyline, 2, out pSplitPnt);
                                    //        double percentSplit = splitDist / len;
                                    //        double dblMidVal;
                                    //        if (m_Config[0].SplitFormatString == "")
                                    //        {
                                    //            dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
                                    //        }
                                    //        else
                                    //        {
                                    //            dblMidVal = Convert.ToDouble(string.Format(m_Config[0].SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

                                    //        }


                                    //        //Split feature
                                    //        pSet = featureEdit.SplitWithUpdate(pSplitPnt);

                                    //        if (pSet.Count == 1)
                                    //        {
                                    //            while ((pSplitResFeat = pSet.Next() as IFeature) != null)
                                    //            {
                                    //                if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pSplitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pSplitPnt.Y)
                                    //                {
                                    //                    pSplitResFeat.set_Value(intHighIdx, dblMidVal);
                                    //                }
                                    //                else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pSplitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pSplitPnt.Y)
                                    //                {
                                    //                    pSplitResFeat.set_Value(intLowIdx, dblMidVal);

                                    //                }
                                    //            }

                                    //        }
                                    //        if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pSplitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pSplitPnt.Y)
                                    //        {
                                    //            lineFeature.set_Value(intHighIdx, dblMidVal);
                                    //        }
                                    //        else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pSplitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pSplitPnt.Y)
                                    //        {
                                    //            lineFeature.set_Value(intLowIdx, dblMidVal);

                                    //        }
                                    //    }else
                                    //        pSet = featureEdit.SplitWithUpdate(pSplitPnt);


                                    //    pSplitPnt = null;
                                    //}

                                    lineFeature = lineCursor.NextFeature();
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);

                            }

                            //Update progress bar
                            i += 1;
                            progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_11") + i.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + total.ToString() + ".";
                            stepProgressor.Step();
                            ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                            statusBar.set_Message(0, i.ToString());

                            //Check if the cancel button was pressed. If so, stop process
                            if (!trackCancel.Continue())
                            {
                                break;
                            }
                            pointFeature = (IFeature)pointCursor.NextRow();
                        }

                    }
                }
                catch (Exception ex)
                {
                    editor.AbortOperation();
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
                    return;
                }
                finally
                {
                    if (pointCursor != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pointCursor);
                    }
                    pointCursor = null;
                    pointSel = null;
                    pointFeature = null;
                    sFilter = null;
                    lineSel = null;
                    lineCursor = null;
                    lineFeature = null;
                    //featureEdit = null;

                    //topoOp = null;
                    //poly = null;
                    //relOp = null;
                    //curve = null;
                }
                progressDialog.HideDialog();


                //Stop the edit operation 
                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_9"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_5"));
                return;
            }
            finally
            {

                pSet = null;
                pSplitPnt = null;
                if (SplitSuspendAA.ToUpper() == "TRUE")
                {
                    pCmd = Globals.GetCommand("ArcGIS4LocalGovernment_AttributeAssistantSuspendOnCommand", app);
                    if (pCmd != null)
                    {
                        pCmd.Execute();
                    }
                    pCmd = null;

                }
                if (progressDialog != null)
                    progressDialog.HideDialog();
                progressDialogFactory = null;
                trackCancel = null;
                stepProgressor = null;

                progressDialog = null;

                pCmd = null;
                eLayers = null;
                editor = null;
                mxdoc = null;
                map = null;

                fLayer = null;
                fSel = null;

                geoFeatureLayerID = null;

                enumLayer = null;


                pointLayers = null;

                layer = null;
                lineLayers = null;
                pSplitResFeat = null;
            }
        }
      
        public static void CreateJumps(IApplication app, JumpTypes jumpType, double jumpDistance)
        {
            IProgressDialog2 progressDialog = default(IProgressDialog2);

            IProgressDialogFactory progressDialogFactory = null;
            ITrackCancel trackCancel = null;
            IStepProgressor stepProgressor = null;
            ICursor lineCursor = null;
            IEditor editor = null;
            IFeature lineFeature = null;

            IMxDocument mxdoc = null;
            IMap map = null;


            UID geoFeatureLayerID = null;
            IEnumLayer enumLayer = null;


            IEditLayers eLayers = null;


            List<IFeatureLayer> lineLayers = null;
            ILayer layer = null;
            IFeatureLayer fLayer = null;
            IFeatureSelection fSel = null;
            try
            {

                //Get editor
                editor = Globals.getEditor(app);
                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_6"));
                    return;
                }

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
                progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_12") +"...";
                progressDialog.Title = A4LGSharedFunctions.Localizer.GetString("GeometryToolsProc_12");
                progressDialog.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                mxdoc = (IMxDocument)app.Document;
                map = editor.Map;

                //Get enumeration of feature layers
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                //Get enumeration of editable layers
                eLayers = (IEditLayers)editor;

                // Create list of visible line layers with selected feature(s)
                lineLayers = new List<IFeatureLayer>();
                enumLayer.Reset();
                layer = enumLayer.Next();
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_2") + fLayer.Name;
                    if (fLayer.Valid && (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                       && (fLayer.Visible))
                    {
                        if (eLayers.IsEditable(fLayer))
                        {

                            fSel = fLayer as IFeatureSelection;
                            if (fSel.SelectionSet.Count > 0)
                            {
                                stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_3") + fLayer.Name;
                                lineLayers.Add(fLayer);
                            }
                        }
                    }
                    layer = enumLayer.Next();
                }
                if (lineLayers.Count == 0)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_7"));
                    return;

                }


                // Create an edit operation enabling undo/redo
                editor.StartOperation();

                IPointCollection linePointCollection = null;
                ISpatialFilter spatialFilter = null;
                IFeatureCursor featureCursor = null;
                IFeature crossFeature = null;

                ITopologicalOperator topologicalOP = null;
                ITopologicalOperator topologicalOP2 = null;
                IPointCollection intersectPointCollection = null;

                IPoint intersectPoint = null;
                IPoint outPoint = null;
                IPoint beginPoint = null;
                IPoint endPoint = null;
                IActiveView activeView = null;
                IInvalidArea invalid = null;

                IEnvelope ext = null;
                ISegmentCollection testSegmentColl = null;
                ISegment testSegment = null;


                ISegmentCollection segmentColl = null;
                ISegmentCollection segmentCollNew = null;
                IConstructCircularArc constructCircArc = null;
                ISegment curveSegment = null;
                //IProximityOperator proximityOP = null;
                IPolycurve3 selCurve = null;
                IPolycurve3 selCurveB = null;
                IPolycurve3 crossCurve = null;
                IPolycurve3 crossCurveB = null;
                IPolycurve3 testSelCurve = null;
                IPolycurve3 testSelCurveB = null;


                object _missing = null;

                IFeatureSelection lineSel = null;
                ISelectionSet2 sel = null;
                IZAware pZAware = null;
                ISegmentCollection testSegmentColl2 = null;
                ISegment testSegment2 = null;


                IGeometryDef pGeometryDefTest = null;

                IFields pFieldsTest = null;

                IField pFieldTest = null;
                IGeoDataset pDS = null;

                try
                {
                    activeView = map as IActiveView;
                    invalid = new InvalidAreaClass();
                    invalid.Display = editor.Display;

                    linePointCollection = (IPointCollection)new PolylineClass();
                    spatialFilter = new SpatialFilterClass();
                    intersectPoint = new PointClass();
                    outPoint = new PointClass();

                    //used for curve test on cross feature
                    bool testProjectOnto;
                    bool testCreatePart;
                    bool testSplitHappened;
                    int testNewPartIndex;
                    int testNewSegmentIndex;

                    //used for curve test on selected feature
                    bool testProjectOnto2;
                    bool testCreatePart2;
                    bool testSplitHappened2;
                    int testNewPartIndex2;
                    int testNewSegmentIndex2;

                    //ICurve lineCurve;
                    double totalDistance;
                    double distAlongCurve = 0;
                    double distFromCurve = 0;
                    bool boolRightSide = false;

                    //test the amount of needed space for the inserted curve
                    double remainderDistance = 0;

                    double pointDistance = 0;

                    bool projectOnto;
                    bool createPart;
                    bool splitHappenedBefore;
                    bool splitHappenedAfter;
                    int newPartIndexBegin;
                    int newSegmentIndexBegin;
                    int newPartIndexEnd;
                    int newSegmentIndexEnd;
                    _missing = Type.Missing;


                    // Step through all line layers with selection sets                
                    foreach (IFeatureLayer lineLayer in lineLayers)
                    {
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_4a") + lineLayer.Name;
                        // Get cursor of selected lines 
                        lineSel = (IFeatureSelection)lineLayer;
                        sel = lineSel.SelectionSet as ISelectionSet2;
                        sel.Search(null, false, out lineCursor);

                        // Process each selected line
                        int idx = 0;
                        while ((lineFeature = (IFeature)lineCursor.NextRow()) != null)
                        {
                            idx++;
                            stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_4b") + idx + A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_4c") + lineLayer.Name;
                            if (lineFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                            {
                                selCurve = (IPolycurve3)lineFeature.ShapeCopy;
                                pZAware = selCurve as IZAware;
                                if (pZAware.ZAware)
                                {
                                    pZAware.ZAware = false;
                                }

                                // Get cursor of crossing features
                                spatialFilter.Geometry = selCurve;
                                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                                featureCursor = lineLayer.Search(spatialFilter, false);

                                // Process each crossing feature
                                while ((crossFeature = (IFeature)featureCursor.NextFeature()) != null)
                                {
                                    topologicalOP = (ITopologicalOperator)crossFeature.Shape;
                                    //topologicalOP2 = (ITopologicalOperator)polylinePointCollection;
                                    topologicalOP2 = (ITopologicalOperator)selCurve;

                                    topologicalOP2.Simplify();
                                    topologicalOP.Simplify();
                                    intersectPointCollection = (IPointCollection)topologicalOP.Intersect((IGeometry)selCurve, esriGeometryDimension.esriGeometry0Dimension);

                                    for (int i = 0; i < intersectPointCollection.PointCount; i++)
                                    {
                                        intersectPoint = intersectPointCollection.get_Point(i);

                                        //Determine if the crossing segement is an arc.
                                        //Continue if it is not.
                                        testProjectOnto = true;
                                        testCreatePart = false;
                                        crossCurve = (IPolycurve3)crossFeature.ShapeCopy;
                                        crossCurve.SplitAtPoint(intersectPoint, testProjectOnto, testCreatePart, out testSplitHappened, out testNewPartIndex, out testNewSegmentIndex);
                                        crossCurveB = (IPolycurve3)crossFeature.ShapeCopy;
                                        testSegmentColl = crossCurveB as ISegmentCollection;
                                        testSegment = testSegmentColl.get_Segment(testNewSegmentIndex - 1);
                                        if (testSegment.GeometryType != esriGeometryType.esriGeometryCircularArc)
                                        {
                                            //Determine if the current location of the selected line is an arc.
                                            //Continue if it is not.
                                            testProjectOnto2 = true;
                                            testCreatePart2 = false;
                                            testSelCurve = (IPolycurve3)lineFeature.ShapeCopy;
                                            testSelCurve.SplitAtPoint(intersectPoint, testProjectOnto2, testCreatePart2, out testSplitHappened2, out testNewPartIndex2, out testNewSegmentIndex2);
                                            testSelCurveB = (IPolycurve3)lineFeature.ShapeCopy;

                                            testSegmentColl2 = testSelCurveB as ISegmentCollection;
                                            testSegment2 = testSegmentColl2.get_Segment(testNewSegmentIndex2 - 1);
                                            if (testSegment2.GeometryType != esriGeometryType.esriGeometryCircularArc)
                                            {
                                                //Find distance along the original selected line
                                                //focusPolyline = (IPolyline)lineFeature.ShapeCopy;
                                                selCurveB = (IPolycurve3)lineFeature.ShapeCopy;
                                                selCurveB.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, intersectPoint, false, outPoint, ref distAlongCurve, ref distFromCurve, ref boolRightSide);
                                                remainderDistance = selCurveB.Length - distAlongCurve;
                                                totalDistance = distAlongCurve + jumpDistance;


                                                //Continue if there is enough room
                                                if ((selCurveB.Length >= (jumpDistance / 2)) && (remainderDistance >= (jumpDistance / 2)))
                                                {
                                                    //find the points where the curve will begin and end
                                                    beginPoint = new PointClass();
                                                    endPoint = new PointClass();
                                                    selCurveB.QueryPoint(esriSegmentExtension.esriNoExtension, (distAlongCurve - (jumpDistance / 2)), false, beginPoint);
                                                    selCurveB.QueryPoint(esriSegmentExtension.esriNoExtension, (distAlongCurve + (jumpDistance / 2)), false, endPoint);

                                                    //split the original line at the two points (vertices for begin and end of new curve)
                                                    projectOnto = true;
                                                    createPart = false;
                                                    selCurveB.SplitAtPoint(beginPoint, projectOnto, createPart, out splitHappenedBefore, out newPartIndexBegin, out newSegmentIndexBegin);
                                                    selCurveB.SplitAtPoint(endPoint, projectOnto, createPart, out splitHappenedAfter, out newPartIndexEnd, out newSegmentIndexEnd);

                                                    if ((splitHappenedBefore = true) && (splitHappenedAfter = true))
                                                    {
                                                        //Create the curve segment and add it to the polyline
                                                        constructCircArc = new CircularArcClass();
                                                       // proximityOP = (IProximityOperator)intersectPoint;
                                                        //pointDistance = proximityOP.ReturnDistance(beginPoint);
                                                        pointDistance = jumpDistance;

                                                        //check for direction of line to always make the jump on top
                                                        if (jumpType == JumpTypes.Over)
                                                            if (endPoint.X > beginPoint.X)
                                                            {
                                                                try
                                                                {
                                                                    constructCircArc.ConstructChordDistance(intersectPoint, beginPoint, false, (pointDistance));
                                                                }
                                                                catch
                                                                {
                                                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_8"));

                                                                    continue;
                                                                }
                                                            }

                                                            else
                                                            {
                                                                try
                                                                {
                                                                    constructCircArc.ConstructChordDistance(intersectPoint, beginPoint, true, (pointDistance));
                                                                }
                                                                catch
                                                                {
                                                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_8"));
                                                                    continue;
                                                                }
                                                            }
                                                        else
                                                        {
                                                            if (endPoint.X <= beginPoint.X)
                                                            {
                                                                try
                                                                {
                                                                    constructCircArc.ConstructChordDistance(intersectPoint, beginPoint, false, (pointDistance));
                                                                }
                                                                catch
                                                                {
                                                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_8"));
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    constructCircArc.ConstructChordDistance(intersectPoint, beginPoint, true, (pointDistance));
                                                                }
                                                                catch
                                                                {
                                                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_8"));
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                        curveSegment = constructCircArc as ISegment;
                                                        if (curveSegment != null & curveSegment.Length > 0)
                                                        {
                                                            segmentCollNew = (ISegmentCollection)new PolylineClass();
                                                            segmentCollNew.AddSegment(curveSegment, ref _missing, ref _missing);
                                                            segmentColl = (ISegmentCollection)selCurveB;
                                                            segmentColl.ReplaceSegmentCollection(newSegmentIndexBegin, 1, segmentCollNew);





                                                            string sShpName = lineLayer.FeatureClass.ShapeFieldName;

                                                            pFieldsTest = lineLayer.FeatureClass.Fields;
                                                            int lGeomIndex = pFieldsTest.FindField(sShpName);

                                                            pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                                                            pGeometryDefTest = pFieldTest.GeometryDef;
                                                            bool bZAware;
                                                            bool bMAware;
                                                            //Determine if M or Z aware
                                                            bZAware = pGeometryDefTest.HasZ;
                                                            bMAware = pGeometryDefTest.HasM;

                                                            if (bZAware)
                                                            {

                                                                // IGeometry pGeo = new PolylineClass();
                                                                pZAware = selCurveB as IZAware;
                                                                if (pZAware.ZAware)
                                                                {

                                                                }
                                                                else
                                                                {
                                                                    pZAware.ZAware = true;

                                                                    //pZAware.DropZs();
                                                                }
                                                                // pZAware.DropZs();
                                                                IZ pZ = selCurveB as IZ;
                                                                pZ.SetConstantZ(0);
                                                            }
                                                            if (bMAware)
                                                            {

                                                            }
                                                            pDS = lineLayer.FeatureClass as IGeoDataset;

                                                            selCurveB.SpatialReference = pDS.SpatialReference;

                                                            lineFeature.Shape = (IGeometry)selCurveB;
                                                            lineFeature.Store();

                                                            //Prepare to redraw area around feature
                                                            ext = curveSegment.Envelope;
                                                            ext.Expand(2, 2, true);
                                                            invalid.Add(ext);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_9"));
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    editor.AbortOperation();
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_6") + "\n" + ex.Message, ex.Source);
                }

                finally
                {

                    // Refresh
                    if (invalid!= null)
                        invalid.Invalidate((short)esriScreenCache.esriAllScreenCaches);

                    linePointCollection = null;
                    spatialFilter = null;
                    if (featureCursor != null)
                        Marshal.ReleaseComObject(featureCursor);
                    featureCursor = null;
                    crossFeature = null;

                    topologicalOP = null;
                    topologicalOP2 = null;
                    intersectPointCollection = null;

                    intersectPoint = null;
                    outPoint = null;
                    beginPoint = null;
                    endPoint = null;
                    activeView = null;
                    invalid = null;

                    ext = null;
                    testSegmentColl = null;
                    testSegment = null;


                    segmentColl = null;
                    segmentCollNew = null;
                    constructCircArc = null;
                    curveSegment = null;
                    //proximityOP = null;
                    selCurve = null;
                    selCurveB = null;
                    crossCurve = null;
                    crossCurveB = null;
                    testSelCurve = null;
                    testSelCurveB = null;


                    _missing = null;

                    lineSel = null;
                    sel = null;
                    pZAware = null;
                    testSegmentColl2 = null;
                    testSegment2 = null;


                    pGeometryDefTest = null;

                    pFieldsTest = null;

                    pFieldTest = null;
                    pDS = null;


                }

                // Stop the edit operation 
                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_6"));

              
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_6") + "\n" + ex.Message, ex.Source);
                return;
            }
            finally
            {
                if (lineCursor != null)
                {
                    Marshal.ReleaseComObject(lineCursor);
                }
                // Cleanup
                if (progressDialog != null)
                {
                    progressDialog.HideDialog();

                    //progressDialog = null;
                    Marshal.ReleaseComObject(progressDialog);
                    //progressDialogFactory = null;
                    //Marshal.ReleaseComObject(progressDialogFactory);
                    //trackCancel = null;
                    //Marshal.ReleaseComObject(trackCancel);
                    //stepProgressor = null;
                    //Marshal.ReleaseComObject(stepProgressor);
                    //appCursor = null;
                }

                progressDialog = null;

                progressDialogFactory = null;
                trackCancel = null;
                stepProgressor = null;
                lineCursor = null;
                editor = null;
                lineFeature = null;

                mxdoc = null;
                map = null;


                geoFeatureLayerID = null;
                enumLayer = null;
                eLayers = null;


                lineLayers = null;
                layer = null;
                fLayer = null;
                fSel = null;

            }
        }
        public static void FlipLines(IApplication app, FlipTypes FlipType)
        {
            IEditor editor = null;
            IMap map = null;


            IMxDocument mxdoc = null;
            ILayer layer = null;
            //Get the Feature layer and feature class
            IFeatureClass fc = null;
            IFeatureSelection fSel = null;
            IFeatureLayer fLayer = null;

            IGeometricNetwork gnet = null;
            IUtilityNetwork unet = null;
            INetworkClass netFC = null;
            INetElements netelems = null;

            try
            {

                editor = Globals.getEditor(app);
                if (editor == null)
                {
                    return;
                }

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7"));
                    return;
                }

                // Verify that there are layers in the table on contents
                map = editor.Map;
                if (map.LayerCount < 1)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_1"));
                    return;
                }

                //Get highlighted layer in the TOC
                mxdoc = app.Document as IMxDocument;
                layer = mxdoc.SelectedLayer as ILayer;
                if (layer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_6") + Environment.NewLine +
                                     A4LGSharedFunctions.Localizer.GetString("GeometryToolsMess_5"));
                    return;
                }

                //Verify that it is a feature layer
                fLayer = layer as IFeatureLayer;
                if (fLayer == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_1"));
                    return;
                }

                //Get the Feature layer and feature class
                fc = fLayer.FeatureClass;
                fSel = fLayer as IFeatureSelection;

                //Verify that it is a line layer
                if (fc.ShapeType != esriGeometryType.esriGeometryPolyline)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_2"));
                    return;
                }
                //Verify that the layer is part of a geometric network if using the establish flow by AnicillaryRole

                netFC = fc as INetworkClass;

                if (FlipType == FlipTypes.FlipLinesToMatchFlow && netFC == null)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("TOC_3"));
                    return;
                }

                //Get network for this feature class
                if (netFC != null)
                {
                    gnet = netFC.GeometricNetwork;
                    unet = gnet.Network as IUtilityNetwork;
                    netelems = unet as INetElements;
                }

                //Verify it has some features selected
                if (fSel.SelectionSet.Count < 1)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsError_4"));
                    return;
                }


                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg"), A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7"));
                    return;
                }

                // Create an edit operation enabling undo/redo
                try
                {
                    editor.StartOperation();
                }
                catch
                {
                    editor.AbortOperation();
                    editor.StartOperation();
                }
                IEnumIDs enumIds = null;

                IFeature feat = null;
                ICurve curve = null;
                INetworkFeature netFeat = null;
                IEdgeFeature edgeFeat = null;
                IComplexNetworkFeature complexNetFeat = null;
                ISimpleEdgeFeature simpleEdgeFeat = null;
                int iEID;
                IEnumNetEID enumNetEID = null;
                IEnumNetEID enumNetEID2 = null;
                IComplexEdgeFeature complexEdgeFeat = null;
                try
                {
                    enumIds = fSel.SelectionSet.IDs;
                    enumIds.Reset();

                    for (int id = enumIds.Next(); id > -1; id = enumIds.Next())
                    {
                        feat = fc.GetFeature(id);
                        curve = feat.ShapeCopy as ICurve;

                        if (FlipType == FlipTypes.FlipLines)
                        {
                            curve.ReverseOrientation();
                            feat.Shape = curve;
                            feat.Store();
                        }
                        else
                        {
                            netFeat = feat as INetworkFeature;
                            edgeFeat = feat as IEdgeFeature;
                            complexNetFeat = feat as IComplexNetworkFeature;
                            simpleEdgeFeat = feat as ISimpleEdgeFeature;

                            //Get the first (or only) element id for this feature
                            iEID = -1101;
                            complexNetFeat = edgeFeat as IComplexNetworkFeature;
                            complexEdgeFeat = edgeFeat as IComplexEdgeFeature;
                            if (complexNetFeat != null)
                            {

                                enumNetEID = netelems.GetEIDs(feat.Class.ObjectClassID, feat.OID, esriElementType.esriETEdge);
                                enumNetEID.Reset();
                                iEID = enumNetEID.Last();
                            }
                            else
                                iEID = simpleEdgeFeat.EID;


                            //If this element is digitized against the flow...
                            if ((iEID > -1) && (unet.GetFlowDirection(iEID) == esriFlowDirection.esriFDAgainstFlow))
                            {
                                curve.ReverseOrientation();
                                feat.Shape = curve;
                                feat.Store();

                                //Set flow to uninitalized...
                                //for each element of a complex edge
                                enumNetEID2 = netelems.GetEIDs(feat.Class.ObjectClassID, feat.OID, esriElementType.esriETEdge);
                                enumNetEID2.Reset(); int cid = 0;
                                cid = enumNetEID2.Next();
                                while (cid > 0)
                                {
                                    unet.SetFlowDirection(cid, esriFlowDirection.esriFDWithFlow);
                                    cid = enumNetEID2.Next();
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    editor.AbortOperation();
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7") + "\n" + ex.Message, ex.Source);
                }
                finally
                {

                    enumIds = null;

                    feat = null;
                    curve = null;
                    netFeat = null;
                    edgeFeat = null;
                    complexNetFeat = null;
                    simpleEdgeFeat = null;

                    enumNetEID = null;
                    enumNetEID2 = null;
                    complexEdgeFeat = null;
                }
                // Stop the edit operation 
                if (FlipType == FlipTypes.FlipLines)
                    editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7"));
                else
                    editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_8"));

                // object Missing = Type.Missing;
                //mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,Missing, mxdoc.ActiveView.Extent);
                mxdoc.ActiveView.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7") + "\n" + ex.Message, A4LGSharedFunctions.Localizer.GetString("GeometryToolsLbl_7"));
                return;
            }
            finally
            {
                editor = null;
                map = null;


                mxdoc = null;
                layer = null;

                fc = null;
                fSel = null;
                fLayer = null;

                gnet = null;
                unet = null;
                netFC = null;
                netelems = null;

            }

        }
        public static void showMergeDialog(IApplication app)
        {

            IEditor editor = null;
            frmMergeGNLines mergeLines = null;
            
            try 
            {
                

                editor = Globals.getEditor(app);
                mergeLines = new frmMergeGNLines(app, editor);
                if (mergeLines.loadDialog())
                    mergeLines.Show(new Globals.WindowWrapper((IntPtr)app.hWnd));
            }
            catch 
            { 
            
            }
            finally 
            {
                editor = null;
                mergeLines = null;
            }
                


        }
    }
 
}
