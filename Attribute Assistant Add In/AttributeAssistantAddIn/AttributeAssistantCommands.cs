/*
 | Version 10.2
 | Copyright 2013 Esri
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


        }

        protected override void OnClick()
        {


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

         
        }

        protected override void OnClick()
        {
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
            m_Editor = Globals.getEditor(ArcMap.Application);
   
        }
       
        protected override void OnClick()
        {

            if (AAState.PerformUpdates)
            {
                AAState.WriteLine("Attribute Assistant is being suspended");
                AAState.PerformUpdates= false;

                AAState.unInitEditing();
            }
            else
            {
                AAState.WriteLine("Attribute Assistant is activated ");
                AAState.PerformUpdates= true;
                AAState.initEditing();
            }
      

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {
            AAState.setIcon();
        }
   
    }
    public class AttributeAssistantSuspendCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendCommand()
        {


        }

        protected override void OnClick()
        {

            if (AAState._Suspend)
            {
                AAState.WriteLine("Attribute Assistant is being actived from suspended");
                AAState._Suspend = false;

            }
            else
            {

                AAState.WriteLine("Attribute Assistant is being suspended");
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


        }

        protected override void OnClick()
        {

            AAState._Suspend = false;
            AAState.WriteLine("Attribute Assistant is not suspended");


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


        }

        protected override void OnClick()
        {

            AAState._Suspend = true;
            AAState.WriteLine("Attribute Assistant is suspended");


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
            if (AAState.PerformUpdates== false)
            {
                MessageBox.Show("Please turn on the attribute assistant before using this tool");
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("Please start editing to run this tool");
                return;

            }
            RunChangeRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates== false)
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

                    if (MessageBox.Show("Are you sure you wish to apply attribute assistant Change rules for the selected " + totalCount + " rows and features?",
                        "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {

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
                                if (fLayer.Valid && eLayers.IsEditable(fLayer))//fLayer.Visible &&
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
                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;





                                            AAState.WriteLine("AA - Removed the Feature Change event to run the change rules");
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
                                            AAState.WriteLine("AA - Feature Change event readded after change rules");

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
                                editor.StopOperation("Run Change Rules - Features");
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
                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;

                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine("AA - Removed the Feature Change event to run the change rules");
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
                                            AAState.WriteLine("AA - Feature Change event readded after change rules");
                                         

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
                                editor.StopOperation("Run Change Rules - Features");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERROR TURNING ON THE AA, Restart ArcMap");

                            }

                        }

                    }

                }
                else
                {
                    MessageBox.Show("Please select some features or rows to run this process.");

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
                    MessageBox.Show("Process has completed successfully");

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
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show("Please turn on the attribute assistant before using this tool");
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("Please start editing to run this tool");
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
                int totalCount =  Convert.ToInt32(featCount);


                if (totalCount >= 1)
                {

                    if (MessageBox.Show("Are you sure you wish to apply attribute assistant Change rules for the selected " + totalCount + " rows and features?",
                        "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {

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
                                if (fLayer.Valid && eLayers.IsEditable(fLayer))//fLayer.Visible &&
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
                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine("AA - Removed the Feature Change event to run the change rules");
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
                                            AAState.WriteLine("AA - Feature Change event readded after change rules");

                                        

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







                    }

                }
                else
                {
                    MessageBox.Show("Please select some features or rows to run this process.");

                }

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
                    MessageBox.Show("Process has completed successfully");

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
            if (AAState.PerformUpdates== false)
            {
                MessageBox.Show("Please turn on the attribute assistant before using this tool");
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("Please start editing to run this tool");
                return;

            }
            RunManualRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates== false)
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
                    editor.StartOperation();

                    if (MessageBox.Show("Are you sure you wish to apply attribute assistant manual rules for the selected " + totalCount + " rows and features?",
                        "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
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
                                if (fLayer.Valid && eLayers.IsEditable(fLayer))//fLayer.Visible &&
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
                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;
                                            AAState.WriteLine("AA - Removed the Feature Change event to run the manual rules");
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine("AA - Feature Change event readded after manual rules");

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
                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;
                                           
                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine("AA - Removed the Feature Change event to run the manual rules");
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine("AA - Feature Change event readded after manual rules");
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
                            MessageBox.Show("RunManualRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {
                         

                        }
                        try
                        {
                            editor.StopOperation("Run Manual Rules - Features");

                        }
                        catch
                        {


                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please select some features or rows to run this process.");

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
                    MessageBox.Show("Process has completed successfully");

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
            if (AAState.PerformUpdates== false)
            {
                MessageBox.Show("Please turn on the attribute assistant before using this tool");
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("Please start editing to run this tool");
                return;

            }
            RunCreateRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates== false)
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

                    if (MessageBox.Show("Are you sure you wish to apply attribute assistant Create rules for the selected " + totalCount + " rows and features?",
                        "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {

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
                                if (fLayer.Valid && eLayers.IsEditable(fLayer))//fLayer.Visible &&
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

                                            lastOID = feat.OID;
                                            lastLay = fLayer.Name;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine("AA - Removed the Feature Change event to run the create rules");
                                            AAState._editEvents.OnCreateFeature -= AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureCreate(pObj);

                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnCreateFeature += AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine("AA - Feature Change event readded after create rules");


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
                            MessageBox.Show("RunCreateRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartCreateMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation("Run Create Rules - Features");
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
                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;
                                            IObject pObj = pRow as IObject;


                                            AAState.WriteLine("AA - Removed the Feature Change event to run the create rules");
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
                                            AAState.WriteLine("AA - Feature Change event readded after create rules");

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
                                editor.StopOperation("Run Create Rules - Features");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERROR TURNING ON THE AA, Restart ArcMap");

                            }

                        }

                    }

                }
                else
                {
                    MessageBox.Show("Please select some features or rows to run this process.");

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
                    MessageBox.Show("Process has completed successfully");

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
        }

        protected override void OnClick()
        {
            //DockableWindow pDockWin = getDockableWindow() as DockableWindow;
            //if (pDockWin == null)
            //    return;
            //pDockWin.Show(!pDockWin.IsVisible());
            //string ConfigPath = ConfigUtil.generateUserCachePath();

            //string[] ConfigFiles = ConfigUtil.GetConfigFiles();

            m_ConfigForm = new ConfigForm();
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