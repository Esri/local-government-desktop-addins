
' | Version 10.4
' | Copyright 2016 Esri
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

Public Class SelectExistingProject
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool

    Public Sub New()

    End Sub
    Protected Overrides Sub OnUpdate()
        Me.Enabled = My.Globals.Variables.v_CIPWindowsValid

    End Sub
    Protected Overrides Sub OnActivate()
        Dim pDockWin As DockableWindow = TryCast(CostEstimatingExtension.GetCostEstimatingWindow, DockableWindow)
        If pDockWin Is Nothing Then Return
        pDockWin.Show(True)
        MyBase.OnActivate()
        CostEstimatingWindow.SelectTool(CostEstimatingWindow.CIPTools.SelectExistingProject, True)
    End Sub
    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        CostEstimatingWindow.LoadProjectFromLocation(My.ArcMap.Document.CurrentLocation)

        MyBase.OnMouseDown(arg)
    End Sub
    Protected Overrides Function OnDeactivate() As Boolean
        Try
            CostEstimatingWindow.SelectTool(CostEstimatingWindow.CIPTools.SelectExistingProject, False)

        Catch ex As Exception
            MsgBox("Error in the Costing Tools - CIPHighlightCandidate: Deactivate" & vbCrLf & ex.Message)
        Finally

        End Try

        Return MyBase.OnDeactivate()
    End Function

End Class
