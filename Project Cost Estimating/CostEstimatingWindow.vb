
' | Version 10.1
' | Copyright 2014 Esri
' |
' | Licensed under the Apache License, Version 2.0 (the "License");
' | you may not use this file except in compliance with the License.
' | You may obtain a copy of the License at
' |
' |    http://www.apache.org/licenses/LICENSE-2.0
' |
' | Unless required by applicable law or agreed to in writing, software
' | distributed under the License is distributed on an "AS IS" BASIS,
' | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' | See the License for the specific language governing permissions and
' | limitations under the License.


Option Strict Off
Option Explicit On
Imports System.Runtime.InteropServices

Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.SystemUI

Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ArcMapUI

'Imports ESRI.ArcGIS.ArcMap
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.DataSourcesGDB



''' <summary>
''' Designer class of the dockable window add-in. It contains user interfaces that
''' make up the dockable window.
''' </summary>
''' 
Partial Public Class CostEstimatingWindow
    Inherits UserControl
    Private Shared m_BufferAmountConvext As Double = 60
    Private Shared m_BufferAmount As Double = 15


    Private Shared s_enabled As Boolean


    'Shared References
    Private Shared s_dgCIP As myDG
    Private Shared s_tbCntCIPDetails As TabControl

    Private Shared s_lstInventory As ListBox

    Private Shared s_btnSavePrj As System.Windows.Forms.Button
    Private Shared s_btnClear As System.Windows.Forms.Button
    Private Shared s_btnSave As System.Windows.Forms.Button
    Private Shared s_btnStartEditing As System.Windows.Forms.Button
    Private Shared s_btnStopEditing As System.Windows.Forms.Button


    Private Shared s_btnSelect As System.Windows.Forms.RadioButton
    Private Shared s_btnSelectAssets As System.Windows.Forms.RadioButton
    Private Shared s_btnSketch As System.Windows.Forms.RadioButton
    Private Shared s_btnSelectPrj As System.Windows.Forms.RadioButton

    Private Shared s_lblTotalCost As System.Windows.Forms.Label
    Private Shared s_lblTotLength As System.Windows.Forms.Label
    Private Shared s_lblLength As System.Windows.Forms.Label
    Private Shared s_lblTotArea As System.Windows.Forms.Label
    Private Shared s_lblArea As System.Windows.Forms.Label
    Private Shared s_lblTotPnt As System.Windows.Forms.Label
    Private Shared s_lblPoint As System.Windows.Forms.Label

    Private Shared s_gpBxCIPCostingLayers As System.Windows.Forms.GroupBox
    Private Shared s_gpBxControls As System.Windows.Forms.GroupBox
    Private Shared s_gpBxCIPCan As System.Windows.Forms.GroupBox
    Private Shared s_gpBxSwitch As System.Windows.Forms.GroupBox
    Private Shared s_gpBxCIPInven As System.Windows.Forms.GroupBox
    Private Shared s_gpBxCIPPrj As System.Windows.Forms.GroupBox

    Private Shared s_cboAction As System.Windows.Forms.ComboBox
    Private Shared s_cboDefLayers As System.Windows.Forms.ComboBox
    Private Shared s_cboStrategy As System.Windows.Forms.ComboBox
    Private Shared s_cboCIPInvTypes As System.Windows.Forms.ComboBox

    Private Shared s_TotalDisplay As System.Windows.Forms.FlowLayoutPanel

    Private Shared s_tblDisabled As System.Windows.Forms.TableLayoutPanel

    Private Shared s_ctxMenu As System.Windows.Forms.ContextMenuStrip

    Private Shared s_ShowLength As ToolStripItem
    Private Shared s_ShowPoint As ToolStripItem
    Private Shared s_ShowArea As ToolStripItem

    Private Shared s_numCIPInvCount As System.Windows.Forms.NumericUpDown
    Public Sub init()

    End Sub
    Private Sub New(ByVal hook As Object)
        Try


            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Me.Hook = hook

            Dim txtTmp As String = My.Globals.Functions.GetConfigValue("BufferAmount")

            If (txtTmp <> "") Then

                Double.TryParse(txtTmp, m_BufferAmount)

            End If

            txtTmp = My.Globals.Functions.GetConfigValue("BufferAmountConvex")
            If (txtTmp <> "") Then

                Double.TryParse(txtTmp, m_BufferAmountConvext)

            End If



            'Init Shared Controls
            s_tblDisabled = tblDisabled

            s_tbCntCIPDetails = tbCntCIPDetails
            ' s_tbCntCIPDetails.Dock = DockStyle.Fill
            s_lstInventory = lstInventory
            s_btnSavePrj = btnSavePrj
            s_btnSave = btnSave
            s_btnClear = btnClear
            s_btnStartEditing = btnStartEditing
            s_btnStopEditing = btnStopEditing

            s_btnSelect = btnSelect
            s_btnSelectAssets = btnSelectAssets
            s_btnSketch = btnSketch

            AddHandler s_btnSketch.Click, AddressOf btnSketch_Click


            s_lblTotalCost = lblTotalCost
            s_lblTotLength = lblTotLength

            s_lblLength = lblLength
            s_lblTotArea = lblTotArea
            s_lblArea = lblArea
            s_lblTotPnt = lblTotPnt
            s_lblPoint = lblPoint
            s_gpBxCIPCostingLayers = gpBxCIPCostingLayers
            s_gpBxControls = gpBxControls
            s_gpBxCIPCan = gpBxCIPCan
            s_cboDefLayers = cboDefLayers
            s_cboStrategy = cboStrategy
            s_cboAction = cboAction
            s_cboCIPInvTypes = cboCIPInvTypes
            s_gpBxSwitch = gpBxSwitch
            s_gpBxCIPInven = gpBxCIPInven
            s_gpBxCIPPrj = gpBxCIPPrj
            s_TotalDisplay = TotalDisplay
            s_btnSelectPrj = btnSelectPrj
            s_ctxMenu = ctxMenu

            s_ShowArea = ShowArea
            s_ShowLength = ShowLength
            s_ShowPoint = ShowPoint

            s_numCIPInvCount = numCIPInvCount



            ' Add any initialization after the InitializeComponent() call.
            makeImagesTrans()
            InitGrid()
            createGraphicSymbols()

            'gpBxSwitch.Dock = DockStyle.Right
            's_gpBxSwitch.Width = 85
            's_gpBxCIPInven.Visible = False
            's_gpBxCIPCostingLayers.Visible = False
            's_gpBxCIPPrj.Visible = False
            's_gpBxCIPCan.Visible = True


            s_gpBxCIPCan.Dock = DockStyle.Fill

            s_gpBxCIPInven.Dock = DockStyle.Fill
            s_gpBxCIPPrj.Dock = DockStyle.Fill
            s_gpBxCIPCostingLayers.Dock = DockStyle.Fill
            '  btnToggle.BackgroundImage = My.Resources.Leftdark
            '   btnToggle.BackgroundImageLayout = ImageLayout.Zoom

            's_gpBxCIPCostingLayers.Visible = False
            's_gpBxCIPInven.Visible = False
            's_gpBxCIPPrj.Visible = False
            's_gpBxControls.Visible = False
            's_gpBxCIPCan.Visible = False
            's_gpBxSwitch.Visible = False
            SetEnabled(CostEstimatingExtension.IsExtensionEnabled)

            s_lblTotLength.Text = ".00"
            s_lblTotArea.Text = ".00"
            s_lblTotPnt.Text = "0"
            s_lblTotalCost.Text = FormatCurrency("0.00", 2, TriState.True, TriState.True) 'Format(Total, "#,###.00")

            s_lblTotalCost.Parent.Refresh()
            If (Not CostEstimatingWindow.Exists) Then
                Return
            End If


            CostEstimatingWindow.ResetControls(True)

            If CostEstimatingExtension.CheckForCIPLayers() Then
                CostEstimatingExtension.initCIPLayers()
                CostEstimatingWindow.EnableWindowControls(True)
                CostEstimatingExtension.raiseEventInExt(True)

                CostEstimatingWindow.LoadControlsToDetailForm()

            Else
                CostEstimatingWindow.EnableWindowControls(False)
                CostEstimatingExtension.raiseEventInExt(False)

            End If
            CostEstimatingWindow.EnableSavePrj()
            CostEstimatingWindow.SetEnabled(True)
            loadDefLayersCboBox()


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: New" & vbCrLf & ex.Message)
        End Try
    End Sub

    Friend Shared Sub SetEnabled(ByVal enabled As Boolean)
        s_enabled = enabled
        If s_gpBxCIPCostingLayers Is Nothing Then Return


        If enabled Then
            s_gpBxSwitch.Width = 85
            s_gpBxCIPInven.Visible = False
            s_gpBxCIPCostingLayers.Visible = False
            s_gpBxCIPPrj.Visible = False
            s_gpBxCIPCan.Visible = True
            s_gpBxControls.Visible = True

            s_gpBxSwitch.Visible = True
            s_tblDisabled.Visible = False
        Else
            s_gpBxCIPCan.Visible = False
            s_gpBxCIPCostingLayers.Visible = False
            s_gpBxCIPInven.Visible = False
            s_gpBxCIPPrj.Visible = False
            s_gpBxControls.Visible = False

            s_gpBxSwitch.Visible = False
            s_tblDisabled.Visible = True

        End If
        ' if the dockable window was never displayed, listview could be null

        'hide controls
        'If s_listView Is Nothing Then
        '    Return
        'End If

        'If enabled Then
        '    s_label.Visible = False
        '    s_listView.Visible = True
        'Else
        '    Clear()
        '    s_label.Visible = True
        '    s_listView.Visible = False
        'End If
    End Sub
#Region "Overrides"
    Protected Overrides Sub Finalize()
        Try


            If s_dgCIP IsNot Nothing Then
                s_dgCIP.Dispose()

            End If
            s_dgCIP = Nothing
            MyBase.Finalize()
        Catch ex As Exception

        End Try

    End Sub
#End Region
#Region "Private Shared functions"

    Private Shared Sub ShuffleControls(ByVal Vertical As Boolean)
        If s_tbCntCIPDetails Is Nothing Then Return
        If s_tbCntCIPDetails.SelectedIndex = -1 Then Return
        If s_tbCntCIPDetails.TabPages.Count = 0 Then Return

        If Vertical Then
            Try
                'Spacing between last control and the bottom of the page
                Dim pBottomPadding As Integer = 120
                'Padding for the left of each control
                Dim pLeftPadding As Integer = 10
                'Spacing between firstcontrol and the top
                Dim pTopPadding As Integer = 3
                'Padding for the right of each control
                Dim pRightPadding As Integer = 15

                Dim pCurTabIdx As Integer = s_tbCntCIPDetails.SelectedIndex

                Dim pTbPageCo() As TabPage = Nothing

                Dim pCurTabPage As TabPage = New TabPage
                'pCurTabPage.Name = strName
                'pCurTabPage.Text = strName
                Dim pCntlNextTop As Integer = pTopPadding
                For Each tb As TabPage In s_tbCntCIPDetails.TabPages


                    Dim bLoop As Boolean = True
                    While bLoop = True

                        If tb.Controls.Count = 0 Then
                            Exit While

                        End If

                        Dim cnt As Control = tb.Controls(0)

                        If TypeOf cnt Is System.Windows.Forms.Button Then
                            tb.Controls.Remove(cnt)

                        Else


                            cnt.Top = pCntlNextTop
                            cnt.Width = s_tbCntCIPDetails.Width
                            If TypeOf cnt Is Panel Then
                                For Each pnlCnt As Control In cnt.Controls

                                    If TypeOf pnlCnt Is System.Windows.Forms.Button Then
                                        Dim controls() As Control = CType(CType(pnlCnt, System.Windows.Forms.Button).Parent, Panel).Controls.Find("txtEdit" & pnlCnt.Tag, False)
                                        If controls.Length = 1 Then
                                            controls(0).Width = controls(0).Width - pnlCnt.Width - 5
                                            pnlCnt.Left = controls(0).Width + controls(0).Left + 5

                                        End If
                                    ElseIf TypeOf pnlCnt Is CustomPanel Then
                                        pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding
                                        If pnlCnt.Controls.Count = 2 Then
                                            pnlCnt.Controls(0).Left = pLeftPadding
                                            pnlCnt.Controls(1).Left = (pnlCnt.Width / 2)
                                        End If

                                    Else
                                        pnlCnt.Width = s_tbCntCIPDetails.Width - pLeftPadding - pRightPadding
                                    End If

                                    'End If


                                Next

                            End If

                            pCurTabPage.Controls.Add(cnt)
                            pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                            If pCntlNextTop >= s_tbCntCIPDetails.Height - pBottomPadding Then
                                'Dim pBtn As System.Windows.Forms.Button
                                'pBtn = New System.Windows.Forms.Button
                                'pBtn.Name = "btnSaveEdit"
                                'pBtn.Text = "Save"
                                'pBtn.Font = My.Globals.Constants.c_Fnt
                                'pBtn.Top = pCntlNextTop
                                'pBtn.AutoSize = True

                                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                'pCurTabPage.Controls.Add(pBtn)
                                'pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                                'pBtn = New System.Windows.Forms.Button
                                'pBtn.Name = "btnClearEdit"
                                'pBtn.Text = "Clear"
                                'pBtn.Font = My.Globals.Constants.c_Fnt
                                'pBtn.Top = pCntlNextTop
                                'pBtn.AutoSize = True

                                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                'pCurTabPage.Controls.Add(pBtn)
                                'pBtn.Left = (tbCntCIPDetails.Width / 2) + 10


                                If pTbPageCo Is Nothing Then
                                    ReDim Preserve pTbPageCo(0)
                                Else
                                    ReDim Preserve pTbPageCo(pTbPageCo.Length)
                                End If
                                pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                                pCurTabPage = New TabPage


                                'pCurTabPage.Name = strName
                                'pCurTabPage.Text = strName

                                pCntlNextTop = pTopPadding
                                'pBtn = Nothing

                            End If
                        End If
                    End While
                Next



                If pCurTabPage.Controls.Count > 0 Then
                    'Dim pBtn As System.Windows.Forms.Button
                    'pBtn = New System.Windows.Forms.Button
                    'pBtn.Name = "btnSaveEdit"
                    'pBtn.Text = "Save"
                    'pBtn.Font = My.Globals.Constants.c_Fnt
                    'pBtn.Top = pCntlNextTop
                    'pBtn.AutoSize = True

                    'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                    'pCurTabPage.Controls.Add(pBtn)
                    'pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                    'pBtn = New System.Windows.Forms.Button
                    'pBtn.Name = "btnClearEdit"
                    'pBtn.Text = "Clear"
                    'pBtn.Font = My.Globals.Constants.c_Fnt
                    'pBtn.Top = pCntlNextTop
                    'pBtn.AutoSize = True

                    'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                    'pCurTabPage.Controls.Add(pBtn)
                    'pBtn.Left = (tbCntCIPDetails.Width / 2) + 10
                    If pTbPageCo Is Nothing Then
                        ReDim Preserve pTbPageCo(0)
                    Else
                        ReDim Preserve pTbPageCo(pTbPageCo.Length)
                    End If

                    pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                Else

                End If

                s_tbCntCIPDetails.TabPages.Clear()
                For Each tbp As TabPage In pTbPageCo

                    s_tbCntCIPDetails.TabPages.Add(tbp)

                    tbp.Visible = True

                    tbp.Update()
                Next
                If s_tbCntCIPDetails.TabPages.Count >= pCurTabIdx Then
                    s_tbCntCIPDetails.SelectedIndex = pCurTabIdx
                Else
                    s_tbCntCIPDetails.SelectedIndex = s_tbCntCIPDetails.TabPages.Count - 1
                End If

                pTbPageCo = Nothing
                pCurTabPage = Nothing
            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: ShuffleControls" & vbCrLf & ex.Message)

            End Try


        Else 'horizontel
            Try
                'Spacing between last control and the bottom of the page
                Dim pBottomPadding As Integer = 25
                'Padding for the left of each control
                Dim pLeftPadding As Integer = 10
                'Spacing between firstcontrol and the top
                Dim pTopPadding As Integer = 3
                'Padding for the right of each control
                Dim pRightPadding As Integer = 15
                Dim pCntSpacing As Integer = 5
                Dim pCurTabIdx As Integer = s_tbCntCIPDetails.SelectedIndex

                Dim pTbPageCo() As TabPage = Nothing
                Dim pCurTabPage As TabPage = New TabPage
                pCurTabPage.Name = "Page 1"
                pCurTabPage.Text = "Page 1"
                Dim pCntlNextTop As Integer = pTopPadding
                Dim pCntlNextLeft As Integer = pLeftPadding
                For Each tb As TabPage In s_tbCntCIPDetails.TabPages


                    Dim bLoop As Boolean = True
                    While bLoop = True

                        If tb.Controls.Count = 0 Then
                            Exit While

                        End If

                        Dim cnt As Control = tb.Controls(0)

                        If TypeOf cnt Is System.Windows.Forms.Button Then
                            tb.Controls.Remove(cnt)

                        Else


                            cnt.Top = pCntlNextTop
                            cnt.Left = pCntlNextLeft
                            cnt.Width = My.Globals.Constants.c_ControlWidth
                            If TypeOf cnt Is Panel Then
                                For Each pnlCnt As Control In cnt.Controls

                                    If TypeOf pnlCnt Is System.Windows.Forms.Button Then
                                        Dim controls() As Control = CType(CType(pnlCnt, System.Windows.Forms.Button).Parent, Panel).Controls.Find("txtEdit" & pnlCnt.Tag, False)
                                        If controls.Length = 1 Then
                                            controls(0).Width = cnt.Width - cnt.Height - 5
                                            pnlCnt.Left = controls(0).Width + controls(0).Left + 5

                                        End If
                                    ElseIf TypeOf pnlCnt Is CustomPanel Then
                                        pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding
                                        If pnlCnt.Controls.Count = 2 Then
                                            pnlCnt.Controls(0).Left = pLeftPadding
                                            pnlCnt.Controls(1).Left = (pnlCnt.Width / 2)
                                        End If

                                    Else
                                        pnlCnt.Width = cnt.Width - pLeftPadding - pRightPadding
                                    End If

                                    'End If


                                Next

                            End If


                            If pCntlNextTop + cnt.Height + pTopPadding >= s_tbCntCIPDetails.Parent.Parent.Height - s_gpBxControls.Height - pBottomPadding - pBottomPadding Then
                                'Dim pBtn As System.Windows.Forms.Button
                                'pBtn = New System.Windows.Forms.Button
                                'pBtn.Name = "btnSaveEdit"
                                'pBtn.Text = "Save"
                                'pBtn.Font = My.Globals.Constants.c_Fnt
                                'pBtn.Top = pCntlNextTop
                                'pBtn.AutoSize = True

                                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                'pCurTabPage.Controls.Add(pBtn)
                                'pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                                'pBtn = New System.Windows.Forms.Button
                                'pBtn.Name = "btnClearEdit"
                                'pBtn.Text = "Clear"
                                'pBtn.Font = My.Globals.Constants.c_Fnt
                                'pBtn.Top = pCntlNextTop
                                'pBtn.AutoSize = True

                                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                                'pCurTabPage.Controls.Add(pBtn)
                                'pBtn.Left = (tbCntCIPDetails.Width / 2) + 10

                                'If pCntlNextLeft + pCntSpacing + (My.Globals.Constants.c_ControlWidth * 2) > s_tbCntlDisplay.Width Then
                                If pCntlNextLeft + pCntSpacing + (My.Globals.Constants.c_ControlWidth * 2) > s_tbCntCIPDetails.Parent.Parent.Width - s_gpBxSwitch.Width Then
                                    If pTbPageCo Is Nothing Then
                                        ReDim Preserve pTbPageCo(0)
                                    Else
                                        ReDim Preserve pTbPageCo(pTbPageCo.Length)
                                    End If
                                    pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage

                                    pCurTabPage = New TabPage
                                    pCurTabPage.Name = "Page" & pTbPageCo.Length + 1
                                    pCurTabPage.Text = "Page" & pTbPageCo.Length + 1

                                    pCntlNextTop = pTopPadding
                                    pCntlNextLeft = pLeftPadding
                                Else
                                    pCntlNextTop = pTopPadding
                                    pCntlNextLeft = pCntlNextLeft + My.Globals.Constants.c_ControlWidth + pCntSpacing
                                End If

                                cnt.Top = pCntlNextTop
                                cnt.Left = pCntlNextLeft
                                pCurTabPage.Controls.Add(cnt)
                                pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                                'pBtn = Nothing
                            Else
                                pCurTabPage.Controls.Add(cnt)
                                pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding

                            End If



                        End If
                    End While
                Next



                If pCurTabPage.Controls.Count > 0 Then
                    'Dim pBtn As System.Windows.Forms.Button
                    'pBtn = New System.Windows.Forms.Button
                    'pBtn.Name = "btnSaveEdit"
                    'pBtn.Text = "Save"
                    'pBtn.Font = My.Globals.Constants.c_Fnt
                    'pBtn.Top = pCntlNextTop
                    'pBtn.AutoSize = True

                    'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                    'pCurTabPage.Controls.Add(pBtn)
                    'pBtn.Left = (tbCntCIPDetails.Width / 2) - pBtn.Width - 10

                    'pBtn = New System.Windows.Forms.Button
                    'pBtn.Name = "btnClearEdit"
                    'pBtn.Text = "Clear"
                    'pBtn.Font = My.Globals.Constants.c_Fnt
                    'pBtn.Top = pCntlNextTop
                    'pBtn.AutoSize = True

                    'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                    'pCurTabPage.Controls.Add(pBtn)
                    'pBtn.Left = (tbCntCIPDetails.Width / 2) + 10
                    If pTbPageCo Is Nothing Then
                        ReDim Preserve pTbPageCo(0)
                    Else
                        ReDim Preserve pTbPageCo(pTbPageCo.Length)
                    End If

                    pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                Else

                End If

                s_tbCntCIPDetails.TabPages.Clear()
                For Each tbp As TabPage In pTbPageCo

                    s_tbCntCIPDetails.TabPages.Add(tbp)

                    tbp.Visible = True

                    tbp.Update()
                Next
                If s_tbCntCIPDetails.TabPages.Count >= pCurTabIdx Then
                    s_tbCntCIPDetails.SelectedIndex = pCurTabIdx
                Else
                    s_tbCntCIPDetails.SelectedIndex = s_tbCntCIPDetails.TabPages.Count - 1
                End If

                pTbPageCo = Nothing
                pCurTabPage = Nothing
            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: ShuffleControls" & vbCrLf & ex.Message)

            End Try

        End If

    End Sub
    Private Shared Sub AddControls()
        Try




            'Exit if the layer is not found
            If My.Globals.Variables.v_CIPLayerOver Is Nothing Then Exit Sub
            If My.Globals.Variables.v_CIPLayerOver.FeatureClass Is Nothing Then Exit Sub

            Dim pLeftPadding As Integer = 10

            'Clear out the controls from the container
            s_tbCntCIPDetails.TabPages.Clear()
            s_tbCntCIPDetails.Controls.Clear()
            Dim pTbPg As TabPage = New TabPage
            s_tbCntCIPDetails.TabPages.Add(pTbPg)


            'Controls to display attributes
            'Dim pTbPg As TabPage = Nothing
            Dim pTxtBox As TextBox
            Dim pLbl As Label
            Dim pNumBox As NumericUpDown
            Dim pBtn As System.Windows.Forms.Button
            Dim pCBox As ComboBox
            Dim pRDButton As RadioButton

            Dim pDateTime As DateTimePicker
            'Spacing between each control
            Dim intCtrlSpace As Integer = 5
            'Spacing between a lable and a control
            Dim intLabelCtrlSpace As Integer = 0


            'Set the width of each control
            '   Dim my.Globals.Constants.c_ControlWidth As Integer = 50
            'used for sizing text, only used when text is larger then display
            Dim g As Graphics
            Dim s As SizeF


            'Used to loop through featurelayer
            Dim pDCs As IFields
            Dim pDc As IField
            Dim pSubTypeDefValue As Integer = 0

            'Get the columns for hte layer
            pDCs = My.Globals.Variables.v_CIPLayerOver.FeatureClass.Fields
            Dim pSubType As ISubtypes = My.Globals.Variables.v_CIPLayerOver.FeatureClass
            If pSubType.HasSubtype Then
                pSubTypeDefValue = pSubType.DefaultSubtypeCode     'pfl.Columns(pfl.SubtypeColumnIndex).DefaultValue
            End If


            'Field Name
            Dim strfld As String
            'Field Alias
            Dim strAli As String


            Dim pDom As IDomain
            For i = 0 To pDCs.FieldCount - 1
                pDc = pDCs.Field(i)
                Dim pLayerFields As ILayerFields
                Dim pFieldInfo As IFieldInfo

                pLayerFields = My.Globals.Variables.v_CIPLayerOver
                pFieldInfo = pLayerFields.FieldInfo(pLayerFields.FindField(pDc.Name))
                '  pFieldInfo.Visible = False
                If pFieldInfo.Visible = False Then
                ElseIf pDc.Name = My.Globals.Constants.c_CIPProjectLayCostField Then
                ElseIf pDc.Name = My.Globals.Constants.c_CIPProjectLayTotLenField Then
                ElseIf pDc.Name = My.Globals.Constants.c_CIPProjectLayTotAreaField Then
                ElseIf pDc.Name = My.Globals.Constants.c_CIPProjectLayTotPntField Then
                Else


                    pDom = Nothing

                    'Get the field names
                    strfld = pDc.Name
                    strAli = pDc.AliasName


                    'Check the field types
                    If My.Globals.Variables.v_CIPLayerOver.FeatureClass.ShapeFieldName = strfld Or My.Globals.Variables.v_CIPLayerOver.FeatureClass.OIDFieldName = strfld Or _
                         UCase(strfld) = UCase("shape.len") Or UCase(strfld) = UCase("shape.area") Or _
                         UCase(strfld) = UCase("shape_length") Or _
                         UCase(strfld) = UCase("shape_len") Or UCase(strfld) = UCase("shape_area") Or _
                           UCase(strfld) = UCase("LASTUPDATE") Or UCase(strfld) = UCase("LASTEDITOR") Or pDc.Editable = False _
                           Or My.Globals.Variables.v_CIPLayerOver.FeatureClass.AreaField.Name = strfld Or My.Globals.Variables.v_CIPLayerOver.FeatureClass.LengthField.Name = strfld Then



                        'Reserved Columns
                    ElseIf pSubType.SubtypeFieldName = strfld Then
                        'Create a lable for the field name
                        pLbl = New Label
                        'Apply the field alias to the field name
                        pLbl.Text = strAli & " (Set This Value First)"
                        'Link the field to the name of the control
                        pLbl.Name = "lblEdit" & strfld

                        'Add the control at the determined Location

                        pLbl.Left = 0

                        pLbl.Top = 0
                        pLbl.ForeColor = Color.Blue

                        'Apply global font
                        pLbl.Font = My.Globals.Constants.c_FntLbl
                        'Create a graphics object to messure the text
                        g = pLbl.CreateGraphics
                        s = g.MeasureString(pLbl.Text, pLbl.Font)

                        pLbl.Height = s.Height
                        'If the text is larger then the control, truncate the control
                        If s.Width >= My.Globals.Constants.c_ControlWidth Then
                            pLbl.Width = My.Globals.Constants.c_ControlWidth
                        Else 'Use autosize if it fits
                            pLbl.AutoSize = True
                        End If


                        If My.Globals.Functions.SubtypeCount(pSubType.Subtypes) = 2 Then
                            Dim pNewGpBox As New CustomPanel
                            pNewGpBox.Tag = strfld
                            pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None
                            pNewGpBox.BackColor = Color.White
                            '  pNewGpBox.BorderColor = Pens.LightGray

                            pNewGpBox.Width = My.Globals.Constants.c_ControlWidth
                            pNewGpBox.Top = 0
                            pNewGpBox.Left = 0

                            pRDButton = New RadioButton
                            pRDButton.Font = My.Globals.Constants.c_Fnt
                            pRDButton.Name = "Rdo1Sub"
                            Dim codeVal As String = "", displayVal As String = ""
                            My.Globals.Functions.SubtypeValuesAtIndex(0, pSubType, codeVal, displayVal)
                            pRDButton.Tag = codeVal

                            pRDButton.Text = displayVal

                            pRDButton.Left = pLeftPadding


                            pRDButton.AutoSize = True
                            pNewGpBox.Controls.Add(pRDButton)


                            pNewGpBox.Height = pRDButton.Height + 12
                            pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2


                            pRDButton = New RadioButton
                            pRDButton.Font = My.Globals.Constants.c_Fnt
                            pRDButton.Name = "Rdo2Sub"
                            My.Globals.Functions.SubtypeValuesAtIndex(1, pSubType, codeVal, displayVal)

                            pRDButton.Tag = codeVal
                            pRDButton.Text = displayVal
                            pRDButton.Left = pNewGpBox.Width / 2


                            pRDButton.AutoSize = True
                            pNewGpBox.Controls.Add(pRDButton)
                            pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2




                            Dim pPnl As Panel = New Panel
                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pNewGpBox.Top = pLbl.Height + 5

                            pPnl.Width = My.Globals.Constants.c_ControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = 0
                            pPnl.Left = 0
                            pPnl.Height = pNewGpBox.Height + pLbl.Height + 10
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pNewGpBox)

                            pTbPg.Controls.Add(pPnl)

                            pNewGpBox = Nothing
                            '  pPf = Nothing

                        Else
                            pCBox = New ComboBox
                            pCBox.Tag = strfld
                            pCBox.Name = "cboEdt" & strfld
                            pCBox.Left = 0
                            pCBox.Top = 0
                            pCBox.Width = My.Globals.Constants.c_ControlWidth
                            pCBox.Height = pCBox.Height + 5
                            pCBox.DropDownStyle = ComboBoxStyle.DropDownList

                            pCBox.Font = My.Globals.Constants.c_Fnt
                            pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                            pCBox.DataSource = My.Globals.Functions.SubtypeToList(pSubType)
                            pCBox.DisplayMember = "Display"
                            pCBox.ValueMember = "Value"
                            ' pCmdBox.MaxLength = pDc.Length





                            AddHandler pCBox.SelectionChangeCommitted, AddressOf cmbSubTypChange_Click




                            Dim pPnl As Panel = New Panel
                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pCBox.Top = pLbl.Height + 5

                            pPnl.Width = My.Globals.Constants.c_ControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = 0
                            pPnl.Left = 0
                            pPnl.Height = pCBox.Height + pLbl.Height + 15
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pCBox)

                            pTbPg.Controls.Add(pPnl)
                            Dim codeVal As String = "", displayVal As String = ""
                            My.Globals.Functions.SubtypeValuesAtIndex(0, pSubType, codeVal, displayVal)

                            pCBox.Text = displayVal

                        End If


                    Else
                        If pSubType.HasSubtype Then

                            pDom = pSubType.Domain(pSubTypeDefValue, pDc.Name)


                        Else
                            pDom = pDc.Domain

                        End If
                        'No Domain Found
                        If pDom Is Nothing Then


                            If pDc.Type = esriFieldType.esriFieldTypeString Or _
                               pDc.Type = esriFieldType.esriFieldTypeDouble Or _
                               pDc.Type = esriFieldType.esriFieldTypeInteger Or _
                                pDc.Type = esriFieldType.esriFieldTypeSingle Or _
                               pDc.Type = esriFieldType.esriFieldTypeSmallInteger Then

                                'Create a lable for the field name
                                pLbl = New Label
                                'Apply the field alias to the field name
                                pLbl.Text = strAli
                                'Link the field to the name of the control
                                pLbl.Name = "lblEdit" & strfld
                                'Add the control at the determined Location
                                pLbl.Left = 0

                                pLbl.Top = 0
                                'Apply global font
                                pLbl.Font = My.Globals.Constants.c_FntLbl
                                'Create a graphics object to messure the text
                                g = pLbl.CreateGraphics
                                s = g.MeasureString(pLbl.Text, pLbl.Font)

                                pLbl.Height = s.Height
                                'If the text is larger then the control, truncate the control
                                If s.Width >= My.Globals.Constants.c_ControlWidth Then
                                    pLbl.Width = My.Globals.Constants.c_ControlWidth
                                Else 'Use autosize if it fits
                                    pLbl.AutoSize = True
                                End If

                                'Create a new control to display the attributes                    
                                pTxtBox = New TextBox

                                'Tag the control with the field it represents
                                pTxtBox.Tag = Trim(strfld)
                                'Name the field with the field name
                                pTxtBox.Name = "txtEdit" & strfld
                                'Locate the control on the display
                                pTxtBox.Left = 0

                                pTxtBox.Width = My.Globals.Constants.c_ControlWidth
                                If pDc.Type = esriFieldType.esriFieldTypeString Then
                                    'Make the box taller if it is a long field
                                    If pDc.Length > 125 Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * 3

                                    End If

                                End If
                                If pDc.Length > 0 Then
                                    pTxtBox.MaxLength = pDc.Length
                                End If


                                'Apply global font
                                pTxtBox.Font = My.Globals.Constants.c_Fnt

                                'Group into panels to assist resizing
                                Dim pPnl As Panel = New Panel
                                pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                pLbl.Top = 0
                                pTxtBox.Top = 5 + pLbl.Height
                                pPnl.Width = My.Globals.Constants.c_ControlWidth
                                pPnl.Margin = New Padding(0)
                                pPnl.Padding = New Padding(0)





                                pPnl.Top = 0
                                pPnl.Left = 0
                                pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                                pPnl.Controls.Add(pLbl)
                                pPnl.Controls.Add(pTxtBox)
                                pTbPg.Controls.Add(pPnl)

                            ElseIf pDc.Type = esriFieldType.esriFieldTypeDate Then
                                'Create a lable for the field name
                                pLbl = New Label
                                'Apply the field alias to the field name
                                pLbl.Text = strAli
                                'Link the field to the name of the control
                                pLbl.Name = "lblEdit" & strfld
                                'Add the control at the determined Location
                                pLbl.Left = 0

                                '   pLbl.Top = pNextControlTop
                                'Apply global font
                                pLbl.Font = My.Globals.Constants.c_FntLbl
                                'Create a graphics object to messure the text
                                g = pLbl.CreateGraphics
                                s = g.MeasureString(pLbl.Text, pLbl.Font)

                                pLbl.Height = s.Height
                                'If the text is larger then the control, truncate the control
                                If s.Width >= My.Globals.Constants.c_ControlWidth Then
                                    pLbl.Width = My.Globals.Constants.c_ControlWidth
                                Else 'Use autosize if it fits
                                    pLbl.AutoSize = True
                                End If
                                'Determine the Location for the next control
                                '   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                                pDateTime = New DateTimePicker
                                pDateTime.Font = My.Globals.Constants.c_Fnt
                                'pDateTime.CustomFormat = "m/d/yyyy"
                                pDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
                                pDateTime.CustomFormat = "M-d-yy" ' h:mm tt"
                                pDateTime.ShowCheckBox = True
                                pDateTime.Tag = strfld
                                pDateTime.Name = "dtEdt" & strfld
                                pDateTime.Left = 0
                                '   pDateTime.Top = pNextControlTop
                                pDateTime.Width = My.Globals.Constants.c_ControlWidth



                                'Determine the Location for the next control
                                'pNextControlTop = pDateTime.Top + pDateTime.Height + intCtrlSpace
                                Dim pPnl As Panel = New Panel
                                pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                pLbl.Top = 0
                                pDateTime.Top = 5 + pLbl.Height
                                pPnl.Width = My.Globals.Constants.c_ControlWidth
                                pPnl.Margin = New Padding(0)
                                pPnl.Padding = New Padding(0)





                                pPnl.Top = 0
                                pPnl.Left = 0
                                pPnl.Height = pDateTime.Height + pLbl.Height + 10
                                pPnl.Controls.Add(pLbl)
                                pPnl.Controls.Add(pDateTime)
                                pTbPg.Controls.Add(pPnl)

                            ElseIf pDc.Type = esriFieldType.esriFieldTypeBlob Then

                                'Create a lable for the field name
                                pLbl = New Label
                                'Apply the field alias to the field name
                                pLbl.Text = strAli
                                'Link the field to the name of the control
                                pLbl.Name = "lblEdit" & strfld
                                'Add the control at the determined Location
                                pLbl.Left = 0
                                pLbl.Top = 0
                                'Apply global font
                                pLbl.Font = My.Globals.Constants.c_FntLbl
                                'Create a graphics object to messure the text
                                g = pLbl.CreateGraphics
                                s = g.MeasureString(pLbl.Text, pLbl.Font)
                                pLbl.Height = s.Height
                                'If the text is larger then the control, truncate the control
                                If s.Width >= My.Globals.Constants.c_ControlWidth Then
                                    pLbl.Width = 0
                                Else 'Use autosize if it fits
                                    pLbl.AutoSize = True
                                End If
                                'Determine the Location for the next control


                                'Create a new control to display the attributes                    
                                pTxtBox = New TextBox
                                'Disable the control
                                '  pPic.ReadOnly = True
                                'Tag the control with the field it represents
                                pTxtBox.Tag = Trim(strfld)
                                'Name the field with the field name
                                pTxtBox.Name = "txtEdit" & strfld
                                'Locate the control on the display
                                pTxtBox.Left = 0
                                pTxtBox.Top = 0
                                pTxtBox.Width = My.Globals.Constants.c_ControlWidth - pTxtBox.Height
                                If pDc.Type = esriFieldType.esriFieldTypeString Then
                                    'Make the box taller if it is a long field
                                    If pDc.Length > 125 Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * 3

                                    End If

                                End If
                                If pDc.Length > 0 Then
                                    pTxtBox.MaxLength = pDc.Length
                                End If

                                pTxtBox.BackgroundImageLayout = ImageLayout.Stretch

                                'Apply global font
                                pTxtBox.Font = My.Globals.Constants.c_Fnt

                                pBtn = New Windows.Forms.Button

                                pBtn.Tag = Trim(strfld)
                                'Name the field with the field name
                                pBtn.Name = "btnEdit" & strfld
                                'Locate the control on the display
                                pBtn.Left = pTxtBox.Left + pTxtBox.Width + 5
                                pBtn.Top = 0
                                Dim img As System.Drawing.Bitmap

                                img = My.Resources.Open2

                                img.MakeTransparent(img.GetPixel(img.Width - 1, 1))

                                pBtn.BackgroundImageLayout = ImageLayout.Center
                                pBtn.BackgroundImage = img
                                img = Nothing
                                pBtn.Width = pTxtBox.Height
                                pBtn.Height = pTxtBox.Height

                                AddHandler pBtn.Click, AddressOf btnLoadImgClick


                                Dim pPnl As Panel = New Panel
                                pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                pLbl.Top = 0
                                pTxtBox.Top = 5 + pLbl.Height
                                pBtn.Top = pTxtBox.Top
                                pPnl.Width = My.Globals.Constants.c_ControlWidth
                                pPnl.Margin = New Padding(0)
                                pPnl.Padding = New Padding(0)





                                pPnl.Top = 0
                                pPnl.Left = 0
                                pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                                pPnl.Controls.Add(pLbl)
                                pPnl.Controls.Add(pTxtBox)
                                pPnl.Controls.Add(pBtn)
                                pTbPg.Controls.Add(pPnl)

                            End If
                        Else
                            If TypeOf pDom Is CodedValueDomain Then
                                Dim pCV As ICodedValueDomain

                                'Create a lable for the field name
                                pLbl = New Label
                                'Apply the field alias to the field name
                                pLbl.Text = strAli
                                'Link the field to the name of the control
                                pLbl.Name = "lblEdit" & strfld
                                'Add the control at the determined Location
                                pLbl.Left = 0
                                pLbl.Top = 0
                                'Apply global font
                                pLbl.Font = My.Globals.Constants.c_FntLbl
                                'Create a graphics object to messure the text
                                g = pLbl.CreateGraphics
                                s = g.MeasureString(pLbl.Text, pLbl.Font)
                                pLbl.Height = s.Height
                                'If the text is larger then the control, truncate the control
                                If s.Width >= My.Globals.Constants.c_ControlWidth Then
                                    pLbl.Width = My.Globals.Constants.c_ControlWidth
                                Else 'Use autosize if it fits
                                    pLbl.AutoSize = True
                                End If
                                'Determine the Location for the next control
                                '    pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                                pCV = CType(pDom, CodedValueDomain)
                                ' pTbPg.Controls.Add(pLbl)

                                If pCV.CodeCount = 2 Then
                                    Dim pNewGpBox As New CustomPanel
                                    pNewGpBox.Tag = strfld
                                    pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None
                                    pNewGpBox.BackColor = Color.White
                                    '  pNewGpBox.BorderColor = Pens.LightGray

                                    pNewGpBox.Width = My.Globals.Constants.c_ControlWidth
                                    pNewGpBox.Top = 0
                                    pNewGpBox.Left = 0

                                    pRDButton = New RadioButton
                                    pRDButton.Name = "Rdo1"
                                    Dim codeVal As String = "", displayVal As String = ""
                                    My.Globals.Functions.DomainValuesAtIndex(0, pCV, codeVal, displayVal)

                                    pRDButton.Tag = codeVal
                                    pRDButton.Text = displayVal
                                    pRDButton.Font = My.Globals.Constants.c_Fnt
                                    'Dim pPf As SizeF = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)

                                    ''pRDButton.Height = pPf.Height
                                    'pRDButton.Width = pPf.Width + 25

                                    pRDButton.Left = pLeftPadding

                                    pRDButton.AutoSize = True
                                    pNewGpBox.Controls.Add(pRDButton)


                                    pNewGpBox.Height = pRDButton.Height + 12
                                    pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2


                                    pRDButton = New RadioButton
                                    pRDButton.Font = My.Globals.Constants.c_Fnt
                                    pRDButton.Name = "Rdo2"
                                    My.Globals.Functions.DomainValuesAtIndex(1, pCV, codeVal, displayVal)

                                    pRDButton.Tag = codeVal
                                    pRDButton.Text = displayVal
                                    pRDButton.Left = pNewGpBox.Width / 2
                                    'pPf = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)
                                    'pRDButton.Height = pPf.Height
                                    'pRDButton.Width = pPf.Width + 25


                                    pRDButton.AutoSize = True
                                    pNewGpBox.Controls.Add(pRDButton)
                                    pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2


                                    ' pTbPg.Controls.Add(pNewGpBox)

                                    '  pNextControlTop = pNewGpBox.Top + pNewGpBox.Height + 7 + intLabelCtrlSpace


                                    Dim pPnl As Panel = New Panel
                                    pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                    pLbl.Top = 0
                                    pNewGpBox.Top = 5 + pLbl.Height

                                    pPnl.Width = My.Globals.Constants.c_ControlWidth
                                    pPnl.Margin = New Padding(0)
                                    pPnl.Padding = New Padding(0)





                                    pPnl.Top = 0
                                    pPnl.Left = 0
                                    pPnl.Height = pNewGpBox.Height + pLbl.Height + 10
                                    pPnl.Controls.Add(pLbl)
                                    pPnl.Controls.Add(pNewGpBox)

                                    pTbPg.Controls.Add(pPnl)

                                    pNewGpBox = Nothing
                                    '  pPf = Nothing

                                Else
                                    pCBox = New ComboBox
                                    pCBox.Tag = strfld
                                    pCBox.Name = "cboEdt" & strfld
                                    pCBox.Left = 0
                                    pCBox.Top = 0
                                    pCBox.Width = My.Globals.Constants.c_ControlWidth
                                    pCBox.Height = pCBox.Height + 5
                                    pCBox.DropDownStyle = ComboBoxStyle.DropDownList
                                    pCBox.Font = My.Globals.Constants.c_Fnt
                                    pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                                    pCBox.DataSource = My.Globals.Functions.DomainToList(pCV)
                                    pCBox.DisplayMember = "Display"
                                    pCBox.ValueMember = "Value"
                                    ' pCmdBox.MaxLength = pDc.Length





                                    Dim pPnl As Panel = New Panel
                                    pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                    pLbl.Top = 0
                                    pCBox.Top = 5 + pLbl.Height

                                    pPnl.Width = My.Globals.Constants.c_ControlWidth
                                    pPnl.Margin = New Padding(0)
                                    pPnl.Padding = New Padding(0)





                                    pPnl.Top = 0
                                    pPnl.Left = 0
                                    pPnl.Height = pCBox.Height + pLbl.Height + 15
                                    pPnl.Controls.Add(pLbl)
                                    pPnl.Controls.Add(pCBox)

                                    pTbPg.Controls.Add(pPnl)


                                    '   pTbPg.Controls.Add(pCBox)
                                    ' MsgBox(pCBox.Items.Count)
                                    pCBox.Visible = True

                                    Dim codeVal As String = "", displayVal As String = ""
                                    My.Globals.Functions.DomainValuesAtIndex(0, pCV, codeVal, displayVal)

                                    pCBox.Text = displayVal

                                    'Try

                                    'pCBox.SelectedIndex = 0
                                    'Catch ex As Exception

                                    'End Try
                                    pCBox.Visible = True
                                    pCBox.Refresh()

                                    '  pNextControlTop = pCBox.Top + pCBox.Height + 7 + intLabelCtrlSpace
                                End If


                            ElseIf TypeOf pDom Is RangeDomain Then
                                Dim pRV As IRangeDomain
                                'Create a lable for the field name
                                pLbl = New Label
                                'Apply the field alias to the field name
                                pLbl.Text = strAli
                                'Link the field to the name of the control
                                pLbl.Name = "lblEdit" & strfld
                                'Add the control at the determined Location
                                pLbl.Left = 0
                                pLbl.Top = 0
                                'Apply global font
                                pLbl.Font = My.Globals.Constants.c_FntLbl
                                'Create a graphics object to messure the text
                                g = pLbl.CreateGraphics
                                s = g.MeasureString(pLbl.Text, pLbl.Font)
                                pLbl.Height = s.Height
                                'If the text is larger then the control, truncate the control
                                If s.Width >= My.Globals.Constants.c_ControlWidth Then
                                    pLbl.Width = My.Globals.Constants.c_ControlWidth
                                Else 'Use autosize if it fits
                                    pLbl.AutoSize = True
                                End If
                                'Determine the Location for the next control

                                pRV = CType(pDom, RangeDomain)
                                pNumBox = New NumericUpDown
                                '    AddHandler pNumBox.MouseDown, AddressOf numericClickEvt_MouseDown




                                If pDc.Type = esriFieldType.esriFieldTypeInteger Then
                                    pNumBox.DecimalPlaces = 0

                                ElseIf pDc.Type = esriFieldType.esriFieldTypeDouble Then

                                    pNumBox.DecimalPlaces = 2 'pDc.DataType.
                                ElseIf pDc.Type = esriFieldType.esriFieldTypeSingle Then

                                    pNumBox.DecimalPlaces = 1 'pDc.DataType.
                                Else
                                    pNumBox.DecimalPlaces = 2 'pDc.DataType.
                                End If

                                pNumBox.Minimum = pRV.MinValue
                                pNumBox.Maximum = pRV.MaxValue
                                Dim pf As NumericUpDownAcceleration = New NumericUpDownAcceleration(3, CInt((pNumBox.Maximum - pNumBox.Minimum) * 0.02))


                                pNumBox.Accelerations.Add(pf)

                                pNumBox.Tag = strfld
                                pNumBox.Name = "numEdt" & strfld
                                pNumBox.Left = 0
                                pNumBox.BackColor = Color.White
                                pNumBox.Top = 0
                                pNumBox.Width = My.Globals.Constants.c_ControlWidth
                                pNumBox.Font = My.Globals.Constants.c_Fnt

                                Dim pPnl As Panel = New Panel
                                pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                pLbl.Top = 0
                                pNumBox.Top = 5 + pLbl.Height

                                pPnl.Width = My.Globals.Constants.c_ControlWidth
                                pPnl.Margin = New Padding(0)
                                pPnl.Padding = New Padding(0)





                                pPnl.Top = 0
                                pPnl.Left = 0
                                pPnl.Height = pNumBox.Height + pLbl.Height + 15
                                pPnl.Controls.Add(pLbl)
                                pPnl.Controls.Add(pNumBox)

                                pTbPg.Controls.Add(pPnl)

                            End If

                        End If

                    End If
                    pLayerFields = Nothing
                    pFieldInfo = Nothing


                End If
            Next 'pDC



            If pSubType.HasSubtype Then
                SubtypeChange(pSubTypeDefValue, pSubType.SubtypeFieldName)

            End If
            'cleanup
            pBtn = Nothing
            pDCs = Nothing
            pDc = Nothing

            pTbPg = Nothing

            pTxtBox = Nothing
            pLbl = Nothing
            pNumBox = Nothing

            pRDButton = Nothing

            pCBox = Nothing
            pDateTime = Nothing
            g = Nothing
            s = Nothing
            s_tbCntCIPDetails.ResumeLayout()
            s_tbCntCIPDetails.Refresh()

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: AddControls" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub cmbSubTypChange_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If CType(sender, ComboBox).SelectedIndex = -1 Then Return

        SubtypeChange(CType(sender, ComboBox).SelectedValue, CType(sender, ComboBox).Tag)

    End Sub

    Private Shared Sub SubtypeChange(ByVal value As Integer, ByVal SubtypeField As String)
        Try
            Dim intSubVal As Integer = value

            'Feature layer being Identified

            'Exit if the layer is not found
            If My.Globals.Variables.v_CIPLayerOver Is Nothing Then Exit Sub

            Dim strFld As String
            Dim pCmbBox As ComboBox
            Dim pNUP As NumericUpDown
            Dim pCV As ICodedValueDomain
            Dim pRg As IRangeDomain

            Dim pSubTypes As ISubtypes = My.Globals.Variables.v_CIPLayerOver.FeatureClass
            Dim pLeftPadding As Integer = 10



            'Loop through all controls 
            For Each tbPg As TabPage In s_tbCntCIPDetails.TabPages
                For Each cntrl As Control In tbPg.Controls
                    'If the control is a combobox, then reapply the domain
                    If TypeOf cntrl Is Panel Then

                        For Each cntrlPnl As Control In cntrl.Controls
                            If TypeOf cntrlPnl Is ComboBox Then

                                pCmbBox = cntrlPnl
                                If SubtypeField <> pCmbBox.Tag.ToString Then

                                    'Get the Field
                                    strFld = pCmbBox.Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get the domain

                                    pCV = pSubTypes.Domain(intSubVal, strFld)
                                    If pCV Is Nothing Then
                                        pCmbBox.DataSource = Nothing

                                    Else
                                        'If the domain has two values, remove the combo box and add a custompanel
                                        If pCV.CodeCount = 2 Then
                                            Dim pNewGpBox As New CustomPanel
                                            Dim pRDButton As RadioButton
                                            pNewGpBox.Tag = pCmbBox.Tag
                                            pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None
                                            pNewGpBox.BackColor = Color.White
                                            '  pNewGpBox.BorderColor = Pens.LightGray

                                            pNewGpBox.Width = pCmbBox.Width
                                            pNewGpBox.Top = pCmbBox.Top
                                            pNewGpBox.Left = pCmbBox.Left

                                            pRDButton = New RadioButton
                                            pRDButton.Name = "Rdo1"
                                            Dim codeVal As String = "", displayVal As String = ""

                                            My.Globals.Functions.SubtypeValuesAtIndex(0, pCV, codeVal, displayVal)

                                            pRDButton.Tag = codeVal
                                            pRDButton.Text = displayVal

                                            pRDButton.Left = pLeftPadding

                                            pRDButton.AutoSize = True
                                            pNewGpBox.Controls.Add(pRDButton)


                                            pNewGpBox.Height = pRDButton.Height + 12
                                            pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2


                                            pRDButton = New RadioButton
                                            pRDButton.Name = "Rdo2"
                                            My.Globals.Functions.SubtypeValuesAtIndex(1, pCV, codeVal, displayVal)

                                            pRDButton.Tag = codeVal
                                            pRDButton.Text = displayVal

                                            pRDButton.Left = pNewGpBox.Width / 2

                                            pRDButton.AutoSize = True
                                            pNewGpBox.Controls.Add(pRDButton)
                                            pRDButton.Top = pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2


                                            tbPg.Controls.Add(pNewGpBox)
                                            Try

                                                tbPg.Controls.Remove(pCmbBox)
                                                'Dim cnts() As Control = tbPg.Controls.Find("lblEdit" & strFld, False)
                                                'If cnts.Length > 0 Then
                                                '    tbPg.Controls.Remove(cnts(0))
                                                'End If


                                            Catch ex As Exception

                                            End Try

                                            pNewGpBox = Nothing
                                            pRDButton = Nothing

                                        Else
                                            'Set the domain value

                                            pCmbBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                                            pCmbBox.DataSource = My.Globals.Functions.DomainToList(pCV)
                                            pCmbBox.DisplayMember = "Display"
                                            pCmbBox.ValueMember = "Value"
                                            pCmbBox.Visible = True
                                            pCmbBox.Refresh()

                                            Dim codeVal As String = "", displayVal As String = ""
                                            My.Globals.Functions.SubtypeValuesAtIndex(0, pCV, codeVal, displayVal)
                                            pCmbBox.Text = displayVal
                                        End If

                                    End If



                                End If
                                'If the contorl is a coded value domain with two values
                            ElseIf TypeOf cntrlPnl Is CustomPanel Then


                                'Get the Field
                                strFld = cntrlPnl.Tag
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                'Get the fomain
                                pCV = pSubTypes.Domain(intSubVal, strFld)

                                If pCV Is Nothing Then
                                    cntrlPnl.Controls.Clear()


                                Else
                                    'If the domain has more than two values, remove the custompanel and add a combo box 
                                    If pCV.CodeCount = 2 Then
                                        Try
                                            'Set up the proper domain values
                                            Dim pRdoBut As RadioButton
                                            pRdoBut = cntrlPnl.Controls("Rdo1")
                                            Dim codeVal As String = "", displayVal As String = ""
                                            My.Globals.Functions.SubtypeValuesAtIndex(0, pCV, codeVal, displayVal)

                                            pRdoBut.Tag = codeVal
                                            pRdoBut.Text = displayVal

                                            pRdoBut = cntrlPnl.Controls("Rdo2")
                                            My.Globals.Functions.SubtypeValuesAtIndex(1, pCV, codeVal, displayVal)

                                            pRdoBut.Tag = codeVal
                                            pRdoBut.Text = displayVal
                                        Catch ex As Exception

                                        End Try

                                    Else
                                        Dim pCBox As ComboBox
                                        pCBox = New ComboBox
                                        pCBox.Tag = strFld
                                        pCBox.Name = "cboEdt" & strFld
                                        pCBox.Left = cntrlPnl.Left
                                        pCBox.Top = cntrlPnl.Top
                                        pCBox.Width = cntrlPnl.Width
                                        pCBox.Height = pCBox.Height + 5
                                        pCBox.DropDownStyle = ComboBoxStyle.DropDownList

                                        pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never
                                        pCBox.DataSource = My.Globals.Functions.DomainToList(pCV)
                                        pCBox.DisplayMember = "Display"
                                        pCBox.ValueMember = "Value"
                                        pCBox.Visible = True
                                        pCBox.Refresh()

                                        Dim codeVal As String = "", displayVal As String = ""
                                        My.Globals.Functions.SubtypeValuesAtIndex(0, pCV, codeVal, displayVal)
                                        pCBox.Text = displayVal
                                        ' pCmdBox.MaxLength = pDc.Length


                                        tbPg.Controls.Add(pCBox)
                                        ' MsgBox(pCBox.Items.Count)

                                        pCBox.Visible = True
                                        pCBox.Refresh()

                                        tbPg.Controls.Remove(cntrlPnl)

                                        pCBox = Nothing

                                    End If

                                End If
                                'If the contorl is a range domain
                            ElseIf TypeOf cntrlPnl Is NumericUpDown Then
                                'get the control
                                pNUP = cntrlPnl
                                'Get the field
                                strFld = pNUP.Tag
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                'Get the domain
                                pRg = pSubTypes.Domain(intSubVal, strFld)
                                If pRg Is Nothing Then
                                    pNUP.Enabled = False

                                Else
                                    pNUP.Enabled = True
                                    pNUP.Minimum = pRg.MinValue
                                    pNUP.Maximum = pRg.MaxValue
                                End If


                                pNUP.Refresh()
                            End If
                        Next

                    End If
                Next
            Next
        Catch ex As Exception
            MsgBox("Error in the edit control subtype change" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub btnLoadImgClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            'Opens a dialog to browse out for an image
            Dim openFileDialog1 As System.Windows.Forms.OpenFileDialog

            openFileDialog1 = New System.Windows.Forms.OpenFileDialog()



            'Filter the image types
            openFileDialog1.Filter = "Jpg (*.jpg) |*.jpg|Bitmap (*.bmp) |*.bmp|Gif (*.gif)| *.gif"
            'If the user selects an image
            If openFileDialog1.ShowDialog() = DialogResult.OK Then
                'Set the path of the image to the text box
                Dim controls() As Control = CType(CType(sender, Windows.Forms.Button).Parent, Panel).Controls.Find("txtEdit" & sender.tag, False)
                'If the control was found
                If controls.Length > 0 Then
                    controls(0).Text = openFileDialog1.FileName
                End If


            End If
        Catch ex As Exception
            MsgBox("Error in the edit control loading an image" & vbCrLf & ex.Message)

        End Try
    End Sub

    Private Shared Sub LoadExistingAssetsToForm(ByVal ProjectName As String)
        'Dim pCIPLayerPrj As IFeatureLayer = Nothing
        'Dim pCIPLayerOver As IFeatureLayer = Nothing
        'Dim pCIPLayerPoint As IFeatureLayer = Nothing
        'Dim pCIPLayerPolygon As IFeatureLayer = Nothing
        'Dim pCIPLayerPolyline As IFeatureLayer = Nothing
        ' Dim pCIPInvTable As ITable = Nothing

        'Dim pDefTbl As ITable
        Try



            '            pCIPLayerPoint = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPointLayName)
            '           pCIPLayerPolygon = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolygonLayName)
            'pCIPLayerPolyline = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolylineLayName)
            '
            'pCIPInvTable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPInvLayName, my.ArcMap.Document.FocusMap )

            'Dim pInvFilt As IQueryFilter = New QueryFilter
            'pInvFilt.WhereClause = my.Globals.Constants.c_CIPInvLayProjNameField & " = '" & ProjectName & "'"
            'Dim pInvCur As ICursor
            'pInvCur = pCIPInvTable.Search(pInvFilt, True)
            'Dim pInvRow As IRow = pInvCur.NextRow
            'Do Until pInvRow Is Nothing

            '    lstInventory.Items.Add(pInvRow.Value(pInvRow.Fields.FindField(my.Globals.Constants.c_CIPInvLayInvTypefield)) & ": " & pInvRow.Value(pInvRow.Fields.FindField(my.Globals.Constants.c_CIPInvLayNumOfInvField)))


            '    pInvRow = pInvCur.NextRow
            'Loop
            'pInvRow = Nothing
            'Marshal.ReleaseComObject(pInvCur)
            'pInvFilt = Nothing
            'pInvCur = Nothing


            'pDefTbl = My.Globals.Functions.FindTable(My.Globals.Constants.c_CIPDefTableName, My.ArcMap.Document.FocusMap)
            'If pDefTbl Is Nothing Then
            '    Return
            'End If

            For i = 0 To 2
                Dim pFilt As IQueryFilter = New QueryFilter
                Dim pFCur As IFeatureCursor = Nothing

                pFilt.WhereClause = My.Globals.Constants.c_CIPProjectAssetNameField & " = '" & ProjectName & "'"

                Select Case i
                    Case 0
                        pFCur = My.Globals.Variables.v_CIPLayerPoint.Search(pFilt, True)

                    Case 1
                        pFCur = My.Globals.Variables.v_CIPLayerPolygon.Search(pFilt, True)

                    Case 2
                        pFCur = My.Globals.Variables.v_CIPLayerPolyline.Search(pFilt, True)

                End Select


                Dim pfeat As IFeature
                pfeat = pFCur.NextFeature
                Dim pLastFeatLay As String = ""
                Dim pLastFeatFiltField1 As String = ""
                Dim pLastFeatFiltField2 As String = ""

                Do Until pfeat Is Nothing
                    If pLastFeatLay = "" Or pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField)) <> pLastFeatLay Then
                        Dim pDefFilt As IQueryFilter = New QueryFilter

                        pDefFilt.WhereClause = My.Globals.Constants.c_CIPDefNameField & " = '" & pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField)) & "'"
                        Dim pDefCurs As ICursor = My.Globals.Variables.v_CIPTableDef.Search(pDefFilt, True)
                        If pDefCurs Is Nothing Then Return
                        Dim pRow As IRow
                        pRow = pDefCurs.NextRow
                        pLastFeatFiltField1 = ""
                        If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot Nothing Then
                            If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot DBNull.Value Then
                                pLastFeatFiltField1 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))
                            End If
                        End If
                        pLastFeatFiltField2 = ""

                        If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot Nothing Then
                            If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot DBNull.Value Then
                                pLastFeatFiltField2 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))
                            End If
                        End If

                        pLastFeatLay = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField))


                        pRow = Nothing
                        Marshal.ReleaseComObject(pDefCurs)
                        pDefCurs = Nothing
                        pDefFilt = Nothing

                    End If

                    Dim strType As String = ""
                    Dim strID As String = ""
                    Dim strCost As String = ""
                    Dim strAddCost As String = ""
                    Dim strLen As Double = "0.0"
                    Dim strTotCost As String = ""
                    Dim strExt1 As String = ""
                    Dim strExt2 As String = ""
                    Dim strOID As String = ""
                    Dim strPro1 As String = ""
                    Dim strPro2 As String = ""
                    Dim strStrat As String = ""
                    Dim strAction As String = ""
                    Dim strMulti As String = ""
                    Dim strNotes As String = ""


                    If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField)) IsNot Nothing Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField)) IsNot DBNull.Value Then
                            strType = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField))
                        End If
                    End If


                    If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetIDField)) IsNot Nothing Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetIDField)) IsNot DBNull.Value Then
                            strID = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetIDField))
                        End If
                    End If

                    If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetCostField)) IsNot Nothing Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetCostField)) IsNot DBNull.Value Then
                            strCost = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetCostField))
                        End If
                    End If
                    If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetAddCostField)) IsNot Nothing Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetAddCostField)) IsNot DBNull.Value Then
                            strAddCost = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetAddCostField))
                        End If
                    End If
                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField)) IsNot DBNull.Value Then
                                strLen = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField))
                            End If
                        End If
                    End If
                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTotCostField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTotCostField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTotCostField)) IsNot DBNull.Value Then
                                strTotCost = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTotCostField))
                            End If
                        End If
                    End If

                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt1Field) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt1Field)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt1Field)) IsNot DBNull.Value Then
                                strExt1 = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt1Field))
                            End If
                        End If
                    End If


                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt2Field) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt2Field)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt2Field)) IsNot DBNull.Value Then
                                strExt2 = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt2Field))
                            End If
                        End If
                    End If



                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetOIDField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetOIDField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetOIDField)) IsNot DBNull.Value Then
                                strOID = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetOIDField))
                            End If
                        End If
                    End If



                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt1Field) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt1Field)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt1Field)) IsNot DBNull.Value Then
                                strPro1 = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt1Field))
                            End If
                        End If
                    End If


                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt2Field) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt2Field)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt2Field)) IsNot DBNull.Value Then
                                strPro2 = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt2Field))
                            End If
                        End If
                    End If

                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetStrategyField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetStrategyField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetStrategyField)) IsNot DBNull.Value Then
                                strStrat = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetStrategyField))
                            End If
                        End If
                    End If

                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetActionField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetActionField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetActionField)) IsNot DBNull.Value Then
                                strAction = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetActionField))
                            End If
                        End If
                    End If


                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetMultiField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetMultiField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetMultiField)) IsNot DBNull.Value Then
                                strMulti = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetMultiField))
                            End If
                        End If
                    End If



                    If pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNotesField) > 0 Then
                        If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNotesField)) IsNot Nothing Then
                            If pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNotesField)) IsNot DBNull.Value Then
                                strNotes = pfeat.Value(pfeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNotesField))
                            End If
                        End If
                    End If


                    loadRecord(pfeat.Shape, strType, strType, strID, strCost, strAddCost, strLen, strTotCost, strExt1, strExt2, _
                               pLastFeatFiltField1, pLastFeatFiltField2, strOID, strPro1, strPro2, strStrat, strAction, strMulti, strNotes)




                    pfeat = pFCur.NextFeature

                Loop



                pFilt = Nothing
                pfeat = Nothing
                Marshal.ReleaseComObject(pFCur)
                pFCur = Nothing

            Next i


            If s_dgCIP.Rows.Count > 0 Then
                s_dgCIP.Rows(0).Selected = True

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: LoadExistingAssetsToForm" & vbCrLf & ex.Message)

        End Try

    End Sub
    Public Shared Sub EnableWindowControls(ByVal enable As Boolean)
        Try
            If s_btnSketch Is Nothing Then Return

            If enable Then

                s_btnSketch.Enabled = True
                s_btnSelect.Enabled = True

                s_cboDefLayers.Enabled = True
                s_btnSelectAssets.Enabled = True
                s_btnSelectPrj.Enabled = True
                s_btnClear.Enabled = True
                s_cboStrategy.Enabled = True
                s_cboAction.Enabled = True
            Else

                s_btnSketch.Enabled = False
                s_btnSelect.Enabled = False
                s_btnSelectPrj.Enabled = False
                s_cboDefLayers.Enabled = False
                s_btnSelectAssets.Enabled = False
                s_cboStrategy.Enabled = False
                s_cboAction.Enabled = False

                'pUID.Value = "{ce0409b7-5c18-4b55-90ad-56701a01eea7}"
                'Try
                '    pCmdItem = pIDoc.CommandBars.Find(pUID)

                '    If My.ArcMap.Application.CurrentTool IsNot Nothing Then
                '        If My.ArcMap.Application.CurrentTool Is pCmdItem Then
                '            My.ArcMap.Application.CurrentTool = Nothing

                '        End If

                '    End If
                'Catch ex As Exception

                'End Try

            End If
            loadStrategyCboBox()
            loadDefLayersCboBox()

        Catch ex As Exception

            MsgBox("Error in the Costing Tools - CIPProjectWindow: EnableCIPTools" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Function FindPrjAtLocation(ByVal pPnt As IPoint) As IFeature
        Dim pSFilt As ISpatialFilter = Nothing
        Dim pFCurs As IFeatureCursor = Nothing
        Try
            'Sub to load a record to the form
            'Gets the feature layer 
            'Determine if the layer has subtypes
            pSFilt = New SpatialFilter
            pSFilt.Geometry = pPnt
            pSFilt.GeometryField = My.Globals.Variables.v_CIPLayerOver.FeatureClass.ShapeFieldName
            pSFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            pFCurs = My.Globals.Variables.v_CIPLayerOver.Search(pSFilt, True)
            If pFCurs Is Nothing Then Return Nothing


            Return pFCurs.NextFeature

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: FindPrjAtLocation" & vbCrLf & ex.Message)

            Return Nothing

        Finally
            pSFilt = Nothing
            Marshal.ReleaseComObject(pFCurs)

            pFCurs = Nothing
        End Try

    End Function
    Private Shared Function loadProjectToForm(ByVal pFeat As IFeature) As String
        If pFeat Is Nothing Then Return ""

        '    Dim pFCurs As IFeatureCursor = Nothing

        Dim pFC As IFeatureClass = Nothing
        Dim strPrjName As String = ""
        Dim strFld As String = ""

        Dim pSubType As ISubtypes = Nothing
        Dim bSubType As Boolean
        ' Dim pSFilt As ISpatialFilter = Nothing

        Try
            pFC = pFeat.Class
            pSubType = pFC
            bSubType = pSubType.HasSubtype
            'If the layer has subtypes, load the subtype value first
            If bSubType Then
                'Loop through each control in the tab control
                For Each pCntrl As Control In s_tbCntCIPDetails.Controls
                    'If the control is a tabpage
                    If TypeOf pCntrl Is TabPage Then
                        'Loop through each ocntrol on the tab oage
                        For Each cCntrl As Control In pCntrl.Controls
                            'If the control is a combo box(used for domains)
                            If TypeOf cCntrl Is Panel Then

                                For Each cCntrlPnl As Control In cCntrl.Controls
                                    If TypeOf cCntrlPnl Is ComboBox Then
                                        'Get the field
                                        strFld = CType(cCntrlPnl, ComboBox).Tag
                                        'Make sure no link is specified
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If
                                        'If the field is the subtype field
                                        If pSubType.SubtypeFieldName = strFld Then
                                            'Set the value
                                            If pFeat.Value(pFeat.Fields.FindField(strFld)) IsNot DBNull.Value Then

                                                CType(cCntrlPnl, ComboBox).SelectedValue = pFeat.Value(pFeat.Fields.FindField(strFld))
                                            Else
                                                CType(cCntrlPnl, ComboBox).SelectedIndex = 0
                                            End If
                                            'Raise the subtype change event, this loads all the proper domains based on the subtype value
                                            Call cmbSubTypChange_Click(CType(cCntrlPnl, ComboBox), Nothing)

                                            Exit For

                                        End If



                                    End If
                                Next cCntrlPnl
                            End If
                        Next
                    End If

                Next
            End If
            'Loop through all the controls and set their value
            For Each pCntrl As Control In s_tbCntCIPDetails.Controls
                If TypeOf pCntrl Is TabPage Then
                    For Each cCntrl As Control In pCntrl.Controls
                        'If the control is a 2 value domain(Checkboxs)
                        If TypeOf cCntrl Is Panel Then
                            For Each cCntrlPnl As Control In cCntrl.Controls
                                If TypeOf cCntrlPnl Is CustomPanel Then
                                    'Get the Field
                                    strFld = CType(cCntrlPnl, CustomPanel).Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get the target value
                                    Dim pTargetVal As String = ""
                                    If pFeat.Value(pFeat.Fields.FindField(strFld)) IsNot DBNull.Value Then
                                        pTargetVal = pFeat.Value(pFeat.Fields.FindField(strFld))
                                    ElseIf pFeat.Value(pFeat.Fields.FindField(strFld)) Is DBNull.Value Then
                                        If pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue Is DBNull.Value Then
                                            pTargetVal = ""
                                        Else
                                            pTargetVal = pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue 'pFL.Columns(strFld).DefaultValue
                                        End If

                                    ElseIf pFeat.Value(pFeat.Fields.FindField(strFld)) = "" Then
                                        If pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue Is DBNull.Value Then
                                            pTargetVal = ""
                                        Else
                                            pTargetVal = pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue
                                        End If
                                    Else
                                        If pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue Is DBNull.Value Then
                                            pTargetVal = ""
                                        Else
                                            pTargetVal = pFeat.Fields.Field(pFeat.Fields.FindField(strFld)).DefaultValue
                                        End If
                                    End If
                                    Dim pCsPn As CustomPanel = cCntrlPnl
                                    'Loop through the checkboxes to set the proper value

                                    For Each rdCn As Control In pCsPn.Controls
                                        If TypeOf rdCn Is RadioButton Then
                                            If pTargetVal = "" Then
                                                CType(rdCn, RadioButton).Checked = True

                                                Exit For
                                            End If
                                            If rdCn.Tag.ToString = pTargetVal Then
                                                CType(rdCn, RadioButton).Checked = True

                                                Exit For


                                            End If
                                        End If
                                    Next
                                    'If the control is a text box
                                ElseIf TypeOf cCntrlPnl Is TextBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, TextBox).Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Set the Value
                                    If pFeat.Value(pFeat.Fields.FindField(strFld)) IsNot DBNull.Value Then
                                        CType(cCntrlPnl, TextBox).Text = pFeat.Value(pFeat.Fields.FindField(strFld)).ToString
                                    Else
                                        ' CType(cCntrlPnl, TextBox).Text = ""
                                    End If
                                    If strFld = My.Globals.Constants.c_CIPProjectAssetNameField Then
                                        strPrjName = pFeat.Value(pFeat.Fields.FindField(strFld)).ToString
                                    End If
                                    'if the control is a combo box(domain)
                                ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, ComboBox).Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Skip the subtype column
                                    If pSubType.SubtypeFieldName <> strFld Then
                                        'Set the value
                                        If pFeat.Value(pFeat.Fields.FindField(strFld)) IsNot DBNull.Value Then
                                            If pFeat.Value(pFeat.Fields.FindField(strFld)).ToString = "" Or pFeat.Value(pFeat.Fields.FindField(strFld)) Is DBNull.Value Then
                                                '   Dim pCV As CodedValueDomain = CType(cCntrlPnl, ComboBox).DataSource
                                                '   Dim i As Integer = pCV.Rows.Count
                                                'If CType(cCntrlPnl, ComboBox).DataSource IsNot Nothing Then
                                                '    CType(cCntrlPnl, ComboBox).Text = CType(cCntrlPnl, ComboBox).DataSource.Rows(0)("Value")
                                                'End If


                                            Else
                                                CType(cCntrlPnl, ComboBox).SelectedValue = pFeat.Value(pFeat.Fields.FindField(strFld))
                                            End If


                                            'CType(cCntrlPnl, ComboBox).Text = pFeat.Value(pFeat.Fields.FindField(strFld)).ToString
                                        Else
                                            CType(cCntrlPnl, ComboBox).SelectedIndex = 0
                                        End If
                                    End If
                                    'if the contorl is a data time field
                                ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, DateTimePicker).Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get and set the value
                                    If pFeat.Value(pFeat.Fields.FindField(strFld)) IsNot DBNull.Value Then
                                        CType(cCntrlPnl, DateTimePicker).Text = pFeat.Value(pFeat.Fields.FindField(strFld)).ToString
                                        CType(cCntrlPnl, DateTimePicker).Checked = True
                                    Else
                                        CType(cCntrlPnl, DateTimePicker).Checked = False

                                    End If
                                    'If the field is a range domain
                                ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, NumericUpDown).Tag
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get and set the value
                                    If pFeat.Value(pFeat.Fields.FindField(strFld)) Is DBNull.Value Then
                                        CType(cCntrlPnl, NumericUpDown).ReadOnly = True
                                    ElseIf pFeat.Value(pFeat.Fields.FindField(strFld)) > CType(cCntrlPnl, NumericUpDown).Maximum Or _
                                       pFeat.Value(pFeat.Fields.FindField(strFld)) < CType(cCntrlPnl, NumericUpDown).Minimum Then
                                        CType(cCntrlPnl, NumericUpDown).ReadOnly = True

                                    Else
                                        CType(cCntrlPnl, NumericUpDown).Value = pFeat.Value(pFeat.Fields.FindField(strFld)).ToString


                                    End If


                                End If
                            Next

                        End If

                    Next
                End If
            Next
            '    Marshal.ReleaseComObject(pSFilt)

            Return strPrjName

        Catch ex As Exception
            '  MsgBox("Error in the edit control record loader" & vbCrLf & ex.Message)
            Return ""
        Finally
            '   pSFilt = Nothing
            '   Marshal.ReleaseComObject(pFCurs)

            pSubType = Nothing
            pFeat = Nothing

            '   pFCurs = Nothing
        End Try
    End Function

    Friend Shared Sub ResetControls(ByVal DeactivateTools As Boolean)
        Try
            ClearControl()
            RemoveControl(False)
            s_lstInventory.Items.Clear()
            s_btnSavePrj.Enabled = False
            If DeactivateTools Then
                s_btnSelect.Checked = False
                s_btnSelectAssets.Checked = False
                s_btnSketch.Checked = False


            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow:  ResetGrid" & vbCrLf & ex.Message)

        End Try

    End Sub



    Private Shared Sub loadLayersToPanel()
        Try
            s_gpBxCIPCostingLayers.Controls.Clear()
            If My.Globals.Variables.v_CIPTableDef Is Nothing Then
                Return
            End If

            Dim pCurs As ICursor = My.Globals.Variables.v_CIPTableDef.Search(Nothing, True)
            If pCurs Is Nothing Then Return
            Dim pRow As IRow
            pRow = pCurs.NextRow

            Do Until pRow Is Nothing
                Dim pChk As CheckBox = New CheckBox

                pChk.Font = My.Globals.Constants.c_FntSmall
                pChk.AutoSize = True
                pChk.Text = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))
                pChk.Tag = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))
                If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField)) IsNot Nothing And pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField)) IsNot DBNull.Value Then
                    If UCase(pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField))) = "FALSE" Or UCase(pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField))) = "NO" Then
                        pChk.Checked = False
                    Else
                        pChk.Checked = True
                    End If
                Else
                    pChk.Checked = False
                End If
                
                AddHandler pChk.CheckedChanged, AddressOf layerChecked
                s_gpBxCIPCostingLayers.Controls.Add(pChk)

                pRow = pCurs.NextRow
                pChk = Nothing

            Loop


            Marshal.ReleaseComObject(pCurs)
            pCurs = Nothing



            pRow = Nothing


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: LoadLayersToPanel" & vbCrLf & ex.Message)

        End Try

    End Sub
    Friend Shared Sub EnableSavePrj()
        Try
            If s_btnSavePrj Is Nothing Then Return

            If My.Globals.Variables.v_SaveEnabled Then
                If s_dgCIP.Rows.Count > 0 Then
                    s_btnSavePrj.Enabled = True


                Else
                    s_btnSavePrj.Enabled = False
                End If


            Else
                s_btnSavePrj.Enabled = False
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: EnableSavePrj" & vbCrLf & ex.Message)

        End Try
    End Sub

    Private Shared Sub deleteCIPProjects(ByVal prjName As String)
        'Dim pCIPLayerPrj As IFeatureLayer = Nothing
        'Dim pCIPLayerOver As IFeatureLayer = Nothing
        'Dim pCIPLayerPoint As IFeatureLayer = Nothing
        'Dim pCIPLayerPolygon As IFeatureLayer = Nothing
        'Dim pCIPLayerPolyline As IFeatureLayer = Nothing
        'Dim pCIPInvTable As ITable = Nothing

        Dim pFC As IFeatureClass = Nothing
        Dim pTbl As ITable = Nothing
        Dim pQFilt As IQueryFilter = Nothing

        Try



            'pCIPLayerPrj = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectLayName)
            'pCIPLayerOver = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPOverviewLayName)
            'pCIPLayerPoint = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPointLayName)
            'pCIPLayerPolygon = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolygonLayName)
            'pCIPLayerPolyline = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolylineLayName)
            'pCIPInvTable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPInvLayName, my.ArcMap.Document.FocusMap )


            pQFilt = New QueryFilter
            pQFilt.WhereClause = My.Globals.Constants.c_CIPProjectAssetNameField & " = '" & prjName & "'"

            pFC = My.Globals.Variables.v_CIPLayerPrj.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)

            pFC = My.Globals.Variables.v_CIPLayerOver.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)

            pFC = My.Globals.Variables.v_CIPLayerOverPoint.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)

            pFC = My.Globals.Variables.v_CIPLayerPoint.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)

            pFC = My.Globals.Variables.v_CIPLayerPolygon.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)

            pFC = My.Globals.Variables.v_CIPLayerPolyline.FeatureClass
            pTbl = pFC
            pTbl.DeleteSearchedRows(pQFilt)


            'pFC = pCIPInvTable.FeatureClass
            'pTbl = pFC
            'pTbl.DeleteSearchedRows(pQFilt)


            'For i = 0 To 5
            '    Select Case i
            '        Case 0
            '        Case 1
            '        Case 2
            '        Case 3
            '        Case 4
            '        Case 5

            '    End Select
            'Next
            'Dim pDelBuf As IFeatureBuffer
            'Dim pDelCurs As IFeatureCursor = pCIPLayerPrj.FeatureClass.Update(pQFilt, False)
            'Dim pFeat As IFeature

            'Do Until pDelCurs.NextFeature Is Nothing
            '    pDelCurs.DeleteFeature()

            'Loop

        Catch ex As Exception
        Finally
            'pCIPLayerPrj = Nothing
            'pCIPLayerOver = Nothing
            'pCIPLayerPoint = Nothing
            'pCIPLayerPolygon = Nothing
            'pCIPLayerPolyline = Nothing
            'pCIPInvTable = Nothing

            pFC = Nothing
            pTbl = Nothing
            pQFilt = Nothing
        End Try

    End Sub


    Private Shared Sub CreateCIPProject() 'ByVal strPrjName As String, ByVal strPrjCost As String, ByVal StartDate As Date, ByVal EndDate As Date, ByVal CIPStat As String, ByVal CIPStim As String, ByVal strEng As String, ByVal strManager As String, ByVal strNotes As String)
        'Dim pCIPLayerPrj As IFeatureLayer = Nothing
        'Dim pCIPLayerOver As IFeatureLayer = Nothing
        'Dim pCIPLayerPoint As IFeatureLayer = Nothing
        'Dim pCIPLayerPolygon As IFeatureLayer = Nothing
        'Dim pCIPLayerPolyline As IFeatureLayer = Nothing
        ''  Dim pCIPInvTable As ITable = Nothing


        Try




            'pCIPLayerPrj = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectLayName)
            'pCIPLayerOver = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPOverviewLayName)
            'pCIPLayerPoint = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPointLayName)
            'pCIPLayerPolygon = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolygonLayName)
            'pCIPLayerPolyline = My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolylineLayName)
            '  pCIPInvTable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPInvLayName, my.ArcMap.Document.FocusMap )


            If My.Globals.Variables.v_CIPLayerOver Is Nothing Or My.Globals.Variables.v_CIPLayerOverPoint Is Nothing Or My.Globals.Variables.v_CIPLayerPrj Is Nothing Or My.Globals.Variables.v_CIPLayerPoint Is Nothing _
            Or My.Globals.Variables.v_CIPLayerPolyline Is Nothing Or My.Globals.Variables.v_CIPLayerPolygon Is Nothing Then 'Or pCIPInvTable Is Nothing Then
                MsgBox("The CIP Project layer is not in the geodatabase being edited, exiting")
                Return

            End If


        Catch ex As Exception

        End Try
        Dim pProDlg As IProgressDialog2 = Nothing
        Try
            Dim pOverGeo As IGeometry = New Polygon
            Dim pPrjGeo As IGeometry = New Polygon
            Dim pTopo As ITopologicalOperator = Nothing
            Dim pPntCnt As Integer = 0
            Dim pTotArea As Double

            Dim pProDlgFact As IProgressDialogFactory
            Dim pStepPro As IStepProgressor

            Dim pTrkCan As ITrackCancel
            If My.Globals.Variables.v_Editor.EditState = esriEditState.esriStateNotEditing Then
                MsgBox("Please Start editing")
                Return

            End If

            ' Create a CancelTracker  
            pTrkCan = New CancelTracker
            ' Create the ProgressDialog. This automatically displays the dialog  

            pProDlgFact = New ProgressDialogFactory
            pProDlg = pProDlgFact.Create(pTrkCan, 0)
            ' Set the properties of the ProgressDialog  
            pProDlg.CancelEnabled = True
            pProDlg.Description = "Creating CIP Project"
            pProDlg.Title = "Processing..."
            pProDlg.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral
            ' Set the properties of the Step Progressor  
            pStepPro = pProDlg
            pStepPro.MinRange = 0
            pStepPro.MaxRange = s_dgCIP.RowCount + 1
            pStepPro.StepValue = 1
            pStepPro.Message = "Loading Candidates to CIP Project "
            ' Step. Do your big process here.  
            Dim boolCont As Boolean = True


            pProDlg.ShowDialog()

            Dim pCIPFeat As IFeature = My.Globals.Variables.v_CIPLayerPrj.FeatureClass.CreateFeature
            Dim pCIPOverFeat As IFeature = My.Globals.Variables.v_CIPLayerOver.FeatureClass.CreateFeature
            Dim pCIPOverPointFeat As IFeature = My.Globals.Variables.v_CIPLayerOverPoint.FeatureClass.CreateFeature
            pStepPro.Message = "Getting Project info"
            Dim pPrjName As String = ""
            My.Globals.Variables.v_Editor.StartOperation()

            For Each tbpg As TabPage In s_tbCntCIPDetails.TabPages
                For Each pnlCnt As Control In tbpg.Controls
                    If TypeOf pnlCnt Is Panel Then
                        For Each inCnt As Control In pnlCnt.Controls
                            If TypeOf inCnt Is TextBox Then
                                If inCnt.Tag = My.Globals.Constants.c_CIPProjectAssetNameField Then
                                    pPrjName = inCnt.Text
                                    If pPrjName = "" Then
                                        MsgBox("Please provide a project name")
                                        My.Globals.Variables.v_Editor.AbortOperation()

                                        Return

                                    End If
                                End If
                                If pCIPFeat.Fields.FindField(inCnt.Tag) > 0 Then
                                    pCIPFeat.Value(pCIPFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverPointFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If

                            ElseIf TypeOf inCnt Is DateTimePicker Then


                                If pCIPFeat.Fields.FindField(inCnt.Tag) > 0 Then
                                    pCIPFeat.Value(pCIPFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverPointFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                            ElseIf TypeOf inCnt Is ComboBox Then

                                If pCIPFeat.Fields.FindField(inCnt.Tag) > 0 Then
                                    pCIPFeat.Value(pCIPFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverPointFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                            ElseIf TypeOf inCnt Is NumericUpDown Then

                                If pCIPFeat.Fields.FindField(inCnt.Tag) > 0 Then
                                    pCIPFeat.Value(pCIPFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                                If pCIPOverPointFeat.Fields.FindField(inCnt.Tag) > 0 Then

                                    pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(inCnt.Tag)) = inCnt.Text
                                End If
                            End If

                        Next
                    ElseIf TypeOf pnlCnt Is CustomPanel Then
                        If CType(pnlCnt.Controls(0), RadioButton).Checked = True Then

                            If pCIPFeat.Fields.FindField(pnlCnt.Tag) > 0 Then
                                pCIPFeat.Value(pCIPFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If
                            If pCIPOverFeat.Fields.FindField(pnlCnt.Tag) > 0 Then

                                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If

                            If pCIPOverPointFeat.Fields.FindField(pnlCnt.Tag) > 0 Then

                                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If
                        Else

                            If pCIPFeat.Fields.FindField(pnlCnt.Tag) > 0 Then
                                pCIPFeat.Value(pCIPFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If
                            If pCIPOverFeat.Fields.FindField(pnlCnt.Tag) > 0 Then

                                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If
                            If pCIPOverPointFeat.Fields.FindField(pnlCnt.Tag) > 0 Then

                                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(pnlCnt.Tag)) = CType(pnlCnt.Controls(0), RadioButton).Tag
                            End If
                        End If
                    End If
                Next
            Next

            If pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField) > 0 Then
                pCIPFeat.Value(pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField)) = s_lblTotLength.Text
            End If
            If pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField) > 0 Then
                pCIPFeat.Value(pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField)) = pTotArea
            End If
            If pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField) > 0 Then
                pCIPFeat.Value(pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField)) = pPntCnt
            End If
            
            If pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField) > 0 Then
                pCIPFeat.Value(pCIPFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField)) = s_lblTotalCost.Text
            End If
            If pCIPFeat.Fields.FindField("LASTUPDATE") > 0 Then
                pCIPFeat.Value(pCIPOverFeat.Fields.FindField("LASTUPDATE")) = Now 'FormatDateTime(Now, DateFormat.LongDate)
            End If
            If pCIPFeat.Fields.FindField("LASTEDITOR") > 0 Then
                pCIPFeat.Value(pCIPOverFeat.Fields.FindField("LASTEDITOR")) = SystemInformation.UserName
            End If

            If pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField) > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField)) = pTotArea
            End If
            If pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField) > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField)) = pPntCnt
            End If
            If pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField) > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField)) = s_lblTotLength.Text
            End If
            If pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField) > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField)) = s_lblTotalCost.Text
            End If
            If pCIPOverFeat.Fields.FindField("LASTUPDATE") > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField("LASTUPDATE")) = Now 'FormatDateTime(Now, DateFormat.LongDate)
            End If
            If pCIPOverFeat.Fields.FindField("LASTEDITOR") > 0 Then
                pCIPOverFeat.Value(pCIPOverFeat.Fields.FindField("LASTEDITOR")) = SystemInformation.UserName
            End If



            If pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField) > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotAreaField)) = pTotArea
            End If
            If pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField) > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotPntField)) = pPntCnt
            End If
            If pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField) > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayTotLenField)) = s_lblTotLength.Text
            End If
            If pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField) > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCostField)) = s_lblTotalCost.Text
            End If
            If pCIPOverPointFeat.Fields.FindField("LASTUPDATE") > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField("LASTUPDATE")) = Now 'FormatDateTime(Now, DateFormat.LongDate)
            End If
            If pCIPOverPointFeat.Fields.FindField("LASTEDITOR") > 0 Then
                pCIPOverPointFeat.Value(pCIPOverPointFeat.Fields.FindField("LASTEDITOR")) = SystemInformation.UserName
            End If


            pStepPro.Message = "Checking Project Name"
            If pPrjName = "" Then
                MsgBox("The CIP Project name is was not found or is invalid")
                My.Globals.Variables.v_Editor.AbortOperation()
                Return

            End If

            Try
                Dim pQFilt As IQueryFilter = New QueryFilter
                pQFilt.WhereClause = My.Globals.Constants.c_CIPProjectLayNameField & " = '" & pPrjName & "'"
                If My.Globals.Variables.v_CIPLayerPrj.FeatureClass.FeatureCount(pQFilt) <> 0 Then
                    If MsgBox("The CIP Project name is already in use, Override project?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        deleteCIPProjects(pPrjName)
                    Else
                        pQFilt = Nothing
                        My.Globals.Variables.v_Editor.AbortOperation()
                        Return

                    End If

                End If
            Catch ex As Exception
                MsgBox("Error trying to check Project Names - Make sure the overview layer has a field named: " & My.Globals.Constants.c_CIPProjectAssetNameField)
                My.Globals.Variables.v_Editor.AbortOperation()
                Return
            End Try
            Dim pGeometryDefTest As IGeometryDef
            Dim pFieldsTest As IFields
            Dim lGeomIndex As Integer
            Dim pFieldTest As IField
            Dim bZAware As Boolean
            Dim bMAware As Boolean
            Dim pZAware As IZAware

            pStepPro.Message = "Costing Candidates"
            For Each pDataRow As DataGridViewRow In s_dgCIP.Rows
                Dim strTag As String

                pStepPro.Message = "Loading Candidates to CIP Project " & pDataRow.Cells(0).Value & ":" & pDataRow.Cells("OID").Value

                strTag = "CIPTools:" & pDataRow.Cells("ASSETTYP").Value & ":" & pDataRow.Cells("OID").Value

                Dim pGeo As IGeometry = My.Globals.Functions.GetShapeFromGraphic(strTag, "CIPTools:")

                Dim pTmpGeo As IGeometry
                Dim pNewFeat As IFeature

                'Dim pGeometryDefTest As IGeometryDef
                'Dim pFieldsTest As IFields
                'Dim lGeomIndex As Integer
                'Dim pFieldTest As IField
                'Dim bZAware As Boolean
                'Dim bMAware As Boolean

                Dim pDS As IGeoDataset = My.Globals.Variables.v_CIPLayerPolygon

                pGeo.Project(pDS.SpatialReference)

                Select Case pGeo.GeometryType
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                        pTmpGeo = pGeo

                        pNewFeat = My.Globals.Variables.v_CIPLayerPolygon.FeatureClass.CreateFeature
                        pTotArea = pTotArea + CType(pNewFeat.Shape, IArea).Area
                        pFieldsTest = My.Globals.Variables.v_CIPLayerPolygon.FeatureClass.Fields
                        lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerPolygon.FeatureClass.ShapeFieldName)

                        pFieldTest = pFieldsTest.Field(lGeomIndex)
                        pGeometryDefTest = pFieldTest.GeometryDef
                        '    Determine if M or Z aware
                        bZAware = pGeometryDefTest.HasZ
                        pZAware = pGeo

                        If bZAware = False Then
                            ''Dim pGeoNew As IGeometry = New Polyline

                            If (pZAware.ZAware) Then
                                'pZAware.DropZs()
                                pZAware.ZAware = False
                            Else

                            End If

                        Else

                            If pZAware.ZAware = False Then
                                pZAware.ZAware = True
                                Dim pZ As IZ = pGeo
                                pZ.SetConstantZ(0)

                            End If

                        End If

                        bMAware = pGeometryDefTest.HasM
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                        pTopo = pGeo
                        pTmpGeo = pTopo.Buffer(m_BufferAmount)
                        pNewFeat = My.Globals.Variables.v_CIPLayerPolyline.FeatureClass.CreateFeature
                        pFieldsTest = My.Globals.Variables.v_CIPLayerPolyline.FeatureClass.Fields
                        lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerPolyline.FeatureClass.ShapeFieldName)

                        pFieldTest = pFieldsTest.Field(lGeomIndex)
                        pGeometryDefTest = pFieldTest.GeometryDef
                        '    Determine if M or Z aware
                        bZAware = pGeometryDefTest.HasZ
                        pZAware = pGeo

                        If bZAware = False Then
                            ''Dim pGeoNew As IGeometry = New Polyline

                            If (pZAware.ZAware) Then
                                'pZAware.DropZs()
                                pZAware.ZAware = False
                            Else

                            End If

                        Else

                            If pZAware.ZAware = True Then
                                'pZAware.ZAware = True
                                Dim pZ As IZ = pGeo
                                ' pZ.CalculateNonSimpleZs()
                                ' pZ.SetConstantZ(0)
                            Else
                                pZAware.ZAware = True
                                Dim pZ As IZ = pGeo
                                pZ.SetConstantZ(0)
                            End If

                        End If



                        bMAware = pGeometryDefTest.HasM
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                        pTopo = pGeo
                        pTmpGeo = pTopo.Buffer(m_BufferAmount)
                        pNewFeat = My.Globals.Variables.v_CIPLayerPoint.FeatureClass.CreateFeature
                        pPntCnt = pPntCnt + 1


                        pFieldsTest = My.Globals.Variables.v_CIPLayerPoint.FeatureClass.Fields
                        lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerPoint.FeatureClass.ShapeFieldName)

                        pFieldTest = pFieldsTest.Field(lGeomIndex)
                        pGeometryDefTest = pFieldTest.GeometryDef
                        '    Determine if M or Z aware
                        bZAware = pGeometryDefTest.HasZ
                        pZAware = pGeo

                        If bZAware = False Then
                            ''Dim pGeoNew As IGeometry = New Polyline

                            If (pZAware.ZAware) Then
                                'pZAware.DropZs()
                                pZAware.ZAware = False
                            Else

                            End If

                        Else

                            If pZAware.ZAware = False Then
                                pZAware.ZAware = True
                                ' Dim pZ As IZ = pGeo
                                'pZ.SetConstantZ(0)
                                Dim pntZ As IPoint = pGeo
                                pntZ.Z = 0

                            End If

                        End If

                        bMAware = pGeometryDefTest.HasM

                    Case Else
                        Continue For

                End Select


                If pOverGeo Is Nothing Or pOverGeo.IsEmpty Then
                    pOverGeo = pTmpGeo


                Else
                    pTopo = pOverGeo
                    pOverGeo = pTopo.Union(pTmpGeo)


                End If
                pTmpGeo = Nothing


                pNewFeat.Shape = pGeo

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNameField)) = pPrjName
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTypeField)) = pDataRow.Cells("ASSETTYP").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetIDField)) = pDataRow.Cells("ASSETID").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetCostField)) = pDataRow.Cells("COST").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetAddCostField)) = pDataRow.Cells("ADDCOST").Value

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetTotCostField)) = pDataRow.Cells("TOTCOST").Value

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt1Field)) = pDataRow.Cells("exFiltVal1").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetExistingFilt2Field)) = pDataRow.Cells("exFiltVal2").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt1Field)) = pDataRow.Cells("proFiltVal1").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetProposedFilt2Field)) = pDataRow.Cells("proFiltVal2").Value

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetOIDField)) = pDataRow.Cells("OID").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetNotesField)) = pDataRow.Cells("Notes").Value

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetMultiField)) = pDataRow.Cells("Multi").Value
                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetStrategyField)) = pDataRow.Cells("Strategy").Value

                pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetActionField)) = pDataRow.Cells("Action").Value

                If pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField) > 0 Then
                    pNewFeat.Value(pNewFeat.Fields.FindField(My.Globals.Constants.c_CIPProjectAssetLenField)) = pDataRow.Cells("LENGTH").Value()

                End If
                If pNewFeat.Fields.FindField("LASTUPDATE") > 0 Then

                    pNewFeat.Value(pNewFeat.Fields.FindField("LASTUPDATE")) = Now ' FormatDateTime(Now, DateFormat.LongDate)

                End If
                If pNewFeat.Fields.FindField("LASTEDITOR") > 0 Then

                    pNewFeat.Value(pNewFeat.Fields.FindField("LASTEDITOR")) = SystemInformation.UserName

                End If

                pNewFeat.Store()


                boolCont = pTrkCan.Continue
                If Not boolCont Then

                    My.Globals.Variables.v_Editor.AbortOperation()
                    Exit Try
                End If
                pGeo = Nothing

                pTmpGeo = Nothing
                pNewFeat = Nothing
                pStepPro.Step()
            Next

            If pOverGeo IsNot Nothing And pOverGeo.IsEmpty = False Then
                pStepPro.Message = "Creating Project Extents"

                Dim pClone As IClone = pOverGeo
                Dim pPoly As IPolygon = pClone.Clone
                Dim pOverTopo As ITopologicalOperator = pPoly
                Dim pUnitConverter As IUnitConverter = New UnitConverter
                Dim pGeoDs As IGeoDataset = My.Globals.Variables.v_CIPLayerOver
                Dim pBufAmount As Double

                If TypeOf pGeoDs.SpatialReference Is IProjectedCoordinateSystem Then
                    Dim pPrjCoord As IProjectedCoordinateSystem

                    pPrjCoord = pGeoDs.SpatialReference

                    pBufAmount = pUnitConverter.ConvertUnits(m_BufferAmount, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, My.Globals.Functions.ConvertUnitType(pPrjCoord.CoordinateUnit))
                    pPrjCoord = Nothing

                Else
                    Dim pGeoCoord As IGeographicCoordinateSystem

                    pGeoCoord = pGeoDs.SpatialReference

                    pBufAmount = pUnitConverter.ConvertUnits(m_BufferAmount, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, My.Globals.Functions.ConvertUnitType(pGeoCoord.CoordinateUnit))
                    pGeoCoord = Nothing

                End If
                pGeoDs = Nothing


                pOverTopo = pOverTopo.ConvexHull()
                pOverTopo = pOverTopo.Buffer(m_BufferAmountConvext)



                pFieldsTest = My.Globals.Variables.v_CIPLayerOver.FeatureClass.Fields
                lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerOver.FeatureClass.ShapeFieldName)

                pFieldTest = pFieldsTest.Field(lGeomIndex)
                pGeometryDefTest = pFieldTest.GeometryDef
                '    Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ
                pZAware = pOverTopo

                If bZAware = False Then
                    ''Dim pGeoNew As IGeometry = New Polyline

                    If (pZAware.ZAware) Then
                        'pZAware.DropZs()
                        pZAware.ZAware = False
                    Else

                    End If

                Else

                    If pZAware.ZAware = True Then
                        'pZAware.ZAware = True
                        Dim pZ As IZ = pOverTopo
                        ' pZ.CalculateNonSimpleZs()
                        ' pZ.SetConstantZ(0)
                    Else
                        pZAware.ZAware = True
                        Dim pZ As IZ = pOverTopo
                        pZ.SetConstantZ(0)
                    End If

                End If



             





                pFieldsTest = My.Globals.Variables.v_CIPLayerPrj.FeatureClass.Fields
                lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerPrj.FeatureClass.ShapeFieldName)

                pFieldTest = pFieldsTest.Field(lGeomIndex)
                pGeometryDefTest = pFieldTest.GeometryDef
                '    Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ
                pZAware = pOverGeo

                If bZAware = False Then
                    ''Dim pGeoNew As IGeometry = New Polyline

                    If (pZAware.ZAware) Then
                        'pZAware.DropZs()
                        pZAware.ZAware = False
                    Else

                    End If

                Else

                    If pZAware.ZAware = True Then
                        'pZAware.ZAware = True
                        Dim pZ As IZ = pOverGeo
                        ' pZ.CalculateNonSimpleZs()
                        ' pZ.SetConstantZ(0)
                    Else
                        pZAware.ZAware = True
                        Dim pZ As IZ = pOverGeo
                        pZ.SetConstantZ(0)
                    End If

                End If


                Dim pOverPnt As IGeometry = CType(pOverTopo, IArea).Centroid


                pFieldsTest = My.Globals.Variables.v_CIPLayerOverPoint.FeatureClass.Fields
                lGeomIndex = pFieldsTest.FindField(My.Globals.Variables.v_CIPLayerOverPoint.FeatureClass.ShapeFieldName)

                pFieldTest = pFieldsTest.Field(lGeomIndex)
                pGeometryDefTest = pFieldTest.GeometryDef
                '    Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ
                pZAware = pOverPnt

                If bZAware = False Then
                    ''Dim pGeoNew As IGeometry = New Polyline

                    If (pZAware.ZAware) Then
                        'pZAware.DropZs()
                        pZAware.ZAware = False
                    Else

                    End If

                Else

                    If pZAware.ZAware = True Then
                        'pZAware.ZAware = True
                        Dim pZ As IZ = pOverPnt
                        ' pZ.CalculateNonSimpleZs()
                        ' pZ.SetConstantZ(0)
                    Else
                        Try

                            pZAware.ZAware = True
                            Dim pntZ As IPoint = pOverPnt
                            pntZ.Z = 0

                            ' Dim pZ As IZ = pOverPnt
                            '    pZ.SetConstantZ(0)
                        Catch ex As Exception

                        End Try

                    End If

                End If




                
                pCIPFeat.Shape = pOverGeo
                pCIPOverFeat.Shape = pOverTopo 'pPoly 'pgoncoll
                pCIPOverPointFeat.Shape = pOverPnt 'CType(pOverTopo, IArea).Centroid 'pPoly 'pgoncoll
                pCIPFeat.Store()
                pCIPOverFeat.Store()
                pCIPOverPointFeat.Store()

                pClone = Nothing

                pCIPFeat = Nothing
                pCIPOverFeat = Nothing
                pCIPOverPointFeat = Nothing
                pUnitConverter = Nothing
                pPoly = Nothing

            End If
            'Try


            '    If lstInventory.Items.Count > 0 Then
            '        For Each lstItem As String In lstInventory.Items
            '            Dim pRow As IRow = pCIPInvTable.CreateRow
            '            pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPInvLayProjNameField)) = pPrjName
            '            Dim pVals() As String = lstItem.Split(":")
            '            pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPInvLayInvTypefield)) = Trim(pVals(0))
            '            pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPInvLayNumOfInvField)) = Trim(pVals(1))
            '            pRow.Store()
            '            pRow = Nothing
            '        Next
            '    End If
            'Catch ex As Exception
            '    MsgBox("Error saving inventory, project will save")
            'End Try
            My.Globals.Variables.v_Editor.StopOperation("CIP Project Created")

            ClearControl()
            pStepPro.Step()




            pOverGeo = Nothing
            pPrjGeo = Nothing
            pTopo = Nothing
            pProDlgFact = Nothing
            pStepPro = Nothing

            pTrkCan = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: CreateCIPProject" & vbCrLf & ex.Message)
            My.Globals.Variables.v_Editor.AbortOperation()

        Finally
            If pProDlg IsNot Nothing Then

                pProDlg.HideDialog()
                Marshal.ReleaseComObject(pProDlg)
                pProDlg = Nothing
            End If
        End Try


        My.ArcMap.Document.ActiveView.Refresh()
        'pCIPLayerPrj = Nothing
        'pCIPLayerOver = Nothing
        'pCIPLayerPoint = Nothing
        'pCIPLayerPolygon = Nothing
        'pCIPLayerPolyline = Nothing
        'pCIPInvTable = Nothing

    End Sub

    Private Shared Function createSimpleRenderer(ByVal type As ESRI.ArcGIS.Geometry.esriGeometryType) As ISimpleRenderer

        Try




            Dim pSimpleRenderer As ISimpleRenderer = New SimpleRenderer
            Dim pColor As IColor



            pColor = New RgbColor
            pColor.RGB = RGB(255, 25, 25)

            Select Case type
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    ' create a new fill symbol
                    Dim pFillSymbol As ISimpleFillSymbol = New SimpleFillSymbol
                    pFillSymbol = New SimpleFillSymbol

                    pFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSHollow

                    ' set the color of the fill symbol
                    pFillSymbol.Color = pColor

                    Dim pLineSymbol As ISimpleLineSymbol
                    pLineSymbol = New SimpleLineSymbol
                    pLineSymbol.Width = 2

                    pLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
                    ' set the color of the fill symbol
                    pLineSymbol.Color = pColor
                    pFillSymbol.Outline = pLineSymbol

                    ' set the renderer's symbol, label, and description 
                    pSimpleRenderer.Symbol = pFillSymbol
                    pLineSymbol = Nothing
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    ' create a new fill symbol

                    Dim pFillSymbol As ISimpleLineSymbol
                    pFillSymbol = New SimpleLineSymbol

                    pFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
                    ' set the color of the fill symbol
                    pFillSymbol.Color = pColor
                    pFillSymbol.Width = 2
                    ' set the renderer's symbol, label, and description 
                    pSimpleRenderer.Symbol = pFillSymbol
                    pFillSymbol = Nothing
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    ' create a new fill symbol

                    Dim pFillSymbol As ISimpleMarkerSymbol
                    pFillSymbol = New SimpleMarkerSymbol

                    pFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle
                    ' set the color of the fill symbol
                    pFillSymbol.Color = pColor
                    ' set the renderer's symbol, label, and description 
                    pSimpleRenderer.Symbol = pFillSymbol
                    pFillSymbol = Nothing
                Case Else

            End Select

            pSimpleRenderer.Label = "CIP"
            pSimpleRenderer.Description = ""

            pSimpleRenderer = Nothing
            pColor = Nothing



            Return pSimpleRenderer
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: createSimpleRenderer" & vbCrLf & ex.Message)
            Return Nothing

        End Try
    End Function
    Private Shared Sub createGraphicSymbols()

        Try


            My.Globals.Variables.v_PolygonGraphicSymbol = New SimpleFillSymbol
            My.Globals.Variables.v_PolygonHighlightGraphicSymbol = New SimpleFillSymbol

            My.Globals.Variables.v_LineGraphicSymbol = New MultiLayerLineSymbol
            My.Globals.Variables.v_LineHighlightGraphicSymbol = New SimpleLineSymbol

            My.Globals.Variables.v_PointGraphicSymbol = New MultiLayerMarkerSymbol
            My.Globals.Variables.v_PointHighlightGraphicSymbol = New SimpleMarkerSymbol



            Dim pSelectColor As IColor
            Dim pColor As IColor
            Dim pOutColor As IColor
            pColor = New RgbColor
            pColor.RGB = My.Globals.Constants.c_DrawColor

            pOutColor = New RgbColor
            pOutColor.RGB = My.Globals.Constants.c_OutDrawColor

            pSelectColor = New RgbColor
            pSelectColor.RGB = My.Globals.Constants.c_HighlightColor


            My.Globals.Variables.v_PolygonGraphicSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSHollow
            My.Globals.Variables.v_PolygonHighlightGraphicSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSHollow
            ' set the color of the fill symbol
            My.Globals.Variables.v_PolygonGraphicSymbol.Color = pColor
            My.Globals.Variables.v_PolygonHighlightGraphicSymbol.Color = pSelectColor

            Dim pLineSymbol As ISimpleLineSymbol
            pLineSymbol = New SimpleLineSymbol
            pLineSymbol.Width = 3

            pLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
            ' set the color of the fill symbol
            pLineSymbol.Color = pColor
            My.Globals.Variables.v_PolygonGraphicSymbol.Outline = pLineSymbol

            pLineSymbol.Color = pSelectColor
            My.Globals.Variables.v_PolygonHighlightGraphicSymbol.Outline = pLineSymbol


            Dim pLineSym As ISimpleLineSymbol = New SimpleLineSymbol

            pLineSym.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
            ' set the color of the fill symbol
            pLineSym.Color = pOutColor
            pLineSym.Width = 4

            Dim pOutLineSym As ISimpleLineSymbol = New SimpleLineSymbol

            pOutLineSym.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
            ' set the color of the fill symbol
            pOutLineSym.Color = pColor
            pOutLineSym.Width = 6
            My.Globals.Variables.v_LineGraphicSymbol.AddLayer(pOutLineSym)

            My.Globals.Variables.v_LineGraphicSymbol.AddLayer(pLineSym)

            My.Globals.Variables.v_LineHighlightGraphicSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid
            ' set the color of the fill symbol
            My.Globals.Variables.v_LineHighlightGraphicSymbol.Color = pSelectColor
            My.Globals.Variables.v_LineHighlightGraphicSymbol.Width = 6

            Dim pOutMarkSym As IMarkerSymbol = New SimpleMarkerSymbol
            Dim pMarkSym As IMarkerSymbol = New SimpleMarkerSymbol

            ' pMarkSym.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle
            ' set the color of the fill symbol
            pMarkSym.Color = pColor
            pOutMarkSym.Size = 4

            'pOutMarkSym.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle
            ' set the color of the fill symbol
            pOutMarkSym.Color = pOutColor
            pOutMarkSym.Size = 6
            My.Globals.Variables.v_PointGraphicSymbol.AddLayer(pMarkSym)

            My.Globals.Variables.v_PointGraphicSymbol.AddLayer(pOutMarkSym)

            pOutMarkSym = Nothing
            pMarkSym = Nothing

            My.Globals.Variables.v_PointHighlightGraphicSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle
            ' set the color of the fill symbol
            My.Globals.Variables.v_PointHighlightGraphicSymbol.Color = pSelectColor

            pOutMarkSym = Nothing
            pMarkSym = Nothing
            pOutLineSym = Nothing
            pLineSym = Nothing
            pSelectColor = Nothing
            pColor = Nothing
            pLineSymbol = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: createGraphicSymbols" & vbCrLf & ex.Message)


        End Try
    End Sub

    Private Shared Sub ClearControl()
        Try
            If s_enabled = False Then Return
            If s_tbCntCIPDetails Is Nothing Then Return

            For Each tbpg As TabPage In s_tbCntCIPDetails.TabPages
                For Each pnlCnt As Control In tbpg.Controls
                    If TypeOf pnlCnt Is Panel Then
                        For Each inCnt As Control In pnlCnt.Controls
                            If TypeOf inCnt Is TextBox Then
                                inCnt.Text = ""
                            ElseIf TypeOf inCnt Is DateTimePicker Then



                            ElseIf TypeOf inCnt Is ComboBox Then


                            ElseIf TypeOf inCnt Is NumericUpDown Then


                            End If

                        Next
                    ElseIf TypeOf pnlCnt Is CustomPanel Then
                        If CType(pnlCnt.Controls(0), RadioButton).Checked = True Then


                        Else


                        End If
                    End If
                Next
            Next

            My.Globals.Variables.v_LastSelection = ""
            UnSelectRow(My.Globals.Variables.v_LastSelection)

            RemoveHandler s_dgCIP.SelectionChanged, AddressOf dgCIP_SelectionChanged

            s_dgCIP.Rows.Clear()
            AddHandler s_dgCIP.SelectionChanged, AddressOf dgCIP_SelectionChanged
            s_lstInventory.Items.Clear()
            s_lblTotLength.Text = ".00"
            s_lblTotArea.Text = ".00"
            s_lblTotPnt.Text = "0"
            s_lblTotalCost.Text = FormatCurrency("0.00", 2, TriState.True, TriState.True) 'Format(Total, "#,###.00")

            s_lblTotalCost.Parent.Refresh()
            Cleargraphics()
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: ClearControl" & vbCrLf & ex.Message)


        End Try
    End Sub
    Private Shared Sub makeImagesTrans()
        Try
            For Each cnt As Control In s_gpBxControls.Controls
                If TypeOf cnt Is System.Windows.Forms.Button Then


                    Dim img As System.Drawing.Bitmap

                    img = CType(cnt, System.Windows.Forms.Button).Image
                    If img IsNot Nothing Then
                        img.MakeTransparent(img.GetPixel(0, 0))

                        CType(cnt, System.Windows.Forms.Button).Image = img

                    End If
                    img = Nothing

                ElseIf TypeOf cnt Is System.Windows.Forms.CheckBox Then


                    Dim img As System.Drawing.Bitmap

                    img = CType(cnt, System.Windows.Forms.CheckBox).Image
                    If img IsNot Nothing Then
                        img.MakeTransparent(img.GetPixel(0, 0))

                        CType(cnt, System.Windows.Forms.CheckBox).Image = img

                    End If
                    img = Nothing
                End If
            Next


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: makeImagesTrans" & vbCrLf & ex.Message)
        End Try
    End Sub

    Friend Shared Sub RemoveControl(ByVal Save As Boolean)
        Try

            For Each cnt As Control In s_dgCIP.Controls
                If TypeOf cnt Is TextBox Then
                    If Save Then
                        SaveTextBox(cnt)
                    End If
                    s_dgCIP.Controls.Remove(cnt)
                ElseIf TypeOf cnt Is ComboBox Then
                    s_dgCIP.Controls.Remove(cnt)
                End If
            Next

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: RemoveControl" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub addTextBox(ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Try
            Dim Rectangle As System.Drawing.Rectangle = s_dgCIP.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)

            Dim pTxtBox As TextBox = New TextBox

            pTxtBox.Left = Rectangle.Left
            pTxtBox.Top = Rectangle.Top
            pTxtBox.Width = Rectangle.Width
            pTxtBox.Tag = e.ColumnIndex & "|" & e.RowIndex
            pTxtBox.Height = Rectangle.Height - 20
            pTxtBox.Text = s_dgCIP.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

            s_dgCIP.Controls.Add(pTxtBox)

            pTxtBox.Focus()

            pTxtBox.Show()
            Rectangle = Nothing
            pTxtBox = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: addTextBox" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub addComboBox(ByVal e As System.Windows.Forms.DataGridViewCellEventArgs, ByVal ListValues As ArrayList)

        Try
            Dim Rectangle As System.Drawing.Rectangle = s_dgCIP.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
            Dim pCboBox As System.Windows.Forms.ComboBox = New System.Windows.Forms.ComboBox

            pCboBox.DropDownStyle = Windows.Forms.ComboBoxStyle.DropDownList

            pCboBox.Left = Rectangle.Left
            pCboBox.Top = Rectangle.Top
            pCboBox.Width = Rectangle.Width
            pCboBox.DataSource = ListValues

            pCboBox.DisplayMember = "Display"
            pCboBox.ValueMember = "Value"
            pCboBox.Tag = e.ColumnIndex & "|" & e.RowIndex
            pCboBox.Height = Rectangle.Height - 20
            pCboBox.DropDownWidth = 300
            s_dgCIP.Controls.Add(pCboBox)


            pCboBox.Show()
            AddHandler pCboBox.SelectedValueChanged, AddressOf comboClick
            Rectangle = Nothing
            pCboBox.DroppedDown = True

            pCboBox = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: addComboBox" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub saveCombo(ByVal pCboBox As System.Windows.Forms.ComboBox)

        Try

            Dim pRowCol() As String = pCboBox.Tag.ToString.Split("|")

            If s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = pCboBox.Text Then
                pCboBox.Parent.Controls.Remove(pCboBox)
                Return

            End If


            '
            ' MsgBox("Translate display values to code values")
            s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = pCboBox.Text
            'Dim pExistingDis1 As String = ""
            'Dim pExistingDis2 As String = ""
            Dim pExistingVal1 As String = ""
            Dim pExistingVal2 As String = ""

            Dim pReplaceDis1 As String = ""
            Dim pReplaceDis2 As String = ""

            Dim pReplaceVal1 As String = ""
            Dim pReplaceVal2 As String = ""

            pReplaceDis1 = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value
            pReplaceDis2 = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value
            pReplaceVal1 = pReplaceDis1

            pReplaceVal2 = pReplaceDis2

            Dim pAssetLay As IFeatureLayer = My.Globals.Functions.FindLayer(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value)
            Dim pDomFilt1 As IDomain = Nothing
            If s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld1").Value <> "" Then
                If pAssetLay.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld1").Value) > 0 Then

                    pDomFilt1 = pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld1").Value)).Domain
                    If TypeOf pDomFilt1 Is ICodedValueDomain Then
                        pReplaceVal1 = My.Globals.Functions.GetDomainValue(pReplaceDis1, pDomFilt1)
                        pExistingVal1 = My.Globals.Functions.GetDomainValue(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal1").Value, pDomFilt1)
                    End If
                End If
            End If

            Dim pDomFilt2 As IDomain = Nothing
            If s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld2").Value <> "" Then
                If pAssetLay.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld2").Value) > 0 Then

                    pDomFilt2 = pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld2").Value)).Domain
                    If TypeOf pDomFilt2 Is ICodedValueDomain Then
                        pReplaceVal2 = My.Globals.Functions.GetDomainValue(pReplaceDis2, pDomFilt2)
                        pExistingVal2 = My.Globals.Functions.GetDomainValue(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal2").Value, pDomFilt2)
                    End If

                End If
            End If

            Dim pSubVal As String = My.Globals.Functions.GetSubtypeValue(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("STRATEGY").FormattedValue.ToString, My.Globals.Variables.v_CIPTableCost)
            Dim pSubDec As String = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("STRATEGY").FormattedValue.ToString

            If s_dgCIP.Columns(CInt(pRowCol(0))).Name = "STRATEGY" Then






                Dim pSubType As ISubtypes = My.Globals.Variables.v_CIPTableCost

                s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value = _
                            My.Globals.Functions.GetDomainDisplay(pSubType.DefaultValue(pSubVal, My.Globals.Constants.c_CIPCostActionField), pSubType.Domain(pSubVal, My.Globals.Constants.c_CIPCostActionField))

                If UCase(pCboBox.Text) = UCase("Proposed") Then
                    'dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal1").Value = ""
                    'dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value = ""'dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal2").Value
                    'System.Configuration.ConfigurationManager.AppSettings("CIPReplacementValue")
                ElseIf UCase(pCboBox.Text) = UCase(My.Globals.Variables.v_CIPReplaceValue) Then
                    getReplacementValues(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value, pExistingVal1, pExistingVal2, pReplaceVal1, pReplaceVal2)
                    If pReplaceVal1 <> "" And pDomFilt1 IsNot Nothing Then
                        s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value = My.Globals.Functions.GetDomainDisplay(pReplaceVal1, pDomFilt1)
                    Else
                        If pReplaceVal1 <> "" Then
                            s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value = pReplaceVal1
                        Else
                            pReplaceVal1 = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value
                            pReplaceVal1 = My.Globals.Functions.GetDomainValue(pReplaceVal1, pDomFilt1)
                        End If

                    End If
                    If pReplaceVal2 <> "" And pDomFilt2 IsNot Nothing Then
                        s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value = My.Globals.Functions.GetDomainDisplay(pReplaceVal2, pDomFilt2)
                    Else
                        If pReplaceVal2 <> "" Then

                            s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value = pReplaceVal2
                        Else
                            pReplaceVal2 = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value
                            pReplaceVal2 = My.Globals.Functions.GetDomainValue(pReplaceVal2, pDomFilt2)
                        End If
                    End If
                Else
                    s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal1").Value
                    s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("exFiltVal2").Value
                    pReplaceVal2 = pExistingVal2
                    pReplaceVal1 = pExistingVal1
                End If

            End If

            Dim pDS As IDataset = pAssetLay.FeatureClass

            Dim strFCName As String = My.Globals.Functions.getClassName(pDS)

            Dim pCostRow As IRow = CheckForCostFeat(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value, strFCName, pSubVal, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value, pReplaceVal1, pReplaceVal2)
            'dgCIP.Rows(CInt(pRowCol(1))).Cells("STRATEGY").Value
            If pCostRow IsNot Nothing Then
                applyCostToRow(pCostRow, pRowCol(1))
            End If
            pCostRow = Nothing

            pCboBox.Parent.Controls.Remove(pCboBox)
            pDomFilt2 = Nothing
            pDomFilt1 = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: saveCombo" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub SetRowsTotal(ByVal Row As Integer)
        Try
            Dim dblAddVal As Double = 0
            If s_dgCIP.Rows(Row).Cells("ADDCOST").Value IsNot Nothing Then
                If s_dgCIP.Rows(Row).Cells("ADDCOST").Value IsNot DBNull.Value Then
                    If IsNumeric(s_dgCIP.Rows(Row).Cells("ADDCOST").Value) Then
                        dblAddVal = s_dgCIP.Rows(Row).Cells("ADDCOST").Value
                    End If

                End If
            End If

            Dim dblCostVal As Double = 0
            If s_dgCIP.Rows(Row).Cells("COST").Value IsNot Nothing Then
                If s_dgCIP.Rows(Row).Cells("COST").Value IsNot DBNull.Value Then
                    If IsNumeric(s_dgCIP.Rows(Row).Cells("COST").Value) Then
                        dblCostVal = s_dgCIP.Rows(Row).Cells("COST").Value
                    End If

                End If
            End If

            Dim dblTotCost As Double = dblCostVal

            If s_dgCIP.Rows(Row).Cells("LENGTH").Value IsNot Nothing Then
                If s_dgCIP.Rows(Row).Cells("LENGTH").Value IsNot DBNull.Value Then
                    If s_dgCIP.Rows(Row).Cells("LENGTH").Value <> 0 Then
                        dblTotCost = s_dgCIP.Rows(Row).Cells("LENGTH").Value * dblCostVal
                    End If
                End If

            End If

            If s_dgCIP.Rows(Row).Cells("MULTI").Value IsNot Nothing Then
                If s_dgCIP.Rows(Row).Cells("MULTI").Value IsNot DBNull.Value Then
                    If IsNumeric(s_dgCIP.Rows(Row).Cells("MULTI").Value) Then
                        If s_dgCIP.Rows(Row).Cells("MULTI").Value <> 0 Then
                            dblTotCost = dblTotCost * s_dgCIP.Rows(Row).Cells("MULTI").Value
                        End If
                    End If
                End If

            End If
            dblTotCost = dblTotCost + dblAddVal

            If IsNumeric(dblTotCost) Then
                s_dgCIP.Rows(Row).Cells("TOTCOST").Value = FormatCurrency(dblTotCost, 2, TriState.True, True) 'Format(Math.Round(dblTotCost, 2), "#,###.00")
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: setRowsTotal" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub applyCostToRow(ByVal CostRow As IRow, ByVal Row As Integer)
        Try
            If CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) IsNot Nothing Then
                If CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) IsNot DBNull.Value Then

                    If IsNumeric(CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField))) Then
                        s_dgCIP.Rows(Row).Cells("ADDCOST").Value = FormatCurrency(CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)), 2, TriState.True, TriState.True)
                    Else
                        s_dgCIP.Rows(Row).Cells("ADDCOST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
                    End If
                Else
                    s_dgCIP.Rows(Row).Cells("ADDCOST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
                End If
            Else
                s_dgCIP.Rows(Row).Cells("ADDCOST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
            End If
            If CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) IsNot Nothing Then
                If CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) IsNot DBNull.Value Then

                    If IsNumeric(CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostCostField))) Then
                        s_dgCIP.Rows(Row).Cells("COST").Value = FormatCurrency(CostRow.Value(CostRow.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)), 2, TriState.True, TriState.True)
                    Else
                        s_dgCIP.Rows(Row).Cells("COST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
                    End If
                Else
                    s_dgCIP.Rows(Row).Cells("COST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
                End If
            Else
                s_dgCIP.Rows(Row).Cells("COST").Value = FormatCurrency(0.0, 2, TriState.True, TriState.True)
            End If


            SetRowsTotal(Row)

            setProjectCostAndTotal()
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: applyCostToRow" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub SaveTextBox(ByVal pTxtBox As System.Windows.Forms.TextBox)
        Try
            Dim pRowCol() As String = pTxtBox.Tag.ToString.Split("|")
            Dim strExistVal As String = ""
            If s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value IsNot Nothing Then
                strExistVal = s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value.ToString

            End If


            If strExistVal.Replace("$", "") <> pTxtBox.Text Then

                Select Case s_dgCIP.Columns(CInt(pRowCol(0))).Name
                    Case "Notes"
                        s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = pTxtBox.Text

                    Case "proFiltVal1", "proFiltVal2"
                        s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = pTxtBox.Text
                        Dim subVal As String = s_dgCIP.Rows(CInt(pRowCol(1))).Cells("STRATEGY").Value



                        Dim pFl As IFeatureLayer = My.Globals.Functions.FindLayer(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").FormattedValue.ToString)
                        Dim pD As IDomain = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld1").FormattedValue.ToString)).Domain
                        Dim dmVal1 As String = My.Globals.Functions.GetDomainValue(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value.ToString(), pD)
                        pD = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("FiltFld2").FormattedValue.ToString)).Domain

                        Dim dmVal2 As String = My.Globals.Functions.GetDomainValue(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value, pD)


                        ' s_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal1").Value
                        's_dgCIP.Rows(CInt(pRowCol(1))).Cells("proFiltVal2").Value

                        Dim pCostRow As IRow = CheckForCostFeat(s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ASSETTYP").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("STRATEGY").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("ACTION").Value, dmVal1, dmVal2)

                        '(s_dgCIP.Rows(CInt(pRowCol(1))).Cells(0).Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells(4).Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells(5).Value, s_dgCIP.Rows(CInt(pRowCol(1))).Cells("OID").Value)
                        If pCostRow IsNot Nothing Then
                            applyCostToRow(pCostRow, pRowCol(1))
                            setProjectCostAndTotal()
                        End If

                        pCostRow = Nothing
                    Case "MULTI"

                        If IsNumeric(pTxtBox.Text) Then
                            s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = pTxtBox.Text



                            SetRowsTotal(pRowCol(1))
                        End If
                        setProjectCostAndTotal()

                    Case Else


                        If IsNumeric(pTxtBox.Text) Then
                            s_dgCIP.Rows(CInt(pRowCol(1))).Cells(CInt(pRowCol(0))).Value = FormatCurrency(pTxtBox.Text, 2, TriState.True, TriState.True)


                            SetRowsTotal(pRowCol(1))
                        End If
                        setProjectCostAndTotal()




                End Select

            End If

            pTxtBox.Parent.Controls.Remove(pTxtBox)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: SaveTextBox" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub RowRemoved(ByVal Tag As String)
        Try


            My.ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, RemoveGraphic(Tag), My.ArcMap.Document.ActiveView.Extent)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: RowRemoved" & vbCrLf & ex.Message)


        End Try
    End Sub
    Private Shared Function RemoveGraphic(ByVal Tag As String) As IGeometry

        Try


            My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

            Dim pElem As IElement
            Dim pElProp As IElementProperties
            pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next

            If Not (Tag.Contains("CIPTools:")) Then
                Tag = "CIPTools:" & Tag
            End If

            Do Until pElem Is Nothing
                pElProp = pElem
                If pElProp.CustomProperty IsNot Nothing Then
                    If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                        Dim pStrElem As String = pElProp.CustomProperty.ToString
                        If pStrElem = Tag Then
                            If My.Globals.Variables.v_LastSelection = Tag Then
                                My.Globals.Variables.v_LastSelection = ""
                            End If
                            My.ArcMap.Document.ActiveView.GraphicsContainer.DeleteElement(pElem)
                            Return pElem.Geometry
                            '          m_pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pElem.Geometry, m_pMxDoc.ActiveView.Extent)
                            Exit Do

                        End If
                    End If

                End If
                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
            Loop




            pElem = Nothing
            pElProp = Nothing
            Return Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: RowRemoved" & vbCrLf & ex.Message)

            Return Nothing

        End Try
    End Function
    Private Shared Sub AddGraphic(ByVal pGeo As IGeometry, ByVal Tag As String)

        Try


            Dim pElem As IElement

            Dim pElementProp As IElementProperties2
            Select Case pGeo.GeometryType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    Dim pFSElem As IMarkerElement
                    pFSElem = New MarkerElement
                    pFSElem.Symbol = My.Globals.Variables.v_PointGraphicSymbol

                    pElem = pFSElem
                    pElem.Geometry = pGeo
                    pElementProp = pElem
                    pFSElem = Nothing
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    Dim pFSElem As IFillShapeElement
                    pFSElem = New PolygonElement
                    pFSElem.Symbol = My.Globals.Variables.v_PolygonGraphicSymbol

                    pElem = pFSElem
                    pElem.Geometry = pGeo
                    pElementProp = pElem
                    pFSElem = Nothing
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    Dim pFSElem As ILineElement
                    pFSElem = New LineElement
                    pFSElem.Symbol = My.Globals.Variables.v_LineGraphicSymbol

                    pElem = pFSElem
                    pElem.Geometry = pGeo
                    pElementProp = pElem
                    pFSElem = Nothing
                Case Else
                    MsgBox("Unsupported type, exiting, AddGraphic function")
                    Return


            End Select

            pElementProp.CustomProperty = "CIPTools:" & Tag
            pElementProp.Name = Tag
            My.ArcMap.Document.ActiveView.GraphicsContainer.AddElement(pElem, 0)


            pElem = Nothing
            pElementProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: AddGraphic" & vbCrLf & ex.Message)

        End Try
    End Sub




    'Private Shared function CheckForCostCbo(ByVal strLayName As String, ByVal strFilt1 As String, ByVal strFilt2 As String, ByVal OID As Integer) As IRow
    '    Try
    '        Dim pAssetLay As IFeatureLayer = My.Globals.Functions.findLayer(strLayName, my.ArcMap.Document.FocusMap )
    '        Dim pDefTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPDefTableName, my.ArcMap.Document.FocusMap )
    '        Dim pCostTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPCostTableName, my.ArcMap.Document.FocusMap )
    '        Dim pQFiltDef As IQueryFilter = New QueryFilter
    '        Dim pSQL As String = ""
    '        pSQL = my.Globals.Constants.c_CIPDefNameField & " = '" & strLayName & "'"
    '        '  pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & my.Globals.Constants.c_CIPCostActiveField & " is Null)"
    '        pQFiltDef.WhereClause = pSQL
    '        Dim pDefCurs As ICursor
    '        pDefCurs = pDefTbl.Search(pQFiltDef, True)
    '        Dim pDefRow As IRow = pDefCurs.NextRow
    '        '"(" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & "' OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"
    '        If Not pDefRow Is Nothing Then
    '            If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField1)) IsNot Nothing Then
    '                If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField1)) IsNot DBNull.Value Then
    '                    If Trim(strFilt1) <> "" Then
    '                        If pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField1)))).Domain Is Nothing Then
    '                            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                                pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                            Else
    '                                pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                            End If


    '                        Else
    '                            Dim pDom As IDomain = pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField1)))).Domain
    '                            If TypeOf pDom Is ICodedValueDomain Then
    '                                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = '" & GetDomainValue(strFilt1, pDom) & "'"
    '                                Else
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = " & GetDomainValue(strFilt1, pDom)
    '                                End If


    '                            Else
    '                                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                                Else
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                                End If


    '                            End If
    '                            pDom = Nothing

    '                        End If


    '                    Else
    '                        If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                            If pSQL = "" Then

    '                                pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                            Else

    '                                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                            End If

    '                        Else
    '                            If pSQL = "" Then

    '                                pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                            Else

    '                                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                            End If

    '                        End If
    '                    End If
    '                Else
    '                    If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                        If pSQL = "" Then

    '                            pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                        Else

    '                            pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                        End If

    '                    Else
    '                        If pSQL = "" Then

    '                            pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                        Else

    '                            pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                        End If

    '                    End If
    '                End If
    '            Else
    '                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                    If pSQL = "" Then

    '                        pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                    Else

    '                        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                    End If

    '                Else
    '                    If pSQL = "" Then

    '                        pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"



    '                    Else

    '                        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '                    End If

    '                End If
    '            End If
    '            If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField2)) IsNot Nothing Then
    '                If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField2)) IsNot DBNull.Value Then
    '                    If Trim(strFilt2) <> "" Then
    '                        If pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField2)))).Domain Is Nothing Then
    '                            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                                If pSQL = "" Then
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                Else
    '                                    pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                End If

    '                            Else
    '                                If pSQL = "" Then
    '                                    pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2 & ""
    '                                Else
    '                                    pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2 & ""
    '                                End If

    '                            End If


    '                        Else
    '                            Dim pDom As IDomain = pAssetLay.FeatureClass.Fields.Field(pAssetLay.FeatureClass.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPDefFiltField2)))).Domain
    '                            If TypeOf pDom Is ICodedValueDomain Then
    '                                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                                    If pSQL = "" Then
    '                                        pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = '" & GetDomainValue(strFilt2, pDom) & "'"
    '                                    Else
    '                                        pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & GetDomainValue(strFilt2, pDom) & "'"
    '                                    End If

    '                                Else

    '                                    If pSQL = "" Then
    '                                        pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = " & GetDomainValue(strFilt2, pDom)
    '                                    Else
    '                                        pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & GetDomainValue(strFilt2, pDom)


    '                                    End If

    '                                End If


    '                            Else
    '                                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                                    If pSQL = "" Then
    '                                        pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                    Else
    '                                        pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                    End If

    '                                Else
    '                                    If pSQL = "" Then
    '                                        pSQL = my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2 & ""
    '                                    Else
    '                                        pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2 & ""
    '                                    End If

    '                                End If



    '                            End If
    '                            pDom = Nothing

    '                        End If
    '                    Else
    '                        If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                            If pSQL = "" Then

    '                                pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                            Else

    '                                pSQL = pSQL & " AND " & "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                            End If

    '                        Else
    '                            If pSQL = "" Then

    '                                pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                            Else

    '                                pSQL = pSQL & " AND " & "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                            End If

    '                        End If


    '                    End If
    '                Else
    '                    If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                        If pSQL = "" Then

    '                            pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                        Else

    '                            pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                        End If

    '                    Else
    '                        If pSQL = "" Then

    '                            pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                        Else

    '                            pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                            pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                        End If

    '                    End If
    '                End If
    '            Else
    '                If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                    If pSQL = "" Then

    '                        pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                    Else

    '                        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                    End If

    '                Else
    '                    If pSQL = "" Then

    '                        pSQL = "(" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '                    Else

    '                        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                        pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '                    End If

    '                End If
    '            End If
    '            If pSQL = "" Then
    '                pSQL = my.Globals.Constants.c_CIPCostNameField & " = '" & strLayName & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostNameField & " = '" & strLayName & "'"
    '            End If
    '            '   pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & my.Globals.Constants.c_CIPCostActiveField & " is Null)"
    '        End If
    '        pQFiltDef = New QueryFilter
    '        pQFiltDef.WhereClause = pSQL

    '        pDefCurs = pCostTbl.Search(pQFiltDef, True)

    '        If pDefCurs IsNot Nothing Then

    '            pDefRow = pDefCurs.NextRow

    '            'If pAssetLay IsNot Nothing Then
    '            '    Do Until pDefRow Is Nothing


    '            '        If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) Is Nothing Then
    '            '            If pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) Is DBNull.Value Then
    '            '                Dim pQfilt As IQueryFilter = New QueryFilter
    '            '                pQfilt.WhereClause = pAssetLay.FeatureClass.OIDFieldName & " = " & OID & _
    '            '                                    pDefRow.Value(pDefRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField))
    '            '                If pAssetLay.FeatureClass.FeatureCount(pQfilt) > 0 Then Exit Do

    '            '            Else
    '            '                Exit Do

    '            '            End If
    '            '        Else

    '            '            Exit Do

    '            '        End If
    '            '        pDefRow = pDefCurs.NextRow

    '            '    Loop
    '            'End If

    '        End If


    '        Marshal.ReleaseComObject(pDefCurs)

    '        pQFiltDef = Nothing
    '        pAssetLay = Nothing
    '        pDefTbl = Nothing
    '        pCostTbl = Nothing
    '        pDefCurs = Nothing


    '        Return pDefRow

    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CIPProjectWindow: CheckForCostCBO" & vbCrLf & ex.Message)
    '        Return Nothing
    '    End Try

    'End Function
    'Private Shared function CheckForCostTxt(ByVal strLayName As String, ByVal strFilt1 As String, ByVal strFilt2 As String, ByVal oid As Integer) As IRow
    '    Try
    '        Dim pAssetLay As IFeatureLayer = My.Globals.Functions.findLayer(strLayName, my.ArcMap.Document.FocusMap )
    '        Dim pDefTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPDefTableName, my.ArcMap.Document.FocusMap )
    '        Dim pCostTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPCostTableName, my.ArcMap.Document.FocusMap )
    '        Dim pQFiltDef As IQueryFilter = New QueryFilter
    '        Dim pSQL As String = ""
    '        pSQL = my.Globals.Constants.c_CIPCostNameField & " = '" & strLayName & "'"
    '        '  pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & my.Globals.Constants.c_CIPCostActiveField & " is Null)"
    '        If Trim(strFilt1) <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFilt1 & "'"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"
    '            Else
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFilt1
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"

    '            End If


    '        End If

    '        If strFilt2 <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFilt2 & "'"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '            Else
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFilt2
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"

    '            End If

    '        End If

    '        Dim pQFiltCost As IQueryFilter = New QueryFilter
    '        pQFiltCost.WhereClause = pSQL

    '        Dim pCurs As ICursor = pCostTbl.Search(pQFiltCost, True)
    '        Dim pRow As IRow
    '        If pCurs IsNot Nothing Then

    '            pRow = pCurs.NextRow
    '            '' Dim pFL As IFeatureLayer = My.Globals.Functions.findLayer(strLayName, my.ArcMap.Document.FocusMap )
    '            'If pAssetLay IsNot Nothing Then

    '            '    If CInt(oid) > 0 Then
    '            '        Do Until pRow Is Nothing


    '            '            If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot Nothing Then
    '            '                If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot DBNull.Value Then

    '            '                    pQFiltCost.WhereClause = pAssetLay.FeatureClass.OIDFieldName & " = " & oid & _
    '            '                                        pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField))
    '            '                    If pAssetLay.FeatureClass.FeatureCount(pQFiltCost) > 0 Then
    '            '                        Exit Do
    '            '                    End If
    '            '                Else
    '            '                    Exit Do
    '            '                End If
    '            '            Else
    '            '                Exit Do

    '            '            End If

    '            '            pRow = pCurs.NextRow
    '            '        Loop
    '            '    End If
    '            'End If
    '        End If

    '        Marshal.ReleaseComObject(pCurs)

    '        pCurs = Nothing

    '        pQFiltCost = Nothing
    '        pAssetLay = Nothing
    '        pDefTbl = Nothing
    '        pCostTbl = Nothing
    '        pRow = Nothing
    '        Return pRow


    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools -  CIPProjectWindow: CheckForCostTxt" & vbCrLf & ex.Message)
    '        Return Nothing
    '    End Try

    'End Function
    Private Shared Function getFeatureCount(ByVal pFeatLayer As IFeatureLayer, ByVal pGeo As IGeometry, Optional ByVal sql As String = "") As Integer
        Try
            If pGeo.IsEmpty Then Return 0


            Dim pSpatFilt As ISpatialFilter = New SpatialFilter
            pSpatFilt.GeometryField = pFeatLayer.FeatureClass.ShapeFieldName
            pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            pSpatFilt.Geometry = pGeo
            If sql <> "" Then
                pSpatFilt.WhereClause = sql
            End If

            getFeatureCount = pFeatLayer.FeatureClass.FeatureCount(pSpatFilt)
            pSpatFilt = Nothing

            Return getFeatureCount
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - getFeatureCount" & vbCrLf & ex.Message)
            Return Nothing

        End Try
    End Function
    Private Shared Function ValidDefTable(ByVal DefTable As ITable) As Boolean
        Try
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefNameField) < 0 Then Return False
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefLenField) < 0 Then Return False
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefIDField) < 0 Then Return False
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2) < 0 Then Return False
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1) < 0 Then Return False
            If DefTable.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField) < 0 Then Return False
            Return True
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIP Selection: ValidDefTable" & vbCrLf & ex.Message)
            Return False

        End Try
    End Function

    'Private Shared function CheckForCostFeatWithAdv(ByVal strLayName As String, ByVal strFiltField1 As String, ByVal strFiltField2 As String, ByVal FeatureToCost As IFeature, ByVal SourceLayer As IFeatureLayer) As IRow

    '    Try
    '        Dim pDefTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPDefTableName, my.ArcMap.Document.FocusMap )
    '        Dim pCostTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPCostTableName, my.ArcMap.Document.FocusMap )
    '        Dim pQFiltDef As IQueryFilter = New QueryFilter
    '        Dim pSQL As String = ""

    '        pSQL = my.Globals.Constants.c_CIPCostNameField & " = '" & strLayName & "'"

    '        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & my.Globals.Constants.c_CIPCostActiveField & " is Null)"



    '        If Trim(strFiltField1) <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & FeatureToCost.Value(FeatureToCost.Fields.FindField(strFiltField1)) & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = " & FeatureToCost.Value(FeatureToCost.Fields.FindField(strFiltField1))
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = ''"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"


    '            End If


    '        End If

    '        If strFiltField2 <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & FeatureToCost.Value(FeatureToCost.Fields.FindField(strFiltField2)) & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & FeatureToCost.Value(FeatureToCost.Fields.FindField(strFiltField2))
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = ''"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '            End If

    '        End If

    '        Dim pQFiltCost As IQueryFilter = New QueryFilter
    '        pQFiltCost.WhereClause = pSQL

    '        Dim pCurs As ICursor = pCostTbl.Search(pQFiltCost, True)
    '        Dim pRow As IRow = Nothing
    '        If pCurs IsNot Nothing Then

    '            pRow = pCurs.NextRow


    '            Do Until pRow Is Nothing


    '                If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot Nothing Then
    '                    If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot DBNull.Value Then
    '                        If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) <> "" Then
    '                            pQFiltCost.WhereClause = SourceLayer.FeatureClass.OIDFieldName & " = " & FeatureToCost.OID & _
    '                                                pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField))
    '                            If SourceLayer.FeatureClass.FeatureCount(pQFiltCost) > 0 Then
    '                                Exit Do
    '                            Else

    '                            End If
    '                        Else
    '                            Exit Do

    '                        End If
    '                    Else
    '                        Exit Do
    '                    End If
    '                Else
    '                    Exit Do

    '                End If

    '                pRow = pCurs.NextRow
    '            Loop


    '        End If


    '        Marshal.ReleaseComObject(pCurs)

    '        pCurs = Nothing

    '        pQFiltCost = Nothing

    '        pDefTbl = Nothing
    '        pCostTbl = Nothing

    '        Return pRow


    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools -  CIPProjectWindow: CheckForCostFeature" & vbCrLf & ex.Message)
    '        Return Nothing
    '    End Try

    'End Function
    Private Shared Function CheckForCostFeat(ByVal strLayNameConfig As String, ByVal strClassName As String, ByVal strStrat As String, ByVal strActionVal As String, ByVal strActionLookup As String, ByVal strFiltVal1 As String, ByVal strFiltVal2 As String) As IRow

        Try

            Dim pQFilt As IQueryFilter = New QueryFilter
            Dim pSQL As String = ""

            Dim pRow As IRow = Nothing
            Dim pCurs As ICursor
            If strLayNameConfig = strClassName Then
                Dim pFL As IFeatureLayer = My.Globals.Functions.FindLayer(strLayNameConfig)
                If pFL IsNot Nothing Then

                    pSQL = "(" & My.Globals.Constants.c_CIPDefNameField & " = '" & strLayNameConfig & "' OR "


                    Dim pFC As IFeatureClass = pFL.FeatureClass
                    Dim pDT As IDataset = pFC
                    Dim strName As String = pDT.Name
                    If strName = "" Then
                        strName = pDT.BrowseName
                    End If
                    If strName = "" Then
                        strName = pDT.FullName.ToString
                    End If
                    If strName.Contains(".") Then
                        strName = strName.Substring(strName.LastIndexOf("."))

                    End If

                    pSQL = pSQL & My.Globals.Constants.c_CIPDefNameField & " = '" & pDT.Name & "' ) "
                Else

                    pSQL = "" & My.Globals.Constants.c_CIPDefNameField & " = '" & strLayNameConfig & "' "

                End If


            Else
                pSQL = "(" & My.Globals.Constants.c_CIPDefNameField & " = '" & strLayNameConfig & "' or "
                pSQL = pSQL & My.Globals.Constants.c_CIPDefNameField & " = '" & strClassName & "' ) "

            End If
            '     pSQL = "(" & My.Globals.Constants.c_CIPDefNameField & " = '" & strLayNameConfig & "' or "
            '     pSQL = pSQL & My.Globals.Constants.c_CIPDefNameField & " = '" & strClassName & "' ) "
            pQFilt.WhereClause = pSQL
            pCurs = My.Globals.Variables.v_CIPTableDef.Search(pQFilt, True)
            If pCurs Is Nothing Then Return Nothing
            pRow = pCurs.NextRow
            If pRow Is Nothing Then Return Nothing

            Dim strFiltField1 As String = "", strFiltField2 As String = ""

            If pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1) > 0 Then
                If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot Nothing Then
                    If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot DBNull.Value Then
                        strFiltField1 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))
                    End If
                End If
            End If

            If pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2) > 0 Then
                If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot Nothing Then
                    If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot DBNull.Value Then
                        strFiltField2 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))
                    End If
                End If
            End If


            pSQL = "(" & My.Globals.Constants.c_CIPCostNameField & " = '" & strLayNameConfig & "' OR "
            pSQL = pSQL & My.Globals.Constants.c_CIPCostNameField & " = '" & strClassName & "')"
            If My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostStrategyField)).Type = esriFieldType.esriFieldTypeString Then
                pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostStrategyField & " = '" & strStrat & "'"

            Else
                If IsNumeric(strStrat) = False Then
                    strStrat = My.Globals.Functions.GetSubtypeValue(strStrat, My.Globals.Variables.v_CIPTableCost)

                End If
                pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostStrategyField & " = " & strStrat & ""

            End If


            If strActionVal = strActionLookup Then
                pSQL = pSQL & " AND ("
                Dim strAct As String = strActionVal
                

                pSQL = pSQL & My.Globals.Constants.c_CIPCostActionField & " = '" & strAct & "' OR "
                strAct = strActionLookup

                strAct = My.Globals.Functions.GetDomainValue(strAct, ActionDomain(strStrat, My.Globals.Variables.v_CIPTableCost))

                pSQL = pSQL & My.Globals.Constants.c_CIPCostActionField & " = '" & strAct & "'  "



                pSQL = pSQL & ")"
            Else
                pSQL = pSQL & " AND ("
                Dim strAct As String = strActionVal
                strAct = My.Globals.Functions.GetDomainValue(strAct, My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.FindField(My.Globals.Constants.c_CIPCostActionField)).Domain)


                pSQL = pSQL & My.Globals.Constants.c_CIPCostActionField & " = '" & strAct & "' OR "
                strAct = strActionLookup
                strAct = My.Globals.Functions.GetDomainValue(strAct, My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.FindField(My.Globals.Constants.c_CIPCostActionField)).Domain)

                pSQL = pSQL & My.Globals.Constants.c_CIPCostActionField & " = '" & strAct & "'  "



                pSQL = pSQL & ")"
            End If
          



            If strFiltField1 <> "" Then

                If Trim(strFiltVal1) <> "" Then
                    If My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
                        pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFiltVal1 & "'"
                    Else
                        pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostFiltField1 & " = " & strFiltVal1
                    End If
                Else
                    If My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
                        pSQL = pSQL & " AND (" & My.Globals.Constants.c_CIPCostFiltField1 & " = ''"
                        pSQL = pSQL & " OR " & My.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"


                    End If


                End If
            End If
            If strFiltField2 <> "" Then


                If Trim(strFiltVal2) <> "" Then
                    If My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
                        pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFiltVal2 & "'"
                    Else
                        pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostFiltField2 & " = " & strFiltVal2
                    End If
                Else
                    If My.Globals.Variables.v_CIPTableCost.Fields.Field(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
                        pSQL = pSQL & " AND (" & My.Globals.Constants.c_CIPCostFiltField2 & " = ''"
                        pSQL = pSQL & " OR " & My.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



                    End If

                End If
            End If
            pQFilt = New QueryFilter

            pQFilt.WhereClause = pSQL

            pCurs = My.Globals.Variables.v_CIPTableCost.Search(pQFilt, True)
            pRow = Nothing

            If pCurs IsNot Nothing Then
                pRow = pCurs.NextRow
            End If


            Marshal.ReleaseComObject(pCurs)

            pCurs = Nothing

            pQFilt = Nothing



            Return pRow


        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: CheckForCostFeature" & vbCrLf & ex.Message)
            Return Nothing
        End Try

    End Function
    Private Shared Sub getReplacementValues(ByVal strLayNameConfig As String, ByVal strClassName As String, ByVal strAction As String, ByVal strFiltVal1 As String, ByVal strFiltVal2 As String, ByRef strDefVal1 As String, ByRef strDefVal2 As String)

        Try

            Dim pQFiltDef As IQueryFilter = New QueryFilter
            Dim pSQL As String = ""

            pSQL = "(" & My.Globals.Constants.c_CIPReplaceNameField & " = '" & strLayNameConfig & "' or "
            pSQL = pSQL & My.Globals.Constants.c_CIPReplaceNameField & " = '" & strClassName & "')"
            pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPCostActionField & " = '" & strAction & "'"



            If Trim(strFiltVal1) <> "" Then
                If My.Globals.Variables.v_CIPTableReplace.Fields.Field(My.Globals.Variables.v_CIPTableReplace.Fields.FindField(My.Globals.Constants.c_CIPReplaceFiltField1)).Type = esriFieldType.esriFieldTypeString Then
                    pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPReplaceFiltField1 & " = '" & strFiltVal1 & "'"
                Else
                    pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPReplaceFiltField1 & " = " & strFiltVal1
                End If
            Else
                If My.Globals.Variables.v_CIPTableReplace.Fields.Field(My.Globals.Variables.v_CIPTableReplace.Fields.FindField(My.Globals.Constants.c_CIPReplaceFiltField1)).Type = esriFieldType.esriFieldTypeString Then
                    pSQL = pSQL & " AND (" & My.Globals.Constants.c_CIPReplaceFiltField1 & " = ''"
                    pSQL = pSQL & " OR " & My.Globals.Constants.c_CIPReplaceFiltField1 & " is NULL" & ")"


                End If


            End If

            If Trim(strFiltVal2) <> "" Then
                If My.Globals.Variables.v_CIPTableReplace.Fields.Field(My.Globals.Variables.v_CIPTableReplace.Fields.FindField(My.Globals.Constants.c_CIPReplaceFiltField2)).Type = esriFieldType.esriFieldTypeString Then
                    pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPReplaceFiltField2 & " = '" & strFiltVal2 & "'"
                Else
                    pSQL = pSQL & " AND " & My.Globals.Constants.c_CIPReplaceFiltField2 & " = " & strFiltVal2
                End If
            Else
                If My.Globals.Variables.v_CIPTableReplace.Fields.Field(My.Globals.Variables.v_CIPTableReplace.Fields.FindField(My.Globals.Constants.c_CIPReplaceFiltField2)).Type = esriFieldType.esriFieldTypeString Then
                    pSQL = pSQL & " AND (" & My.Globals.Constants.c_CIPReplaceFiltField2 & " = ''"
                    pSQL = pSQL & " OR " & My.Globals.Constants.c_CIPReplaceFiltField2 & " is NULL" & ")"



                End If

            End If

            Dim pQFiltCost As IQueryFilter = New QueryFilter
            pQFiltCost.WhereClause = pSQL

            Dim pCurs As ICursor = My.Globals.Variables.v_CIPTableReplace.Search(pQFiltCost, True)
            strDefVal1 = strFiltVal1
            strDefVal2 = strFiltVal2
            Dim pRow As IRow = Nothing
            If pCurs IsNot Nothing Then
                pRow = pCurs.NextRow
                If pRow IsNot Nothing Then
                    If strFiltVal1 <> "" Then
                        If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField1)) IsNot Nothing Then
                            If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField1)) IsNot DBNull.Value Then
                                strDefVal1 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField1))
                            End If
                        End If

                    End If
                    If strFiltVal2 <> "" Then
                        If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField2)) IsNot Nothing Then
                            If pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField2)) IsNot DBNull.Value Then
                                strDefVal2 = pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPReplaceDefField2))
                            End If
                        End If


                    End If

                End If

            End If

            pRow = Nothing

            Marshal.ReleaseComObject(pCurs)

            pCurs = Nothing

            pQFiltCost = Nothing





        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: getReplacementValues" & vbCrLf & ex.Message)
            Return
        End Try

    End Sub
    'Private Shared function CheckForCostFeatByValue(ByVal strLayName As String, ByVal strFiltfield1 As String, ByVal strFiltField2 As String, ByVal strFiltVal1 As String, ByVal strFiltVal2 As String, ByVal FeatureToCost As IFeature, ByVal SourceLayer As IFeatureLayer) As IRow

    '    Try
    '        Dim pDefTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPDefTableName, my.ArcMap.Document.FocusMap )
    '        Dim pCostTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPCostTableName, my.ArcMap.Document.FocusMap )
    '        Dim pQFiltDef As IQueryFilter = New QueryFilter
    '        Dim pSQL As String = ""

    '        pSQL = my.Globals.Constants.c_CIPCostNameField & " = '" & strLayName & "'"

    '        pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & my.Globals.Constants.c_CIPCostActiveField & " is Null)"



    '        If Trim(strFiltfield1) <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = '" & strFiltVal1 & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField1 & " = " & strFiltVal1
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField1)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField1 & " = ''"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField1 & " is NULL" & ")"


    '            End If


    '        End If

    '        If strFiltField2 <> "" Then
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = '" & strFiltVal2 & "'"
    '            Else
    '                pSQL = pSQL & " AND " & my.Globals.Constants.c_CIPCostFiltField2 & " = " & strFiltVal2
    '            End If
    '        Else
    '            If pCostTbl.Fields.Field(pCostTbl.Fields.FindField(my.Globals.Constants.c_CIPCostFiltField2)).Type = esriFieldType.esriFieldTypeString Then
    '                pSQL = pSQL & " AND (" & my.Globals.Constants.c_CIPCostFiltField2 & " = ''"
    '                pSQL = pSQL & " OR " & my.Globals.Constants.c_CIPCostFiltField2 & " is NULL" & ")"



    '            End If

    '        End If

    '        Dim pQFiltCost As IQueryFilter = New QueryFilter
    '        pQFiltCost.WhereClause = pSQL

    '        Dim pCurs As ICursor = pCostTbl.Search(pQFiltCost, True)
    '        Dim pRow As IRow = Nothing
    '        If pCurs IsNot Nothing Then

    '            pRow = pCurs.NextRow


    '            Do Until pRow Is Nothing


    '                If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot Nothing Then
    '                    If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) IsNot DBNull.Value Then
    '                        If pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField)) <> "" Then
    '                            pQFiltCost.WhereClause = SourceLayer.FeatureClass.OIDFieldName & " = " & FeatureToCost.OID & _
    '                                                pRow.Value(pRow.Fields.FindField(my.Globals.Constants.c_CIPCostAdvFiltField))
    '                            If SourceLayer.FeatureClass.FeatureCount(pQFiltCost) > 0 Then
    '                                Exit Do
    '                            Else

    '                            End If
    '                        Else
    '                            Exit Do

    '                        End If
    '                    Else
    '                        Exit Do
    '                    End If
    '                Else
    '                    Exit Do

    '                End If

    '                pRow = pCurs.NextRow
    '            Loop


    '        End If


    '        Marshal.ReleaseComObject(pCurs)

    '        pCurs = Nothing

    '        pQFiltCost = Nothing

    '        pDefTbl = Nothing
    '        pCostTbl = Nothing

    '        Return pRow


    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools -  CIPProjectWindow: CheckForCostFeature" & vbCrLf & ex.Message)
    '        Return Nothing
    '    End Try

    'End Function

    Private Shared Sub saveControl()
        Try

            For Each cnt As Control In s_dgCIP.Controls
                If TypeOf cnt Is TextBox Then
                    SaveTextBox(cnt)
                ElseIf TypeOf cnt Is ComboBox Then
                    saveCombo(cnt)

                End If

            Next
        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: saveControl" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub addQuote(ByVal chrCode As Single)
        Try
            For Each cnt As Control In s_dgCIP.Controls
                If TypeOf cnt Is TextBox Then
                    Dim pLoc As Int16 = CType(cnt, TextBox).SelectionStart

                    cnt.Text = cnt.Text.Substring(0, pLoc) & Chr(chrCode) & cnt.Text.Substring(pLoc)


                    CType(cnt, TextBox).SelectionStart = pLoc + 1
                End If
            Next
        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: addQuote" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub addPeriod()
        Try

            For Each cnt As Control In s_dgCIP.Controls
                If TypeOf cnt Is TextBox Then
                    If cnt.Text.Contains(".") Then Return
                    Dim pLoc As Int16 = CType(cnt, TextBox).SelectionStart

                    cnt.Text = cnt.Text.Substring(0, pLoc) & Chr(46) & cnt.Text.Substring(pLoc)


                    CType(cnt, TextBox).SelectionStart = pLoc + 1

                End If
            Next
        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: addQuote" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub deleteRecord()
        Try

            If s_dgCIP.Rows.Count = 0 Then Return



            Dim strTag As String = s_dgCIP.SelectedRows(0).Cells(0).Value & ":" & s_dgCIP.SelectedRows(0).Cells("OID").Value

            s_dgCIP.Rows.Remove(s_dgCIP.SelectedRows(0))
            RowRemoved(strTag)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: deleteRecord" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub InitGrid()
        Try



            s_dgCIP = New myDG
            s_dgCIP.AllowUserToAddRows = False
            s_dgCIP.EditMode = DataGridViewEditMode.EditProgrammatically

            s_dgCIP.AllowUserToDeleteRows = False
            AddHandler s_dgCIP.RowsAdded, AddressOf dgCIP_RowsAdded
            AddHandler s_dgCIP.Scroll, AddressOf dgCIP_Scroll

            AddHandler s_dgCIP.CellClick, AddressOf dgCIP_CellClick

            AddHandler s_dgCIP.CellMouseClick, AddressOf dgCIP_CellMouseClick

            AddHandler s_dgCIP.DataGridKeyIntercept, AddressOf dgCIP_DataGridKeyIntercept
            AddHandler s_dgCIP.SelectionChanged, AddressOf dgCIP_SelectionChanged

            s_gpBxCIPCan.Controls.Add(s_dgCIP)

            s_dgCIP.Dock = DockStyle.Fill

            s_dgCIP.ColumnCount = 18
            With s_dgCIP.ColumnHeadersDefaultCellStyle
                .BackColor = Color.Navy
                .ForeColor = Color.White
                .Font = New Font(s_dgCIP.Font, FontStyle.Bold)
                .Alignment = DataGridViewContentAlignment.MiddleCenter
            End With

            With s_dgCIP

                .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells

                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
                .CellBorderStyle = DataGridViewCellBorderStyle.Single
                .GridColor = Color.Black
                .RowHeadersVisible = False

                .Columns(0).Name = "ASSETTYP"
                .Columns(0).HeaderText = "Source Asset"
                .Columns(1).Name = "ASSETID"
                .Columns(1).HeaderText = "Asset ID"
                .Columns(2).Name = "STRATEGY"
                .Columns(2).HeaderText = "Strategy"
                .Columns(3).Name = "ACTION"
                .Columns(3).HeaderText = "Action"

                .Columns(4).Name = "exFiltVal1"
                .Columns(4).HeaderText = "Existing: 1"
                .Columns(5).Name = "exFiltVal2"
                .Columns(5).HeaderText = "Existing: 2"
                .Columns(6).Name = "proFiltVal1"
                .Columns(6).HeaderText = "Proposed: 1"
                .Columns(7).Name = "proFiltVal2"
                .Columns(7).HeaderText = "Proposed: 2"
                .Columns(8).Name = "COST"
                .Columns(8).HeaderText = "Cost"
                .Columns(8).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(9).Name = "MULTI"
                .Columns(9).HeaderText = "Multiplier"
                .Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                .Columns(10).Name = "ADDCOST"
                .Columns(10).HeaderText = "Add. Cost"
                .Columns(10).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(11).Name = "LENGTH"
                .Columns(11).HeaderText = "Length/Area"
                .Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                .Columns(12).Name = "TOTCOST"
                .Columns(12).HeaderText = "Total Cost"
                .Columns(12).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(12).DefaultCellStyle.Font = _
                    New Font(s_dgCIP.DefaultCellStyle.Font, FontStyle.Bold)
                .Columns(13).Name = "GeoType"
                .Columns(13).HeaderText = "GeoType"
                .Columns(13).Visible = False
                .Columns(14).Name = "FiltFld1"
                .Columns(14).HeaderText = "FiltFld1"
                .Columns(14).Visible = False
                .Columns(15).Name = "FiltFld2"
                .Columns(15).HeaderText = "FiltFld2"

                .Columns(15).Visible = False
                .Columns(16).Name = "OID"
                .Columns(16).HeaderText = "OID"
                .Columns(16).Visible = False
                .Columns(17).Name = "Notes"
                .Columns(17).HeaderText = "Notes"
                .Columns(17).Visible = True


                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .MultiSelect = False
                .Dock = DockStyle.Fill

            End With




        Catch ex As Exception
            MsgBox("Error in the Costing Tools -  CIPProjectWindow: InitGrid" & vbCrLf & ex.Message)


        End Try
    End Sub

    Friend Shared Function GetTarget() As layerAndTypes
        Try

            If s_cboDefLayers.Items.Count = 0 Then Return Nothing
            Return s_cboDefLayers.SelectedItem
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: GetTarget" & vbCrLf & ex.Message)
            Return Nothing

        End Try
    End Function
    Friend Shared Sub HighlightAtLocation(ByVal Envelope As IEnvelope)
        Dim pMxApp As IMxApplication


        Dim pEnumElem As IEnumElement
        Dim pElem As IElement
        Dim pElemProp As IElementProperties
        Try



            pEnumElem = My.ArcMap.Document.ActiveView.GraphicsContainer.LocateElementsByEnvelope(Envelope)

            If pEnumElem IsNot Nothing Then


                pElem = pEnumElem.Next
                If pElem IsNot Nothing Then
                    pElemProp = pElem
                    If pElemProp.CustomProperty IsNot Nothing Then
                        If pElemProp.CustomProperty.ToString.Contains("CIPTools") Then
                            Dim pStrTag() As String = pElemProp.CustomProperty.ToString.Split(":")
                            If pStrTag.Length = 3 Then
                                FindRow(pStrTag(1), pStrTag(2)).Selected = True
                                Return
                            ElseIf pStrTag.Length = 4 Then
                                FindRow(pStrTag(1), pStrTag(2) & ":" & pStrTag(3)).Selected = True

                                Return

                            End If

                        End If
                    End If
                    pElemProp = Nothing

                End If
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: HighlightAtLocation" & vbCrLf & ex.Message)
        Finally
            pMxApp = Nothing
            pEnumElem = Nothing
            pElem = Nothing
        End Try
    End Sub
    Friend Shared Sub HighlightAtLocation(ByVal pPnt As IPoint)

        Dim pEnumElem As IEnumElement
        Dim pElem As IElement
        Dim pElemProp As IElementProperties
        Try
            'Dim pMxApp As IMxApplication
            ' pMxApp = m_application

            ' Dim pPnt As IPoint = pMxApp.Display.DisplayTransformation.ToMapPoint(x, y)


            pEnumElem = My.ArcMap.Document.ActiveView.GraphicsContainer.LocateElements(pPnt, My.Globals.Functions.ConvertUnits(15, My.ArcMap.Document.FocusMap.MapUnits))

            If pEnumElem IsNot Nothing Then


                pElem = pEnumElem.Next
                If pElem IsNot Nothing Then
                    pElemProp = pElem
                    If pElemProp.CustomProperty IsNot Nothing Then
                        If pElemProp.CustomProperty.ToString.Contains("CIPTools") Then
                            Dim pStrTag() As String = pElemProp.CustomProperty.ToString.Split(":")
                            If pStrTag.Length = 3 Then
                                FindRow(pStrTag(1), pStrTag(2)).Selected = True

                            End If

                        End If
                    End If
                    pElemProp = Nothing

                End If
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: HighlightAtLocation" & vbCrLf & ex.Message)
        Finally

            pEnumElem = Nothing
            pElem = Nothing
            pElemProp = Nothing
        End Try
    End Sub
    Private Shared Function FindRow(ByVal LayerName As String, ByVal OID As String) As DataGridViewRow
        Try

            If s_dgCIP.Rows.Count = 0 Then Return Nothing
            For Each pRow As DataGridViewRow In s_dgCIP.Rows
                If pRow.Cells("ASSETTYP").Value.ToString = LayerName And pRow.Cells("OID").Value = OID Then
                    Return pRow

                End If
            Next
            Return Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: FindRow" & vbCrLf & ex.Message)
            Return Nothing
        End Try
    End Function
    Private Shared Function GetCIPWorkspace() As IWorkspace
        Try
            If My.Globals.Variables.v_CIPLayerOver Is Nothing Then Return Nothing
            If My.Globals.Variables.v_CIPLayerOver.FeatureClass Is Nothing Then Return Nothing

            Dim pDataset As IDataset = My.Globals.Variables.v_CIPLayerOver
            GetCIPWorkspace = pDataset.Workspace
            '  My.Globals.Variables.v_CIPLayerOver = Nothing
            pDataset = Nothing
            Return GetCIPWorkspace
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: GetCIPWorkspace" & vbCrLf & ex.Message)
            Return Nothing
        End Try


    End Function
    Private Shared Function CheckEditingWorkspace(ByVal WorkSpace As IWorkspace) As Boolean
        Try

            If WorkSpace Is Nothing Then Return False

            Return My.Globals.Variables.v_Editor.EditWorkspace.Equals(WorkSpace)


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: CheckEditingWorkspace" & vbCrLf & ex.Message)
            Return False
        End Try


    End Function
    Private Shared Sub loadDefLayersCboBox()
        Try

            If My.Globals.Variables.v_CIPTableDef Is Nothing Then
                setDefLayersToDropdown(Nothing)
                Return
            End If
            Dim pQFilt As IQueryFilter = New QueryFilter
            pQFilt.WhereClause = My.Globals.Constants.c_CIPDefActiveField & " = " & "'Yes'" & " OR " & My.Globals.Constants.c_CIPDefActiveField & " is null"

            Dim pCurs As ICursor = My.Globals.Variables.v_CIPTableDef.Search(pQFilt, True)
            If pCurs Is Nothing Then Return
            Dim pRow As IRow
            pRow = pCurs.NextRow
            Dim pAr As ArrayList = New ArrayList
            Do Until pRow Is Nothing
                Dim pFL As IFeatureLayer = My.Globals.Functions.FindLayer(pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)))
                If pFL IsNot Nothing Then
                    If pFL.FeatureClass IsNot Nothing Then
                        pAr.Add(New layerAndTypes(pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)), pFL.FeatureClass.ShapeType))
                    End If


                End If
                pRow = pCurs.NextRow
                pFL = Nothing
            Loop

            setDefLayersToDropdown(pAr)
            Marshal.ReleaseComObject(pCurs)
            pCurs = Nothing


            pQFilt = Nothing
            pAr = Nothing
            pRow = Nothing


        Catch ex As Exception
            If Not ex.Message.Contains("COM object") Then

                MsgBox("Error in the Costing Tools - CIPProjectWindow: loadDefLayersCboBox" & vbCrLf & ex.Message)
            End If

        End Try
    End Sub
    Private Shared Function ActionDomain(ByVal subTypeVal As Integer, ByVal pCostTbl As ITable) As IDomain
        'Dim pDefTbl As ITable = My.Globals.Functions.findTable(my.Globals.Constants.c_CIPCostTableName, my.ArcMap.Document.FocusMap )
        'If pDefTbl Is Nothing Then
        '    cboAction.DataSource = Nothing
        '    Return Nothing

        'End If
        Try


            Dim pSubType As ISubtypes = pCostTbl
            Dim pDom As ICodedValueDomain
            Dim pFl As IField
            pFl = pCostTbl.Fields.Field(pCostTbl.Fields.FindField(My.Globals.Constants.c_CIPCostActionField))

            If pSubType.HasSubtype Then

                pDom = pSubType.Domain(subTypeVal, pFl.Name)
            Else


                pDom = pFl.Domain
            End If

            If pDom Is Nothing Then
                MsgBox("Action Domains is missing")
                Return Nothing

            End If

            pFl = Nothing

            pSubType = Nothing

            Return pDom
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: ActionDomain" & vbCrLf & ex.Message)
            Return Nothing
        End Try
    End Function


    Private Shared Sub loadActionCboBox()
        Try
            If s_cboStrategy.Text = "" Then Return
            If s_cboStrategy.DataSource Is Nothing Then Return

            If My.Globals.Variables.v_CIPTableCost Is Nothing Then

                Return

            End If

            s_cboAction.DisplayMember = "Display"
            s_cboAction.ValueMember = "Value"

            ' s_cboAction.DataSource = My.Globals.Functions.DomainToList(ActionDomain(CType(s_cboStrategy.SelectedValue, My.Globals.Functions.DomSubList).Value, My.Globals.Variables.v_CIPTableCost))
            s_cboAction.DataSource = My.Globals.Functions.DomainToList(ActionDomain(s_cboStrategy.SelectedValue, My.Globals.Variables.v_CIPTableCost))

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: loadActionCboBox" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub loadStrategyCboBox()
        Try

            If My.Globals.Variables.v_CIPTableCost Is Nothing Then
                s_cboStrategy.DataSource = Nothing
                Return
            End If
            Dim pSubType As ISubtypes = My.Globals.Variables.v_CIPTableCost
            Dim pLst As ArrayList = My.Globals.Functions.SubtypeToList(pSubType)

            If pLst Is Nothing Then
                MsgBox("The Subtype list for the cost table is empty")
            End If
            If pLst.Count = 0 Then
                MsgBox("The Subtype list for the cost table is empty")
            End If
            'RemoveHandler 
            s_cboStrategy.DisplayMember = "Display"
            s_cboStrategy.ValueMember = "Value"
            s_cboStrategy.DataSource = pLst



            pLst = Nothing
            pSubType = Nothing

        Catch ex As Exception
            If Not ex.Message.Contains("COM object") Then

                MsgBox("Error in the Costing Tools - CIPProjectWindow: loadStrategyCboBox" & vbCrLf & ex.Message)
            End If

        End Try
    End Sub
    Private Shared Function AddRecordFromGraphic(ByVal geo As IGeometry, ByVal AddToGraphicLayer As Boolean, ByVal ConfigLayerName As String, Optional ByVal oid As Integer = -9999999) As Boolean
        Try

            If geo Is Nothing Then Return False
            Dim pFl As IFeatureLayer = My.Globals.Functions.FindLayer(ConfigLayerName)
            If pFl Is Nothing Then Return False
            Dim pDS As IDataset = pFl.FeatureClass
            Dim pFCName As String = My.Globals.Functions.getClassName(pDS)

            Dim pTblCursor As ICursor
            Dim pDefRow As IRow
            Dim pCostRow As IRow
            Dim pQFilt As IQueryFilter

            Dim strMultiField As String = ""
            Dim strLenField As String = ""


            Dim dblSourceCost As Double = 0.0
            Dim dblSourceAddCost As Double = 0.0
            Dim dblLength As Double = 0.0
            Dim strNotes As String = ""
            Dim dblTotalCost As Double = 0.0
            Dim dblMulti As Double = 1.0


            Dim strFiltVal1 As String = ""
            Dim strFiltVal2 As String = ""

            Dim strFiltField1 As String = ""
            Dim strFiltField2 As String = ""

            Dim strDefVal1 As String = "", strDefVal2 As String = ""

            If My.Globals.Variables.v_CIPTableCost Is Nothing Then
                MsgBox("The CIP Cost table cannot be found, exiting")
                Return False

            End If

            If My.Globals.Variables.v_CIPTableDef Is Nothing Then
                MsgBox("The CIP Definition table cannot be found, exiting")

                Return False

            End If

            pQFilt = New QueryFilter
            pQFilt.WhereClause = "(" & My.Globals.Constants.c_CIPDefNameField & " = '" & ConfigLayerName & "' OR " & _
                     My.Globals.Constants.c_CIPDefNameField & " = '" & pFCName & "')" & _
                     " AND (" & My.Globals.Constants.c_CIPDefActiveField & " = " & "'Yes'" & " OR " & My.Globals.Constants.c_CIPDefActiveField & " is null)"

            pTblCursor = My.Globals.Variables.v_CIPTableDef.Search(pQFilt, True)
            pDefRow = pTblCursor.NextRow
            If pDefRow Is Nothing Then
                pQFilt = Nothing

                Marshal.ReleaseComObject(pTblCursor)
                pTblCursor = Nothing
                Return False
            End If
            Dim pSQL As String = ""



            If My.Globals.Constants.c_CIPDefFiltField1 <> "" Then
                If pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1) > 0 Then
                    If pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot Nothing Then
                        If pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) IsNot DBNull.Value Then
                            strFiltField1 = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))
                        End If
                    End If
                End If


            End If
            If My.Globals.Constants.c_CIPDefFiltField2 <> "" Then
                If pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2) > 0 Then
                    If pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot Nothing Then
                        If pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) IsNot DBNull.Value Then
                            strFiltField2 = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))
                        End If
                    End If
                End If


            End If


            

            If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) Is Nothing Then
                If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) Is DBNull.Value Then
                    If Not Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))) = "" Then
                        If pFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))) < 0 Then
                            MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                            Return False
                        Else

                            strMultiField = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))

                        End If
                    End If
                End If
            End If


            If strFiltField1 <> "" Then
                If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField1)).DefaultValue IsNot Nothing Then
                    If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField1)).DefaultValue IsNot DBNull.Value Then
                        strDefVal1 = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField1)).DefaultValue
                    End If

                End If

            End If

            If strFiltField2 <> "" Then
                If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField2)).DefaultValue IsNot Nothing Then
                    If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField2)).DefaultValue IsNot DBNull.Value Then

                        strDefVal2 = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField2)).DefaultValue
                    End If

                End If

            End If






            ' MsgBox("Do sketchs have a strategy of new")

            pCostRow = CheckForCostFeat(ConfigLayerName, pFCName, s_cboStrategy.SelectedValue, s_cboAction.Text, s_cboAction.SelectedValue, strDefVal1, strDefVal2)
            Select Case geo.GeometryType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    Try

                        dblLength = CType(geo, ICurve).Length

                    Catch ex As Exception


                    End Try
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    Try

                        dblLength = Math.Abs(CType(geo, IArea).Area)


                    Catch ex As Exception


                    End Try
            End Select



            If pCostRow IsNot Nothing Then




                If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) Is Nothing Then
                    If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) Is DBNull.Value Then
                        dblSourceCost = pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField))
                    End If
                End If
                If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) Is Nothing Then
                    If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) Is DBNull.Value Then
                        dblSourceAddCost = pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField))
                    End If
                End If



                Select Case geo.GeometryType
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                        dblTotalCost = (dblMulti * dblSourceCost) + dblSourceAddCost
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                        dblTotalCost = (dblLength * dblSourceCost * dblMulti) + dblSourceAddCost
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                        dblTotalCost = (dblMulti * dblLength * dblSourceCost) + dblSourceAddCost
                End Select
                'dblTotalCost = dblMulti * dblTotalCost



            End If
            If strFiltField1 <> "" Then
                If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField1)).Domain IsNot Nothing Then
                    Dim pDom As IDomain = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField1)).Domain
                    If pDom.Type = esriDomainType.esriDTCodedValue Then

                        strDefVal1 = My.Globals.Functions.GetDomainDisplay(strDefVal1, pDom)
                    End If
                End If
            End If

            If strFiltField2 <> "" Then
                If pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField2)).Domain IsNot Nothing Then
                    Dim pDom As IDomain = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(strFiltField2)).Domain
                    If pDom.Type = esriDomainType.esriDTCodedValue Then

                        strDefVal2 = My.Globals.Functions.GetDomainDisplay(strDefVal2, pDom)
                    End If
                End If

            End If

            If oid = -9999999 Then
                oid = My.Globals.Variables.v_intSketchID
                My.Globals.Variables.v_intSketchID = My.Globals.Variables.v_intSketchID - 1
            End If
            loadRecord(geo, ConfigLayerName, pFCName, "Sketch:" & oid, dblSourceCost, dblSourceAddCost, dblLength, dblTotalCost, strFiltVal1, strFiltVal2, strFiltField1, strFiltField2, oid, strDefVal1, strDefVal2, s_cboStrategy.Text, s_cboAction.Text, dblMulti, strNotes)





            setProjectCostAndTotal()

            My.ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, Nothing, My.ArcMap.Document.ActiveView.Extent)

            pFl = Nothing

            pQFilt = Nothing
            pCostRow = Nothing
            pDefRow = Nothing

            Marshal.ReleaseComObject(pTblCursor)
            pTblCursor = Nothing

            Return True
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: AddRecordFromGraphic" & vbCrLf & ex.Message)

        End Try
    End Function
    Private Shared Sub loadGraphicsToGrid()
        Try


            Dim pElem As IElement
            Dim pElProp As IElementProperties


            Try



                My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                Do Until pElem Is Nothing
                    pElProp = pElem
                    If pElProp.CustomProperty IsNot Nothing Then
                        If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                            Dim strProp() As String = pElProp.CustomProperty.ToString.Split(":")

                            '  Dim strEl As String = pElProp.CustomProperty
                            AddRecordFromGraphic(pElem.Geometry, False, strProp(1), strProp(2))

                        End If
                    End If
                    pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                Loop



            Catch ex As Exception
            Finally

            End Try

            pElem = Nothing
            pElProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: loadGraphicsToGrid" & vbCrLf & ex.Message)


        End Try
    End Sub

    Private Shared Function Split_At_Point(ByVal pSplitPoint As IPoint, ByVal pPline As IPolyline) As IPolyline
        Try

            Dim boolSplitHappened As Boolean
            Dim lngNewPartIndex As Long
            Dim lngNewSegmentIndex As Long


            pPline.SplitAtPoint(pSplitPoint, False, True, boolSplitHappened, lngNewPartIndex, lngNewSegmentIndex)

            Return pPline
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: Split_At_Point" & vbCrLf & ex.Message, MsgBoxStyle.DefaultButton1, "Error")
            Return Nothing

        End Try

    End Function

    Private Shared Sub splitSegmentAtLocation(ByVal pPnt As IPoint, ByVal Split As Boolean)
        Dim pNewRow As DataGridViewRow
        Dim pNewGeoCol As IGeometryCollection = Nothing
        Dim pElem As IElement = Nothing

        Dim hitDistance As System.Double = 0
        Dim hitPartIndex As System.Int32 = 0
        Dim hitPoint As IPoint = New ESRI.ArcGIS.Geometry.Point
        Dim hitSegmentIndex As System.Int32 = 0
        Dim rightSide As System.Boolean = Nothing

        Dim hitTest As ESRI.ArcGIS.Geometry.IHitTest = Nothing
        Dim foundGeometry As System.Boolean = False

        Dim pPolyLine1 As IPolyline
        Dim pPolyLine2 As IPolyline

        Try


            If s_dgCIP.SelectedRows.Count = 0 Then Return
            If s_dgCIP.SelectedRows(0).Cells("Geotype").Value <> "Polyline" Then
                MsgBox("Only lines are supported")
                Return
            End If

            Dim strTag As String = "CIPTools:" & s_dgCIP.SelectedRows(0).Cells("ASSETTYP").Value & ":" & s_dgCIP.SelectedRows(0).Cells("OID").Value

            pElem = getCIPGraphic(strTag)


            If pElem.Geometry Is Nothing Then Return
            hitTest = CType(pElem.Geometry, ESRI.ArcGIS.Geometry.IHitTest)
            foundGeometry = hitTest.HitTest(pPnt, My.Globals.Functions.ConvertFeetToMapUnits(30), ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary, hitPoint, hitDistance, hitPartIndex, hitSegmentIndex, rightSide)

            If foundGeometry = True Then
                Dim pGeoColl As IGeometryCollection

                pGeoColl = Split_At_Point(hitPoint, pElem.Geometry)
                If pGeoColl.GeometryCount = 1 Then
                    MsgBox("The split point was at the end of the line, no split occured", , "Cost Estimating Tools")
                ElseIf pGeoColl.GeometryCount = 2 Then
                    'UnSelectRow(strTag)

                    Dim pTag() As String = strTag.Split(":")
                    Dim strExistingSplitTag As String = ""
                    If pTag.Length = 4 Then
                        strExistingSplitTag = pTag(3)
                        '   strTag = pTag(0) & ":" & pTag(1) & ":" & pTag(2)

                    End If


                    pNewGeoCol = New Polyline
                    pNewGeoCol.AddGeometry(pGeoColl.Geometry(0))
                    pPolyLine1 = pNewGeoCol

                    pNewGeoCol = New Polyline
                    pNewGeoCol.AddGeometry(pGeoColl.Geometry(1))
                    pPolyLine2 = pNewGeoCol

                    RemoveGraphic(strTag)



                    If Split Then

                        '1st half

                        AddGraphic(pPolyLine1, pTag(1) & ":" & pTag(2) & ":" & strExistingSplitTag & "a")

                        s_dgCIP.SelectedRows(0).Cells("OID").Value = pTag(2) & ":" & strExistingSplitTag & "a"

                        If s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue.ToString = "" Then
                            s_dgCIP.SelectedRows(0).Cells("NOTES").Value = "User Reshaped"
                        Else
                            s_dgCIP.SelectedRows(0).Cells("NOTES").Value = s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue.ToString & " | " & "User Reshaped"
                        End If

                        s_dgCIP.SelectedRows(0).Cells("LENGTH").Value = Format(pPolyLine1.Length, "#,###.00")

                        SetRowsTotal(s_dgCIP.SelectedRows(0).Index)


                        '2nd half
                        AddGraphic(pPolyLine2, pTag(1) & ":" & pTag(2) & ":" & strExistingSplitTag & "b")

                        pNewRow = CopyRecord(s_dgCIP.SelectedRows(0).Index)
                        pNewRow.Cells("OID").Value = pTag(2) & ":" & strExistingSplitTag & "b"
                        pNewRow.Cells("LENGTH").Value = Format(pPolyLine2.Length, "#,###.00")

                        SetRowsTotal(pNewRow.Index)

                        SelectRow(pTag(0) & ":" & pTag(1) & ":" & pTag(2) & ":" & strExistingSplitTag & "a")

                    Else

                        If pPolyLine1.Length > pPolyLine2.Length Then
                            If strExistingSplitTag <> "" Then
                                strExistingSplitTag = ":" & strExistingSplitTag
                            End If
                            AddGraphic(pPolyLine1, pTag(1) & ":" & pTag(2) & strExistingSplitTag)

                            '1st half
                            s_dgCIP.SelectedRows(0).Cells("OID").Value = pTag(2) & strExistingSplitTag ' & "a"

                            If s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue.ToString = "" Then
                                s_dgCIP.SelectedRows(0).Cells("NOTES").Value = "User Reshaped"
                            Else
                                s_dgCIP.SelectedRows(0).Cells("NOTES").Value = s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue & " | " & "User Reshaped"
                            End If

                            s_dgCIP.SelectedRows(0).Cells("LENGTH").Value = Format(pPolyLine1.Length, "#,###.00")

                            SetRowsTotal(s_dgCIP.SelectedRows(0).Index)

                        Else
                            If strExistingSplitTag <> "" Then
                                strExistingSplitTag = ":" & strExistingSplitTag
                            End If
                            AddGraphic(pPolyLine2, pTag(1) & ":" & pTag(2) & strExistingSplitTag)

                            '1st half
                            s_dgCIP.SelectedRows(0).Cells("OID").Value = pTag(2) & strExistingSplitTag ' & "a"

                            If s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue.ToString = "" Then
                                s_dgCIP.SelectedRows(0).Cells("NOTES").Value = "User Reshaped"
                            Else
                                s_dgCIP.SelectedRows(0).Cells("NOTES").Value = s_dgCIP.SelectedRows(0).Cells("NOTES").FormattedValue & " | " & "User Reshaped"
                            End If

                            s_dgCIP.SelectedRows(0).Cells("LENGTH").Value = Format(pPolyLine2.Length, "#,###.00")

                            SetRowsTotal(s_dgCIP.SelectedRows(0).Index)

                        End If
                        SelectRow(pTag(0) & ":" & pTag(1) & ":" & pTag(2) & strExistingSplitTag)

                    End If




                    setProjectCostAndTotal()


                    My.ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, hitTest, My.ArcMap.Document.ActiveView.Extent)

                Else
                    MsgBox("Unsupported Split")
                    Return
                End If


            Else
                MsgBox("A location was not found on the line, please click closer", , "ArcGIS Cost Estimate Tools")

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: splitSegmentAtLocation" & vbCrLf & ex.Message, MsgBoxStyle.DefaultButton1, "Error")
        Finally
            pPolyLine1 = Nothing
            pPolyLine2 = Nothing
            pNewRow = Nothing
            pNewGeoCol = Nothing
            pElem = Nothing
            hitPoint = Nothing
            hitTest = Nothing


        End Try

    End Sub
#End Region
#Region "Events"

    Private Shared Sub cboStrategy_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStrategy.SelectedIndexChanged
        Try
            If s_cboStrategy Is Nothing Then Return

            loadActionCboBox()
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: cboStrategy_SelectedIndexChanged" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub gpBxCIPPrj_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpBxCIPPrj.Resize
        Try

            If s_gpBxCIPPrj Is Nothing Then Return

            If s_gpBxCIPPrj.Visible = False Then Exit Sub

            ShuffleControls(False)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: gpBxCIPPrj_Resize" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnRemoveInv_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveInv.Click
        Try
            If s_lstInventory Is Nothing Then Return


            If s_lstInventory.SelectedItem IsNot Nothing Then
                s_lstInventory.Items.Remove(s_lstInventory.SelectedItem)
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnRemoveInv_Click" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub lblTotalCost_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblTotalCost.TextChanged
        Try

            If s_lblTotalCost Is Nothing Then Return

            Dim g As Graphics
            Dim s As SizeF

            g = s_lblTotalCost.CreateGraphics
            s = g.MeasureString(s_lblTotalCost.Text, s_lblTotalCost.Font)
            s_TotalDisplay.Left = s_lblTotalCost.Left + s.Width + 10
            'lblLength.Left = lblTotalCost.Left + s.Width + 10
            'lblTotLength.Left = lblLength.Left + lblLength.Width

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: lblTotalCost_TextChanged" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnSelectPrj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelectPrj.Click
        Try

            If s_btnSelectPrj Is Nothing Then Return

            If s_btnSelectPrj.Checked = True Then


                My.ArcMap.Application.CurrentTool = ArcGIS4LocalGovernment.CostEstimatingExtension.GetSelectPrjTool

            Else
                My.ArcMap.Application.CurrentTool = Nothing
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnSelectPrj_clcik" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnStopEditing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStopEditing.Click
        If My.Globals.Variables.v_Editor Is Nothing Then Return
        If My.Globals.Variables.v_Editor.EditState <> esriEditState.esriStateNotEditing Then
            My.Globals.Variables.v_Editor.StopEditing(True)

        End If
    End Sub
    Private Shared Sub gpBxCIPCostingLayers_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpBxCIPCostingLayers.Resize
        If s_gpBxCIPCostingLayers Is Nothing Then Return
        Dim pTop, pleft As Integer
        pTop = 25
        pleft = 15
        Dim g As Graphics
        Dim s As SizeF

        For Each cnt As Control In s_gpBxCIPCostingLayers.Controls
            cnt.Left = pleft
            cnt.Top = pTop
            pTop = pTop + cnt.Height + 5

            g = cnt.CreateGraphics
            s = g.MeasureString(cnt.Text, cnt.Font)

            If s.Width > 250 Then
                Do Until s.Width < 240
                    cnt.Text = cnt.Text.Substring(0, cnt.Text.Length - 1)

                    s = g.MeasureString(cnt.Text, cnt.Font)

                Loop
                cnt.Text = cnt.Text + ".."

                cnt.Width = 260
                cnt.AutoSize = False
            Else
                cnt.AutoSize = True

            End If

            If pTop + cnt.Height >= s_gpBxCIPCostingLayers.Height - 10 Then
                pTop = 25
                pleft = pleft + 260

            End If
        Next
        s = Nothing
        g = Nothing

    End Sub
    Private Shared Sub layerChecked(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim pMouse As IMouseCursor
            pMouse = New MouseCursor
            pMouse.SetCursor(2)


            If My.Globals.Variables.v_Editor.EditState = esriEditState.esriStateNotEditing Then

                MsgBox("You must be editing to toggle cost status")
                RemoveHandler CType(sender, CheckBox).CheckedChanged, AddressOf layerChecked

                CType(sender, CheckBox).Checked = Not CType(sender, CheckBox).Checked
                AddHandler CType(sender, CheckBox).CheckedChanged, AddressOf layerChecked

                Return
            End If

            If My.Globals.Variables.v_CIPTableDef Is Nothing Then
                Return
            End If
            Dim pQFilt As IQueryFilter = New QueryFilter
            pQFilt.WhereClause = My.Globals.Constants.c_CIPDefNameField & " = '" & CType(sender, CheckBox).Tag & "'"


            Dim pCurs As ICursor = My.Globals.Variables.v_CIPTableDef.Search(pQFilt, False)
            If pCurs Is Nothing Then Return
            Dim pRow As IRow
            pRow = pCurs.NextRow
            If pRow Is Nothing Then
                MsgBox("Cost Layer not found, If you removed the layer, please add it back")
            Else
                My.Globals.Variables.v_Editor.StartOperation()
                If CType(sender, CheckBox).Checked = False Then
                    pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField)) = "No"
                Else
                    pRow.Value(pRow.Fields.FindField(My.Globals.Constants.c_CIPDefActiveField)) = "Yes"
                End If
                pRow.Store()
                Marshal.ReleaseComObject(pRow)
                My.Globals.Variables.v_Editor.StopOperation("Cost Change")

            End If

            Marshal.ReleaseComObject(pCurs)
            pCurs = Nothing
            pQFilt = Nothing

            pRow = Nothing
            Marshal.ReleaseComObject(pMouse)

            pMouse = Nothing
            loadDefLayersCboBox()

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: LayerCheck" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnSavePrj_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSavePrj.Click
        Try
            If s_btnSavePrj Is Nothing Then Return

            CreateCIPProject() 'txtPrjName.Text, lblTotalCost.Text, dtTimeStart.Value.ToShortDateString, dtTimeEnd.Value.ToShortDateString, cboCIPStat.Text, cboCIPStim.Text, cboEnginner.Text, cboPrjMan.Text, txtNotes.Text)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnSavePrj_Click" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            ResetControls(False)
            '' CostEstimatingExtension.DeactivateCIPTools(False)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow:  btnClear_Click" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub rdoBtnShowCan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoBtnShowCan.CheckedChanged
        Try
            If s_gpBxCIPCan Is Nothing Then Return

            If CType(sender, RadioButton).Checked = True Then
                s_gpBxCIPCan.Visible = True
                s_gpBxCIPCostingLayers.Visible = False
                s_gpBxCIPPrj.Visible = False
                s_gpBxCIPInven.Visible = False
                Call gpBxCIPCostingLayers_Resize(Nothing, Nothing)

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: rdoBtnShowCan_CheckedChanged" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub rdoBtnShowDetails_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoBtnShowDetails.CheckedChanged
        Try
            If CType(sender, RadioButton).Checked = True Then
                s_gpBxCIPCan.Visible = False
                s_gpBxCIPCostingLayers.Visible = False
                s_gpBxCIPPrj.Visible = True
                s_gpBxCIPInven.Visible = False
                Call gpBxCIPCostingLayers_Resize(Nothing, Nothing)

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: rdoBtnShowDetails_CheckedChanged" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub rdoShowInv_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoShowInv.CheckedChanged

        Try
            If CType(sender, RadioButton).Checked = True Then
                s_gpBxCIPCan.Visible = False
                s_gpBxCIPCostingLayers.Visible = False
                s_gpBxCIPPrj.Visible = False
                s_gpBxCIPInven.Visible = True
                Call gpBxCIPCostingLayers_Resize(Nothing, Nothing)

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: rdoShowInv_CheckedChanged" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub rdoBtnShowLayers_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoBtnShowLayers.CheckedChanged
        Try


            If CType(sender, RadioButton).Checked = True Then
                s_gpBxCIPCan.Visible = False
                s_gpBxCIPCostingLayers.Visible = True
                s_gpBxCIPPrj.Visible = False

                loadLayersToPanel()
                s_gpBxCIPInven.Visible = False

                Call gpBxCIPCostingLayers_Resize(Nothing, Nothing)
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: rdoBtnShowLayers_CheckedChanged" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub btnAddInv_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddInv.Click
        Try
            s_lstInventory.Items.Add(s_cboCIPInvTypes.Text & ":" & s_numCIPInvCount.Value)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnAddInv_Click" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Function getCIPGraphic(ByVal Tag As String) As IElement
        Try

            Dim pElem As IElement
            Dim pElProp As IElementProperties

            Try

                My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()


                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                Do Until pElem Is Nothing
                    pElProp = pElem
                    If pElProp.CustomProperty IsNot Nothing Then
                        If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                            Dim strEl As String = pElProp.CustomProperty
                            If strEl = Tag Then

                                Return pElem
                            End If
                        End If
                    End If

                    pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                Loop

            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: getCIPGrpahic" & vbCrLf & ex.Message)

            Finally
                pElProp = Nothing
            End Try



            pElem = Nothing
            pElProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: getCIPGrpahic" & vbCrLf & ex.Message)


        End Try
        Return Nothing

    End Function
    Private Shared Sub RowChanged(ByVal Tag As String)

        Try
            Dim pElem As IElement
            Dim pElProp As IElementProperties
            Dim pNewGeo As IGeometry = Nothing, pOldGeo As IGeometry = Nothing


            Try


                My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                Do Until pElem Is Nothing
                    pElProp = pElem
                    If pElProp.CustomProperty IsNot Nothing Then
                        If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                            Dim strEl As String = pElProp.CustomProperty
                            If strEl = Tag Then


                                If TypeOf pElem Is IPolygonElement Then

                                    Dim pFSElem As IFillShapeElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PolygonHighlightGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is ILineElement Then


                                    Dim pFSElem As ILineElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_LineHighlightGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is IMarkerElement Then
                                    Dim pFSElem As IMarkerElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PointHighlightGraphicSymbol
                                    pFSElem = Nothing

                                End If
                                pNewGeo = pElem.Geometry
                            End If
                            If strEl = My.Globals.Variables.v_LastSelection Then
                                If TypeOf pElem Is IPolygonElement Then

                                    Dim pFSElem As IFillShapeElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PolygonGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is ILineElement Then


                                    Dim pFSElem As ILineElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_LineGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is IMarkerElement Then
                                    Dim pFSElem As IMarkerElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PointGraphicSymbol
                                    pFSElem = Nothing

                                End If
                                pOldGeo = pElem.Geometry

                            End If
                        End If
                    End If

                    If pNewGeo IsNot Nothing And pOldGeo IsNot Nothing Then Exit Do


                    pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                Loop

                My.Globals.Variables.v_LastSelection = Tag
                'If pNewGeo IsNot Nothing Then
                '    m_pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pNewGeo, m_pMxDoc.ActiveView.Extent)
                'End If
                'If pOldGeo IsNot Nothing Then
                '    m_pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pOldGeo, m_pMxDoc.ActiveView.Extent)
                'End If
                My.ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pOldGeo, My.ArcMap.Document.ActiveView.Extent)
            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: RowChanged" & vbCrLf & ex.Message)

            Finally

            End Try
            pNewGeo = Nothing
            pOldGeo = Nothing



            pElem = Nothing
            pElProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: RowChanged" & vbCrLf & ex.Message)


        End Try
    End Sub
    Private Shared Sub UnSelectRow(ByVal Tag As String)

        Try

            Dim pElem As IElement
            Dim pElProp As IElementProperties
            Dim pNewGeo As IGeometry = Nothing, pOldGeo As IGeometry = Nothing


            Try
                If My.ArcMap.Document Is Nothing Then Return
                If My.ArcMap.Document.ActiveView Is Nothing Then Return
                If My.ArcMap.Document.ActiveView.GraphicsContainer Is Nothing Then Return
                My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                Do Until pElem Is Nothing
                    pElProp = pElem
                    If pElProp IsNot Nothing Then


                        If pElProp.CustomProperty IsNot Nothing Then
                            If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                                Dim strEl As String = pElProp.CustomProperty
                                If strEl = Tag Then

                                    My.Globals.Variables.v_LastSelection = ""

                                    If TypeOf pElem Is IPolygonElement Then

                                        Dim pFSElem As IFillShapeElement
                                        pFSElem = pElem
                                        pFSElem.Symbol = My.Globals.Variables.v_PolygonGraphicSymbol
                                        pFSElem = Nothing
                                    ElseIf TypeOf pElem Is ILineElement Then


                                        Dim pFSElem As ILineElement
                                        pFSElem = pElem
                                        pFSElem.Symbol = My.Globals.Variables.v_LineGraphicSymbol
                                        pFSElem = Nothing
                                    ElseIf TypeOf pElem Is IMarkerElement Then
                                        Dim pFSElem As IMarkerElement
                                        pFSElem = pElem
                                        pFSElem.Symbol = My.Globals.Variables.v_PointGraphicSymbol
                                        pFSElem = Nothing

                                    End If
                                    'm_pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pOldGeo, My.Globals.Variables.v_pMxDoc.ActiveView.Extent)
                                    Return

                                End If

                            End If
                        End If

                    End If
                    pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                Loop



            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: UnRowChanged" & vbCrLf & ex.Message)

            Finally

            End Try
            pNewGeo = Nothing
            pOldGeo = Nothing




            pElem = Nothing
            pElProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: unSelectRow" & vbCrLf & ex.Message)


        End Try
    End Sub
    Private Shared Sub SelectRow(ByVal Tag As String)

        Try

            Dim pElem As IElement
            Dim pElProp As IElementProperties
            Dim pNewGeo As IGeometry = Nothing, pOldGeo As IGeometry = Nothing


            Try



                My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                Do Until pElem Is Nothing
                    pElProp = pElem
                    If pElProp.CustomProperty IsNot Nothing Then
                        If pElProp.CustomProperty.ToString.Contains("CIPTools:") Then
                            Dim strEl As String = pElProp.CustomProperty
                            If strEl = Tag Then



                                If TypeOf pElem Is IPolygonElement Then

                                    Dim pFSElem As IFillShapeElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PolygonHighlightGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is ILineElement Then


                                    Dim pFSElem As ILineElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_LineHighlightGraphicSymbol
                                    pFSElem = Nothing
                                ElseIf TypeOf pElem Is IMarkerElement Then
                                    Dim pFSElem As IMarkerElement
                                    pFSElem = pElem
                                    pFSElem.Symbol = My.Globals.Variables.v_PointHighlightGraphicSymbol
                                    pFSElem = Nothing

                                End If
                                My.Globals.Variables.v_LastSelection = Tag
                                '             m_pMxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, pOldGeo, m_pMxDoc.ActiveView.Extent)
                                Return

                            End If

                        End If
                    End If


                    pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                Loop



            Catch ex As Exception
                MsgBox("Error in the Costing Tools - CIPProjectWindow: SelectRow" & vbCrLf & ex.Message)

            Finally

            End Try
            pNewGeo = Nothing
            pOldGeo = Nothing


            pElem = Nothing
            pElProp = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: SelectRow" & vbCrLf & ex.Message)


        End Try
    End Sub

    Private Shared Sub CIPProjectWindow_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            If s_gpBxSwitch Is Nothing Then Return
            s_gpBxSwitch.Width = 85

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: CIPProjectWindow_Resize" & vbCrLf & ex.Message)

        End Try


    End Sub
    Private Shared Sub txtPrjName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            EnableSavePrj()
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: txtPrjName_TextChanged" & vbCrLf & ex.Message)

        End Try

    End Sub
    Private Shared Sub dgCIP_RowsAdded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs)
        Try
            If s_dgCIP Is Nothing Then Return
            If s_dgCIP.Rows.Count > 0 Then
                If My.Globals.Variables.v_Editor.EditState = esriEditState.esriStateEditing Then
                    s_btnSavePrj.Enabled = True
                Else
                    s_btnSavePrj.Enabled = False
                End If

            Else
                s_btnSavePrj.Enabled = False
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_dgCIP_RowsAdded" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub dgCIP_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Try
            If s_dgCIP Is Nothing Then Return
            RemoveControl(True)
            If e.RowIndex = -1 Then Return

            If s_dgCIP.Columns(e.ColumnIndex).Name = "proFiltVal1" Then
                If UCase(s_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value) <> UCase(My.Globals.Variables.v_CIPAbandonValue) Then
                    'Or s_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value = "Rehabilitate" Or  s_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value = "Proposed" Then

                    If s_dgCIP.Rows(e.RowIndex).Cells("FiltFld1").FormattedValue.ToString = "" Then
                        addTextBox(e)
                    Else


                        Dim pFl As IFeatureLayer = My.Globals.Functions.FindLayer(s_dgCIP.Rows(e.RowIndex).Cells("ASSETTYP").FormattedValue.ToString)
                        Dim pFeat As IFeature
                        If s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Contains("-") Then
                            pFeat = Nothing
                        Else
                            If s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Contains(":") Then
                                pFeat = pFl.FeatureClass.GetFeature(s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Split(":")(0))
                            Else
                                pFeat = pFl.FeatureClass.GetFeature(s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString)
                            End If

                        End If

                        Dim pD As IDomain = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(s_dgCIP.Rows(e.RowIndex).Cells("FiltFld1").FormattedValue.ToString)).Domain

                        If pD Is Nothing Then
                            addTextBox(e)
                        Else
                            If TypeOf pD Is ICodedValueDomain Then
                                Dim pCdV As ICodedValueDomain = pD

                                addComboBox(e, My.Globals.Functions.DomainToList(pCdV))
                                pCdV = Nothing

                            Else
                                addTextBox(e)
                            End If

                        End If

                    End If
                End If


            ElseIf s_dgCIP.Columns(e.ColumnIndex).Name = "proFiltVal2" Then
                If UCase(s_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value) <> UCase(My.Globals.Variables.v_CIPAbandonValue) Then 's_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value = "Replacement" Or s_dgCIP.Rows(e.RowIndex).Cells("Strategy").Value = "Proposed" Then

                    If s_dgCIP.Rows(e.RowIndex).Cells("FiltFld2").FormattedValue.ToString = "" Then
                        addTextBox(e)
                    Else


                        Dim pFl As IFeatureLayer = My.Globals.Functions.FindLayer(s_dgCIP.Rows(e.RowIndex).Cells("ASSETTYP").FormattedValue.ToString)
                        Dim pFeat As IFeature
                        If s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Contains("-") Then
                            pFeat = Nothing
                        Else
                            If s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Contains(":") Then
                                pFeat = pFl.FeatureClass.GetFeature(s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString.Split(":")(0))
                            Else
                                pFeat = pFl.FeatureClass.GetFeature(s_dgCIP.Rows(e.RowIndex).Cells("OID").FormattedValue.ToString)
                            End If

                        End If


                        Dim pD As IDomain = pFl.FeatureClass.Fields.Field(pFl.FeatureClass.Fields.FindField(s_dgCIP.Rows(e.RowIndex).Cells("FiltFld2").FormattedValue.ToString)).Domain

                        If pD Is Nothing Then
                            addTextBox(e)
                        Else
                            If TypeOf pD Is ICodedValueDomain Then
                                Dim pCdV As ICodedValueDomain = pD

                                addComboBox(e, My.Globals.Functions.DomainToList(pCdV))
                                pCdV = Nothing

                            Else
                                addTextBox(e)
                            End If

                        End If

                    End If
                End If

            ElseIf s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "Action".ToUpper Then
                If My.Globals.Variables.v_CIPTableDef Is Nothing Then

                    Return

                End If

                Dim pSubVal As String = My.Globals.Functions.GetSubtypeValue(s_dgCIP.Rows(e.RowIndex).Cells("STRATEGY").FormattedValue.ToString, My.Globals.Variables.v_CIPTableCost)

                Dim pDom As IDomain = ActionDomain(pSubVal, My.Globals.Variables.v_CIPTableCost)

                addComboBox(e, My.Globals.Functions.DomainToList(pDom))
                pDom = Nothing

            ElseIf s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "Strategy".ToUpper Then


                addComboBox(e, s_cboStrategy.DataSource)


            ElseIf s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "NOTES" Or _
                   s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "MULTI" Or _
                   s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "COST" Or _
                   s_dgCIP.Columns(e.ColumnIndex).Name.ToUpper = "ADDCOST" Then
                addTextBox(e)
            End If


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_dgCIP_CellClick" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub comboClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            saveCombo(sender)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: comboClick" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub dgCIP_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs)
        Try
            If s_dgCIP Is Nothing Then Return
            RemoveControl(True)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_dgCIP_Scroll" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub dgCIP_DataGridKeyIntercept(ByVal e As Integer)
        Try
            If s_dgCIP Is Nothing Then Return

            If e = Keys.Escape Then
                RemoveControl(False)
            ElseIf e = Keys.Enter Then
                saveControl()
            ElseIf e = 34 Then
                addQuote(34)
            ElseIf e = 39 Then
                addQuote(39)
            ElseIf e = 96 Then
                addQuote(96)
            ElseIf e = 222 Then
                addQuote(222)
            ElseIf e = 46 Then
                addPeriod()
                'ElseIf e = 46 Then
                '    deleteRecord()


            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools -CIPProjectWindow:  s_dgCIP_DataGridKeyIntercept" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub dgCIP_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            If s_dgCIP Is Nothing Then Return

            Dim strTag As String

            If s_dgCIP.SelectedRows.Count > 0 Then
                strTag = "CIPTools:" & s_dgCIP.SelectedRows(0).Cells("ASSETTYP").Value & ":" & s_dgCIP.SelectedRows(0).Cells("OID").Value
                RowChanged(strTag)
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_dgCIP_SelectionChanged" & vbCrLf & ex.Message)

        End Try
    End Sub
    Private Shared Sub btnStartEditing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartEditing.Click
        Try
            If s_btnStartEditing Is Nothing Then Return

            Dim pMouse As IMouseCursor
            pMouse = New MouseCursor
            pMouse.SetCursor(2)
            Dim pWkSpace As IWorkspace = GetCIPWorkspace()
            If pWkSpace IsNot Nothing Then

                If My.Globals.Variables.v_Editor.EditState = esriEditState.esriStateNotEditing Then


                    My.Globals.Variables.v_Editor.StartEditing(pWkSpace)

                Else


                    If CheckEditingWorkspace(pWkSpace) Then

                    Else
                        MsgBox("A edit session is already active on another workspace")
                    End If
                End If
            Else
                MsgBox("The CIP layers are not present, cannot start editing")

            End If
            pWkSpace = Nothing
            pMouse = Nothing

        Catch ex As Exception

            'If (ex.Message.ToString().Contains("a lock")) Then

            MsgBox("The workspace could not be edited, it may be locked by another edit session.")

            'Else

            'MsgBox("Error in the Costing Tools - CIPProjectWindow: btnStartEditing_Click" & vbCrLf & ex.Message)
            'End If




        End Try
    End Sub
    Private Shared Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If s_btnSave Is Nothing Then Return

            Dim workspaceEdit As IWorkspaceEdit2 = My.Globals.Variables.v_Editor.EditWorkspace
            If workspaceEdit.IsInEditOperation Then
                workspaceEdit.StopEditOperation()
            End If
            My.Globals.Variables.v_Editor.StopEditing(True)
            My.Globals.Variables.v_Editor.StartEditing(workspaceEdit)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnSave_Click" & vbCrLf & ex.Message)
        End Try

    End Sub
    Private Shared Sub ctxMenu_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ctxMenu.ItemClicked
        Try
            If s_ctxMenu Is Nothing Then Return

            Select Case UCase(e.ClickedItem.Name)
                Case UCase("tlStDelete")
                    deleteRecord()
                Case UCase("tlStZoom")
                Case UCase("tlStFlash")

            End Select
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: ctxMenu_ItemClicked" & vbCrLf & ex.Message)
        End Try

    End Sub
    Private Shared Sub dgCIP_CellMouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs)
        Try

            If s_dgCIP Is Nothing Then Return

            If e.RowIndex >= 0 And e.ColumnIndex >= 0 And e.Button = MouseButtons.Right Then
                s_dgCIP.Rows(e.RowIndex).Selected = True

                Dim r As System.Drawing.Rectangle = s_dgCIP.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True)


                s_ctxMenu.Show(sender, r.Left + e.X, r.Top + e.Y)


            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_dgCIP_CellMouseClick" & vbCrLf & ex.Message)
        End Try


    End Sub
    Private Shared Sub btnSelectAssets_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAssets.Click
        Try
            If s_btnSelectAssets Is Nothing Then Return
            If s_btnSelectAssets.Checked = True Then
                My.ArcMap.Application.CurrentTool = ArcGIS4LocalGovernment.CostEstimatingExtension.GetSelectAssetTool

            Else
                My.ArcMap.Application.CurrentTool = Nothing
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: s_btnSelectAssets_Click" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click

        Try

            If s_btnSelect Is Nothing Then Return

            If s_btnSelect.Checked = True Then


                My.ArcMap.Application.CurrentTool = ArcGIS4LocalGovernment.CostEstimatingExtension.GetSelectCostedAssetTool

            Else
                My.ArcMap.Application.CurrentTool = Nothing
            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnSelect_Click" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub btnSketch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) 
        Try
            If s_btnSketch Is Nothing Then Return
         
                Dim pCmdItem As ICommandItem

            pCmdItem = My.Globals.Functions.GetCommand("ArcGIS4LocalGovernment_CreateAssetForGrid")
                If My.ArcMap.Application.CurrentTool IsNot pCmdItem Then
                My.ArcMap.Application.CurrentTool = pCmdItem

                pCmdItem = Nothing

                s_cboDefLayers.Enabled = False



            Else
                s_cboDefLayers.Enabled = True
                My.ArcMap.Application.CurrentTool = Nothing

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: btnSelect_Click" & vbCrLf & ex.Message)
        End Try
    End Sub
    Private Shared Sub ctxMenuTotals_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ctxMenuTotals.ItemClicked
        Try
            If s_ShowLength Is Nothing Then Return


            If e.ClickedItem Is s_ShowLength Then
                s_lblTotLength.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked
                s_lblLength.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked


            ElseIf e.ClickedItem Is s_ShowArea Then
                s_lblTotArea.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked
                s_lblArea.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked

            ElseIf e.ClickedItem Is s_ShowPoint Then
                s_lblTotPnt.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked
                s_lblPoint.Visible = Not CType(e.ClickedItem, ToolStripMenuItem).Checked

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: ctxMenuTotals_ItemClicked" & vbCrLf & ex.Message, MsgBoxStyle.DefaultButton1, "Error")
        End Try
    End Sub
    'Private Shared Sub ApplyDomainToProject()
    '    Try

    '        Dim pPrjLay As IFeatureLayer = My.Globals.Functions.findLayer(My.Globals.Constants.c_CIPProjectLayName, my.ArcMap.Document.FocusMap )
    '        If pPrjLay IsNot Nothing Then
    '            If pPrjLay.FeatureClass IsNot Nothing Then



    '                If pPrjLay.FeatureClass.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCIPStimField) > 0 Then

    '                    Dim pFld As IField = pPrjLay.FeatureClass.Fields.Field(pPrjLay.FeatureClass.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCIPStimField))

    '                    Dim pDom As IDomain = pFld.Domain
    '                    Dim pLst As IList
    '                    If pDom IsNot Nothing Then
    '                        pLst = DomainToList(pDom)
    '                        s_cboCIPStim.DataSource = pLst
    '                        s_cboCIPStim.SelectedItem = pFld.DefaultValue


    '                    End If


    '                    pFld = pPrjLay.FeatureClass.Fields.Field(pPrjLay.FeatureClass.Fields.FindField(My.Globals.Constants.c_CIPProjectLayCIPStatField))
    '                    pDom = pFld.Domain
    '                    If pDom IsNot Nothing Then


    '                        pLst = DomainToList(pDom)
    '                        s_cboCIPStat.DataSource = pLst
    '                        s_cboCIPStat.SelectedItem = pFld.DefaultValue
    '                    End If


    '                    pFld = pPrjLay.FeatureClass.Fields.Field(pPrjLay.FeatureClass.Fields.FindField(My.Globals.Constants.c_CIPProjectLayPrjManField))
    '                    pDom = pFld.Domain
    '                    If pDom IsNot Nothing Then


    '                        pLst = DomainToList(pDom)
    '                        s_cboPrjMan.DataSource = pLst
    '                        s_cboPrjMan.SelectedItem = pFld.DefaultValue
    '                    End If

    '                    pFld = pPrjLay.FeatureClass.Fields.Field(pPrjLay.FeatureClass.Fields.FindField(My.Globals.Constants.c_CIPProjectLaySenEngField))
    '                    pDom = pFld.Domain
    '                    If pDom IsNot Nothing Then


    '                        pLst = DomainToList(pDom)
    '                        s_cboEnginner.DataSource = pLst
    '                        s_cboEnginner.SelectedItem = pFld.DefaultValue
    '                    End If

    '                    pLst = Nothing
    '                    pFld = Nothing
    '                    pDom = Nothing

    '                End If
    '            End If
    '        End If
    '        pPrjLay = Nothing
    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CIPProjectWindow:  ApplyDomainToProject" & vbCrLf & ex.Message)
    '    End Try
    'End Sub

    'Private Shared function onMapChange() As Boolean
    '    Try
    '        Call onActiveViewChanged()



    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CIPProjectWindow: onMapChange" & vbCrLf & ex.Message)


    '    End Try
    'End Function
    'Private Shared Sub LoadControlsToDetailForm()
    '    Try
    '        If s_tbCntCIPDetails.Controls.Count = 0 Then
    '            'add controls from the overlview layer
    '            AddControls()
    '            ShuffleControls(False)
    '        End If
    '        'If s_cboCIPInvTypes.DataSource Is Nothing Then
    '        '    Dim pInvTbl As ITable = My.Globals.Functions.findTable(My.Globals.Constants.c_CIPInvLayName, my.ArcMap.Document.FocusMap )
    '        '    If pInvTbl IsNot Nothing Then



    '        '        If pInvTbl.Fields.FindField(My.Globals.Constants.c_CIPInvLayInvTypefield) > 0 Then

    '        '            Dim pFld As IField = pInvTbl.Fields.Field(pInvTbl.Fields.FindField(My.Globals.Constants.c_CIPInvLayInvTypefield))

    '        '            Dim pDom As IDomain = pFld.Domain
    '        '            Dim pLst As IList
    '        '            If pDom IsNot Nothing Then
    '        '                pLst = DomainToList(pDom)
    '        '                s_cboCIPInvTypes.DataSource = pLst
    '        '                s_cboCIPInvTypes.DisplayMember = "getDisplay"
    '        '                s_cboCIPInvTypes.ValueMember = "getValue"
    '        '                s_cboCIPInvTypes.SelectedItem = pFld.DefaultValue


    '        '            End If


    '        '            pLst = Nothing
    '        '            pFld = Nothing
    '        '            pDom = Nothing

    '        '        End If
    '        '    End If
    '        '    pInvTbl = Nothing
    '        'End If


    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CIPProjectWindow:  ApplyDomainToProject" & vbCrLf & ex.Message)
    '    End Try
    'End Sub


#End Region
#Region "IDockableWindowDef Members"





    'Public ReadOnly Property UserData() As Object Implements ESRI.ArcGIS.Framework.IDockableWindowDef.UserData
    '    Get
    '        Return Me
    '    End Get
    'End Property

#End Region
#Region "Friend Shared Functions"
    Friend Shared Sub EditingStarted()
        Try


            Dim pWkSpace As IWorkspace = GetCIPWorkspace()

            If CheckEditingWorkspace(pWkSpace) Then
                If s_btnSave Is Nothing Then Return

                s_btnSave.Enabled = True
                s_btnStartEditing.Enabled = False
                s_btnStopEditing.Enabled = True
                s_gpBxCIPCostingLayers.Enabled = True
                s_btnClear.Enabled = True

                My.Globals.Variables.v_SaveEnabled = True
                EnableSavePrj()

            Else
                If s_btnSave Is Nothing Then Return
                s_btnSave.Enabled = False
                s_btnStartEditing.Enabled = False
                s_btnStopEditing.Enabled = False
                s_gpBxCIPCostingLayers.Enabled = False

                My.Globals.Variables.v_SaveEnabled = False
                EnableSavePrj()

                'btnClear.Enabled = False
            End If
            pWkSpace = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: m_EditorEvents_OnStartEditing" & vbCrLf & ex.Message)

        End Try

    End Sub
    Public Shared Sub EditingStopped()
        Try
            If s_btnSave Is Nothing Then Return
            s_btnSave.Enabled = False
            s_gpBxCIPCostingLayers.Enabled = False
            s_btnStartEditing.Enabled = True
            s_btnStopEditing.Enabled = False
            My.Globals.Variables.v_SaveEnabled = False
            '   btnClear.Enabled = False

            EnableSavePrj()
            'If gpBxCIPCostingLayers.Visible = True Then
            '    gpBxCIPCostingLayers.Visible = False
            '    If gpBxSwitch.Dock = DockStyle.Left Then
            '        gpBxCIPPrj.Visible = True
            '        gpBxCIPCan.Visible = False
            '    Else
            '        gpBxCIPPrj.Visible = False
            '        gpBxCIPCan.Visible = True
            '    End If
            'End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: m_EditorEvents_OnStopEditing" & vbCrLf & ex.Message)

        End Try

    End Sub
    Friend Shared Sub SplitLines(ByVal pPnt As IPoint, ByVal Split As Boolean)
        splitSegmentAtLocation(pPnt, Split)

    End Sub

    Friend Shared Sub LoadProjectFromLocation(ByVal pPnt As IPoint)
        Dim pFeat As IFeature
        Try
            pFeat = FindPrjAtLocation(pPnt)

            If pFeat Is Nothing Then Return

            ClearControl()


            Dim strPrjName As String = loadProjectToForm(pFeat)
            If strPrjName = "" Then Return

            LoadExistingAssetsToForm(strPrjName)
            setProjectCostAndTotal()
            '        Call s_dgCIP_SelectionChanged(Nothing, Nothing)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: LoadProjectFromLocation" & vbCrLf & ex.Message)
        Finally
            pFeat = Nothing

        End Try

    End Sub
    'Friend Shared Function EnableTools() As Boolean
    '    Return s_btnSelectAssets.Enabled

    'End Function
    Friend Shared Sub LoadAssetsByShape(ByVal pEnv As IEnvelope)



        Dim pProDlgFact As IProgressDialogFactory = Nothing
        Dim pStepPro As IStepProgressor = Nothing
        Dim pProDlg As IProgressDialog2 = Nothing
        Dim pTrkCan As ITrackCancel = Nothing


        Try
            If s_dgCIP.RowCount = 0 Then
                Cleargraphics()
            End If



            'Dim pDefTbl As ITable
            'pDefTbl = My.Globals.Functions.FindTable(My.Globals.Constants.c_CIPDefTableName, m_pMxDoc.FocusMap)
            Dim pDefCursor As ICursor
            Dim pDefRow As IRow

            Dim pDefFilt As IQueryFilter = New QueryFilter
            pDefFilt.WhereClause = My.Globals.Constants.c_CIPDefActiveField & " = " & "'Yes'" & " OR " & My.Globals.Constants.c_CIPDefActiveField & " is null"


            If My.Globals.Variables.v_CIPTableDef Is Nothing Then
                MsgBox("The CIP Definition table cannot be found, exiting")
                Return

            End If
            If ValidDefTable(My.Globals.Variables.v_CIPTableDef) = False Then
                MsgBox("The CIP Definition table schema is incorrect, exiting")
                Return

            End If
            If My.Globals.Variables.v_CIPTableDef.RowCount(pDefFilt) = 0 Then
                MsgBox("The CIP Definition table does not contain active values, exiting")
                Return

            End If



            pDefCursor = My.Globals.Variables.v_CIPTableDef.Search(pDefFilt, True)
            pDefRow = pDefCursor.NextRow
            If pDefRow Is Nothing Then
                MsgBox("The CIP Definition table does not contain any values, exiting")
                setDefLayersToDropdown(Nothing)

                Return
            End If

            Dim boolCont As Boolean

            ' Create a CancelTracker  
            pTrkCan = New CancelTracker
            ' Create the ProgressDialog. This automatically displays the dialog  

            pProDlgFact = New ProgressDialogFactory
            pProDlg = pProDlgFact.Create(pTrkCan, 0)
            ' Set the properties of the ProgressDialog  
            pProDlg.CancelEnabled = True
            pProDlg.Description = "Processing Cost Table"
            pProDlg.Title = "Costing Assets"
            pProDlg.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral
            ' Set the properties of the Step Progressor  
            pStepPro = pProDlg
            pStepPro.MinRange = 1
            pStepPro.MaxRange = My.Globals.Variables.v_CIPTableDef.RowCount(Nothing) + 1
            pStepPro.StepValue = 1
            pStepPro.Message = "Loading Cost Tables"
            ' Step. Do your big process here.  
            boolCont = True

            ' Dim iLoopCnt As Int16 = 0

            pProDlg.ShowDialog()
            Do Until pDefRow Is Nothing
                Dim pAssetFl As IFeatureLayer = Nothing


                ' iLoopCnt = iLoopCnt + 1


                Dim pArr As ArrayList = My.Globals.Functions.FindLayers(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)))
                For Each pAssetFl In pArr
                    'pAssetFl = My.Globals.Functions.FindLayer(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)))
                    If pAssetFl IsNot Nothing Then

                        If My.Globals.Functions.isVisible(pAssetFl) = False Then
                            pStepPro.Message = "Skipping assets from " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & " Not visible"
                        Else
                       
                            Dim pRowCount As Integer = getFeatureCount(pAssetFl, pEnv, "")

                            pStepPro.Message = "Costing " & pRowCount & " assets from " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))

                            If pRowCount > 0 Then
                                Dim pDs As IDataset = pAssetFl.FeatureClass

                                '   Dim pAssetSelectionSet As ISelectionSet2 = getSelectionSet(pAssetFl, pEnv, "")
                                Dim strSourceClassName As String = My.Globals.Functions.getClassName(pDs)
                                Dim strSourceLayerNameConfig As String = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))

                                ' pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))

                                '  Dim pCostQFilt As IQueryFilter = New QueryFilter
                                '   Dim pSQL As String = My.Globals.Constants.c_CIPCostNameField & " = '" & strSourceLayer & "'"
                                '  pSQL = pSQL & " AND (" & My.Globals.Constants.c_CIPCostActiveField & " = 'Yes' OR " & My.Globals.Constants.c_CIPCostActiveField & " is Null)"
                                ' pCostQFilt.WhereClause = pSQL
                                ' pStepPro.Message = "Checking Cost table for " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))


                                'If pRowCount > 0 Then
                                'pStepPro.Message = "Looking up Cost for " & pRowCount & " assets from " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))
                                Dim strFiltField1 As String = ""
                                Dim strFiltField2 As String = ""
                                Dim strMultiField As String = ""
                                Dim strLenField As String = ""

                                If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) Is Nothing Then
                                    If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) Is DBNull.Value Then
                                        If Not Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))) = "" Then
                                            If pAssetFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))) < 0 Then
                                                MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                                                Return
                                            Else

                                                strMultiField = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefMultiField))

                                            End If
                                        End If
                                    End If
                                End If
                                If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField)) Is Nothing Then
                                    If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField)) Is DBNull.Value Then
                                        If Not Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField))) = "" Then
                                            If UCase(Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField)))).Contains("SHAPE") Then
                                                If pAssetFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField))) > 0 Then
                                                    strLenField = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField))
                                                ElseIf pAssetFl.FeatureClass.FindField("Shape.len") > 0 Then
                                                    strLenField = "Shape.len"
                                                ElseIf pAssetFl.FeatureClass.FindField("Shape_length") > 0 Then
                                                    strLenField = "Shape_length"
                                                ElseIf pAssetFl.FeatureClass.FindField("Shape.length") > 0 Then
                                                    strLenField = "Shape.length"
                                                ElseIf pAssetFl.FeatureClass.FindField("Shape_len") > 0 Then
                                                    strLenField = "Shape_len"
                                                End If
                                                If strLenField = "" Then
                                                    MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                                                    Return
                                                End If
                                            Else
                                                If pAssetFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField))) < 0 Then
                                                    MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                                                    Return
                                                Else

                                                    strLenField = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefLenField))

                                                End If
                                            End If



                                        End If
                                    End If
                                End If


                                If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) Is Nothing Then
                                    If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) Is DBNull.Value Then
                                        If Not Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))) = "" Then
                                            If pAssetFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))) < 0 Then
                                                MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                                                Return
                                            Else

                                                strFiltField1 = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField1))

                                            End If
                                        End If
                                    End If
                                End If


                                If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) Is Nothing Then
                                    If Not pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) Is DBNull.Value Then
                                        If Not Trim(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))) = "" Then
                                            If pAssetFl.FeatureClass.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))) < 0 Then

                                                MsgBox(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2)) & " does not exist in layer " & pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField)) & ", exiting")

                                                Return
                                            Else
                                                strFiltField2 = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefFiltField2))
                                            End If
                                        End If
                                    End If
                                End If



                                Dim pSpatQ As ISpatialFilter = New SpatialFilter
                                pSpatQ.Geometry = pEnv
                                pSpatQ.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                                pSpatQ.GeometryField = pAssetFl.FeatureClass.ShapeFieldName
                                Dim pFCursor As IFeatureCursor = pAssetFl.FeatureClass.Search(pSpatQ, True)
                                Dim pFeat As IFeature = pFCursor.NextFeature
                                Dim loopCnt As Integer = 1

                                Do Until pFeat Is Nothing
                                    pStepPro.Message = "Processing " & loopCnt & " of " & pRowCount & " for " & strSourceLayerNameConfig
                                    Dim strDefVal1 As String = "", strDefVal2 As String = ""



                                    Dim strSourceLayerID As String
                                    Dim dblSourceCost As Double = 0.0
                                    Dim dblSourceAddCost As Double = 0.0
                                    Dim dblLength As Double = 0.0
                                    Dim strNotes As String = ""
                                    Dim dblTotalCost As Double = 0.0
                                    Dim dblMulti As Double = 1.0


                                    Dim strFiltVal1 As String = ""
                                    Dim strFiltVal2 As String = ""


                                    Dim strFiltValDisplay1 As String = ""
                                    Dim strFiltValDisplay2 As String = ""

                                    Dim pGeo As IGeometry

                                    pGeo = pFeat.Shape
                                    'strSourceLayer = pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefNameField))
                                    Try
                                        If pFeat.Value(pFeat.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefIDField)))) Is Nothing Then
                                            MsgBox("The ID is missing for asset with an OID of " & pFeat.OID & " in " & strSourceLayerNameConfig & vbCrLf & "Skipping this asset")

                                            pFeat = pFCursor.NextFeature

                                            Continue Do
                                        ElseIf pFeat.Value(pFeat.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefIDField)))) Is DBNull.Value Then
                                            MsgBox("The ID is missing for asset with an OID of " & pFeat.OID & " in " & strSourceLayerNameConfig & vbCrLf & "Skipping this asset")

                                            pFeat = pFCursor.NextFeature

                                            Continue Do
                                        Else
                                            strSourceLayerID = pFeat.Value(pFeat.Fields.FindField(pDefRow.Value(pDefRow.Fields.FindField(My.Globals.Constants.c_CIPDefIDField))))
                                        End If

                                    Catch ex As Exception
                                        MsgBox("The ID Field cannot be found for: " & strSourceLayerNameConfig)
                                        Exit Do

                                    End Try
                                    If strMultiField <> "" Then
                                        If Not pFeat.Value(pFeat.Fields.FindField(strMultiField)) Is Nothing Then
                                            If Not pFeat.Value(pFeat.Fields.FindField(strMultiField)) Is DBNull.Value Then
                                                If IsNumeric(pFeat.Value(pFeat.Fields.FindField(strMultiField))) Then

                                                    If CInt(pFeat.Value(pFeat.Fields.FindField(strMultiField))) <> 0 Then
                                                        dblMulti = pFeat.Value(pFeat.Fields.FindField(strMultiField))
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                    If strFiltField1 <> "" Then
                                        If Not pFeat.Value(pFeat.Fields.FindField(strFiltField1)) Is Nothing Then
                                            If Not pFeat.Value(pFeat.Fields.FindField(strFiltField1)) Is DBNull.Value Then
                                                strFiltVal1 = pFeat.Value(pFeat.Fields.FindField(strFiltField1))
                                                strFiltValDisplay1 = strFiltVal1
                                                If pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField1)).Domain IsNot Nothing Then
                                                    Dim pDom As IDomain = pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField1)).Domain
                                                    If pDom.Type = esriDomainType.esriDTCodedValue Then
                                                        strFiltValDisplay1 = My.Globals.Functions.GetDomainDisplay(pFeat.Value(pFeat.Fields.FindField(strFiltField1)), pDom)

                                                    End If


                                                End If
                                            End If
                                        End If




                                    End If
                                    If strFiltField2 <> "" Then
                                        If Not pFeat.Value(pFeat.Fields.FindField(strFiltField2)) Is Nothing Then
                                            If Not pFeat.Value(pFeat.Fields.FindField(strFiltField2)) Is DBNull.Value Then
                                                strFiltVal2 = pFeat.Value(pFeat.Fields.FindField(strFiltField2))
                                                strFiltValDisplay2 = strFiltVal2
                                                If pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField2)).Domain IsNot Nothing Then
                                                    Dim pDom As IDomain = pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField2)).Domain
                                                    If pDom.Type = esriDomainType.esriDTCodedValue Then

                                                        strFiltValDisplay2 = My.Globals.Functions.GetDomainDisplay(strFiltVal2, pDom)

                                                    End If

                                                End If
                                            End If
                                        End If





                                    End If
                                    'System.Configuration.ConfigurationManager.AppSettings("CIPReplacementValue")
                                    If UCase(s_cboStrategy.Text) = UCase(My.Globals.Variables.v_CIPReplaceValue) Then
                                        getReplacementValues(strSourceLayerNameConfig, strSourceClassName, s_cboAction.SelectedValue, strFiltVal1, strFiltVal2, strDefVal1, strDefVal2)


                                    Else
                                        strDefVal2 = strFiltVal2
                                        strDefVal1 = strFiltVal1
                                    End If

                                    Try
                                        If strLenField <> "" Then

                                            dblLength = pFeat.Value(pFeat.Fields.FindField(strLenField))
                                        End If
                                        Select Case pGeo.GeometryType
                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint

                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline


                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                                                If dblLength = 0.0 Then
                                                    dblLength = CType(pGeo, IArea).Area
                                                End If


                                        End Select
                                    Catch ex As Exception
                                        MsgBox("The length field for: " & strSourceLayerNameConfig & " is not correct")
                                        pFeat = pFCursor.NextFeature
                                        Continue Do

                                    End Try
                                    Dim pCostRow As IRow = CheckForCostFeat(strSourceClassName, strSourceLayerNameConfig, s_cboStrategy.SelectedValue, s_cboAction.Text, s_cboAction.SelectedValue, strDefVal1, strDefVal2)

                                    If pCostRow IsNot Nothing Then




                                        If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) Is Nothing Then
                                            If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField)) Is DBNull.Value Then
                                                dblSourceCost = pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostCostField))
                                            End If
                                        End If
                                        If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) Is Nothing Then
                                            If Not pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField)) Is DBNull.Value Then
                                                dblSourceAddCost = pCostRow.Value(My.Globals.Variables.v_CIPTableCost.Fields.FindField(My.Globals.Constants.c_CIPCostAddCostField))
                                            End If
                                        End If



                                        Select Case pGeo.GeometryType
                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                                                dblTotalCost = (dblMulti * dblSourceCost) + dblSourceAddCost
                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline

                                                dblTotalCost = (dblMulti * dblLength * dblSourceCost) + dblSourceAddCost
                                            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon

                                                dblTotalCost = (dblMulti * dblLength * dblSourceCost) + dblSourceAddCost
                                        End Select
                                        '  dblTotalCost =dblTotalCost


                                    End If
                                    'Translate the default values to display values
                                    If strFiltField1 <> "" Then
                                        If pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField1)).Domain IsNot Nothing Then
                                            Dim pDom As IDomain = pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField1)).Domain
                                            If pDom.Type = esriDomainType.esriDTCodedValue Then

                                                strDefVal1 = My.Globals.Functions.GetDomainDisplay(strDefVal1, pDom)
                                            End If
                                        End If
                                    End If

                                    If strFiltField2 <> "" Then
                                        If pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField2)).Domain IsNot Nothing Then
                                            Dim pDom As IDomain = pFeat.Fields.Field(pFeat.Fields.FindField(strFiltField2)).Domain
                                            If pDom.Type = esriDomainType.esriDTCodedValue Then

                                                strDefVal2 = My.Globals.Functions.GetDomainDisplay(strDefVal2, pDom)
                                            End If
                                        End If

                                    End If


                                    loadRecord(pGeo, strSourceLayerNameConfig, strSourceClassName, strSourceLayerID, dblSourceCost, dblSourceAddCost, dblLength, dblTotalCost, strFiltValDisplay1, strFiltValDisplay2, strFiltField1, strFiltField2, pFeat.OID, strDefVal1, strDefVal2, s_cboStrategy.Text, s_cboAction.Text, dblMulti, strNotes)

                                    pGeo = Nothing
                                    pCostRow = Nothing

                                    pFeat = pFCursor.NextFeature
                                    loopCnt = loopCnt + 1
                                Loop

                                Marshal.ReleaseComObject(pFCursor)
                                pFCursor = Nothing
                                pFeat = Nothing
                                pSpatQ = Nothing
                                '    pCostQFilt = Nothing
                            End If ' No Active Cost Records Found

                            'End If 'No records found in the envelope
                        End If
                    End If ' Featurelayer was not found
                Next

                pStepPro.Step()
                pDefRow = pDefCursor.NextRow
            Loop
            pDefRow = Nothing
            Marshal.ReleaseComObject(pDefCursor)

            pDefCursor = Nothing



            pDefFilt = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools: LoadAssetsByShape - Error costing assets" & vbCrLf & ex.Message)

        Finally
            If pProDlg IsNot Nothing Then
                pProDlg.HideDialog()
                Marshal.ReleaseComObject(pProDlg)
            End If

            pProDlg = Nothing
            pTrkCan = Nothing
            pStepPro = Nothing
            pProDlgFact = Nothing
            pProDlg = Nothing



        End Try

        setProjectCostAndTotal()


        My.ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, Nothing, My.ArcMap.Document.ActiveView.Extent)


    End Sub
    Friend Shared Sub Cleargraphics()
        Try

            If My.ArcMap.Document Is Nothing Then Return
            If My.ArcMap.Document.ActiveView Is Nothing Then Return
            If My.ArcMap.Document.ActiveView.GraphicsContainer Is Nothing Then Return


            My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

            Dim pElem As IElement
            Dim pElProp As IElementProperties
            pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next



            Do Until pElem Is Nothing
                pElProp = pElem

                If pElProp.CustomProperty IsNot Nothing Then
                    If pElProp.CustomProperty IsNot DBNull.Value Then
                        If pElProp.CustomProperty.contains("CIPTools:") Then
                            My.ArcMap.Document.ActiveView.GraphicsContainer.DeleteElement(pElem)



                        End If
                    End If
                End If


                pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
            Loop

            My.ArcMap.Document.ActiveView.Refresh()



            pElem = Nothing
            pElProp = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: Cleargraphics" & vbCrLf & ex.Message)


        End Try
    End Sub
    Friend Shared Function AddGraphicSketch(ByVal Geometry As IGeometry) As Boolean

        AddRecordFromGraphic(Geometry, True, s_cboDefLayers.Text)
        setProjectCostAndTotal()
    End Function
    Friend Shared Function GetSketchFeatureName() As String
        Return s_cboDefLayers.Text

    End Function

    Friend Shared Sub setProjectCostAndTotal()

        Try

            Dim TotCost As Double = 0.0
            Dim TotLen As Double = 0.0
            Dim TotPnt As Double = 0.0
            Dim TotArea As Double = 0.0
            For Each pRow As DataGridViewRow In s_dgCIP.Rows
                TotCost = TotCost + pRow.Cells("TOTCOST").Value


                If pRow.Cells("GeoType").Value.ToString.ToUpper = "POLYLINE" Then
                    TotLen = TotLen + pRow.Cells("LENGTH").Value
                ElseIf pRow.Cells("GeoType").Value.ToString.ToUpper = "POLYGON" Then

                    TotArea = TotArea + pRow.Cells("LENGTH").Value
                ElseIf pRow.Cells("GeoType").Value.ToString.ToUpper = "POINT" Then
                    TotPnt = TotPnt + 1


                End If
            Next
            s_lblTotalCost.Text = FormatCurrency(TotCost, 2, TriState.True, TriState.True) 'Format(Total, "#,###.00")
            s_lblTotLength.Text = Format(TotLen, "#,###.00")
            s_lblTotArea.Text = Format(TotArea, "#,###.00")
            s_lblTotPnt.Text = Format(TotPnt, "#,###")

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: setProjectCostAndTotal" & vbCrLf & ex.Message)

        End Try
    End Sub
    Friend Shared Sub loadRecord(ByVal pGeo As IGeometry, ByVal strSourceLayerNameConfig As String, ByVal strSourceClassName As String, ByVal strSourceLayerID As String, ByVal dblSourceCost As String, _
                           ByVal dblSourceAddCost As String, ByVal dblLength As Double, ByVal dblTotalCost As String, ByVal strFiltVal1 As String, _
                           ByVal strFiltVal2 As String, ByVal strFiltFld1 As String, ByVal strFiltFld2 As String, ByVal strOID As String, _
                           ByVal strDefVal1 As String, ByVal strDefVal2 As String, ByVal strStrategy As String, ByVal strAction As String, ByVal dblMulti As Double, ByVal strNotes As String)

        Try



            pGeo.Project(My.ArcMap.Document.FocusMap.SpatialReference)

            For Each row As DataGridViewRow In s_dgCIP.Rows
                Dim pOIDVal As String = row.Cells("OID").Value
                If pOIDVal.Contains(":") Then
                    pOIDVal = row.Cells("OID").Value.ToString.Split(":")(0)
                End If
                If ((row.Cells("ASSETTYP").Value = strSourceClassName Or row.Cells("ASSETTYP").Value = strSourceLayerNameConfig) And pOIDVal = strOID) Then
                    Exit Sub

                End If
            Next

            Dim pNewRow As String() = New String() {}

            If strDefVal1 = "" Then
                strDefVal1 = strFiltVal1
            End If
            If strDefVal2 = "" Then
                strDefVal2 = strFiltVal2
            End If
            dblLength = Math.Round(dblLength, 2)
            dblTotalCost = FormatCurrency(dblTotalCost, 2, TriState.True, TriState.True)
            dblSourceAddCost = FormatCurrency(dblSourceAddCost, 2, TriState.True, TriState.True)
            dblSourceCost = FormatCurrency(dblSourceCost, 2, TriState.True, TriState.True)

            Select Case pGeo.GeometryType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    pNewRow = New String() {strSourceLayerNameConfig, strSourceLayerID, strStrategy, strAction, strFiltVal1, strFiltVal2, strDefVal1, strDefVal2, _
                                     dblSourceCost, dblMulti, dblSourceAddCost, dblLength, dblTotalCost, "Point", strFiltFld1, strFiltFld2, strOID, strNotes}
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    pNewRow = New String() {strSourceLayerNameConfig, strSourceLayerID, strStrategy, strAction, strFiltVal1, strFiltVal2, strDefVal1, strDefVal2, _
                                     dblSourceCost, dblMulti, dblSourceAddCost, dblLength, dblTotalCost, "Polygon", strFiltFld1, strFiltFld2, strOID, strNotes}
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    pNewRow = New String() {strSourceLayerNameConfig, strSourceLayerID, strStrategy, strAction, strFiltVal1, strFiltVal2, strDefVal1, strDefVal2, _
                                     dblSourceCost, dblMulti, dblSourceAddCost, dblLength, dblTotalCost, "Polyline", strFiltFld1, strFiltFld2, strOID, strNotes}

            End Select

            AddGraphic(pGeo, strSourceLayerNameConfig & ":" & strOID)

            s_dgCIP.Rows.Add(pNewRow)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: loadRecord" & vbCrLf & ex.Message)

        Finally
            '   AddHandler pFrm.RowChanged, AddressOf RowChanged
        End Try
    End Sub
    Friend Shared Function CopyRecord(ByVal RowIdx As Integer) As DataGridViewRow



        Try
            Dim pIdx As Integer = s_dgCIP.Rows.Add
            Dim pNewrow As DataGridViewRow = s_dgCIP.Rows(pIdx)
            Dim pOldRow As DataGridViewRow = s_dgCIP.Rows(RowIdx)
            For i = 0 To s_dgCIP.ColumnCount - 1
                pNewrow.Cells(i).Value = pOldRow.Cells(i).Value
            Next
            Return pNewrow
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: CopyRecord" & vbCrLf & ex.Message)
            Return Nothing

        End Try
    End Function
    Friend Shared Sub setDefLayersToDropdown(ByVal Layers As ArrayList)
        Try

            If Layers Is Nothing Then
                s_cboDefLayers.DataSource = Nothing

                s_cboDefLayers.Items.Clear()
                Return

            End If
            If Layers.Count = 0 Then
                s_cboDefLayers.DataSource = Nothing

                s_cboDefLayers.Items.Clear()
                Return
            End If

            Dim pStrPrev As String = s_cboDefLayers.Text

            s_cboDefLayers.DataSource = Layers
            s_cboDefLayers.DisplayMember = "getLayerName"
            s_cboDefLayers.ValueMember = "getGeoType"
            If pStrPrev <> "" Then
                If s_cboDefLayers.Items.Contains(pStrPrev) Then
                    s_cboDefLayers.Text = pStrPrev
                End If


            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: setDefLayersToDropdown" & vbCrLf & ex.Message)

        End Try
    End Sub
    Friend Shared Sub LoadControlsToDetailForm()
        Try
            If s_tbCntCIPDetails Is Nothing Then Return

            If s_tbCntCIPDetails.Controls.Count = 0 Then
                'add controls from the overlview layer
                AddControls()
                ShuffleControls(False)
            End If
            'If cboCIPInvTypes.DataSource Is Nothing Then
            '    Dim pInvTbl As ITable = FindTable(c_CIPInvLayName, m_pMxDoc.FocusMap)
            '    If pInvTbl IsNot Nothing Then



            '        If pInvTbl.Fields.FindField(c_CIPInvLayInvTypefield) > 0 Then

            '            Dim pFld As IField = pInvTbl.Fields.Field(pInvTbl.Fields.FindField(c_CIPInvLayInvTypefield))

            '            Dim pDom As IDomain = pFld.Domain
            '            Dim pLst As IList
            '            If pDom IsNot Nothing Then
            '                pLst = DomainToList(pDom)
            '                cboCIPInvTypes.DataSource = pLst
            '                cboCIPInvTypes.DisplayMember = "getDisplay"
            '                cboCIPInvTypes.ValueMember = "getValue"
            '                cboCIPInvTypes.SelectedItem = pFld.DefaultValue


            '            End If


            '            pLst = Nothing
            '            pFld = Nothing
            '            pDom = Nothing

            '        End If
            '    End If
            '    pInvTbl = Nothing
            'End If


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow:  LoadControlsToDetailForm" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Shared Sub setStratAction(ByVal strStrat As String, ByVal strAct As String)
        Try
            If s_cboStrategy.Text = strStrat Then Return

            s_cboStrategy.Text = strStrat
            s_cboAction.Text = strAct
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: setStratAction" & vbCrLf & ex.Message)

        End Try

    End Sub
    Friend Shared Sub SelectTool(ByVal strTool As CIPTools, ByVal bCheck As Boolean)
        Try

            If s_btnSelect Is Nothing Then Return

            Select Case strTool
                Case CIPTools.Sketch
                    s_btnSketch.Checked = bCheck
                    s_cboDefLayers.Enabled = Not bCheck
                    If bCheck Then
                        setStratAction("Proposed", "Open Cut")
                    End If
                Case CIPTools.SelectAssetsForGrid
                    s_btnSelectAssets.Checked = bCheck
                Case CIPTools.SelectExistingProject
                    s_btnSelectPrj.Checked = bCheck
                Case CIPTools.SelectCostedAsset

                    s_btnSelect.Checked = bCheck
            End Select

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: SelectTool" & vbCrLf & ex.Message)
        End Try
    End Sub
#End Region

#Region "Structures"

    Public Class layerAndTypes
        Public LayerName As String
        Public GeoType As ESRI.ArcGIS.Geometry.esriGeometryType
        Public Sub New(ByVal LayerNameVal As String, ByVal GeoTypeVal As ESRI.ArcGIS.Geometry.esriGeometryType)
            LayerName = LayerNameVal
            GeoType = GeoTypeVal
        End Sub
        Public Property getLayerName() As String
            Get
                Return LayerName
            End Get
            Set(ByVal Value As String)
                LayerName = Value
            End Set
        End Property
        Public Property getGeoType() As ESRI.ArcGIS.Geometry.esriGeometryType
            Get
                Return GeoType
            End Get
            Set(ByVal Value As ESRI.ArcGIS.Geometry.esriGeometryType)
                GeoType = Value
            End Set
        End Property

    End Class
#End Region
#Region "Enums"
    Public Enum CIPTools
        SelectAssetsForGrid
        SelectCostedAsset
        SelectExistingProject
        Sketch

    End Enum
#End Region
    Friend Shared ReadOnly Property Exists() As Boolean
        Get
            Return If((s_dgCIP Is Nothing), False, True)
        End Get
    End Property




    Private m_hook As Object
    ''' <summary>
    ''' Host object of the dockable window
    ''' </summary> 
    Public Property Hook() As Object
        Get
            Return m_hook
        End Get
        Set(ByVal value As Object)
            m_hook = value
        End Set
    End Property











    ''' <summary>
    ''' Implementation class of the dockable window add-in. It is responsible for
    ''' creating and disposing the user interface class for the dockable window.
    ''' </summary>
    Public Class AddinImpl
        Inherits ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        Private m_windowUI As CostEstimatingWindow

        Protected Overrides Function OnCreateChild() As System.IntPtr
            m_windowUI = New CostEstimatingWindow(Me.Hook)

            Return m_windowUI.Handle
        End Function

        Protected Overrides Sub Dispose(ByVal Param As Boolean)
            If m_windowUI IsNot Nothing Then
                m_windowUI.Dispose(Param)
            End If

            MyBase.Dispose(Param)
        End Sub

    End Class

    Private Sub gpBxCIPCan_Enter(sender As System.Object, e As System.EventArgs) Handles gpBxCIPCan.Enter

    End Sub
End Class




