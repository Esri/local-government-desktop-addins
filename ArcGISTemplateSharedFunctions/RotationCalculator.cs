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
                        rotationAngle = rotFunc.RotatePoint(pMxDoc.FocusMap, pMxDoc.SearchTolerance,inFeature, m_rotationType == esriSymbolRotationType.esriRotateSymbolArithmetic, m_diameterFieldName,m_onlyLayerName);


                    }
                    else
                    {
                        rotationAngle = rotFunc.RotatePointByNetwork(pMxDoc.FocusMap, (INetworkFeature)inFeature, m_rotationType == esriSymbolRotationType.esriRotateSymbolArithmetic, m_diameterFieldName,m_onlyLayerName);

                    }

                    //If needed, convert to geographic degrees(zero north clockwise)
                    if (rotationAngle != null )
                    {
                        if (rotationAngle > 360) rotationAngle -= 360;
                        if (rotationAngle < 0) rotationAngle += 360;

                        rotationAngle += m_spinAngle;
                        if (rotationAngle > 360) rotationAngle -= 360;
                        if (rotationAngle < 0) rotationAngle += 360;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Rotation Feature at " + inFeature.Class.AliasName + " with Object ID:" + inFeature.OID + "\n" + ex.ToString());
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