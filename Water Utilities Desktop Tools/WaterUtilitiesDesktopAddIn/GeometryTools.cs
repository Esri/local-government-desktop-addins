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
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Editor;
using A4LGSharedFunctions;


namespace A4WaterUtilities
{
    public class AttributeTransferLoaderButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public AttributeTransferLoaderButton()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);
        }


        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            attributeTransferLoader.LoadAttributeTransfer(ArcMap.Application, ConfigUtil.GetAttributeTransferConfig());

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
    public class CreateJumpsOver : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;

        public CreateJumpsOver()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            double jumpDistance = ConfigUtil.GetConfigValue("CreateJumps_Distance", 10.0);
            jumpDistance = jumpDistance * 2;  //doubled so the height of the jump will be represented by this number

            GeometryTools.CreateJumps(ArcMap.Application, GeometryTools.JumpTypes.Over, jumpDistance);

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
    public class CreateJumpsUnder : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;

        public CreateJumpsUnder()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            double jumpDistance = ConfigUtil.GetConfigValue("CreateJumps_Distance", 10.0);
            jumpDistance = jumpDistance * 2;  //doubled so the height of the jump will be represented by this number

            GeometryTools.CreateJumps(ArcMap.Application, GeometryTools.JumpTypes.Under, jumpDistance);


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
    public class FlipLines : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public FlipLines()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);
            Enabled = false;


        }



        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            GeometryTools.FlipLines(ArcMap.Application, GeometryTools.FlipTypes.FlipLines);

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
    public class FlipLinesFlow : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;

        public FlipLinesFlow()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);


        }


        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            GeometryTools.FlipLines(ArcMap.Application, GeometryTools.FlipTypes.FlipLinesToMatchFlow);

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
    public class SplitLinesClick : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        IEditor m_Editor;
        public SplitLinesClick()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnMouseDown(MouseEventArgs arg)
        {

            ConfigUtil.type = "water";

            ESRI.ArcGIS.Geometry.IPoint point = ArcMap.Document.CurrentLocation;//ArcMap.Document.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            GeometryTools.SplitLinesAtClick(ArcMap.Application, ConfigUtil.GetConfigValue("SplitLinesSuspendAA", "true"), ConfigUtil.GetConfigValue("SplitLinesAtLocation_Snap", 10.0), ConfigUtil.GetConfigValue("SplitLines_SkipDistance", .5), point, true, false, true);


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
    public class SplitLines : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public SplitLines()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnClick()
        {

            ConfigUtil.type = "water";
            GeometryTools.SplitLines(ArcMap.Application, ConfigUtil.GetConfigValue("SplitLinesSuspendAA", "true"), ConfigUtil.GetConfigValue("SplitLinesAtLocation_Snap", 10.0), ConfigUtil.GetConfigValue("SplitLines_SkipDistance", .5));



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
    public class RotateSelected : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public RotateSelected()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            GeometryTools.RotateSelected(ArcMap.Application, ConfigUtil.GetConfigValue("RotateSelected_SpinAngle", 0.0), ConfigUtil.GetConfigValue("RotateSelected_DiameterFieldName", "DIAMETER"));

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
    public class AdditionalRotate : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public AdditionalRotate()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnClick()
        {


            ConfigUtil.type = "water";
            GeometryTools.AddRotate(ArcMap.Application, ConfigUtil.GetConfigValue("AddRotateSuspendAA", "true"), ConfigUtil.GetConfigValue("AdditionalRotate_SpinAngle", 0.0));


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
    public class SetMeasuresOnLines : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public SetMeasuresOnLines()
        {
            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }


        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            GeometryTools.SetMeasures(ArcMap.Application);

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
    public class MergeGNLines : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        IEditor m_Editor;
        public MergeGNLines()
        {

            ConfigUtil.type = "water";
            m_Editor = Globals.getEditor(ArcMap.Application);


        }

        protected override void OnClick()
        {
            ConfigUtil.type = "water";
            GeometryTools.showMergeDialog(ArcMap.Application);

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
}
