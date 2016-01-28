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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

using ESRI.ArcGIS.Editor;
namespace A4LGSharedFunctions
{
    public partial class SelectTemplateFormGraphic : Form
    {
        private ListView m_list;
  
        private IEditTemplate _currentTemplate = null;
       
            private IFeatureLayer _featLayer = null;
        public SelectTemplateFormGraphic(IFeatureLayer featLayer)
        {            
            InitializeComponent();
            try{
            this.Text = A4LGSharedFunctions.Localizer.GetString("AAOptionDialogTemplate");
            }
            catch
            {
            }
            _featLayer = featLayer;
            LoadListView();
        }

        private void SelectTemplateForm_Load(object sender, EventArgs e)
        {

        }
        public IEditTemplate GetSelected()
        {
            return _currentTemplate;
        }

        private void LoadListView()
        {
            IEditTemplateManager editTemplateMgr = null;
            //IFeatureLayer2 fl2 = null;
            ListViewGroup lvg = null;

            try
            {
                if (_featLayer == null)
                    return;
                listView1.SmallImageList = new ImageList();




                //fl2 = featLayer as IFeatureLayer2;

                //get templates for the layer

                editTemplateMgr = Globals.GetEditTemplateManager(_featLayer);
                if (editTemplateMgr == null)
                    return;

                //create listviewgroup for the layer
                lvg = new ListViewGroup(_featLayer.Name);

                //loop through each template for the layer and assign to listviewgroup
                for (int i = 0; i < editTemplateMgr.Count; i++)
                {
                    IEditTemplate editTemplate = editTemplateMgr.get_EditTemplate(i);

                    //Create listviewitem for the template and populate
                    ListViewItem lvi = new ListViewItem(lvg);
                    lvi.Text = editTemplate.Name;
                    lvi.Tag = editTemplate;
                    //if (_currentTemplate == null)
                    //    _currentTemplate = editTemplate;

                    //get the templates symbol as a bitmap
                    Bitmap bitmap = Globals.BitmapFromTemplate(editTemplate, this.listView1);

                    //add template bitmap to the listview's image list
                 
                    int index = listView1.SmallImageList.Images.Add(bitmap, Color.Transparent);
                    lvi.ImageIndex = index;
                    listView1.Items.Add(lvi);
                }
                listView1.Groups.Add(lvg);


                //sort listviewgroup by layer name
                ListViewGroup[] groupsArray = new ListViewGroup[listView1.Groups.Count];
                listView1.Groups.CopyTo(groupsArray, 0);
                System.Array.Sort(groupsArray, new ListViewGroupSorter(SortOrder.Ascending));
                listView1.Groups.Clear();
                listView1.Groups.AddRange(groupsArray);
            }
            catch { }
            finally { }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                _currentTemplate = listView1.SelectedItems[0].Tag as IEditTemplate;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //clear and reload templates into listview with search
            listView1.Clear();
            listView1.Groups.Clear();
            imageListSym.Images.Clear();
            LoadListView();

            SortByRank.Sort(listView1, tbSearch.Text);
            ColumnSort();
        }
        private void ColumnSort()
        {
            //sort list view items and set column width
            //only works when the form is visible and populated

            //set the column width to the widest template text
            ColumnHeader header = new ColumnHeader();
            header.Name = "Name";
            header.TextAlign = HorizontalAlignment.Left;
            header.Width = 125;
            listView1.Columns.Add(header);

            listView1.View = View.Details;
            listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            int width = listView1.Columns[0].Width;
            listView1.View = View.SmallIcon;
            listView1.Columns[0].Width = System.Math.Max(width, 10);

            //select the current template item
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem lvi = listView1.Items[i];
                if ((IEditTemplate)lvi.Tag == _currentTemplate)
                {
                    listView1.Focus();
                    lvi.Focused = true;
                    lvi.Selected = true;
                    listView1.EnsureVisible(lvi.Index);
                    return;
                }
            }
        }
        private class ListViewGroupSorter : IComparer
        //ICompare sorter for listviewgroup
        {
            private SortOrder order;

            public ListViewGroupSorter(SortOrder theOrder)
            {
                order = theOrder;
            }

            // Compares the groups by header value, using the saved sort
            // order to return the correct value.
            public int Compare(object x, object y)
            {
                int result = String.Compare(
                    ((ListViewGroup)x).Header,
                    ((ListViewGroup)y).Header
                );
                if (order == SortOrder.Ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }

        private class SortByRank : System.Collections.IComparer
        //ICompare sorter for search templates. Return by search rank.
        {
            public static void Sort(ListView list, string filterTypeStr)
            {
                SortByRank sorter = new SortByRank(list, filterTypeStr);
            }

            private SortByRank(ListView list, string filterTypeStr)
            {
                m_list = list;

                if (!list.IsHandleCreated)
                    return;

                List<int> removeIndex = new List<int>();

                // Replace item data with pointer with new RankTemplate struct.
                for (int i = 0; i < m_list.Items.Count; i++)
                {
                    IEditTemplate template = m_list.Items[i].Tag as IEditTemplate;
                    int nName, nTags, nLayerName, nDesc, nPartial;
                    if (!AnalyzeItem(filterTypeStr, template, out nName, out nTags, out nLayerName, out nDesc, out nPartial))
                    {
                        removeIndex.Insert(0, i);
                        continue;
                    }

                    m_list.Items[i].Tag = new RankTemplate(FindRank(nName, nTags, nLayerName, nDesc, nPartial), template);
                }

                for (int i = 0; i < removeIndex.Count(); i++)
                    m_list.Items.RemoveAt(removeIndex[i]);

                IComparer oldComparer = m_list.ListViewItemSorter;
                m_list.ListViewItemSorter = this as IComparer; //assigning performs the sort
                m_list.ListViewItemSorter = null;

                // Restore item data from RankTemplate struct.
                for (int i = 0; i < m_list.Items.Count; i++)
                {
                    RankTemplate rankTemplate = m_list.Items[i].Tag as RankTemplate;
                    if (rankTemplate != null)
                        m_list.Items[i].Tag = rankTemplate.Template;
                }
            }

            public int Compare(object x, object y)
            {
                //The comparison function must return a negative value if the first item should precede the second, a positive value if the first item should follow the second, or zero if the two items are equivalent.
                RankTemplate xx = ((ListViewItem)x).Tag as RankTemplate;
                RankTemplate yy = ((ListViewItem)y).Tag as RankTemplate;
                if (xx.Rank > yy.Rank)
                    return -1;
                else if (xx.Rank < yy.Rank)
                    return 1;
                else
                    return 0;
            }

            private class RankTemplate
            {
                public RankTemplate(double rank, IEditTemplate template)
                {
                    _rank = rank;
                    _template = template;
                }
                private double _rank;
                private IEditTemplate _template;
                public double Rank { get { return _rank; } }
                public IEditTemplate Template { get { return _template; } }
            }

            private enum Match
            {
                eFull,
                ePartial,
                eNone
            };

            private ListView m_list;

            static private double FindRank(int inName, int inTags, int inLayerName, int inDesc, int inPartial)
            {
                return inName * 1.0 + inTags * 0.5 + inLayerName * 0.25 + inDesc * 0.1 + inPartial * 0.05;
            }
            static private bool AnalyzeItem(string filterTypeStr, IEditTemplate template, out int nName, out int nTags, out int nLayerName, out int nDesc, out int nPartial)
            {
                nName = nTags = nLayerName = nDesc = nPartial = 0;
                foreach (string word in filterTypeStr.ToUpperInvariant().Split(' '))
                {
                    int inName = 0, inTags = 0, inLayerName = 0, inDesc = 0, inPartial = 0;
                    if (AnalyzeItem2(word, template, ref inName, ref inTags, ref inLayerName, ref inDesc, ref inPartial))
                    {
                        nName += inName;
                        nTags += inTags;
                        nLayerName += inLayerName;
                        nDesc += inDesc;
                        nPartial += inPartial;
                    }
                    else
                        return false;
                }
                return nName + nTags + nLayerName + nDesc + nPartial > 0;
            }

            static private Match MatchSearchTerm(string s, string searchTerm)
            {
                s = s.ToUpperInvariant();
                bool partial = false;
                int pos = s.IndexOf(searchTerm);
                while (pos != -1)
                {
                    partial = true;
                    if (pos == 0 || !Char.IsLetter(s.ElementAt(pos - 1)))
                        return Match.eFull;

                    pos = s.IndexOf(searchTerm, pos + 1);
                }

                if (partial)
                    return Match.ePartial;

                return Match.eNone;
            }
            static private bool AnalyzeItem2(string searchTerm, IEditTemplate template, ref int inName, ref int inTags, ref int inLayerName, ref int inDesc, ref int inPartial)
            {
                inName = inTags = inLayerName = inDesc = inPartial = 0;

                Match match = MatchSearchTerm(template.Name, searchTerm);
                if (match == Match.eFull)
                    inName += 1;
                else if (match == Match.ePartial)
                    inPartial += 1;

                match = MatchSearchTerm(template.Tags, searchTerm);
                if (match == Match.eFull)
                    inTags += 1;
                else if (match == Match.ePartial)
                    inPartial += 1;

                ILayer layer = template.Layer;
                if (layer != null)
                {
                    match = MatchSearchTerm(layer.Name, searchTerm);
                    if (match == Match.eFull)
                        inLayerName += 1;
                    else if (match == Match.ePartial)
                        inPartial += 1;
                }

                match = MatchSearchTerm(template.Description, searchTerm);
                if (match == Match.eFull)
                    inDesc += 1;
                else if (match == Match.ePartial)
                    inPartial += 1;

                return inName + inDesc + inTags + inLayerName + inPartial > 0;
            }
        }
    }
}
