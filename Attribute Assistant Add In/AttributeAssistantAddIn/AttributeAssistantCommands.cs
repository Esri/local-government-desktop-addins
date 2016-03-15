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

using System.Diagnostics;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Reflection;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using A4LGSharedFunctions;


namespace ArcGIS4LocalGovernment
{
    public class MonitorAA : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public MonitorAA()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {
            if (AAState._Suspend == true)
            { }

        }


    }
    public class AttributeAssistantLoadLastValue : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantLoadLastValue()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            AAState.promptLastValueProperrtySetOneForm();

        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantToggleCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor m_Editor;

        public AttributeAssistantToggleCommand()
        {
            ConfigUtil.type = "aa";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            try
            {


                if (AAState.PerformUpdates)
                {
                    AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1a"));
                    AAState.PerformUpdates = false;

                    AAState.unInitEditing();
                }
                else
                {
                    AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1b"));
                    AAState.PerformUpdates = true;
                    AAState.initEditing();
                }
            }
            catch
            { }

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {
            try
            {
                AAState.setIcon();
            }
            catch
            { }
        }

    }
    public class AttributeAssistantSuspendCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendCommand()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            if (AAState._Suspend)
            {
                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1c"));
                AAState._Suspend = false;

            }
            else
            {

                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1a"));
                AAState._Suspend = true;

            }


        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantSuspendOnCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendOnCommand()
        {

            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            AAState._Suspend = false;
            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1d"));


        }
        protected override void Dispose(bool value)
        {
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantSuspendOffCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendOffCommand()
        {

            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            AAState._Suspend = true;
            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1e"));


        }
        protected override void Dispose(bool value)
        {
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class RunChangeRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunChangeRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunChangeRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunChangeRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }


                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {


                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;






                        ran = true;


                        editor.StartOperation();


                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {


                            //   AAState.StopChangeMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;

                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {


                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {
                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;





                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureChange(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }


                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }
                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }

                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunChangeRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_2a"));
                            }
                            catch (Exception ex)
                            { }

                        }







                        editor.StartOperation();
                        try
                        {
                            AAState.StopChangeMonitor();
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {

                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {
                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;

                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureChange(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);

                                        pRow = null;


                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunChangeRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {
                            AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_2a"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorWarn_14a") + " " + ex.Message);

                            }

                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunChangeRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //  MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class RunChangeGeoRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunChangeGeoRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunChangeGeoRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunChangeGeoRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                long featCount = map.SelectionCount;
                int totalCount = Convert.ToInt32(featCount);


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {



                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;



                        ran = true;


                        editor.StartOperation();

                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            //   AAState.StopChangeMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {



                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());



                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureGeoChange(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }


                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunChangeRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation("Run Change Geo Rules - Features");
                            }
                            catch (Exception ex)
                            { }

                        }


                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;




                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunChangeGeoRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class RunManualRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunManualRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunManualRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }
        public void RunManualRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }

                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {


                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_3a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {


                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = "Running Manual Rules";

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;

                        editor.StartOperation();


                        ran = true;


                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {

                                        // test = true;

                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4a"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunManualRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {


                        }








                        try
                        {
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {
                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;

                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4a"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);


                                    }
                                }
                            }
                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunManualRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {


                        }
                        try
                        {
                            editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_4a"));

                        }
                        catch
                        {


                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunManualRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }

    }
    public class RunCreateRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunCreateRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunCreateRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunCreateRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }


                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_4a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {



                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;

                        ran = true;
                        editor.StartOperation();

                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            //   AAState.StopCreateMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {


                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        feat = (IFeature)fCursor.NextFeature();
                                        while (feat != null)
                                        {


                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());


                                            lastOID = feat.OID;
                                            lastLay = fLayer.Name;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5a"));
                                            AAState._editEvents.OnCreateFeature -= AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                //AAState.FeatureCreate(pObj);
                                                Debug.WriteLine("Feature with " + feat.OID);
                                                Debug.WriteLine(progressVal.ToString() + " : Count is " + fSel.SelectionSet.Count);
                                                //feat.Shape = feat.Shape;
                                                //feat.set_Value(0, feat.get_Value(0));
                                               
                                                AAState.FeatureCreate(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnCreateFeature += AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                            feat = (IFeature)fCursor.NextFeature();
                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }

                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunCreateRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartCreateMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_5a"));
                            }
                            catch (Exception ex)
                            { }

                        }







                        editor.StartOperation();
                        try
                        {
                            AAState.StopChangeMonitor();
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {
                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());


                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;
                                            IObject pObj = pRow as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5a"));
                                            AAState._editEvents.OnCreateFeature -= AAState.FeatureCreate;


                                            try
                                            {
                                                AAState.FeatureCreate(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnCreateFeature += AAState.FeatureCreate;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);


                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunCreateRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {
                            AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_5a"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorWarn_14a") + " " + ex.Message);

                            }

                        }

                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunCreateRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //  MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class ShowConfigForm : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //internal static ESRI.ArcGIS.Framework.IDockableWindow s_dockWindow;
        ConfigForm m_ConfigForm;
        public ShowConfigForm()
        {
            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            //DockableWindow pDockWin = getDockableWindow() as DockableWindow;
            //if (pDockWin == null)
            //    return;
            //pDockWin.Show(!pDockWin.IsVisible());
            //string ConfigPath = ConfigUtil.generateUserCachePath();

            //string[] ConfigFiles = ConfigUtil.GetConfigFiles();
            ConfigUtil.type = "aa";
            m_ConfigForm = new ConfigForm("aa");
            m_ConfigForm.ShowDialog();

        }

        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }
        //private ESRI.ArcGIS.Framework.IDockableWindow getDockableWindow()
        //{

        //    // Only get/create the dockable window if they ask for it
        //    if (s_dockWindow == null)
        //    {
        //        UID dockWinID = new UID();
        //        dockWinID.Value = "A4WaterUtilities_ConfigDetails";
        //        s_dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
        //        //s_extension.UpdateSelCountDockWin()
        //    }

        //    return s_dockWindow;
        //}
    }

}