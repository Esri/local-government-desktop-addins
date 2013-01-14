/*
 | Version 10.1.1
 | Copyright 2012 Esri
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
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    public class EstablishFlowAncillary : ESRI.ArcGIS.Desktop.AddIns.Button
    {



        IEditor m_Editor = null;

        public EstablishFlowAncillary()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);


        }


        protected override void OnClick()
        {
            A4WaterUtilities.GeoNetTools.EstablishFlow(Globals.GNFlowDirection.AncillaryRole, ArcMap.Application);
        }

        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);
        }
    }
    public class EstablishFlowDigitized : ESRI.ArcGIS.Desktop.AddIns.Button
    {


        IEditor m_Editor = null;

        public EstablishFlowDigitized()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);



        }

        protected override void OnClick()
        {
            A4WaterUtilities.GeoNetTools.EstablishFlow(Globals.GNFlowDirection.Digitized, ArcMap.Application);
        }

        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);
        }
    }
    public class AddFlag : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


        public AddFlag()
        {
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {

            double snapVal = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);



            A4WaterUtilities.GeoNetTools.AddFlag(ArcMap.Document.CurrentLocation, ArcMap.Application, snapVal);



            base.OnMouseDown(arg);

        }

        protected override void OnUpdate()
        {
            try
            {

                Enabled = true;
            }
            catch { }
            finally
            {
                base.OnUpdate();
            }
        }

    }
    public class AddBarrier : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


        public AddBarrier()
        {

        }


        protected override void OnMouseDown(MouseEventArgs arg)
        {


            A4WaterUtilities.GeoNetTools.AddBarrier(ArcMap.Document.CurrentLocation, ArcMap.Application, ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0));



            base.OnMouseDown(arg);

        }

        protected override void OnUpdate()
        {
            try
            {
                Enabled = true;
            }
            catch { }
            finally
            {
                base.OnUpdate();
            }
        }

    }
    public class RemoveFlagBarrier : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


        public RemoveFlagBarrier()
        {


        }


        protected override void OnMouseDown(MouseEventArgs arg)
        {


            // IPoint point = ArcMap.Document.CurrentLocation;//ArcMap.Document.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            GeoNetTools.RemoveFlagBarrier(ArcMap.Document.CurrentLocation, ArcMap.Application, ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0));

            base.OnMouseDown(arg);

        }

        protected override void OnUpdate()
        {
            try
            {
                //if (m_Editor == null)
                //{
                //    Enabled = false;
                //    return;
                //}
                //if (m_Editor.EditState != esriEditState.esriStateEditing)
                //{
                //    Enabled = false;
                //    return;
                //}
                Enabled = true;
            }
            catch { }
            finally
            {
                base.OnUpdate();
            }
        }

    }
    public class ShowFlowArrows : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public ShowFlowArrows()
        {

        }

        protected override void OnClick()
        {
            A4WaterUtilities.GeoNetTools.ShowArrows(ArcMap.Application);
        }

        protected override void OnUpdate()
        {
            Enabled = true;

        }

    }
    public class ConnectSelected : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        IEditor m_Editor;

        public ConnectSelected()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            GeoNetTools.ConnectSelected(ArcMap.Application);
        }

        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);
        }

    }
    public class DisconnectSelected : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        IEditor m_Editor;

        public DisconnectSelected()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            GeoNetTools.DisconnectSelected(ArcMap.Application);
        }

        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;
        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);
        }

    }
    public class ConnectionChecker : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor = null;


        public ConnectionChecker()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            GeoNetTools.CheckConnections(ArcMap.Application, ConfigUtil.GetConfigValue("ConnectionChecker_CheckOnlyVisibleLayers", false));
        }

        protected override void OnUpdate()
        {


            if (m_Editor == null)
            {
                m_Editor = Globals.getEditor(ArcMap.Application);
                if (m_Editor == null)
                {

                    Enabled = false;
                    return;
                }

            }
            if (m_Editor.EditState == esriEditState.esriStateNotEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;
        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;

            base.Dispose(value);
        }
    }
    public class ClearTraceResults : ESRI.ArcGIS.Desktop.AddIns.Button
    {



        public ClearTraceResults()
        {

        }

        protected override void OnClick()
        {
            Globals.ClearSelected(ArcMap.Document.FocusMap, true);
            Globals.RemoveTraceGraphics(ArcMap.Document.FocusMap, true);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Flags);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Barries);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Results);


        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

    }
    public class ToggleOperableStatus : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


        IEditor m_Editor = null;

        public ToggleOperableStatus()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);
        }
        protected override void OnMouseDown(MouseEventArgs arg)
        {


            string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");

            string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");
            string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');

            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);

            IPoint point = ArcMap.Document.CurrentLocation;//ArcMap.Document.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            IEnvelope env = GeoNetTools.ToggleOperableStatus(ArcMap.Application, point, true,
                                                            ISOvalveFeatureLayerName, ISOsourceFeatureLayerName,
                                                            ISOoperableFieldNameValves, ISOoperableFieldNameSources,
                                                            ISOoperableValues, SnapTol);
            if (env != null)
                ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, env);
            base.OnMouseDown(arg);
            point = null;
            env = null;

        }
        protected override void OnUpdate()
        {
            try
            {
                if (m_Editor == null)
                {
                    Enabled = false;
                    return;
                }
                if (m_Editor.EditState != esriEditState.esriStateEditing)
                {
                    Enabled = false;
                    return;
                }
                Enabled = true;
            }
            catch { }
            finally
            {
                base.OnUpdate();
            }
        }

    }
    public class SelectByJunctionEdgeCount : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        frmSelectByJunctionCount _frmSelectJunc;

        public SelectByJunctionEdgeCount()
        {
            _frmSelectJunc = new frmSelectByJunctionCount(ArcMap.Application);

        }

        private void reloadOccured(object sender, EventArgs e)
        {

        }

        protected override void OnClick()
        {
            if (_frmSelectJunc == null)
            {
                _frmSelectJunc = new frmSelectByJunctionCount(ArcMap.Application);

            }

            _frmSelectJunc.LoadJunctionsFeatureLayers();
            _frmSelectJunc.Show(Globals.GetWindowFromHost(ArcMap.Application.hWnd));


        }



        protected override void OnUpdate()
        {


            Enabled = true;
        }

    }
    public class TraceDownstream : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


        public TraceDownstream()
        {

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            bool selectEdges = false;
            if (Control.ModifierKeys == Keys.Control)
                selectEdges = true;
            else
                selectEdges = false;
            IPoint point = ArcMap.Document.CurrentLocation;
            GeoNetTools.TraceFlow(ref point, ArcMap.Application, esriFlowMethod.esriFMDownstream, SnapTol, traceIndeterminate, selectEdges);


            point = null;
        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

    }
    public class TraceUpstream : ESRI.ArcGIS.Desktop.AddIns.Tool
    {



        public TraceUpstream()
        {


        }
        private void reloadOccured(object sender, EventArgs e)
        {

        }
        protected override void OnMouseDown(MouseEventArgs arg)
        {


            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            bool selectEdges = false;
            if (Control.ModifierKeys == Keys.Control)
                selectEdges = true;
            else
                selectEdges = false;
            IPoint point = ArcMap.Document.CurrentLocation;
            GeoNetTools.TraceFlow(ref point, ArcMap.Application, esriFlowMethod.esriFMUpstream, SnapTol, traceIndeterminate, selectEdges);


            point = null;


        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

    }
    public class TraceIsolation : ESRI.ArcGIS.Desktop.AddIns.Tool
    {




        public TraceIsolation()
        {


        }
        protected override void OnMouseDown(MouseEventArgs arg)
        {
            bool selectEdges = false;
            if (Control.ModifierKeys == Keys.Control)
                selectEdges = true;
            else
                selectEdges = false;
            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");
            string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");

            string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');
            string ISOvalveAddSQL = ConfigUtil.GetConfigValue("TraceIsolation_Valve_AddSQL", "");
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            string ClearFlagBeforeIso = ConfigUtil.GetConfigValue("TraceIsolation_ClearFlagsOnClick", "true");

            Globals.RemoveTraceGraphics(((IMxDocument)ArcMap.Application.Document).FocusMap, false);

            Globals.ClearSelected(ArcMap.Application, false);


            if (ClearFlagBeforeIso.ToUpper() == "TRUE")
            {
                Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Flags);


            }


            IPoint point = ArcMap.Document.CurrentLocation;
            string returnVal = GeoNetTools.TraceIsolation(new double[] { point.X }, new double[] { point.Y }, ArcMap.Application, ISOsourceFeatureLayerName, ISOvalveFeatureLayerName, ISOoperableFieldNameValves, ISOoperableFieldNameSources, SnapTol, true, ISOoperableValues, ISOvalveAddSQL, traceIndeterminate, true, selectEdges, "", "", "");
            if (returnVal != null)
            {
                string[] retVals = returnVal.Split('_');

                switch (retVals.Length)
                {
                    case 1:
                        break;
                    case 2:
                        MessageBox.Show(retVals[1]);
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
            }
            point = null;
        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);


        }

    }
    public class TraceIsolationRerun : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public TraceIsolationRerun()
        {


        }
        protected override void OnClick()
        {
            bool selectEdges = false;
            if (Control.ModifierKeys == Keys.Control)
                selectEdges = true;
            else
                selectEdges = false;
            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");
            string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");

            string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');
            string ISOvalveAddSQL = ConfigUtil.GetConfigValue("TraceIsolation_Valve_AddSQL", "");
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            string ClearFlagBeforeIso = ConfigUtil.GetConfigValue("TraceIsolation_ClearFlagsOnClick", "true");

            Globals.RemoveTraceGraphics(((IMxDocument)ArcMap.Application.Document).FocusMap, false);

            Globals.ClearSelected(ArcMap.Application, false);



            //IPoint point = ArcMap.Document.CurrentLocation;
            string returnVal = GeoNetTools.TraceIsolation(null, null, ArcMap.Application, ISOsourceFeatureLayerName, ISOvalveFeatureLayerName, ISOoperableFieldNameValves, ISOoperableFieldNameSources, SnapTol, true, ISOoperableValues, ISOvalveAddSQL, traceIndeterminate, true, selectEdges, "", "", "");





        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);


        }

    }
    public class TraceSecondaryIsolation : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public TraceSecondaryIsolation()
        {

        }
        private void reloadOccured(object sender, EventArgs e)
        {

        }
        protected override void OnClick()
        {
            //string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            //string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");

            //string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            //string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");
            //string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');

            //double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5);
            //if (GeoNetTools.ToggleOperableStatus(ArcMap.Application, null, false, ISOvalveFeatureLayerName, ISOsourceFeatureLayerName,
            //                                                ISOoperableFieldNameValves, ISOoperableFieldNameSources,
            //                                                ISOoperableValues, SnapTol) != null)
            //{
            //    gnTools.TraceNetwork(GeometricNetworkToolsFunc.TraceType.SecondaryIsolation);
            //}
            //else
            //{
            //    //  MessageBox.Show("Please select some valves to proceed");

            //}
            bool selectEdges = false;
            if (Control.ModifierKeys == Keys.Control)
                selectEdges = true;
            else
                selectEdges = false;
            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");
            string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");

            string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');
            string ISOvalveAddSQL = ConfigUtil.GetConfigValue("TraceIsolation_Valve_AddSQL", "");
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            string ClearFlagBeforeIso = ConfigUtil.GetConfigValue("TraceIsolation_ClearFlagsOnClick", "true");



            if (GeoNetTools.ToggleOperableStatus(ArcMap.Application, null, false, ISOvalveFeatureLayerName, ISOsourceFeatureLayerName,
                                                            ISOoperableFieldNameValves, ISOoperableFieldNameSources,
                                                            ISOoperableValues, SnapTol) != null)
            {
                Globals.RemoveTraceGraphics(((IMxDocument)ArcMap.Application.Document).FocusMap, false);


                Globals.ClearSelected(ArcMap.Application, false);

                string returnVal = GeoNetTools.TraceIsolation(null, null, ArcMap.Application, ISOsourceFeatureLayerName, ISOvalveFeatureLayerName, ISOoperableFieldNameValves, ISOoperableFieldNameSources, SnapTol, true, ISOoperableValues, ISOvalveAddSQL, traceIndeterminate, true, selectEdges, "", "", "");
            }
            else
            {
                //  MessageBox.Show("Please select some valves to proceed");

            }
        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);


        }

    }
    public class TraceSummaryIsolation : ESRI.ArcGIS.Desktop.AddIns.Button
    {


        IEditor m_Editor = null;

        public TraceSummaryIsolation()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {

            bool traceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
            string ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");
            string ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");
            string ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
            string ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");

            string[] ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');
            string ISOvalveAddSQL = ConfigUtil.GetConfigValue("TraceIsolation_Valve_AddSQL", "");
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);


            string TraceSum_LayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_LayerName", "");
            string TraceSum_FacilityIDField = ConfigUtil.GetConfigValue("TraceIsolationSummary_FacilityIDField", "");
            string TraceSum_DateFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_DateFieldName", "");
            string TraceSum_ValveCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_ValveCountFieldName", "");
            string TraceSum_MeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_MeterCountFieldName", "");
            string TraceSum_CritMeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CritMeterCountFieldName", "");
            string TraceSum_CommentsFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CommentsFieldName", "");
            string TraceSum_MainsFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Main_FeatureLayer", "");
            string TraceSum_MeterFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_FeatureLayer", "");
            string TraceSum_MeterCritFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_Critical_Field", "");
            string TraceSum_MeterCritValue = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_Critical_Value", "");



            Globals.RemoveTraceGraphics(((IMxDocument)ArcMap.Application.Document).FocusMap, false);

            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Flags);

            GeoNetTools.TraceIsolationSummary(ArcMap.Application, ISOsourceFeatureLayerName, ISOvalveFeatureLayerName, ISOoperableFieldNameValves, ISOoperableFieldNameSources, SnapTol,
                false, ISOoperableValues, ISOvalveAddSQL, traceIndeterminate, true, TraceSum_MainsFeatureLayerName, TraceSum_MeterFeatureLayerName, TraceSum_MeterCritFieldName, TraceSum_MeterCritValue,
              TraceSum_LayerName, TraceSum_FacilityIDField, TraceSum_DateFieldName, TraceSum_ValveCountFieldName, TraceSum_MeterCountFieldName, TraceSum_CritMeterCountFieldName, TraceSum_CommentsFieldName);

        }


        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;
        }

    }
    public class SewerProfile : ESRI.ArcGIS.Desktop.AddIns.Tool
    {

        public SewerProfile()
        {

        }

        protected override void OnActivate()
        {
            Globals.ClearSelected(ArcMap.Document.FocusMap, true);
            Globals.RemoveTraceGraphics(ArcMap.Document.FocusMap, true);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Flags);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Barries);
            Globals.ClearGNFlags(ArcMap.Application, Globals.GNTypes.Results);
            base.OnActivate();
            MessageBox.Show("Please Click Two Points for Tracing");

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            double SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0);
            List<ProfileGraphDetails> ProfileGraph = ConfigUtil.GetProfileGraphConfig();
            GeoNetTools.AddFlagsForSewerProfile(ArcMap.Application, SnapTol, ProfileGraph);
            ProfileGraph = null;





        }


        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

    }
    public class FlowAccumulation : ESRI.ArcGIS.Desktop.AddIns.Button
    {


        IEditor m_Editor = null;

        public FlowAccumulation()
        {
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {

            GeoNetTools.CalculateFlowAccum(ConfigUtil.GetFlowAccumConfig(), ArcMap.Application);

        }


        protected override void OnUpdate()
        {

            if (m_Editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_Editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;
        }

    }
    public class FlowAccumulationLoc : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


       
        public FlowAccumulationLoc()
        {
            // m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {

            GeoNetTools.CalculateFlowAccumAtLocation(ConfigUtil.GetFlowAccumConfig(), ArcMap.Application, Convert.ToDouble(ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0)));

        }


        protected override void OnUpdate()
        {

            //if (m_Editor == null)
            //{
            //    Enabled = false;
            //    return;
            //}
            //if (m_Editor.EditState != esriEditState.esriStateEditing)
            //{
            //    Enabled = false;
            //    return;
            //}
            Enabled = true;
        }

    }

    public class FindClosest : ESRI.ArcGIS.Desktop.AddIns.Tool
    {


       
        public FindClosest()
        {
            // m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            IPoint point = ArcMap.Document.CurrentLocation;

            //GeoNetTools.CalculateFlowAccumAtLocation(ConfigUtil.GetFlowAccumConfig(), ArcMap.Application, Convert.ToDouble(ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5)));
            string test = GeoNetTools.TraceFindClosest(new double[] { point.X }, new double[] { point.Y }, ArcMap.Application, "wHydrant", "Length", 45, true, true);


        }


        protected override void OnUpdate()
        {

            //if (m_Editor == null)
            //{
            //    Enabled = false;
            //    return;
            //}
            //if (m_Editor.EditState != esriEditState.esriStateEditing)
            //{
            //    Enabled = false;
            //    return;
            //}
            Enabled = true;
        }

    }



    //    private void GetConfigSettings()
    //{

    //string TraceSum_MeterFeatureLayerName;
    //   string TraceSum_MeterCritFieldName;
    //   string TraceSum_MainsFeatureLayerName;
    //   string TraceSum_LayerName;
    //   string TraceSum_FacilityIDField;
    //   string TraceSum_DateFieldName;
    //   string TraceSum_ValveCountFieldName;
    //   string TraceSum_MeterCountFieldName;
    //   string TraceSum_CritMeterCountFieldName;
    //   string TraceSum_CommentsFieldName;


    //TraceSum_LayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_LayerName", "");
    //    TraceSum_FacilityIDField = ConfigUtil.GetConfigValue("TraceIsolationSummary_FacilityIDField", "");
    //    TraceSum_DateFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_DateFieldName", "");
    //    TraceSum_ValveCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_ValveCountFieldName", "");
    //    TraceSum_MeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_MeterCountFieldName", "");
    //    TraceSum_CritMeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CritMeterCountFieldName", "");
    //    TraceSum_CommentsFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CommentsFieldName", "");
    //    TraceSum_MainsFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Main_FeatureLayer", "");
    //    TraceSum_MeterFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_FeatureLayer", "");
    //    TraceSum_MeterCritFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_Critical_Field", "");


    //    m_SnapTol = ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5);

    //    m_TFtraceIndeterminate = ConfigUtil.GetConfigValue("TraceFlow_Interminate", false);
    //    m_ISOsourceFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Source_FeatureLayer", "");
    //    m_ISOvalveFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolation_Valve_FeatureLayer", "");


    //    m_ISOoperableFieldNameValves = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Valves", "");
    //    m_ISOoperableFieldNameSources = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Field_Sources", "");
    //    m_ISOoperableValues = ConfigUtil.GetConfigValue("TraceIsolation_Operable_Values", "").Split('|');
    //    m_ClearFlagBeforeIso = ConfigUtil.GetConfigValue("TraceIsolation_ClearFlagsOnClick", "true");
    //    m_ISOvalveAddSQL = ConfigUtil.GetConfigValue("TraceIsolation_Valve_AddSQL", "");

    //    m_TraceSum_LayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_LayerName", "");
    //    m_TraceSum_FacilityIDField = ConfigUtil.GetConfigValue("TraceIsolationSummary_FacilityIDField", "");
    //    m_TraceSum_DateFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_DateFieldName", "");
    //    m_TraceSum_ValveCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_ValveCountFieldName", "");
    //    m_TraceSum_MeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_MeterCountFieldName", "");
    //    m_TraceSum_CritMeterCountFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CritMeterCountFieldName", "");
    //    m_TraceSum_CommentsFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_CommentsFieldName", "");
    //    m_TraceSum_MainsFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Main_FeatureLayer", "");
    //    m_TraceSum_MeterFeatureLayerName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_FeatureLayer", "");
    //    m_TraceSum_MeterCritFieldName = ConfigUtil.GetConfigValue("TraceIsolationSummary_Meter_Critical_Field", "");


    //    m_ProfileGraph = ConfigUtil.GetProfileGraphConfig();


    //    m_CheckVisibleOnly = ConfigUtil.GetConfigValue("ConnectionChecker_CheckOnlyVisibleLayers", false);

    //}
}

