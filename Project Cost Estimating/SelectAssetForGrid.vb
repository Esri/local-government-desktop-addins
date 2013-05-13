
' | Version 10.2
' | Copyright 2013 Esri
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


Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.SystemUI

Public Class SelectAssetForGrid
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool

    Public Sub New()

    End Sub
#Region "Overrides"
    Protected Overrides Sub OnMouseDown(ByVal arg As MouseEventArgs)

        Try


            If arg.Button = Windows.Forms.MouseButtons.Left Then

                Dim pEnv As IEnvelope
                Dim pRubberEnv As IRubberBand
                Try

                    pRubberEnv = New RubberEnvelope


                    ' Return a new Polygon from the tracker object using TrackNew
                    pEnv = TryCast(pRubberEnv.TrackNew(My.ArcMap.Document.ActiveView.ScreenDisplay, Nothing), IEnvelope)
                    pRubberEnv = Nothing
                    If pEnv Is Nothing Then Return

                Catch ex As Exception
                    pRubberEnv = Nothing
                    MsgBox("Error in the Costing Tools: OnMouseDown - Drawing Selection Envelope" & vbCrLf & ex.Message)

                    Return

                End Try
                Try

                    CostEstimatingWindow.LoadAssetsByShape(pEnv)

                Catch ex As Exception
                    MsgBox("Error in the Costing Tools: OnMouseDown - Costing Assets" & vbCrLf & ex.Message)

                End Try


            End If
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSelectAssetForCandidate: OnMouseDown" & vbCrLf & ex.Message)
        End Try

    End Sub
    Protected Overrides Sub OnActivate()
        MyBase.OnActivate()
        Dim pDockWin As DockableWindow = TryCast(CostEstimatingExtension.GetCostEstimatingWindow, DockableWindow)
        If pDockWin Is Nothing Then Return
        pDockWin.Show(True)
        CostEstimatingWindow.SelectTool(CostEstimatingWindow.CIPTools.SelectAssetsForGrid, True)
    End Sub
    Protected Overrides Sub OnUpdate()
        Me.Enabled = My.Globals.Variables.v_CIPWindowsValid

    End Sub
    Protected Overrides Function OnContextMenu(ByVal x As Integer, ByVal y As Integer) As Boolean
        Dim pShortCut As ICommandBar
        Try

            pShortCut = My.Globals.Functions.CreateContextMenu("ArcGIS4LocalGovernment_SelectedMultiItem")
            ' SelectedMultiItem.s_PopUpLocation = My.ArcMap.Document.CurrentLocation

            pShortCut.Popup(0, 0)

            Return True
        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPSelectAssetForCandidate: OnContextMenu" & vbCrLf & ex.Message)
            Return False
        Finally
            pShortCut = Nothing

        End Try


    End Function
    Protected Overrides Function OnDeactivate() As Boolean
        Try
            CostEstimatingWindow.SelectTool(CostEstimatingWindow.CIPTools.SelectAssetsForGrid, False)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPHighlightCandidate: Deactivate" & vbCrLf & ex.Message)
        Finally

        End Try

        Return MyBase.OnDeactivate()
    End Function



#End Region
#Region "Private Functions"


#End Region

End Class
