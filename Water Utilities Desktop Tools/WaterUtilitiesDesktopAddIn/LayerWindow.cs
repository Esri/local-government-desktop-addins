
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using A4LGSharedFunctions;


namespace A4WaterUtilities
{
    public partial class LayerWindow : UserControl
    {
       //private static IApplication _app;
        private static Label s_lblCount;
        private static TextBox s_txtScale;
        private static UserControl s_userControl;
        private static TextBox s_txtQuery;
        private static TabControl s_tbCntlDisplay;
        private static GroupBox s_gpBoxOptions;
        private static SplitContainer s_splContMain;
        private static CheckBox s_chkZoomToOnAdvance;
        private static ComboBox s_cboLayers;
        private static System.Windows.Forms.Button s_BtnNext;
        private static System.Windows.Forms.Button s_BtnPrevious;
        private static System.Windows.Forms.Button s_BtnRefresh;
        private static System.Windows.Forms.Button s_BtnZoomTo;
        private static System.Drawing.Font c_Fnt = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        private static System.Drawing.Font c_FntLbl = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        private static System.Drawing.Font c_FntSmall = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
        private static int c_ControlWidth = 200;
        private static int v_ViewerRecordIndex;
        private static IFeatureLayer v_ViewerLayer;
        private static IFeatureCursor v_ViewerLayerCursor;
        private static ArrayList v_ViewerLayerCursorArray;
        private static LayerViewerConfig m_layCfg;
        private static string v_CurrentLayerText;
        public LayerWindow(object hook)
        {
            
            InitializeComponent();
            try
            {
                ConfigUtil.type = "water";
                s_userControl = this;
                s_userControl.Resize += tbCntlDisplay_Resize;
                s_tbCntlDisplay = tbCntlDisplay;
                s_gpBoxOptions = gpBoxOptions;
            
                s_cboLayers = cboLayers;
                s_BtnNext = btnNext;
                s_BtnRefresh = btnRefresh;
                s_BtnZoomTo = btnZoomTo;
                s_BtnPrevious = btnPrevious;
                s_splContMain = splContMain;
                s_chkZoomToOnAdvance = chkZoomToOnAdvance;
                s_txtScale = txtScale;
                s_txtQuery = txtQuery;
                s_lblCount = lblCount;
                s_lblCount.Text = "";
                s_splContMain.Resize += s_splContMain_Resize;

                s_BtnNext.Click += s_BtnNextClick;
                s_BtnRefresh.Click += s_BtnRefreshClick;
                s_BtnZoomTo.Click += s_BtnZoomToClick;
                s_BtnPrevious.Click += s_BtnPreviousClick;
                s_cboLayers.SelectedIndexChanged += s_cboLayers_SelectedIndexChanged;
                s_gpBoxOptions.Click += s_gpBoxOptions_Click;
                
                // Add any initialization after the InitializeComponent() call.
                this.Hook = hook;
                 m_layCfg = ConfigUtil.GetLayerViewerConfig();
                 if (m_layCfg != null)
                {
                    s_chkZoomToOnAdvance.Checked = m_layCfg.ZoomOnRecordChange;
                    
                    //if (m_node.Attributes["zoomScale"] != null)
                    //{
                    //    s_txtScale.Text = m_node.Attributes["zoomScale"].Value;

                    //}
                }
             //   _app = (IApplication)(hook);
             //   IDocumentEvents_Event s_docEvent = (IDocumentEvents_Event)(IMxDocument)_app.Document;
              
                ArcMap.Events.NewDocument += ArcMap_NewDocument;
                ArcMap.Events.OpenDocument += ArcMap_OpenDocument;
              //  s_docEvent.NewDocument += ArcMap_NewDocument;
               // s_docEvent.OpenDocument += ArcMap_OpenDocument;

              
                Initialize();



                initAddin();
                s_gpBoxOptions.Height = 15;
                CenterButton();
                setButtonState();
                ReloadMonitor.reloadConfig += new ReloadEventHandler(reloadOccured);
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_1") + Environment.NewLine + ex.Message);

            }
        }
         
        
        private void reloadOccured(object sender, EventArgs e)
        {
            ConfigUtil.type = "water";
            m_layCfg = ConfigUtil.GetLayerViewerConfig();
            if (m_layCfg != null)
            {
                s_chkZoomToOnAdvance.Checked = m_layCfg.ZoomOnRecordChange;

            }
            initAddin();
            s_gpBoxOptions.Height = 15;
            CenterButton();
            setButtonState();
        }

        //private object m_hook;


        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }
    
        private void Initialize()
        {
            ConfigUtil.type = "water";
            // Reset event handlers
           // IActiveViewEvents_Event avEvent = ((IMxDocument)_app.Document).FocusMap as IActiveViewEvents_Event;
            IActiveViewEvents_Event avEvent = (IActiveViewEvents_Event)ArcMap.Document.FocusMap;
            if (avEvent == null) return;
            try
            {//Error here about RCW disconnected
                avEvent.ItemAdded += AvEvent_ItemAdded;
                avEvent.ItemDeleted += AvEvent_ItemDeleted;

            }
            catch//( Exception ex)
            { }
        }

        #region "Events"
        private void ArcMap_NewDocument()
        {
            IActiveViewEvents_Event pageLayoutEvent = (IActiveViewEvents_Event)ArcMap.Document.PageLayout;
        
            //IActiveViewEvents_Event pageLayoutEvent = ((IMxDocument)_app.Document).PageLayout as IActiveViewEvents_Event;
            pageLayoutEvent.FocusMapChanged += AVEvents_FocusMapChanged;

            //  Initialize()
            initAddin();

        }
        private void ArcMap_OpenDocument()
        {
           // IActiveViewEvents_Event pageLayoutEvent = ((IMxDocument)_app.Document).PageLayout as IActiveViewEvents_Event;
            IActiveViewEvents_Event pageLayoutEvent = (IActiveViewEvents_Event)ArcMap.Document.PageLayout;
        
            pageLayoutEvent.FocusMapChanged += AVEvents_FocusMapChanged;

            Initialize();
            initAddin();

        }
        private void avEvent_ContentsChanged()
        {
            Initialize();
            initAddin();

        }

        private void AvEvent_ItemAdded(object Item)
        {
            initAddin();

        }
        private void AvEvent_ItemDeleted(object Item)
        {
            initAddin();

        }
        private void AVEvents_FocusMapChanged()
        {
            initAddin();
        }

        private static void s_BtnNextClick(System.Object sender, System.EventArgs e)
        {

            IFeature pFeat = default(IFeature);

            try
            {
                v_ViewerRecordIndex = v_ViewerRecordIndex + 1;
                s_lblCount.Text = v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                pFeat = v_ViewerLayerCursorArray[v_ViewerRecordIndex] as IFeature;
                LoadFeatureToViewer(pFeat);
                if (s_chkZoomToOnAdvance.Checked)
                {
                    Globals.CenterMapOnFeatureWithScale(pFeat, ArcMap.Application, Convert.ToDouble(s_txtScale.Text));
                }

                setButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_2") + Environment.NewLine + ex.Message);

            }
            finally
            {
                pFeat = null;
            }
        }
        private static void s_BtnZoomToClick(System.Object sender, System.EventArgs e)
        {

            IFeature pFeat = default(IFeature);

            try
            {
                pFeat = v_ViewerLayerCursorArray[v_ViewerRecordIndex] as IFeature;
                 
                    Globals.CenterMapOnFeatureWithScale(pFeat, ArcMap.Application, Convert.ToDouble(s_txtScale.Text));
                

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_3") + Environment.NewLine + ex.Message);

            }
            finally
            {
                pFeat = null;
            }
        }
        private static void s_BtnPreviousClick(System.Object sender, System.EventArgs e)
        {
            IFeature pFeat = default(IFeature);
            try
            {
                v_ViewerRecordIndex = v_ViewerRecordIndex - 1;
                s_lblCount.Text = v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                pFeat = v_ViewerLayerCursorArray[v_ViewerRecordIndex] as IFeature;
                LoadFeatureToViewer(pFeat);
                if (s_chkZoomToOnAdvance.Checked)
                {
                    Globals.CenterMapOnFeatureWithScale(pFeat, ArcMap.Application, s_txtScale.Text);
                }



                setButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_4") + Environment.NewLine + ex.Message);

            }
            finally
            {
                pFeat = null;

            }
        }
        private static void s_BtnRefreshClick(System.Object sender, System.EventArgs e)
        {
            IFeature pFeat = default(IFeature);

            try
            {

                if (LoadCursor() == false)
                {
                    v_ViewerRecordIndex = -1;
                    s_lblCount.Text = A4LGSharedFunctions.Localizer.GetString("FeatNotFound");//v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                    v_ViewerLayerCursorArray.Clear();
                    v_ViewerLayerCursor = null;
                    setButtonState();
                    return;

                }
                if (v_ViewerLayerCursorArray == null) {
                    v_ViewerRecordIndex = -1;
                    s_lblCount.Text = A4LGSharedFunctions.Localizer.GetString("FeatNotFound");//v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                    v_ViewerLayerCursor = null;
                    setButtonState();
                    return;
                }
                if (v_ViewerLayerCursorArray.Count == 0)
                {
                    v_ViewerRecordIndex = -1;
                    s_lblCount.Text = A4LGSharedFunctions.Localizer.GetString("FeatNotFound");//v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                    v_ViewerLayerCursor = null;
                    setButtonState();
                    return;
                }

                v_ViewerRecordIndex = 0;
                s_lblCount.Text = v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                pFeat = v_ViewerLayerCursorArray[v_ViewerRecordIndex] as IFeature;


                LoadFeatureToViewer(pFeat);
                if (s_chkZoomToOnAdvance.Checked)
                {
                    Globals.CenterMapOnFeatureWithScale(pFeat, ArcMap.Application, s_txtScale.Text);
                }

                setButtonState();

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_5") + Environment.NewLine + ex.Message);

            }
            finally
            {
                pFeat = null;

            }
        }
        private static void s_splContMain_Resize(System.Object sender, System.EventArgs e)
        {
            if (s_gpBoxOptions.Height < 20)
            {
                s_splContMain.SplitterDistance = s_splContMain.Height - 65;
            }
            else
            {
                s_splContMain.SplitterDistance = s_splContMain.Height - 125;
            }
            CenterButton();
        }

        #endregion
        #region "Private Functions"

        private static void initAddin()
        {

            ConfigUtil.type = "water";
            try
            {
                try
                {
                    if (ArcMap.Document==null )
                        return;
                }
                catch (InvalidComObjectException ex)
                {
                    //the ArcMap Object is released alreay
                    return;
                }
                if (ArcMap.Document.FocusMap == null) return;
                s_cboLayers.SelectedIndexChanged -= s_cboLayers_SelectedIndexChanged;
                int bCurrentLayerIdx = -1;
                string strExistingCBOVal = s_cboLayers.Text;

               // bool bLayStillValid = false;
                //ArrayList pTmpArrList = new ArrayList();

                //for (int i = 0; i < m_layCfg.LayerViewerLayer.Length; i++)
                //{
                //    if (Globals.LayerExist((ArcMap.Document).FocusMap, (m_layCfg.LayerViewerLayer[i]).LayerName))
                //    {
                //        pTmpArrList.Add(m_layCfg.LayerViewerLayer[i]);

                //        if ((m_layCfg.LayerViewerLayer[i]).LayerName == strExistingCBOVal)
                //        {
                //        //    bLayStillValid = true;
                //        }
                //    }
                //}
                //if (pTmpArrList.Count == 0)
                //    return;
                s_cboLayers.DisplayMember = "LayerName";
                s_cboLayers.ValueMember = "QueryAndZoom";
                if (m_layCfg != null)
                {
                    s_cboLayers.DataSource = m_layCfg.LayerViewerLayer;
                }
                if (s_cboLayers.Items.Count == 0)
                    return;

                if (bCurrentLayerIdx > -1)
                {
                    s_cboLayers.Text = strExistingCBOVal;
                    s_cboLayers.SelectedIndexChanged += s_cboLayers_SelectedIndexChanged;
                }
                else
                {
                    s_cboLayers.SelectedIndexChanged += s_cboLayers_SelectedIndexChanged;

                    s_cboLayers.SelectedItem = s_cboLayers.Items[0];
                    s_cboLayers_SelectedIndexChanged(null, null);

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LV_1") + Environment.NewLine + ex.Message);


            }

        }
        private static bool LoadCursor()
        {
            if (v_ViewerLayer == null )return false;

            IQueryFilter pQFilt = default(IQueryFilter);
            IFeature pFeat = default(IFeature);
            try
            {
                if (v_ViewerLayerCursorArray == null)
                {
                    v_ViewerLayerCursorArray = new ArrayList();
                }
                else
                {
                    v_ViewerLayerCursorArray.Clear();
                }


                pQFilt = new QueryFilter();
                string[] pstr = s_cboLayers.SelectedValue.ToString().Split(',');


                pQFilt.WhereClause = pstr[0];
                s_txtScale.Text = pstr[1];
                s_txtQuery.Text = pstr[0];

                if (v_ViewerLayer.FeatureClass.FeatureCount(pQFilt) == 0)
                    return false;

                v_ViewerLayerCursor = v_ViewerLayer.Search(pQFilt, false);

                pFeat = v_ViewerLayerCursor.NextFeature();

                while (!(pFeat == null))
                {

                    v_ViewerLayerCursorArray.Add(pFeat);
                    pFeat = v_ViewerLayerCursor.NextFeature();
                }
                pQFilt = null;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_7") + Environment.NewLine + ex.Message);

                return false;
            }
            finally
            {
                pQFilt = null;
                pFeat = null;
            }

        }
        private static void ShuffleControls(bool Vertical)
        {
            if (s_tbCntlDisplay == null)
                return;
            if (s_tbCntlDisplay.SelectedIndex == -1)
                return;
            if (s_tbCntlDisplay.TabPages.Count == 0)
                return;

            if (Vertical)
            {
                try
                {
                    //Spacing between last control and the bottom of the page
                    int pBottomPadding = 120;
                    //Padding for the left of each control
                    int pLeftPadding = 10;
                    //Spacing between firstcontrol and the top
                    int pTopPadding = 3;
                    //Padding for the right of each control
                    int pRightPadding = 15;

                    int pCurTabIdx = s_tbCntlDisplay.SelectedIndex;

                    TabPage[] pTbPageCo = null;

                    TabPage pCurTabPage = new TabPage();
                    //pCurTabPage.Name = strName
                    //pCurTabPage.Text = strName
                    int pCntlNextTop = pTopPadding;

                    foreach (TabPage tb in s_tbCntlDisplay.TabPages)
                    {

                        bool bLoop = true;

                        while (bLoop == true)
                        {
                            if (tb.Controls.Count == 0)
                            {
                                break; // TODO: might not be correct. Was : Exit While

                            }

                            Control cnt = tb.Controls[0];

                            if (cnt is System.Windows.Forms.Button)
                            {
                                tb.Controls.Remove(cnt);


                            }
                            else
                            {

                                cnt.Top = pCntlNextTop;
                                cnt.Width = s_tbCntlDisplay.Width;
                                if (cnt is Panel)
                                {

                                    foreach (Control pnlCnt in cnt.Controls)
                                    {
                                        if (pnlCnt is System.Windows.Forms.Button)
                                        {
                                            Control[] controls = ((Panel)((System.Windows.Forms.Button)pnlCnt).Parent).Controls.Find("txtEdit" + pnlCnt.Tag, false);
                                            if (controls.Length == 1)
                                            {
                                                controls[0].Width = controls[0].Width - pnlCnt.Width - 5;
                                                pnlCnt.Left = controls[0].Width + controls[0].Left + 5;

                                            }
                                        }
                                        else if (pnlCnt is CustomPanel)
                                        {
                                            pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding;
                                            if (pnlCnt.Controls.Count == 2)
                                            {
                                                pnlCnt.Controls[0].Left = pLeftPadding;
                                                pnlCnt.Controls[1].Left = (pnlCnt.Width / 2);
                                            }

                                        }
                                        else
                                        {
                                            pnlCnt.Width = s_tbCntlDisplay.Width - pLeftPadding - pRightPadding;
                                        }

                                        //End If


                                    }

                                }

                                pCurTabPage.Controls.Add(cnt);
                                pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding;
                                if (pCntlNextTop >= s_tbCntlDisplay.Height - pBottomPadding)
                                {
                                    //Dim pBtn As System.Windows.Forms.Button
                                    //pBtn = New System.Windows.Forms.Button
                                    //pBtn.Name = "btnSaveEdit"
                                    //pBtn.Text = "Save"
                                    //pBtn.Font =c_Fnt
                                    //pBtn.Top = pCntlNextTop
                                    //pBtn.AutoSize = True

                                    //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                    //pCurTabPage.Controls.Add(pBtn)
                                    //pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                                    //pBtn = New System.Windows.Forms.Button
                                    //pBtn.Name = "btnClearEdit"
                                    //pBtn.Text = "Clear"
                                    //pBtn.Font =c_Fnt
                                    //pBtn.Top = pCntlNextTop
                                    //pBtn.AutoSize = True

                                    //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                    //pCurTabPage.Controls.Add(pBtn)
                                    //pBtn.Left = (tbCntCIPDetails.Width / 2) + 10


                                    if (pTbPageCo == null)
                                    {
                                        System.Array.Resize(ref pTbPageCo, 1);
                                    }
                                    else
                                    {
                                        System.Array.Resize(ref pTbPageCo, pTbPageCo.Length + 1);
                                    }
                                    pTbPageCo[pTbPageCo.Length - 1] = pCurTabPage;
                                    pCurTabPage = new TabPage();


                                    //pCurTabPage.Name = strName
                                    //pCurTabPage.Text = strName

                                    pCntlNextTop = pTopPadding;
                                    //pBtn = Nothing

                                }
                            }
                        }
                    }



                    if (pCurTabPage.Controls.Count > 0)
                    {
                        //Dim pBtn As System.Windows.Forms.Button
                        //pBtn = New System.Windows.Forms.Button
                        //pBtn.Name = "btnSaveEdit"
                        //pBtn.Text = "Save"
                        //pBtn.Font =c_Fnt
                        //pBtn.Top = pCntlNextTop
                        //pBtn.AutoSize = True

                        //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        //pCurTabPage.Controls.Add(pBtn)
                        //pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                        //pBtn = New System.Windows.Forms.Button
                        //pBtn.Name = "btnClearEdit"
                        //pBtn.Text = "Clear"
                        //pBtn.Font =c_Fnt
                        //pBtn.Top = pCntlNextTop
                        //pBtn.AutoSize = True

                        //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        //pCurTabPage.Controls.Add(pBtn)
                        //pBtn.Left = (tbCntCIPDetails.Width / 2) + 10
                        if (pTbPageCo == null)
                        {
                            System.Array.Resize(ref pTbPageCo, 1);
                        }
                        else
                        {
                            System.Array.Resize(ref pTbPageCo, pTbPageCo.Length + 1);
                        }

                        pTbPageCo[pTbPageCo.Length - 1] = pCurTabPage;

                    }
                    else
                    {
                    }

                    s_tbCntlDisplay.TabPages.Clear();

                    foreach (TabPage tbp in pTbPageCo)
                    {
                        s_tbCntlDisplay.TabPages.Add(tbp);

                        tbp.Visible = true;

                        tbp.Update();
                    }
                    if (s_tbCntlDisplay.TabPages.Count >= pCurTabIdx)
                    {
                        s_tbCntlDisplay.SelectedIndex = pCurTabIdx;
                    }
                    else
                    {
                        s_tbCntlDisplay.SelectedIndex = s_tbCntlDisplay.TabPages.Count - 1;
                    }

                    pTbPageCo = null;
                    pCurTabPage = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("CT_1") + Environment.NewLine + ex.Message);

                }


                //horizontel
            }
            else
            {
                try
                {
                    //Spacing between last control and the bottom of the page
                    int pBottomPadding = 12;
                    //Padding for the left of each control
                    int pLeftPadding = 10;
                    //Spacing between firstcontrol and the top
                    int pTopPadding = 3;
                    //Padding for the right of each control
                    int pRightPadding = 15;
                    int pCntSpacing = 5;
                    int pCurTabIdx = s_tbCntlDisplay.SelectedIndex;

                    TabPage[] pTbPageCo = null;
                    TabPage pCurTabPage = new TabPage();
                    pCurTabPage.Name = "Page 1";
                    pCurTabPage.Text = "Page 1";
                    int pCntlNextTop = pTopPadding;
                    int pCntlNextLeft = pLeftPadding;

                    foreach (TabPage tb in s_tbCntlDisplay.TabPages)
                    {

                        bool bLoop = true;

                        while (bLoop == true)
                        {
                            if (tb.Controls.Count == 0)
                            {
                                break; // TODO: might not be correct. Was : Exit While

                            }

                            Control cnt = tb.Controls[0];

                            if (cnt is System.Windows.Forms.Button)
                            {
                                tb.Controls.Remove(cnt);


                            }
                            else
                            {

                                cnt.Top = pCntlNextTop;
                                cnt.Left = pCntlNextLeft;
                                cnt.Width = c_ControlWidth;
                                if (cnt is Panel)
                                {

                                    foreach (Control pnlCnt in cnt.Controls)
                                    {
                                        if (pnlCnt is System.Windows.Forms.Button)
                                        {
                                            Control[] controls = ((Panel)((System.Windows.Forms.Button)pnlCnt).Parent).Controls.Find("txtEdit" + pnlCnt.Tag, false);
                                            if (controls.Length == 1)
                                            {
                                                controls[0].Width = cnt.Width - cnt.Height - 5;
                                                pnlCnt.Left = controls[0].Width + controls[0].Left + 5;

                                            }
                                        }
                                        else if (pnlCnt is CustomPanel)
                                        {
                                            pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding;
                                            if (pnlCnt.Controls.Count == 2)
                                            {
                                                pnlCnt.Controls[0].Left = pLeftPadding;
                                                pnlCnt.Controls[1].Left = (pnlCnt.Width / 2);
                                            }

                                        }
                                        else
                                        {
                                            pnlCnt.Width = cnt.Width - pLeftPadding - pRightPadding;
                                        }

                                        //End If


                                    }

                                }


                                //If pCntlNextTop + cnt.Height + pTopPadding >= s_tbCntlDisplay.Height - pBottomPadding Then
                                if (pCntlNextTop + cnt.Height + pTopPadding >= s_splContMain.Parent.Height - s_splContMain.Panel2.Height - pBottomPadding - pBottomPadding)
                                {
                                    //Dim pBtn As System.Windows.Forms.Button
                                    //pBtn = New System.Windows.Forms.Button
                                    //pBtn.Name = "btnSaveEdit"
                                    //pBtn.Text = "Save"
                                    //pBtn.Font =c_Fnt
                                    //pBtn.Top = pCntlNextTop
                                    //pBtn.AutoSize = True

                                    //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                    //pCurTabPage.Controls.Add(pBtn)
                                    //pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                                    //pBtn = New System.Windows.Forms.Button
                                    //pBtn.Name = "btnClearEdit"
                                    //pBtn.Text = "Clear"
                                    //pBtn.Font =c_Fnt
                                    //pBtn.Top = pCntlNextTop
                                    //pBtn.AutoSize = True

                                    //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                    //pCurTabPage.Controls.Add(pBtn)
                                    //pBtn.Left = (tbCntCIPDetails.Width / 2) + 10

                                    //If pCntlNextLeft + pCntSpacing + (My.Globals.Constants.c_ControlWidth * 2) > s_tbCntlDisplay.Width Then
                                    if (pCntlNextLeft + pCntSpacing + (c_ControlWidth * 2) > s_splContMain.Parent.Width)
                                    {
                                        if (pTbPageCo == null)
                                        {
                                            System.Array.Resize(ref pTbPageCo, 1);
                                        }
                                        else
                                        {
                                            System.Array.Resize(ref pTbPageCo, pTbPageCo.Length + 1);
                                        }
                                        pTbPageCo[pTbPageCo.Length - 1] = pCurTabPage;

                                        pCurTabPage = new TabPage();
                                        int pgNum = pTbPageCo.Length + 1;
                                        pCurTabPage.Name = A4LGSharedFunctions.Localizer.GetString("Page") + pgNum;
                                        pCurTabPage.Text = A4LGSharedFunctions.Localizer.GetString("Page") + pgNum;

                                        pCntlNextTop = pTopPadding;
                                        pCntlNextLeft = pLeftPadding;
                                    }
                                    else
                                    {
                                        pCntlNextTop = pTopPadding;
                                        pCntlNextLeft = pCntlNextLeft + c_ControlWidth + pCntSpacing;
                                    }

                                    cnt.Top = pCntlNextTop;
                                    cnt.Left = pCntlNextLeft;
                                    pCurTabPage.Controls.Add(cnt);
                                    pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding;
                                    //pBtn = Nothing
                                }
                                else
                                {
                                    pCurTabPage.Controls.Add(cnt);
                                    pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding;

                                }
                            }
                        }
                    }



                    if (pCurTabPage.Controls.Count > 0)
                    {
                        //Dim pBtn As System.Windows.Forms.Button
                        //pBtn = New System.Windows.Forms.Button
                        //pBtn.Name = "btnSaveEdit"
                        //pBtn.Text = "Save"
                        //pBtn.Font =c_Fnt
                        //pBtn.Top = pCntlNextTop
                        //pBtn.AutoSize = True

                        //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        //pCurTabPage.Controls.Add(pBtn)
                        //pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                        //pBtn = New System.Windows.Forms.Button
                        //pBtn.Name = "btnClearEdit"
                        //pBtn.Text = "Clear"
                        //pBtn.Font =c_Fnt
                        //pBtn.Top = pCntlNextTop
                        //pBtn.AutoSize = True

                        //AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        //pCurTabPage.Controls.Add(pBtn)
                        //pBtn.Left = (tbCntCIPDetails.Width / 2) + 10
                        if (pTbPageCo == null)
                        {
                            System.Array.Resize(ref pTbPageCo, 1);
                        }
                        else
                        {
                            System.Array.Resize(ref pTbPageCo, pTbPageCo.Length + 1);
                        }

                        pTbPageCo[pTbPageCo.Length - 1] = pCurTabPage;

                    }
                    else
                    {
                    }

                    s_tbCntlDisplay.TabPages.Clear();

                    foreach (TabPage tbp in pTbPageCo)
                    {
                        s_tbCntlDisplay.TabPages.Add(tbp);

                        tbp.Visible = true;

                        tbp.Update();
                    }
                    if (s_tbCntlDisplay.TabPages.Count >= pCurTabIdx)
                    {
                        s_tbCntlDisplay.SelectedIndex = pCurTabIdx;
                    }
                    else
                    {
                        s_tbCntlDisplay.SelectedIndex = s_tbCntlDisplay.TabPages.Count - 1;
                    }

                    pTbPageCo = null;
                    pCurTabPage = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("CT_1") + Environment.NewLine + ex.Message);

                }

            }

        }
        private static void AddControlsDisplay()
        {

            try
            {



                //Exit if the layer is not found
                if (v_ViewerLayer == null)
                    return;
                if (v_ViewerLayer.FeatureClass == null)
                    return;

                //int pLeftPadding = 10;

                //Clear out the controls from the container
                s_tbCntlDisplay.TabPages.Clear();
                s_tbCntlDisplay.Controls.Clear();
                TabPage pTbPg = new TabPage();
                s_tbCntlDisplay.TabPages.Add(pTbPg);


                //Controls to display attributes
                //Dim pTbPg As TabPage = Nothing
                //TextBox pTxtBox = default(TextBox);
               // Label pLbl = default(Label);

              //  DateTimePicker pDateTime = default(DateTimePicker);
                //Spacing between each control
              //  int intCtrlSpace = 5;
                //Spacing between a lable and a control
             //  int intLabelCtrlSpace = 0;


                //Set the width of each control
                //   Dim my.Globals.Constants.c_ControlWidth As Integer = 50
                //used for sizing text, only used when text is larger then display
                //Graphics g = default(Graphics);
                //SizeF s = default(SizeF);


                //Used to loop through featurelayer
                IFields pDCs = default(IFields);
                IField pDc = default(IField);
                int pSubTypeDefValue = 0;

                //Get the columns for hte layer
                pDCs = v_ViewerLayer.FeatureClass.Fields;
                ISubtypes pSubType = (ISubtypes)v_ViewerLayer.FeatureClass;
                if (pSubType.HasSubtype)
                {
                    pSubTypeDefValue = pSubType.DefaultSubtypeCode;
                    //pfl.Columns(pfl.SubtypeColumnIndex).DefaultValue
                }


                //Field Name
                string strfld = null;
                //Field Alias
                string strAli = null;


                //IDomain pDom = null;
                for (int i = 0; i <= pDCs.FieldCount - 1; i++)
                {
                    pDc = pDCs.get_Field(i);
                    ILayerFields pLayerFields = default(ILayerFields);
                    IFieldInfo pFieldInfo = default(IFieldInfo);

                    pLayerFields = (ILayerFields)v_ViewerLayer;
                    pFieldInfo = pLayerFields.get_FieldInfo(pLayerFields.FindField(pDc.Name));
                    //  pFieldInfo.Visible = False

                    if (pFieldInfo.Visible == false)
                    {

                    }
                    else
                    {


                        //Get the field names
                        strfld = pDc.Name;
                        strAli = pDc.AliasName;


                        //Check the field types

                        if (v_ViewerLayer.FeatureClass.ShapeFieldName == strfld ||
                            v_ViewerLayer.FeatureClass.OIDFieldName == strfld ||
                            (strfld).ToUpper() == ("shape.len").ToUpper() ||
                                (strfld).ToUpper() == ("shape.area").ToUpper() ||
                                (strfld).ToUpper() == ("shape_length").ToUpper() ||
                                (strfld).ToUpper() == ("shape_len").ToUpper() ||
                                (strfld).ToUpper() == ("shape_area").ToUpper())
                        {


                        }
                    }
                }
                //pDC


                pDCs = null;
                pDc = null;

                pTbPg = null;

                //pTxtBox = null;
                //pLbl = null;
               // pDateTime = null;
                //g = null;
                // s = null;
                s_tbCntlDisplay.ResumeLayout();
                s_tbCntlDisplay.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("CT_2") + Environment.NewLine + ex.Message);

            }

        }
        private static string LoadFeatureToViewer(IFeature pFeat)
        {
            if (pFeat == null)
                return "";

            //    Dim pFCurs As IFeatureCursor = Nothing

            IFeatureClass pFC = null;
            string strPrjName = "";
            string strFld = "";

            ISubtypes pSubType = null;
            bool bSubType = false;
            // Dim pSFilt As ISpatialFilter = Nothing

            try
            {
                int pSubValue = 0;
                pFC = (IFeatureClass)pFeat.Class;
                pSubType = (ISubtypes)pFC;
                bSubType = pSubType.HasSubtype;
                //If the layer has subtypes, load the subtype value first
                if (bSubType)
                {
                    pSubValue = (int)pFeat.get_Value(pSubType.SubtypeFieldIndex);

                    //Loop through each control in the tab control
                    foreach (Control pCntrl in s_tbCntlDisplay.Controls)
                    {
                        //If the control is a tabpage
                        if (pCntrl is TabPage)
                        {
                            //Loop through each ocntrol on the tab oage
                            foreach (Control cCntrl in pCntrl.Controls)
                            {
                                //If the control is a combo box(used for domains)

                                if (cCntrl is Panel)
                                {
                                    foreach (Control cCntrlPnl in cCntrl.Controls)
                                    {
                                        if (cCntrlPnl is ComboBox)
                                        {
                                            //Get the field
                                            strFld = ((ComboBox)cCntrlPnl).Tag.ToString();
                                            //Make sure no link is specified
                                            if (strFld.IndexOf("|") > 0)
                                            {
                                                strFld = strFld.Substring(0, strFld.IndexOf("|")).Trim();
                                            }
                                            //If the field is the subtype field
                                            if (pSubType.SubtypeFieldName == strFld)
                                            {
                                                //Set the value

                                                if (!object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                                {

                                                    ((ComboBox)cCntrlPnl).SelectedValue = pFeat.get_Value(pFeat.Fields.FindField(strFld));
                                                }
                                                else
                                                {
                                                    ((ComboBox)cCntrlPnl).SelectedIndex = 0;
                                                }
                                                //Raise the subtype change event, this loads all the proper domains based on the subtype value
                                                cmbSubTypChange_Click((ComboBox)cCntrlPnl, null);

                                                break; // TODO: might not be correct. Was : Exit For

                                            }



                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                //Loop through all the controls and set their value
                foreach (Control pCntrl in s_tbCntlDisplay.Controls)
                {
                    if (pCntrl is TabPage)
                    {
                        foreach (Control cCntrl in pCntrl.Controls)
                        {
                            //If the control is a 2 value domain(Checkboxs)
                            if (cCntrl is Panel)
                            {
                                foreach (Control cCntrlPnl in cCntrl.Controls)
                                {
                                    if (cCntrlPnl is CustomPanel)
                                    {
                                        //Get the Field
                                        strFld = ((CustomPanel)cCntrlPnl).Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = strFld.Substring(0, strFld.IndexOf("|")).Trim();
                                        }
                                        //Get the target value
                                        string pTargetVal = "";
                                        if (!object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                        {
                                            pTargetVal = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                        }
                                        else if (object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                        {
                                            if (object.ReferenceEquals(pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue, DBNull.Value))
                                            {
                                                pTargetVal = "";
                                            }
                                            else
                                            {
                                                pTargetVal = pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue.ToString();
                                                //pFL.Columns(strFld).DefaultValue
                                            }

                                        }
                                        else if (string.IsNullOrEmpty(pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString()))
                                        {
                                            if (object.ReferenceEquals(pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue, DBNull.Value))
                                            {
                                                pTargetVal = "";
                                            }
                                            else
                                            {
                                                pTargetVal = pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue.ToString();
                                            }
                                        }
                                        else
                                        {
                                            if (object.ReferenceEquals(pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue, DBNull.Value))
                                            {
                                                pTargetVal = "";
                                            }
                                            else
                                            {
                                                pTargetVal = pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld)).DefaultValue.ToString();
                                            }
                                        }
                                        CustomPanel pCsPn = (CustomPanel)cCntrlPnl;
                                        //Loop through the checkboxes to set the proper value

                                        foreach (Control rdCn in pCsPn.Controls)
                                        {
                                            if (rdCn is RadioButton)
                                            {
                                                if (string.IsNullOrEmpty(pTargetVal))
                                                {
                                                    ((RadioButton)rdCn).Checked = true;

                                                    break; // TODO: might not be correct. Was : Exit For
                                                }
                                                if (rdCn.Tag.ToString() == pTargetVal)
                                                {
                                                    ((RadioButton)rdCn).Checked = true;

                                                    break; // TODO: might not be correct. Was : Exit For


                                                }
                                            }
                                        }
                                        //If the control is a text box
                                    }
                                    else if (cCntrlPnl is TextBox)
                                    {
                                        //Get the field
                                        strFld = ((TextBox)cCntrlPnl).Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = strFld.Substring(0, strFld.IndexOf("|")).Trim();
                                        }
                                        //Set the Value
                                        IField pFld = pFeat.Fields.get_Field(pFeat.Fields.FindField(strFld));
                                        string displayVal = "";
                                        if (object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                        {
                                            displayVal = "";
                                        }
                                        else if (pSubType != null)
                                        {
                                            if (pFld.Name == pSubType.SubtypeFieldName)
                                            {
                                                displayVal = pSubType.get_SubtypeName((int)pFeat.get_Value(pFeat.Fields.FindField(strFld)));
                                            }
                                            else
                                            {
                                                IDomain pDom = pSubType.get_Domain(pSubValue, pFld.Name);
                                                if (pDom != null)
                                                {
                                                    if (pDom is ICodedValueDomain)
                                                    {
                                                        displayVal = Globals.GetDomainValue(pFeat.get_Value(pFeat.Fields.FindField(strFld)), (ICodedValueDomain)pDom);


                                                    }
                                                    else
                                                    {
                                                        displayVal = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                                    }

                                                }
                                                else
                                                {
                                                    displayVal = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                                }
                                                pDom = null;
                                            }
                                        }
                                        else
                                        {
                                            IDomain pDom = pFld.Domain;

                                            if (pDom != null)
                                            {
                                                if (pDom is ICodedValueDomain)
                                                {
                                                    displayVal = Globals.GetDomainValue(pFeat.get_Value(pFeat.Fields.FindField(strFld)), (ICodedValueDomain)pDom);


                                                }
                                                else
                                                {
                                                    displayVal = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                                }

                                            }
                                            else
                                            {
                                                displayVal = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                            }
                                            pDom = null;
                                        }

                                        ((TextBox)cCntrlPnl).Text = displayVal;
                                        pFld = null;

                                        //if the control is a combo box(domain)
                                    }
                                    else if (cCntrlPnl is ComboBox)
                                    {
                                        //Get the field
                                        strFld = ((ComboBox)cCntrlPnl).Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = (strFld.Substring(0, strFld.IndexOf("|"))).ToString();
                                        }
                                        //Skip the subtype column
                                        if (pSubType.SubtypeFieldName != strFld)
                                        {
                                            //Set the value
                                            if (!object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                            {
                                                if (string.IsNullOrEmpty(pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString()) || object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                                {
                                                    //   Dim pCV As CodedValueDomain = CType(cCntrlPnl, ComboBox).DataSource
                                                    //   Dim i As Integer = pCV.Rows.Count
                                                    //If CType(cCntrlPnl, ComboBox).DataSource IsNot Nothing Then
                                                    //    CType(cCntrlPnl, ComboBox).Text = CType(cCntrlPnl, ComboBox).DataSource.Rows(0)("Value")
                                                    //End If


                                                }
                                                else
                                                {
                                                    ((ComboBox)cCntrlPnl).SelectedValue = pFeat.get_Value(pFeat.Fields.FindField(strFld));
                                                }


                                                //CType(cCntrlPnl, ComboBox).Text = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString()
                                            }
                                            else
                                            {
                                                ((ComboBox)cCntrlPnl).SelectedIndex = 0;
                                            }
                                        }
                                        //if the contorl is a data time field
                                    }
                                    else if (cCntrlPnl is DateTimePicker)
                                    {
                                        //Get the field
                                        strFld = ((DateTimePicker)cCntrlPnl).Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = strFld.Substring(0, strFld.IndexOf("|")).ToString();
                                        }
                                        //Get and set the value
                                        if (!object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                        {
                                            ((DateTimePicker)cCntrlPnl).Text = pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString();
                                            ((DateTimePicker)cCntrlPnl).Checked = true;
                                        }
                                        else
                                        {
                                            ((DateTimePicker)cCntrlPnl).Checked = false;

                                        }
                                        //If the field is a range domain
                                    }
                                    else if (cCntrlPnl is NumericUpDown)
                                    {
                                        //Get the field
                                        strFld = ((NumericUpDown)cCntrlPnl).Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = (strFld.Substring(0, strFld.IndexOf("|"))).Trim();
                                        }
                                        //Get and set the value
                                        if (object.ReferenceEquals(pFeat.get_Value(pFeat.Fields.FindField(strFld)), DBNull.Value))
                                        {
                                            ((NumericUpDown)cCntrlPnl).ReadOnly = true;
                                        }
                                        else if (Convert.ToDecimal(pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString()) > ((NumericUpDown)cCntrlPnl).Maximum ||
                                                  Convert.ToDecimal(pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString()) < ((NumericUpDown)cCntrlPnl).Minimum)
                                        {
                                            ((NumericUpDown)cCntrlPnl).ReadOnly = true;

                                        }
                                        else
                                        {
                                            ((NumericUpDown)cCntrlPnl).Value = Convert.ToDecimal(pFeat.get_Value(pFeat.Fields.FindField(strFld)).ToString());


                                        }


                                    }
                                }

                            }

                        }
                    }
                }
                //    Marshal.ReleaseComObject(pSFilt)

                return strPrjName;

            }
            catch// (Exception ex)
            {
                //  MsgBox("Error in the edit control record loader" & vbCrLf & ex.Message)
                return "";
            }
            finally
            {
                //   pSFilt = Nothing
                //   Marshal.ReleaseComObject(pFCurs)

                pSubType = null;
                pFeat = null;

                //   pFCurs = Nothing
            }
        }
        private static void AddControls(bool displayOnly)
        {

            try
            {

                s_tbCntlDisplay.TabPages.Clear();
                s_tbCntlDisplay.Controls.Clear();
           

                //Exit if the layer is not found
                if (v_ViewerLayer == null)
                    return;
                if (v_ViewerLayer.FeatureClass == null)
                    return;

                int pLeftPadding = 10;

                //Clear out the controls from the container
               // s_tbCntlDisplay.TabPages.Clear();
              //  s_tbCntlDisplay.Controls.Clear();
                TabPage pTbPg = new TabPage();
                s_tbCntlDisplay.TabPages.Add(pTbPg);


                //Controls to display attributes
                //Dim pTbPg As TabPage = Nothing
                TextBox pTxtBox = default(TextBox);
                Label pLbl = default(Label);
                NumericUpDown pNumBox = default(NumericUpDown);
                System.Windows.Forms.Button pBtn = null;
                ComboBox pCBox = default(ComboBox);
                RadioButton pRDButton = default(RadioButton);

                DateTimePicker pDateTime = default(DateTimePicker);
                //Spacing between each control
               // int intCtrlSpace = 5;
                //Spacing between a lable and a control
                //int intLabelCtrlSpace = 0;


                //Set the width of each control
                //   Dim my.Globals.Constants.c_ControlWidth As Integer = 50
                //used for sizing text, only used when text is larger then display
                Graphics g = default(Graphics);
                SizeF s = default(SizeF);


                //Used to loop through featurelayer
                IFields pDCs = default(IFields);
                IField pDc = default(IField);
                int pSubTypeDefValue = 0;

                //Get the columns for hte layer
                pDCs = v_ViewerLayer.FeatureClass.Fields;
                ISubtypes pSubType = (ISubtypes)v_ViewerLayer.FeatureClass;
                if (pSubType.HasSubtype)
                {
                    pSubTypeDefValue = pSubType.DefaultSubtypeCode;
                    //pfl.Columns(pfl.SubtypeColumnIndex).DefaultValue
                }


                //Field Name
                string strfld = null;
                //Field Alias
                string strAli = null;


                IDomain pDom = default(IDomain);
                for (int i = 0; i <= pDCs.FieldCount - 1; i++)
                {
                    pDc = (IField)pDCs.get_Field(i);
                    ILayerFields pLayerFields = default(ILayerFields);
                    IFieldInfo pFieldInfo = default(IFieldInfo);

                    pLayerFields = (ILayerFields)v_ViewerLayer;
                    pFieldInfo = pLayerFields.get_FieldInfo(pLayerFields.FindField(pDc.Name));
                    //  pFieldInfo.Visible = False

                    if (pFieldInfo.Visible == false)
                    {

                    }
                    else
                    {

                        pDom = null;

                        //Get the field names
                        strfld = pDc.Name;
                        strAli = pDc.AliasName;


                        //Check the field types

                        if (v_ViewerLayer.FeatureClass.ShapeFieldName == strfld ||
                            v_ViewerLayer.FeatureClass.OIDFieldName == strfld ||
                            (strfld).ToUpper() == ("shape.len").ToUpper() ||
                           (strfld).ToUpper() == ("shape.area").ToUpper() ||
                           (strfld).ToUpper() == ("shape_length").ToUpper() ||
                           (strfld).ToUpper() == ("shape_len").ToUpper() ||
                           (strfld).ToUpper() == ("shape_area").ToUpper() ||
                           (strfld).ToUpper() == ("LASTUPDATE").ToUpper() ||
                           (strfld).ToUpper() == ("LASTEDITOR").ToUpper())
                        {

                        }
                        else if (displayOnly)
                        {


                            //Create a lable for the field name
                            pLbl = new Label();
                            //Apply the field alias to the field name
                            pLbl.Text = strAli;
                            //Link the field to the name of the control
                            pLbl.Name = "lblEdit" + strfld;
                            //Add the control at the determined Location
                            pLbl.Left = 0;

                            pLbl.Top = 0;
                            //Apply global font
                            pLbl.Font = c_FntLbl;
                            //Create a graphics object to messure the text
                            g = pLbl.CreateGraphics();
                            s = g.MeasureString(pLbl.Text, pLbl.Font);

                            pLbl.Height = Convert.ToInt32(s.Height);
                            //If the text is larger then the control, truncate the control
                            if (s.Width >= c_ControlWidth)
                            {
                                pLbl.Width = c_ControlWidth;
                                //Use autosize if it fits
                            }
                            else
                            {
                                pLbl.AutoSize = true;
                            }

                            //Create a new control to display the attributes                    
                            pTxtBox = new TextBox();

                            //Tag the control with the field it represents
                            pTxtBox.Tag = (strfld).Trim();
                            //Name the field with the field name
                            pTxtBox.Name = "txtEdit" + strfld;
                            //Locate the control on the display
                            pTxtBox.Left = 0;
                            // pTxtBox.Enabled = False
                            pTxtBox.BackColor = Color.White;
                            pTxtBox.ReadOnly = true;

                            pTxtBox.Width = c_ControlWidth;
                            if (pDc.Type == esriFieldType.esriFieldTypeString)
                            {
                                //Make the box taller if it is a long field
                                if (pDc.Length > 125)
                                {
                                    pTxtBox.Multiline = true;
                                    pTxtBox.Height = pTxtBox.Height * 3;

                                }

                            }
                            if (pDc.Length > 0)
                            {
                                pTxtBox.MaxLength = pDc.Length;
                            }


                            //Apply global font
                            pTxtBox.Font = c_Fnt;

                            //Group into panels to assist resizing
                            Panel pPnl = new Panel();
                            pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                            pLbl.Top = 0;
                            pTxtBox.Top = 5 + pLbl.Height;
                            pPnl.Width = c_ControlWidth;
                            pPnl.Margin = new Padding(0);
                            pPnl.Padding = new Padding(0);





                            pPnl.Top = 0;
                            pPnl.Left = 0;
                            pPnl.Height = pTxtBox.Height + pLbl.Height + 10;
                            pPnl.Controls.Add(pLbl);
                            pPnl.Controls.Add(pTxtBox);
                            pTbPg.Controls.Add(pPnl);

                            //Reserved Columns
                        }
                        else if (pSubType.SubtypeFieldName == strfld)
                        {
                            //Create a lable for the field name
                            pLbl = new Label();
                            //Apply the field alias to the field name
                            pLbl.Text = strAli + A4LGSharedFunctions.Localizer.GetString("SetValue");
                            //Link the field to the name of the control
                            pLbl.Name = "lblEdit" + strfld;

                            //Add the control at the determined Location

                            pLbl.Left = 0;

                            pLbl.Top = 0;
                            pLbl.ForeColor = Color.Blue;

                            //Apply global font
                            pLbl.Font = c_FntLbl;
                            //Create a graphics object to messure the text
                            g = pLbl.CreateGraphics();
                            s = g.MeasureString(pLbl.Text, pLbl.Font);

                            pLbl.Height = Convert.ToInt32(s.Height);
                            //If the text is larger then the control, truncate the control
                            if (s.Width >= c_ControlWidth)
                            {
                                pLbl.Width = c_ControlWidth;
                                //Use autosize if it fits
                            }
                            else
                            {
                                pLbl.AutoSize = true;
                            }


                            if (Globals.SubtypeCount((ISubtypes)pSubType.Subtypes) == 2)
                            {
                                CustomPanel pNewGpBox = new CustomPanel();
                                pNewGpBox.Tag = strfld;
                                pNewGpBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                                pNewGpBox.BackColor = Color.White;
                                //  pNewGpBox.BorderColor = Pens.LightGray

                                pNewGpBox.Width = c_ControlWidth;
                                pNewGpBox.Top = 0;
                                pNewGpBox.Left = 0;

                                pRDButton = new RadioButton();
                                pRDButton.Font = c_Fnt;
                                pRDButton.Name = "Rdo1Sub";
                                string codeVal = "";
                                string displayVal = "";
                                Globals.SubtypeValuesAtIndex(0, (ISubtypes)pSubType, ref codeVal, ref displayVal);
                                pRDButton.Tag = codeVal;

                                pRDButton.Text = displayVal;

                                pRDButton.Left = pLeftPadding;


                                pRDButton.AutoSize = true;
                                pNewGpBox.Controls.Add(pRDButton);


                                pNewGpBox.Height = pRDButton.Height + 12;
                                pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;


                                pRDButton = new RadioButton();
                                pRDButton.Font = c_Fnt;
                                pRDButton.Name = "Rdo2Sub";
                                Globals.SubtypeValuesAtIndex(1, pSubType, ref codeVal, ref displayVal);

                                pRDButton.Tag = codeVal;
                                pRDButton.Text = displayVal;
                                pRDButton.Left = pNewGpBox.Width / 2;


                                pRDButton.AutoSize = true;
                                pNewGpBox.Controls.Add(pRDButton);
                                pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;




                                Panel pPnl = new Panel();
                                pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                pLbl.Top = 0;
                                pNewGpBox.Top = pLbl.Height + 5;

                                pPnl.Width = c_ControlWidth;
                                pPnl.Margin = new Padding(0);
                                pPnl.Padding = new Padding(0);





                                pPnl.Top = 0;
                                pPnl.Left = 0;
                                pPnl.Height = pNewGpBox.Height + pLbl.Height + 10;
                                pPnl.Controls.Add(pLbl);
                                pPnl.Controls.Add(pNewGpBox);

                                pTbPg.Controls.Add(pPnl);

                                pNewGpBox = null;
                                //  pPf = Nothing

                            }
                            else
                            {
                                pCBox = new ComboBox();
                                pCBox.Tag = strfld;
                                pCBox.Name = "cboEdt" + strfld;
                                pCBox.Left = 0;
                                pCBox.Top = 0;
                                pCBox.Width = c_ControlWidth;
                                pCBox.Height = pCBox.Height + 5;
                                pCBox.DropDownStyle = ComboBoxStyle.DropDownList;

                                pCBox.Font = c_Fnt;
                                pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never;

                                pCBox.DataSource = Globals.SubtypeToList(pSubType);
                                pCBox.DisplayMember = "getDisplay";
                                pCBox.ValueMember = "getValue";
                                // pCmdBox.MaxLength = pDc.Length





                                pCBox.SelectionChangeCommitted += cmbSubTypChange_Click;




                                Panel pPnl = new Panel();
                                pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                pLbl.Top = 0;
                                pCBox.Top = pLbl.Height + 5;

                                pPnl.Width = c_ControlWidth;
                                pPnl.Margin = new Padding(0);
                                pPnl.Padding = new Padding(0);





                                pPnl.Top = 0;
                                pPnl.Left = 0;
                                pPnl.Height = pCBox.Height + pLbl.Height + 15;
                                pPnl.Controls.Add(pLbl);
                                pPnl.Controls.Add(pCBox);

                                pTbPg.Controls.Add(pPnl);
                                string codeVal = "";
                                string displayVal = "";
                                Globals.SubtypeValuesAtIndex(0, (ISubtypes)pSubType, ref codeVal, ref displayVal);

                                pCBox.Text = displayVal;

                            }


                        }
                        else
                        {

                            if (pSubType.HasSubtype)
                            {
                                pDom = pSubType.get_Domain(pSubTypeDefValue, pDc.Name);


                            }
                            else
                            {
                                pDom = pDc.Domain;

                            }
                            //No Domain Found

                            if (pDom == null)
                            {


                                if (pDc.Type == esriFieldType.esriFieldTypeString || pDc.Type == esriFieldType.esriFieldTypeDouble || pDc.Type == esriFieldType.esriFieldTypeInteger || pDc.Type == esriFieldType.esriFieldTypeSingle || pDc.Type == esriFieldType.esriFieldTypeSmallInteger)
                                {
                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;

                                    pLbl.Top = 0;
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);

                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = c_ControlWidth;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }

                                    //Create a new control to display the attributes                    
                                    pTxtBox = new TextBox();

                                    //Tag the control with the field it represents
                                    pTxtBox.Tag = (strfld).Trim();
                                    //Name the field with the field name
                                    pTxtBox.Name = "txtEdit" + strfld;
                                    //Locate the control on the display
                                    pTxtBox.Left = 0;

                                    pTxtBox.Width = c_ControlWidth;
                                    if (pDc.Type == esriFieldType.esriFieldTypeString)
                                    {
                                        //Make the box taller if it is a long field
                                        if (pDc.Length > 125)
                                        {
                                            pTxtBox.Multiline = true;
                                            pTxtBox.Height = pTxtBox.Height * 3;

                                        }

                                    }
                                    if (pDc.Length > 0)
                                    {
                                        pTxtBox.MaxLength = pDc.Length;
                                    }


                                    //Apply global font
                                    pTxtBox.Font = c_Fnt;

                                    //Group into panels to assist resizing
                                    Panel pPnl = new Panel();
                                    pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                    pLbl.Top = 0;
                                    pTxtBox.Top = 5 + pLbl.Height;
                                    pPnl.Width = c_ControlWidth;
                                    pPnl.Margin = new Padding(0);
                                    pPnl.Padding = new Padding(0);





                                    pPnl.Top = 0;
                                    pPnl.Left = 0;
                                    pPnl.Height = pTxtBox.Height + pLbl.Height + 10;
                                    pPnl.Controls.Add(pLbl);
                                    pPnl.Controls.Add(pTxtBox);
                                    pTbPg.Controls.Add(pPnl);

                                }
                                else if (pDc.Type == esriFieldType.esriFieldTypeDate)
                                {
                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;

                                    //   pLbl.Top = pNextControlTop
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);

                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = c_ControlWidth;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }
                                    //Determine the Location for the next control
                                    //   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                                    pDateTime = new DateTimePicker();
                                    pDateTime.Font = c_Fnt;
                                    //pDateTime.CustomFormat = "m/d/yyyy"
                                    pDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
                                    pDateTime.CustomFormat = "M-d-yy";
                                    // h:mm tt"
                                    pDateTime.ShowCheckBox = true;
                                    pDateTime.Tag = strfld;
                                    pDateTime.Name = "dtEdt" + strfld;
                                    pDateTime.Left = 0;
                                    //   pDateTime.Top = pNextControlTop
                                    pDateTime.Width = c_ControlWidth;



                                    //Determine the Location for the next control
                                    //pNextControlTop = pDateTime.Top + pDateTime.Height + intCtrlSpace
                                    Panel pPnl = new Panel();
                                    pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                    pLbl.Top = 0;
                                    pDateTime.Top = 5 + pLbl.Height;
                                    pPnl.Width = c_ControlWidth;
                                    pPnl.Margin = new Padding(0);
                                    pPnl.Padding = new Padding(0);





                                    pPnl.Top = 0;
                                    pPnl.Left = 0;
                                    pPnl.Height = pDateTime.Height + pLbl.Height + 10;
                                    pPnl.Controls.Add(pLbl);
                                    pPnl.Controls.Add(pDateTime);
                                    pTbPg.Controls.Add(pPnl);


                                }
                                else if (pDc.Type == esriFieldType.esriFieldTypeRaster)
                                {
                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;
                                    pLbl.Top = 0;
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);
                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = 0;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }
                                    //Determine the Location for the next control


                                    //Create a new control to display the attributes                    
                                    pTxtBox = new TextBox();
                                    //Disable the control
                                    //  pPic.ReadOnly = True
                                    //Tag the control with the field it represents
                                    pTxtBox.Tag = (strfld).Trim();
                                    //Name the field with the field name
                                    pTxtBox.Name = "txtEdit" + strfld;
                                    //Locate the control on the display
                                    pTxtBox.Left = 0;
                                    pTxtBox.Top = 0;
                                    pTxtBox.Width = c_ControlWidth - pTxtBox.Height;
                                    if (pDc.Type == esriFieldType.esriFieldTypeString)
                                    {
                                        //Make the box taller if it is a long field
                                        if (pDc.Length > 125)
                                        {
                                            pTxtBox.Multiline = true;
                                            pTxtBox.Height = pTxtBox.Height * 3;

                                        }

                                    }
                                    if (pDc.Length > 0)
                                    {
                                        pTxtBox.MaxLength = pDc.Length;
                                    }

                                    pTxtBox.BackgroundImageLayout = ImageLayout.Stretch;

                                    //Apply global font
                                    pTxtBox.Font = c_Fnt;

                                    pBtn = new System.Windows.Forms.Button();

                                    pBtn.Tag = (strfld).Trim();
                                    //Name the field with the field name
                                    pBtn.Name = "btnEdit" + strfld;
                                    //Locate the control on the display
                                    pBtn.Left = pTxtBox.Left + pTxtBox.Width + 5;
                                    pBtn.Top = 0;
                                    System.Drawing.Bitmap img = null;

                                    img = Properties.Resources.Open2;

                                    img.MakeTransparent(img.GetPixel(img.Width - 1, 1));

                                    pBtn.BackgroundImageLayout = ImageLayout.Center;
                                    pBtn.BackgroundImage = img;
                                    img = null;
                                    pBtn.Width = pTxtBox.Height;
                                    pBtn.Height = pTxtBox.Height;

                                    pBtn.Click += btnLoadImgClick;


                                    Panel pPnl = new Panel();
                                    pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                    pLbl.Top = 0;
                                    pTxtBox.Top = 5 + pLbl.Height;
                                    pBtn.Top = pTxtBox.Top;
                                    pPnl.Width = c_ControlWidth;
                                    pPnl.Margin = new Padding(0);
                                    pPnl.Padding = new Padding(0);





                                    pPnl.Top = 0;
                                    pPnl.Left = 0;
                                    pPnl.Height = pTxtBox.Height + pLbl.Height + 10;
                                    pPnl.Controls.Add(pLbl);
                                    pPnl.Controls.Add(pTxtBox);
                                    pPnl.Controls.Add(pBtn);
                                    pTbPg.Controls.Add(pPnl);

                                }
                                else if (pDc.Type == esriFieldType.esriFieldTypeBlob)
                                {
                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;
                                    pLbl.Top = 0;
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);
                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = 0;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }
                                    //Determine the Location for the next control


                                    //Create a new control to display the attributes                    
                                    pTxtBox = new TextBox();
                                    //Disable the control
                                    //  pPic.ReadOnly = True
                                    //Tag the control with the field it represents
                                    pTxtBox.Tag = (strfld).Trim();
                                    //Name the field with the field name
                                    pTxtBox.Name = "txtEdit" + strfld;
                                    //Locate the control on the display
                                    pTxtBox.Left = 0;
                                    pTxtBox.Top = 0;
                                    pTxtBox.Width = c_ControlWidth - pTxtBox.Height;
                                    if (pDc.Type == esriFieldType.esriFieldTypeString)
                                    {
                                        //Make the box taller if it is a long field
                                        if (pDc.Length > 125)
                                        {
                                            pTxtBox.Multiline = true;
                                            pTxtBox.Height = pTxtBox.Height * 3;

                                        }

                                    }
                                    if (pDc.Length > 0)
                                    {
                                        pTxtBox.MaxLength = pDc.Length;
                                    }

                                    pTxtBox.BackgroundImageLayout = ImageLayout.Stretch;

                                    //Apply global font
                                    pTxtBox.Font = c_Fnt;

                                    pBtn = new System.Windows.Forms.Button();

                                    pBtn.Tag = (strfld).Trim();
                                    //Name the field with the field name
                                    pBtn.Name = "btnEdit" + strfld;
                                    //Locate the control on the display
                                    pBtn.Left = pTxtBox.Left + pTxtBox.Width + 5;
                                    pBtn.Top = 0;
                                    System.Drawing.Bitmap img = null;

                                    img = Properties.Resources.Open2;

                                    img.MakeTransparent(img.GetPixel(img.Width - 1, 1));

                                    pBtn.BackgroundImageLayout = ImageLayout.Center;
                                    pBtn.BackgroundImage = img;
                                    img = null;
                                    pBtn.Width = pTxtBox.Height;
                                    pBtn.Height = pTxtBox.Height;

                                    pBtn.Click += btnLoadImgClick;


                                    Panel pPnl = new Panel();
                                    pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                    pLbl.Top = 0;
                                    pTxtBox.Top = 5 + pLbl.Height;
                                    pBtn.Top = pTxtBox.Top;
                                    pPnl.Width = c_ControlWidth;
                                    pPnl.Margin = new Padding(0);
                                    pPnl.Padding = new Padding(0);





                                    pPnl.Top = 0;
                                    pPnl.Left = 0;
                                    pPnl.Height = pTxtBox.Height + pLbl.Height + 10;
                                    pPnl.Controls.Add(pLbl);
                                    pPnl.Controls.Add(pTxtBox);
                                    pPnl.Controls.Add(pBtn);
                                    pTbPg.Controls.Add(pPnl);

                                }
                            }
                            else
                            {
                                if (pDom is CodedValueDomain)
                                {
                                    ICodedValueDomain pCV = default(ICodedValueDomain);

                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;
                                    pLbl.Top = 0;
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);
                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = c_ControlWidth;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }
                                    //Determine the Location for the next control
                                    //    pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                                    pCV = (ICodedValueDomain)pDom;
                                    // pTbPg.Controls.Add(pLbl)

                                    if (pCV.CodeCount == 2)
                                    {
                                        CustomPanel pNewGpBox = new CustomPanel();
                                        pNewGpBox.Tag = strfld;
                                        pNewGpBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                                        pNewGpBox.BackColor = Color.White;
                                        //  pNewGpBox.BorderColor = Pens.LightGray

                                        pNewGpBox.Width = c_ControlWidth;
                                        pNewGpBox.Top = 0;
                                        pNewGpBox.Left = 0;

                                        pRDButton = new RadioButton();
                                        pRDButton.Name = "Rdo1";
                                        string codeVal = "";
                                        string displayVal = "";
                                        Globals.DomainValuesAtIndex(0, (ICodedValueDomain)pCV, ref codeVal, ref displayVal);

                                        pRDButton.Tag = codeVal;
                                        pRDButton.Text = displayVal;
                                        pRDButton.Font = c_Fnt;
                                        //Dim pPf As SizeF = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)

                                        //'pRDButton.Height = pPf.Height
                                        //pRDButton.Width = pPf.Width + 25

                                        pRDButton.Left = pLeftPadding;

                                        pRDButton.AutoSize = true;
                                        pNewGpBox.Controls.Add(pRDButton);


                                        pNewGpBox.Height = pRDButton.Height + 12;
                                        pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;


                                        pRDButton = new RadioButton();
                                        pRDButton.Font = c_Fnt;
                                        pRDButton.Name = "Rdo2";
                                        Globals.DomainValuesAtIndex(1, (ICodedValueDomain)pCV, ref codeVal, ref displayVal);

                                        pRDButton.Tag = codeVal;
                                        pRDButton.Text = displayVal;
                                        pRDButton.Left = pNewGpBox.Width / 2;
                                        //pPf = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)
                                        //pRDButton.Height = pPf.Height
                                        //pRDButton.Width = pPf.Width + 25


                                        pRDButton.AutoSize = true;
                                        pNewGpBox.Controls.Add(pRDButton);
                                        pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;


                                        // pTbPg.Controls.Add(pNewGpBox)

                                        //  pNextControlTop = pNewGpBox.Top + pNewGpBox.Height + 7 + intLabelCtrlSpace


                                        Panel pPnl = new Panel();
                                        pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                        pLbl.Top = 0;
                                        pNewGpBox.Top = 5 + pLbl.Height;

                                        pPnl.Width = c_ControlWidth;
                                        pPnl.Margin = new Padding(0);
                                        pPnl.Padding = new Padding(0);





                                        pPnl.Top = 0;
                                        pPnl.Left = 0;
                                        pPnl.Height = pNewGpBox.Height + pLbl.Height + 10;
                                        pPnl.Controls.Add(pLbl);
                                        pPnl.Controls.Add(pNewGpBox);

                                        pTbPg.Controls.Add(pPnl);

                                        pNewGpBox = null;
                                        //  pPf = Nothing

                                    }
                                    else
                                    {
                                        pCBox = new ComboBox();
                                        pCBox.Tag = strfld;
                                        pCBox.Name = "cboEdt" + strfld;
                                        pCBox.Left = 0;
                                        pCBox.Top = 0;
                                        pCBox.Width = c_ControlWidth;
                                        pCBox.Height = pCBox.Height + 5;
                                        pCBox.DropDownStyle = ComboBoxStyle.DropDownList;
                                        pCBox.Font = c_Fnt;
                                        pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never;

                                        pCBox.DataSource = Globals.DomainToList((IDomain)pCV);
                                        pCBox.DisplayMember = "getDisplay";
                                        pCBox.ValueMember = "getValue";
                                        // pCmdBox.MaxLength = pDc.Length





                                        Panel pPnl = new Panel();
                                        pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                        pLbl.Top = 0;
                                        pCBox.Top = 5 + pLbl.Height;

                                        pPnl.Width = c_ControlWidth;
                                        pPnl.Margin = new Padding(0);
                                        pPnl.Padding = new Padding(0);





                                        pPnl.Top = 0;
                                        pPnl.Left = 0;
                                        pPnl.Height = pCBox.Height + pLbl.Height + 15;
                                        pPnl.Controls.Add(pLbl);
                                        pPnl.Controls.Add(pCBox);

                                        pTbPg.Controls.Add(pPnl);


                                        //   pTbPg.Controls.Add(pCBox)
                                        // MsgBox(pCBox.Items.Count)
                                        pCBox.Visible = true;

                                        string codeVal = "";
                                        string displayVal = "";
                                        Globals.DomainValuesAtIndex(0, (ICodedValueDomain)pCV, ref codeVal, ref displayVal);

                                        pCBox.Text = displayVal;

                                        //Try

                                        //pCBox.SelectedIndex = 0
                                        //Catch ex As Exception

                                        //End Try
                                        pCBox.Visible = true;
                                        pCBox.Refresh();

                                        //  pNextControlTop = pCBox.Top + pCBox.Height + 7 + intLabelCtrlSpace
                                    }


                                }
                                else if (pDom is RangeDomain)
                                {
                                    IRangeDomain pRV = default(IRangeDomain);
                                    //Create a lable for the field name
                                    pLbl = new Label();
                                    //Apply the field alias to the field name
                                    pLbl.Text = strAli;
                                    //Link the field to the name of the control
                                    pLbl.Name = "lblEdit" + strfld;
                                    //Add the control at the determined Location
                                    pLbl.Left = 0;
                                    pLbl.Top = 0;
                                    //Apply global font
                                    pLbl.Font = c_FntLbl;
                                    //Create a graphics object to messure the text
                                    g = pLbl.CreateGraphics();
                                    s = g.MeasureString(pLbl.Text, pLbl.Font);
                                    pLbl.Height = Convert.ToInt32(s.Height);
                                    //If the text is larger then the control, truncate the control
                                    if (s.Width >= c_ControlWidth)
                                    {
                                        pLbl.Width = c_ControlWidth;
                                        //Use autosize if it fits
                                    }
                                    else
                                    {
                                        pLbl.AutoSize = true;
                                    }
                                    //Determine the Location for the next control

                                    pRV = (IRangeDomain)pDom;
                                    pNumBox = new NumericUpDown();
                                    //    AddHandler pNumBox.MouseDown, AddressOf numericClickEvt_MouseDown




                                    if (pDc.Type == esriFieldType.esriFieldTypeInteger)
                                    {
                                        pNumBox.DecimalPlaces = 0;


                                    }
                                    else if (pDc.Type == esriFieldType.esriFieldTypeDouble)
                                    {
                                        pNumBox.DecimalPlaces = 2;
                                        //pDc.DataType.

                                    }
                                    else if (pDc.Type == esriFieldType.esriFieldTypeSingle)
                                    {
                                        pNumBox.DecimalPlaces = 1;
                                        //pDc.DataType.
                                    }
                                    else
                                    {
                                        pNumBox.DecimalPlaces = 2;
                                        //pDc.DataType.
                                    }

                                    pNumBox.Minimum = Convert.ToDecimal(pRV.MinValue.ToString());
                                    pNumBox.Maximum = Convert.ToDecimal(pRV.MaxValue.ToString());
                                    NumericUpDownAcceleration pf = new NumericUpDownAcceleration(3, Convert.ToInt32(Convert.ToDouble(pNumBox.Maximum - pNumBox.Minimum) * 0.02));


                                    pNumBox.Accelerations.Add(pf);

                                    pNumBox.Tag = strfld;
                                    pNumBox.Name = "numEdt" + strfld;
                                    pNumBox.Left = 0;
                                    pNumBox.BackColor = Color.White;
                                    pNumBox.Top = 0;
                                    pNumBox.Width = c_ControlWidth;
                                    pNumBox.Font = c_Fnt;

                                    Panel pPnl = new Panel();
                                    pPnl.BorderStyle = System.Windows.Forms.BorderStyle.None;

                                    pLbl.Top = 0;
                                    pNumBox.Top = 5 + pLbl.Height;

                                    pPnl.Width = c_ControlWidth;
                                    pPnl.Margin = new Padding(0);
                                    pPnl.Padding = new Padding(0);





                                    pPnl.Top = 0;
                                    pPnl.Left = 0;
                                    pPnl.Height = pNumBox.Height + pLbl.Height + 15;
                                    pPnl.Controls.Add(pLbl);
                                    pPnl.Controls.Add(pNumBox);

                                    pTbPg.Controls.Add(pPnl);

                                }

                            }

                        }
                        pLayerFields = null;
                        pFieldInfo = null;


                    }
                }
                //pDC



                if (pSubType.HasSubtype)
                {
                    SubtypeChange(pSubTypeDefValue, pSubType.SubtypeFieldName);

                }
                //cleanup
                pBtn = null;
                pDCs = null;
                pDc = null;

                pTbPg = null;

                pTxtBox = null;
                pLbl = null;
                pNumBox = null;

                pRDButton = null;

                pCBox = null;
                pDateTime = null;
                g = null;
                //s = null;
                s_tbCntlDisplay.ResumeLayout();
                s_tbCntlDisplay.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("CT_2") + Environment.NewLine + ex.Message);

            }

        }
        private static void btnLoadImgClick(object sender, System.EventArgs e)
        {
            try
            {
                //Opens a dialog to browse out for an image
                System.Windows.Forms.OpenFileDialog openFileDialog1 = null;

                openFileDialog1 = new System.Windows.Forms.OpenFileDialog();



                //Filter the image types
                openFileDialog1.Filter = "Jpg (*.jpg) |*.jpg|Bitmap (*.bmp) |*.bmp|Gif (*.gif)| *.gif";
                //If the user selects an image
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Set the path of the image to the text box
                    Control[] controls = ((Panel)((System.Windows.Forms.Button)sender).Parent).Controls.Find("txtEdit" + ((System.Windows.Forms.Button)sender).Tag.ToString(), false);
                    //If the control was found
                    if (controls.Length > 0)
                    {
                        controls[0].Text = openFileDialog1.FileName;
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LoadImg") + Environment.NewLine + ex.Message);

            }
        }
        private static void cmbSubTypChange_Click(object sender, System.EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
                return;

            SubtypeChange(Convert.ToInt32(((ComboBox)sender).SelectedValue.ToString()), ((ComboBox)sender).Tag.ToString());

        }
        private static void SubtypeChange(int value, string SubtypeField)
        {
            try
            {
                int intSubVal = value;

                //Feature layer being Identified

                //Exit if the layer is not found
                if (v_ViewerLayer == null)
                    return;

                string strFld = null;
                ComboBox pCmbBox = default(ComboBox);
                NumericUpDown pNUP = default(NumericUpDown);
                ICodedValueDomain pCV = default(ICodedValueDomain);
                IRangeDomain pRg = default(IRangeDomain);

                ISubtypes pSubTypes = (ISubtypes)v_ViewerLayer.FeatureClass;
                int pLeftPadding = 10;



                //Loop through all controls 
                foreach (TabPage tbPg in s_tbCntlDisplay.TabPages)
                {
                    foreach (Control cntrl in tbPg.Controls)
                    {
                        //If the control is a combobox, then reapply the domain

                        if (cntrl is Panel)
                        {
                            foreach (Control cntrlPnl in cntrl.Controls)
                            {

                                if (cntrlPnl is ComboBox)
                                {
                                    pCmbBox = (ComboBox)cntrlPnl;

                                    if (SubtypeField != pCmbBox.Tag.ToString())
                                    {
                                        //Get the Field
                                        strFld = pCmbBox.Tag.ToString();
                                        if (strFld.IndexOf("|") > 0)
                                        {
                                            strFld = (strFld.Substring(0, strFld.IndexOf("|"))).Trim();
                                        }
                                        //Get the domain

                                        pCV = (ICodedValueDomain)pSubTypes.get_Domain(intSubVal, strFld);
                                        if (pCV == null)
                                        {
                                            pCmbBox.DataSource = null;

                                        }
                                        else
                                        {
                                            //If the domain has two values, remove the combo box and add a custompanel
                                            if (pCV.CodeCount == 2)
                                            {
                                                CustomPanel pNewGpBox = new CustomPanel();
                                                RadioButton pRDButton = default(RadioButton);
                                                pNewGpBox.Tag = pCmbBox.Tag;
                                                pNewGpBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                                                pNewGpBox.BackColor = Color.White;
                                                //  pNewGpBox.BorderColor = Pens.LightGray

                                                pNewGpBox.Width = pCmbBox.Width;
                                                pNewGpBox.Top = pCmbBox.Top;
                                                pNewGpBox.Left = pCmbBox.Left;

                                                pRDButton = new RadioButton();
                                                pRDButton.Name = "Rdo1";
                                                string codeVal = "";
                                                string displayVal = "";

                                                Globals.SubtypeValuesAtIndex(0, (ISubtypes)pCV, ref codeVal, ref  displayVal);

                                                pRDButton.Tag = codeVal;
                                                pRDButton.Text = displayVal;

                                                pRDButton.Left = pLeftPadding;

                                                pRDButton.AutoSize = true;
                                                pNewGpBox.Controls.Add(pRDButton);


                                                pNewGpBox.Height = pRDButton.Height + 12;
                                                pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;


                                                pRDButton = new RadioButton();
                                                pRDButton.Name = "Rdo2";
                                                Globals.SubtypeValuesAtIndex(1, (ISubtypes)pCV, ref codeVal, ref displayVal);

                                                pRDButton.Tag = codeVal;
                                                pRDButton.Text = displayVal;

                                                pRDButton.Left = pNewGpBox.Width / 2;

                                                pRDButton.AutoSize = true;
                                                pNewGpBox.Controls.Add(pRDButton);
                                                pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2;


                                                tbPg.Controls.Add(pNewGpBox);

                                                try
                                                {
                                                    tbPg.Controls.Remove(pCmbBox);
                                                    //Dim cnts() As Control = tbPg.Controls.Find("lblEdit" & strFld, False)
                                                    //If cnts.Length > 0 Then
                                                    //    tbPg.Controls.Remove(cnts(0))
                                                    //End If



                                                }
                                                catch// (Exception ex)
                                                {
                                                }

                                                pNewGpBox = null;
                                                pRDButton = null;

                                            }
                                            else
                                            {
                                                //Set the domain value

                                                pCmbBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never;

                                                pCmbBox.DataSource = Globals.DomainToList((IDomain)pCV);
                                                pCmbBox.DisplayMember = "getDisplay";
                                                pCmbBox.ValueMember = "getValue";
                                                pCmbBox.Visible = true;
                                                pCmbBox.Refresh();

                                                string codeVal = "";
                                                string displayVal = "";
                                                Globals.SubtypeValuesAtIndex(0, (ISubtypes)pCV, ref codeVal, ref displayVal);
                                                pCmbBox.Text = displayVal;
                                            }

                                        }



                                    }
                                    //If the contorl is a coded value domain with two values

                                }
                                else if (cntrlPnl is CustomPanel)
                                {

                                    //Get the Field
                                    strFld = cntrlPnl.Tag.ToString();
                                    if (strFld.IndexOf("|") > 0)
                                    {
                                        strFld = (strFld.Substring(0, strFld.IndexOf("|"))).Trim();
                                    }
                                    //Get the fomain
                                    pCV = (ICodedValueDomain)pSubTypes.get_Domain(intSubVal, strFld);

                                    if (pCV == null)
                                    {
                                        cntrlPnl.Controls.Clear();


                                    }
                                    else
                                    {
                                        //If the domain has more than two values, remove the custompanel and add a combo box 
                                        if (pCV.CodeCount == 2)
                                        {
                                            try
                                            {
                                                //Set up the proper domain values
                                                RadioButton pRdoBut = default(RadioButton);
                                                pRdoBut = (RadioButton)cntrlPnl.Controls["Rdo1"];
                                                string codeVal = "";
                                                string displayVal = "";
                                                Globals.SubtypeValuesAtIndex(0, (ISubtypes)pCV, ref  codeVal, ref  displayVal);

                                                pRdoBut.Tag = codeVal;
                                                pRdoBut.Text = displayVal;

                                                pRdoBut = (RadioButton)cntrlPnl.Controls["Rdo2"];
                                                Globals.SubtypeValuesAtIndex(1, (ISubtypes)pCV, ref codeVal, ref displayVal);

                                                pRdoBut.Tag = codeVal;
                                                pRdoBut.Text = displayVal;

                                            }
                                            catch //(Exception ex)
                                            {
                                            }

                                        }
                                        else
                                        {
                                            ComboBox pCBox = default(ComboBox);
                                            pCBox = new ComboBox();
                                            pCBox.Tag = strFld;
                                            pCBox.Name = "cboEdt" + strFld;
                                            pCBox.Left = cntrlPnl.Left;
                                            pCBox.Top = cntrlPnl.Top;
                                            pCBox.Width = cntrlPnl.Width;
                                            pCBox.Height = pCBox.Height + 5;
                                            pCBox.DropDownStyle = ComboBoxStyle.DropDownList;

                                            pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never;
                                            pCBox.DataSource = Globals.DomainToList((IDomain)pCV);
                                            pCBox.DisplayMember = "getDisplay";
                                            pCBox.ValueMember = "getValue";
                                            pCBox.Visible = true;
                                            pCBox.Refresh();

                                            string codeVal = "";
                                            string displayVal = "";
                                            Globals.SubtypeValuesAtIndex(0, (ISubtypes)pCV, ref codeVal, ref displayVal);
                                            pCBox.Text = displayVal;
                                            // pCmdBox.MaxLength = pDc.Length


                                            tbPg.Controls.Add(pCBox);
                                            // MsgBox(pCBox.Items.Count)

                                            pCBox.Visible = true;
                                            pCBox.Refresh();

                                            tbPg.Controls.Remove(cntrlPnl);

                                            pCBox = null;

                                        }

                                    }
                                    //If the contorl is a range domain
                                }
                                else if (cntrlPnl is NumericUpDown)
                                {
                                    //get the control
                                    pNUP = (NumericUpDown)cntrlPnl;
                                    //Get the field
                                    strFld = pNUP.Tag.ToString();
                                    if (strFld.IndexOf("|") > 0)
                                    {
                                        strFld = (strFld.Substring(0, strFld.IndexOf("|"))).Trim();
                                    }
                                    //Get the domain
                                    pRg = (IRangeDomain)pSubTypes.get_Domain(intSubVal, strFld);
                                    if (pRg == null)
                                    {
                                        pNUP.Enabled = false;

                                    }
                                    else
                                    {
                                        pNUP.Enabled = true;
                                        pNUP.Minimum = Convert.ToDecimal(pRg.MinValue.ToString());
                                        pNUP.Maximum = Convert.ToDecimal(pRg.MaxValue.ToString());
                                    }


                                    pNUP.Refresh();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("SubTpChange") + Environment.NewLine + ex.Message);

            }
        }
        #endregion
        #region "Freind Functions"


        static internal void s_gpBoxOptions_Click(object sender, System.EventArgs e)
        {

            try
            {
                if (s_gpBoxOptions.Height < 20)
                {
                    s_gpBoxOptions.Height = 85;
                }
                else
                {
                    s_gpBoxOptions.Height = 15;
                }
                s_splContMain_Resize(null, null);
                ShuffleControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_8") + Environment.NewLine + ex.Message);

            }

        }
        static internal void s_cboLayers_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {

            try
            {
                if (v_CurrentLayerText == s_cboLayers.Text)
                    return;
                v_CurrentLayerText = s_cboLayers.Text;
                if (v_ViewerLayerCursorArray != null)
                {
                   // Marshal.ReleaseComObject(v_ViewerLayerCursorArray);

                    v_ViewerLayerCursorArray = null;
                }
                bool FCorLayerView = true;
                v_ViewerLayer = (IFeatureLayer)Globals.FindLayer(ArcMap.Application, s_cboLayers.Text, ref FCorLayerView);
                if (v_ViewerLayer == null)
                {
                    AddControls(true);
           
           //         LoadFeatureToViewer(null);
                    setButtonState();

                    return;
                }
                v_CurrentLayerText = s_cboLayers.Text;

                AddControls(true);
                s_tbCntlDisplay.Parent.Parent.Update();
                s_tbCntlDisplay.Parent.Parent.Refresh();
                tbCntlDisplay_Resize(null, null);


                LoadCursor();
                if (v_ViewerLayerCursorArray.Count == 0)
                    return;
                v_ViewerRecordIndex = 0;
                s_lblCount.Text = v_ViewerRecordIndex + 1 + A4LGSharedFunctions.Localizer.GetString("OutOf") + v_ViewerLayerCursorArray.Count;

                IFeature pFeat = v_ViewerLayerCursorArray[v_ViewerRecordIndex] as IFeature;


                LoadFeatureToViewer(pFeat);
                if (s_chkZoomToOnAdvance.Checked)
                {
                    Globals.CenterMapOnFeatureWithScale(pFeat, ArcMap.Application, s_txtScale.Text);
                }

                setButtonState();
                //'LoadFeatureToViewer()
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_9") + Environment.NewLine + ex.Message);

            }
        }

        static internal void tbCntlDisplay_Resize(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (s_tbCntlDisplay == null)
                    return;

                if (s_tbCntlDisplay.Width > s_tbCntlDisplay.Height)
                {
                    ShuffleControls(false);
                }
                else
                {
                    ShuffleControls(false);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_10") + Environment.NewLine + ex.Message);


            }
        }
        static internal void CenterButton()
        {
            try
            {
                s_BtnZoomTo.Left = (s_BtnRefresh.Parent.Width / 2) - 5 - s_BtnZoomTo.Width;

                s_BtnRefresh.Left = s_BtnRefresh.Parent.Width / 2 + 5;
                s_BtnPrevious.Left = s_BtnZoomTo.Left - 10 - s_BtnPrevious.Width;
                s_BtnNext.Left = s_BtnRefresh.Left + s_BtnRefresh.Width + 10;

                s_lblCount.Left = (s_BtnRefresh.Parent.Width / 2) - (s_lblCount.Width / 2);
                s_lblCount.Top = 27;
                s_BtnRefresh.Parent.Update();
                s_BtnRefresh.Parent.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_11") + Environment.NewLine + ex.Message);


            }
        }
        static internal void setButtonState()
        {
            try
            {
                if (v_ViewerLayer == null)
                {
                    s_BtnPrevious.Enabled = false;
                    s_BtnRefresh.Enabled = false;
                    s_BtnNext.Enabled = false;
                    s_BtnZoomTo.Enabled = false;
                }
                else if (v_ViewerLayerCursorArray == null)
                {
                    s_BtnPrevious.Enabled = false;
                    s_BtnRefresh.Enabled = true;
                    s_BtnNext.Enabled = false;
                    s_BtnZoomTo.Enabled = false;
                }
                else if (v_ViewerLayerCursorArray.Count == 0)
                {
                    s_BtnPrevious.Enabled = false;
                    s_BtnRefresh.Enabled = true;

                    s_BtnNext.Enabled = false;
                    s_BtnZoomTo.Enabled = false;
                }
                else if (v_ViewerLayerCursorArray.Count == 1)
                {
                    s_BtnPrevious.Enabled = false;
                    s_BtnRefresh.Enabled = true;

                    s_BtnNext.Enabled = false;
                    s_BtnZoomTo.Enabled = true;
                }
                else if (v_ViewerRecordIndex >= v_ViewerLayerCursorArray.Count - 1)
                {
                    s_BtnPrevious.Enabled = true;
                    s_BtnRefresh.Enabled = true;

                    s_BtnNext.Enabled = false;
                    s_BtnZoomTo.Enabled = true;
                }
                else if (v_ViewerRecordIndex == 0)
                {
                    s_BtnPrevious.Enabled = false;
                    s_BtnNext.Enabled = true;
                    s_BtnRefresh.Enabled = true;
                    s_BtnZoomTo.Enabled = true;

                }
                else
                {
                    s_BtnPrevious.Enabled = true;
                    s_BtnNext.Enabled = true;
                    s_BtnRefresh.Enabled = true;
                    s_BtnZoomTo.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + A4LGSharedFunctions.Localizer.GetString("LW_12") + Environment.NewLine + ex.Message);


            }
        }


        #endregion



        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private LayerWindow m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new LayerWindow(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

      
    }
}
