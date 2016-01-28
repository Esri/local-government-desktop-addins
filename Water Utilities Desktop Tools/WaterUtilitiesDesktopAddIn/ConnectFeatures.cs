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
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using A4LGSharedFunctions;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
namespace A4WaterUtilities
{
    public class MoveConnections : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        IEditor m_Editor;
        public MoveConnections()
        {

            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);
         
        }

        protected override void OnActivate()
        {
            ConfigUtil.type = "water";
            if (m_Editor == null)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("StrtEditing"));   
                return;
            }
            if (m_Editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("StrtEditing"));
                return;
            }
            
            base.OnActivate();
            Globals.RemoveTraceGraphics(ArcMap.Document.FocusMap, true);
         
            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("SlctSrcAndTarget"));

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            ConfigUtil.type = "water";
            List<MoveConnectionsDetails> moveConnectionsConfig = ConfigUtil.GetMoveConnectionsConfig();

            GeoNetTools.MoveConnectionsToNewLine(ArcMap.Application, ConfigUtil.GetConfigValue("Trace_Click_Point_Tolerence", 5.0), moveConnectionsConfig);





        }
        protected override void OnUpdate()
        {
            if (m_Editor == null)
            {
                Enabled = false;
                return;
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

    public class AddLaterals : ESRI.ArcGIS.Desktop.AddIns.Button
    {
     
        IEditor m_editor;
     
        public AddLaterals()
        {
            ConfigUtil.type = "water";
            m_editor = Globals.getEditor(ArcMap.Application);

            
        }
        
        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            m_editor.StartOperation();


            string resetFlow = AddLateralsLinesCmds.AddLaterals(ArcMap.Application, ConfigUtil.GetAddLateralsConfig(), null, true, false, true, true);

          

            m_editor.Display.Invalidate((ArcMap.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);

     
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

        protected override void OnUpdate()
        {

            if (m_editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool __p1)
        {
            base.Dispose(__p1);
        
            m_editor = null;
        }
    }
    public class AddLateralsFromMain : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        IEditor m_editor;

        public AddLateralsFromMain()
        {
            ConfigUtil.type = "water";
            m_editor = Globals.getEditor(ArcMap.Application);


        }

        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            m_editor.StartOperation();
          
            string resetFlow = AddLateralsFromPoint.AddLateralsFromMainPoint(ArcMap.Application, ConfigUtil.GetAddLateralsFromMainConfig(), null, false, false, false);
            // m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);


            m_editor.Display.Invalidate((ArcMap.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);

         

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

        protected override void OnUpdate()
        {

            if (m_editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool __p1)
        {
            base.Dispose(__p1);

            m_editor = null;
        }
    }
    public class CreateTapPointsOnMain : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        IEditor m_editor;

        public CreateTapPointsOnMain()
        {
            ConfigUtil.type = "water";
          //  m_editor = Globals.getEditor(ArcMap.Application);


        }

        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            //if (m_editor.EditState != esriEditState.esriStateEditing)
            //{
            //    Enabled = false;
            //    return;
            //}
            //m_editor.StartOperation();

             AddLateralsFromPoint.createTapPoints(ArcMap.Application, ConfigUtil.GetCreateTapPointsOnMainConfig());
            // m_editor.Map.SelectFeature(m_editor.CurrentTemplate.Layer as IFeatureLayer, pFeat);


            m_editor.Display.Invalidate((ArcMap.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);



            (ArcMap.Document as IMxDocument).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, (ArcMap.Document as IMxDocument).ActiveView.Extent.Envelope);


          
           // m_editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("CrtAssetAndLat"));

        }

        protected override void OnUpdate()
        {

            //if (m_editor == null)
            //{
            //    Enabled = false;
            //    return;
            //}
            //if (m_editor.EditState != esriEditState.esriStateEditing)
            //{
            //    Enabled = false;
            //    return;
            //}
            Enabled = true;

        }
        protected override void Dispose(bool __p1)
        {
            base.Dispose(__p1);

            m_editor = null;
        }
    }
    public class ConnectClosests : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_editor;
        public ConnectClosests()
        {
            ConfigUtil.type = "water";
            m_editor = Globals.getEditor(ArcMap.Application);

     
        }
     
        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            ConnectClosest.ConnectClosestFeature(ArcMap.Application,ConfigUtil.GetConnectClosestConfig(),true, false, "");

        }
        protected override void OnUpdate()
        {

            if (m_editor == null)
            {
                Enabled = false;
                return;
            }
            if (m_editor.EditState != esriEditState.esriStateEditing)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

        }
        protected override void Dispose(bool __p1)
        {
            base.Dispose(__p1);

            m_editor = null;
        }

    }
   
}
