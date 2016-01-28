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


using ESRI.ArcGIS.Geometry;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;

namespace A4LGSharedFunctions
{
    public class AddressReturnInfo
    {
        public AddressReturnInfo()
        {
          
        }
        public AddressReturnInfo( AddressInfo addressDetails , string addressPointKey)
        {
            AddressDetails = addressDetails;
           
            AddressPointKey = "";

        }
        public AddressInfo AddressDetails { get; set; }
        
        public string AddressPointKey { get; set; }
    }
   
    public class AddressInfo
    {
        public AddressInfo(string messages)
        {
            LeftAddress = 0;
            RightAddress = 0;
            StreetName = "";
            StreetGeometry = null;
            Messages = messages;
        }
        
        public AddressInfo()
        {
            LeftAddress = 0;
            RightAddress = 0;
            StreetName = "";
            StreetGeometry = null;
            Messages = "";
        }
        public AddressInfo(int leftAddress, int rightAddress, string streetName,IGeometry streetGeometry)
        {
            LeftAddress = leftAddress;
            RightAddress = rightAddress;
            StreetName = streetName;
            StreetGeometry =streetGeometry;
            Messages = "";
        }
        
        public double LeftAddress { get; set; }
        public double RightAddress { get; set; }
        public string StreetName { get; set; }
        public IGeometry StreetGeometry { get; set; }
        public string Messages{ get; set; }
    }

    [XmlRootAttribute(ElementName = "Line_Along", IsNullable = true)]
    public class Line_Along
    {
        public Line_Along()
        {


        }
        [XmlElement("Layer_Name")]
        public string Layer_Name { get; set; }
        [XmlElement("Line_UpStreamElevationField")]
        public string Line_UpStreamElevationField { get; set; }
        [XmlElement("Line_DownStreamElevationField")]
        public string Line_DownStreamElevationField { get; set; }
        [XmlElement("Line_IDField")]
        public string Line_IDField { get; set; }

    }
  
    [XmlRootAttribute(ElementName = "ProfileGraphDetails", IsNullable = false)]
    public class ProfileGraphDetails
    {

        public ProfileGraphDetails()
        {

        }
        [XmlElement("Network_Name")]
        public string Network_Name { get; set; }

        [XmlElement("Point_LayerName")]
        public string Point_LayerName { get; set; }

        [XmlElement("Point_RimElevationField")]
        public string Point_RimElevationField { get; set; }

        [XmlElement("Point_InvertField")]
        public string Point_InvertField { get; set; }

        [XmlElement("Point_InvertElevationField")]
        public string Point_InvertElevationField { get; set; }
        
        [XmlElement("Point_TopElevationField")]
        public string Point_TopElevationField { get; set; }

        [XmlElement("Point_BottomElevationField")]
        public string Point_BottomElevationField { get; set; }

        [XmlElement("Point_BottomElevationTypeField")]
        public string Point_BottomElevationTypeField { get; set; }
      
        [XmlElement("Point_IDField")]
        public string Point_IDField { get; set; }

        [XmlElement("Line_LayerName")]
        public string Line_LayerName { get; set; }

        [XmlElement("Line_UpStreamElevationField")]
        public string Line_UpStreamElevationField { get; set; }

        [XmlElement("Line_DownStreamElevationField")]
        public string Line_DownStreamElevationField { get; set; }

        [XmlElement("Line_IDField")]
        public string Line_IDField { get; set; }

        [XmlArray("Line_Labels"), XmlArrayItem(ElementName = "Line_Label", Type = typeof(string))]
        public string[] Line_Labels { get; set; }


        [XmlArray("Lines_Along"), XmlArrayItem(ElementName = "Line_Along", Type = typeof(Line_Along))]
        public Line_Along[] Lines_Along { get; set; }


        [XmlElement("PointAlong_LayerName")]
        public string PointAlong_LayerName { get; set; }

        [XmlElement("PointAlong_IDField")]
        public string PointAlong_IDField { get; set; }


        [XmlElement("PointAlong_ShowLabels")]
        public string PointAlong_ShowLabels { get; set; }
        
        [XmlArray("PointAlong_Labels"), XmlArrayItem(ElementName = "PointAlong_Label", Type = typeof(string))]
        public string[] PointAlong_Labels { get; set; }


        [XmlElement("Elevation_LayerName")]
        public string Elevation_LayerName { get; set; }


        [XmlElement("Graph_Name")]
        public string Graph_Name { get; set; }
        
            [XmlElement("GraphTitle_Name")]
        public string GraphTitle_Name { get; set; }
        [XmlElement("Legend_Name")]
        public string Legend_Name { get; set; }
        [XmlElement("LeftAxis_Name")]
        public string LeftAxis_Name { get; set; }
        [XmlElement("TopAxis_Name")]
        public string TopAxis_Name { get; set; }
        [XmlElement("BottomAxis_Name")]
        public string BottomAxis_Name { get; set; }
        




    }

   [XmlRootAttribute(ElementName = "FlowLayerDetails", IsNullable = true)]
    public class FlowLayerDetails
    {
    
    // <FlowLayerDetails>
    //  <LayerName>Sewer Manholes</LayerName>
    //  <SumFlowField>SUMFLOW</SumFlowField>
    //  <WeightName></WeightName>
    //  <FlowDirection>UpStream</FlowDirection>
    //</FlowLayerDetails>    
        public FlowLayerDetails()
        {

        }

        [XmlElement("LayerName")]
        public string LayerName { get; set; }

        [XmlElement("SumFlowField")]
        public string SumFlowField { get; set; }

        [XmlElement("WeightName")]
        public string WeightName { get; set; }

        [XmlElement("FlowDirection")]
        public string FlowDirection { get; set; }

        

    }


    [XmlRootAttribute(ElementName = "AddressCenterlineDetails", IsNullable = true)]
    public class AddressCenterlineDetails
    {
        
    // <AddressCenterlineDetails>
    //    <LayerName></LayerName>
    //    <FullName></FullName>
    //    <LeftFrom></LeftFrom>
    //    <LeftTo></LeftTo>
    //    <RightFrom></RightFrom>
    //    <RightTo></RightTo>
      
    //</AddressCenterlineDetails>
        public AddressCenterlineDetails()
        {

        }

        [XmlElement("FeatureClassName")]
        public string FeatureClassName { get; set; }

        [XmlElement("FullName")]
        public string FullName { get; set; }

        [XmlElement("LeftFrom")]
        public string LeftFrom { get; set; }

        [XmlElement("LeftTo")]
        public string LeftTo { get; set; }

        [XmlElement("RightFrom")]
        public string RightFrom { get; set; }

        [XmlElement("RightTo")]
        public string RightTo { get; set; }
        

    }

    //<CreatePointWithReferenceDetails>
   
        // <LayerName>Site Address Point</LayerName>
        //<ValueField>FULLNAME</ValueField>
        //<ReferencePointLayerName>Address Point</ReferencePointLayerName>
        //<CreateIfExisting>false</CreateIfExisting>
        //<ProrateAddressInfo>true</ProrateAddressInfo>
        //<AddressCenterlineDetails>
        //  <LayerName>Road Centerlines</LayerName>
        //  <FullName>FULLNAME</FullName>
        //  <LeftFrom>FROMLEFT</LeftFrom>
        //  <LeftTo>TOLEFT</LeftTo>
        //  <RightFrom>FROMRIGHT</RightFrom>
        //  <RightTo>TORIGHT</RightTo>

        //</AddressCenterlineDetails>
        
    //  </CreatePointWithReferenceDetails>
    [XmlRootAttribute(ElementName = "CreatePointWithReferenceDetails", IsNullable = false)]
    public class CreatePointWithReferenceDetails
    {


        public CreatePointWithReferenceDetails()
        {

        }

        [XmlElement("LayerName")]
        public string LayerName { get; set; }
         
        [XmlElement("AddressField")]
        public string AddressField { get; set; }

        [XmlElement("StreetNameField")]
        public string StreetNameField { get; set; }
        
        [XmlElement("AddressPntKeyField")]
        public string AddressPntKeyField{ get; set; }

        [XmlElement("ReferencePointLayerName")]
        public string ReferencePointLayerName { get; set; }

        [XmlElement("ReferencePointIDField")]
        public string ReferencePointIDField { get; set; }

        [XmlElement("ReferencePointEditTemplate")]
        public string ReferencePointEditTemplate { get; set; }

        
        //[XmlElement("CreateIfExisting", IsNullable = true)]
        //public string CreateIfExisting { get; set; }

        //[XmlElement("ProrateAddressInfo", IsNullable = true)]
        //public string ProrateAddressInfo { get; set; }

        [XmlElement("AddressCenterlineDetails", IsNullable = true)]
        public AddressCenterlineDetails AddressCenterlineDetails { get; set; }
        

    }

    [XmlRootAttribute(ElementName = "MoveConnectionsDetails", IsNullable = false)]
    public class MoveConnectionsDetails
    {
    //    <MoveConnection Name="Water Mains">
    //  <LineLayer>wMain</LineLayer>
    //  <LayersToMove>
    //    <Layer>wFittings</Layer>
    //    <Layer>wNetworkStructure</Layer>
    //  </LayersToMove>
    //</MoveConnection>

        public MoveConnectionsDetails()
        {

        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("LineLayer")]
        public string LineLayer { get; set; }

        
        [XmlArray("LayersToMove"), XmlArrayItem(ElementName = "Layer", Type = typeof(string))]
        public List<string> LayersToMove { get; set; }

        
      
    }


    [XmlRootAttribute(ElementName = "ConstructLineWithPointsDetails", IsNullable = false)]
    public class ConstructLineWithPointsDetails
    {   

        public ConstructLineWithPointsDetails()
        {

        }
        [XmlElement("Line_LayerName")]
        public string Line_LayerName{get;set;}

        [XmlElement("Point_Start_LayerName")]
        public string Point_Start_LayerName { get; set; }
        [XmlElement("Point_Start_EditTemplate")]
        public string Point_Start_EditTemplate { get; set; }

        [XmlElement("Point_Along_LayerName")]
        public string Point_Along_LayerName { get; set; }
        [XmlElement("Point_Along_EditTemplate")]
         public string Point_Along_EditTemplate { get; set; }

         [XmlElement("Point_End_LayerName")]
         public string Point_End_LayerName { get; set; }
         [XmlElement("Point_End_EditTemplate")]
         public string Point_End_EditTemplate { get; set; }
        
        //[XmlElement("Point_LayerName")]
         //public string Point_LayerName { get; set; }
         //[XmlElement("Point_EditTemplate")]
         //public string Point_EditTemplate { get; set; }
         [XmlElement("TwoPointLines")]
         public string TwoPointLines { get; set; }
         [XmlElement("PointAtVertices")]
         public string PointAtVertices { get; set; }
       
    }

    [XmlRootAttribute(ElementName = "ConnectClosestDetails", IsNullable = false)]
    public class ConnectClosestDetails
    {

        public ConnectClosestDetails()
        {

        }
        //public ConnectClosestDetails(string pointLayerName, string connectLineLayerName,
        //                           int connectLineSubtype, string connectLineFieldName,
        //                           string connectLineValue, int searchThreshold)
        //{
        //    _pointLayerName = pointLayerName;
        //    _connectLineLayerName = connectLineLayerName;
        //    _connectLineSubtype = connectLineSubtype;
        //    _connectLineFieldName = connectLineFieldName;
        //    _connectLineValue = connectLineValue;
        //    _searchThreshold = searchThreshold;

        //}
        [XmlElement("Point_Layer")]
        public string Point_Layer { get; set; }
        [XmlElement("Line_Layer")]
        public string Line_Layer { get; set; }
        //[XmlElement("Line_Subtype")]
        //public int Line_Subtype { get; set; }
        //[XmlElement("Line_FieldToPopulate")]
        //public string Line_FieldToPopulate { get; set; }
        //[XmlElement("Line_ValueToPopulate")]
        //public string Line_ValueToPopulate { get; set; }
        [XmlElement("Line_EditTemplate")]
        public string Line_EditTemplate { get; set; }
        
        [XmlElement("Search_Threshold")]
        public int Search_Threshold { get; set; }

        [XmlElement("Reset_Flow")]
        [System.ComponentModel.DefaultValue("NONE")]
        public string Reset_Flow { get; set; }

    }

    [XmlRootAttribute(ElementName = "AttributeTransferDetails", IsNullable = false)]
    public class AttributeTransferDetails
    {

        public AttributeTransferDetails()
        {

        }
        //public AttributeTransferLayers(string Name, string SourceLayerName, string TargetLayerName,
        //                           FromToLayers[] FieldMatching)
        //{


        //}
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("SourceLayerName")]
        public string SourceLayerName { get; set; }
        [XmlElement("TargetLayerName")]
        public string TargetLayerName { get; set; }

        [XmlArray("FromToFields"), XmlArrayItem(ElementName = "FromToField", Type = typeof(FromToField))]
        public FromToField[] FromToFields { get; set; }

    }
    public class FromToField
    {
        public FromToField() { }


        [XmlElement("SourceField")]
        public string SourceField { get; set; }

         [XmlElement("TargetField")]
        public string TargetField { get; set; }

        [XmlElement("Prefix")]
        public string Prefix { get; set; }
    }
    public class FieldPair
    {
        public FieldPair() { }


        [XmlElement("Field1")]
        public string Field1 { get; set; }

        [XmlElement("Field2")]
        public string Field2 { get; set; }

        
    }
    public class Field
    {
        public Field() { }


        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("MergeRule")]
        public string MergeRule { get; set; }
        
        [XmlElement("SplitRule")]
        public string SplitRule { get; set; }

    }

      
    [XmlRootAttribute(ElementName = "MergeSplitGeoNetFeatures", IsNullable = false)]
    public class MergeSplitGeoNetFeatures
    {
        public MergeSplitGeoNetFeatures()
        {
        }
        [XmlElement("MergeSplitElev")]
        public string MergeSplitElev { get; set; }

        [XmlElement("SplitFormatString")]
        public string SplitFormatString { get; set; }
        
        [XmlArray("Fields"), XmlArrayItem(ElementName = "Field", Type = typeof(Field))]
        public Field[] Fields { get; set; }


    }
       
    [XmlRootAttribute(ElementName = "AddLateralDetails", IsNullable = false)]
    public class AddLateralDetails
    {
        public AddLateralDetails()
        {
        }
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Point_LayerName")]
        public string Point_LayerName { get; set; }
        
        [XmlElement("MainLine_LayerName")]
        public string MainLine_LayerName { get; set; }
        
        [XmlElement("LateralLine_LayerName")]
        public string LateralLine_LayerName { get; set; }
        
        [XmlElement("LateralLine_EditTemplate",IsNullable = true)]
        public string LateralLine_EditTemplate { get; set; }
        
      
        [XmlArray("FromToFields"), XmlArrayItem(ElementName = "FromToField", Type = typeof(FromToField))]
        public FromToField[] FromToFields { get; set; }

     
        [XmlElement("LateralLine_StartAtMain")]
        [System.ComponentModel.DefaultValue(false)]
        public bool LateralLine_StartAtMain { get; set; }
        
        [XmlElement("DeleteExistingLines")]
        [System.ComponentModel.DefaultValue(false)]
        public bool DeleteExistingLines { get; set; }
        
        [XmlElement("TolerenceForDelete")]
        [System.ComponentModel.DefaultValue(.5)]
        public double TolerenceForDelete { get; set; }
        
        [XmlElement("SearchOnLayer")]
        [System.ComponentModel.DefaultValue(true)]
        public bool SearchOnLayer { get; set; }
        
        [XmlElement("SearchDistance")]
        [System.ComponentModel.DefaultValue(500)]
        public int SearchDistance { get; set; }
        
        [XmlArray("PointsAlong"), XmlArrayItem(ElementName = "PointAlong", Type = typeof(PointAlong))]
        public PointAlong[] PointAlong { get; set; }
        
        [XmlElement("Dual_When_Two_Selected")]
        [System.ComponentModel.DefaultValue(false)]
        public bool Dual_When_Two_Selected { get; set; }
        
        [XmlElement("Dual_When_Nearby")]
        [System.ComponentModel.DefaultValue(false)]
        public bool Dual_When_Nearby { get; set; }
        
        [XmlElement("Dual_Max_Distance_When_Nearby")]
        [System.ComponentModel.DefaultValue(30)]
        public double Dual_Max_Distance_When_Nearby { get; set; }

        [XmlElement("Dual_Max_Distance_When_Two_Selected")]
        [System.ComponentModel.DefaultValue(100)]
        public double Dual_Max_Distance_When_Two_Selected { get; set; }
        
        [XmlElement("Dual_Option_Make_Square")]
        [System.ComponentModel.DefaultValue(false)]
        public bool Dual_Option_Make_Square { get; set; }
        
        [XmlElement("Hook_DoglegDistance")]
        [System.ComponentModel.DefaultValue(0.0)]
        public double Hook_DoglegDistance { get; set; }
        
        [XmlElement("Hook_DistanceIsPercent")]
        [System.ComponentModel.DefaultValue(false)] 
        public bool Hook_DistanceIsPercent { get; set; }
        
        [XmlElement("Hook_Angle")]
        [System.ComponentModel.DefaultValue(45.0)]
        public double Hook_Angle { get; set; }
        
        [XmlElement("Reset_Flow")]
        [System.ComponentModel.DefaultValue("NONE")]
        public string Reset_Flow { get; set; }
        //[XmlElement("Direction_StartAtMain")]
        //public bool Direction_StartAtMain { get; set; }
        


    }

    [XmlRootAttribute(ElementName = "AddLateralFromMainPointDetails", IsNullable = false)]
    public class AddLateralFromMainPointDetails
    {
        public AddLateralFromMainPointDetails()
        {
        }
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Point_LayerName")]
        public string Point_LayerName { get; set; }

      

        [XmlElement("MainLine_LayerName")]
        public string MainLine_LayerName { get; set; }

        [XmlElement("LateralLine_LayerName")]
        public string LateralLine_LayerName { get; set; }

        [XmlElement("LateralLine_EditTemplate", IsNullable = true)]
        public string LateralLine_EditTemplate { get; set; }

        [XmlElement("LateralLine_AngleDetails")]
        [System.ComponentModel.DefaultValue(false)]
        public LateralLine_AngleDetails LateralLine_AngleDetails { get; set; }
        
        [XmlElement("LateralLine_StartAtMain")]
        [System.ComponentModel.DefaultValue(false)]
        public bool LateralLine_StartAtMain { get; set; }


        [XmlArray("FromToFields"), XmlArrayItem(ElementName = "FromToField", Type = typeof(FromToField))]
        public FromToField[] FromToFields { get; set; }

       

        [XmlElement("SearchOnLayer")]
        [System.ComponentModel.DefaultValue(true)]
        public bool SearchOnLayer { get; set; }

        [XmlArray("PointsAlong"), XmlArrayItem(ElementName = "PointAlong", Type = typeof(PointAlong))]
        public PointAlong[] PointAlong { get; set; }

        [XmlElement("Reset_Flow")]
        [System.ComponentModel.DefaultValue("NONE")]
        public string Reset_Flow { get; set; }
    


    }
    public class LateralLine_AngleDetails
    {

        [XmlElement("AngleField")]
        [System.ComponentModel.DefaultValue("90")]
        public string AngleField { get; set; }

        [XmlElement("AngleType")]
        [System.ComponentModel.DefaultValue("CLOCK")]
        public string AngleType { get; set; }

        [XmlElement("AddAngleToLineAngle")]
        [System.ComponentModel.DefaultValue("true")]
        public string AddAngleToLineAngle { get; set; }

        [XmlElement("DirectionField")]
        [System.ComponentModel.DefaultValue("D")]
        public string DirectionField { get; set; }


        [XmlElement("LookingUpstreamValue")]
        [System.ComponentModel.DefaultValue("U")]
        public string LookingUpstreamValue { get; set; }
        
        [XmlElement("LookingDownstreamValue")]
        [System.ComponentModel.DefaultValue("D")]
        public string LookingDownstreamValue { get; set; }

        [XmlElement("OnlyPerp")]
        [System.ComponentModel.DefaultValue("true")]
        public string OnlyPerp { get; set; }

        [XmlElement("LengthField")]
        [System.ComponentModel.DefaultValue("15")]
        public string LengthField { get; set; }

    }
    public class TapPointDetails
    {
        [XmlElement("TapPointTableName")]
        [System.ComponentModel.DefaultValue("Tap Points")]
        public string TapPointTableName { get; set; }

        [XmlElement("TapPointResultLayerName")]
        [System.ComponentModel.DefaultValue("Tap Points FC")]
        public string TapPointResultLayerName { get; set; }

        [XmlElement("DistanceField")]
        [System.ComponentModel.DefaultValue("DISTANCE")]
        public string DistanceField { get; set; }

        [XmlElement("DirectionField")]
        [System.ComponentModel.DefaultValue("D")]
        public string DirectionField { get; set; }


        [XmlElement("LookingUpstreamValue")]
        [System.ComponentModel.DefaultValue("U")]
        public string LookingUpstreamValue { get; set; }

        [XmlElement("LookingDownstreamValue")]
        [System.ComponentModel.DefaultValue("D")]
        public string LookingDownstreamValue { get; set; }

        [XmlElement("MainIDField")]
        [System.ComponentModel.DefaultValue("FACILITYID")]
        public string MainIDField { get; set; }


        [XmlElement("MainLayerName")]
        [System.ComponentModel.DefaultValue("Water Mains")]
        public string MainLayerName { get; set; }

        [XmlElement("MainLayerIDField")]
        [System.ComponentModel.DefaultValue("FACILITYID")]
        public string MainLayerIDField { get; set; }

    }
    public class PointAlong
    {
        public PointAlong()
        {
        }
        // private string _FeatureClass;
        [XmlElement("LayerName")]
        public string LayerName { get; set; }
        [XmlElement("PolygonOffsetLayerName")]
        public string PolygonOffsetLayerName { get; set; }
        [XmlElement("PolygonOffsetSide")]
        public string PolygonOffsetSide { get; set; }
        //public int Subtype { get; set; }
   
        //public string FieldToPopulate { get; set; }
   
        //public string ValueToPopulate { get; set; }
      [XmlElement("Distance")]
        public double Distance { get; set; }
       [XmlElement("DistanceIsPercent")]
        public bool DistanceIsPercent { get; set; }
        [XmlElement("EditTemplate")]
       public string EditTemplate { get; set; }

    }


    [XmlRootAttribute(ElementName = "LayerViewerConfig", IsNullable = false)]
    public class LayerViewerConfig
    {
        public LayerViewerConfig()
        {
        }
        [XmlAttribute("ZoomOnRecordChange")]
        public bool ZoomOnRecordChange { get; set; }
        [XmlArray("LayerViewerLayers"), XmlArrayItem(ElementName = "LayerViewerLayer", Type = typeof(LayerViewerLayer))]
        public LayerViewerLayer[] LayerViewerLayer { get; set; }

    }
    public class LayerViewerLayer
    {
        public LayerViewerLayer()
        { }
        public string LayerName { get; set; }
        public string Query { get; set; }
        public double ZoomScale { get; set; }
        public string QueryAndZoom
        {
            get { return Query + "," + ZoomScale; }
        }

    }
}
