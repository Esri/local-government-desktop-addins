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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace A4LGSharedFunctions
{
    class Rotate
    {
        public double RotatePoint(IMap pMap, IFeature pPointFeature, bool bArithmeticAngle, string strDiameterFld, string strLayerName)
        {
            IFeatureClass pPointFC = default(IFeatureClass);
            ISpatialFilter pSFilter = default(ISpatialFilter);
            IFeatureCursor pLineCursor = default(IFeatureCursor);
            IFeature pLineFeature = default(IFeature);
            IPoint pPoint = default(IPoint);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            ILayer pLayer = default(ILayer);
            IFeatureLayer pFLayer = default(IFeatureLayer);
            UID pId = new UID();
            UID pUID = new UID();
            ITopologicalOperator pTopo = null;

            List<Double> cAngles = new List<Double>();
            List<string> pLstInt = new List<string>();
            List<double> cDiameters = new List<double>();


            double dblAngle = 0;
            double dblDiameter = 0;
            double ltest = 0;
            int iLineDiameterFieldPos = 0;
            try
            { 
            //This routine is used by both RotateDuringCreateFeature and RotateSelectedFeature.
            //It contains all of logic for determining the rotation angle.

            const int iAngleTol = 5;
            //Used for Tees> a straight line is 180 + or - iAngleTol

          
            
            
                pPointFC = (IFeatureClass)pPointFeature.Class;
            pPoint = (IPoint)pPointFeature.Shape;

            //Create spatial filter to find intersecting features at this given point
            pTopo = (ITopologicalOperator)pPoint;

            pSFilter = new SpatialFilter();
            pSFilter.Geometry = pTopo.Buffer(0.5);
            //pPoint
            pSFilter.GeometryField = pPointFC.ShapeFieldName;
            pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            //Step through each feature layer
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            //GeoFeatureLayer
            pEnumLayer = (IEnumLayer)pMap.get_Layers(pUID, true);
            pEnumLayer.Reset();
            pLayer = (ILayer)pEnumLayer.Next();
           

            while ((pLayer != null))
            {
                //Verify that this is a line layer
                pFLayer = (IFeatureLayer)pLayer;

                if (pFLayer.FeatureClass != null)
                {


                    if (pFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline && pFLayer.Visible && (strLayerName == "" || strLayerName == Globals.getClassName((IDataset)pFLayer.FeatureClass)))
                    {
                        //Apply the filter this line layer
                        pLineCursor = pFLayer.FeatureClass.Search(pSFilter, true);

                        //Loop through the found lines for this layer
                        pLineFeature = pLineCursor.NextFeature();
                        while ((pLineFeature != null))
                        {
                            if (pLstInt.Count > 0)
                            {
                                if (pLstInt.Contains(pLineFeature.Class.ObjectClassID + " " + pLineFeature.OID))
                                {
                                    pLineFeature = pLineCursor.NextFeature();
                                    continue;
                                }
                            }
                            pLstInt.Add(pLineFeature.Class.ObjectClassID.ToString() + " " + pLineFeature.OID.ToString());

                            dblAngle = Globals.GetAngleOfLineAtPoint((IPolyline)pLineFeature.ShapeCopy, (IPoint)pPointFeature.ShapeCopy, Globals.GetXYTolerance(pPointFeature));
                            dblAngle = Globals.ConvertRadsToDegrees(dblAngle);


                            //Convert to geographic degrees(zero north clockwise)
                            if (!(bArithmeticAngle))
                            {
                                dblAngle = Globals.ConvertArithmeticToGeographic(dblAngle);
                            }
                            //Round angle
                            dblAngle = Math.Round(dblAngle, 4);

                            //Find diameter field, if it exists
                            iLineDiameterFieldPos = pFLayer.FeatureClass.FindField(strDiameterFld);

                            //Get diameter of line
                            if (iLineDiameterFieldPos < 0)
                            {
                                dblDiameter = -9999;
                            }
                            else if (pLineFeature.get_Value(iLineDiameterFieldPos) == null)
                            {
                                dblDiameter = -9999;
                            }
                            else if (object.ReferenceEquals(pLineFeature.get_Value(iLineDiameterFieldPos), DBNull.Value))
                            {
                                dblDiameter = -9999;
                            }
                            else
                            {
                                Double.TryParse(pLineFeature.get_Value(iLineDiameterFieldPos).ToString(), out dblDiameter);

                            }

                            //add this line (angle and diameter) to a collection of line info for this point
                            cAngles.Add(dblAngle);

                            if (dblDiameter != -9999)
                            {
                                cDiameters.Add(dblDiameter);
                            }

                            //Get next line
                            pLineFeature = pLineCursor.NextFeature();
                        }
                    }
                }
                //Get next line layer
                pLayer = pEnumLayer.Next();
            }

            //Read the collection of line segment angles and diameters
            //and use them to derive a symbol rotation angle for the point
            switch (cAngles.Count)
            {
                case 0:
                    //One line such as at valves

                    return 0.0;

                case 1:
                    //One line such as at valves
                    return cAngles[0];
                case 2:
                    //Two lines such as at reducers Or at tee fittings where line is not broken

                    if (cDiameters.Count == 2)
                    {
                        //If cDiameters(0) Is Nothing Or cDiameters(1) Is Nothing Then
                        //    Return cAngles.Item(0)
                        //Else
                        if (cDiameters[0] > cDiameters[1])
                        {
                            return cAngles[1];
                            //If cAngles.Item(0) = cAngles.Item(1) Then
                            //    Return cAngles.Item(1)
                            //Else
                            //    Return cAngles.Item(1) - 180
                            //End If

                        }
                        else
                        {
                            return cAngles[0];
                            //If cAngles.Item(0) = cAngles.Item(1) Then
                            //    Return cAngles.Item(0) - 180
                            //Else
                            //    Return cAngles.Item(1)
                            //End If
                        }
                    }
                    else
                    {
                        return cAngles[0];
                    }

                    break;
                case 3:
                    //Three lines such as at tee fittings where line is broken
                    ltest = Math.Abs(cAngles[0] - cAngles[1]);
                    if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                    {
                        return cAngles[2];
                    }
                    else
                    {
                        ltest = Math.Abs(cAngles[0] - cAngles[2]);
                        if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                        {
                            return cAngles[1];
                        }
                        else
                        {
                            ltest = Math.Abs(cAngles[1] - cAngles[2]);
                            if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                            {
                                return cAngles[0];
                            }
                            else
                            {
                                return -360;
                            }
                        }
                    }
                    break;
                case 4:
                    //Four lines such as at crosses
                    //the angle of any of the four lines should work since the symbol should be symetrically
                    return cAngles[0];
                default:
                    return 0;
            }

            //Clear collections
            }
            catch 
            {
                return 0;
            }
            finally 
            {

                 pPointFC = null;
                 pSFilter = null;
                 pLineCursor = null;
                 pLineFeature = null;
                 pPoint = null;
                 pEnumLayer = null;
                 pLayer = null;
                 pFLayer = null;
                 pId = null;
                 pUID = null;
                 pTopo = null;

                 cAngles.Clear();
                 pLstInt.Clear();
                cDiameters.Clear();

            }

        }
        public class diameterMeterFeat
        {
            public double dblDiameter { get; set; }
            public IPoint pntStart{ get; set; }
            public IPoint pntEnd { get; set; }
            public double angle { get; set; }
        }
        public double RotatePointByNetwork(IMap pMap, INetworkFeature pPointFeature, bool bArithmeticAngle, string strDiameterFld, string strLayerName )
        {
            //This routine is used by both RotateDuringCreateFeature and RotateSelectedFeature.
            //It contains all of logic for determining the rotation angle.

            const int iAngleTol = 5;
            double dblAngle = 0;
            double dblDiameter = 0;
            double ltest = 0;
            diameterMeterFeat diamPnt = null;
            int iLineDiameterFieldPos = 0;
      
            IPoint pPoint = default(IPoint);
            ISimpleJunctionFeature pSimpJunc = null;
            IEdgeFeature pEdgeFeat = default(IEdgeFeature);

            List<string> pLstInt = new List<string>();
            List<double> cAngles = new List<double>();
            List<diameterMeterFeat> diametersWithPoints = new List<diameterMeterFeat>();
         

            UID pId = new UID();
            IFeature pTempFeat = null;
            try
            {



                pPoint = (IPoint)((IFeature)pPointFeature).Shape;
                if (pPoint.IsEmpty)
                {
                    return 0;
                }
                //Create spatial filter to find intersecting features at this given point


                pSimpJunc = (ISimpleJunctionFeature)pPointFeature;


                for (int i = 0; i <= pSimpJunc.EdgeFeatureCount - 1; i++)
                {
                    pEdgeFeat = pSimpJunc.get_EdgeFeature(i);

                    if (pLstInt.Count > 0)
                    {
                        if (pLstInt.Contains(((IFeature)pEdgeFeat).Class.ObjectClassID + " " + ((IFeature)pEdgeFeat).OID))
                        {
                            continue;

                        }
                    }

                    pTempFeat = (IFeature)pEdgeFeat;

                    if (strLayerName == Globals.getClassName((IDataset)pTempFeat.Class) || strLayerName == "" || strLayerName == null)
                    {
                        pLstInt.Add(((IFeature)pEdgeFeat).Class.ObjectClassID.ToString() + " " + ((IFeature)pEdgeFeat).OID.ToString());

                        dblAngle = Globals.GetAngleOfLineAtPoint((IPolyline)pTempFeat.ShapeCopy, pPoint, Globals.GetXYTolerance(pPoint));
                        dblAngle = Globals.ConvertRadsToDegrees(dblAngle);


                        //Convert to geographic degrees(zero north clockwise)
                        if (!(bArithmeticAngle))
                        {
                            dblAngle = Globals.ConvertArithmeticToGeographic(dblAngle);
                        }



                        //Round angle
                        dblAngle = Math.Round(dblAngle, 4);

                        //Find diameter field, if it exists
                        iLineDiameterFieldPos = ((IFeature)pEdgeFeat).Fields.FindField(strDiameterFld);

                        //Get diameter of line
                        if (iLineDiameterFieldPos < 0)
                        {
                            dblDiameter = -9999;
                        }
                        else if (((IFeature)pEdgeFeat).get_Value(iLineDiameterFieldPos) == null)
                        {
                            dblDiameter = -9999;
                        }
                        else if (object.ReferenceEquals(((IFeature)pEdgeFeat).get_Value(iLineDiameterFieldPos), DBNull.Value))
                        {
                            dblDiameter = -9999;
                        }
                        else
                        {
                            double.TryParse(((IFeature)pEdgeFeat).get_Value(iLineDiameterFieldPos).ToString(), out dblDiameter);
                        }


                        //add this line (angle and diameter) to a collection of line info for this point
                        cAngles.Add(dblAngle);

                      
                            diamPnt = new diameterMeterFeat();
                            
                            diamPnt.dblDiameter = dblDiameter;
                            diamPnt.pntStart = ((IPolyline)pTempFeat.Shape).FromPoint;
                            diamPnt.pntEnd = ((IPolyline)pTempFeat.Shape).ToPoint;
                            diamPnt.angle = dblAngle;
                            diametersWithPoints.Add(diamPnt);

                           
                      

                    }
                    

                }

                //Read the collection of line segment angles and diameters
                //and use them to derive a symbol rotation angle for the point
                switch (cAngles.Count)
                {
                    case 0:
                        //One line such as at valves

                        return 0.0;

                    case 1:
                        //One line such as at valves
                        return cAngles[0];
                    case 2:
                        //Two lines such as at reducers Or at tee fittings where line is not broken

                        if (diametersWithPoints.Count == 2)
                        {
                            //If cDiameters(0) Is Nothing Or cDiameters(1) Is Nothing Then
                            //    Return cAngles.Item(0)
                            //Else
                            if (diametersWithPoints[0].dblDiameter == -9999 && diametersWithPoints[1].dblDiameter == -9999)
                            {
                                return diametersWithPoints[0].angle;
                            }
                            else if (diametersWithPoints[0].dblDiameter == -9999)
                            {
                                return diametersWithPoints[1].angle;
                            }
                            else if (diametersWithPoints[1].dblDiameter == -9999) {
                                return diametersWithPoints[0].angle;
                            }

                            else if (diametersWithPoints[0].dblDiameter > diametersWithPoints[1].dblDiameter)
                            {
                                if (Globals.pointscoincident(diametersWithPoints[0].pntStart, diametersWithPoints[1].pntStart))
                                {
                                    //Checked
                                    return diametersWithPoints[0].angle;
                                }
                                else if (Globals.pointscoincident(diametersWithPoints[0].pntEnd, diametersWithPoints[1].pntEnd))
                                {
                                    //Checked
                                    if (diametersWithPoints[0].angle >= 180)
                                    {
                                        return diametersWithPoints[0].angle - 180;
                                    }
                                    else
                                    {
                                        return diametersWithPoints[0].angle + 180;
                                    }
                                }
                                else if (Globals.pointscoincident(diametersWithPoints[0].pntEnd, diametersWithPoints[1].pntStart))
                                {
                                    //Checked
                                    if (diametersWithPoints[0].angle >= 180)
                                    {
                                        return diametersWithPoints[0].angle - 180;
                                    }
                                    else
                                    {
                                        return diametersWithPoints[0].angle + 180;
                                    }

                                }
                                else
                                {
                                    //Checked
                                    return diametersWithPoints[0].angle;
                                }


                            }
                            else if (diametersWithPoints[0].dblDiameter < diametersWithPoints[1].dblDiameter)
                            {
                                if (Globals.pointscoincident(diametersWithPoints[0].pntStart, diametersWithPoints[1].pntStart))
                                {
                                    //Checked
                                    return diametersWithPoints[1].angle;
                                }
                                else if (Globals.pointscoincident(diametersWithPoints[0].pntEnd, diametersWithPoints[1].pntEnd))
                                {
                                    //Checked
                                    if (diametersWithPoints[1].angle >= 180)
                                    {
                                        return diametersWithPoints[1].angle - 180;
                                    }
                                    else
                                    {
                                        return diametersWithPoints[1].angle + 180;
                                    }
                                }
                                else if (Globals.pointscoincident(diametersWithPoints[0].pntEnd, diametersWithPoints[1].pntStart))
                                {
                                    //Checked
                                    return diametersWithPoints[1].angle;
                                }
                                else
                                {
                                    //Checked
                                    if (diametersWithPoints[1].angle >= 180)
                                    {
                                        return diametersWithPoints[1].angle - 180;
                                    }
                                    else
                                    {
                                        return diametersWithPoints[1].angle + 180;
                                    }
                                }
                            }
                            else
                            {
                                return diametersWithPoints[0].angle;

                            }

                        }
                        else
                        {
                            return cAngles[0];
                        }

                        break;
                    case 3:
                        //Three lines such as at tee fittings where line is broken
                        ltest = Math.Abs(cAngles[0] - cAngles[1]);
                        if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                        {
                            return cAngles[2];
                        }
                        else
                        {
                            ltest = Math.Abs(cAngles[0] - cAngles[2]);
                            if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                            {
                                return cAngles[1];
                            }
                            else
                            {
                                ltest = Math.Abs(cAngles[1] - cAngles[2]);
                                if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                                {
                                    return cAngles[0];
                                }
                                else
                                {
                                    return -360;
                                }
                            }
                        }
                        break;
                    case 4:
                        //Four lines such as at crosses
                        //the angle of any of the four lines should work since the symbol should be symetrically
                        return cAngles[0];
                    default:
                        return 0;
                }

                //Clear collections

            }
            catch
            {
                return 0;
            }
            finally
            {
                diamPnt = null;
                pPoint=null;
                pSimpJunc = null;
                pEdgeFeat = null;

                pLstInt.Clear();
                cAngles.Clear();
                diametersWithPoints.Clear(); 
                pId  = null;
                pTempFeat = null;
            }

        }

    }
}
