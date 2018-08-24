
' | Version 1.17.2018
' | Copyright 2018 Esri
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


Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Carto
Imports System.Windows.Forms
Public Class CreateAssetForGrid
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool
    Private m_bMouseHasMoved As Boolean
    Private m_pPt As IPoint
    Private m_pNewPolyFeedback As INewPolygonFeedback
    Private m_pNewLineFeedback As INewLineFeedback
    Private m_pSketchType As SketchType = SketchType.none
    Public Sub New()

    End Sub
#Region "Class Overrides"
    Protected Overrides Sub OnUpdate()
        Me.Enabled = My.Globals.Variables.v_CIPWindowsValid

    End Sub
    Protected Overrides Sub Finalize()
        Try
            m_pPt = Nothing
            m_pNewPolyFeedback = Nothing
            m_pNewLineFeedback = Nothing
            m_pSketchType = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSketchNewCandidate: Finalize" & vbCrLf & ex.ToString())
        Finally
            Try
                MyBase.Finalize()
            Catch ex As Exception

            End Try

        End Try
    End Sub

    Private Function IsInputNumeric(input As String) As Boolean
        If String.IsNullOrWhiteSpace(input) Then Return False
        If IsNumeric(input) Then Return True
        Dim parts() As String = input.Split("/"c)
        If parts.Length <> 2 Then Return False
        Return IsNumeric(parts(0)) AndAlso IsNumeric(parts(1))
    End Function
    Protected Overrides Sub OnKeyDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.KeyEventArgs)
        Try


            If m_pPt Is Nothing Or My.ArcMap.Document Is Nothing Then Return
            If m_pNewLineFeedback Is Nothing And m_pNewPolyFeedback Is Nothing Then Return

            If arg.KeyCode = Keys.F2 Then
                finishSketch()
            ElseIf arg.KeyCode = Keys.Escape Then

                If m_bMouseHasMoved Then
                    My.Globals.Variables.v_Editor.InvertAgent(m_pPt, 0) 'erase the old agent
                    m_bMouseHasMoved = False
                    m_pPt = Nothing
                End If
                If m_pNewLineFeedback IsNot Nothing Then
                    m_pNewLineFeedback.Stop()
                    m_pNewLineFeedback.Refresh(My.ArcMap.Document.ActiveView.ScreenDisplay.hDC)
                End If
                m_pNewLineFeedback = Nothing
                If m_pNewPolyFeedback IsNot Nothing Then
                    m_pNewPolyFeedback.Stop()
                    m_pNewPolyFeedback.Refresh(My.ArcMap.Document.ActiveView.ScreenDisplay.hDC)
                End If
                m_pNewPolyFeedback = Nothing
                OnRefresh(My.ArcMap.Document.ActiveView.ScreenDisplay.hDC)
                destroyMapTip()
            ElseIf arg.KeyCode = Keys.L And arg.Control = True Then
                'If m_pSketchType = SketchType.Polyline Then
                '    Dim lengthValve As String = InputBox("Length of line segment", "Enter length", " ")

                '    If IsInputNumeric(lengthValve) Then
                '        OnMouseDown()
                '    End If

                '    MyBase.OnKeyDown(arg)
                'End If
            End If
            'Dim pKeyEv As ESRI.ArcGIS.Desktop.AddIns.Tool.KeyEventArgs
            'pKeyEv = ESRI.ArcGIS.Desktop.AddIns.Tool.KeyEventArgs
            MyBase.OnKeyDown(arg)
        Catch ex As Exception

        End Try
    End Sub
    Protected Overrides Function OnDeactivate() As Boolean
        Dim screenDisplay As ESRI.ArcGIS.Display.IScreenDisplay = My.ArcMap.Document.ActiveView.ScreenDisplay
        OnRefresh(screenDisplay.hDC)
        screenDisplay = Nothing

        m_pPt = Nothing
        m_bMouseHasMoved = Nothing


        ArcGIS4LocalGovernment.CostEstimatingWindow.SelectTool(ArcGIS4LocalGovernment.CostEstimatingWindow.CIPTools.Sketch, False)

        screenDisplay = Nothing
        destroyMapTip()
        Return MyBase.OnDeactivate()
    End Function
    Private m_pSR As ISpatialReference
    Protected Overrides Sub OnActivate()
        Try

            Dim pApp As IMxApplication = CType(My.ArcMap.Application, ESRI.ArcGIS.ArcMapUI.IMxApplication)
            Dim pDoc As IMxDocument = pApp.Document
            Dim pMap As IMap = pDoc.FocusMap ' document may contain many maps, this is the acive one'
            m_pSR = pMap.SpatialReference

            ArcGIS4LocalGovernment.CostEstimatingWindow.SelectTool(ArcGIS4LocalGovernment.CostEstimatingWindow.CIPTools.Sketch, True)
            resetType()
            If My.Globals.Variables.v_Editor Is Nothing Then
                MsgBox("You must start editing if you want snapping", MsgBoxStyle.OkOnly, "Snap Warning")

            ElseIf My.Globals.Variables.v_Editor.EditState = esriEditState.esriStateNotEditing Then
                MsgBox("You must start editing if you want snapping", MsgBoxStyle.OkOnly, "Snap Warning")

            Else
                setSnappingForLayer()
            End If

        Catch ex As Exception
            MsgBox("Error in the Costing Tools: OnClick - CIPSketchNewCandidate" & vbCrLf & ex.ToString())

        End Try


        MyBase.OnActivate()
    End Sub
    Protected Overrides Sub OnDoubleClick()


        finishSketch()

        MyBase.OnDoubleClick()
    End Sub

    Private m_line_points As IPointCollection = Nothing
    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        Try

            Dim pMPnt As ESRI.ArcGIS.Geometry.IPoint

            pMPnt = My.ArcMap.ThisApplication.Display.DisplayTransformation.ToMapPoint(arg.X, arg.Y)
            Dim pSnapenv As ISnapEnvironment

            If My.Globals.Variables.v_Editor IsNot Nothing Then
                If My.Globals.Variables.v_Editor.EditState <> esriEditState.esriStateNotEditing Then
                    pSnapenv = TryCast(My.Globals.Variables.v_Editor, ISnapEnvironment)
                    pSnapenv.SnapPoint(pMPnt)
                    pSnapenv = Nothing

                End If
            End If


            If m_pSketchType = SketchType.Point Then
                m_line_points = Nothing
                ArcGIS4LocalGovernment.CostEstimatingWindow.AddGraphicSketch(pMPnt)


            ElseIf m_pSketchType = SketchType.Polygon Then
                m_line_points = Nothing
                If m_pNewPolyFeedback Is Nothing Then
                    Dim pSLineSymFeed As ISimpleLineSymbol
                    Dim pRGB As IRgbColor

                    m_pNewPolyFeedback = New NewPolygonFeedback
                    ' Get the new Feedback's symbol by reference
                    pSLineSymFeed = m_pNewPolyFeedback.Symbol

                    pRGB = New RgbColor
                    ' Make a color
                    With pRGB
                        .Red = 250
                        .Green = 25
                        .Blue = 25
                    End With
                    pSLineSymFeed.Width = 2
                    ' Setup the symbol with color and style
                    pSLineSymFeed.Color = pRGB
                    pSLineSymFeed.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSDot
                    'Set the new Feedback's Display and StartPoint
                    m_pNewPolyFeedback.Display = My.ArcMap.Document.ActiveView.ScreenDisplay
                    m_pNewPolyFeedback.Start(pMPnt)
                    pSLineSymFeed = Nothing
                    pRGB = Nothing

                Else
                    m_pNewPolyFeedback.AddPoint(pMPnt)

                End If

            ElseIf m_pSketchType = SketchType.Polyline Then
                If m_pNewLineFeedback Is Nothing Then
                    Dim temp_polyline As IPolyline = New PolylineClass
                    m_line_points = temp_polyline
                    Dim pSLineSymFeed As ISimpleLineSymbol
                    Dim pRGB As IRgbColor

                    m_pNewLineFeedback = New NewLineFeedback
                    ' Get the new Feedback's symbol by reference
                    pSLineSymFeed = m_pNewLineFeedback.Symbol

                    pRGB = New RgbColor
                    ' Make a color
                    With pRGB
                        .Red = 250
                        .Green = 25
                        .Blue = 25
                    End With
                    pSLineSymFeed.Width = 2
                    ' Setup the symbol with color and style
                    pSLineSymFeed.Color = pRGB
                    pSLineSymFeed.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSDot
                    'Set the new Feedback's Display and StartPoint
                    m_pNewLineFeedback.Display = My.ArcMap.Document.ActiveView.ScreenDisplay
                    m_pNewLineFeedback.Start(pMPnt)
                    pSLineSymFeed = Nothing
                    pRGB = Nothing

                Else
                    m_pNewLineFeedback.AddPoint(pMPnt)

                End If
                m_line_points.AddPoint(pMPnt)



            End If
            pMPnt = Nothing
            pSnapenv = Nothing


        Catch ex As Exception
            MsgBox("Error in the Costing Tools: OnDblClick - CIPSketchNewCandidate" & vbCrLf & ex.ToString())

        End Try

        MyBase.OnMouseDown(arg)
    End Sub
    Protected Overrides Sub OnMouseMove(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)

        Try

            If m_pSketchType = SketchType.none Then Return

            If m_bMouseHasMoved Then
                If My.Globals.Variables.v_Editor IsNot Nothing Then
                    If My.Globals.Variables.v_Editor.EditState <> esriEditState.esriStateNotEditing Then
                        My.Globals.Variables.v_Editor.InvertAgent(m_pPt, 0) 'erase the old agent
                    End If
                End If



            End If
            m_bMouseHasMoved = True

            m_pPt = My.ArcMap.ThisApplication.Display.DisplayTransformation.ToMapPoint(arg.X, arg.Y)

            '+++ get the snap agent, if it is being used

            Dim pSnapenv As ISnapEnvironment
            If My.Globals.Variables.v_Editor IsNot Nothing Then
                If My.Globals.Variables.v_Editor.EditState <> esriEditState.esriStateNotEditing Then


                    pSnapenv = TryCast(My.Globals.Variables.v_Editor, ISnapEnvironment)
                    pSnapenv.SnapPoint(m_pPt)

                    '+++ draws the agent
                    My.Globals.Variables.v_Editor.InvertAgent(m_pPt, 0)
                End If
            End If
            pSnapenv = Nothing

            If m_pSketchType = SketchType.Polygon Then

                If m_pNewPolyFeedback IsNot Nothing Then
                    m_pNewPolyFeedback.MoveTo(m_pPt)
                End If

            ElseIf m_pSketchType = SketchType.Polyline Then

                If m_pNewLineFeedback IsNot Nothing Then
                    m_pNewLineFeedback.MoveTo(m_pPt)
                    UpdateMapTip(arg)

                End If


            End If



        Catch ex As Exception
            MsgBox("Error in the Costing Tools - OnMouseMove - CIPSketchNewCandidate" & vbCrLf & ex.ToString())


        End Try
        MyBase.OnMouseMove(arg)
    End Sub
    Protected Overrides Sub OnRefresh(ByVal hDC As Integer)

        Try

            If m_pNewLineFeedback IsNot Nothing Then
                m_pNewLineFeedback.Refresh(hDC)
            End If
            If m_pNewPolyFeedback IsNot Nothing Then
                m_pNewPolyFeedback.Refresh(hDC)

            End If


            m_bMouseHasMoved = False
            MyBase.OnRefresh(hDC)
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - Refresh - CIPSketchNewCandidate" & vbCrLf & ex.ToString())

        End Try

    End Sub
#End Region
#Region "Enum's"
    Private Enum SketchType
        Polygon
        Polyline
        Point
        none
    End Enum
#End Region
#Region "Private Functions"
    Private Sub destroyMapTip()
        If Not m_lengthMaptip Is Nothing Then
            m_lengthMaptip.Visible = False
            m_lengthMaptip = Nothing
        End If


    End Sub

    Private Sub finishSketch()
        Try
            destroyMapTip()
            If m_pSketchType = SketchType.Point Then

            ElseIf m_pSketchType = SketchType.Polygon Then
                Dim pGeompoly As IGeometry

                'Get the geometry (Polygon) returned from the feedback
                pGeompoly = m_pNewPolyFeedback.Stop
                ArcGIS4LocalGovernment.CostEstimatingWindow.AddGraphicSketch(pGeompoly)

                If m_bMouseHasMoved Then
                    My.Globals.Variables.v_Editor.InvertAgent(m_pPt, 0) 'erase the old agent
                    m_bMouseHasMoved = False
                    m_pPt = Nothing
                End If
                m_pNewPolyFeedback = Nothing
                pGeompoly = Nothing

            ElseIf m_pSketchType = SketchType.Polyline Then

                Dim pGeompoly As IGeometry

                'Get the geometry (Polygon) returned from the feedback
                pGeompoly = m_pNewLineFeedback.Stop
                ArcGIS4LocalGovernment.CostEstimatingWindow.AddGraphicSketch(pGeompoly)
                If m_bMouseHasMoved Then
                    My.Globals.Variables.v_Editor.InvertAgent(m_pPt, 0) 'erase the old agent
                    m_bMouseHasMoved = False
                    m_pPt = Nothing
                End If
                pGeompoly = Nothing

                m_pNewLineFeedback = Nothing

            End If


        Catch ex As Exception
            m_pNewLineFeedback = Nothing
            m_pNewPolyFeedback = Nothing
            MsgBox("Error in the Costing Tools: OnDblClick - CIPSketchNewCandidate" & vbCrLf & ex.ToString())

        End Try
    End Sub
    Private Sub UpdateMapTip(arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        If m_lengthMaptip Is Nothing Then Return
        Dim pMPnt As IPoint = My.ArcMap.ThisApplication.Display.DisplayTransformation.ToMapPoint(arg.X, arg.Y)

        'Dim screenDisplay As ESRI.ArcGIS.Display.IScreenDisplay = My.ArcMap.Document.ActiveView.ScreenDisplay

        'Dim displayTransformation As ESRI.ArcGIS.Display.IDisplayTransformation = screenDisplay.DisplayTransformation

        'My.ArcMap.ThisApplication.Display.DisplayTransformation.FromMapPoint(pMPnt, x, y)
        'displayTransformation.FromMapPoint(pMPnt, x, y)

        m_lengthMaptip.SetLabel(getLength(pMPnt))
        m_lengthMaptip.Top = System.Windows.Forms.Cursor.Position.Y - 20
        m_lengthMaptip.Left = System.Windows.Forms.Cursor.Position.X + 5


        m_lengthMaptip.Visible = True

    End Sub
    Private m_lengthMaptip As LengthTip
    Private Sub resetType()

        Try


            'Initialize address map tip
            m_lengthMaptip = Nothing
            Dim pTarget As ArcGIS4LocalGovernment.CostEstimatingWindow.layerAndTypes = ArcGIS4LocalGovernment.CostEstimatingWindow.GetTarget
            If pTarget Is Nothing Then
                m_pSketchType = SketchType.none
            ElseIf pTarget.getGeoType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon Then
                m_pSketchType = SketchType.Polygon


            ElseIf pTarget.getGeoType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint Then
                m_pSketchType = SketchType.Point

            ElseIf pTarget.getGeoType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline Then
                m_pSketchType = SketchType.Polyline
                m_lengthMaptip = New LengthTip()

                Dim mxPtr As IntPtr = New IntPtr(My.ArcMap.Document.ActiveView.ScreenDisplay.hWnd)
                m_lengthMaptip.Show(Control.FromHandle(mxPtr))
                m_lengthMaptip.Visible = False

            End If
            pTarget = Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools: resetType - CIPSketchNewCandidate" & vbCrLf & ex.ToString())

        End Try

    End Sub


    'Private Function CompareFC(ByVal fc As Ifeatureclass) As Boolean
    '    Try
    '        '************************************************************************************
    '        'Produce by: Michael Miller                                                         *
    '        'Purpose:    Compares Featureclasses                   *
    '        '************************************************************************************

    '        Dim pEnumLayer As IEnumLayer
    '        Dim pLay As ILayer


    '        pEnumLayer = m_pMxDoc.FocusMap.Layers(Nothing, True)
    '        pEnumLayer.Reset()
    '        pLay = pEnumLayer.Next
    '        Do Until pLay Is Nothing
    '            If TypeOf pLay Is IFeatureLayer Then
    '                If CType(pLay, IFeatureLayer).FeatureClass Is fc Then
    '                    Return True
    '                End If
    '            End If

    '            pLay = pEnumLayer.Next
    '        Loop


    '        pEnumLayer = Nothing
    '        pLay = Nothing

    '        Return False
    '    Catch ex As Exception
    '        MsgBox("Error in the Costing Tools - CompareFC" & vbCrLf & ex.ToString())
    '        Return False
    '    End Try
    'End Function

    Private Function getFeatureClass(ByVal layerName) As IFeatureClass
        Try
            '************************************************************************************
            'Produce by: Michael Miller                                                         *
            'Purpose:    Compares Featureclasses                   *
            '************************************************************************************

            Dim pLay As ILayer = My.Globals.Functions.FindLayer(layerName)
            If pLay IsNot Nothing Then
                Return CType(pLay, IFeatureLayer).FeatureClass
            End If

            Return Nothing

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSketchNewCandidate: getFeatureClass" & vbCrLf & ex.ToString())
            Return Nothing
        End Try
    End Function
    Private Sub setSnappingForLayer()
        Try


            If My.Globals.Variables.v_Editor IsNot Nothing Then
                If My.Globals.Variables.v_Editor.EditState <> esriEditState.esriStateNotEditing Then

                    Dim pSnapFd As Boolean = False
                    Dim pSnapAg As ISnapAgent
                    Dim pFeatSnapAg As IFeatureSnapAgent = Nothing
                    Dim pSnapEn As ISnapEnvironment
                    pSnapEn = TryCast(My.Globals.Variables.v_Editor, ISnapEnvironment)
                    For i = 0 To pSnapEn.SnapAgentCount - 1
                        pSnapAg = pSnapEn.SnapAgent(i)
                        If TypeOf pSnapAg Is FeatureSnap Then
                            pFeatSnapAg = TryCast(pSnapAg, IFeatureSnapAgent)
                            If getFeatureClass(ArcGIS4LocalGovernment.CostEstimatingWindow.GetSketchFeatureName) Is pFeatSnapAg.FeatureClass Then
                                pFeatSnapAg.HitType = ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary + ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartVertex + ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartEndpoint
                                pSnapFd = True
                                Exit For

                            End If
                        End If


                    Next
                    If pSnapFd = False Then
                        pFeatSnapAg = New FeatureSnap


                        pFeatSnapAg.FeatureClass = getFeatureClass(ArcGIS4LocalGovernment.CostEstimatingWindow.GetSketchFeatureName)
                        pFeatSnapAg.HitType = ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary + ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartVertex + ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartEndpoint
                        pSnapEn.AddSnapAgent(pFeatSnapAg)

                    End If
                    pSnapAg = Nothing
                    pFeatSnapAg = Nothing
                    pSnapEn = Nothing

                    RefreshSnapWindow()

                End If

            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSketchNewCandidate: setSnappingForLayer" & vbCrLf & ex.ToString())
        End Try
    End Sub
    Private Function getLength(curPnt As IPoint) As Double


        Dim pClone As IClone = m_line_points
        Dim tempPC As IPointCollection = pClone.Clone()
        tempPC.AddPoint(curPnt)
        Dim polyline As IPolyline = tempPC
        polyline.SpatialReference = m_pSR
        If TypeOf (m_pSR) Is IProjectedCoordinateSystem Then

            Dim pCurDes As IPolycurveGeodetic = polyline

            If Not pCurDes Is Nothing Then
                Dim pProjSys As IProjectedCoordinateSystem = m_pSR
                Return pCurDes.LengthGeodetic(0, pProjSys.CoordinateUnit)


            End If
        Else
            Dim curve As ICurve = polyline
            Return curve.Length
        End If
    End Function
    Private Sub RefreshSnapWindow()
        Try


            Dim snapWindowUID As New UID
            snapWindowUID.Value = "esriEditor.SnappingWindow"
            Dim snapWindow As ISnappingWindow
            snapWindow = CType(My.Globals.Variables.v_Editor.FindExtension(snapWindowUID), ISnappingWindow)
            snapWindow.RefreshContents()

            snapWindowUID = Nothing
            snapWindow = Nothing
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSketchNewCandidate: RefreshSnapWindow" & vbCrLf & ex.ToString())
        End Try
    End Sub
#End Region

End Class
