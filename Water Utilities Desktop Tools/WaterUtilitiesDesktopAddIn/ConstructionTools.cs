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
using ESRI.ArcGIS.Display;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    public partial class AddLateralsConstructionTool : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;

        public AddLateralsConstructionTool()
        {
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
            m_edSketch = m_editor as IEditSketch3;
            
            m_editor.CurrentTask = null;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryPoint;
            
            //IEditTaskSearch editTaskSearch = m_editor as IEditTaskSearch;
            //IEditTask editTask = editTaskSearch.get_TaskByUniqueName("GarciaUI_CreateNewFeatureTask");
            //m_editor.CurrentTask = editTask;

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
            //if (m_edSketch.Geometry == null)
            //    return;
            //if (m_edSketch.Geometry.IsEmpty )
            //    return;
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    // Finish part
            //    ISketchOperation pso = new SketchOperation();
            //    pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
            //    pso.Start(m_editor);
            //    m_edSketch.FinishSketchPart();
            //    pso.Finish(null);
            //}
            //else
              
            //        m_edSketch.FinishSketch();
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))

                m_csc.SketchModified();
        }



        private void OnShapeConstructorChanged()
        {
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            //m_editor.UndoOperation();
            IFeature pFeat = null;
            IEnvelope pEnv = null;
            try
            {
               
                
                m_editor.StartOperation();
                Globals.ClearSelected(ArcMap.Application, false, new List<esriGeometryType>() { esriGeometryType.esriGeometryPoint });

                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);

                //CreatePoint(m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate);

                if (pFeat == null)
                    return;


                // addLat.AddLateralAtPoint(m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name);

                //((IFeatureSelection)m_editor.CurrentTemplate.Layer).Clear();
                m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);

                string resetFlow = AddLateralsLinesCmds.AddLaterals(ArcMap.Application, ConfigUtil.GetAddLateralsConfig(), pFeat, false, true, false, false, (IFeatureLayer)m_editor.CurrentTemplate.Layer);
                // m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);


                m_editor.Display.Invalidate((ArcMap.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);

                pFeat.Store();
              
                (ArcMap.Document as IMxDocument).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, (ArcMap.Document as IMxDocument).ActiveView.Extent.Envelope);


                if (resetFlow.ToUpper() == "DIGITIZED")
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowDigitized", ArcMap.Application).Execute();

                }
                else if (resetFlow.ToUpper() == "ROLE")
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
                }
                else if (resetFlow.ToUpper() == "Ancillary".ToUpper())
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
                }
                else
                {
                }
                m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("CrtAssetAndLat"));

                //(A4LGSharedFunctions.Localizer.GetString("CrtAssetAndLat"));
            }
            catch(Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ALT_1") + ex.Message);
                m_editor.AbortOperation();

            }
            finally
            {
                pFeat = null;
                pEnv = null;
            }
        }


    }

    public partial class AddLateralsFromMainPointConstructionTool : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;

        public AddLateralsFromMainPointConstructionTool()
        {
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
            m_editor.CurrentTask = null;

            m_edSketch = m_editor as IEditSketch3;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryPoint;
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
            //if (m_edSketch.Geometry == null)
            //    return;
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    // Finish part
            //    ISketchOperation pso = new SketchOperation();
            //    pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
            //    pso.Start(m_editor);
            //    m_edSketch.FinishSketchPart();
            //    pso.Finish(null);
            //}
            //else
            //    m_edSketch.FinishSketch();
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))

                m_csc.SketchModified();
        }

        private void OnShapeConstructorChanged()
        {
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            //m_editor.UndoOperation();
            IFeature pFeat = null;
            IEnvelope pEnv = null;
            try
            {


                m_editor.StartOperation();
                Globals.ClearSelected(ArcMap.Application, false, new List<esriGeometryType>() { esriGeometryType.esriGeometryPoint });

                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);

                //CreatePoint(m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate);

                if (pFeat == null)
                    return;


                // addLat.AddLateralAtPoint(m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name);

                //((IFeatureSelection)m_editor.CurrentTemplate.Layer).Clear();
                m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);

                string resetFlow = AddLateralsFromPoint.AddLateralsFromMainPoint(ArcMap.Application, ConfigUtil.GetAddLateralsFromMainConfig(), pFeat, false, true, false);
                // m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);


                m_editor.Display.Invalidate((ArcMap.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);

                pFeat.Store();

                (ArcMap.Document as IMxDocument).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, (ArcMap.Document as IMxDocument).ActiveView.Extent.Envelope);


                if (resetFlow.ToUpper() == "DIGITIZED")
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowDigitized", ArcMap.Application).Execute();

                }
                else if (resetFlow.ToUpper() == "ROLE")
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
                }
                else if (resetFlow.ToUpper() == "Ancillary".ToUpper())
                {
                    Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
                }
                else
                {
                }
                m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("CrtAssetAndLat"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("ALT_1") + ex.Message);
                m_editor.AbortOperation();

            }
            finally
            {
                pFeat = null;
                pEnv = null;
            }
        }


    }
  
    public partial class AddLineWithEndPoints : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;

        public AddLineWithEndPoints()
        {
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
            if (m_edSketch.Geometry == null)
                return;
            if (Control.ModifierKeys == Keys.Shift)
            {
                // Finish part
                ISketchOperation pso = new SketchOperation();
                pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
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
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            IFeature pFeat = null;
            ISegmentCollection pSegColl = null;
            IEnumSegment pESeg = null;
            ISegment testSegment = null;
            ISegmentCollection segColTest = null;
            object Missing = null;
            try
            {
                // Send a shift-tab to hide the construction toolbar
            
                try
                {
                    m_editor.StartOperation();
                }
                catch
                {

                    m_editor.AbortOperation();
                    m_editor.StartOperation();
                }
                bool twoPoint = false;
                (ArcMap.Application.Document as IMxDocument).FocusMap.ClearSelection();
                List<IFeature> pLstFeat = null;
                if (Control.ModifierKeys == Keys.Control)
                {
                    twoPoint = CreateLineWithEndPoints.CreatePoints(ArcMap.Application, ConfigUtil.GetLinePointAtEndsConfig(), m_edSketch.Geometry as IPolyline, (IFeatureLayer)m_editor.CurrentTemplate.Layer, false, out pLstFeat);
                }
                else
                {
                    twoPoint = CreateLineWithEndPoints.CreatePoints(ArcMap.Application, ConfigUtil.GetLinePointAtEndsConfig(), m_edSketch.Geometry as IPolyline, (IFeatureLayer)m_editor.CurrentTemplate.Layer, true, out pLstFeat);
                }


                if (twoPoint)
                {


                    pSegColl = (ISegmentCollection)m_edSketch.Geometry;
                    pESeg = pSegColl.EnumSegments;
                    pESeg.Reset();


                    int partIndex = 0;
                    int segmentIndex = 0;

                    pESeg.Next(out testSegment, ref partIndex, ref segmentIndex);

                    while (testSegment != null)
                    {

                        segColTest = new PolylineClass();

                        Missing = Type.Missing;
                        segColTest.AddSegment(testSegment, ref Missing, ref Missing);

                        pFeat = Globals.CreateFeature(segColTest as IGeometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);
                        pFeat.Store();
                        pESeg.Next(out testSegment, ref partIndex, ref segmentIndex);
                    }
                }
                else
                {
                    pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);
                    pFeat.Store();

                }

                foreach (IFeature pFt in pLstFeat)
                {
                    pFt.Store();
                }
                pLstFeat = null;

                m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("CrtLnWithPts"));
                (ArcMap.Application.Document as IMxDocument).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null);
               
               
            }
            catch { }
            finally
            {
                pFeat = null;
                pSegColl = null;
                pESeg = null;
                testSegment = null;
                segColTest = null;
                Missing = null;

            }
            
        }


    }

    public partial class AddPointSplitLine : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;

        public AddPointSplitLine()
        {
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
            m_editor.CurrentTask = null;

            m_edSketch = m_editor as IEditSketch3;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryPoint;
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
            //if (m_edSketch.Geometry == null)
            //    return;
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    // Finish part
            //    ISketchOperation pso = new SketchOperation();
            //    pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
            //    pso.Start(m_editor);
            //    m_edSketch.FinishSketchPart();
            //    pso.Finish(null);
            //}
            //else
            //    m_edSketch.FinishSketch();
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))

                m_csc.SketchModified();
        }


        private void OnShapeConstructorChanged()
        {
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            IFeature pFeat;
            try
            {

                m_editor.StartOperation();

                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate as IEditTemplate, m_editor, ArcMap.Application, false, false, true);




                GeometryTools.SplitLinesAtClick(ArcMap.Application, ConfigUtil.GetConfigValue("SplitLinesSuspendAA", "true"), ConfigUtil.GetConfigValue("SplitLinesAtLocation_Snap", 10.0), ConfigUtil.GetConfigValue("SplitLines_SkipDistance", .5), m_edSketch.Geometry as IPoint, false, true, false);



                pFeat.Store();
                m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AddPtsAndSplitLn"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("AddPtsAndSplitLn") + "\n" + ex.Message);
                m_editor.AbortOperation();

            }
            finally
            {
                pFeat = null;
            }
        }


    }

    public partial class ConnectClosestsConstructTool : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;

        public ConnectClosestsConstructTool()
        {
            ConfigUtil.type = "water";
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

            ConfigUtil.type = "water";
            m_editor.CurrentTask = null;

            m_edSketch = m_editor as IEditSketch3;
            m_edSketch.GeometryType = esriGeometryType.esriGeometryPoint;
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
            //if (m_edSketch.Geometry == null)
            //    return;
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    // Finish part
            //    ISketchOperation pso = new SketchOperation();
            //    pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
            //    pso.Start(m_editor);
            //    m_edSketch.FinishSketchPart();
            //    pso.Finish(null);
            //}
            //else
            //    m_edSketch.FinishSketch();
        }

        private void OnSketchModified()
        {
            if (Globals.IsShapeConstructorOkay(m_csc))

             m_csc.SketchModified();
        }

        private void OnShapeConstructorChanged()
        {
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            Keys ModKey = Control.ModifierKeys;

            // Send a shift-tab to hide the construction toolbar
           
            m_editor.StartOperation();
            IFeature pFeat = null;
            returnFeatArray pRetVal = null;
            if (ModKey == Keys.Shift)
            {
                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);
                pRetVal = ConnectClosest.ConnectClosestFeatureAtPoint(ArcMap.Application, ConfigUtil.GetConnectClosestConfig(), m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name, true, ModKey);
                //   pFeat.Store();
            }
            else if (ModKey == (Keys.Control | Keys.Shift))
            {
                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, false, true);
                pRetVal = ConnectClosest.ConnectClosestFeatureAtPoint(ArcMap.Application, ConfigUtil.GetConnectClosestConfig(), m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name, true, ModKey);
                // pFeat.Store();
            }
            else if (ModKey == Keys.Control)
            {
                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, true, true);
                pRetVal = ConnectClosest.ConnectClosestFeatureAtPoint(ArcMap.Application, ConfigUtil.GetConnectClosestConfig(), m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name, true, ModKey);
                //    pFeat.Store();
            }
            else
            {

                pFeat = Globals.CreateFeature(m_edSketch.Geometry, m_editor.CurrentTemplate, m_editor, ArcMap.Application, false, true, true);
                pRetVal = ConnectClosest.ConnectClosestFeatureAtPoint(ArcMap.Application, ConfigUtil.GetConnectClosestConfig(), m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name, true, ModKey);

            }
            pFeat.Store();
            foreach (IFeature featus in pRetVal.Features)
            {
                featus.Store();

            }

            if (pRetVal.Options== "DIGITIZED")
            {
                Globals.GetCommand("A4WaterUtilities_EstablishFlowDigitized", ArcMap.Application).Execute();

            }
            else if (pRetVal.Options == "ROLE")
            {
                Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
            }
            else if (pRetVal.Options == "Ancillary".ToUpper())
            {
                Globals.GetCommand("A4WaterUtilities_EstablishFlowAncillary", ArcMap.Application).Execute();
            }
            else
            {
            }
            //            addLat.AddLateralAtPoint(m_edSketch.Geometry as IPoint, m_editor.CurrentTemplate.Layer.Name);

            m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("CrtAssetAndLat"));

            //IEnvelope pEnv = pFeat.Shape.Envelope;
            //pEnv.Expand(8, 8, true);

            (ArcMap.Application.Document as IMxDocument).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, pFeat, null);
            //pEnv = null;
            pFeat = null;
            pRetVal = null;
        }


    }
    public partial class PointsAlongLineTool : ESRI.ArcGIS.Desktop.AddIns.Tool, IShapeConstructorTool, ISketchTool
    {
        private IEditor3 m_editor;
        private IEditEvents_Event m_editEvents;
        private IEditEvents5_Event m_editEvents5;
        private IEditSketch3 m_edSketch;
        private IShapeConstructor m_csc;
        private PointsAlongLineForm m_form;

        public PointsAlongLineTool()
        {
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
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
            ConfigUtil.type = "water";
            if (m_edSketch.Geometry == null)
                return;
            if (Control.ModifierKeys == Keys.Shift)
            {
                // Finish part
                ISketchOperation pso = new SketchOperation();
                pso.MenuString_2 = A4LGSharedFunctions.Localizer.GetString("FinishSktchPrt");
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
            if (m_csc != null)
                m_csc.Deactivate();
            m_csc = null;
            m_csc = m_edSketch.ShapeConstructor;
            if (m_csc != null)
                m_csc.Activate();
        }

        private void OnSketchFinished()
        {
            ConfigUtil.type = "water";
            //send a space to hide the construction toolbar
            SendKeys.SendWait(" ");

            //Create form and pass initialization parameters
            if (m_form != null)
                m_form = new PointsAlongLineForm(m_editor);

            //Show the dialog modal
            m_form.ShowDialog();
        }


    }
  
}
