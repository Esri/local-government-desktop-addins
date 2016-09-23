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
using ESRI.ArcGIS.Editor;
using A4LGSharedFunctions;



namespace A4LGAddressManagement

{
    public class AddressFlipLines : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
    

        public AddressFlipLines()
        {
            ConfigUtil.type = "address";
            m_Editor = Globals.getEditor(ArcMap.Application);
      
        }
  
        protected override void OnClick()
        {
            ConfigUtil.type = "address";
            AMGeometryTools.FlipLines(ArcMap.Application, ConfigUtil.GetAddressCenterlineConfig(),true);

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
    public class AddressFlipLinesNoAddress : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;


        public AddressFlipLinesNoAddress()
        {
            ConfigUtil.type = "address";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "address";
            AMGeometryTools.FlipLines(ArcMap.Application, ConfigUtil.GetAddressCenterlineConfig(),false);

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
    public class AddressCreateIntersectionPoints : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //IEditor m_Editor;


        public AddressCreateIntersectionPoints()
        {
            ConfigUtil.type = "address";
          //  m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "address";
            AMGeometryTools.CreateIntersectionPoints(ArcMap.Application, ConfigUtil.GetAddressCenterlineConfig(), false);

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
    public class ShowConfigForm : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //internal static ESRI.ArcGIS.Framework.IDockableWindow s_dockWindow;
        ConfigFormNoLog m_ConfigForm;
        public ShowConfigForm()
        {
            ConfigUtil.type = "address";
        }

        protected override void OnClick()
        {

            ConfigUtil.type = "address";
            m_ConfigForm = new ConfigFormNoLog();
            m_ConfigForm.ShowDialog();

        }

        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

    }
}
