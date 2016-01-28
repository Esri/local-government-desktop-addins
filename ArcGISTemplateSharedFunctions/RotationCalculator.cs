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


using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ArcMapUI;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.EditorExt;

namespace A4LGSharedFunctions
{

    public class RotationCalculator
    {
        IApplication _app;
        public RotationCalculator(IApplication app)
        {
            _app = app;
        }
        #region Properties

        public ESRI.ArcGIS.Carto.esriSymbolRotationType RotationType
        {
            get { return m_rotationType; }
            set { m_rotationType = value; }
        }
        public Boolean UseDiameter
        {
            get { return m_useDiameter; }
            set { m_useDiameter = value; }
        }
        public string DiameterFieldName
        {
            get { return m_diameterFieldName; }
            set { m_diameterFieldName = value; }
        }
        public string OnlyLayerName
        {
            get { return m_onlyLayerName; }
            set { m_onlyLayerName = value; }
        }
        public double SpinAngle
        {
            get { return m_spinAngle; }
            set { m_spinAngle = value; }
        }

        private esriSymbolRotationType m_rotationType;
        private Boolean m_useDiameter;
        private string m_diameterFieldName;
        private string m_onlyLayerName;
        private double m_spinAngle;

        #endregion

        // Uses Geometric Network to find connected edges which determine desired rotation of point

        public Nullable<double> GetRotationUsingConnectedEdges(IFeature inFeature)
        {
            Nullable<double> rotationAngle = null;

            if (inFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {

                try
                {
                   // double diameter = -1;
                    List<double> angles = new List<double>();
                    List<double> diameters = new List<double>();
                    List<Boolean> flipDirections = new List<Boolean>();

                    IPoint pnt = (ESRI.ArcGIS.Geometry.IPoint)inFeature.ShapeCopy;
                    IMxDocument pMxDoc = (IMxDocument)_app.Document;
                    double snapdistnet = Globals.ConvertPixelsToMap(2, pMxDoc.FocusMap);


                    // IMxDocument pMxDoc = (IMxDocument)_app.Document;
                    // VBRotate rotFunc = new VBRotate();
                    Rotate rotFunc = new Rotate();

                    if (!(inFeature is INetworkFeature))
                    {
                        rotationAngle = rotFunc.RotatePoint(pMxDoc.FocusMap, inFeature, m_rotationType == esriSymbolRotationType.esriRotateSymbolArithmetic, m_diameterFieldName,m_onlyLayerName);


                    }
                    else
                    {
                        rotationAngle = rotFunc.RotatePointByNetwork(pMxDoc.FocusMap, (INetworkFeature)inFeature, m_rotationType == esriSymbolRotationType.esriRotateSymbolArithmetic, m_diameterFieldName,m_onlyLayerName);

                    }

                    //else
                    //{
                    //    INetworkAnalysisExt pNetAnalysisExt;
                    //    UID pUID = new UIDClass();
                    //    pUID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    //    pNetAnalysisExt = (INetworkAnalysisExt)_app.FindExtensionByCLSID(pUID);

                    //    INetworkFeature netFeat = (INetworkFeature)inFeature;
                    //    ISimpleJunctionFeature simpleJunctionFeature = (ISimpleJunctionFeature)netFeat;
                    //    INetworkClass netClass = (INetworkClass)inFeature.Class;
                    //    IGeometricNetwork geomNetwork = (IGeometricNetwork)netClass.GeometricNetwork;
                    //    INetwork network = (INetwork)geomNetwork.Network;
                    //    INetElements netElements = (INetElements)network;

                    //    IFeatureClass fc = null;
                    //    IFeature feat = null;
                    //    IGeometry geometry = null;
                    //    IPolyline polyline = null;
                    //    ISegmentCollection segmentCollection = null;
                    //    ISegmentCollection segColTest = null;
                    //    ISegment testSegment = null;
                    //    // IEnumSegment enumSegment = null;
                    //    System.Object edgeWeight;
                    //    Boolean edgeOrient;
                    //    //int partIndex = 0;
                    //    //int segmentIndex = 0;
                    //    int edgesCount;
                    //    int edgeEID;
                    //    int classId; int userId; int subId;
                    //    int posField; double angle;
                    //    object Missing = Type.Missing;

                    //    IPoint toPoint, fromPoint;
                    //    //ITopologicalOperator topoOp = null ;
                    //    //IPolygon poly = null;

                    //    IForwardStarGEN forwardStar = (IForwardStarGEN)network.CreateForwardStar(false, null, null, null, null);
                    //    forwardStar.FindAdjacent(0, simpleJunctionFeature.EID, out edgesCount);

                    //    if (edgesCount == 0)
                    //    {
                    //        if (simpleJunctionFeature == null)
                    //            return null;
                    //        if (simpleJunctionFeature.EdgeFeatureCount <= 0)
                    //            return null;
                    //        for (int i = 0; i < simpleJunctionFeature.EdgeFeatureCount; i++)
                    //        {
                    //            IEdgeFeature edgeFeat = null;
                    //            try
                    //            {
                    //                edgeFeat = simpleJunctionFeature.get_EdgeFeature(i);
                    //            }
                    //            catch
                    //            {

                    //                MessageBox.Show("GetRotationUsingConnectedEdge:  Not an edge feature");
                    //            }


                    //            try
                    //            {
                    //                polyline = (IPolyline5)edgeFeat.get_GeometryForEdgeElement(0);
                    //            }
                    //            catch
                    //            {

                    //                MessageBox.Show("GetRotationUsingConnectedEdge:  Not a polyline");
                    //            }


                    //            IHitTest pHt = (IHitTest)polyline;
                    //            //double pHitDist = -1;
                    //            //int pHitPrt = -1;
                    //            //int pHitSeg = -1;
                    //            //bool pHitSide = false;
                    //            IPoint pHitPnt = new PointClass();
                    //            // IPoint pHit = new Point();
                    //            System.Double hitDistance = 0;
                    //            System.Int32 hitPartIndex = 0;
                    //            System.Int32 hitSegmentIndex = 0;
                    //            System.Boolean rightSide = false;



                    //            System.Boolean foundGeometry = pHt.HitTest(pnt, pNetAnalysisExt.SnapTolerance * 2,
                    //                ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary,
                    //                pHitPnt, ref hitDistance, ref hitPartIndex, ref hitSegmentIndex, ref
                    //                rightSide);


                    //            //pHt.HitTest(pnt, pNetAnalysisExt.SnapTolerance * 2, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPnt, dht, lPart, lVertex, ref bright);


                    //            if (foundGeometry)
                    //            {
                    //                ESRI.ArcGIS.Geometry.IGeometryCollection geometryCollection_Polyline = (ESRI.ArcGIS.Geometry.IGeometryCollection)polyline;
                    //                ESRI.ArcGIS.Geometry.IGeometry geoFound = geometryCollection_Polyline.get_Geometry(hitPartIndex);
                    //                IPath pPath = geoFound as IPath;



                    //                segmentCollection = (ISegmentCollection)pPath;

                    //                testSegment = segmentCollection.get_Segment(hitSegmentIndex);

                    //                try
                    //                {
                    //                    angle = GetAngleOfSegment(testSegment);
                    //                }
                    //                catch
                    //                {
                    //                    angle = -1;
                    //                    MessageBox.Show("GetRotationUsingConnectedEdge:  Error Getting Angle");
                    //                }

                    //                toPoint = testSegment.ToPoint;
                    //                //  foPoint = testSegment.ToPoint;
                    //                segColTest = new PolylineClass();
                    //                segColTest.AddSegment(testSegment, ref Missing, ref Missing);
                    //                IRelationalOperator2 relOp;

                    //                relOp = toPoint as IRelationalOperator2;
                    //                ITopologicalOperator topoOp;
                    //                IPolygon poly;
                    //                topoOp = pnt as ITopologicalOperator;

                    //                poly = topoOp.Buffer(pNetAnalysisExt.SnapTolerance * 2) as IPolygon;
                    //                //    poly = topoOp.Buffer(snapdistnet) as IPolygon;
                    //                if (relOp.Touches(poly))
                    //                {
                    //                    flipDirections.Add(relOp.Contains(pnt));
                    //                    if (diameter >= 0)
                    //                    {
                    //                        diameters.Add(diameter);
                    //                    }
                    //                }
                    //                angles.Add(angle);

                    //            }

                    //        }
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < edgesCount; i++)
                    //        {

                    //            forwardStar.QueryAdjacentEdge(i, out edgeEID, out edgeOrient, out edgeWeight);

                    //            try
                    //            {
                    //                geometry = geomNetwork.get_GeometryForEdgeEID(edgeEID);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                netElements.QueryIDs(edgeEID, esriElementType.esriETEdge, out classId, out userId, out subId);
                    //                fc = GetFeatureClassByClassId(classId, geomNetwork);
                    //                MessageBox.Show("Error getting Edge Geometry\n" +
                    //                                "Edge EID is " + edgeEID + "\n" +
                    //                                fc.AliasName + " with Object ID of " + userId + "\n" +
                    //                                 inFeature.Class.AliasName + " with Object ID:" + inFeature.OID + "\n" + ex.Message +
                    //                                 "\n-------------------------\n" +
                    //                                 "If the error says no Geometric newtork, it probably should say\n" +
                    //                                "The edge feature has the same from and to junction feature - delete network element and rebuild connectivity\n");
                    //                continue;
                    //            }
                    //            // IEdgeFeature edgeFeat = null;
                    //            try
                    //            {
                    //                polyline = (IPolyline5)geometry;
                    //            }
                    //            catch
                    //            {
                    //                polyline = null;
                    //                MessageBox.Show("GetRotationUsingConnectedEdge:  error casting to polyline");
                    //            }





                    //            //Special case for reducer
                    //            if (m_useDiameter & (edgesCount == 2))
                    //            {
                    //                netElements.QueryIDs(edgeEID, esriElementType.esriETEdge, out classId, out userId, out subId);
                    //                fc = GetFeatureClassByClassId(classId, geomNetwork);
                    //                feat = fc.GetFeature(userId);
                    //                posField = GetFieldPosition(m_diameterFieldName, feat);
                    //                if (posField > -1)
                    //                {
                    //                    if (feat.get_Value(posField) != null)
                    //                    {
                    //                        string DiaVal = feat.get_Value(posField).ToString();
                    //                        if (Globals.IsNumeric(DiaVal))
                    //                        {
                    //                            try
                    //                            {
                    //                                diameter = Convert.ToDouble(DiaVal);
                    //                            }
                    //                            catch
                    //                            {
                    //                                MessageBox.Show("Error getting the Diameter from " + feat.Class.AliasName + " OID: " + feat.OID);
                    //                            }


                    //                        }
                    //                        else
                    //                        {
                    //                            diameter = -1;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        diameter = -1;
                    //                    }
                    //                }
                    //            }
                    //            IHitTest pHt = (IHitTest)polyline;
                    //            //double pHitDist = -1;
                    //            //int pHitPrt = -1;
                    //            //int pHitSeg = -1;
                    //            //bool pHitSide = false;
                    //            IPoint pHitPnt = new PointClass();
                    //            // IPoint pHit = new Point();
                    //            System.Double hitDistance = 0;
                    //            System.Int32 hitPartIndex = 0;
                    //            System.Int32 hitSegmentIndex = 0;
                    //            System.Boolean rightSide = false;



                    //            System.Boolean foundGeometry = pHt.HitTest(pnt, pNetAnalysisExt.SnapTolerance * 2,
                    //                ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary,
                    //                pHitPnt, ref hitDistance, ref hitPartIndex, ref hitSegmentIndex, ref
                    //                rightSide);


                    //            //pHt.HitTest(pnt, pNetAnalysisExt.SnapTolerance * 2, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPnt, dht, lPart, lVertex, ref bright);


                    //            if (foundGeometry)
                    //            {
                    //                ESRI.ArcGIS.Geometry.IGeometryCollection geometryCollection_Polyline = (ESRI.ArcGIS.Geometry.IGeometryCollection)polyline;
                    //                ESRI.ArcGIS.Geometry.IGeometry geoFound = geometryCollection_Polyline.get_Geometry(hitPartIndex);
                    //                IPath pPath = geoFound as IPath;



                    //                segmentCollection = (ISegmentCollection)pPath;
                    //                IRelationalOperator2 relOp;

                    //                testSegment = segmentCollection.get_Segment(hitSegmentIndex);
                    //                toPoint = testSegment.ToPoint;
                    //                fromPoint = testSegment.FromPoint;
                    //                //    relOp = fromPoint as IRelationalOperator2;
                    //                // ITopologicalOperator topoOp;
                    //                //IPolygon poly;
                    //                // topoOp = pnt as ITopologicalOperator;
                    //                relOp = fromPoint as IRelationalOperator2;
                    //                //relOp = pnt as IRelationalOperator;//(topoOp.Buffer(snapdistnet) as IPolygon) as IRelationalOperator2;
                    //                //relOp = (topoOp.Buffer(snapdistnet) as IPolygon) as IRelationalOperator2;
                    //                // bool flipAng;
                    //                if (relOp.IsNear(pnt, snapdistnet))
                    //                {
                    //                    //flipDirections.Add(true);
                    //                    //flipAng = true;
                    //                    //segColTest = new PolylineClass();
                    //                    //segColTest.AddSegment(testSegment, ref Missing, ref Missing);
                    //                    //ICurve pCurv = segColTest as ICurve;
                    //                    //pCurv.ReverseOrientation();
                    //                    //segmentCollection = (ISegmentCollection)pCurv;
                    //                    //testSegment = segmentCollection.get_Segment(0);

                    //                    flipDirections.Add(false);
                    //                }
                    //                else
                    //                {
                    //                    toPoint = testSegment.FromPoint;
                    //                    fromPoint = testSegment.ToPoint;
                    //                    //flipDirections.Add(true);
                    //                    //testSegment.ToPoint = fromPoint;
                    //                    //  testSegment.FromPoint = toPoint;
                    //                    //flipDirections.Add(false);
                    //                    //flipAng = false;
                    //                }

                    //                try
                    //                {
                    //                    angle = GetAngleOfSegment(fromPoint, toPoint);
                    //                    //angle = GetAngleOfSegment2(testSegment);

                    //                }
                    //                catch
                    //                {
                    //                    angle = -1;
                    //                    MessageBox.Show("GetRotationUsingConnectedEdge:  error getting angle");
                    //                }




                    //                if (diameter >= 0)
                    //                {
                    //                    diameters.Add(diameter);
                    //                }
                    //                //if (flipAng == true && angle > 180)
                    //                //{
                    //                //    angle = angle - 180;
                    //                //}
                    //                //else if (flipAng == true && angle < 180)
                    //                //{
                    //                //    angle = angle + 180;
                    //                //}
                    //                angles.Add(angle);

                    //            }

                    //            //given line and point, return angles of all touching segments
                    //            //segmentCollection = (ISegmentCollection)polyline;
                    //            //enumSegment = (IEnumSegment)segmentCollection.EnumSegments;
                    //            //enumSegment.Next(out testSegment, ref partIndex, ref segmentIndex);

                    //            //while (testSegment != null)
                    //            //{
                    //            //    angle = GetAngleOfSegment(testSegment);
                    //            //    toPoint = testSegment.ToPoint;
                    //            //    topoOp = toPoint as ITopologicalOperator;
                    //            //    //poly = topoOp.Buffer(0.01) as IPolygon;
                    //            //    poly = topoOp.Buffer(snapdistnet) as IPolygon;
                    //            //    //ML: 20090617 Added test for segment touching point to be rotated
                    //            //    segColTest = new PolylineClass();
                    //            //    segColTest.AddSegment(testSegment, ref Missing, ref Missing);
                    //            //    relOp = segColTest as IRelationalOperator;

                    //            //    if (relOp.Touches(pnt))
                    //            //    {
                    //            //        relOp = poly as IRelationalOperator;
                    //            //        flipDirections.Add(relOp.Contains(pnt));
                    //            //        diameters.Add(diameter);
                    //            //        angles.Add(angle);
                    //            //    }
                    //            //    enumSegment.Next(out testSegment, ref partIndex, ref segmentIndex);

                    //            //}

                    //            ///end of possible function returning list of angles


                    //        }
                    //    }
                    //    switch (angles.Count)
                    //    {
                    //        case 0:
                    //            break;
                    //        case 1:
                    //            // End cap or plug fitting or simliar.
                    //            rotationAngle = angles[0];
                    //            if (flipDirections.Count == 1)
                    //            {
                    //                if (flipDirections[0]) rotationAngle += 180;
                    //            }
                    //            break;
                    //        case 2:
                    //            if (diameters != null)
                    //            {



                    //                if (diameters.Count >= 2)
                    //                {


                    //                    if (angles[0] > angles[1])
                    //                    {
                    //                        if (((angles[0] - angles[1]) % 180) < 15 || ((angles[0] - angles[1]) % 180) > 165)
                    //                        {

                    //                            if (diameters[0] != null && diameters[1] != null)
                    //                            {
                    //                                if (m_useDiameter & (diameters[0] < diameters[1]))
                    //                                {
                    //                                    rotationAngle = angles[1];
                    //                                    //if (flipDirections[0]) rotationAngle += 180;
                    //                                }
                    //                                else if (m_useDiameter & (diameters[0] >= diameters[1]))
                    //                                {
                    //                                    rotationAngle = angles[0];
                    //                                    //if (flipDirections[1]) rotationAngle += 180;
                    //                                }
                    //                                else rotationAngle = angles[0];
                    //                            }
                    //                            else
                    //                                rotationAngle = angles[0];

                    //                        }
                    //                        else
                    //                        {
                    //                            if ((angles[0] - angles[1]) % 90 > 15 && ((angles[0] - angles[1]) % 90 < 85))
                    //                            {

                    //                                if (diameters[0] != null && diameters[1] != null)
                    //                                {
                    //                                    if (m_useDiameter & (diameters[0] < diameters[1]))
                    //                                    {
                    //                                        rotationAngle = angles[1];
                    //                                        //if (flipDirections[0]) rotationAngle += 180;
                    //                                    }
                    //                                    else if (m_useDiameter & (diameters[0] >= diameters[1]))
                    //                                    {
                    //                                        rotationAngle = angles[0];
                    //                                        //if (flipDirections[1]) rotationAngle += 180;
                    //                                    }
                    //                                    else rotationAngle = angles[0];
                    //                                }
                    //                                else
                    //                                    rotationAngle = angles[0];

                    //                            }
                    //                            else
                    //                            {
                    //                                rotationAngle = (angles[0] + angles[1]) / 2;
                    //                            }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        if (((angles[1] - angles[0]) % 180) < 15 || ((angles[1] - angles[0]) % 180) > 165)
                    //                        {

                    //                            if (diameters[0] != null && diameters[1] != null)
                    //                            {
                    //                                if (m_useDiameter & (diameters[0] < diameters[1]))
                    //                                {
                    //                                    rotationAngle = angles[1];
                    //                                    //if (flipDirections[0]) rotationAngle += 180;
                    //                                }
                    //                                else if (m_useDiameter & (diameters[0] >= diameters[1]))
                    //                                {
                    //                                    rotationAngle = angles[0];
                    //                                    //if (flipDirections[1]) rotationAngle += 180;
                    //                                }
                    //                                else rotationAngle = angles[0];
                    //                            }
                    //                            else
                    //                                rotationAngle = angles[0];

                    //                        }
                    //                        else
                    //                        {
                    //                            if ((angles[1] - angles[0]) % 90 > 15 && ((angles[1] - angles[0]) % 90 < 85))
                    //                            {

                    //                                if (diameters[0] != null && diameters[1] != null)
                    //                                {
                    //                                    if (m_useDiameter & (diameters[0] < diameters[1]))
                    //                                    {
                    //                                        rotationAngle = angles[1];
                    //                                        //if (flipDirections[0]) rotationAngle += 180;
                    //                                    }
                    //                                    else if (m_useDiameter & (diameters[0] >= diameters[1]))
                    //                                    {
                    //                                        rotationAngle = angles[0];
                    //                                        //if (flipDirections[1]) rotationAngle += 180;
                    //                                    }
                    //                                    else rotationAngle = angles[0];
                    //                                }
                    //                                else
                    //                                    rotationAngle = angles[0];

                    //                            }
                    //                            else
                    //                            {
                    //                                rotationAngle = (angles[1] + angles[0]) / 2;
                    //                            }
                    //                        }
                    //                    }







                    //                }
                    //                else
                    //                    if (angles[0] > angles[1])
                    //                    {
                    //                        if (((angles[0] - angles[1]) % 180) < 15 || ((angles[0] - angles[1]) % 180) > 165)
                    //                        {
                    //                            rotationAngle = angles[0];
                    //                        }
                    //                        else
                    //                        {
                    //                            if ((angles[0] - angles[1]) % 90 > 15 && ((angles[0] - angles[1]) % 90 < 85))
                    //                            {
                    //                                rotationAngle = angles[0];
                    //                            }
                    //                            else
                    //                            {
                    //                                rotationAngle = (angles[0] + angles[1]) / 2;
                    //                            }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        if (((angles[1] - angles[0]) % 180) < 15 || ((angles[1] - angles[0]) % 180) > 165)
                    //                        {
                    //                            rotationAngle = angles[0];
                    //                        }
                    //                        else
                    //                        {
                    //                            if ((angles[1] - angles[0]) % 90 > 15 && ((angles[1] - angles[0]) % 90 < 85))
                    //                            {
                    //                                rotationAngle = angles[0];
                    //                            }
                    //                            else
                    //                            {
                    //                                rotationAngle = (angles[1] + angles[0]) / 2;
                    //                            }
                    //                        }
                    //                    }


                    //            }
                    //            break;

                    //        case 3:
                    //            //Tee or Tap fitting or similiar.  Rotate toward the odd line.
                    //            //int tee = 0;
                    //            //try
                    //            //{
                    //            //    tee = FindTee(angles[0], angles[1], angles[2]);
                    //            //}
                    //            //catch
                    //            //{
                    //            //    MessageBox.Show("Error finding Tee");
                    //            //}
                    //            //rotationAngle = angles[tee];
                    //            //if (flipDirections[tee]) rotationAngle += 180;
                    //            rotationAngle = TeeAngle(angles[0], angles[1], angles[2]);
                    //            break;
                    //        case 4:
                    //            // Cross fitting or similar. Any of the angles should work.
                    //            rotationAngle = (int)angles[0];
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //If needed, convert to geographic degrees(zero north clockwise)
                    if (rotationAngle > 360) rotationAngle -= 360;
                    if (rotationAngle < 0) rotationAngle += 360;
                    //if (rotationAngle != null & m_rotationType == esriSymbolRotationType.esriRotateSymbolArithmetic)
                    //{
                    //    int a = (int)rotationAngle;


                    //    double newAngle;// = rotationAngle


                    //    newAngle = (double)rotationAngle + 270;
                    //    while (newAngle > 360)
                    //    {
                    //        newAngle = newAngle - 360;
                    //    }
                    //    // newAngle = newAngle - 360;
                    //    //if (a > 0 & a <= 90)
                    //    //    rotationAngle = 90 - a;
                    //    //else if (a > 90 & a <= 360)
                    //    //    rotationAngle = 450 - a;

                    //}

                    //Apply any spin angle
                    if (rotationAngle != null)
                    {
                        rotationAngle += m_spinAngle;
                        if (rotationAngle > 360) rotationAngle -= 360;
                        if (rotationAngle < 0) rotationAngle += 360;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Rotation Feature at " + inFeature.Class.AliasName + " with Object ID:" + inFeature.OID + "\n" + ex.Message);
                    return -1;
                }

            }




            return rotationAngle;
        }

        #region Helper methods
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double GetAngleOfSegment(ISegment inSegment)
        {
            //double testa =  DegreeToRadian(Math.Atan((inSegment.ToPoint.Y - inSegment.FromPoint.Y) / (inSegment.ToPoint.X - inSegment.FromPoint.X)));


            ////degrees(atan((top - bottom)/(right - left)))
            ILine line = new LineClass();
            double outAngle;
            //double pi = 4 * System.Math.Atan(1);
            line.PutCoords(inSegment.FromPoint, inSegment.ToPoint);

            //line.FromPoint = inSegment.FromPoint;
            //line.ToPoint = inSegment.ToPoint;
            outAngle = (180 - (line.Angle * 180) / 3.14159265358979);

            //outAngle = (int)System.Math.Round(((180 * line.Angle) / pi), 0);
            //outAngle = line.Angle;

            if (outAngle < 0)
            {
                outAngle += 360;
            }
            return outAngle;
        }
        private static double GetAngleOfSegment(IPoint FromPoint, IPoint ToPoint)
        {
            //double testa =  DegreeToRadian(Math.Atan((inSegment.ToPoint.Y - inSegment.FromPoint.Y) / (inSegment.ToPoint.X - inSegment.FromPoint.X)));


            ////degrees(atan((top - bottom)/(right - left)))
            ILine line = new LineClass();
            double outAngle;
            double pi = 4 * System.Math.Atan(1);
            line.PutCoords(FromPoint, ToPoint);
            //if (line.Angle < 0)
            //    outAngle = (180 * line.Angle) / pi;
            //else
            //    outAngle = (180 - (line.Angle * 180) / 3.14159265358979);
            outAngle = (180 - (line.Angle * 180) / 3.14159265358979);

            //double dangle = Math.Abs(line.Angle) * 360 / (2 * pi);

            //line.FromPoint = inSegment.FromPoint;
            //line.ToPoint = inSegment.ToPoint;
            //outAngle = (int)System.Math.Round(((180 * line.Angle) / pi), 0);
            //outAngle = line.Angle;
            //outAngle = outAngle + 90;

            if (outAngle < 0)
            {
                outAngle += 360;
            }
            if (outAngle > 360)
            {
                outAngle -= 360;
            }
            return outAngle;
        }
        private static double GetAngleOfSegment2(ISegment inSegment)
        {
            ILine line = new LineClass();
            double outAngle;
            double pi = 4 * System.Math.Atan(1);
            line.FromPoint = inSegment.FromPoint;
            line.ToPoint = inSegment.ToPoint;
            outAngle = (int)System.Math.Round(((180 * line.Angle) / pi), 0);
            //outAngle = line.Angle;

            if (outAngle < 0)
            {
                outAngle += 360;
            }
            return outAngle;
        }
        private static Boolean IsStraight(int angleA, int angleB)
        {
            int tolerance = 20;
            int testAngle = Math.Abs(angleA - angleB);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle <= tolerance) return true;
            return false;
        }
        private static int FindTee(double angleA, double angleB, double angleC)
        {
            int tolerance = 20;
            double testAngle;

            testAngle = Math.Abs(angleB - angleA);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle <= tolerance) return 0;

            testAngle = Math.Abs(angleB - angleC);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle <= tolerance) return 1;

            testAngle = Math.Abs(angleA - angleC);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle <= tolerance) return 2;


            return 2;
        }
        private static double TeeAngle(double angleA, double angleB, double angleC)
        {
            int tolerance = 20;
            double testAngle;
            double smallDiff = 1000;
            double angGuess;
            testAngle = Math.Abs(angleB - angleA);
            if (testAngle > 180) testAngle -= 180;
            {
                smallDiff = testAngle;
                angGuess = angleA;
            }
            if (testAngle <= tolerance || 5 >= 180 - testAngle)
            {

                if (angleB < angleA)
                {
                    if (angleC < angleB || angleC > angleA)
                    {
                        return angleB + 180;

                    }
                    else
                    {
                        return angleB;

                    }
                }
                else if (angleA < angleB)
                {
                    if (angleC < angleA || angleC > angleB)
                    {
                        return angleB;

                    }
                    else
                    {
                        return angleB + 180;

                    }
                }
                else
                {
                    if (angleC < angleA || angleC > angleB)
                    {
                        return angleB;

                    }
                    else
                    {
                        return angleB + 180;

                    }
                }


                //if (angleA < angleC)
                //{
                //    return angleA;
                //}
                //else
                //   return  angleA + 180;

                //if (angleC < 90)

                //    return angleA + 90;
                //else if (angleC < 180)
                //    return angleC + 90;
                //else if (angleC < 270)
                //    return angleC - 90;
                //else if (angleC < 360)
                //    return angleC - 270;

                //if (angleA > angleC)
                //    return angleA + 180;
                //else
                //    return angleA ;
            }

            testAngle = Math.Abs(angleB - angleC);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle < smallDiff)
            {
                smallDiff = testAngle;
                angGuess = angleB;
            }
            if (testAngle <= tolerance || 5 >= 180 - testAngle)
            {


                if (angleB < angleC)
                {
                    if (angleA < angleB || angleA > angleC)
                    {
                        return angleB + 180;

                    }
                    else
                    {
                        return angleB;

                    }
                }
                else if (angleC < angleB)
                {
                    if (angleA < angleC || angleA > angleB)
                    {
                        return angleB;

                    }
                    else
                    {
                        return angleB + 180;

                    }
                }
                //if (angleB < angleA)
                //{
                //    return angleB + 180;
                //}
                //else
                //    return angleB ;

                // return angleA;
                //if (angleA < 90)

                //    return angleA - 90;
                //else if (angleA < 180)
                //    return angleA - 90;
                //else if (angleA < 270)
                //    return angleA - 90;
                //else if (angleA < 360)
                //    return angleA - 270;

                //if (angleB > angleA)
                //    return angleB + 180;
                //else
                //    return angleB;
            }
            testAngle = Math.Abs(angleA - angleC);
            if (testAngle > 180) testAngle -= 180;
            if (testAngle < smallDiff)
            {
                smallDiff = testAngle;
                angGuess = angleC;
            }

            if (testAngle <= tolerance || 5 >= 180 - testAngle)
            {
                if (angleA < angleC)
                {
                    if (angleB < angleA || angleB > angleC)
                    {
                        return angleA + 180;

                    }
                    else
                    {
                        return angleA;

                    }
                }
                else if (angleC < angleA)
                {
                    if (angleB < angleC || angleB > angleA)
                    {
                        return angleC + 180;

                    }
                    else
                    {
                        return angleC;

                    }
                }
                //if (angleB < angleA || angleB > angleC)

                //    return angleA + 180;
                //else if (angleB < angleC || angleB > angleA)
                //        return angleA + 180;
                //else
                //    return angleA;
                // return angleB;
                //if (angleB < 90)

                //    return angleB - 90;
                //else if (angleB < 180)
                //    return angleB + 90;
                //else if (angleB < 270)
                //    return angleB - 90;
                //else if (angleB < 360)
                //    return angleB - 270;

                //if (angleA > angleB)
                //    return angleA + 180;
                //else
                //    return angleA;
            }

            //if (flipAng == true && angle > 180)
            //{
            //    angle = angle - 180;
            //}
            //else if (flipAng == true && angle < 180)
            //{
            //    angle = angle + 180;
            //}
            // if (angGuess < 90)

            //    return angGuess + 90;
            //else if (angGuess < 180)
            //    return angGuess - 90;
            //else if (angGuess < 270)
            //    return angGuess - 90;
            //else if (angGuess < 360)
            //    return angGuess - 270;
            //else
            return angGuess;

        }
        private int GetFieldPosition(string fieldName, IFeature feature)
        {
            int posField = feature.Fields.FindField(fieldName);
            if (posField > -1)
            {
                switch (feature.Fields.get_Field(posField).Type)
                {
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeSmallInteger:
                        return posField;
                    default:
                        posField = -1;
                        return posField;
                }
            }
            return posField;

        }
        private IFeatureClass GetFeatureClassByClassId(int classId, IGeometricNetwork geomNetwork)
        {
            IEnumFeatureClass enumFC;
            IFeatureClass fc;
            enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
            fc = enumFC.Next();
            while (fc != null)
            {
                if (fc.FeatureClassID == classId) return fc;
                fc = enumFC.Next();
            }
            enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
            fc = enumFC.Next();
            while (fc != null)
            {
                if (fc.FeatureClassID == classId) return fc;
                fc = enumFC.Next();
            }
            return null;

        }
        #endregion


    }
}