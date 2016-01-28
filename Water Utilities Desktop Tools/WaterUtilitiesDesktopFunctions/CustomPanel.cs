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


using System.Windows.Forms;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.SystemUI;

public partial class CustomPanel : System.Windows.Forms.Panel
{
    private System.Drawing.Pen m_Pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(127, 157, 185));

    //A custom panel used for two domain field.  This is requred to set the border color of a panel
    public CustomPanel()
        : base()
    {
    }
    public System.Drawing.Pen BorderColor
    {
        get { return m_Pen; }

        set { m_Pen = value; }
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        e.Graphics.DrawRectangle(m_Pen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
        base.OnPaint(e);
    }
}