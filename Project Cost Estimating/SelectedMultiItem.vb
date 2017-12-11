
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


Public Class SelectedMultiItem
  Inherits ESRI.ArcGIS.Desktop.AddIns.MultiItem
    '  Friend Shared s_PopUpLocation As ESRI.ArcGIS.Geometry.IPoint

    Public Sub New()

        'AddItem("ESRI.ResourceCenterTemplates.CIPSelectCandidateAtLocation")
        '        BeginGroup() 'Separator
        '        '  AddItem("ESRI.ResourceCenterTemplates.CIPTrimClosestCandidate")
        '        AddItem("ESRI.ResourceCenterTemplates.CIPTrimSelectedCandidate")
        '        AddItem("ESRI.ResourceCenterTemplates.CIPSplitCandidateAtLocation")

    End Sub

  Protected Overrides Sub OnPopup(ByVal items As ItemCollection)
        Dim item As New Item()
        item.Caption = "Trim selected costed asset"
        item.Enabled = True
        item.Message = "Trim selected costed asset"
        item.Tag = "Trim"
        item.Image = My.Resources.split
        items.Add(item)


        item = New Item()
        item.Caption = "Split selected costed asset"
        item.Enabled = True
        item.Message = "Split selected costed asset"
        item.Tag = "Split"
        item.Image = My.Resources.splitSelected
        items.Add(item)

        item = New Item()
        item.Caption = "Select costed asset at this location"
        item.Enabled = True
        item.Message = "Select costed asset at this location"
        item.Tag = "Select"
        item.Image = My.Resources.CIPSelectAssetSmall
        items.Add(item)


  End Sub

  Protected Overrides Sub OnClick(ByVal item As Item)
        Select Case TryCast(item.Tag, String)
            Case "Trim"
                's_PopUpLocation = s_PopUpLocation
                ArcGIS4LocalGovernment.CostEstimatingWindow.SplitLines(My.ArcMap.Document.CurrentLocation, False)
            Case "Split"
                ArcGIS4LocalGovernment.CostEstimatingWindow.SplitLines(My.ArcMap.Document.CurrentLocation, True)
            Case "Select"
                ArcGIS4LocalGovernment.CostEstimatingWindow.HighlightAtLocation(My.ArcMap.Document.CurrentLocation)
        End Select

    End Sub
End Class