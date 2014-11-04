/*
 | Version 10.1
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using A4LGSharedFunctions;
using A4WaterUtilities;


namespace A4GasUtilities
{
  
  
    public class ShowLayerWindow : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        internal static ESRI.ArcGIS.Framework.IDockableWindow s_dockWindow;
      
        public ShowLayerWindow()
        {
            
        }
        
        protected override void OnClick()
        {
            
          
           

           
            DockableWindow pDockWin = getDockableWindow() as DockableWindow;
            if (pDockWin == null)
                return;
            pDockWin.Show(!pDockWin.IsVisible());
        }

        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }
        private ESRI.ArcGIS.Framework.IDockableWindow getDockableWindow()
        {

            // Only get/create the dockable window if they ask for it
            if (s_dockWindow == null)
            {
                UID dockWinID = new UID();
                dockWinID.Value = "A4GasUtilities_LayerViewer";
                s_dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
                //s_extension.UpdateSelCountDockWin()
            }

            return s_dockWindow;
        }
    }
 
    public class IdentifySelected : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        //string _caption = "Connection Checker";
        //IEditor m_Editor = null;
        DataTools m_Tools = null;
        public IdentifySelected()
        {
            //m_Editor = Globals.getEditor(ArcMap.Application);
            m_Tools = new A4WaterUtilities.DataTools(ArcMap.Application);
        }

        protected override void OnClick()
        {
            m_Tools.IdentifySelected();

        }

        protected override void OnUpdate()
        {
            //   if (m_Editor == null)
            //{
            //    Enabled = false;
            //    return;
            //}
            //if (m_Editor.EditState != esriEditState.esriStateEditing)
            //    Enabled = false;
            Enabled = (ArcMap.Application != null);

        }

    }
    public class ExportToExcel : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        //string _caption = "Connection Checker";
        //IEditor m_Editor = null;
        DataTools m_Tools = null;
        public ExportToExcel()
        {
            //m_Editor = Globals.getEditor(ArcMap.Application);
            m_Tools = new DataTools(ArcMap.Application);
        }

        protected override void OnClick()
        {
            m_Tools.ExportSelectedRecordsToExcel();
        }

        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

            //   if (m_Editor == null)
            //{
            //    Enabled = false;
            //    return;
            //}
            //if (m_Editor.EditState != esriEditState.esriStateEditing)
            //    Enabled = false;

        }

    }

}
