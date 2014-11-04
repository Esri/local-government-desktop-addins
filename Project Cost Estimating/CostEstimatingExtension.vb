
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


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.SystemUI
Imports ESRI.ArcGIS.Desktop.AddIns
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.ADF.BaseClasses


Imports ESRI.ArcGIS.Geometry


Public Class CostEstimatingExtension
    Inherits ESRI.ArcGIS.Desktop.AddIns.Extension

    Private Shared s_dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
    Private Shared s_extension As CostEstimatingExtension
    Private Shared s_SelectAssetTool As ICommandItem
    Private Shared s_SelectPrj As ICommandItem
    Private Shared s_SelectCostedAsset As ICommandItem


    'Shared Events
    Friend Shared Event ChangeCIPToolStatus(ByVal enable As Boolean)



    Public Sub New()

    End Sub

    ' Overrides

    Protected Overrides Sub OnStartup()
        s_extension = Me

        ' Wire up events
        AddHandler My.ArcMap.Events.NewDocument, AddressOf ArcMap_NewDocument
        AddHandler My.ArcMap.Events.OpenDocument, AddressOf ArcMap_OpenDocument

        Dim pID As New UID

        pID.Value = "esriEditor.Editor"

        My.Globals.Variables.v_Editor = TryCast(My.ArcMap.Application.FindExtensionByCLSID(pID), IEditor2)

        If My.Globals.Variables.v_Editor Is Nothing Then Return

        My.Globals.Variables.v_EditorEvents = TryCast(My.Globals.Variables.v_Editor, Editor)


        pID = Nothing
        'If Me.State = ExtensionState.Enabled Then
        '    Initialize()

        'End If

    End Sub

    Protected Overrides Function OnSetState(ByVal state As ExtensionState) As Boolean
        ' Optionally check for a license here
        Me.State = state
        If state = ExtensionState.Enabled Then
            Initialize()
            initAddin()
        Else
            Uninitialize()
        End If

        Return MyBase.OnSetState(state)
    End Function

    Protected Overrides Function OnGetState() As ExtensionState
        Return Me.State
    End Function


    Protected Overrides Sub OnShutdown()

    End Sub
    ' Privates
    Private Sub initAddin()
        If State = ExtensionState.Enabled Then
            GetCostEstimatingWindow()

            If CheckForCIPLayers() Then
                If My.Globals.Variables.v_CIPWindowsValid = True Then Return


                initCIPLayers()
                CostEstimatingWindow.EnableWindowControls(True)
                CostEstimatingWindow.LoadControlsToDetailForm()
                My.Globals.Variables.v_CIPWindowsValid = True

            Else
                CostEstimatingWindow.EnableWindowControls(False)
                My.Globals.Variables.v_CIPWindowsValid = False


            End If
            CostEstimatingWindow.EnableSavePrj()
        Else

            CostEstimatingWindow.EnableWindowControls(False)
            My.Globals.Variables.v_CIPWindowsValid = False

        End If

    End Sub
    Private Sub Initialize()
        If s_extension Is Nothing Or Me.State <> ExtensionState.Enabled Then
            Return
        End If

        ' Reset event handlers
        Dim avEvent As IActiveViewEvents_Event = TryCast(My.ArcMap.Document.FocusMap, IActiveViewEvents_Event)
        AddHandler avEvent.ItemAdded, AddressOf AvEvent_ItemAdded
        AddHandler avEvent.ItemDeleted, AddressOf AvEvent_ItemDeleted
        'AddHandler avEvent.SelectionChanged, AddressOf UpdateSelCountDockWin
        'AddHandler avEvent.ContentsChanged, AddressOf avEvent_ContentsChanged

        ' If the dockview hasn't been created yet

        If (Not CostEstimatingWindow.Exists) Then
            Return
        End If


        CostEstimatingWindow.ResetControls(True)

        If CheckForCIPLayers() Then
            initCIPLayers()
            CostEstimatingWindow.EnableWindowControls(True)
            RaiseEvent ChangeCIPToolStatus(True)

            CostEstimatingWindow.LoadControlsToDetailForm()

        Else
            CostEstimatingWindow.EnableWindowControls(False)
            RaiseEvent ChangeCIPToolStatus(False)

        End If
        CostEstimatingWindow.EnableSavePrj()
        CostEstimatingWindow.SetEnabled(True)

    End Sub

    Friend Shared Sub raiseEventInExt(ByVal bVal As Boolean)
        RaiseEvent ChangeCIPToolStatus(bVal)

    End Sub

    Friend Shared Function CheckForCIPLayers() As Boolean
        Try
            If My.ArcMap.Document Is Nothing Then Return False
            If My.ArcMap.Document.FocusMap Is Nothing Then Return False
            If My.ArcMap.Document.FocusMap.LayerCount = 0 Then Return False

            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPProjectPointLayName) = False Then Return False
            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPProjectPolygonLayName) = False Then Return False
            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPProjectPolylineLayName) = False Then Return False
            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPProjectLayName) = False Then Return False
            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPOverviewLayName) = False Then Return False
            If My.Globals.Functions.LayerExist(My.Globals.Constants.c_CIPOverviewPointLayName) = False Then Return False
            If My.Globals.Functions.TableExist(My.Globals.Constants.c_CIPDefTableName) = False Then Return False
            If My.Globals.Functions.TableExist(My.Globals.Constants.c_CIPCostTableName) = False Then Return False
            If My.Globals.Functions.TableExist(My.Globals.Constants.c_CIPReplaceTableName) = False Then Return False
            ' If  my.Globals.Functions.TableExist(my.Globals.Constants.c_CIPInvLayName, pMap) = False Then Return False
            Return True


        Catch ex As Exception
            ' MsgBox("Error in the Costing Tools - CIPProjectWindow: CheckForCIPLayers" & vbCrLf & ex.Message)


        End Try

    End Function
    Friend Shared Function initCIPLayers() As Boolean
        Try

            My.Globals.Variables.v_CIPLayerPoint = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPointLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPLayerPolygon = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolygonLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPLayerPolyline = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectPolylineLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPLayerPrj = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPProjectLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPLayerOver = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPOverviewLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPLayerOverPoint = TryCast(My.Globals.Functions.FindLayer(My.Globals.Constants.c_CIPOverviewPointLayName), IFeatureLayer)
            My.Globals.Variables.v_CIPTableDef = TryCast(My.Globals.Functions.FindTable(My.Globals.Constants.c_CIPDefTableName), ITable)
            My.Globals.Variables.v_CIPTableCost = TryCast(My.Globals.Functions.FindTable(My.Globals.Constants.c_CIPCostTableName), ITable)
            My.Globals.Variables.v_CIPTableReplace = TryCast(My.Globals.Functions.FindTable(My.Globals.Constants.c_CIPReplaceTableName), ITable)

            'Dim pConfFiles() As String = My.Globals.Functions.GetConfigFiles
            'If pConfFiles.Length = 0 Then Return False
            My.Globals.Variables.v_CIPReplaceValue = My.Globals.Functions.GetConfigValue("CIPReplacementValue")
            My.Globals.Variables.v_CIPAbandonValue = My.Globals.Functions.GetConfigValue("CIPAbandonValue")
            Return True


        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: initCIPLayers" & vbCrLf & ex.Message)
            Return False


        End Try

    End Function
    Private Sub Uninitialize()
        If s_extension Is Nothing Then
            Return
        End If

        ' Detach event handlers
        Dim avEvent As IActiveViewEvents_Event = TryCast(My.ArcMap.Document.FocusMap, IActiveViewEvents_Event)
        RemoveHandler avEvent.ItemAdded, AddressOf AvEvent_ItemAdded
        RemoveHandler avEvent.ItemDeleted, AddressOf AvEvent_ItemDeleted
        ' RemoveHandler avEvent.SelectionChanged, AddressOf UpdateSelCountDockWin
        ' RemoveHandler avEvent.ContentsChanged, AddressOf avEvent_ContentsChanged
        avEvent = Nothing

        ' Update UI
        'Dim selCombo As SelectionTargetComboBox = SelectionTargetComboBox.GetSelectionComboBox()
        'If selCombo IsNot Nothing Then
        'selCombo.ClearAll()
        ' End If

        'SelCountDockWin.SetEnabled(False)
        CostEstimatingWindow.SetEnabled(False)
        My.Globals.Variables.v_CIPWindowsValid = False

    End Sub

    ' Event handlers

    Private Sub ArcMap_NewDocument()
        Dim pageLayoutEvent As IActiveViewEvents_Event = TryCast(My.ArcMap.Document.PageLayout, IActiveViewEvents_Event)
        Try
            AddHandler pageLayoutEvent.FocusMapChanged, AddressOf AVEvents_FocusMapChanged
        Catch ex As Exception

        End Try


        '  Initialize()
        initAddin()

    End Sub
    Private Sub ArcMap_OpenDocument()
        Dim pageLayoutEvent As IActiveViewEvents_Event = TryCast(My.ArcMap.Document.PageLayout, IActiveViewEvents_Event)
        AddHandler pageLayoutEvent.FocusMapChanged, AddressOf AVEvents_FocusMapChanged

        Initialize()
        initAddin()

    End Sub

    Private Sub avEvent_ContentsChanged()
        Initialize()
        initAddin()

    End Sub

    Private Sub AvEvent_ItemAdded(ByVal Item As Object)
        Try

            initAddin()

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: layerAdded" & vbCrLf & ex.Message)


        End Try

    End Sub
    Private Sub AvEvent_ItemDeleted(ByVal Item As Object)
        Try
            initAddin()
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPProjectWindow: layerAdded" & vbCrLf & ex.Message)


        End Try

    End Sub
    Private Sub AVEvents_FocusMapChanged()
        initAddin()
    End Sub
    'Private Function onActiveViewChanged() As Boolean
    '    Try
    '        ResetControls(True)
    '        m_actViewEvents = CType(m_pMxDoc.FocusMap, IActiveViewEvents_Event)
    '        If m_actViewEvents Is Nothing Then Return False
    '        If m_pMxDoc.FocusMap Is Nothing Then Return False






    '        AddHandler m_actViewEvents.ItemAdded, AddressOf layerAdded
    '        AddHandler m_actViewEvents.ItemDeleted, AddressOf layerAdded
    '        If CheckForCIPLayers(m_pMxDoc.FocusMap) Then
    '            EnableCIPTools(True)
    '            LoadControlsFromLayers()
    '            EnableSavePrj()
    '        Else
    '            EnableCIPTools(False)
    '            EnableSavePrj()
    '        End If


    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CIPProjectWindow: onActiveViewChanged" & vbCrLf & ex.Message)


    '    End Try
    'End Function
    ' Statics
    Friend Shared Sub ToggleCIPTools(ByVal enable As Boolean)
        GetSelectAssetTool()
        'RaiseEvent ChangeCIPToolStatus(enable)
    End Sub
    Friend Shared Function GetCostEstimatingWindow() As ESRI.ArcGIS.Framework.IDockableWindow
        If s_extension Is Nothing Then
            GetExtension()
        End If

        ' Only get/create the dockable window if they ask for it
        If s_dockWindow Is Nothing Then
            Dim dockWinID As UID = New UID()
            dockWinID.Value = "ArcGIS4LocalGovernment_CostEstimatingWindow"
            s_dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
            's_extension.UpdateSelCountDockWin()
        End If

        Return s_dockWindow
    End Function
    Friend Shared Function GetSelectAssetTool() As ICommandItem
        If s_extension Is Nothing Then
            GetExtension()
        End If
        If s_SelectAssetTool Is Nothing Then
            s_SelectAssetTool = My.Globals.Functions.GetCommand("ArcGIS4LocalGovernment_SelectAssetForGrid")

        End If

        Return s_SelectAssetTool
    End Function
    Friend Shared Function GetSelectPrjTool() As ICommandItem
        If s_extension Is Nothing Then
            GetExtension()
        End If
        If s_SelectPrj Is Nothing Then
            s_SelectPrj = My.Globals.Functions.GetCommand("ArcGIS4LocalGovernment_SelectExistingProject")


        End If

        Return s_SelectPrj
    End Function
    Friend Shared Function GetSelectCostedAssetTool() As ICommandItem
        If s_extension Is Nothing Then
            GetExtension()
        End If
        If s_SelectCostedAsset Is Nothing Then
            s_SelectCostedAsset = My.Globals.Functions.GetCommand("ArcGIS4LocalGovernment_SelectCostedAsset")


        End If

        Return s_SelectCostedAsset
    End Function
    Friend Shared Function IsExtensionEnabled() As Boolean
        If s_extension Is Nothing Then
            GetExtension()
        End If

        If s_extension Is Nothing Then
            Return False
        End If

        Return s_extension.State = ExtensionState.Enabled
    End Function
    Friend Shared Function HasSelectableLayer() As Boolean
        'If s_extension Is Nothing Then
        '    GetExtension()
        'End If

        'If s_extension Is Nothing Then
        '    Return False
        'End If

        'Return s_extension.m_hasSelectableLayer
    End Function

    Private Shared Function GetExtension() As CostEstimatingExtension
        If s_extension Is Nothing Then
            Dim extID As UID = New UIDClass()
            extID.Value = "ArcGIS4LocalGovernment_ArcGISProjectCostEstimatingExtension"
            ' Call FindExtension to load this just-in-time extension.
            My.ArcMap.Application.FindExtensionByCLSID(extID)
        End If
        Return s_extension
    End Function
End Class
