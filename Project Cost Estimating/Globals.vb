
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


Option Strict Off
Option Explicit On

Imports System.Drawing
Imports System.IO
Imports System.Xml

Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.SystemUI

Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports Microsoft.Win32

Namespace My



    Friend Class Globals
        Friend Class Constants


            Public Shared c_Fnt As Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
            Public Shared c_FntLbl As Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
            Public Shared c_FntSmall As Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))



            Public Shared c_ControlWidth As Integer = 200
            Public Shared c_HighlightColor As Integer = RGB(0, 255, 255)
            Public Shared c_DrawColor As Integer = RGB(0, 0, 0)
            Public Shared c_OutDrawColor As Integer = RGB(255, 0, 0)

            'CIP Def Table
            Public Shared c_CIPDefTableName As String = "piCIPDefinition"
            Public Shared c_CIPDefNameField As String = "ASSETNA"
            Public Shared c_CIPDefLenField As String = "LENFIELD"
            Public Shared c_CIPDefIDField As String = "ID"
            Public Shared c_CIPDefFiltField1 As String = "FILTFLD1"
            Public Shared c_CIPDefFiltField2 As String = "FILTFLD2"
            Public Shared c_CIPDefActiveField As String = "ACTIVE"
            Public Shared c_CIPDefNotesField As String = "NOTES"
            Public Shared c_CIPDefMultiField As String = "MULTI"
            Public Shared c_CIPDefIntersectField As String = "INTERSCT"

            'CIP Cost Table
            Public Shared c_CIPCostTableName As String = "piCIPCost"
            Public Shared c_CIPCostNameField As String = "ASSETNA"
            Public Shared c_CIPCostCostField As String = "COST"
            Public Shared c_CIPCostAddCostField As String = "ADDCOST"
            Public Shared c_CIPCostFiltField1 As String = "FILTVAL1"
            Public Shared c_CIPCostFiltField2 As String = "FILTVAL2"
            Public Shared c_CIPCostNotesField As String = "NOTES"
            Public Shared c_CIPCostStrategyField As String = "STRATEGY"
            Public Shared c_CIPCostActionField As String = "ACTION"
            Public Shared c_CIPCostIntersectCostField As String = "INTERCST"
            '    Public shared c_CIPCostAdvFiltField As String = "ADVFILT"


            'CIP Replacement Table
            Public Shared c_CIPReplaceTableName As String = "piCIPReplacement"
            Public Shared c_CIPReplaceNameField As String = "ASSETNA"
            Public Shared c_CIPReplaceFiltField1 As String = "FILTVAL1"
            Public Shared c_CIPReplaceFiltField2 As String = "FILTVAL2"
            Public Shared c_CIPReplaceDefField1 As String = "DEFVAL1"
            Public Shared c_CIPReplaceDefField2 As String = "DEFVAL2"
            Public Shared c_CIPReplaceNotesField As String = "NOTES"
            Public Shared c_CIPReplaceActionField As String = "ACTION"
            '  Public shared c_CIPReplaceAdvFiltField As String = "ADVFILT"
            '  Public shared c_CIPReplaceActiveField As String = "ACTIVE"


            'CIP Asset Layer - Points, Lines and Polygon
            Public Shared c_CIPProjectPointLayName As String = "CIPPoints"
            Public Shared c_CIPProjectPolylineLayName As String = "CIPPolylines"
            Public Shared c_CIPProjectPolygonLayName As String = "CIPPolygons"

            'CIP Asset Layer - Points, Lines and Polygon fields
            Public Shared c_CIPProjectAssetNameField As String = "PROJNAME"
            Public Shared c_CIPProjectAssetCostField As String = "COST"
            Public Shared c_CIPProjectAssetAddCostField As String = "ADDCOST"
            Public Shared c_CIPProjectAssetIDField As String = "FACILITYID"
            Public Shared c_CIPProjectAssetTypeField As String = "ASSETTYP"
            Public Shared c_CIPProjectAssetTotCostField As String = "TOTCOST"
            Public Shared c_CIPProjectAssetExistingFilt1Field As String = "EXTFILT1"
            Public Shared c_CIPProjectAssetExistingFilt2Field As String = "EXTFILT2"
            Public Shared c_CIPProjectAssetProposedFilt1Field As String = "PROFILT1"
            Public Shared c_CIPProjectAssetProposedFilt2Field As String = "PROFILT2"
            Public Shared c_CIPProjectAssetNotesField As String = "NOTES"
            Public Shared c_CIPProjectAssetOIDField As String = "SOURCOID"
            Public Shared c_CIPProjectAssetMultiField As String = "MULTI"
            Public Shared c_CIPProjectAssetStrategyField As String = "STRATEGY"
            Public Shared c_CIPProjectAssetActionField As String = "ACTION"
            'CIP Asset Layer - Lines fields
            Public Shared c_CIPProjectAssetLenField As String = "LENGTH"




            'CIP Project Layer and CIP Overview
            Public Shared c_CIPProjectLayName As String = "CIPProjects"
            Public Shared c_CIPOverviewLayName As String = "CIPProjectsOverview"
            Public Shared c_CIPOverviewPointLayName As String = "CIPProjectsLocations"
            'CIP Project Layer and CIP Overview Fields
            Public Shared c_CIPProjectLayNameField As String = "PROJNAME"
            Public Shared c_CIPProjectLayCostField As String = "TOTCOST"
            Public Shared c_CIPProjectLayTotLenField As String = "TOTLEN"
            Public Shared c_CIPProjectLayTotAreaField As String = "TOTAREA"
            Public Shared c_CIPProjectLayTotPntField As String = "TOTPNT"
        End Class
        Friend Class Variables

            Public Shared v_CIPWindowsValid As Boolean = False


            Public Shared v_LastSelection As String
            Public Shared v_intSketchID As Integer = -1
            Public Shared v_SaveEnabled As Boolean = False


            Public Shared v_PolygonGraphicSymbol As ISimpleFillSymbol
            Public Shared v_LineGraphicSymbol As IMultiLayerLineSymbol
            Public Shared v_PointGraphicSymbol As IMultiLayerMarkerSymbol
            Public Shared v_PolygonHighlightGraphicSymbol As ISimpleFillSymbol
            Public Shared v_LineHighlightGraphicSymbol As ISimpleLineSymbol
            Public Shared v_PointHighlightGraphicSymbol As ISimpleMarkerSymbol

            Public Shared v_CIPLayerPrj As IFeatureLayer = Nothing
            Public Shared v_CIPLayerOver As IFeatureLayer = Nothing
            Public Shared v_CIPLayerOverPoint As IFeatureLayer = Nothing

            Public Shared v_CIPLayerPoint As IFeatureLayer = Nothing
            Public Shared v_CIPLayerPolygon As IFeatureLayer = Nothing
            Public Shared v_CIPLayerPolyline As IFeatureLayer = Nothing
            Public Shared v_CIPTableDef As ITable = Nothing
            Public Shared v_CIPTableCost As ITable = Nothing
            Public Shared v_CIPTableReplace As ITable = Nothing

            Public Shared v_CIPReplaceValue As String = Nothing
            Public Shared v_CIPAbandonValue As String = Nothing

            Public Shared v_Editor As IEditor2 = Nothing

            Public Shared WithEvents v_EditorEvents As Editor = Nothing
            Private Shared Sub m_EditorEvents_OnStartEditing() Handles v_EditorEvents.OnStartEditing
                CostEstimatingWindow.EditingStarted()
            End Sub
            Private Shared Sub m_EditorEvents_OnStopEditing(ByVal save As Boolean) Handles v_EditorEvents.OnStopEditing
                CostEstimatingWindow.EditingStopped()
            End Sub
        End Class
        Friend Class Functions
            Friend Shared Function GetMapCoordinatesFromScreenCoordinates(ByVal screenPoint As ESRI.ArcGIS.Geometry.IPoint) As ESRI.ArcGIS.Geometry.IPoint
                Try

                    If screenPoint Is Nothing OrElse screenPoint.IsEmpty Then
                        Return Nothing
                    End If

                    Dim screenDisplay As ESRI.ArcGIS.Display.IScreenDisplay = My.ArcMap.Document.ActiveView.ScreenDisplay
                    Dim displayTransformation As ESRI.ArcGIS.Display.IDisplayTransformation = screenDisplay.DisplayTransformation

                    Return displayTransformation.ToMapPoint(CInt(Fix(screenPoint.X)), CInt(Fix(screenPoint.Y)))
                Catch ex As Exception
                    MsgBox("Error in the Project Costing Tools - Globals.Functions: GetMapCoordinatesFromScreenCoordinates" & vbCrLf & ex.ToString())
                End Try

            End Function
            Friend Shared Function GetScreenCoordinatesFromMapCoordinates(ByVal MapPoint As ESRI.ArcGIS.Geometry.IPoint) As System.Drawing.Point
                Try


                    If MapPoint Is Nothing OrElse MapPoint.IsEmpty Then
                        Return Nothing
                    End If

                    Dim screenDisplay As ESRI.ArcGIS.Display.IScreenDisplay = My.ArcMap.Document.ActiveView.ScreenDisplay
                    Dim displayTransformation As ESRI.ArcGIS.Display.IDisplayTransformation = screenDisplay.DisplayTransformation
                    Dim pScPnt As System.Drawing.Point = New System.Drawing.Point
                    displayTransformation.FromMapPoint(MapPoint, pScPnt.X, pScPnt.Y)
                    Return pScPnt

                Catch ex As Exception
                    MsgBox("Error in Project Costing Tools - Globals.Functions: GetScreenCoordinatesFromMapCoordinates" & vbCrLf & ex.ToString())
                End Try

            End Function
            Friend Shared Function getClassName(ByVal Dataset As IDataset) As String

                If Dataset.BrowseName <> "" And Dataset.BrowseName.Contains(".") Then
                    Return Dataset.BrowseName.Substring(Dataset.BrowseName.LastIndexOf(".") + 1)

                ElseIf Dataset.FullName.NameString <> "" And Dataset.FullName.NameString.Contains(".") Then
                    Return Dataset.FullName.NameString.Substring(Dataset.FullName.NameString.LastIndexOf(".") + 1)
                ElseIf Dataset.Name <> "" And Dataset.Name.Contains(".") Then
                    Return Dataset.Name.Substring(Dataset.Name.LastIndexOf(".") + 1)

                ElseIf Dataset.BrowseName <> "" Then

                    Return Dataset.BrowseName
                ElseIf Dataset.FullName.NameString <> "" Then
                    Return Dataset.FullName.NameString

                Else
                    Return Dataset.Name
                End If
            End Function
            Friend Shared Function FindLayers(ByVal sLName As String) As ArrayList
                Try
                    '************************************************************************************
                    'Produce by: Michael Miller                                                         *
                    'Purpose:    To return a refernece to a layer specified by sLName                   *
                    '************************************************************************************
                    If sLName = "" Then Return Nothing

                    Dim pEnumLayer As IEnumLayer
                    Dim pLay As ILayer
                    Dim pDataset As IDataset
                    Dim pRetLay As ArrayList = New ArrayList

                    pEnumLayer = My.ArcMap.Document.FocusMap.Layers(Nothing, True)
                    pEnumLayer.Reset()
                    pLay = pEnumLayer.Next
                    Do Until pLay Is Nothing
                        If UCase(pLay.Name) = UCase(sLName) Then
                            pRetLay.Add(pLay)

                        ElseIf TypeOf pLay Is IDataset Then
                            pDataset = pLay
                            If UCase(pDataset.BrowseName) = UCase(sLName) Then
                                pRetLay.Add(pLay)

                            ElseIf UCase(pDataset.FullName.NameString) = UCase(sLName) Then
                                pRetLay.Add(pLay)

                            ElseIf UCase(pDataset.BrowseName).Substring(pDataset.BrowseName.LastIndexOf(".") + 1) = UCase(sLName) Then
                                pRetLay.Add(pLay)

                            ElseIf UCase(pDataset.FullName.NameString).Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) = UCase(sLName) Then
                                pRetLay.Add(pLay)

                            End If
                        End If

                        pLay = pEnumLayer.Next
                    Loop


                    pEnumLayer = Nothing
                    pLay = Nothing

                    Return pRetLay
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - FindLayers" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Function FindLayer(ByVal sLName As String) As ILayer
                Try
                    '************************************************************************************
                    'Produce by: Michael Miller                                                         *
                    'Purpose:    To return a refernece to a layer specified by sLName                   *
                    '************************************************************************************
                    If sLName = "" Then Return Nothing

                    Dim pEnumLayer As IEnumLayer
                    Dim pLay As ILayer
                    Dim pDataset As IDataset

                    pEnumLayer = My.ArcMap.Document.FocusMap.Layers(Nothing, True)
                    pEnumLayer.Reset()
                    pLay = pEnumLayer.Next
                    Do Until pLay Is Nothing
                        If UCase(pLay.Name) = UCase(sLName) Then
                            FindLayer = pLay
                            Exit Function
                        End If
                        If TypeOf pLay Is IDataset Then
                            pDataset = pLay
                            If UCase(pDataset.BrowseName) = UCase(sLName) Then
                                FindLayer = pLay
                                Exit Function

                            End If
                            If UCase(pDataset.FullName.NameString) = UCase(sLName) Then
                                FindLayer = pLay
                                Exit Function

                            End If
                            If UCase(pDataset.BrowseName).Substring(pDataset.BrowseName.LastIndexOf(".") + 1) = UCase(sLName) Then
                                FindLayer = pLay
                                Exit Function

                            End If
                            If UCase(pDataset.FullName.NameString).Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) = UCase(sLName) Then
                                FindLayer = pLay
                                Exit Function

                            End If
                        End If

                        pLay = pEnumLayer.Next
                    Loop


                    pEnumLayer = Nothing
                    pLay = Nothing

                    Return Nothing
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - FindLayer" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Function FindTable(ByVal sLName As String) As ITable
                Try

                    If sLName = "" Then Return Nothing
                    Dim pDS As IDataset
                    Dim pTableCollection As ITableCollection


                    pTableCollection = My.ArcMap.Document.FocusMap
                    For i As Integer = 0 To pTableCollection.TableCount - 1
                        pDS = pTableCollection.Table(i)
                        If pDS IsNot Nothing Then
                            If UCase(pDS.Name) = UCase(sLName) Then
                                Return pDS

                            End If
                            If UCase(pDS.Name.Substring(pDS.Name.LastIndexOf(".") + 1)) = UCase(sLName) Then
                                Return pDS

                            End If
                        End If

                    Next i
                    pDS = Nothing
                    pTableCollection = Nothing
                    Return Nothing
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - FindTable" & vbCrLf & ex.ToString())
                    Return Nothing

                End Try
            End Function
            Friend Shared Function TableExist(ByVal sLName As String) As Boolean
                Try
                    If sLName = "" Then Return False


                    Dim pDS As IDataset
                    Dim pTableCollection As ITableCollection


                    pTableCollection = My.ArcMap.Document.FocusMap
                    For i As Integer = 0 To pTableCollection.TableCount - 1
                        pDS = pTableCollection.Table(i)
                        'Dim pDispTable2 As IDisplayTable
                        'Dim pTbl As ITable = pDS



                        If pDS IsNot Nothing Then
                            ' If TypeOf pDS Is IStandaloneTable Then


                            '  Dim pTbl As IStandaloneTable = pDS

                            If UCase(pDS.Name) = UCase(sLName) Then
                                Return True 'pTbl.Valid

                            End If
                            If UCase(pDS.Name.Substring(pDS.Name.LastIndexOf(".") + 1)) = UCase(sLName) Then
                                Return True 'pTbl.Valid

                            End If
                            'pTbl = Nothing
                            'End If
                        End If
                    Next i
                    pDS = Nothing
                    pTableCollection = Nothing
                    Return False
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - TableExist" & vbCrLf & ex.ToString())
                    Return False

                End Try
            End Function
            Friend Shared Function LayerExist(ByVal sLName As String) As Boolean
                '************************************************************************************
                'Produce by: Michael Miller                                                         *
                'Purpose:    To return a refernece to a layer specified by sLName                   *
                '************************************************************************************
                Try

                    If sLName = "" Then Return False
                    Dim pEnumLayer As IEnumLayer
                    Dim pLay As ILayer
                    Dim pDataset As IDataset

                    pEnumLayer = My.ArcMap.Document.FocusMap.Layers(Nothing, True)
                    pEnumLayer.Reset()
                    pLay = pEnumLayer.Next
                    Do Until pLay Is Nothing
                        Try


                            If pLay.Valid Then

                                If UCase(pLay.Name) = UCase(sLName) Then

                                    Return True

                                End If
                                If TypeOf pLay Is IDataset Then
                                    pDataset = pLay
                                    If UCase(pDataset.BrowseName) = UCase(sLName) Then
                                        Return True


                                    End If
                                    If UCase(pDataset.FullName.NameString) = UCase(sLName) Then
                                        Return True

                                    End If
                                    If UCase(pDataset.FullName.NameString).Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) = UCase(sLName) Then
                                        Return True
                                    End If
                                    If UCase(pDataset.BrowseName).Substring(pDataset.BrowseName.LastIndexOf(".") + 1) = UCase(sLName) Then
                                        Return True
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                        pLay = pEnumLayer.Next
                    Loop


                    pEnumLayer = Nothing
                    pLay = Nothing
                    Return False

                Catch ex As Exception
                    MsgBox("Error in LayerExist: " & ex.ToString())

                    Return False

                End Try

            End Function
            Friend Shared Function CreateContextMenu(ByVal MultiItemRef As String) As ICommandBar
                Dim pContextMenu As ICommandBar
                Dim pUID As ESRI.ArcGIS.esriSystem.UID
                Try

                    pContextMenu = My.ArcMap.Application.Document.CommandBars.Create("StylePopup", ESRI.ArcGIS.SystemUI.esriCmdBarType.esriCmdBarTypeShortcutMenu)
                    pUID = New ESRI.ArcGIS.esriSystem.UID
                    pUID.Value = MultiItemRef
                    pContextMenu.Add(pUID)
                    Return pContextMenu
                Catch ex As Exception
                    MsgBox("Error creating context menu - CreateContext" & vbCrLf & ex.ToString())
                    Return Nothing

                Finally
                    pUID = Nothing
                    pContextMenu = Nothing

                End Try

            End Function
            Friend Shared Function GetCommand(ByVal CommandReference As String) As ESRI.ArcGIS.Framework.ICommandItem
                Dim pUID As New ESRI.ArcGIS.esriSystem.UID
                Try

                    pUID.Value = CommandReference
                    Return My.ArcMap.Application.Document.CommandBars.Find(pUID)

                Catch ex As Exception
                    MsgBox("Error finding command item - GetCommand" & vbCrLf & ex.ToString())
                    Return Nothing

                Finally
                    pUID = Nothing
                End Try


            End Function
            Friend Shared Function IsEditable(ByVal LayerName As String) As Boolean
                Try 'My.Globals.Constants.c_CIPProjectPointLayName)
                    Dim pEditLayers As IEditLayers
                    pEditLayers = My.Globals.Variables.v_Editor

                    Return pEditLayers.IsEditable(My.Globals.Functions.FindLayer(LayerName))

                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - Globals: isEditable" & vbCrLf & ex.ToString())


                End Try
            End Function
            Friend Shared Function get_linear_unit(pGeo As IGeometry) As ILinearUnit
                If TypeOf pGeo.SpatialReference Is IProjectedCoordinateSystem Then
                    Dim pPrjCoord As IProjectedCoordinateSystem

                    pPrjCoord = pGeo.SpatialReference

                    Return pPrjCoord.CoordinateUnit

                Else
                    Dim pGeoCoord As IGeographicCoordinateSystem

                    pGeoCoord = pGeo.SpatialReference

                    Return pGeoCoord.CoordinateUnit
                    'pGeoCoord = Nothing

                End If
            End Function
            Friend Shared Function getMeasureOfGeo(pGeo As IGeometry, useGeodetic As Boolean, unit As ILinearUnit) As Double
                Dim pLinUnit As ILinearUnit = Nothing
                If unit Is Nothing Then
                    pLinUnit = get_linear_unit(pGeo)
                Else
                    pLinUnit = unit
                End If

                Select Case pGeo.GeometryType
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                        Try

                            If useGeodetic = True Then
                                Try
                                    Dim pCurveGeop As IPolycurveGeodetic
                                    pCurveGeop = CType(pGeo, IPolycurve)
                                    Return pCurveGeop.LengthGeodetic(esriGeodeticType.esriGeodeticTypeGeodesic, pLinUnit)
                                Catch ex As Exception
                                    Return CType(pGeo, ICurve).Length
                                End Try
                            Else
                                Return CType(pGeo, ICurve).Length
                            End If

                        Catch ex As Exception
                        End Try
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                        Try
                            If useGeodetic = True Then
                                Try
                                    Dim pPolyGeop As IAreaGeodetic
                                    pPolyGeop = CType(pGeo, IPolygon)
                                    Return Math.Abs(pPolyGeop.AreaGeodetic(esriGeodeticType.esriGeodeticTypeGeodesic, pLinUnit))

                                Catch ex As Exception
                                    Return Math.Abs(CType(pGeo, IArea).Area)

                                End Try
                            Else
                                Return Math.Abs(CType(pGeo, IArea).Area)
                            End If

                        Catch ex As Exception

                        End Try
                End Select
                Return Nothing
            End Function
            Friend Shared Function ConvertFeetToMapUnits(ByVal unitsFeet As Double) As Double


                Dim pUnitConverter As IUnitConverter = New UnitConverter

                If TypeOf My.ArcMap.Document.FocusMap.SpatialReference Is IProjectedCoordinateSystem Then
                    Dim pPrjCoord As IProjectedCoordinateSystem

                    pPrjCoord = My.ArcMap.Document.FocusMap.SpatialReference

                    Return pUnitConverter.ConvertUnits(unitsFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, ConvertUnitType(pPrjCoord.CoordinateUnit))


                    'pPrjCoord = Nothing

                Else
                    Dim pGeoCoord As IGeographicCoordinateSystem

                    pGeoCoord = My.ArcMap.Document.FocusMap.SpatialReference

                    Return pUnitConverter.ConvertUnits(unitsFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, ConvertUnitType(pGeoCoord.CoordinateUnit))
                    'pGeoCoord = Nothing

                End If
            End Function
            Friend Shared Function unitToLinearUnit(unit As String) As ILinearUnit

                Dim spatialReferenceFactory As ISpatialReferenceFactory2 = New SpatialReferenceEnvironment
                Select Case UCase(unit)
                    Case "FEET", "FOOT"
                        Return spatialReferenceFactory.CreateUnit(esriSRUnitType.esriSRUnit_Foot)
                    Case "METER"
                        Return spatialReferenceFactory.CreateUnit(esriSRUnitType.esriSRUnit_Meter)
                    Case "MILE", "MILES"
                        Return spatialReferenceFactory.CreateUnit(esriSRUnitType.esriSRUnit_SurveyMile)
                    Case "KILOMETER"
                        Return spatialReferenceFactory.CreateUnit(esriSRUnitType.esriSRUnit_Kilometer)
                End Select
                Return Nothing
             
            End Function
            Friend Shared Function ConvertUnitType2(ByVal linearUnit As ESRI.ArcGIS.Geometry.ILinearUnit) _
                                                As ESRI.ArcGIS.esriSystem.esriUnits


                Select Case (linearUnit.WKID)
                    Case 109006
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters



                    Case 9102
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees

                    Case 109005
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimeters
                    Case 9003
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet

                    Case 109008

                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriInches
                    Case 9036
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers

                    Case 9001

                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMeters
                    Case 9035
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMiles

                    Case 109007
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMillimeters

                    Case 9030
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriNauticalMiles

                    Case 109002
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriYards


                End Select



            End Function
            Friend Shared Function ConvertUnitType(ByVal Unit As ESRI.ArcGIS.Geometry.IUnit) _
                                            As ESRI.ArcGIS.esriSystem.esriUnits


                Select Case (Unit.FactoryCode)
                    Case 109006
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters



                    Case 9102
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees

                    Case 109005
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimeters
                    Case 9003
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet

                    Case 109008

                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriInches
                    Case 9036
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers

                    Case 9001

                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMeters
                    Case 9035
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMiles

                    Case 109007
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriMillimeters

                    Case 9030
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriNauticalMiles

                    Case 109002
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriYards

                    Case Else
                        Return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet


                End Select



            End Function
            Friend Shared Function Env2Polygon(ByVal pEnv As IEnvelope) As IPolygon
                Dim pPointColl As IPointCollection
                pPointColl = New Polygon
                pPointColl.AddPoint(pEnv.LowerLeft)
                pPointColl.AddPoint(pEnv.UpperLeft)
                pPointColl.AddPoint(pEnv.UpperRight)
                pPointColl.AddPoint(pEnv.LowerRight)
                pPointColl.AddPoint(pEnv.LowerLeft)
                Env2Polygon = pPointColl
                Env2Polygon.SpatialReference = pEnv.SpatialReference
            End Function
            Friend Shared Function ConvertUnits(ByVal distanceInFeet As Double, ByVal mapUnits As ESRI.ArcGIS.esriSystem.esriUnits) As Double
                Try

                    Dim iuc As ESRI.ArcGIS.esriSystem.IUnitConverter = New ESRI.ArcGIS.esriSystem.UnitConverterClass()
                    Dim convertedValue As Single
                    convertedValue = iuc.ConvertUnits(distanceInFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, mapUnits)
                    iuc = Nothing
                    Return convertedValue
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: ConvertUnits" & vbCrLf & ex.ToString())

                    Return 0.0

                End Try
            End Function
            Friend Shared Function DomainToList(ByVal Dom As IDomain) As ArrayList
                Try
                    Dim CodedValue As ICodedValueDomain = Nothing

                    If TypeOf Dom Is ICodedValueDomain Then
                        CodedValue = Dom
                    Else
                        Return Nothing
                    End If

                    Dim pArrList As New ArrayList
                    For i = 0 To CodedValue.CodeCount - 1
                        Dim pDSL As DomSubList = New DomSubList(CodedValue.Value(i), CodedValue.Name(i))
                        'pDSL.Value = CodedValue.Value(i)
                        'pDSL.Display = CodedValue.Name(i)
                        pArrList.Add(pDSL)


                    Next i

                    Return pArrList
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: DomainToList" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Function SubtypeToList(ByVal pSubTypes As ISubtypes) As ArrayList
                Try
                    '  get  the enumeration of all of the subtypes for this feature class
                    Dim pEnumSubTypes As IEnumSubtype
                    Dim lSubT As Long
                    Dim sSubT As String
                    pEnumSubTypes = pSubTypes.Subtypes

                    ' loop through all of the subtypes and bring up a message
                    ' box with each subtype's code and name
                    ' Indicate when sFeature is found (a passed in string var)
                    sSubT = pEnumSubTypes.Next(lSubT)
                    Dim pArrList As New ArrayList
                    While Len(sSubT) > 0
                        Dim pDSL As DomSubList = New DomSubList(lSubT, pSubTypes.SubtypeName(lSubT))
                        'pDSL.Value = lSubT
                        'pDSL.Display = pSubTypes.SubtypeName(lSubT)
                        pArrList.Add(pDSL)

                        sSubT = pEnumSubTypes.Next(lSubT)
                    End While
                    pEnumSubTypes = Nothing

                    Return pArrList
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: SubtypeToList" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Function GetDomainDisplay(ByVal Value As Object, ByVal CodedValue As IDomain) As String
                Try
                    If CodedValue Is Nothing Then Return Value
                    If Not TypeOf (CodedValue) Is ICodedValueDomain Then Return Value
                    Dim pCod As ICodedValueDomain = CodedValue

                    If Value Is DBNull.Value Then
                        Return pCod.Name(0)
                    End If
                    For i = 0 To pCod.CodeCount - 1
                        If pCod.Value(i).ToString() = Value.ToString() Then Return pCod.Name(i)


                    Next i
                    Return Value
                Catch ex As Exception
                    '  MsgBox("Error in the Costing Tools - CIPProjectWindow: GetDomainDisplay" & vbCrLf & ex.ToString())
                    Return Value
                End Try
            End Function
            Friend Shared Function GetDomainValue(ByVal Display As Object, ByVal CodedValue As IDomain) As String

                Try
                    If CodedValue Is Nothing Then Return Display
                    If Not TypeOf (CodedValue) Is ICodedValueDomain Then Return Display
                    Dim pCod As ICodedValueDomain = CodedValue

                    For i = 0 To pCod.CodeCount - 1
                        If pCod.Name(i) = Display Then Return pCod.Value(i)


                    Next i
                    Return Display
                Catch ex As Exception
                    '    MsgBox("Error in the Costing Tools - CIPProjectWindow: GetDomainValue" & vbCrLf & ex.ToString())
                    Return Display
                End Try
            End Function
            Friend Shared Function GetSubtypeValue(ByVal Display As Object, ByVal pSubtypes As ISubtypes) As String

                Try
                    '  get  the enumeration of all of the subtypes for this feature class
                    Dim pEnumSubTypes As IEnumSubtype
                    Dim lSubT As Long
                    Dim sSubT As String
                    pEnumSubTypes = pSubtypes.Subtypes

                    ' loop through all of the subtypes and bring up a message
                    ' box with each subtype's code and name
                    ' Indicate when sFeature is found (a passed in string var)
                    sSubT = pEnumSubTypes.Next(lSubT)
                    Dim pArrList As New ArrayList
                    While Len(sSubT) > 0
                        If Display = pSubtypes.SubtypeName(lSubT) Then
                            pEnumSubTypes = Nothing
                            Return lSubT
                        End If


                        sSubT = pEnumSubTypes.Next(lSubT)
                    End While
                    pEnumSubTypes = Nothing

                    Return -99

                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: GetDomainValue" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Function GetShapeFromGraphic(ByVal Tag As String, ByVal searchPhrase As String) As IGeometry

                Try

                    Dim pElem As IElement
                    Dim pElProp As IElementProperties


                    Try



                        My.ArcMap.Document.ActiveView.GraphicsContainer.Reset()

                        pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next


                        Do Until pElem Is Nothing
                            pElProp = pElem
                            If pElProp.CustomProperty IsNot Nothing Then
                                If pElProp.CustomProperty.ToString.Contains(searchPhrase) Then

                                    Dim strEl As String = pElProp.CustomProperty
                                    If strEl = Tag Then

                                        Return pElem.Geometry



                                    End If
                                End If
                            End If
                            pElem = My.ArcMap.Document.ActiveView.GraphicsContainer.Next
                        Loop



                    Catch ex As Exception
                        MsgBox("Error in the Costing Tools - CIPProjectWindow: GetGraphicShape" & vbCrLf & ex.ToString())

                        Return Nothing
                    Finally
                        pElem = Nothing
                        pElProp = Nothing
                    End Try
                    Return Nothing


                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: GetGraphicShape" & vbCrLf & ex.ToString())

                    Return Nothing

                End Try
            End Function
            Friend Shared Function isVisible(ByVal pLayer As ILayer) As Boolean
                Dim cAncestors As Collection

                cAncestors = FindAncestors(pLayer)
                If cAncestors.Count > 0 Then
                    Dim l As Long
                    For l = 1 To cAncestors.Count
                        Dim pGLayer As IGroupLayer
                        pGLayer = cAncestors.Item(l)
                        If pGLayer.Visible = False Then
                            Return False
                        End If

                    Next l
                Else
                    Return pLayer.Visible
                End If
                Return pLayer.Visible

            End Function
            Friend Shared Function FindAncestors(ByVal pLayer As ILayer) As Collection
                FindAncestors = New Collection
                Dim pParentLayer As ILayer
                pParentLayer = FindParentLayer(pLayer)
                Do Until pParentLayer Is Nothing
                    FindAncestors.Add(pParentLayer)
                    pParentLayer = FindParentLayer(pParentLayer)
                Loop
            End Function
            Friend Shared Function FindParentLayer(ByVal pChildLayer As ILayer, _
                                     Optional ByVal pCandidate As ICompositeLayer = Nothing) As ICompositeLayer
                If pCandidate Is Nothing Then

                    ' IGrouplayer IID
                    Dim pUID As New UID
                    pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}"
                    On Error Resume Next
                    Dim pEnumLayer As IEnumLayer
                    pEnumLayer = My.ArcMap.Document.FocusMap.Layers(pUID, True)
                    If Not pEnumLayer Is Nothing Then
                        Dim pLayer As ILayer
                        pLayer = pEnumLayer.Next
                        Do Until pLayer Is Nothing
                            Dim pParent As ILayer
                            pParent = FindParentLayer(pChildLayer, pLayer)
                            If Not pParent Is Nothing Then
                                FindParentLayer = pParent
                                Exit Function
                            End If
                            pLayer = pEnumLayer.Next
                        Loop
                    End If
                Else
                    Dim l As Long
                    For l = 0 To pCandidate.Count - 1
                        If pCandidate.Layer(l) Is pChildLayer Then
                            FindParentLayer = pCandidate
                            Exit Function
                        End If
                    Next l
                End If
            End Function
            Friend Shared Function SubtypeCount(ByVal pSubTypes As ISubtypes) As Integer
                Try
                    '  get  the enumeration of all of the subtypes for this feature class
                    Dim pEnumSubTypes As IEnumSubtype
                    Dim lSubT As Long
                    Dim sSubT As String
                    pEnumSubTypes = pSubTypes.Subtypes

                    ' loop through all of the subtypes and bring up a message
                    ' box with each subtype's code and name
                    ' Indicate when sFeature is found (a passed in string var)
                    sSubT = pEnumSubTypes.Next(lSubT)
                    Dim i As Integer = 0
                    While Len(sSubT) > 0
                        i = i + 1

                        sSubT = pEnumSubTypes.Next(lSubT)
                    End While
                    pEnumSubTypes = Nothing

                    Return i
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: getSubtypeCount" & vbCrLf & ex.ToString())
                    Return Nothing
                End Try
            End Function
            Friend Shared Sub SubtypeValuesAtIndex(ByVal index As Integer, ByVal pSubTypes As ISubtypes, ByRef Code As String, ByRef Display As String)
                Try

                    '  get  the enumeration of all of the subtypes for this feature class
                    Dim pEnumSubTypes As IEnumSubtype
                    Dim lSubT As Long
                    Dim sSubT As String
                    pEnumSubTypes = pSubTypes.Subtypes

                    ' loop through all of the subtypes and bring up a message
                    ' box with each subtype's code and name
                    ' Indicate when sFeature is found (a passed in string var)
                    sSubT = pEnumSubTypes.Next(lSubT)
                    Dim i As Integer = 0
                    While Len(sSubT) > 0

                        If i = index Then
                            Code = lSubT
                            Display = pSubTypes.SubtypeName(lSubT) ' sSubT
                            Return
                        End If
                        'MsgBox(lSubT & ": " & pSubTypes.SubtypeName(lSubT))
                        'If pSubTypes.SubtypeName(lSubT) = sFeature Then
                        '    MsgBox("FoundTarget " & sFeature)
                        'End If

                        sSubT = pEnumSubTypes.Next(lSubT)
                        i = i + 1
                    End While
                    pEnumSubTypes = Nothing

                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: getSubtypeValesAtIndex" & vbCrLf & ex.ToString())

                End Try


            End Sub
            Friend Shared Sub DomainValuesAtIndex(ByVal index As Integer, ByVal CodedValue As ICodedValueDomain, ByRef Code As String, ByRef Display As String)
                Try

                    If CodedValue Is Nothing Then Return
                    For i = 0 To CodedValue.CodeCount - 1
                        If i = index Then
                            Code = CodedValue.Value(i)
                            Display = CodedValue.Name(i)
                            Return

                        End If



                    Next i
                Catch ex As Exception
                    MsgBox("Error in the Costing Tools - CIPProjectWindow: my.Globals.Functions.DomainValuesAtIndex" & vbCrLf & ex.ToString())

                End Try

            End Sub
            Public Shared Function GetRegistryValue(ByVal key As String, ByVal name As String) As String
                Dim retVal As String = ""
                '  string infoMessage = "";

                ' Open the registry key, always in HKLM.
                Dim rootkey As RegistryKey = Registry.LocalMachine
                ' Open a sub key for reading.
                ' Debug.WriteLine("Key: " + key);
                Dim subkey As RegistryKey = rootkey.OpenSubKey(key)
                ' If the RegistryKey doesn't exist return null
                If subkey Is Nothing Then
                    Return retVal
                Else
                    Try
                        '       Debug.WriteLine("Subkey: " + name);
                        ' Check for proper registry value data type
                        Dim valKind As RegistryValueKind = subkey.GetValueKind(name)
                        If valKind = RegistryValueKind.DWord OrElse valKind = RegistryValueKind.[String] Then
                            retVal = subkey.GetValue(name).ToString()
                        End If

                    Catch
                    End Try
                End If
                Return retVal
            End Function


            Public Shared Function GetConfigFiles() As String()

                Dim pPathToUserFolder As String = getUserFolder("ArcGISSolutions", "ProjectCostingTools")
                If File.Exists(System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.Config")) Then
                    Return {System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.Config")}

                End If
                Dim cmd1 As String = ""
                Dim pConfigFiles As String() = New String(0) {}
                If cmd1 <> "" Then
                    If File.Exists(System.IO.Path.Combine(cmd1, "Config.Config")) Then
                        pConfigFiles(0) = System.IO.Path.Combine(cmd1, "Config.Config")
                        System.IO.File.Copy(pConfigFiles(0), System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.config"), True)
                        pConfigFiles(0) = System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.config")
                        Return pConfigFiles
                    End If
                End If
                Dim locPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)


                Dim AppPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                If AppPath.IndexOf("file:\") >= 0 Then
                    AppPath = AppPath.Replace("file:\", "")
                End If


                Dim fullAssemblyName As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
                Dim assemblyName As String = fullAssemblyName.Split("."c).GetValue(fullAssemblyName.Split("."c).Length - 1).ToString()
                If File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), assemblyName & ".Config")) Then
                    pConfigFiles(0) = System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), assemblyName & ".Config")
                ElseIf File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), fullAssemblyName & ".Config")) Then
                    pConfigFiles(0) = System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), fullAssemblyName & ".Config")

                ElseIf File.Exists(System.IO.Path.Combine(AppPath, assemblyName & ".Config")) Then
                    pConfigFiles(0) = System.IO.Path.Combine(AppPath, assemblyName & ".Config")
                ElseIf File.Exists(System.IO.Path.Combine(AppPath, fullAssemblyName & ".Config")) Then
                    pConfigFiles(0) = System.IO.Path.Combine(AppPath, fullAssemblyName & ".Config")
                ElseIf File.Exists(System.IO.Path.Combine(AppPath, "Config.Config")) Then
                    pConfigFiles(0) = System.IO.Path.Combine(AppPath, "Config.Config")
                End If
                System.IO.File.Copy(pConfigFiles(0), System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.config"), True)
                pConfigFiles(0) = System.IO.Path.Combine(pPathToUserFolder, "ProjectCost.config")
                Return pConfigFiles


            End Function
            Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Double) As Double

                Try
                    Dim pConfigFiles As String() = GetConfigFiles()
                    Dim keyvalue As String = ""

                    For Each configFile As String In pConfigFiles
                        If File.Exists(configFile) Then
                            Dim oXml As New XmlDocument()
                            oXml.Load(configFile)

                            Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
                            If oList Is Nothing Then
                                Return defaultValue
                            End If

                            For Each oNode As XmlNode In oList
                                For Each oKey As XmlNode In oNode.ChildNodes
                                    If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                                        If oKey.Attributes("key").Value.Equals(keyname) Then
                                            If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                                keyvalue = oKey.Attributes("value").Value

                                                'Try to convert to double
                                                If IsDouble(keyvalue) Then
                                                    Return (Convert.ToDouble(keyvalue))
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    Next

                    Return defaultValue
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show(ex.ToString())
                    Return defaultValue
                End Try
            End Function
            Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Integer) As Integer

                Try
                    Dim pConfigFiles As String() = GetConfigFiles()
                    Dim keyvalue As String = ""

                    For Each configFile As String In pConfigFiles
                        If File.Exists(configFile) Then
                            'NameValueCollection AppSettings = new NameValueCollection();
                            Dim oXml As New XmlDocument()

                            oXml.Load(configFile)

                            Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
                            If oList Is Nothing Then
                                Return defaultValue
                            End If

                            'AppSettings = new NameValueCollection();
                            For Each oNode As XmlNode In oList
                                For Each oKey As XmlNode In oNode.ChildNodes
                                    If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                                        If oKey.Attributes("key").Value.Equals(keyname) Then
                                            If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                                keyvalue = oKey.Attributes("value").Value

                                                'Try to convert to integer32
                                                If IsInteger(keyvalue) Then
                                                    Return (Convert.ToInt32(keyvalue))
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    Next

                    Return defaultValue
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show(ex.ToString())
                    Return defaultValue
                End Try
            End Function
            Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Boolean) As Boolean

                Try
                    Dim pConfigFiles As String() = GetConfigFiles()
                    Dim keyvalue As String = ""

                    For Each configFile As String In pConfigFiles
                        If File.Exists(configFile) Then
                            'NameValueCollection AppSettings = new NameValueCollection();
                            Dim oXml As New XmlDocument()

                            oXml.Load(configFile)

                            Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
                            If oList Is Nothing Then
                                Return defaultValue
                            End If

                            'AppSettings = new NameValueCollection();
                            For Each oNode As XmlNode In oList
                                For Each oKey As XmlNode In oNode.ChildNodes
                                    If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                                        If oKey.Attributes("key").Value.Equals(keyname) Then
                                            If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                                keyvalue = oKey.Attributes("value").Value

                                                'Try to convert to integer32
                                                If IsBoolean(keyvalue) Then
                                                    Return (Convert.ToBoolean(keyvalue))
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    Next

                    Return defaultValue
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show(ex.ToString())
                    Return defaultValue
                End Try
            End Function
            Public Shared Function IsDouble(ByVal theValue As String) As Boolean
                Try
                    Convert.ToDouble(theValue)
                    Return True
                Catch
                    Return False
                End Try
            End Function
            'IsDecimal
            Public Shared Function IsInteger(ByVal theValue As String) As Boolean
                Try
                    Convert.ToInt32(theValue)
                    Return True
                Catch
                    Return False
                End Try
            End Function
            'IsInteger
            Public Shared Function IsBoolean(ByVal theValue As String) As Boolean
                Try
                    Convert.ToBoolean(theValue)
                    Return True
                Catch
                    Return False
                End Try
            End Function
            'IsBoolean
            Public Shared Function getUserFolder(ByVal ParentFolder As String, ByVal ChildFolder As String) As String

                Dim strPath As String


                If (Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", Nothing) <> Nothing) Then


                    strPath = Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", Nothing)



                ElseIf (Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", Nothing) <> Nothing) Then

                    strPath = Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", Nothing)
                Else
                    strPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    If System.IO.Directory.Exists(strPath) = False Then
                        System.IO.Directory.CreateDirectory(strPath)
                    End If
                    strPath = System.IO.Path.Combine(strPath, ParentFolder)
                    If System.IO.Directory.Exists(strPath) = False Then
                        System.IO.Directory.CreateDirectory(strPath)
                    End If
                    strPath = System.IO.Path.Combine(strPath, ChildFolder)
                    If System.IO.Directory.Exists(strPath) = False Then
                        System.IO.Directory.CreateDirectory(strPath)
                    End If


                End If





                Return strPath




            End Function
            Public Shared Function compareConfigValue(ByVal keyname As String, ByVal value As String) As Boolean
                If String.Compare(GetConfigValue(keyname), value) = 0 Then
                    Return True
                Else
                    Return False
                End If

            End Function
            Public Shared Function GetConfigValue(ByVal keyname As String) As String

                Try
                    Dim pConfigFiles As String() = GetConfigFiles()
                    Dim keyvalue As String = ""

                    For Each configFile As String In pConfigFiles
                        If File.Exists(configFile) Then
                            'NameValueCollection AppSettings = new NameValueCollection();
                            Dim oXml As New XmlDocument()

                            oXml.Load(configFile)

                            Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
                            If oList Is Nothing Then
                                Return ""
                            End If

                            'AppSettings = new NameValueCollection();
                            For Each oNode As XmlNode In oList
                                For Each oKey As XmlNode In oNode.ChildNodes
                                    If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                                        If oKey.Attributes("key").Value.Equals(keyname) Then
                                            If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                                keyvalue = oKey.Attributes("value").Value

                                                'Try to convert to integer32
                                                Return keyvalue
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    Next

                    Return ""
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show(ex.ToString())
                    Return ""
                End Try
            End Function
            Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As String) As String

                Try
                    Dim strConfigVal As String = GetConfigValue(keyname)
                    If strConfigVal = "" Then
                        Return defaultValue
                    Else
                        Return strConfigVal
                    End If
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show(ex.ToString())
                    Return defaultValue
                End Try
            End Function
            Private Shared Function KeyExists(ByVal xmlDoc As XmlDocument, ByVal strKey As String) As Boolean
                Dim appSettingsNode As XmlNode = xmlDoc.SelectSingleNode("configuration/appSettings")

                ' Attempt to locate the requested setting.
                For Each childNode As XmlNode In appSettingsNode
                    If childNode IsNot Nothing Then
                        If childNode.NodeType = XmlNodeType.Element Then
                            If childNode.Attributes.Count > 0 Then
                                If childNode.Attributes("key") IsNot Nothing Then
                                    If childNode.Attributes("key").Value IsNot Nothing Then
                                        If childNode.Attributes("key").Value = strKey Then
                                            Return True
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    End If
                Next
                Return False

            End Function




            Public Class DomSubList
                Private m_Value As String
                Private m_Display As String
                Public Sub New(ByVal Value As String, ByVal Display As String)
                    m_Display = Display
                    m_Value = Value
                End Sub
                Public Property Display() As String
                    Get
                        Return m_Display
                    End Get
                    Set(ByVal Display As String)
                        m_Display = Display
                    End Set
                End Property
                Public Property Value() As String
                    Get
                        Return m_Value
                    End Get
                    Set(ByVal Value As String)
                        m_Value = Value
                    End Set
                End Property
                Public Function getValue() As String
                    Return m_Value

                End Function
                Public Function getDisplay() As String
                    Return m_Display

                End Function
            End Class
        End Class

    End Class

End Namespace