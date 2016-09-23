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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using A4LGSharedFunctions;

namespace A4LGAddressManagement
{
    public partial class CreateLineAndSplitIntersectingLines : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;
       
        public CreateLineAndSplitIntersectingLines()
        {
            ConfigUtil.type = "address";
            // Get the editor
            m_editor = ArcMap.Editor as IEditor3;
            m_editEvents = m_editor as IEditEvents_Event;
            m_editEvents5 = m_editor as IEditEvents5_Event;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        protected override void OnActivate()
        {
            ConfigUtil.type = "address";
            m_editor.CurrentTask = null;

            m_edSketch = m_editor as IEditSketch3;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryPolyline;
            // Activate a shape constructor based on the current sketch geometry
            if (m_edSketch.GeometryType == esriGeometryType.esriGeometryPoint | m_edSketch.GeometryType == esriGeometryType.esriGeometryMultipoint)
                m_csc = new PointConstructorClass();
            else
                m_csc = new StraightConstructorClass();

            m_csc.Initialize(m_editor);
            m_edSketch.ShapeConstructor = m_csc;
            m_csc.Activate();

            // Setup events
            m_editEvents.OnSketchModified += OnSketchModified;
            m_editEvents5.OnShapeConstructorChanged += OnShapeConstructorChanged;
            m_editEvents.OnSketchFinished += OnSketchFinished;

        }

        protected override bool OnDeactivate()
        {
            m_editEvents.OnSketchModified -= OnSketchModified;
            m_editEvents5.OnShapeConstructorChanged -= OnShapeConstructorChanged;
            m_editEvents.OnSketchFinished -= OnSketchFinished;
            return true;
        }

        protected override void OnDoubleClick()
        {
            ConfigUtil.type = "address";
            if (m_edSketch.Geometry == null)
                return;
            if (Control.ModifierKeys == Keys.Shift)
            {
                // Finish part
                ISketchOperation pso = new SketchOperation();
                pso.MenuString_2 = "Finish Sketch Part";
                pso.Start(m_editor);
                m_edSketch.FinishSketchPart();
                pso.Finish(null);
            }
            else
                m_edSketch.FinishSketch();
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))
            m_csc.SketchModified();
            
        }

        private void OnShapeConstructorChanged()
        {
            // Activate a new constructor
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "address";
            //send a space to hide the construction toolbar
            SendKeys.SendWait(" ");

            try
            {
                m_editor.StartOperation();
            }
            catch
            {

                m_editor.AbortOperation();
                m_editor.StartOperation();
            }
            IFeature pRoadFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, true, true);
            pRoadFeat.Store();
            //AMGeometryTools.AddPointWithRef(m_application, pPnts.get_Point(0), configDetails, ((IFeatureLayer)m_editor.CurrentTemplate.Layer), ref idxConfig);//,config,null,true,true);

            AMGeometryTools.SplitAndProrate(ArcMap.Application, pRoadFeat, m_editor.CurrentTemplate.Layer as IFeatureLayer, ConfigUtil.GetAddressCenterlineConfig());

            // pFeats.Add(pFeat);
            ((IMxDocument)ArcMap.Application.Document).ActiveView.Refresh();
            try
            {
                m_editor.StopOperation("Create Point with Reference");
            }
            catch
            {


            }
            pRoadFeat = null;

            
        }


    }
    public partial class CreatePointAndRefPoint : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;
        private AddressMapTip m_addressMaptip;
        private IFeatureLayer m_targetLayer;
        private string m_className;
        private AddressSettings m_settings = AddressSettings.Default;

        public CreatePointAndRefPoint()
        {
            ConfigUtil.type = "address";
            // Get the editor
            m_editor = ArcMap.Editor as IEditor3;
            m_editEvents = m_editor as IEditEvents_Event;
            m_editEvents5 = m_editor as IEditEvents5_Event;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        protected override void OnActivate()
        {
            ConfigUtil.type = "address";
            m_editor.CurrentTask = null;
            m_targetLayer = ((IFeatureLayer)m_editor.CurrentTemplate.Layer);
            m_className = Globals.getClassName(m_targetLayer);

            m_edSketch = m_editor as IEditSketch3;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryMultipoint;
            // Activate a shape constructor based on the current sketch geometry
            if (m_edSketch.GeometryType == esriGeometryType.esriGeometryPoint | m_edSketch.GeometryType == esriGeometryType.esriGeometryMultipoint)
                m_csc = new PointConstructorClass();
            else
                m_csc = new StraightConstructorClass();

            m_csc.Initialize(m_editor);
            m_edSketch.ShapeConstructor = m_csc;
            m_csc.Activate();

            // Setup events
            m_editEvents.OnSketchModified += OnSketchModified;
            m_editEvents5.OnShapeConstructorChanged += OnShapeConstructorChanged;
            m_editEvents.OnSketchFinished += OnSketchFinished;

            // Initialize address map tip
            m_addressMaptip = new AddressMapTip();
            var mxPtr = new IntPtr(ArcMap.Application.hWnd);
            m_addressMaptip.Show(Control.FromHandle(mxPtr));
            m_addressMaptip.Visible = false;
        }

        protected override bool OnDeactivate()
        {
            m_editEvents.OnSketchModified -= OnSketchModified;
            m_editEvents5.OnShapeConstructorChanged -= OnShapeConstructorChanged;
            m_editEvents.OnSketchFinished -= OnSketchFinished;

            // Destroy address map tip
            m_addressMaptip.Close();
            m_addressMaptip = null;
            m_settings.Save();

            return true;
        }

        protected override void OnDoubleClick()
        {
            ConfigUtil.type = "address";
            if (m_edSketch.Geometry == null)
                return;
            if (Control.ModifierKeys == Keys.Shift)
            {
                // Finish part
                ISketchOperation pso = new SketchOperation();
                pso.MenuString_2 = "Finish Sketch Part";
                pso.Start(m_editor);
                m_edSketch.FinishSketchPart();
                pso.Finish(null);
            }
            else
                m_edSketch.FinishSketch();
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
            UpdateMapTip();
        }

        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
            if (arg.KeyCode == Keys.A)
            {
                var autoIncrement = new AutoIncrementWindow(m_settings.AutoIncrement);

                var mxPtr = new IntPtr(ArcMap.Application.hWnd);
                if (autoIncrement.ShowDialog(Control.FromHandle(mxPtr)) == DialogResult.OK)
                {
                    m_settings.AutoIncrement = Convert.ToInt16(autoIncrement.GetIncrementValue());
                }
            }
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))
                m_csc.SketchModified();

        }

        private void OnShapeConstructorChanged()
        {
            // Activate a new constructor
            if (m_csc != null)
            m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "address";
            //send a space to hide the construction toolbar
            SendKeys.SendWait(" ");

            List<CreatePointWithReferenceDetails> configDetails = null;
            AddressReturnInfo retInfo = null;
            IPointCollection pPnts = null;
            List<IFeature> pFeats = null;
            IPoint pPnt = null;

            try
            {
                ((IMxDocument)ArcMap.Document).FocusMap.ClearSelection();

                Keys ModKey = Control.ModifierKeys;

                
                pPnts = m_edSketch.Geometry as IPointCollection;
                if (pPnts == null)
                    return;

                if (pPnts.PointCount < 2)
                    return;

                try
                {
                    m_editor.StartOperation();
                }
                catch
                {

                    m_editor.AbortOperation();
                    m_editor.StartOperation();
                }

                int idxConfig = -1;
                // TODO: Add developer code here
                configDetails = ConfigUtil.GetCreatePointWithRefConfig();


                retInfo = AMGeometryTools.AddPointWithRef(ArcMap.Application, pPnts.get_Point(0), configDetails, ((IFeatureLayer)m_editor.CurrentTemplate.Layer), ref idxConfig);//,config,null,true,true);
                if (idxConfig == -1)
                    return;

                if (retInfo == null)
                    return;


                int targetAddFieldIdx = Globals.GetFieldIndex(((IFeatureLayer)m_editor.CurrentTemplate.Layer), configDetails[idxConfig].AddressField);

                if (targetAddFieldIdx == -1)
                    return;

                int targetNameFieldIdx = Globals.GetFieldIndex(((IFeatureLayer)m_editor.CurrentTemplate.Layer), configDetails[idxConfig].StreetNameField);

                if (targetNameFieldIdx == -1)
                    return;

                int targetIDFieldIdx = Globals.GetFieldIndex(((IFeatureLayer)m_editor.CurrentTemplate.Layer), configDetails[idxConfig].AddressPntKeyField);

                int targetCenterlineIDFieldIdx = Globals.GetFieldIndex(((IFeatureLayer)m_editor.CurrentTemplate.Layer), configDetails[idxConfig].StreetIDField);

                //if (targetIDFieldIdx == -1)
                //    return;

                //. .AddLaterals(m_application, ConfigUtil.GetAddLateralsConfig(), pFeat, false, true, false, false);

                //IFeature pFeat = null;
                pFeats = new List<IFeature>();

                for (int i = 1; i < pPnts.PointCount; i++)
                {
                    pFeats.Add(Globals.CreateFeature(pPnts.get_Point(i), m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true));
                    // pFeats.Add(pFeat);

                }

                for (int i = 0; i < pFeats.Count; i++)
                {
                    var pFeat = pFeats[i];
                    if (retInfo.AddressDetails.StreetGeometry == null)
                    {
                        if (targetNameFieldIdx != -1)
                            pFeat.set_Value(targetNameFieldIdx, retInfo.AddressDetails.Messages);

                    }
                    else
                    {

                        bool rightSide = true;
                        pPnt = Globals.GetPointOnLine(pFeat.Shape as IPoint, retInfo.AddressDetails.StreetGeometry as IPolyline, 10000, out rightSide);
                        if (rightSide)
                        {
                            pFeat.set_Value(targetAddFieldIdx, retInfo.AddressDetails.RightAddress + (i * m_settings.AutoIncrement));
                        }
                        else
                        {
                            pFeat.set_Value(targetAddFieldIdx, retInfo.AddressDetails.LeftAddress + (i * m_settings.AutoIncrement));
                        }
                        if (targetNameFieldIdx != -1)
                            pFeat.set_Value(targetNameFieldIdx, retInfo.AddressDetails.StreetName);

                        if (targetIDFieldIdx != -1)
                            pFeat.set_Value(targetIDFieldIdx, retInfo.AddressPointKey);

                        if (targetCenterlineIDFieldIdx != -1)
                            pFeat.set_Value(targetCenterlineIDFieldIdx, retInfo.AddressDetails.StreetID);
                        pFeat.Store();
                    }
                }
                pPnts = null;
                ((IMxDocument)ArcMap.Document).ActiveView.Refresh();
                try
                {
                    m_editor.StopOperation("Create Point with Reference");
                }
                catch
                {


                }
            }
            catch
            {

                configDetails = null;
                retInfo = null;
                pPnts = null;
                pFeats = null;
                pPnt = null;
                try
                {
                    m_editor.AbortOperation();
                }
                catch
                {

                }
            }
        }

        private void UpdateMapTip()
        {
            var point = ((ISketchTool)this).Location;
            var configDetails = ConfigUtil.GetCreatePointWithRefConfig();

            CreatePointWithReferenceDetails createPointDet = null;
            for (int i = 0; i < configDetails.Count; i++)
            {
                if (configDetails[i].LayerName == m_className || configDetails[i].LayerName == m_targetLayer.Name)
                {
                    createPointDet = configDetails[0];

                    bool pointFndAsFL = true;
                    var pointLayer = Globals.FindLayer(ArcMap.Application, createPointDet.ReferencePointLayerName, ref pointFndAsFL) as IFeatureLayer;
                    if (pointLayer == null)
                        continue;

                    var featureClass = pointLayer.FeatureClass;
                    point.Project(((IGeoDataset)featureClass).SpatialReference);
                    point.SnapToSpatialReference();

                    AddressInfo addInfo = Globals.GetAddressInfo(ArcMap.Application, point, createPointDet.AddressCenterlineDetails.FeatureClassName, createPointDet.AddressCenterlineDetails.FullName,
                            createPointDet.AddressCenterlineDetails.LeftTo, createPointDet.AddressCenterlineDetails.RightTo,
                            createPointDet.AddressCenterlineDetails.LeftFrom, createPointDet.AddressCenterlineDetails.RightFrom, createPointDet.AddressCenterlineDetails.IDField, false, 2);

                    if (addInfo == null)
                    {
                        m_addressMaptip.Visible = false;
                        return;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("{0} / {1} {2}", addInfo.LeftAddress, addInfo.RightAddress, addInfo.StreetName));
                    sb.AppendLine(string.Format("Distance: {0}", addInfo.DistanceAlong));
                    m_addressMaptip.SetLabel(sb.ToString());
                    m_addressMaptip.Top = System.Windows.Forms.Cursor.Position.Y + 15;
                    m_addressMaptip.Left = System.Windows.Forms.Cursor.Position.X;
                    m_addressMaptip.Visible = true;
                    return;
                }
            }
        }
    }
}
