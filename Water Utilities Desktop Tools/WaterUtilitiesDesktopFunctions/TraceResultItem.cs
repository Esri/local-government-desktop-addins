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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMap;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.SystemUI;

public interface IFeatureDetails {
     ESRI.ArcGIS.Geometry.IGeometry Geo { get; set; }
    int OID { get; set; }
}
public interface ITraceResultsClass { 
         int ClassID { get; set; }
        string ClassName { get; set; }
        ArrayList Features { get; set; }


        int CompareTo(object obj);
        ArrayList TraceResultItems();
        }
public interface ITraceResult
{
      int count();
      ArrayList TraceResultItems();
      void add(int ClassID, string ClassName, ESRI.ArcGIS.Geometry.IGeometry Geo, int OID, int EID);
      string Serialize();
}

namespace A4WaterUtilities
{
    public class FeatureDetails
    {
        public ESRI.ArcGIS.Geometry.IGeometry Geo { get; set; }
        public int OID { get; set; }
        public int EID { get; set; }
        public FieldPairs[] Fields { get; set; }
    }
    public class FieldPairs
    {
       
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public string FieldValue{ get; set; }
    }
}
namespace A4WaterUtilities
{
    public class TraceResultsClass : IComparable
    {
        public TraceResultsClass()
        {
            Features = new ArrayList();

        }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public ArrayList Features { get; set; }


        public int CompareTo(object obj)
        {
            TraceResultsClass otherTraceRes = obj as TraceResultsClass;
            if (otherTraceRes != null)
                return this.ClassID.CompareTo(otherTraceRes.ClassID);
            else
                throw new ArgumentException("Object is not a TraceResult");
        }


    }
}
namespace A4WaterUtilities
{
    public class TraceResult
    {
        ArrayList m_TraceResultItems;
        public TraceResult()
        {
            m_TraceResultItems = new ArrayList();
        }
        public int count()
        {
            return m_TraceResultItems.Count;
        }
        public ArrayList TraceResultItems
        {
            get
            {
                return m_TraceResultItems;
            }

        }
        public void add(int ClassID, string ClassName, ESRI.ArcGIS.Geometry.IGeometry Geo, int OID, int EID,IFeature pFeat)
        {
            FeatureDetails pFD = new FeatureDetails();
            FieldPairs[] fps = new FieldPairs[pFeat.Fields.FieldCount - 1];
            int fldIdx = 0;
            for (int i = 0; i < pFeat.Fields.FieldCount ; i++)
            {
                if (pFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                { }
                else
                {
                    FieldPairs fp = new FieldPairs();
                    fp.FieldAlias = pFeat.Fields.get_Field(i).AliasName;
                    fp.FieldName = pFeat.Fields.get_Field(i).Name;
                    if (pFeat.get_Value(i) == null)
                    {
                        fp.FieldValue = "";
                    
                    }
                    else
                    {
                        fp.FieldValue = pFeat.get_Value(i).ToString();
                    }
                    fps[fldIdx] = fp;

                    fldIdx++;
                }
                
            }
            pFD.Fields = fps;

            pFD.Geo = Geo;
            pFD.OID = OID;
            pFD.EID = EID;
            TraceResultsClass trRes = TraceResultClassItem(ClassID);
            if (trRes == null)
            {
                trRes = new TraceResultsClass();
                trRes.ClassID = ClassID;
                trRes.ClassName = ClassName;
                if (trRes.Features == null)
                {
                    trRes.Features = new ArrayList();
                }

                trRes.Features.Add(pFD);
                m_TraceResultItems.Add(trRes);

            }
            else
            {
                if (trRes.Features == null)
                {
                    trRes.Features = new ArrayList();
                }
                trRes.Features.Add(pFD);
            }

        }
        private TraceResultsClass TraceResultClassItem(int ClassID)
        {
            //  TraceResultsClass trRes = new TraceResultsClass();
            //    trRes.ClassID = ClassID;
            //int idx = m_TraceResultItems.BinarySearch(trRes);

            //    if (idx != -1) {
            //        return (TraceResultsClass)m_TraceResultItems[idx];
            //    }else
            //    {
            //        return null;
            //    }
            foreach (TraceResultsClass trRes in m_TraceResultItems)
            {

                if (trRes.ClassID == ClassID)
                {
                    return trRes;
                }
            }
            return null;
        }
        private string serializeGeo(ESRI.ArcGIS.Geometry.IGeometry geo, Boolean ESRISerialize)
        {

         
            if (ESRISerialize)
            {
                System.String xmlNodeName = "NodeName";
                System.String elementURI = "http://www.esri.com/schemas/ArcGIS/10";

                // Create xml writer
                ESRI.ArcGIS.esriSystem.IXMLWriter xmlWriter = new ESRI.ArcGIS.esriSystem.XMLWriterClass();

                // Create xml stream
                ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

                // Explicit Cast for IStream and then write to stream 
                xmlWriter.WriteTo((ESRI.ArcGIS.esriSystem.IStream)xmlStream);

                // Serialize 
                ESRI.ArcGIS.esriSystem.IXMLSerializer xmlSerializer = new ESRI.ArcGIS.esriSystem.XMLSerializerClass();


                xmlSerializer.WriteObject(xmlWriter, null, null, xmlNodeName, elementURI, geo);

                return xmlStream.SaveToString();
            }
            else
            {
                switch (geo.GeometryType)
                {
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                        {
                            ESRI.ArcGIS.Geometry.IPoint pPnt = (ESRI.ArcGIS.Geometry.IPoint)geo;

                            return "<Point><X>" + pPnt.X + "</X><Y>" + pPnt.Y + "</Y></Point>";

                            //break;
                        }
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                        {
                            string lineString = "<PathArray>";
                            ESRI.ArcGIS.Geometry.IPolyline pPly = (ESRI.ArcGIS.Geometry.IPolyline)geo;
                            ESRI.ArcGIS.Geometry.IGeometryCollection pGC = (ESRI.ArcGIS.Geometry.IGeometryCollection)pPly;
                            for (int i = 0; i < pGC.GeometryCount; i++)
                            {
                                lineString = lineString + "<Path><PointArray>";
                                //ESRI.ArcGIS.Geometry.IGeometry pGeo2 = pGC.get_Geometry(i);
                                // ESRI.ArcGIS.Geometry.IPath pPath = (ESRI.ArcGIS.Geometry.IPath)pGC.get_Geometry(i);
                                ESRI.ArcGIS.Geometry.IPointCollection pPointCol = (ESRI.ArcGIS.Geometry.IPointCollection)pGC.get_Geometry(i);
                                for (int j = 0; j < pPointCol.PointCount; j++)
                                {
                                    ESRI.ArcGIS.Geometry.IPoint pPathPoint = pPointCol.get_Point(j);

                                    lineString = lineString + "<Point><X>" + pPathPoint.X + "</X><Y>" + pPathPoint.Y + "</Y></Point>";
                                }
                                // string tr = pGeo2.GeometryType.ToString();
                                lineString = lineString + "</PointArray></Path>";
                            }
                            lineString = lineString + "</PathArray>";
                            return lineString;

                            // return "<Point><X>" + pPnt.X + "</X><Y>" + pPnt.Y + "</Y></Point>";
                       //     break;
                        }
                    default:
                        {
                            return "";
                         //   break;
                        }
                }
            }
        }
        public string Serialize()
        {
            try
            {
                string results = "<TraceResults>";
                foreach (TraceResultsClass trRes in TraceResultItems)
                {
                    results = results + "<FeatureClass Name='" + trRes.ClassName + "'" + " ClassID='" + trRes.ClassID + "'>";
                    foreach (FeatureDetails pFD in trRes.Features)
                    {
                        results = results + "<Feature ObjectID='" + pFD.OID + "' EID='" + pFD.EID + "'>";
                        results = results + serializeGeo(pFD.Geo, false);
                        results = results + "</Feature>";

                    }
                    results = results + "</FeatureClass>";
                }
                results = results + "</TraceResults>";
                return results;
            }
            catch (Exception ex)
            {
                return ex.Message;
                //throw ex.Message;
            }

        }
    }
}