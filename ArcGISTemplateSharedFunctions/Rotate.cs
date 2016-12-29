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
        public Nullable<double> RotatePoint(IMap pMap,double mapTol , IFeature pPointFeature, bool bArithmeticAngle, string strDiameterFld, string strLayerName)
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

            
            List<string> pLstInt = new List<string>();
           

            double dblAngle = 0;
            double dblDiameter = 0;
            double ltest = 0;
            int iLineDiameterFieldPos = 0;
            List<diameterMeterFeat> diametersWithPoints = new List<diameterMeterFeat>();
            double xyTol;
//double mapTol = ((IMxDocument)ArcMap.Application.Document).SearchTolerance;
            try
            {
                //This routine is used by both RotateDuringCreateFeature and RotateSelectedFeature.
                //It contains all of logic for determining the rotation angle.

                const int iAngleTol = 5;
               ;
                //Used for Tees> a straight line is 180 + or - iAngleTol
                pPointFC = (IFeatureClass)pPointFeature.Class;
                pPoint = (IPoint)pPointFeature.Shape;
                xyTol = Globals.GetXYTolerance(pPoint);
                //Create spatial filter to find intersecting features at this given point
                pTopo = (ITopologicalOperator)pPoint;

                pSFilter = new SpatialFilter();
                pSFilter.Geometry = pTopo.Buffer(mapTol);
                //pPoint
                pSFilter.GeometryField = pPointFC.ShapeFieldName;
                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //Step through each feature layer
                pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                //GeoFeatureLayer
                pEnumLayer = (IEnumLayer)pMap.get_Layers(pUID, true);
                pEnumLayer.Reset();
                pLayer = (ILayer)pEnumLayer.Next();

                diameterMeterFeat diamPnt = null;
                while ((pLayer != null))
                {
                    //Verify that this is a line layer
                    pFLayer = (IFeatureLayer)pLayer;

                    if (pFLayer.FeatureClass != null)
                    {


                        if (pFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline && pFLayer.Visible && (strLayerName == "" || strLayerName == null || strLayerName == Globals.getClassName((IDataset)pFLayer.FeatureClass)))
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
                                string listInt;
                                diamPnt = null;
                                angleLogic(pPoint, pLineFeature, bArithmeticAngle, strDiameterFld, xyTol, out listInt, out diamPnt);
                                pLstInt.Add(listInt);
                                
                                diametersWithPoints.Add(diamPnt);
                                //Get next line
                                pLineFeature = pLineCursor.NextFeature();
                            }
                        }
                    }
                    //Get next line layer
                    pLayer = pEnumLayer.Next();
                }
                return getAngle(diametersWithPoints, iAngleTol);
              
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

               
                pLstInt.Clear();
                

            }

        }
        private void angleLogic(IPoint pPoint, IFeature pFeat, bool bArithmeticAngle, string strDiameterFld, double xyTol, out string listInt, out diameterMeterFeat diamPnt)
        {
            double dblDiameter = 0;
            double dblAngle;
            int iLineDiameterFieldPos = 0;

            listInt = pFeat.Class.ObjectClassID.ToString() + " " + pFeat.OID.ToString();
            
            dblAngle = Globals.GetAngleOfLineAtPoint((IPolyline)pFeat.ShapeCopy, pPoint, Globals.GetXYTolerance(pPoint));
            dblAngle = Globals.ConvertRadsToDegrees(dblAngle);


            //Convert to geographic degrees(zero north clockwise)
            if (!(bArithmeticAngle))
            {
                dblAngle = Globals.ConvertArithmeticToGeographic(dblAngle);
            }



            //Round angle
            dblAngle = Math.Round(dblAngle, 4);

            //Find diameter field, if it exists
            
            iLineDiameterFieldPos = pFeat.Fields.FindField(strDiameterFld);

            //Get diameter of line
            if (iLineDiameterFieldPos < 0)
            {
                dblDiameter = -9999;
            }
            else if (pFeat.get_Value(iLineDiameterFieldPos) == null)
            {
                dblDiameter = -9999;
            }
            else if (object.ReferenceEquals(pFeat.get_Value(iLineDiameterFieldPos), DBNull.Value))
            {
                dblDiameter = -9999;
            }
            else
            {
                double.TryParse(pFeat.get_Value(iLineDiameterFieldPos).ToString(), out dblDiameter);
            }


            
            diamPnt = new diameterMeterFeat();

            diamPnt.dblDiameter = dblDiameter;
            double distFrom = Globals.GetDistanceBetweenPoints(((IPolyline)pFeat.Shape).FromPoint, pPoint);
            double distTo = Globals.GetDistanceBetweenPoints(((IPolyline)pFeat.Shape).ToPoint, pPoint);
            if (distFrom < xyTol * 2)
            {
                diamPnt.location = "From";
            }
            else if (distTo < xyTol * 2)
            {
                diamPnt.location = "To";
            }
            else
            {
                diamPnt.location = null;
            }
            diamPnt.pntStart = ((IPolyline)pFeat.Shape).FromPoint;
            diamPnt.pntEnd = ((IPolyline)pFeat.Shape).ToPoint;
            diamPnt.angle = dblAngle;
            
        }
        private Nullable<double> getAngle(List<diameterMeterFeat> diametersWithPoints, int iAngleTol)
        {
            double ltest = 0;
            switch (diametersWithPoints.Count)
            {
                case 0:
                    //One line such as at valves

                    return null;

                case 1:
                    //One line such as at valves
                    if (diametersWithPoints[0].location == "From")
                    {
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
                        return diametersWithPoints[0].angle;
                    }
                    return diametersWithPoints[0].angle;
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
                        else if (diametersWithPoints[1].dblDiameter == -9999)
                        {
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
                        return diametersWithPoints[0].angle;
                    }

                    break;
                case 3:
                    double flatAngle1 = flattenAngle(diametersWithPoints[0].angle);
                    double flatAngle2 = flattenAngle(diametersWithPoints[1].angle);
                    double flatAngle3 = flattenAngle(diametersWithPoints[2].angle);

                    double angleDifA = Math.Abs(flatAngle1 - flatAngle2);
                    double angleDifB = Math.Abs(flatAngle1 - flatAngle3);
                    double angleDifC = Math.Abs(flatAngle2 - flatAngle3);

                    if (angleDifA <= (iAngleTol * 2) || angleDifA >= (180 - (iAngleTol * 2)))
                    {
                        if (diametersWithPoints[2].location == "From")
                        {
                            if (diametersWithPoints[2].angle >= 180)
                            {
                                return diametersWithPoints[2].angle - 180;
                            }
                            else
                            {
                                return diametersWithPoints[2].angle + 180;
                            }

                        }
                        else
                        {
                            return diametersWithPoints[2].angle;
                        }
                    }
                    if (angleDifB <= (iAngleTol * 2) || angleDifB >= (180 - (iAngleTol * 2)))
                    {
                        if (diametersWithPoints[1].location == "From")
                        {
                            if (diametersWithPoints[1].angle >= 180)
                            {
                                return diametersWithPoints[1].angle - 180;
                            }
                            else
                            {
                                return diametersWithPoints[1].angle + 180;
                            }

                        }
                        else
                        {
                            return diametersWithPoints[1].angle;
                        }

                    }
                    if (angleDifC <= (iAngleTol * 2) || angleDifC >= (180 - (iAngleTol * 2)))
                    {
                        if (diametersWithPoints[0].location == "From")
                        {
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
                            return diametersWithPoints[0].angle;
                        }
                    }
                    //Three lines such as at tee fittings where line is broken
                    ltest = Math.Abs(diametersWithPoints[0].angle - diametersWithPoints[1].angle);
                    if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                    {
                        return diametersWithPoints[2].angle;
                    }
                    else
                    {
                        ltest = Math.Abs(diametersWithPoints[0].angle - diametersWithPoints[2].angle);
                        if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                        {
                            return diametersWithPoints[1].angle;
                        }
                        else
                        {
                            ltest = Math.Abs(diametersWithPoints[1].angle - diametersWithPoints[2].angle);
                            if (ltest >= 180 - iAngleTol & ltest <= 180 + iAngleTol)
                            {
                                return diametersWithPoints[0].angle;
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
                    return diametersWithPoints[0].angle;
                default:
                    return 0;
            }

        }
        public class diameterMeterFeat
        {
            public double dblDiameter { get; set; }
            public IPoint pntStart { get; set; }
            public IPoint pntEnd { get; set; }
            public double angle { get; set; }
            public string location { get; set; }
        }
        private double flattenAngle(double in_angle)
        {

            double newAngle = in_angle;
            while (newAngle <= 0)
            {
                newAngle = newAngle + 360;
            }
            while (newAngle >= 180)
            {
                newAngle = newAngle - 180;
            }
            return newAngle;

        }
        public Nullable<double> RotatePointByNetwork(IMap pMap, INetworkFeature pPointFeature, bool bArithmeticAngle, string strDiameterFld, string strLayerName)
        {
            //This routine is used by both RotateDuringCreateFeature and RotateSelectedFeature.
            //It contains all of logic for determining the rotation angle.
            double xyTol;

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

                xyTol = Globals.GetXYTolerance(pPoint);
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
                        string listInt;
                        diamPnt = null;
                        angleLogic(pPoint,pTempFeat,bArithmeticAngle,strDiameterFld,xyTol,out listInt,out diamPnt);
                        pLstInt.Add(listInt);
                       
                        diametersWithPoints.Add(diamPnt);

                    }

                }
                return getAngle( diametersWithPoints, iAngleTol);
              

            }
            catch
            {
                return 0;
            }
            finally
            {
                diamPnt = null;
                pPoint = null;
                pSimpJunc = null;
                pEdgeFeat = null;

                pLstInt.Clear();
            
                diametersWithPoints.Clear();
                pId = null;
                pTempFeat = null;
            }

        }

    }
}
