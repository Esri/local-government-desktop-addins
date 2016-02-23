/*
 | Version 10.4
 | Copyright 2014 Esri
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


using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.Analyst3D;


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Resources;

using stdole;

//using System;
//using System.ComponentModel;
//using System.Drawing;
//using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;



using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Location;

//using ESRI.ArcGIS.CartoUI;
//using ESRI.ArcGIS.ArcMapUI;
//using ESRI.ArcGIS.ArcMap;
//using ESRI.ArcGIS.Display;
//using ESRI.ArcGIS.Carto;
//using ESRI.ArcGIS.Framework;
//using ESRI.ArcGIS.Geodatabase;
//using ESRI.ArcGIS.GeoDatabaseUI;
//using ESRI.ArcGIS.Geometry;
//using ESRI.ArcGIS.esriSystem;
//using ESRI.ArcGIS.SystemUI;
//using ESRI.ArcGIS.DataSourcesRaster;
//using ESRI.ArcGIS.DataSourcesFile;
//using ESRI.ArcGIS.DataSourcesGDB;

//using ESRI.ArcGIS.Analyst3D;

using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.EditorExt;

using Microsoft.Win32;
using Microsoft.VisualBasic;


namespace A4LGSharedFunctions
{
    public class geoFeat
    {
        public IGeometryCollection geo { get; set; }
        public IFeature feature { get; set; }

    }
    public class copyFeatureToInMem
    {
        public IFeatureLayer FeatureLayer { get; set; }
        public Dictionary<int, geoFeat> OIDSGeo { get; set; }

    }
    #region WindowsAPI Calls
    //Helper class for converting symbols
    // from the sandpit: http://kiwigis.blogspot.com/2009/05/accessing-esri-style-files-using-adonet.html
    public static class WindowsAPI
    {
        private const int COLORONCOLOR = 3;
        private const int HORZSIZE = 4;
        private const int VERTSIZE = 6;
        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int ASPECTX = 40;
        private const int ASPECTY = 42;
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        private enum PictureTypeConstants
        {
            picTypeNone = 0,
            picTypeBitmap = 1,
            picTypeMetafile = 2,
            picTypeIcon = 3,
            picTypeEMetafile = 4
        }
        private struct PICTDESC
        {
            public int cbSizeOfStruct;
            public int picType;
            public IntPtr hPic;
            public IntPtr hpal;
            public int _pad;
        }
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("olepro32.dll", EntryPoint = "OleCreatePictureIndirect",
                   PreserveSig = false)]
        private static extern int OleCreatePictureIndirect(
            ref PICTDESC pPictDesc, ref Guid riid, bool fOwn,
            out IPictureDisp ppvObj);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC",
            ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr SelectObject(
            IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject",
            ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleBitmap(
            IntPtr hObject, int width, int height);

        [DllImport("user32.dll", EntryPoint = "GetDC",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32", EntryPoint = "CreateSolidBrush",
            ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("user32", EntryPoint = "FillRect",
            ExactSpelling = true, SetLastError = true)]
        private static extern int FillRect(
            IntPtr hdc, ref RECT lpRect, IntPtr hBrush);

        [DllImport("GDI32.dll", EntryPoint = "GetDeviceCaps",
            ExactSpelling = true, SetLastError = true)]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32", EntryPoint = "GetClientRect",
            ExactSpelling = true, SetLastError = true)]
        private static extern int GetClientRect(
            IntPtr hwnd, ref RECT lpRect);

        public static ISymbol GetSymbol(
            string style, string classname, int id)
        {
            ISymbol symbol = null;
            IStyleGallery styleGallery = new StyleGalleryClass();
            IStyleGalleryStorage styleGalleryStorage =
                (IStyleGalleryStorage)styleGallery;
            styleGalleryStorage.TargetFile = style;
            IEnumStyleGalleryItem styleGalleryItems =
                styleGallery.get_Items(classname, style, "");
            styleGalleryItems.Reset();
            IStyleGalleryItem styleGalleryItem = styleGalleryItems.Next();
            while (styleGalleryItem != null)
            {
                if (styleGalleryItem.ID == id)
                {
                    symbol = (ISymbol)styleGalleryItem.Item;
                    break;
                }
                styleGalleryItem = styleGalleryItems.Next();
            }
            styleGalleryItem = null;
            styleGalleryStorage = null;
            styleGallery = null;

            return symbol;
        }
        private static IPictureDisp CreatePictureFromSymbol(
            IntPtr hDCOld, ref IntPtr hBmpNew,
            ISymbol pSymbol, Size size, int lGap, int backColor)
        {
            IntPtr hDCNew = IntPtr.Zero;
            IntPtr hBmpOld = IntPtr.Zero;
            try
            {
                hDCNew = CreateCompatibleDC(hDCOld);
                hBmpNew = CreateCompatibleBitmap(
                    hDCOld, size.Width, size.Height);
                hBmpOld = SelectObject(hDCNew, hBmpNew);

                // Draw the symbol to the new device context.
                bool lResult = DrawToDC(
                    hDCNew, size, pSymbol, lGap, backColor);

                hBmpNew = SelectObject(hDCNew, hBmpOld);
                DeleteDC(hDCNew);

                // Return the Bitmap as an OLE Picture.
                return CreatePictureFromBitmap(hBmpNew);
            }
            catch (Exception error)
            {
                if (pSymbol != null)
                {
                    pSymbol.ResetDC();
                    if ((hBmpNew != IntPtr.Zero) && (hDCNew != IntPtr.Zero)
                        && (hBmpOld != IntPtr.Zero))
                    {
                        hBmpNew = SelectObject(hDCNew, hBmpOld);
                        DeleteDC(hDCNew);
                    }
                }

                throw error;
            }
        }
        private static IPictureDisp CreatePictureFromBitmap(IntPtr hBmpNew)
        {
            try
            {
                Guid iidIPicture =
                    new Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB");

                PICTDESC picDesc = new PICTDESC();
                picDesc.cbSizeOfStruct = Marshal.SizeOf(picDesc);
                picDesc.picType = (int)PictureTypeConstants.picTypeBitmap;
                picDesc.hPic = (IntPtr)hBmpNew;
                picDesc.hpal = IntPtr.Zero;

                // Create Picture object.
                IPictureDisp newPic;
                OleCreatePictureIndirect(
                    ref picDesc, ref iidIPicture, true, out newPic);

                // Return the new Picture object.
                return newPic;
            }
            catch (Exception error)
            {
                throw error;
            }
        }
        private static bool DrawToWnd(IntPtr hWnd, ISymbol pSymbol,
            int lGap, int backColor)
        {
            IntPtr hDC = IntPtr.Zero;
            try
            {
                if (hWnd != IntPtr.Zero)
                {
                    // Calculate size of window.
                    RECT udtRect = new RECT();
                    int lResult = GetClientRect(hWnd, ref udtRect);

                    if (lResult != 0)
                    {
                        int lWidth = (udtRect.Right - udtRect.Left);
                        int lHeight = (udtRect.Bottom - udtRect.Top);

                        hDC = GetDC(hWnd);
                        // Must release the DC afterwards.
                        if (hDC != IntPtr.Zero)
                        {
                            bool ok = DrawToDC(hDC, new Size(lWidth, lHeight)
                            , pSymbol, lGap, backColor);

                            // Release cached DC obtained with GetDC.
                            ReleaseDC(hWnd, hDC);

                            return ok;
                        }
                    }
                }
            }
            catch
            {
                if (pSymbol != null)
                {
                    // Try resetting DC
                    pSymbol.ResetDC();

                    if ((hWnd != IntPtr.Zero) && (hDC != IntPtr.Zero))
                    {
                        ReleaseDC(hWnd, hDC); // Try to release cached DC
                    }
                }
                return false;
            }
            return true;
        }
        private static bool DrawToDC(IntPtr hDC, Size size,
            ISymbol pSymbol, int lGap, int backColor)
        {
            try
            {
                if (hDC != IntPtr.Zero)
                {
                    // First clear the existing device context.
                    if (!Clear(hDC, backColor, 0, 0,
                        size.Width, size.Height))
                    {
                        throw new Exception(
                            "Could not clear the Device Context.");
                    }

                    // Create the Transformation and Geometry
                    // required by ISymbol::Draw.
                    ITransformation pTransformation = CreateTransFromDC(
                        hDC, size.Width, size.Height);
                    IEnvelope pEnvelope = new EnvelopeClass();
                    pEnvelope.PutCoords(lGap, lGap, size.Width - lGap,
                        size.Height - lGap);
                    IGeometry pGeom = CreateSymShape(pSymbol, pEnvelope);

                    // Perform the Draw operation.
                    if ((pTransformation != null) && (pGeom != null))
                    {
                        pSymbol.SetupDC(hDC.ToInt32(), pTransformation);
                        pSymbol.Draw(pGeom);
                        pSymbol.ResetDC();
                    }
                    else
                    {
                        throw new Exception("Could not create required" +
                            "Transformation or Geometry.");
                    }
                }
            }
            catch
            {
                if (pSymbol != null)
                {
                    pSymbol.ResetDC();
                }
                return false;
            }

            return true;
        }
        private static bool Clear(IntPtr hDC, int backgroundColor,
            int xmin, int ymin, int xmax, int ymax)
        {
            IntPtr hBrushBackground = IntPtr.Zero;
            int lResult;
            bool ok;

            try
            {
                RECT udtBounds;
                udtBounds.Left = xmin;
                udtBounds.Top = ymin;
                udtBounds.Right = xmax;
                udtBounds.Bottom = ymax;

                hBrushBackground = CreateSolidBrush(backgroundColor);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not create GDI Brush.");
                }
                lResult = FillRect(hDC, ref udtBounds, hBrushBackground);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not fill Device Context.");
                }
                ok = DeleteObject(hBrushBackground);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not delete GDI Brush.");
                }
            }
            catch
            {
                if (hBrushBackground != IntPtr.Zero)
                {
                    ok = DeleteObject(hBrushBackground);
                }
                return false;
            }

            return true;
        }
        private static ITransformation CreateTransFromDC(IntPtr hDC,
            int lWidth, int lHeight)
        {
            // Calculate the parameters for the new transformation,
            // based on the dimensions passed to this function.
            try
            {
                IEnvelope pBoundsEnvelope = new EnvelopeClass();
                pBoundsEnvelope.PutCoords(0.0, 0.0, (double)lWidth,
                    (double)lHeight);

                tagRECT deviceRect;
                deviceRect.left = 0;
                deviceRect.top = 0;
                deviceRect.right = lWidth;
                deviceRect.bottom = lHeight;

                int dpi = GetDeviceCaps(hDC, LOGPIXELSY);
                if (dpi == 0)
                {
                    throw new Exception(
                      "Could not retrieve Resolution from device context.");
                }

                // Create a new display transformation and set its properties
                IDisplayTransformation newTrans =
                    new DisplayTransformationClass();
                newTrans.VisibleBounds = pBoundsEnvelope;
                newTrans.Bounds = pBoundsEnvelope;
                newTrans.set_DeviceFrame(ref deviceRect);
                newTrans.Resolution = dpi;

                return newTrans;
            }
            catch
            {
                return null;
            }
        }
        private static IGeometry CreateSymShape(ISymbol pSymbol,
            IEnvelope pEnvelope)
        {

            try
            {
                if (pSymbol is IMarkerSymbol)
                {
                    // For a MarkerSymbol return a Point.
                    IArea pArea = (IArea)pEnvelope;
                    return pArea.Centroid;
                }
                else if ((pSymbol is ILineSymbol) ||
                    (pSymbol is ITextSymbol))
                {
                    // For a LineSymbol or TextSymbol return a Polyline.
                    IPolyline pPolyline = new PolylineClass();
                    //diagonal line default
                    //pPolyline.FromPoint = pEnvelope.LowerLeft;
                    //pPolyline.ToPoint = pEnvelope.UpperRight;

                    //straight line like templates
                    IPoint fromPoint = new PointClass();
                    fromPoint.X = pEnvelope.LowerLeft.X;
                    fromPoint.Y = pEnvelope.LowerLeft.Y + (pEnvelope.Height / 2);
                    IPoint toPoint = new PointClass();
                    toPoint.X = pEnvelope.LowerRight.X;
                    toPoint.Y = pEnvelope.LowerRight.Y + (pEnvelope.Height / 2);
                    pPolyline.FromPoint = fromPoint;
                    pPolyline.ToPoint = toPoint;
                    return pPolyline;
                }
                else
                {
                    // For any FillSymbol return an Envelope.
                    return pEnvelope;
                }
            }
            catch
            {
                return null;
            }
        }
        public static Bitmap SymbolToBitmap(ISymbol userSymbol, Size size,
            Graphics gr, int backColor)
        {
            IntPtr graphicsHdc = gr.GetHdc();
            IntPtr hBitmap = IntPtr.Zero;
            IPictureDisp newPic = CreatePictureFromSymbol(
                graphicsHdc, ref hBitmap, userSymbol, size, 1, backColor);
            Bitmap newBitmap = Bitmap.FromHbitmap(hBitmap);
            gr.ReleaseHdc(graphicsHdc);

            return newBitmap;
        }
    }
    #endregion //WindowsAPI calls

    public class Localizer
    {

        ResourceManager manager;

        static Localizer localizer = new Localizer();

        private Localizer()
        {
            //Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
            // System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            //Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()); 

            manager = new ResourceManager("A4LGSharedFunctions.UserMessages", this.GetType().Assembly);

        }

        public static string GetString(string id)
        {

            string ret = localizer.manager.GetString(id);

            if (ret == null)

                throw new Exception(string.Format("The localized string for {0} is not found", id));

            return ret;

        }

        //Example using different resource files
        //string msg  = A4LGSharedFunctions.Localizer.GetString("Test");

        //   System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("es");
        //   System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("es");
        //   msg = A4LGSharedFunctions.Localizer.GetString("Test");

    }



    public static class Globals
    {
        public static IGroupLayer AddGNResultClasses(IGeometricNetwork geomNetwork, IApplication app, string ID, System.DateTime dateTimeValue, string IDFieldName, string DateFieldName, out string suffix, bool addAllLayers, bool removeMZ)
        {
            IEnumFeatureClass enumFC = null;
            IFeatureClass fc = null;
            IGroupLayer groupLayer = null;
            List<IGroupLayer> groupLayers = null;
            IDataset pDataset = null;
            IMap map = null;
            suffix = "";
            try
            {
                string gpLayerName = A4LGSharedFunctions.Localizer.GetString("traceLayerName");
                string fcName = null;
                string fcNameClass = null;
                map = ((IMxDocument)app.Document).FocusMap;

                groupLayers = Globals.FindGroupLayers(map, gpLayerName);
                int idx = 0;
                if (groupLayers == null)
                {
                    idx = idx + 1;
                    groupLayer = new GroupLayerClass();
                    groupLayer.Name = gpLayerName + " " + idx;
                    map.AddLayer(groupLayer);
                }
                else if (groupLayers.Count == 0)
                {
                    idx = idx + 1;
                    groupLayer = new GroupLayerClass();
                    groupLayer.Name = gpLayerName + " " + idx;
                    map.AddLayer(groupLayer);
                }
                else
                {
                    foreach (IGroupLayer gpLay in groupLayers)
                    {
                        string strStrip = gpLay.Name.Replace(gpLayerName, "");
                        int tmpIdx = 0;
                        if (Int32.TryParse(strStrip, out tmpIdx))
                        {
                            if (tmpIdx > idx)
                            {
                                idx = tmpIdx;
                            }
                        }


                    }
                    groupLayer = new GroupLayerClass();
                    idx = idx + 1;
                    groupLayer.Name = gpLayerName + " " + idx;
                    map.AddLayer(groupLayer);
                }
                Globals.FlagsBarriersToLayer(app, map, groupLayer, ID, dateTimeValue, IDFieldName, DateFieldName, idx);

                suffix = " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + idx;

                for (int i = 0; i < 4; i++)
                {
                    if (i == 0)
                    {
                        enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexJunction);
                    }
                    else if (i == 1)
                    {
                        enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                    }
                    else if (i == 2)
                    {
                        enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
                    }
                    else if (i == 3)
                    {
                        enumFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
                    }
                    fc = enumFC.Next();
                    while (fc != null)
                    {
                        if (geomNetwork.OrphanJunctionFeatureClass.FeatureClassID != fc.ObjectClassID)
                        {
                            if (addAllLayers)
                            {
                                pDataset = (IDataset)fc;
                                fcNameClass = Globals.getClassName(pDataset) + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix");
                                fcName = fc.AliasName + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + idx;
                                copyClassToInMemory(fc, fcNameClass, fcName, map, groupLayer, false, IDFieldName, DateFieldName, removeMZ);
                            }
                            else
                            {
                                if (Globals.FindLayerByClassID(map, fc.ObjectClassID.ToString()) != null)
                                {
                                    pDataset = (IDataset)fc;
                                    fcNameClass = Globals.getClassName(pDataset) + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix");
                                    fcName = fc.AliasName + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + idx;
                                    copyClassToInMemory(fc, fcNameClass, fcName, map, groupLayer, false, IDFieldName, DateFieldName, removeMZ);
                                }
                            }


                        }
                        fc = enumFC.Next();

                    }
                }
                Globals.CreateOutageArea(map, groupLayer, ID, IDFieldName, DateFieldName, idx);

                return groupLayer;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                enumFC = null;
                fc = null;

                groupLayers = null;
                pDataset = null;
                map = null;
            }
        }

        public static IFeatureLayer copyClassToInMemory(IFeatureClass sourceClass, string className, string layerName, IMap map, IGroupLayer groupLayer, bool lookForLayer, string IDFieldName, string DateFieldName, bool removeMZ)
        {
            ILayer pLay = null;
            IFields pFlds = null;
            IFeatureClass pFC = null;
            IDataset pDataset = null;
            IFeatureLayer pFL = null;
            IWorkspace pWS = null;
            IFeatureLayer mapLayer = null;
            try
            {
                if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                {
                    pWS = Globals.CreateInMemoryWorkspace();
                }
                pDataset = (IDataset)sourceClass;

                if (lookForLayer)
                {
                    pLay = Globals.FindLayerInWorkspace(map, className, pWS);
                }
                if (pLay == null)
                {
                    pFlds = Globals.copyFields(sourceClass.Fields, sourceClass.LengthField, sourceClass.AreaField, IDFieldName, DateFieldName, removeMZ);
                    pFC = Globals.createFeatureClassInMemory(className, pFlds, pWS, sourceClass.FeatureType);
                    if (pFC != null)
                    {
                        pFL = new FeatureLayerClass();
                        pFL.FeatureClass = pFC;
                        pFL.Name = layerName;
                        if (groupLayer != null)
                        {
                            groupLayer.Add(pFL);
                        }
                        else
                        {
                            map.AddLayer(pFL);
                        }
                    }
                }
                else
                {
                    pFL = pLay as IFeatureLayer;
                }
                if (pFL.FeatureClass == null)
                {
                    pFlds = Globals.copyFields(sourceClass.Fields, sourceClass.LengthField, sourceClass.AreaField, IDFieldName, DateFieldName, removeMZ);
                    pFL.FeatureClass = Globals.createFeatureClassInMemory(className, pFlds, pWS, sourceClass.FeatureType);

                }
                mapLayer = FindLayerByFeatureClass(map, sourceClass, false);
                if (mapLayer != null)
                {
                    copyLayerFromLayer(mapLayer, pFL);
                }
                return pFL;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                mapLayer = null;
                pLay = null;
                pFlds = null;
                pFC = null;
                pDataset = null;

                pWS = null;
            }
        }
        public static IEnvelope TraceResultsToLayer(ref IApplication app, ref  IGeometricNetwork gn,
                                                   ref IEnumEIDInfo enumEidInfoJunc, ref IEnumEIDInfo enumEidInfoEdge,
                                                   ref Hashtable valveHT, ref List<IFeatureLayer> valvesFLayer)
        {

            IEnvelope env = null;
            //IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;

            IGeometry geom = null;
            IFields pFlds = null;
            IFeatureClass pFC = null;
            IFeatureClass pSourceFC = null;
            IFeature pSourceFeat = null;
            IFeatureLayer pFL = null;
            copyFeatureToInMem copFeat = null;
            geoFeat geoFet = null;
            IFeatureCursor pCursor = null;
            IFeatureBuffer pFBuf = null;
            IFeature pFeat = null;
            IField pFld = null;
            ILayer pLay = null;
            ITopologicalOperator topologicalOperator = null;
            IWorkspace pWS = null;

            IEnumGeometry enumGeometry = null;
            IPolyline pline = null;
            ICompositeLayer pCompLayer = null;
            IGroupLayer pGrpLay = null;
            IFeatureSelection pFS = null;
            IGeometryCollection pGeometryCollection = null;
            IPointCollection pPntCollection = null;
            IGeometry pGeo = null;
            IMap map = null;


            bool re;
            System.DateTime dateTimeValue;
            try
            {


                pGeometryCollection = new MultipointClass();

                object Missing = Type.Missing;
                Dictionary<string, copyFeatureToInMem> copyFeats = null;
                IDictionaryEnumerator eidEnum = null;
                int newFldIdx = -1;
                int sourceFldIdx = -1;
                string ID = Globals.generateRandomID(10);
                map = ((IMxDocument)app.Document).FocusMap;

                string IDFieldName = ConfigUtil.GetConfigValue("Trace_ResultLayersIDField", "TRACEID");
                string DateFieldName = ConfigUtil.GetConfigValue("Trace_ResultLayersDateTimeField", "MODELRUNAT");
                string dateFieldOption = ConfigUtil.GetConfigValue("Trace_ResultLayersDateTimeZone", "LOCAL");
                bool removeMZ = ConfigUtil.GetConfigValue("Trace_ResultLayersRemoveMZ", "FALSE").ToUpper() == "FALSE" ? false : true;

                if (dateFieldOption.ToUpper() == "UTC")
                {
                    dateTimeValue = DateTime.UtcNow; //DateTime.Now;

                }
                else
                {
                    dateTimeValue = DateTime.Now;

                }

                bool boolAddAllResultLayers = ConfigUtil.GetConfigValue("Trace_ResultAddAllLayers", "false").ToLower() == "false" ? false : true;

                double bufferAmt = ConfigUtil.GetConfigValue("Trace_ResultBuffer", 25.0);
                string suffix;
                try
                {
                    env = new EnvelopeClass();




                    List<string> valveFCs = new List<string>();


                    //map.ClearSelection();
                    pGrpLay = Globals.AddGNResultClasses(gn, app, ID, dateTimeValue, IDFieldName, DateFieldName, out suffix, boolAddAllResultLayers, removeMZ);
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(((app.Document as IMxDocument).FocusMap))) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pCompLayer = (ICompositeLayer)pGrpLay;

                    copyFeats = new Dictionary<string, copyFeatureToInMem>();

                    for (int f = 0; f < 3; f++)
                    {
                        if (f == 0)
                        {
                            eidEnum = valveHT.GetEnumerator();

                            eidEnum.Reset();
                            re = eidEnum.MoveNext();

                            if (re == false)
                            {
                                eidInfo = null;
                            }
                            else
                            {
                                eidInfo = ((DictionaryEntry)eidEnum.Current).Value as IEIDInfo;

                            }
                        }
                        else if (f == 1)
                        {
                            enumEidInfo = enumEidInfoJunc;

                            enumEidInfo.Reset();
                            eidInfo = enumEidInfo.Next();

                        }
                        else
                        {
                            //enumEidInfo = eidHelper.CreateEnumEIDInfo(edgeEIDs);
                            enumEidInfo = enumEidInfoEdge;
                            enumEidInfo.Reset();
                            eidInfo = enumEidInfo.Next();
                        }
                        bool cont = true;
                        while (eidInfo != null)
                        {
                            cont = true;
                            pSourceFC = ((IFeatureClass)eidInfo.Feature.Class);

                            if (f == 0)
                            {
                                if (valveFCs.Contains(pSourceFC.ObjectClassID.ToString()) == false)
                                {
                                    valveFCs.Add(pSourceFC.ObjectClassID.ToString());
                                }

                                //pSourceFC.CLSID;
                            }
                            else
                            {
                                if (valveFCs.Contains(pSourceFC.ObjectClassID.ToString()) == true)
                                {
                                    cont = false;
                                }
                                else
                                {
                                    cont = true;
                                }
                            }
                            if (cont)
                            {
                                string fcName = pSourceFC.AliasName + suffix;

                                if (!copyFeats.ContainsKey(fcName))
                                {
                                    pLay = Globals.FindLayerInGroup(pCompLayer, fcName);

                                    if (pLay == null && boolAddAllResultLayers)
                                    {
                                        pFlds = Globals.copyFields(eidInfo.Feature.Class.Fields, (eidInfo.Feature.Class as IFeatureClass).LengthField, (eidInfo.Feature.Class as IFeatureClass).AreaField, IDFieldName, DateFieldName, removeMZ);
                                        pFC = Globals.createFeatureClassInMemory(fcName, pFlds, pWS, pSourceFC.FeatureType);
                                        pFL = new FeatureLayerClass();
                                        pFL.FeatureClass = pFC;
                                        pFL.Name = fcName;
                                        pGrpLay.Add(pFL);
                                    }
                                    else if (pLay == null && boolAddAllResultLayers == false)
                                    {
                                        if (Globals.FindLayerByClassID(map, pSourceFC.ObjectClassID.ToString()) != null)
                                        {
                                            pFlds = Globals.copyFields(eidInfo.Feature.Class.Fields, (eidInfo.Feature.Class as IFeatureClass).LengthField, (eidInfo.Feature.Class as IFeatureClass).AreaField, IDFieldName, DateFieldName, removeMZ);
                                            pFC = Globals.createFeatureClassInMemory(fcName, pFlds, pWS, pSourceFC.FeatureType);
                                            pFL = new FeatureLayerClass();
                                            pFL.FeatureClass = pFC;
                                            pFL.Name = fcName;
                                            pGrpLay.Add(pFL);
                                        }
                                        else
                                        {
                                            if (f == 0)
                                            {
                                                re = eidEnum.MoveNext();
                                                if (re == false)
                                                {
                                                    eidInfo = null;
                                                }
                                                else
                                                {
                                                    eidInfo = ((DictionaryEntry)eidEnum.Current).Value as IEIDInfo;

                                                }



                                            }
                                            else
                                            {
                                                eidInfo = enumEidInfo.Next();
                                            }
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        pFL = pLay as IFeatureLayer;
                                    }
                                    if (pFL.FeatureClass == null)
                                    {
                                        pFlds = Globals.copyFields(eidInfo.Feature.Class.Fields, (eidInfo.Feature.Class as IFeatureClass).LengthField, (eidInfo.Feature.Class as IFeatureClass).AreaField, IDFieldName, DateFieldName, removeMZ);
                                        pFL.FeatureClass = Globals.createFeatureClassInMemory(fcName, pFlds, pWS, pSourceFC.FeatureType);

                                    }
                                    else
                                    {
                                        Globals.deleteFeatures(pFL);
                                    }
                                    copFeat = new copyFeatureToInMem();
                                    copFeat.OIDSGeo = new Dictionary<int, geoFeat>();
                                    copFeat.FeatureLayer = pFL;
                                    copyFeats.Add(fcName, copFeat);
                                }
                                else
                                {
                                    copFeat = copyFeats[fcName];
                                }
                                if (!copFeat.OIDSGeo.ContainsKey(eidInfo.Feature.OID))
                                {
                                    geoFet = new geoFeat();
                                    pSourceFeat = pSourceFC.GetFeature(eidInfo.Feature.OID);

                                    geoFet.feature = pSourceFeat;
                                    geoFet.geo = new GeometryBagClass();
                                    geom = eidInfo.Geometry;
                                    if (geom == null)
                                    {
                                        geom = pSourceFeat.ShapeCopy;
                                    }


                                    geoFet.geo.AddGeometry(copyGeometry(geom, removeMZ), ref Missing, ref Missing);

                                    copFeat.OIDSGeo.Add(eidInfo.Feature.OID, geoFet);
                                    env.Union(geom.Envelope);
                                }
                                else
                                {
                                    geoFet = copFeat.OIDSGeo[eidInfo.Feature.OID];
                                    geom = eidInfo.Geometry;
                                    if (geom == null)
                                    {
                                        geom = pSourceFeat.ShapeCopy;
                                    }
                                    geoFet.geo.AddGeometry(copyGeometry(geom, removeMZ), ref Missing, ref Missing);
                                    env.Union(geom.Envelope);
                                }




                            }
                            if (f == 0)
                            {
                                re = eidEnum.MoveNext();
                                if (re == false)
                                {
                                    eidInfo = null;
                                }
                                else
                                {
                                    eidInfo = ((DictionaryEntry)eidEnum.Current).Value as IEIDInfo;

                                }



                            }
                            else
                            {
                                eidInfo = enumEidInfo.Next();
                            }

                        }
                    }


                    foreach (KeyValuePair<string, copyFeatureToInMem> kvp in copyFeats)
                    {
                        pCursor = kvp.Value.FeatureLayer.FeatureClass.Insert(true);
                        foreach (KeyValuePair<int, geoFeat> oidpair in kvp.Value.OIDSGeo)
                        {

                            pFBuf = kvp.Value.FeatureLayer.FeatureClass.CreateFeatureBuffer();
                            pFeat = (IFeature)pFBuf;

                            if (oidpair.Value.geo.GeometryCount == 1)
                            {
                                pGeo = oidpair.Value.geo.get_Geometry(0);

                                pFeat.Shape = pGeo;
                                if (pGeo.GeometryType == esriGeometryType.esriGeometryPoint)
                                {
                                    pGeometryCollection.AddGeometry(pGeo);
                                }
                                else
                                {
                                    pPntCollection = (IPointCollection)pGeo;
                                    for (int p = 0; p < pPntCollection.PointCount; p++)
                                    {
                                        pGeometryCollection.AddGeometry(pPntCollection.get_Point(p));
                                    }
                                }

                            }
                            else
                            {
                                enumGeometry = oidpair.Value.geo as IEnumGeometry;
                                topologicalOperator = new PolylineClass();
                                topologicalOperator.ConstructUnion(enumGeometry);

                                pline = (IPolyline)topologicalOperator;
                                pFeat.Shape = pline;
                                pPntCollection = (IPointCollection)pline;
                                for (int p = 0; p < pPntCollection.PointCount; p++)
                                {
                                    pGeometryCollection.AddGeometry(pPntCollection.get_Point(p));
                                }

                            }
                            for (int i = 0; i < kvp.Value.FeatureLayer.FeatureClass.Fields.FieldCount; i++)
                            {
                                pFld = kvp.Value.FeatureLayer.FeatureClass.Fields.get_Field(i);

                                if (pFld.Type != esriFieldType.esriFieldTypeGeometry &&
                                    pFld.Type != esriFieldType.esriFieldTypeOID &&
                                    pFld != kvp.Value.FeatureLayer.FeatureClass.AreaField &&
                                    pFld != kvp.Value.FeatureLayer.FeatureClass.LengthField)
                                {
                                    newFldIdx = pFBuf.Fields.FindField(pFld.Name);
                                    sourceFldIdx = oidpair.Value.feature.Fields.FindField(pFld.Name);
                                    if (newFldIdx >= 0 && sourceFldIdx >= 0)
                                    {
                                        try
                                        {
                                            pFBuf.set_Value(newFldIdx, oidpair.Value.feature.get_Value(sourceFldIdx));
                                            if (newFldIdx != sourceFldIdx)
                                            {
                                                Console.WriteLine(newFldIdx);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(pFld.Name + " " + ex.Message);
                                        }
                                    }
                                }

                            }
                            newFldIdx = pFBuf.Fields.FindField(IDFieldName);
                            if (newFldIdx >= 0)
                            {
                                pFBuf.set_Value(newFldIdx, ID);
                            }
                            newFldIdx = pFBuf.Fields.FindField(DateFieldName);
                            if (newFldIdx >= 0)
                            {
                                pFBuf.set_Value(newFldIdx, dateTimeValue);
                            }
                            pCursor.InsertFeature(pFBuf);

                        }
                        pCursor.Flush();

                        Marshal.ReleaseComObject(pCursor);

                        pFS = (IFeatureSelection)kvp.Value.FeatureLayer;
                        pFS.SelectFeatures(null, esriSelectionResultEnum.esriSelectionResultNew, false);

                    }
                    topologicalOperator = (ITopologicalOperator)pGeometryCollection;
                    pGeo = topologicalOperator.ConvexHull();
                    topologicalOperator = (ITopologicalOperator)pGeo;
                    pGeo = topologicalOperator.Buffer(bufferAmt);
                    topologicalOperator = (ITopologicalOperator)pGeo;
                    topologicalOperator.Simplify();

                    pGeo = (IGeometry)topologicalOperator;

                    pGeo.SpatialReference = ((IGeoDataset)gn.FeatureDataset).SpatialReference;

                    pLay = Globals.FindLayerInGroup(pCompLayer, A4LGSharedFunctions.Localizer.GetString("OutageAreaName") + suffix);
                    if (pLay != null)
                    {
                        pFL = (IFeatureLayer)pLay;
                        pCursor = pFL.FeatureClass.Insert(true);

                        pFBuf = pFL.FeatureClass.CreateFeatureBuffer();
                        pFeat = (IFeature)pFBuf;
                        pFeat.Shape = pGeo;
                        newFldIdx = pFBuf.Fields.FindField(IDFieldName);
                        if (newFldIdx >= 0)
                        {
                            pFBuf.set_Value(newFldIdx, ID);
                        }
                        newFldIdx = pFBuf.Fields.FindField(DateFieldName);
                        if (newFldIdx >= 0)
                        {
                            pFBuf.set_Value(newFldIdx, dateTimeValue);
                        }

                        pCursor.InsertFeature(pFBuf);


                        pCursor.Flush();

                        Marshal.ReleaseComObject(pCursor);


                    }


                    env.Expand(1.1, 1.1, true);
                    ((IMxDocument)app.Document).ActiveView.Refresh();
                    return env;

                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    env = null;
                    // eidHelper = null;
                    enumEidInfo = null;
                    eidInfo = null;
                    geom = null;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

                map = null;
                // eidHelper = null;
                enumEidInfo = null;
                enumEidInfoJunc = null;
                enumEidInfoEdge = null;

                eidInfo = null;
                geom = null;
                pFlds = null;
                pFC = null;
                pSourceFC = null;
                pSourceFeat = null;
                pFL = null;
                copFeat = null;
                geoFet = null;
                pCursor = null;
                pFBuf = null;
                pFeat = null;
                pFld = null;
                pLay = null;
                topologicalOperator = null;
                pWS = null;

                enumGeometry = null;
                pline = null;
                pCompLayer = null;
                pGrpLay = null;
                pFS = null;
                pGeometryCollection = null;
                pPntCollection = null;
                pGeo = null;
            }

        }


        //public static IEnvelope TraceResultsToLayerOrig(ref IMap map, ref  IGeometricNetwork gn,
        //                                            ref IEnumNetEID edgeEIDs, ref IEnumNetEID juncEIDs,
        //                                             ref Hashtable valveHT, ref List<IFeatureLayer> valvesFLayer)
        //{

        //    IEnvelope env = null;

        //    IEIDHelper eidHelper = null;
        //    IEnumEIDInfo enumEidInfo = null;
        //    IEIDInfo eidInfo = null;
        //    IGeometry geom = null;

        //    IFields pFlds = null;
        //    IFeatureClass pFC = null;
        //    IFeatureClass pSourceFC = null;
        //    IFeature pSourceFeat = null;
        //    IFeatureLayer pFL = null;
        //    copyFeatureToInMem copFeat = null;
        //    geoFeat geoFet = null;
        //    IFeatureCursor pCursor = null;
        //    IFeatureBuffer pFBuf = null;
        //    IFeature pFeat = null;
        //    IField pFld = null;
        //    ILayer pLay = null;
        //    ITopologicalOperator topologicalOperator = null;
        //    IWorkspace pWS = null;

        //    IEnumGeometry enumGeometry;
        //    IPolyline pline;

        //    bool fndAsFL = false;
        //    object Missing = Type.Missing;
        //    Dictionary<string, copyFeatureToInMem> copyFeats = null;
        //    IDictionaryEnumerator eidEnum = null;
        //    //IEIDInfo valveEidInfo = null;
        //    try
        //    {

        //        env = new EnvelopeClass();

        //        if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
        //        {
        //            pWS = Globals.CreateInMemoryWorkspace();
        //        }

        //        copyFeats = new Dictionary<string, copyFeatureToInMem>();

        //        eidHelper = new EIDHelperClass();
        //        eidHelper.GeometricNetwork = gn;
        //        //eidHelper.OutputSpatialReference = map.SpatialReference;
        //        eidHelper.ReturnFeatures = true;
        //        eidHelper.ReturnGeometries = true;
        //        eidHelper.PartialComplexEdgeGeometry = true;
        //        for (int f = 0; f < 3; f++)
        //        {
        //            if (f == 0)
        //            {
        //                enumEidInfo = eidHelper.CreateEnumEIDInfo(edgeEIDs);
        //                enumEidInfo.Reset();
        //                eidInfo = enumEidInfo.Next();

        //            }
        //            else if (f == 1)
        //            {
        //                enumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);
        //                enumEidInfo.Reset();
        //                eidInfo = enumEidInfo.Next();

        //            }
        //            else
        //            {
        //                eidEnum = valveHT.GetEnumerator();

        //                eidEnum.Reset();
        //                eidEnum.MoveNext();
        //                eidInfo = ((DictionaryEntry)eidEnum.Current).Value as IEIDInfo;

        //            }




        //            while (eidInfo != null)
        //            {

        //                pSourceFC = ((IFeatureClass)eidInfo.Feature.Class);
        //                string fcName = pSourceFC.AliasName + " " + A4LGSharedFunctions.Localizer.GetString("traceLayerName");

        //                if (!copyFeats.ContainsKey(fcName))
        //                {
        //                    pLay = Globals.FindLayer(map, fcName, ref fndAsFL);

        //                    if (pLay == null)
        //                    {
        //                        pFlds = Globals.copyFields(eidInfo.Feature.Class.Fields, (eidInfo.Feature.Class as IFeatureClass).LengthField, (eidInfo.Feature.Class as IFeatureClass).AreaField, IDFieldName);
        //                        pFC = Globals.createFeatureClassInMemory(fcName, pFlds, pWS, pSourceFC.FeatureType);
        //                        pFL = new FeatureLayerClass();
        //                        pFL.FeatureClass = pFC;
        //                        pFL.Name = fcName;
        //                        map.AddLayer(pFL);
        //                    }
        //                    else
        //                    {
        //                        pFL = pLay as IFeatureLayer;
        //                    }
        //                    if (pFL.FeatureClass == null)
        //                    {
        //                        pFlds = Globals.copyFields(eidInfo.Feature.Class.Fields, (eidInfo.Feature.Class as IFeatureClass).LengthField, (eidInfo.Feature.Class as IFeatureClass).AreaField, IDFieldName);
        //                        pFL.FeatureClass = Globals.createFeatureClassInMemory(fcName, pFlds, pWS, pSourceFC.FeatureType);

        //                    }
        //                    else
        //                    {
        //                        Globals.deleteFeatures(pFL);
        //                    }
        //                    copFeat = new copyFeatureToInMem();
        //                    copFeat.OIDSGeo = new Dictionary<int, geoFeat>();
        //                    copFeat.FeatureLayer = pFL;
        //                    copyFeats.Add(fcName, copFeat);
        //                }
        //                else
        //                {
        //                    copFeat = copyFeats[fcName];
        //                }
        //                if (!copFeat.OIDSGeo.ContainsKey(eidInfo.Feature.OID))
        //                {
        //                    geoFet = new geoFeat();
        //                    pSourceFeat = pSourceFC.GetFeature(eidInfo.Feature.OID);

        //                    geoFet.feature = pSourceFeat;
        //                    geoFet.geo = new GeometryBagClass();
        //                    geom = eidInfo.Geometry;
        //                    if (geom == null)
        //                    {
        //                        geom = pSourceFeat.ShapeCopy;
        //                    }
        //                    geoFet.geo.AddGeometry(copyGeometry(geom), ref Missing, ref Missing);

        //                    copFeat.OIDSGeo.Add(eidInfo.Feature.OID, geoFet);
        //                    env.Union(geom.Envelope);
        //                }
        //                else
        //                {
        //                    geoFet = copFeat.OIDSGeo[eidInfo.Feature.OID];
        //                    geom = eidInfo.Geometry;
        //                    if (geom == null)
        //                    {
        //                        geom = pSourceFeat.ShapeCopy;
        //                    }
        //                    geoFet.geo.AddGeometry(copyGeometry(geom), ref Missing, ref Missing);
        //                    env.Union(geom.Envelope);
        //                }



        //                if (f == 2)
        //                {
        //                    bool re = eidEnum.MoveNext();
        //                    if (re == false)
        //                    {
        //                        eidInfo = null;
        //                    }
        //                    else
        //                    {
        //                        eidInfo = ((DictionaryEntry)eidEnum.Current).Value as IEIDInfo;

        //                    }



        //                }
        //                else
        //                {
        //                    eidInfo = enumEidInfo.Next();
        //                }

        //            }

        //        }


        //        //}

        //        foreach (KeyValuePair<string, copyFeatureToInMem> kvp in copyFeats)
        //        {
        //            //kvp.Value.FeatureClass
        //            pCursor = kvp.Value.FeatureLayer.FeatureClass.Insert(true);
        //            foreach (KeyValuePair<int, geoFeat> oidpair in kvp.Value.OIDSGeo)
        //            {

        //                pFBuf = kvp.Value.FeatureLayer.FeatureClass.CreateFeatureBuffer();
        //                pFeat = (IFeature)pFBuf;

        //                if (oidpair.Value.geo.GeometryCount == 1)
        //                {
        //                    pFeat.Shape = oidpair.Value.geo.get_Geometry(0);
        //                }
        //                else
        //                {
        //                    enumGeometry = oidpair.Value.geo as IEnumGeometry;
        //                    topologicalOperator = new PolylineClass();
        //                    topologicalOperator.ConstructUnion(enumGeometry);
        //                    //topologicalOperator.Simplify();

        //                    pline = (IPolyline)topologicalOperator;
        //                    pFeat.Shape = pline;
        //                }
        //                for (int i = 0; i < kvp.Value.FeatureLayer.FeatureClass.Fields.FieldCount; i++)
        //                {
        //                    pFld = kvp.Value.FeatureLayer.FeatureClass.Fields.get_Field(i);

        //                    if (pFld.Type != esriFieldType.esriFieldTypeGlobalID &&
        //                       pFld.Type != esriFieldType.esriFieldTypeGeometry &&
        //                        pFld.Type != esriFieldType.esriFieldTypeOID)
        //                    {
        //                        int newFldIdx = pFBuf.Fields.FindField(pFld.Name);

        //                        if (newFldIdx >= 0)
        //                        {
        //                            pFBuf.set_Value(newFldIdx, oidpair.Value.feature.get_Value(i));
        //                        }
        //                    }
        //                }
        //                pCursor.InsertFeature(pFBuf);

        //            }
        //            pCursor.Flush();
        //            Marshal.ReleaseComObject(pCursor);


        //        }

        //        env.Expand(1.1, 1.1, true);

        //        return env;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        env = null;
        //        eidHelper = null;
        //        enumEidInfo = null;
        //        eidInfo = null;
        //        geom = null;

        //    }

        //}


        public static void deleteFeatures(IFeatureLayer featLayer)
        {
            IFeatureCursor pCursor = null;
            IFeatureClass pFC = null;
            IWorkspace work = null;
            IWorkspaceEdit workEdit = null;
            IFeature feature = null;
            try
            {

                work = ((IDataset)pFC).Workspace;

                workEdit = work as IWorkspaceEdit;
                try
                {
                    //if (!workEdit.IsBeingEdited())
                    //{
                    //    workEdit.StartEditing(false);
                    //}

                    //workEdit.StartEditOperation();

                    pCursor = pFC.Update(null, false);
                    feature = null;
                    while ((feature = pCursor.NextFeature()) != null)
                    {
                        pCursor.DeleteFeature();
                    }
                    // workEdit.StopEditOperation();

                    Marshal.ReleaseComObject(pCursor);

                }
                catch (Exception ex)
                { }
                pFC = featLayer.FeatureClass;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                pCursor = null;
                pFC = null;
                work = null;
                workEdit = null;
                feature = null;
            }

        }
        public static void FlagsBarriersToLayer(IApplication app, IMap map, IGroupLayer pGrpLay, string ID, System.DateTime dateTimeValue, string IDFieldName, string DateFieldName, int suffCount)
        {

            bool fndAsFL = true;
            ILayer pFLayer = null;
            ILayer pBLayer = null;
            ICompositeLayer pCompLay = null;
            List<ESRI.ArcGIS.Geometry.IPoint> Flags = null;
            List<ESRI.ArcGIS.Geometry.IPoint> Barriers = null;
            IWorkspace pWS = null;
            IFields pFields = null;
            IPoint pNPt = null;
            IWorkspace work = null;
            IWorkspaceEdit workEdit = null;
            IFeatureCursor pCursor = null;
            IFeatureBuffer pFBuf = null;
            IFeature pFeat = null;
            IFeature feature = null;
            IFeatureClass pFlagsFC = null;
            IFeatureClass pBarriersFC = null;
            IFeatureLayer pFlagsLayer = null;
            IFeatureLayer pBarriersLayer = null;
            ISimpleRenderer pSimpleRend = null;
            IGeoFeatureLayer pGeoFeatLay = null;

            try
            {
                // Open the Workspace
                bool editStarted = false;

                if (pGrpLay != null)
                {
                    pCompLay = (ICompositeLayer)pGrpLay;
                    pFLayer = Globals.FindLayerInGroup(pCompLay, A4LGSharedFunctions.Localizer.GetString("ExportFlagsName") + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString());
                    pBLayer = Globals.FindLayerInGroup(pCompLay, A4LGSharedFunctions.Localizer.GetString("ExportBarriersName") + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString());

                }
                else
                {
                    pFLayer = Globals.FindLayer(map, A4LGSharedFunctions.Localizer.GetString("ExportFlagsName") + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString(), ref fndAsFL);
                    pBLayer = Globals.FindLayer(map, A4LGSharedFunctions.Localizer.GetString("ExportBarriersName") + " " + A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString(), ref fndAsFL);

                }

                pFields = Globals.createFeatureClassFields(map.SpatialReference, esriGeometryType.esriGeometryPoint, IDFieldName, DateFieldName);
                Globals.getFlagsBarriers(app, out Flags, out Barriers);

                if (pFLayer == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pFlagsFC = Globals.createFeatureClassInMemory(A4LGSharedFunctions.Localizer.GetString("ExportFlagsName") +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pFlagsLayer = new FeatureLayerClass();
                    pFlagsLayer.FeatureClass = pFlagsFC;
                    pFlagsLayer.Name = A4LGSharedFunctions.Localizer.GetString("ExportFlagsName") + " " +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString();
                    if (pGrpLay != null)
                    {
                        pGrpLay.Add(pFlagsLayer);
                    }
                    else
                    {
                        map.AddLayer(pFlagsLayer);

                    }
                }
                else
                {
                    pFlagsLayer = pFLayer as IFeatureLayer;
                }
                if (pFlagsLayer.FeatureClass == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pFlagsFC = Globals.createFeatureClassInMemory(A4LGSharedFunctions.Localizer.GetString("ExportFlagsName") +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pFlagsLayer = pFLayer as IFeatureLayer;
                    pFlagsLayer.FeatureClass = pFlagsFC;
                }


                pSimpleRend = new SimpleRendererClass();
                pSimpleRend.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeFlag) as ISymbol;

                pGeoFeatLay = pFlagsLayer as IGeoFeatureLayer;
                pGeoFeatLay.Renderer = pSimpleRend as IFeatureRenderer;

                pFlagsFC = pFlagsLayer.FeatureClass;
                work = ((IDataset)pFlagsFC).Workspace;

                workEdit = work as IWorkspaceEdit;
                int intIdField;
                int intDateField;
                try
                {
                    editStarted = false;
                    if (!workEdit.IsBeingEdited())
                    {
                        workEdit.StartEditing(false);
                        editStarted = true;
                    }

                    workEdit.StartEditOperation();

                    pCursor = pFlagsFC.Update(null, false);
                    feature = null;
                    while ((feature = pCursor.NextFeature()) != null)
                    {
                        pCursor.DeleteFeature();
                    }
                    workEdit.StopEditOperation();

                    Marshal.ReleaseComObject(pCursor);
                    workEdit.StartEditOperation();

                    pCursor = pFlagsFC.Insert(true);
                    intIdField = pFlagsFC.Fields.FindField(IDFieldName);
                    intDateField = pFlagsFC.Fields.FindField(DateFieldName);
                    foreach (ESRI.ArcGIS.Geometry.IPoint pnt in Flags) // Loop through List with foreach
                    {
                        pFBuf = pFlagsFC.CreateFeatureBuffer();
                        pFeat = (IFeature)pFBuf;
                        pNPt = new ESRI.ArcGIS.Geometry.PointClass();
                        pNPt.X = pnt.X;
                        pNPt.Y = pnt.Y;
                        pNPt.SpatialReference = pnt.SpatialReference;


                        pNPt.Project(((IGeoDataset)pFlagsFC).SpatialReference);

                        pFeat.Shape = pNPt;
                        if (intIdField >= 0)
                        {
                            pFeat.set_Value(intIdField, ID);
                        }

                        if (intDateField >= 0)
                        {
                            pFBuf.set_Value(intDateField, dateTimeValue);
                        }
                        pCursor.InsertFeature(pFBuf);

                    }
                    pCursor.Flush();
                    Marshal.ReleaseComObject(pCursor);
                    workEdit.StopEditOperation();

                    if (editStarted)
                    {
                        workEdit.StopEditing(true);
                        editStarted = false;
                    }
                }

                catch (COMException comExc)
                {
                    workEdit.AbortEditOperation();
                    if (editStarted)
                    {
                        workEdit.StopEditing(false);
                        editStarted = false;
                    }

                }

                if (pBLayer == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pBarriersFC = Globals.createFeatureClassInMemory(A4LGSharedFunctions.Localizer.GetString("ExportBarriersName") +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pBarriersLayer = new FeatureLayerClass();
                    pBarriersLayer.FeatureClass = pBarriersFC;
                    pBarriersLayer.Name = A4LGSharedFunctions.Localizer.GetString("ExportBarriersName") + " " +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString();
                    if (pGrpLay != null)
                    {
                        pGrpLay.Add(pBarriersLayer);
                    }
                    else
                    {
                        map.AddLayer(pBarriersLayer);

                    }

                }
                else
                {
                    pBarriersLayer = pBLayer as IFeatureLayer;
                }

                if (pBarriersLayer.FeatureClass == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pBarriersFC = Globals.createFeatureClassInMemory(A4LGSharedFunctions.Localizer.GetString("ExportBarriersName") +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pBarriersLayer = pBLayer as IFeatureLayer;
                    pBarriersLayer.FeatureClass = pBarriersFC;
                }

                pSimpleRend = new SimpleRendererClass();
                pSimpleRend.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeBarrier) as ISymbol;

                pGeoFeatLay = pBarriersLayer as IGeoFeatureLayer;
                pGeoFeatLay.Renderer = pSimpleRend as IFeatureRenderer;

                pBarriersFC = pBarriersLayer.FeatureClass;
                work = ((IDataset)pBarriersFC).Workspace;

                workEdit = work as IWorkspaceEdit;
                try
                {
                    editStarted = false;
                    if (!workEdit.IsBeingEdited())
                    {
                        workEdit.StartEditing(false);
                        editStarted = true;
                    }

                    workEdit.StartEditOperation();

                    pCursor = pBarriersFC.Update(null, false);
                    feature = null;
                    while ((feature = pCursor.NextFeature()) != null)
                    {
                        pCursor.DeleteFeature();
                    }
                    workEdit.StopEditOperation();

                    Marshal.ReleaseComObject(pCursor);

                    pCursor = pBarriersFC.Insert(true);
                    workEdit.StartEditOperation();

                    intIdField = pBarriersFC.Fields.FindField(IDFieldName);
                    intDateField = pBarriersFC.Fields.FindField(DateFieldName);

                    foreach (ESRI.ArcGIS.Geometry.IPoint pnt in Barriers) // Loop through List with foreach
                    {
                        pFBuf = pBarriersFC.CreateFeatureBuffer();
                        pFeat = (IFeature)pFBuf;
                        pNPt = new ESRI.ArcGIS.Geometry.PointClass();
                        pNPt.X = pnt.X;
                        pNPt.Y = pnt.Y;
                        pNPt.SpatialReference = pnt.SpatialReference;

                        pNPt.Project(((IGeoDataset)pBarriersFC).SpatialReference);
                        if (intIdField >= 0)
                        {
                            pFeat.set_Value(intIdField, ID);
                        }
                        if (intDateField >= 0)
                        {
                            pFBuf.set_Value(intDateField, dateTimeValue);
                        }
                        pFeat.Shape = pNPt;
                        pCursor.InsertFeature(pFBuf);

                    }
                    pCursor.Flush();
                    Marshal.ReleaseComObject(pCursor);
                    workEdit.StopEditOperation();

                    if (editStarted)
                    {
                        workEdit.StopEditing(true);
                        editStarted = false;
                    }
                }

                catch (COMException comExc)
                {
                    workEdit.AbortEditOperation();
                    if (editStarted)
                    {
                        workEdit.StopEditing(false);
                        editStarted = false;
                    }

                }

                IQueryFilter pQFilt;
                pQFilt = new QueryFilterClass();
                pQFilt.WhereClause = "1=1";
                IFeatureSelection pFS = (IFeatureSelection)pFlagsLayer;
                pFS.SelectFeatures(pQFilt, esriSelectionResultEnum.esriSelectionResultAdd, false);
                pFS = (IFeatureSelection)pBarriersLayer;
                pFS.SelectFeatures(pQFilt, esriSelectionResultEnum.esriSelectionResultAdd, false);
                pFS = null;
            }
            catch (Exception ex) { }
            finally
            {
                pSimpleRend = null;
                pGeoFeatLay = null;

                pFLayer = null;
                pBLayer = null;
                pCompLay = null;
                Flags = null;
                Barriers = null;
                pWS = null;
                pFields = null;
                pNPt = null;
                work = null;
                workEdit = null;
                pCursor = null;
                pFBuf = null;
                pFeat = null;
                feature = null;
                pFlagsFC = null;
                pBarriersFC = null;
                pFlagsLayer = null;
                pBarriersLayer = null;
            }

        }
        public static IFeatureLayer CreateOutageArea(IMap map, IGroupLayer pGrpLay, string ID, string IDFieldName, string DateFieldName, int suffCount)
        {

            bool fndAsFL = true;
            ILayer pFLayer = null;
            ICompositeLayer pCompLay = null;
            IWorkspace pWS = null;
            IFields pFields = null;
            IFeatureClass pOutageFC = null;
            IFeatureLayer pOutageLayer = null;

            ILayer pTemplateLayer = null;
            IFeatureLayer pFTemplateLayer = null;
            try
            {
                if (pGrpLay != null)
                {
                    pCompLay = (ICompositeLayer)pGrpLay;
                    pFLayer = Globals.FindLayerInGroup(pCompLay, A4LGSharedFunctions.Localizer.GetString("OutageAreaName") + " " +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString());


                }
                else
                {
                    pFLayer = Globals.FindLayer(map, A4LGSharedFunctions.Localizer.GetString("OutageAreaName") + " " +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString(), ref fndAsFL);


                }
                string tempLayName = ConfigUtil.GetConfigValue("TraceIsolation_AreaTemplate", null);
                if (tempLayName == null)
                {
                    pFields = Globals.createFeatureClassFields(map.SpatialReference, esriGeometryType.esriGeometryPolygon, IDFieldName, DateFieldName);
                }
                else
                {
                    pTemplateLayer = Globals.FindLayer(map, tempLayName, ref fndAsFL);
                    if (pTemplateLayer == null)
                    {
                        pFields = Globals.createFeatureClassFields(map.SpatialReference, esriGeometryType.esriGeometryPolygon, IDFieldName, DateFieldName);

                    }
                    else
                    {
                        pFTemplateLayer = (IFeatureLayer)pTemplateLayer;
                        if (pFTemplateLayer.FeatureClass != null)
                        {

                            ESRI.ArcGIS.Geodatabase.IFieldsEdit pFieldsEdit;
                            ESRI.ArcGIS.Geodatabase.IField pField;
                            ESRI.ArcGIS.Geodatabase.IFieldEdit pFieldEdit;
                            IGeometryDefEdit geomDefEdit;


                            pFields = Globals.copyFields(pFTemplateLayer.FeatureClass.Fields, pFTemplateLayer.FeatureClass.LengthField, pFTemplateLayer.FeatureClass.AreaField, IDFieldName, DateFieldName, true);
                            pFieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)pFields; // Explicit Cast


                            pField = pFields.get_Field(pFields.FindField("Shape"));
                            pFieldEdit = (IFieldEdit)pField;
                            geomDefEdit = (IGeometryDefEdit)pField.GeometryDef;

                            geomDefEdit.SpatialReference_2 = map.SpatialReference;

                        }
                        else
                        {
                            pFields = Globals.createFeatureClassFields(map.SpatialReference, esriGeometryType.esriGeometryPolygon, IDFieldName, DateFieldName);

                        }
                    }
                }


                //string pathToUserProf = ConfigUtil.generateUserCachePath();
                //pathToUserProf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGISSolutions\\Templates");

                //if (System.IO.Directory.Exists(pathToUserProf) == false)
                //{
                //    System.IO.Directory.CreateDirectory(pathToUserProf);
                //}
                //string dataPath = "C:\\Work\\ArcGIS for Utilities\\_Water\\Staging\\UtilityNetworkEditingA4W\\MapsandGeodatabase\\Template.gdb";
                //string layerName = "OutageArea";
                //IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                //IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(dataPath, 0);

                //IFeatureClass pFCSource = featureWorkspace.OpenFeatureClass(layerName);
                //pFields = Globals.copyFields(pFCSource.Fields, pFCSource.LengthField, pFCSource.AreaField, IDFieldName);
                //pFields = Globals.createFeatureClassFields(map.SpatialReference, esriGeometryType.esriGeometryPolygon, IDFieldName);

                if (pFLayer == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pOutageFC = Globals.createFeatureClassInMemory("OutageArea" +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pOutageLayer = new FeatureLayerClass();
                    pOutageLayer.FeatureClass = pOutageFC;
                    pOutageLayer.Name = A4LGSharedFunctions.Localizer.GetString("OutageAreaName") + " " +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix") + " " + suffCount.ToString();
                    if (pGrpLay != null)
                    {
                        pGrpLay.Add(pOutageLayer);
                    }
                    else
                    {
                        map.AddLayer(pOutageLayer);

                    }
                }
                else
                {
                    pOutageLayer = pFLayer as IFeatureLayer;
                }
                if (pOutageLayer.FeatureClass == null)
                {
                    if ((pWS = Globals.GetInMemoryWorkspaceFromTOC(map)) == null)
                    {
                        pWS = Globals.CreateInMemoryWorkspace();
                    }
                    pOutageFC = Globals.createFeatureClassInMemory(A4LGSharedFunctions.Localizer.GetString("OutageAreaName") +
                        A4LGSharedFunctions.Localizer.GetString("IsoTraceResultsLayerSuffix"), pFields, pWS, esriFeatureType.esriFTSimpleJunction);
                    pOutageLayer = pFLayer as IFeatureLayer;
                    pOutageLayer.FeatureClass = pOutageFC;
                }
                if (pFTemplateLayer != null)
                {
                    Globals.copyLayerFromLayer(pFTemplateLayer, pOutageLayer);
                }
                return pOutageLayer;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

                pFLayer = null;
                pCompLay = null;
                pWS = null;
                pFields = null;
                pOutageFC = null;

            }
        }

        public static string LogLocations = "";

        public static string lineColors = "255,0,0";

        public static IFeature AddPointAlongLineWithIntersect(ref IApplication app, ref  IEditor editor, ICurve curve, IFeatureLayer pointFLayer, double targetPointDistance,
                                                         bool targetPointDistanceIsPercent, IEditTemplate editTemplate, IFeatureLayer pPolyFL,
                                                         string side)
        {

            double workingDist = targetPointDistance;
            IPoint point = null;
            ISpatialFilter pSpatFilt = null;

            IFeatureCursor pFC = null;
            IFeature pFeat = null;
            IPoint pIntPnt = null;
            IPoint pOutPnt = null;

            try
            {
                if (curve != null && pointFLayer != null)
                {


                    point = new PointClass();
                    pSpatFilt = new SpatialFilterClass();
                    pSpatFilt.Geometry = curve;
                    pSpatFilt.GeometryField = pPolyFL.FeatureClass.ShapeFieldName;
                    pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                    pFC = pPolyFL.Search(pSpatFilt, true);
                    pOutPnt = new PointClass();
                    double distFromCurve = 0, distAlongCurve = 0;
                    bool bSide = false;
                    pFeat = pFC.NextFeature();
                    if (pFeat == null)
                    {
                        return AddPointAlongLine(ref app, ref editor, curve, pointFLayer, targetPointDistance, targetPointDistanceIsPercent, editTemplate);

                    }
                    else
                    {
                        bool intersectFound = false;
                        while (pFeat != null)
                        {
                            pIntPnt = (IPoint)Globals.GetIntersection(pFeat.ShapeCopy, (IPolyline)curve);
                            if (pIntPnt != null)
                            {

                                curve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, pIntPnt, false, pOutPnt, ref distAlongCurve, ref distFromCurve, ref bSide);
                                if (targetPointDistanceIsPercent)
                                {
                                    intersectFound = true;
                                    if (side.ToUpper() == "TO")
                                    {
                                        workingDist = distAlongCurve + ((curve.Length - distAlongCurve) * (targetPointDistance / 100));

                                    }
                                    else
                                    {
                                        workingDist = distAlongCurve - (distAlongCurve * (targetPointDistance / 100));

                                    }
                                    curve.QueryPoint(esriSegmentExtension.esriNoExtension, workingDist, false, point);
                                }
                                else
                                {
                                    if (side.ToUpper() == "TO")
                                        targetPointDistance = distAlongCurve + targetPointDistance;
                                    else
                                        targetPointDistance = distAlongCurve - targetPointDistance;
                                    if (targetPointDistance > 0)
                                    {
                                        if (curve.Length > targetPointDistance)
                                        {
                                            intersectFound = true;
                                            curve.QueryPoint(esriSegmentExtension.esriNoExtension, targetPointDistance, false, point);
                                            break;
                                        }

                                        else if (targetPointDistance > curve.Length)
                                        {
                                            point = null;
                                            intersectFound = false;
                                            pFeat = pFC.NextFeature();
                                            continue;
                                            //distAlongCurve  + (distAlongCurve * (targetPointDistance
                                            //curve.QueryPoint(esriSegmentExtension.esriNoExtension, curve.Length, true, point);

                                        }
                                        else if (targetPointDistance < 0)
                                        {
                                            point = null;
                                            intersectFound = false;
                                            pFeat = pFC.NextFeature();
                                            continue;
                                            //distAlongCurve  + (distAlongCurve * (targetPointDistance
                                            //curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0, true, point);
                                        }
                                    }
                                }
                            }
                            pFeat = pFC.NextFeature();
                        }
                        if (intersectFound == false)
                        {
                            AddPointAlongLine(ref app, ref editor, curve, pointFLayer, targetPointDistance, targetPointDistanceIsPercent, editTemplate);

                        }

                    }

                    if (point != null)
                    {
                        if (!(point.IsEmpty))
                        {

                            if (editTemplate != null)
                            {
                                pFeat = Globals.CreateFeature(point, editTemplate, editor, app, true, false, true);
                            }
                            else
                            {
                                pFeat = Globals.CreateFeature(point, pointFLayer, editor, app, true, false, true);
                            }



                            try
                            {
                                if (pFeat != null)
                                {
                                    Globals.ValidateFeature(pFeat);
                                    pFeat.Store();
                                    return pFeat;
                                }
                                else

                                    return null;


                            }
                            catch
                            {

                                return null;
                            }
                        }
                        else

                            return null;
                    }
                    else

                        return null;
                }
                else

                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in addPointAlongLineWithIntersect: " + ex.Message);
                return null;
            }
            finally
            {
                point = null;
                pSpatFilt = null;

                pFC = null;
                pFeat = null;
                pIntPnt = null;
                pOutPnt = null;

            }

        }
        public static IFeature AddPointAlongLine(ref IApplication app, ref  IEditor editor, ICurve curve, IFeatureLayer pointFLayer, double targetPointDistance,
                                                         bool targetPointDistanceIsPercent, IEditTemplate editTemplate)
        {
            double workingDist = targetPointDistance;

            IPoint point = null;
            IFeature pFeat = null;
            try
            {

                if (curve != null && pointFLayer != null)
                {
                    point = new PointClass();


                    if (targetPointDistanceIsPercent)
                    {
                        workingDist = targetPointDistance;
                        workingDist = workingDist / 100;

                        //if (workingDist >= 0 && workingDist <= 100)
                        //    workingDist = targetPointDistance;

                        if (workingDist >= 0 && workingDist <= 100)
                            curve.QueryPoint(esriSegmentExtension.esriNoExtension, workingDist, true, point);
                        else if (workingDist < 0)
                            curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0, true, point);
                        else
                            curve.QueryPoint(esriSegmentExtension.esriNoExtension, 100, true, point);
                        //curve.QueryPoint(esriSegmentExtension.esriNoExtension, workingDist, true, point);
                    }
                    else if (curve.Length > targetPointDistance)
                        curve.QueryPoint(esriSegmentExtension.esriNoExtension, targetPointDistance, false, point);
                    else
                        curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, point);
                    if (!(point.IsEmpty))
                    {

                        if (editTemplate != null)
                        {
                            pFeat = Globals.CreateFeature(point, editTemplate, editor, app, true, false, true);
                        }
                        else
                        {
                            pFeat = Globals.CreateFeature(point, pointFLayer, editor, app, true, false, true);
                        }



                        try
                        {
                            if (pFeat != null)
                            {
                                Globals.ValidateFeature(pFeat);
                                pFeat.Store();
                            }
                            //if (pFeat is INetworkFeature)
                            //{
                            //    INetworkFeature pNF = (INetworkFeature)pFeat;

                            //    pNF.Connect();
                            //}
                            return pFeat;
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Error storing new feature in the " + pointFLayer.Name + " layer\nThis is typically caused by a rule in the AA causing this feature not to be valid and deleting it\nModule: AddPointAlongLine\n" + ex.Message);

                            return null;
                        }
                    }
                    else

                        return null;
                }
                else

                    return null;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in addPointAlongLine: " + ex.Message);

                return null;
            }
            finally
            {
                point = null;
                pFeat = null;
            }

        }
        public static string FormatValueToFieldLength(IField pFld, string value)
        {
            try
            {
                if (pFld.Type == esriFieldType.esriFieldTypeString)
                {
                    if (value.Length > pFld.Length)
                    {
                        return value.Substring(0, pFld.Length);

                    }
                    else
                    {
                        return value;

                    }
                }
                else
                {
                    return value;

                }
            }
            catch
            {
                return value;
            }

        }
        public static double ConvertClockPositionToDegrees(double value)
        {
            return value * (360 / 12);

        }
        public static IPolyline CreateAngledLineFromLocationOnLine(IPoint inPoint, IFeatureLayer mainLayer, bool boolLayerOrFC,
           double RadianAngle, double LineLength, string AddAngleToLineAngle, bool StartAtInput, bool CheckSelection)
        {

            IPoint snapPnt = null;
            IPolyline pPolyline = null;
            IFeature geoMainLine = null;
            IPoint pNewPt = null;
            IConstructPoint2 pConsPoint = null;
            //double dAlong;

            try
            {

                if (mainLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    return null;

                }
                if (mainLayer == null)
                {
                    return null;
                }


                double searchDist = Globals.GetXYTolerance(mainLayer) * 2000;

                geoMainLine = Globals.GetClosestFeature(inPoint, mainLayer, searchDist, boolLayerOrFC, CheckSelection);
                bool side = false;


                double angleOfLine = 0;
                if (geoMainLine != null)
                {
                    snapPnt = Globals.GetPointOnLine(inPoint, (IGeometry)geoMainLine.ShapeCopy, searchDist, out side);
                    //snapPnt = inPoint;
                    angleOfLine = Globals.GetAngleOfLineAtPoint((IPolyline)geoMainLine.ShapeCopy, snapPnt, searchDist);
                }
                else
                {
                    snapPnt = inPoint;
                }
                // dAlong = Globals.PointDistanceOnLine((IPoint)inPoint, (IPolyline)geoMainLine, 2, out snapPnt);


                if (AddAngleToLineAngle.ToUpper() == "TRUE")
                {
                    RadianAngle = angleOfLine + RadianAngle;
                }

                pNewPt = new PointClass();
                pConsPoint = pNewPt as IConstructPoint2;

                pConsPoint.ConstructAngleDistance(snapPnt, RadianAngle, LineLength);

                pPolyline = new PolylineClass();
                if (StartAtInput)
                {
                    pPolyline.FromPoint = snapPnt;
                    pPolyline.ToPoint = pNewPt;
                }
                else
                {
                    pPolyline.FromPoint = pNewPt;
                    pPolyline.ToPoint = snapPnt;
                }
                return pPolyline;


            }




            catch
            {

                //snapPnt = null;

                //pPolyline = null;
                //geoMainLine = null;
                //pNewPt = null;
                //pConsPoint = null;

                return null;
            }
            finally
            {

                // snapPnt = null;
                pPolyline = null;
                geoMainLine = null;
                pNewPt = null;
                pConsPoint = null;
            }
        }
        public static string generateRandomID(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        public static double ConvertRadsToDegrees(double Angle)
        {
            double pi = 0;
            pi = 4 * Math.Atan(1);
            Angle = Angle * 180 / pi;
            if (Angle < 0)
            {
                Angle = 360 + Angle;
            }
            if (Angle > 360)
            {
                Angle = Angle - 360;
            }
            return Angle;

        }
        public static double ConvertDegToRads(double Angle)
        {
            if (Angle < 0)
            {
                Angle = 360 + Angle;
            }
            if (Angle > 360)
            {
                Angle = Angle - 360;
            }
            double pi = 0;
            pi = 4 * Math.Atan(1);
            return Angle * pi / 180;
        }
        public static double ConvertArithmeticToGeographic(double Angle)
        {

            Angle = 270 - Angle;
            if (Angle < 0)
            {
                Angle = 360 + Angle;
            }
            if (Angle > 360)
            {
                Angle = Angle - 360;
            }
            return Angle;

        }
        public static double GetAngleOfLineAtDistance(IPolyline inLine, double distance)
        {
            ICurve pCurve;
            ILine pLine;
            try
            {
                pLine = new LineClass();

                pCurve = (ICurve)inLine;
                pCurve.QueryTangent(esriSegmentExtension.esriNoExtension, distance, false, pCurve.Length, pLine);
                return pLine.Angle;

            }
            catch
            {
                return -9999;
            }
            finally
            {
                pCurve = null;
                pLine = null;
            }
        }
        public static double GetAngleOfLineAtPoint(IPolyline inLine, IPoint location, double xyTol)
        {

            if (inLine == null || location == null) return 0;
            IPoint pSnapPt = null;

            try
            {
                pSnapPt = null;
                double dist = Globals.PointDistanceOnLine(location, inLine, 15, out pSnapPt);
                double angle = GetAngleOfLineAtDistance(inLine, dist);
                if (angle != -9999)
                    return angle;

            }
            catch
            {
            }
            finally
            {
                pSnapPt = null;
            }


            //old manual way
            ISegmentCollection pSegCol = default(ISegmentCollection);
            ISegmentCollection pSegColTest = default(ISegmentCollection);
            IEnumSegment pEnumSegments = default(IEnumSegment);
            ISegment pSegment = null;
            IPolyline pPolylineTest = default(IPolyline);
            IProximityOperator pProxOp = default(IProximityOperator);
            IRelationalOperator pRelOp = default(IRelationalOperator);
            ISegmentCollection pSegColNearest = null;
            ILine pLine = null;

            try
            {




                double dShortestDistance;
                double dDistance;
                int lngPart = 0;
                int lngSeg = 0;

                bool bFound = false;

                //Get the polyline and get a reference to it's segments

                pSegCol = (ISegmentCollection)inLine;

                //If it has more than one segment...
                if (pSegCol.SegmentCount > 1)
                {
                    //Initalize test
                    bFound = false;


                    //Get segments
                    pEnumSegments = pSegCol.EnumSegments;

                    //Find touching segment...
                    //Get first segment
                    pEnumSegments.Next(out pSegment, ref lngPart, ref lngSeg);
                    pRelOp = (IRelationalOperator)location;
                    while ((pSegment != null))
                    {
                        pSegColTest = new PolylineClass();
                        pSegColTest.AddSegment(pSegment);

                        if (pRelOp.Within((IPolyline)pSegColTest))
                        {
                            bFound = true;
                            pSegColNearest = new PolylineClass();
                            pSegColNearest.AddSegment(pSegment);
                            break;
                        }
                        if (pRelOp.Touches((IPolyline)pSegColTest))
                        {
                            bFound = true;
                            pSegColNearest = new PolylineClass();
                            pSegColNearest.AddSegment(pSegment);
                            break;
                        }

                        pEnumSegments.Next(out pSegment, ref  lngPart, ref lngSeg);
                    }

                    //If no touching segment found - find nearest segement
                    //Get first segment

                    if (!bFound)
                    {

                        //Get segments
                        pEnumSegments = pSegCol.EnumSegments;

                        //Find touching segment...
                        //Get first segment
                        pEnumSegments.Next(out pSegment, ref lngPart, ref lngSeg);


                        dShortestDistance = 9999;


                        while ((pSegment != null))
                        {
                            //Create a new segment collection to hold just this segment
                            pSegColTest = new PolylineClass();
                            pSegColTest.AddSegment(pSegment);

                            //if it's closer to the point, save this segment
                            pProxOp = (IProximityOperator)pSegColTest;

                            //dDistance = pProxOp.ReturnDistance(pPoint)
                            IGeometry pTempGeo = location;
                            pTempGeo.Project(inLine.SpatialReference);
                            dDistance = pProxOp.ReturnDistance(pTempGeo);

                            if (dDistance < dShortestDistance)
                            {
                                dShortestDistance = dDistance;
                                pSegColNearest = new PolylineClass();
                                IClone pClone = (IClone)pSegment;

                                pSegColNearest.AddSegment((ISegment)pClone.Clone());
                            }

                            //Get next segment
                            pEnumSegments.Next(out pSegment, ref lngPart, ref lngSeg);
                        }
                    }

                    //If it only has one segment use that...
                }
                else
                {
                    pSegColNearest = new PolylineClass();
                    pSegColNearest.AddSegment(pSegCol.get_Segment(0));
                }

                //Get the Polyline interface
                pPolylineTest = (IPolyline)pSegColNearest;
                //QI

                //Create a simple line so we can get it's angle
                //Also set it so it's ToPoint will be at the point feature
                pLine = new Line();
                if (pPolylineTest.ToPoint.X == location.X & pPolylineTest.ToPoint.Y == location.Y)
                {
                    pLine.FromPoint = pPolylineTest.FromPoint;
                    pLine.ToPoint = pPolylineTest.ToPoint;
                }
                else
                {
                    pLine.FromPoint = pPolylineTest.ToPoint;
                    pLine.ToPoint = pPolylineTest.FromPoint;
                }

                return pLine.Angle;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {

                pSegCol = null;
                pSegColTest = null;
                pEnumSegments = null;
                pSegment = null;
                pPolylineTest = null;
                pProxOp = null;
                pRelOp = null;
                pSegColNearest = null;
                pLine = null;
                // topoOp = null;
            }
        }
        public static bool toBoolean(string value)
        {
            if (value.Trim() == "1")
            {
                return true;
            }
            else if (value.Trim().ToUpper() == "TRUE")
            {
                return true;
            }
            else if (value.Trim().ToUpper() == "YES")
            {
                return true;
            }

            return false;

        }
        public static Bitmap BitmapFromTemplate(IEditTemplate editTemplate, Control control)
        {
            //get the template symbol by creating a temporary feature then
            //find the symbol from the renderer
            IFeatureLayer featLayer = editTemplate.Layer as IFeatureLayer;
            IFeatureClass featClass = featLayer.FeatureClass;
            IFeatureBuffer featBuffer = featClass.CreateFeatureBuffer();
            IFeature feature = featBuffer as IFeature;
            editTemplate.SetDefaultValues(feature);
            IGeoFeatureLayer geoFeatLayer = featLayer as IGeoFeatureLayer;
            ISymbol symbol = geoFeatLayer.Renderer.get_SymbolByFeature(feature);

            //convert Symbol to Bitmap
            Bitmap bitmap = WindowsAPI.SymbolToBitmap(symbol, new Size(16, 16), control.CreateGraphics(), ColorTranslator.ToWin32(control.BackColor));
            return bitmap;
        }
        public static void copyLayerFromLayer(IFeatureLayer copyFromLayer, IFeatureLayer copyToLayer)
        {
            IGeoFeatureLayer copyGeoFromLayer = null;
            IGeoFeatureLayer copyGeoToLayer = null;
            IObjectCopy pObjectCopy = null;
            try
            {

                copyGeoFromLayer = copyFromLayer as IGeoFeatureLayer;
                copyGeoToLayer = copyToLayer as IGeoFeatureLayer;
                pObjectCopy = new ObjectCopyClass();

                copyGeoToLayer.Renderer = pObjectCopy.Copy(copyGeoFromLayer.Renderer) as IFeatureRenderer;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                copyGeoFromLayer = null;
                copyGeoToLayer = null;
            }
        }

        //public static void SetVertexDefaultSymbol(ref IEditor3 vEditor)
        //{
        //    if (vEditor == null)
        //        return;

        //    IEditProperties editProperties = null;
        //    try
        //    {

        //        editProperties = vEditor as IEditProperties;
        //        editProperties.SketchVertexSymbol = markerSymbol;
        //        editProperties.SelectedVertexSymbol = currentMarkerSymbol;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in the changeVertexSymbol" + ex.Message);
        //    }

        //    finally
        //    {

        //        editProperties = null;
        //    }

        //}
        //public static void ChangeVertexSymbol(ref IEditor3 vEditor, IMarkerSymbol markerSymbol, IMarkerSymbol currentMarkerSymbol)
        //{
        //    if (vEditor == null)
        //        return;

        //    IEditProperties editProperties = null;
        //    try
        //    {

        //        editProperties = vEditor as IEditProperties;
        //        editProperties.SketchVertexSymbol = markerSymbol;
        //        editProperties.SelectedVertexSymbol = currentMarkerSymbol;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in the changeVertexSymbol" + ex.Message);
        //    }

        //    finally
        //    {

        //        editProperties = null;
        //    }

        //}
        //public static void ChangeVertexSymbol(IApplication app, IMarkerSymbol markerSymbol)
        //{
        //    IEditor3 vEditor  =null;
        //    try
        //    {
        //        vEditor = app.FindExtensionByName("ESRI Object Editor") as IEditor3;
        //        ChangeVertexSymbol(ref vEditor, markerSymbol);

        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show("Error in the changeVertexSymbol" + ex.Message);
        //    }

        //    finally
        //    {
        //        vEditor = null;

        //    }

        //}
        public static AddressInfo GetAddressInfo(IApplication app, IPoint pointLocation, string RoadLayerName, string FullNameField,
            string LeftToField, string RightToField, string LeftFromField, string RightFromField, bool searchOnLayer, double searchDistance)
        {
            IFeatureLayer lineLayer = null;

            try
            {

                bool lineFndAsFL = true;
                lineLayer = Globals.FindLayer(app, RoadLayerName, ref lineFndAsFL) as IFeatureLayer;
                if (lineLayer == null)
                    return new AddressInfo("Street Layer not found: " + RoadLayerName);
                int idxRdName, idxRdLtFrm, idxRdRtFrm, idxRdLtTo, idxRdRtTo;
                idxRdName = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, FullNameField);
                idxRdLtTo = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, LeftToField);
                idxRdRtTo = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, RightToField);
                idxRdLtFrm = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, LeftFromField);
                idxRdRtFrm = Globals.GetFieldIndex(lineLayer.FeatureClass.Fields, RightFromField);


                if (idxRdName == -1)
                {
                    return new AddressInfo("Street Layer Field not found: " + FullNameField);

                }

                if (idxRdLtFrm == -1)
                {
                    return new AddressInfo("Street Layer Field not found: " + LeftToField);

                }

                if (idxRdRtFrm == -1)
                {
                    return new AddressInfo("Street Layer Field not found: " + RightToField);

                }

                if (idxRdLtTo == -1)
                {
                    return new AddressInfo("Street Layer Field not found: " + LeftFromField);

                }

                if (idxRdRtTo == -1)
                {
                    return new AddressInfo("Street Layer Field not found: " + RightFromField);

                }

                return GetAddressInfo(app, pointLocation, lineLayer, idxRdName, idxRdLtTo, idxRdRtTo, idxRdLtFrm, idxRdRtFrm, searchOnLayer, searchDistance);


            }
            catch
            {
                return null;
            }
            finally
            {
                lineLayer = null;
            }

        }
        public static AddressInfo GetAddressInfo(IApplication app, IPoint pointLocation, IFeatureLayer RoadLayer, int FullNameField,
                  int LeftToField, int RightToField, int LeftFromField, int RightFromField, bool searchOnLayer, double searchDistance)
        {

            IFeature pLineFeat = null;
            AddressInfo retAdd = null;
            try
            {
                retAdd = new AddressInfo();






                pLineFeat = Globals.GetClosestFeature(pointLocation, RoadLayer, searchDistance, searchOnLayer, false);
                if (pLineFeat == null)
                {
                    return null;
                }
                return GetAddressInfo(app, pointLocation, pLineFeat, FullNameField,
                  LeftToField, RightToField, LeftFromField, RightFromField, searchOnLayer, searchDistance);
            }
            catch
            {
                return null;
            }
            finally
            {

                pLineFeat = null;

            }

        }
        public static AddressInfo GetAddressInfo(IApplication app, IPoint pointLocation, IFeature RoadFeature, int FullNameField,
               int LeftToField, int RightToField, int LeftFromField, int RightFromField, bool searchOnLayer, double searchDistance)
        {


            IPoint pSnapedPoint = null;
            AddressInfo retAdd = null;
            try
            {
                retAdd = new AddressInfo();




                if (FullNameField == -1 || LeftToField == -1 || RightToField == -1 || LeftFromField == -1 || RightFromField == -1)
                {
                    return null;
                }



                if (RoadFeature == null)
                {
                    return null;
                }

                bool rightSide = true;
                pSnapedPoint = Globals.GetPointOnLine(pointLocation, RoadFeature.Shape as IPolyline, searchDistance, out  rightSide);

                IPoint snapPnt = null;

                double dAlong = Globals.PointDistanceOnLine(pSnapedPoint, RoadFeature.Shape as IPolyline, 2, out snapPnt);
                if (Globals.IsNumeric(dAlong.ToString()))
                {
                    snapPnt = null;

                    double totalDist = Globals.GetLineLength(RoadFeature.Shape as IPolyline);
                    double perc = (dAlong / totalDist);//*100
                    double retAddNumLeft = 0;
                    double retAddNumRight = 0;
                    string roadName = RoadFeature.get_Value(FullNameField).ToString();

                    string LeftFrom = RoadFeature.get_Value(LeftFromField).ToString();
                    string LeftTo = RoadFeature.get_Value(LeftToField).ToString();
                    double dblLeftFrom = 0;
                    double dblLeftTo = 0;
                    if (Globals.IsNumeric(LeftFrom) && Globals.IsNumeric(LeftTo))
                    {
                        dblLeftFrom = Convert.ToDouble(LeftFrom);
                        dblLeftTo = Convert.ToDouble(LeftTo);

                        if (dblLeftFrom > dblLeftTo)
                        {
                            retAddNumLeft = dblLeftTo + ((dblLeftFrom - dblLeftTo) * perc);
                        }
                        else if (dblLeftFrom < dblLeftTo)
                        {
                            retAddNumLeft = dblLeftFrom + ((dblLeftTo - dblLeftFrom) * perc);
                        }
                        else
                        {
                            retAddNumLeft = dblLeftFrom;
                        }

                    }
                    else
                    {
                        // return null;
                    }

                    string RightFrom = RoadFeature.get_Value(RightFromField).ToString();
                    string RightTo = RoadFeature.get_Value(RightToField).ToString();
                    double dblRightFrom = 0;
                    double dblRightTo = 0;
                    if (Globals.IsNumeric(RightFrom) && Globals.IsNumeric(RightTo))
                    {
                        dblRightFrom = Convert.ToDouble(RightFrom);
                        dblRightTo = Convert.ToDouble(RightTo);

                        if (dblRightFrom > dblRightTo)
                        {
                            retAddNumRight = dblRightTo + ((dblRightFrom - dblRightTo) * perc);
                        }
                        else if (dblRightFrom < dblRightTo)
                        {
                            retAddNumRight = dblRightFrom + ((dblRightTo - dblRightFrom) * perc);
                        }
                        else
                        {
                            retAddNumRight = dblRightFrom;
                        }

                    }
                    else
                    {
                        // return null;
                    }


                    if (Globals.IsOdd(dblRightFrom))
                    {
                        retAddNumRight = Globals.RoundToEvenOdd(Globals.EvenOdd.Odd, retAddNumRight);


                    }
                    else
                    {

                        retAddNumRight = Globals.RoundToEvenOdd(Globals.EvenOdd.Even, retAddNumRight);

                    }

                    if (Globals.IsOdd(dblLeftFrom))
                    {
                        retAddNumLeft = Globals.RoundToEvenOdd(Globals.EvenOdd.Odd, retAddNumLeft);



                    }
                    else
                    {

                        retAddNumLeft = Globals.RoundToEvenOdd(Globals.EvenOdd.Even, retAddNumLeft);


                    }
                    retAdd.LeftAddress = retAddNumLeft;
                    retAdd.RightAddress = retAddNumRight;
                    retAdd.StreetName = roadName;
                    retAdd.StreetGeometry = RoadFeature.ShapeCopy;

                    return retAdd;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {


                pSnapedPoint = null;

            }

        }



        public static void TransformDataXYToPixelXY(IRaster pRaster, ref double dblX, ref double dblY)
        {// notes:
            // assume LOWER left origin for projected coordinate system
            // origin is UPPER left for raster pixels

            // Function usage: dblX and dblY are passed in in data coords and passed out in pixel coords

            IRasterProps pRasterProps;
            pRasterProps = (IRasterProps)pRaster;

            // get cell size dimensions
            IPnt pPnt;
            pPnt = new PntClass();
            pPnt = pRasterProps.MeanCellSize();

            double dblCellSizeX;
            double dblCellSizeY;
            dblCellSizeX = pPnt.X;
            dblCellSizeY = pPnt.Y;

            double dblPixelX;
            double dblPixelY;

            IEnvelope pEnv;
            pEnv = new EnvelopeClass();
            pEnv = pRasterProps.Extent;

            double dblDataX;
            dblDataX = dblX;

            double dblDataY;
            dblDataY = dblY;

            // X
            dblPixelX = (dblDataX - pEnv.XMin) / dblCellSizeX;

            // Y (taking care of different origins)
            double dblHeight;
            dblHeight = pRasterProps.Height;
            dblPixelY = dblHeight - ((dblDataY - pEnv.YMin) / dblCellSizeY);

            // make sure I don't get anything larger than height/width or less than 0
            double dblWidth;
            dblWidth = pRasterProps.Width;

            if (dblPixelX > dblWidth)
                dblPixelX = dblWidth;
            else if (dblPixelX < 0)
                dblPixelX = 0;

            if (dblPixelY > dblHeight)
                dblPixelY = dblHeight;
            else if (dblPixelY < 0)
                dblPixelY = 0;


            // return transformed coordinates
            dblX = dblPixelX;
            dblY = dblPixelY;

        }

        # region Classes

        public class DomSubList
        {
            private string m_Value;
            private string m_Display;
            public DomSubList(string Value, string Display)
            {
                m_Display = Display;
                m_Value = Value;
            }
            public string Display
            {
                get { return m_Display; }
                set { m_Display = value; }
            }
            public string Value
            {
                get { return m_Value; }
                set { m_Value = value; }
            }
        }
        public class WindowWrapper : IWin32Window
        {
            public IntPtr Handle { get; private set; }
            public WindowWrapper(IntPtr hwnd) { Handle = hwnd; }
        }
        public class OptionsToPresent : IEquatable<OptionsToPresent>//: IComparable<OptionsToPresent> //IComparer,, IEquatable<OptionsToPresent>
        {
            public OptionsToPresent()
            { }

            public OptionsToPresent(int newOID, string newDisplay, string newLayerName, object newValue)
            {
                OID = newOID;
                Display = newDisplay;
                LayerName = newLayerName;
                Value = newValue;

            }
            public int OID { get; set; }
            public string Display { get; set; }
            public string LayerName { get; set; }
            public object Value { get; set; }

            //public override int Compare(object x, object y)
            //{
            //    if (((OptionsToPresent)x).Display == ((OptionsToPresent)y).Display &&
            //        ((OptionsToPresent)x).LayerName == ((OptionsToPresent)y).LayerName &&
            //        ((OptionsToPresent)x).OID == ((OptionsToPresent)y).OID)
            //    {
            //        return 0;
            //    }
            //    else return 1;

            //}
            //public override int CompareTo(object obj)
            //{
            //    if (this.Display == ((OptionsToPresent)obj).Display &&
            //        this.LayerName == ((OptionsToPresent)obj).LayerName &&
            //        this.OID == ((OptionsToPresent)obj).OID)
            //    {
            //        return 1;
            //    }
            //    else return 0;

            //}
            //public override bool CompareTo(OptionsToPresent obj)
            //{
            //    if (this.Display == ((OptionsToPresent)obj).Display &&
            //        this.LayerName == ((OptionsToPresent)obj).LayerName &&
            //        this.OID == ((OptionsToPresent)obj).OID)
            //    {
            //        return true;
            //    }
            //    else return false;

            //}
            //public override int GetHashCode()
            //{
            //    return Text.GetHashCode() ^ Value.GetHashCode();
            //}

            //public override bool Equals(object other)
            //{
            //    // Would still want to check for null etc. first.
            //    return (this.Display == ((OptionsToPresent)other).Display &&
            //        this.LayerName == ((OptionsToPresent)other).LayerName &&
            //        this.OID == ((OptionsToPresent)other).OID);

            //}
            public bool Equals(OptionsToPresent other)
            {
                // Would still want to check for null etc. first.
                return (this.Display == ((OptionsToPresent)other).Display &&
                    this.LayerName == ((OptionsToPresent)other).LayerName &&
                    this.OID == ((OptionsToPresent)other).OID);

            }
            public override String ToString()
            {
                return Display;
            }

        }
        # endregion

        # region Conversion
        public static List<string> CursorToList(ref ICursor pCur, int[] FieldIndex)
        {
            try
            {
                List<string> vals = new List<string>();


                IRow pRow;
                while ((pRow = pCur.NextRow()) != null)
                {
                    string dis = "";
                    for (int i = 0; i < FieldIndex.Length; i++)
                    {
                        if (dis == "")
                        {
                            dis = pRow.get_Value(FieldIndex[i]).ToString();
                        }
                        else
                        {
                            dis = dis + " | " + pRow.get_Value(FieldIndex[i]).ToString();
                        }
                    }
                    vals.Add(dis.Trim());
                }
                pRow = null;

                return vals;
            }
            catch
            {
                return null;
            }
            finally { }
        }
        public static List<string> CursorToList(ref ICursor pCur, string[] FieldName)
        {
            try
            {
                int[] intFldIdx = new int[FieldName.Length];
                for (int i = 0; i < FieldName.Length; i++)
                {

                    int intFldIdxTmp = pCur.FindField(FieldName[i]);
                    if (intFldIdxTmp == -1)
                    {
                        intFldIdxTmp = pCur.Fields.FindFieldByAliasName(FieldName[i]);
                    }

                    if (intFldIdxTmp == -1)
                        return null;
                    intFldIdx[i] = intFldIdxTmp;
                }

                return CursorToList(ref pCur, intFldIdx);

            }
            catch
            {
                return null;
            }
            finally { }
        }
        public static List<string> CursorToList(ref ICursor pCur, int FieldIndex)
        {
            try
            {
                List<string> vals = new List<string>();


                IRow pRow;
                while ((pRow = pCur.NextRow()) != null)
                {
                    vals.Add(pRow.get_Value(FieldIndex).ToString());
                }
                pRow = null;

                return vals;
            }
            catch
            {
                return null;
            }
            finally { }
        }
        public static List<string> CursorToList(ref ICursor pCur, string FieldName)
        {
            try
            {
                int intFldIdx = pCur.FindField(FieldName);
                if (intFldIdx == -1)
                {
                    intFldIdx = pCur.Fields.FindFieldByAliasName(FieldName);
                }

                if (intFldIdx == -1)
                    return null;
                return CursorToList(ref pCur, intFldIdx);

            }
            catch
            {
                return null;
            }
            finally { }
        }
        # endregion

        # region GeometricNetworkOperations

        public static string ReturnAccumulation(ref IApplication app, ref IFeature pFeature, string sWeightName, string flowMethod)
        {
            if (flowMethod.ToUpper().Contains("UP"))
            {
                return ReturnAccumulation(ref app, ref pFeature, sWeightName, esriFlowMethod.esriFMUpstream);
            }
            else
            {
                return ReturnAccumulation(ref app, ref pFeature, sWeightName, esriFlowMethod.esriFMDownstream);
            }

        }
        public static string ReturnAccumulation(ref IApplication app, ref IFeature pFeature, string sWeightName, esriFlowMethod flowMethod)
        {
            IEnumNetEID pJuncSel = null;
            IEnumNetEID pEdgeSel = null;
            IGeometricNetwork pGNetwork = null;
            INetwork pNetwork = null;
            INetworkFeature pNetworkFeature = null;
            INetSchema pNetSchema = null;
            INetWeight pNetWeight = null;
            INetElements pNetElements = null;
            IEdgeFeature pEdge = null;
            IFeature pToFeature = null;
            INetFlag pFlag = null;

            INetSolverWeights pNetSolverW = null;
            IJunctionFlag junctionFlag;
            IEdgeFlag edgeFlag;
            INetworkAnalysisExt pNetAnalysisExt = null;
            ITraceFlowSolverGEN traceFlowSolver = null;
            UID pID = null;
            List<IEdgeFlag> pEdgeFlags = null;
            List<IJunctionFlag> pJunctionFlags = null;
            INetElementBarriers pEdgeElementBarriers;
            INetElementBarriers pJunctionElementBarriers;
            ISelectionSetBarriers pSelectionSetBarriers;
            object pTotalCost = null;
            INetworkAnalysisExtFlags pFlags = null;

            try
            {
                int i;
                //Find the specified network
                if ((pFeature is INetworkFeature) == false)
                {
                    MessageBox.Show("Network not found for specified feature.", "Flow Accumulation");
                    return "Non Network Feature";
                }

                pNetworkFeature = pFeature as INetworkFeature;
                pGNetwork = pNetworkFeature.GeometricNetwork;


                pNetElements = pGNetwork.Network as INetElements;

                pID = new UID();

                pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);

                Globals.SetCurrentNetwork(ref pNetAnalysisExt, ref pGNetwork);
                traceFlowSolver = Globals.CreateTraceFlowSolverFromToolbar(ref pNetAnalysisExt, out pEdgeFlags, out pJunctionFlags, out pEdgeElementBarriers, out pJunctionElementBarriers, out pSelectionSetBarriers) as ITraceFlowSolverGEN;

                pFlags = pNetAnalysisExt as INetworkAnalysisExtFlags;
                pFlags.ClearFlags();
                pID = null;

                //Find wieghts

                pNetwork = pGNetwork.Network;
                pNetSchema = pNetwork as INetSchema;
                for (i = 0; i < pNetSchema.WeightCount; i++)
                {
                    pNetWeight = pNetSchema.get_Weight(i);
                    if (pNetWeight.WeightName == sWeightName)
                        break;

                }
                if (pNetWeight != null)
                {
                    if (pNetWeight.WeightType == esriWeightType.esriWTBitGate || pNetWeight.WeightType == esriWeightType.esriWTNull)
                    {
                        pNetWeight = null;
                    }
                }
                //Get Junction for trace
                if (pFeature.FeatureType == esriFeatureType.esriFTComplexEdge || pFeature.FeatureType == esriFeatureType.esriFTSimpleEdge)
                {

                    pEdge = pFeature as IEdgeFeature;

                    pToFeature = pFeature;
                    int lUserSubId = -1;
                    int lUserClassId = 0, lUserId = 0;
                    int lEID;
                    if (pFeature.FeatureType == esriFeatureType.esriFTComplexEdge)
                    {

                        IComplexNetworkFeature pCNetFeature = (IComplexNetworkFeature)pFeature;

                        lEID = pCNetFeature.FindEdgeEID((pEdge.ToJunctionFeature.GeometryForJunctionElement[0]) as IPoint);
                        pNetElements.QueryIDs(lEID, esriElementType.esriETEdge, out lUserClassId, out lUserId, out lUserSubId);
                        pCNetFeature = null;

                    }

                    pFlag = new EdgeFlagClass();
                    pFlag.UserClassID = pToFeature.Class.ObjectClassID;
                    pFlag.UserID = pToFeature.OID;
                    pFlag.UserSubID = lUserSubId;


                }
                else if (pFeature.FeatureType == esriFeatureType.esriFTSimpleJunction || pFeature.FeatureType == esriFeatureType.esriFTComplexJunction)
                {
                    pToFeature = pFeature;

                    //Create a new junction flag
                    pFlag = new JunctionFlagClass();
                    pFlag.UserClassID = pToFeature.Class.ObjectClassID;
                    pFlag.UserID = pToFeature.OID;
                    pFlag.UserSubID = 0;

                }
                else
                {
                    return "Unsupported Network Feature";

                }




                AddFlagToTraceSolver(pFlag, ref traceFlowSolver, out junctionFlag, out edgeFlag);

                //Get trace weights
                if (pNetWeight != null)
                {
                    pNetSolverW = traceFlowSolver as INetSolverWeights;
                    pNetSolverW.JunctionWeight = pNetWeight;
                    if (flowMethod == esriFlowMethod.esriFMUpstream)
                    {
                        pNetSolverW.ToFromEdgeWeight = pNetWeight;
                    }
                    else
                    {
                        pNetSolverW.FromToEdgeWeight = pNetWeight;
                    }
                }

                //Run this trace
                traceFlowSolver.FindAccumulation(flowMethod, esriFlowElements.esriFEJunctionsAndEdges, out pJuncSel, out pEdgeSel, out  pTotalCost);



                //Debug.Print "Total upstream flow accumulation is " & CStr(pTotalCost) & "."

                return pTotalCost.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the ReturnAccumulation\n" + ex.Message);
                return "Error";
            }
            finally
            {

                pJuncSel = null;
                pEdgeSel = null;
                pGNetwork = null;
                pNetwork = null;
                pNetworkFeature = null;
                pNetSchema = null;
                pNetWeight = null;
                pNetElements = null;
                pEdge = null;
                pToFeature = null;
                pFlag = null;
                pNetSolverW = null;
                junctionFlag = null;
                edgeFlag = null;
                pNetAnalysisExt = null;
                traceFlowSolver = null;
                pID = null;
                pEdgeFlags = null;
                pJunctionFlags = null;
                pEdgeElementBarriers = null;
                pJunctionElementBarriers = null;
                pSelectionSetBarriers = null;
                pTotalCost = null;
                pFlags = null;
            }


        }

        public static int getConnectionCount(IFeature inFeature)
        {
            IJunctionFeature iJuncFeat;
            IEdgeFeature iEdgeFeat;

            INetworkFeature netFeat = inFeature as INetworkFeature;

            if (inFeature.FeatureType == esriFeatureType.esriFTComplexJunction || inFeature.FeatureType == esriFeatureType.esriFTSimpleJunction)
            {

                iJuncFeat = (IJunctionFeature)netFeat;
                ISimpleJunctionFeature iSJunc = iJuncFeat as ISimpleJunctionFeature;
                if (iSJunc == null)
                    return -1;
                return iSJunc.EdgeFeatureCount;




            }
            else if (inFeature.FeatureType == esriFeatureType.esriFTComplexEdge)
            {
                iEdgeFeat = (IEdgeFeature)netFeat;

                // IComplexJunctionFeature iCEd = iJuncFeat as IComplexJunctionFeature;
                IComplexEdgeFeature iCEdge = ((IFeature)iEdgeFeat) as IComplexEdgeFeature;

                if (iCEdge == null)
                    return -1;

                return iCEdge.JunctionFeatureCount;



            }
            else if (inFeature.FeatureType == esriFeatureType.esriFTSimpleEdge)
            {
                //iEdgeFeat = (IEdgeFeature)netFeat;

                //// IComplexJunctionFeature iCEd = iJuncFeat as IComplexJunctionFeature;
                //ISimpleEdgeFeature iSEdge = ((IFeature)iEdgeFeat) as ISimpleEdgeFeature;

                //if (iSEdge == null)
                //    return -1;

                return -1;



            }

            return -1;


        }
        public static ESRI.ArcGIS.Geodatabase.esriFlowDirection GetFlowDirectionAtLocation(IFeature pFeature, IFeatureLayer targetLineFLayer, IPoint pPnt, double toler)
        {
            IEnumNetEID enumNetEID = null;
            try
            {
                if (pFeature is INetworkFeature)
                {
                    //                        INetworkFeature pNF = (INetworkFeature)newFeature;

                    INetworkClass netClass = targetLineFLayer.FeatureClass as INetworkClass;
                    IGeometricNetwork gn = netClass.GeometricNetwork;

                    INetwork net = gn.Network;

                    IEIDHelper pEIDHelperEdges;
                    IEnumEIDInfo pEnumEIDInfoEdges;

                    pEIDHelperEdges = new EIDHelper();
                    pEIDHelperEdges.GeometricNetwork = gn;
                    pEIDHelperEdges.ReturnFeatures = true;

                    pEIDHelperEdges.ReturnGeometries = true;
                    pEIDHelperEdges.PartialComplexEdgeGeometry = true;

                    IUtilityNetworkGEN unet = net as IUtilityNetworkGEN;
                    INetElements netelems;
                    netelems = unet as INetElements;
                    enumNetEID = netelems.GetEIDs(targetLineFLayer.FeatureClass.ObjectClassID, pFeature.OID, esriElementType.esriETEdge);
                    enumNetEID.Reset();
                    pEnumEIDInfoEdges = pEIDHelperEdges.CreateEnumEIDInfo(enumNetEID);


                    pEnumEIDInfoEdges.Reset();  //edges

                    //int edgeEID;
                    IEIDInfo pEdgeInfo;
                    //ISpatialFilter pSpatFil;
                    //pSpatFil = new SpatialFilterClass();
                    //pSpatFil.GeometryField = "SHAPE";
                    //pSpatFil.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects;
                    //pSpatFil.Geometry = pPnt;
                    //ITopologicalOperator2 pTopo;//= new ITopologicalOperator2();
                    //IPoint[] points = new IPoint[1];
                    //points[0] = pPnt;

                    //IPointCollection4 pointCollection = new MultipointClass();
                    //IGeometryBridge geometryBride = new GeometryEnvironmentClass();
                    //geometryBride.AddPoints(pointCollection, ref points);

                    //ITopologicalOperator topoOp = pPnt as ITopologicalOperator;
                    ////if (dist == 0)
                    ////    dist = 500;
                    ////topoOp.Simplify();
                    //IPolygon poly = topoOp.Buffer(toler) as IPolygon;
                    //IRelationalOperator pRelOp = (IRelationalOperator)poly;

                    //for (long j = 0; j < pEnumEIDInfoEdges.Count; j++)
                    //{
                    //    //   edgeEID = enumNetEID.Next();
                    //    pEdgeInfo = pEnumEIDInfoEdges.Next();

                    //    if (pRelOp.Touches(pEdgeInfo.Geometry))
                    //        return unet.GetFlowDirection(pEdgeInfo.EID);

                    //    //pTopo = pPnt
                    //    //   pTopo.
                    //    //  unet.SetFlowDirection(pEdgeInfo.EID, esriFlowDirection.esriFDWithFlow);

                    //}
                    int ClosestEID = -1;
                    IProximityOperator proxOp = pPnt as IProximityOperator;
                    double lastDistance, distance;

                    // proxOp = (IProximityOperator)searchShape;
                    lastDistance = 9999999999;

                    for (long j = 0; j < pEnumEIDInfoEdges.Count; j++)
                    {
                        pEdgeInfo = pEnumEIDInfoEdges.Next();


                        //distance = proxOp.ReturnDistance(pEdgeInfo.Geometry);
                        IGeometry pTempGeo = pEdgeInfo.Geometry;
                        pTempGeo.Project(pPnt.SpatialReference);

                        distance = proxOp.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        if (distance <= lastDistance)
                        {
                            ClosestEID = pEdgeInfo.EID;
                            lastDistance = distance;
                        }

                    }
                    if (ClosestEID == -1)
                    {
                        return ESRI.ArcGIS.Geodatabase.esriFlowDirection.esriFDUninitialized;
                    }
                    else
                    {
                        return unet.GetFlowDirection(ClosestEID);
                    }

                }
                else
                {
                    return ESRI.ArcGIS.Geodatabase.esriFlowDirection.esriFDAgainstFlow;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - getFlowDirection" + Environment.NewLine + ex.Message);

                return ESRI.ArcGIS.Geodatabase.esriFlowDirection.esriFDUninitialized;
            }
            finally
            {
                if (enumNetEID != null)
                {
                    Marshal.ReleaseComObject(enumNetEID);
                }
            }

        }
        public static void SetFlowDirection(IFeature pFeature, IFeatureLayer targetLineFLayer, IMap map)
        {
            IEnumNetEID enumNetEID = null;
            try
            {
                if (pFeature is INetworkFeature)
                {
                    //                        INetworkFeature pNF = (INetworkFeature)newFeature;

                    INetworkClass netClass = targetLineFLayer.FeatureClass as INetworkClass;
                    IGeometricNetwork gn = netClass.GeometricNetwork;

                    INetwork net = gn.Network;


                    IUtilityNetworkGEN unet = net as IUtilityNetworkGEN;
                    INetElements netelems;
                    netelems = unet as INetElements;
                    enumNetEID = netelems.GetEIDs(targetLineFLayer.FeatureClass.ObjectClassID, pFeature.OID, esriElementType.esriETEdge);
                    enumNetEID.Reset();

                    int edgeEID; IFeature pFeatTemp;

                    for (long j = 0; j < enumNetEID.Count; j++)
                    {
                        edgeEID = enumNetEID.Next();
                        unet.SetFlowDirection(edgeEID, esriFlowDirection.esriFDWithFlow);
                        pFeatTemp = GetFeatureByEID(edgeEID, gn, map, esriElementType.esriETEdge);
                        pFeatTemp.Store();
                        pFeatTemp = null;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - SetFlowDirection" + Environment.NewLine + ex.Message);

                return;
            }
            finally
            {
                if (enumNetEID != null)
                {
                    Marshal.ReleaseComObject(enumNetEID);
                }
            }

        }
        public static long GetTotalVisibleNetworkFeatures(IMap pMap)
        {
            IActiveView av = pMap as IActiveView;

            UID geoUID = new UID();
            geoUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            long total = 0;
            IEnumLayer enumLayers = pMap.get_Layers(geoUID, true);
            ILayer layer = enumLayers.Next();
            while (layer != null)
            {
                IFeatureLayer fLayer = layer as IFeatureLayer;
                if (fLayer != null && isVisible(layer, pMap))
                {
                    INetworkClass netclass = fLayer.FeatureClass as INetworkClass;
                    if (netclass != null)
                    {
                        ISpatialFilter sfilter = new SpatialFilterClass();
                        sfilter.Geometry = av.Extent;
                        sfilter.GeometryField = fLayer.FeatureClass.ShapeFieldName;
                        sfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        total += fLayer.FeatureClass.FeatureCount(sfilter);
                    }
                }
                layer = enumLayers.Next();
            }
            return total;
        }
        public static Hashtable GetGeometricNetworksJunctionsLayersHT(ref IMap map, bool onlyVisible)
        {

            if (map == null)
            {
                return null;
            }
            // IMapLayers mapLayers = (IMapLayers)map;
            Hashtable layerList = new Hashtable();
            //   bool found = false;
            ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            try
            {

                ESRI.ArcGIS.Carto.IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
                ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
                while (!(layer == null))
                {
                    IFeatureLayer fLayer = (IFeatureLayer)layer;

                    if (fLayer.Valid)//&& mapLayers.IsLayerVisible(layer)
                    {
                        if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
                        {
                            //Get Geometric network and add to list
                            INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
                            if (netClass != null)
                            {
                                if (onlyVisible)
                                {
                                    if (fLayer.Visible)
                                    {
                                        IDataset pDS = fLayer.FeatureClass as IDataset;
                                        string FCName = getClassName(pDS);
                                        if (!(layerList.ContainsKey(FCName)))
                                            layerList.Add(FCName, fLayer);
                                        //    layerList.Add(fLayer);
                                    }

                                }
                                else
                                {

                                    IDataset pDS = fLayer.FeatureClass as IDataset;
                                    string FCName = getClassName(pDS);
                                    if (!(layerList.ContainsKey(FCName)))
                                        layerList.Add(FCName, fLayer);
                                }
                            }

                        }

                    }
                    layer = enumLayer.Next();
                }
                enumLayer = null;
                layer = null;

                return layerList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - GetGeomtricNetworksJunctionsLayersHT" + Environment.NewLine + ex.Message);

                return null;
            }

        }
        public static ArrayList GetGeometricNetworksJunctionsLayers(ref IMap map, bool onlyVisible)
        {

            if (map == null)
            {
                return null;
            }
            // IMapLayers mapLayers = (IMapLayers)map;
            ArrayList layerList = new ArrayList();
            //   bool found = false;
            ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            try
            {

                ESRI.ArcGIS.Carto.IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
                ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
                while (!(layer == null))
                {
                    IFeatureLayer fLayer = (IFeatureLayer)layer;

                    if (fLayer.Valid)//&& mapLayers.IsLayerVisible(layer)
                    {
                        if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
                        {
                            //Get Geometric network and add to list
                            INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
                            if (netClass != null)
                            {
                                if (onlyVisible)
                                {
                                    if (fLayer.Visible)
                                        layerList.Add(fLayer);
                                }
                                else
                                {

                                    layerList.Add(fLayer);
                                }
                            }

                        }

                    }
                    layer = enumLayer.Next();
                }
                enumLayer = null;
                layer = null;

                return layerList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - GetGeometricNetworksJunctionsLayers" + Environment.NewLine + ex.Message);

                return null;
            }

        }

        public static bool SetCurrentNetwork(ref INetworkAnalysisExt pNetAnalysisExt, ref IGeometricNetwork gn)
        {
            try
            {
                pNetAnalysisExt.CurrentNetwork = gn; //pGeometricNet

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetCurrentNetwork(ref INetworkAnalysisExt pNetAnalysisExt, string strNetworkName)
        {
            IDataset pDataset = null;

            try
            {

                if (strNetworkName.Trim() != "")
                {
                    pDataset = (IDataset)pNetAnalysisExt.CurrentNetwork.Network;
                    if (pDataset.Name == strNetworkName)
                    {
                        return true;  // return without doing anything
                    }
                    else
                    {
                        //          INetworkCollection pNetColl;

                        for (int i = 0; i < pNetAnalysisExt.NetworkCount; i++)
                        {
                            pDataset = (IDataset)pNetAnalysisExt.get_Network(i).Network;
                            if (pDataset.Name == strNetworkName)
                            {
                                pNetAnalysisExt.CurrentNetwork = pNetAnalysisExt.get_Network(i); //pGeometricNet
                                return true;

                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;

            }
            finally
            {
                pDataset = null;
            }

        }


        public static IFeature GetNetworkAndFeatureAtLocation(IPoint pPnt, IApplication app, esriElementType elemType, out IGeometricNetwork GeometricNetwork, double snapTol)
        {
            GeometricNetwork = null;
            List<IGeometricNetwork> pGNs = null;
            IPointToEID pointToEID;
            IGeoDataset pDs;
            IPoint snappedPoint = null;
            double distanceAlong;
            try
            {

                if (pPnt == null)
                {
                    return null;
                }
                if (pPnt.SpatialReference == null)
                {
                    pPnt.SpatialReference = ((IMxDocument)app.Document).FocusMap.SpatialReference;

                }

                pGNs = GetGeometricNetworksCurrentlyVisible(ref app);

                int EIDElement = -1;

                foreach (IGeometricNetwork pGN in pGNs)
                {
                    pointToEID = new PointToEIDClass() as IPointToEID;

                    // find the nearest junction element to this Point
                    pointToEID.GeometricNetwork = pGN as IGeometricNetwork;
                    pointToEID.SourceMap = ((IMxDocument)app.Document).FocusMap;
                    pointToEID.SnapTolerance = snapTol;

                    pDs = (pGN as IGeometricNetwork).FeatureDataset as IGeoDataset;
                    if (pPnt.SpatialReference.Name != pDs.SpatialReference.Name)
                    {
                        pPnt.Project(pDs.SpatialReference);
                    }


                    //pGN.FeatureDataset
                    //pPnt.Project(pGN.FeatureDataset.)
                    if (elemType == esriElementType.esriETEdge)
                    {


                        try
                        {
                            pointToEID.GetNearestEdge(pPnt, out EIDElement, out snappedPoint, out distanceAlong);
                            if (EIDElement > 0)
                            {
                                GeometricNetwork = pGN;
                                return GetFeatureByEID(EIDElement, pGN, ((IMxDocument)app.Document).FocusMap, elemType);
                            }
                        }
                        catch //(Exception ex)
                        {
                        }
                        //EIDElement = pGN.get_EdgeElement(pPnt);
                        //if (EIDElement > 0)
                        //{
                        //    GeometricNetwork = pGN;
                        //    return GetFeatureByEID(EIDElement, pGN, ((IMxDocument)app.Document).FocusMap, elemType);
                        //}

                    }
                    else if (elemType == esriElementType.esriETJunction)
                    {

                        try
                        {
                            pointToEID.GetNearestJunction(pPnt, out EIDElement, out snappedPoint);
                            if (EIDElement > 0)
                            {
                                GeometricNetwork = pGN;
                                return GetFeatureByEID(EIDElement, pGN, ((IMxDocument)app.Document).FocusMap, elemType);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        //EIDElement = pGN.get_JunctionElement(pPnt);
                        //if (EIDElement > 0)
                        //{
                        //    GeometricNetwork = pGN;
                        //    return GetFeatureByEID(EIDElement, pGN, ((IMxDocument)app.Document).FocusMap, elemType);
                        //}
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                pGNs = null;
                MessageBox.Show("Error occurred in GetNetworkAndFeatureAtLocation \r\n" + ex.Message);
                return null;
            }
            finally
            {
                pGNs = null;
                pointToEID = null;
                pDs = null;
                snappedPoint = null;
            }
        }
        public static IFeature GetFeatureByEID(int EID, IGeometricNetwork pGN, IMap map, esriElementType elemType)
        {
            IEIDHelper eidHelper = null;
            IEnumNetEIDBuilderGEN pEnumEIDBuild = null;
            IEnumNetEID pEnumEID = null;


            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo pEdgeInfo = null;
            try
            {
                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = pGN;
                eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = false;
                pEnumEIDBuild = new EnumNetEIDArrayClass();// as IEnumNetEIDBuilderGEN;
                pEnumEIDBuild.ElementType = elemType;
                pEnumEIDBuild.Network = pGN.Network;
                pEnumEIDBuild.Add(EID);

                pEnumEID = pEnumEIDBuild as IEnumNetEID;
                enumEidInfo = eidHelper.CreateEnumEIDInfo(pEnumEID);
                enumEidInfo.Reset();

                while ((pEdgeInfo = enumEidInfo.Next()) != null)
                {


                    return pEdgeInfo.Feature;


                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred in GetFeatureByEID \r\n" + ex.Message);
                return null;
            }
            finally
            {
                eidHelper = null;
                pEnumEIDBuild = null;
                pEnumEID = null;


                enumEidInfo = null;
                pEdgeInfo = null;
            }

        }
        //public static List<IGeometricNetwork> GetGeometricNetworksCurrentlyVisible(ref IMap map)
        //{
        //    if (map == null)
        //    {
        //        return null;
        //    }
        //    IMapLayers mapLayers = (IMapLayers)map;
        //    List<IGeometricNetwork> gnList = new List<IGeometricNetwork>();
        //    bool found = false;
        //    ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
        //    uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        //    try
        //    {

        //        ESRI.ArcGIS.Carto.IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
        //        ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
        //        while (!(layer == null))
        //        {
        //            IFeatureLayer fLayer = (IFeatureLayer)layer;

        //            if (fLayer.Valid && fLayer.Visible && mapLayers.IsLayerVisible(layer))
        //            {
        //                if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
        //                {
        //                    //Get Geometric network and add to list
        //                    INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
        //                    if (netClass != null)
        //                    {
        //                        IGeometricNetwork gn = netClass.GeometricNetwork;
        //                        found = false;
        //                        for (int index = 0; index < gnList.Count; index++)
        //                        {
        //                            if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
        //                            {
        //                                found = true;
        //                                break;
        //                            }
        //                        }
        //                        if (!found)
        //                        {
        //                            gnList.Add(gn);
        //                        }
        //                    }

        //                }

        //            }
        //            layer = enumLayer.Next();
        //        }
        //        return gnList;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in the Global Functions - GetGeomtricNetworksCurrentVisible" + Environment.NewLine + ex.Message);

        //        return null;
        //    }

        //}
        //public static List<IGeometricNetwork> GetGeometricNetworksCurrentlyVisible(ref IApplication app)
        //{
        //    if (app == null)
        //    {
        //        return null;
        //    }
        //    IMapLayers mapLayers = (IMapLayers)(((IMxDocument)app.Document).FocusMap);
        //    List<IGeometricNetwork> gnList = new List<IGeometricNetwork>();
        //    bool found = false;
        //    ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
        //    uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        //    try
        //    {

        //        ESRI.ArcGIS.Carto.IEnumLayer enumLayer = mapLayers.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
        //        ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
        //        while (!(layer == null))
        //        {
        //            IFeatureLayer fLayer = (IFeatureLayer)layer;

        //            if (fLayer.Valid && fLayer.Visible && mapLayers.IsLayerVisible(layer))
        //            {
        //                if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
        //                {
        //                    //Get Geometric network and add to list
        //                    INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
        //                    if (netClass != null)
        //                    {
        //                        IGeometricNetwork gn = netClass.GeometricNetwork;
        //                        found = false;
        //                        for (int index = 0; index < gnList.Count; index++)
        //                        {
        //                            if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
        //                            {
        //                                found = true;
        //                                break;
        //                            }
        //                        }
        //                        if (!found)
        //                        {
        //                            gnList.Add(gn);
        //                        }
        //                    }

        //                }

        //            }
        //            layer = enumLayer.Next();
        //        }
        //        return gnList;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in the Global Functions - GetGeomtricNetworksCurrentVisible" + Environment.NewLine + ex.Message);

        //        return null;
        //    }

        //}
        //public static List<IGeometricNetwork> GetGeometricNetworksCheckedVisible(ref IMap map)
        //{

        //    if (map == null)
        //    {
        //        return null;
        //    }
        //    // IMapLayers mapLayers = (IMapLayers)map;
        //    List<IGeometricNetwork> gnList = new List<IGeometricNetwork>();
        //    bool found = false;
        //    ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
        //    uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        //    try
        //    {

        //        ESRI.ArcGIS.Carto.IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
        //        ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
        //        while (!(layer == null))
        //        {
        //            IFeatureLayer fLayer = (IFeatureLayer)layer;

        //            if (fLayer.Valid && fLayer.Visible)//&& mapLayers.IsLayerVisible(layer)
        //            {
        //                if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
        //                fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
        //                {
        //                    //Get Geometric network and add to list
        //                    INetworkClass netClass = fLayer.FeatureClass as INetworkClass;
        //                    if (netClass != null)
        //                    {
        //                        IGeometricNetwork gn = netClass.GeometricNetwork;
        //                        found = false;
        //                        for (int index = 0; index < gnList.Count; index++)
        //                        {
        //                            if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
        //                            {
        //                                found = true;
        //                                break;
        //                            }
        //                        }
        //                        if (!found)
        //                        {
        //                            gnList.Add(gn);
        //                        }
        //                    }

        //                }

        //            }
        //            layer = enumLayer.Next();
        //        }
        //        enumLayer = null;
        //        layer = null;

        //        return gnList;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.Write(ex);
        //        return null;
        //    }

        //}

        public static List<IGeometricNetwork> GetGeometricNetworksCheckedVisible(ref IMap map)
        {

            if (map == null)
            {
                return null;
            }

            List<IGeometricNetwork> gnList = null;
            bool found = false;
            ESRI.ArcGIS.esriSystem.IUID uid = null;
            ESRI.ArcGIS.Carto.IEnumLayer enumLayer = null;
            ESRI.ArcGIS.Carto.ILayer layer = null;
            IFeatureLayer fLayer = null;
            INetworkClass netClass = null;
            IGeometricNetwork gn = null;
            try
            {
                gnList = new List<IGeometricNetwork>();
                uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";

                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
                layer = enumLayer.Next();
                while (!(layer == null))
                {
                    fLayer = (IFeatureLayer)layer;

                    if (fLayer.Valid && fLayer.Visible)//&& mapLayers.IsLayerVisible(layer)
                    {
                        if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
                        {
                            //Get Geometric network and add to list
                            netClass = fLayer.FeatureClass as INetworkClass;
                            if (netClass != null)
                            {
                                gn = netClass.GeometricNetwork;
                                found = false;
                                for (int index = 0; index < gnList.Count; index++)
                                {
                                    if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    gnList.Add(gn);
                                }
                            }

                        }

                    }
                    layer = enumLayer.Next();
                }
                enumLayer = null;
                layer = null;

                return gnList;
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
                return null;
            }
            finally
            {
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);

                }
                uid = null;
                enumLayer = null;
                layer = null;
                fLayer = null;
                netClass = null;
                gn = null;
            }

        }
        public static List<IGeometricNetwork> GetGeometricNetworksCurrentlyVisible(ref IMap map)
        {

            if (map == null)
            {
                return null;
            }
            IMapLayers mapLayers = null;
            List<IGeometricNetwork> gnList = null;
            IFeatureLayer fLayer = null;
            ESRI.ArcGIS.Carto.IEnumLayer enumLayer = null;
            ESRI.ArcGIS.Carto.ILayer layer = null;
            INetworkClass netClass = null;
            bool found = false;
            IGeometricNetwork gn = null;
            ESRI.ArcGIS.esriSystem.IUID uid = null;
            List<string> gnNames = new List<string>();
            IVersionedWorkspace versionedWorkspace = null;
            IVersion version = null;
            string strVer;
            try
            {
                mapLayers = (IMapLayers)map;
                gnList = new List<IGeometricNetwork>();
                uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
                layer = enumLayer.Next();
                while (!(layer == null))
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.Valid && fLayer.Visible && mapLayers.IsLayerVisible(layer))
                    {
                        if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
                        {
                            //Get Geometric network and add to list
                            netClass = fLayer.FeatureClass as INetworkClass;
                            if (netClass != null)
                            {
                                gn = netClass.GeometricNetwork;
                                strVer = "default";
                                if (gn.FeatureDataset.Workspace is IVersionedWorkspace)
                                {
                                    versionedWorkspace = (IVersionedWorkspace)gn.FeatureDataset.Workspace;
                                    version = (IVersion)versionedWorkspace;
                                    strVer = version.VersionName;
                                }
                                if (gnNames.IndexOf(gn.FeatureDataset.Name + ":" + strVer) < 0)
                                {
                                    gnNames.Add(gn.FeatureDataset.Name + ":" + strVer);
                                    gnList.Add(gn);
                                }
                                //found = false;
                                //for (int index = 0; index < gnList.Count; index++)
                                //{
                                //    if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
                                //    {
                                //        found = true;
                                //        break;
                                //    }
                                //}
                                //if (!found)
                                //{
                                //    gnList.Add(gn);
                                //}
                            }

                        }

                    }
                    layer = enumLayer.Next();
                }
                return gnList;
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
                return null;
            }
            finally
            {
                if (mapLayers != null)
                {
                    Marshal.ReleaseComObject(mapLayers);
                }
                //List<IGeometricNetwork>gnList = new List<IGeometricNetwork>();

                if (fLayer != null)
                {
                    Marshal.ReleaseComObject(fLayer);
                }
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
                if (layer != null)
                {
                    Marshal.ReleaseComObject(layer);
                }

                if (netClass != null)
                {
                    Marshal.ReleaseComObject(netClass);
                }
                if (gn != null)
                {
                    Marshal.ReleaseComObject(gn);
                }
                if (uid != null)
                {
                    Marshal.ReleaseComObject(uid);
                }

                mapLayers = null;

                fLayer = null;
                enumLayer = null;
                layer = null;
                netClass = null;

                gn = null;
                uid = null;

            }

        }
        public static List<IGeometricNetwork> GetGeometricNetworksCurrentlyVisible(ref IApplication app)
        {
            if (app == null)
            {
                return null;
            }
            IMap pMap = (((IMxDocument)app.Document).FocusMap);
            return GetGeometricNetworksCurrentlyVisible(ref pMap);

        }
        public static List<IGeometricNetwork> GetGeometricNetworks(ref IMap map)
        {
            if (map == null)
            {
                return null;
            }
            IMapLayers mapLayers = null;
            List<IGeometricNetwork> gnList = null;

            ESRI.ArcGIS.esriSystem.IUID uid = null;
            ESRI.ArcGIS.Carto.IEnumLayer enumLayer = null;
            ESRI.ArcGIS.Carto.ILayer layer = null;
            IFeatureLayer fLayer = null;
            INetworkClass netClass = null;
            IGeometricNetwork gn = null;
            bool found = false;
            try
            {
                gnList = new List<IGeometricNetwork>();
                uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                mapLayers = (IMapLayers)map;
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)(uid)), true);
                layer = enumLayer.Next();
                while (!(layer == null))
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.FeatureClass != null)
                    {
                        if (fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTComplexEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleEdge ||
                        fLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
                        {
                            //Get Geometric network and add to list
                            netClass = fLayer.FeatureClass as INetworkClass;
                            if (netClass != null)
                            {
                                gn = netClass.GeometricNetwork;
                                found = false;
                                for (int index = 0; index < gnList.Count; index++)
                                {
                                    if (IsInNetwork(fLayer.FeatureClass.FeatureClassID, gnList[index] as IGeometricNetwork, true))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    gnList.Add(gn);
                                }
                            }

                        }
                    }

                    layer = enumLayer.Next();
                }
                return gnList;
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
                return null;
            }
            finally
            {
                if (mapLayers != null)
                {
                    Marshal.ReleaseComObject(mapLayers);
                }
                //List<IGeometricNetwork>gnList = new List<IGeometricNetwork>();

                if (fLayer != null)
                {
                    Marshal.ReleaseComObject(fLayer);
                }
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
                if (layer != null)
                {
                    Marshal.ReleaseComObject(layer);
                }

                if (netClass != null)
                {
                    Marshal.ReleaseComObject(netClass);
                }
                if (gn != null)
                {
                    Marshal.ReleaseComObject(gn);
                }
                if (uid != null)
                {
                    Marshal.ReleaseComObject(uid);
                }

                mapLayers = null;

                fLayer = null;
                enumLayer = null;
                layer = null;
                netClass = null;

                gn = null;
                uid = null;


            }

        }
        public static int GetGeometricNetwork(ref List<IGeometricNetwork> gnList, string NetName)
        {
            IGeometricNetwork GN = null;
            IDataset pNetData = null;
            try
            {
                for (int i = 0; i < gnList.Count; i++)
                {
                    GN = (IGeometricNetwork)gnList[i];
                    pNetData = (IDataset)GN;


                    if (NetName == pNetData.Name)
                    {
                        return i;

                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                GN = null;
                pNetData = null;
            }


        }
        public static bool IsInNetwork(int lFeatureClassID, IGeometricNetwork gn, bool includeOrphan)
        {
            IEnumFeatureClass enumFC = null;
            IFeatureClass fc = null;
            try
            {
                //Return false if this is just the orphan junction class unless with allow that with includeOrphan = true
                if (!includeOrphan)
                {
                    if (lFeatureClassID == gn.OrphanJunctionFeatureClass.FeatureClassID) return false;
                }

                bool isIn = false;


                //Get an enumeration of the simple junction feature classes in the network
                enumFC = gn.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                enumFC.Reset();
                fc = enumFC.Next();
                while (fc != null)
                {
                    if (fc.FeatureClassID == lFeatureClassID)
                    {
                        isIn = true;
                        return isIn;
                    }
                    fc = enumFC.Next();
                }

                //Get an enumeration of the complex junction feature classes in the network
                enumFC = gn.get_ClassesByType(esriFeatureType.esriFTComplexJunction);
                enumFC.Reset();
                fc = enumFC.Next();
                while (fc != null)
                {
                    if (fc.FeatureClassID == lFeatureClassID)
                    {
                        isIn = true;
                        return isIn;
                    }
                    fc = enumFC.Next();
                }


                //Get an enumeration of the complex junction feature classes in the network
                enumFC = gn.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
                enumFC.Reset();
                fc = enumFC.Next();
                while (fc != null)
                {
                    if (fc.FeatureClassID == lFeatureClassID)
                    {
                        isIn = true;
                        return isIn;
                    }
                    fc = enumFC.Next();
                }

                //Get an enumeration of the complex junction feature classes in the network
                enumFC = gn.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
                enumFC.Reset();
                fc = enumFC.Next();
                while (fc != null)
                {
                    if (fc.FeatureClassID == lFeatureClassID)
                    {
                        isIn = true;
                        return isIn;
                    }
                    fc = enumFC.Next();
                }


                return isIn;

            }
            catch
            {
                return false;
            }
            finally
            {
                if (enumFC != null)
                {
                    Marshal.ReleaseComObject(enumFC);
                }
                enumFC = null;
                fc = null;

            }
        }
        public static void ClearGNFlags(IApplication app, GNTypes removeType)
        {
            ICommandItem pCmdItem = null;
            try
            {


                pCmdItem = GetCommand("{48EE0DEF-BF70-11D2-BABE-00C04FA33C20}", app, (int)removeType);
                if (pCmdItem != null)
                    pCmdItem.Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ClearGNFlags\n" + ex.Message);

            }
            finally
            {
                pCmdItem = null;
            }



        }
        public enum GNTypes
        {
            Flags = 1, Barries = 2, Results = 3

        };
        public enum flagType { EdgeFlag, JunctionFlag, EdgeBarrier, JunctionBarrier };

        public static bool isOrpanJunction(IFeature inFeature)
        {
            INetworkFeature inNetFeat = null;
            try
            {
                if (inFeature != null)
                {
                    // Try to get Network Feature
                    if (inFeature is INetworkFeature)
                    {
                        inNetFeat = inFeature as INetworkFeature;

                        // If network feature and orphan junction feature class, return true
                        if ((inNetFeat != null) &&
                            (inFeature.Class.ObjectClassID == inNetFeat.GeometricNetwork.OrphanJunctionFeatureClass.ObjectClassID))
                            return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                inNetFeat = null;

            }
        }
        public static IFeatureClass GetFeatureClassFromGeometricNetwork(string sourceFCName, IGeometricNetwork gn, esriFeatureType featureType)
        {
            IEnumFeatureClass enumFC = null;
            IFeatureClass fc = null;
            IDataset pDataset = null;
            try
            {
                if (gn != null)
                {

                    enumFC = gn.get_ClassesByType(featureType);
                    fc = enumFC.Next();

                    while (fc != null)
                    {

                        pDataset = fc as IDataset;

                        if ((pDataset.BrowseName).ToUpper() == (sourceFCName).ToUpper())
                        {
                            return fc;

                        }
                        if ((pDataset.FullName.NameString).ToUpper() == (sourceFCName).ToUpper())
                        {
                            return fc;

                        }
                        if ((pDataset.FullName.NameString).Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1).ToUpper() == (sourceFCName).ToUpper())
                        {
                            return fc;


                        }
                        if ((pDataset.BrowseName).Substring(pDataset.BrowseName.LastIndexOf(".") + 1).ToUpper() == (sourceFCName).ToUpper())
                        {
                            return fc;


                        }

                        fc = enumFC.Next();
                    }
                }
                return null;
            }
            finally
            {
                if (enumFC != null)
                {
                    Marshal.ReleaseComObject(enumFC);

                }
                enumFC = null;

                pDataset = null;

            }
        }
        public static IJunctionFlag GetJunctionFlagWithGN(double x, double y, ref  IMap map, ref IGeometricNetwork gn, ref double snapTol, out IPoint snappedPoint, out int EID, out  IFlagDisplay pFlagDisplay, bool Flag)
        {
            IPoint point = null;
            //Initialize output variables
            snappedPoint = null; EID = -1;
            pFlagDisplay = null;
            try
            {


                // convert the (x,y) map coordinate to a new Point object
                point = new PointClass() as IPoint;
                point.X = x;
                point.Y = y;
                point.SpatialReference = map.SpatialReference;
                point.SnapToSpatialReference();
                return GetJunctionFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint, out EID, out  pFlagDisplay, Flag);
            }
            catch
            {
                return null;
            }
            finally
            {
                point = null;
            }
        }
        public static IJunctionFlag GetJunctionFlagWithGN(ref IPoint point, ref IMap map, ref IGeometricNetwork gn, double snapTol, out IPoint snappedPoint, out int EID, out  IFlagDisplay pFlagDisplay, bool Flag)
        {
            //Initialize output variables
            snappedPoint = null;
            EID = -1;
            pFlagDisplay = null;
            int FCID = -1, FID = -1, subID = -1;

            IGeoDataset pDS = null;
            IPointToEID pointToEID = null;
            INetElements netElements = null;
            INetFlag junctionFlag = null;
            try
            {
                pFlagDisplay = null;

                pDS = gn.FeatureDataset as IGeoDataset;
                point.Project(pDS.SpatialReference);



                pointToEID = new PointToEIDClass() as IPointToEID;

                // find the nearest junction element to this Point
                pointToEID.GeometricNetwork = gn as IGeometricNetwork;
                pointToEID.SourceMap = map;
                pointToEID.SnapTolerance = snapTol;

                try
                {
                    pointToEID.GetNearestJunction(point, out EID, out snappedPoint);
                }
                catch (Exception ex)
                {

                }


                if (snappedPoint == null)
                    return null;

                // convert the EID to a feature class ID, feature ID, and sub ID
                netElements = gn.Network as INetElements;

                try
                {
                    netElements.QueryIDs(EID, esriElementType.esriETJunction, out FCID, out FID, out subID);

                }
                catch (Exception ex)
                {

                    return null;
                }

                //Create flag for start of trace
                junctionFlag = new JunctionFlagClass() as INetFlag;
                junctionFlag.UserClassID = FCID;
                junctionFlag.UserID = FID;
                junctionFlag.UserSubID = subID;

                if (junctionFlag is IEdgeFlag)
                {
                    pFlagDisplay = new EdgeFlagDisplayClass();

                    if (Flag)
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeFlag) as ISymbol;
                    else
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeBarrier) as ISymbol;
                }
                else
                {
                    pFlagDisplay = new JunctionFlagDisplayClass();
                    if (Flag)
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.JunctionFlag) as ISymbol;
                    else
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.JunctionBarrier) as ISymbol;
                }

                pFlagDisplay.ClientClassID = FCID;
                pFlagDisplay.FeatureClassID = FID;
                pFlagDisplay.SubID = subID;
                pFlagDisplay.Geometry = snappedPoint;

                return junctionFlag as IJunctionFlag;
            }
            catch
            {
                return null;
            }
            finally
            {
                pDS = null;
                pointToEID = null;
                netElements = null;

            }


        }
        public static IJunctionFlag GetJunctionFlag(double x, double y, ref  IMap map, ref List<IGeometricNetwork> gnList, double snapTol, ref int gnIdx, out IPoint snappedPoint, out int EID, out  IFlagDisplay pFlagDisplay, bool Flag)
        {

            //Initialize output variables
            snappedPoint = null; EID = -1;
            pFlagDisplay = null;

            IPoint point = null;
            IGeometricNetwork gn = null;

            IJunctionFlag junctionFlag = null;
            try
            {

                point = new PointClass() as IPoint;
                point.X = x;
                point.Y = y;
                point.SpatialReference = map.SpatialReference;
                point.SnapToSpatialReference();
                for (int i = 0; i < gnList.Count; i++)
                {
                    gn = gnList[i] as IGeometricNetwork;

                    junctionFlag = GetJunctionFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint, out EID, out  pFlagDisplay, Flag);


                    if (junctionFlag != null)
                    {
                        gnIdx = i;
                        return junctionFlag;

                    }
                }
                gnIdx = -1;
                return null;
            }
            catch
            {
                gnIdx = -1;
                return null;
            }
            finally
            {

                point = null;
                gn = null;

            }



        }
        public static IJunctionFlag GetJunctionFlag(ref IPoint point, ref  IMap map, ref List<IGeometricNetwork> gnList, double snapTol, ref int gnIdx, out IPoint snappedPoint, out int EID, out  IFlagDisplay pFlagDisplay, bool Flag)
        {

            //Initialize output variables
            snappedPoint = null; EID = -1;
            pFlagDisplay = null;


            IGeometricNetwork gn = null;

            IJunctionFlag junctionFlag = null;
            try
            {


                for (int i = 0; i < gnList.Count; i++)
                {
                    gn = gnList[i] as IGeometricNetwork;

                    junctionFlag = GetJunctionFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint, out EID, out  pFlagDisplay, Flag);


                    if (junctionFlag != null)
                    {
                        gnIdx = i;
                        return junctionFlag;

                    }
                }
                gnIdx = -1;
                return null;
            }
            catch
            {
                gnIdx = -1;
                return null;
            }
            finally
            {


                gn = null;

            }



        }
        public static IEdgeFlag GetEdgeFlagWithGN(double x, double y, ref  IMap map, ref IGeometricNetwork gn, double snapTol, out IPoint snappedPoint, out int EID, out double distanceAlong, out  IFlagDisplay pFlagDisplay, bool Flag)
        {
            //Initialize output variables
            pFlagDisplay = null;
            snappedPoint = null; EID = -1; distanceAlong = -1;
            IPoint point = null;
            try
            {
                // convert the (x,y) map coordinate to a new Point object
                point = new PointClass() as IPoint;
                point.X = x;
                point.Y = y;
                point.SpatialReference = map.SpatialReference;
                point.SnapToSpatialReference();

                return GetEdgeFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint, out EID, out distanceAlong, out  pFlagDisplay, Flag);
            }
            catch
            {
                return null;
            }
            finally
            {
                point = null;
            }
        }
        public static int getEIDAtLocation(ref IPoint point, ref IMap map, ref IGeometricNetwork gn, double snapTol)
        {

            //Initialize output variables

            IPoint snappedPoint = null;
            int EID = -1;
            double distanceAlong = -1;

            IGeoDataset pDS = null;
            IPointToEID pointToEID = null;

            // INetElements netElements = null;
            try
            {
                pDS = gn.FeatureDataset as IGeoDataset;
                point.Project(pDS.SpatialReference);


                pointToEID = new PointToEIDClass() as IPointToEID;


                // find the nearest junction element to this Point
                pointToEID.GeometricNetwork = gn;
                pointToEID.SourceMap = map;
                pointToEID.SnapTolerance = snapTol;
                try
                {
                    //pointToEID.GetNearestJunction(point, out EID, out snappedPoint);
                    pointToEID.GetNearestEdge(point, out EID, out snappedPoint, out distanceAlong);
                }
                catch (Exception ex)
                {

                }


                // convert the EID to a feature class ID, feature ID, and sub ID
                //netElements = gn.Network as INetElements;
                //int FCID = -1, FID = -1, subID = -1;
                //try
                //{
                //    netElements.QueryIDs(EID, esriElementType.esriETEdge, out FCID, out FID, out subID);
                //}
                //catch (Exception ex)
                //{


                //}
                return EID;


            }
            catch
            {
                return -1;
            }
            finally
            {
                pDS = null;
                pointToEID = null;

                // netElements = null;
            }


        }

        public static IEdgeFlag GetEdgeFlagWithGN(ref IPoint point, ref IMap map, ref IGeometricNetwork gn, double snapTol, out IPoint snappedPoint, out int EID, out double distanceAlong, out  IFlagDisplay pFlagDisplay, bool Flag)
        {

            //Initialize output variables
            pFlagDisplay = null;
            snappedPoint = null; EID = -1; distanceAlong = -1;

            IGeoDataset pDS = null;
            IPointToEID pointToEID = null;
            INetFlag edgeFlag = null;
            INetElements netElements = null;
            try
            {
                pDS = gn.FeatureDataset as IGeoDataset;
                point.Project(pDS.SpatialReference);


                pointToEID = new PointToEIDClass() as IPointToEID;


                // find the nearest junction element to this Point
                pointToEID.GeometricNetwork = gn;
                pointToEID.SourceMap = map;
                pointToEID.SnapTolerance = snapTol;
                try
                {
                    pointToEID.GetNearestEdge(point, out EID, out snappedPoint, out distanceAlong);
                }
                catch (Exception ex)
                {

                }

                if (snappedPoint == null) return null;

                // convert the EID to a feature class ID, feature ID, and sub ID
                netElements = gn.Network as INetElements;
                int FCID = -1, FID = -1, subID = -1;
                try
                {
                    netElements.QueryIDs(EID, esriElementType.esriETEdge, out FCID, out FID, out subID);
                }
                catch (Exception ex)
                {

                    return null;
                }

                //Create flag for start of trace
                edgeFlag = new EdgeFlagClass() as INetFlag;
                edgeFlag.UserClassID = FCID;
                edgeFlag.UserID = FID;
                edgeFlag.UserSubID = subID;


                if (edgeFlag is IEdgeFlag)
                {
                    pFlagDisplay = new EdgeFlagDisplayClass();

                    if (Flag)
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeFlag) as ISymbol;
                    else
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.EdgeBarrier) as ISymbol;
                }
                else
                {
                    pFlagDisplay = new JunctionFlagDisplayClass();
                    if (Flag)
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.JunctionFlag) as ISymbol;
                    else
                        pFlagDisplay.Symbol = CreateNetworkFlagBarrierSymbol(flagType.JunctionBarrier) as ISymbol;
                }
                pFlagDisplay.ClientClassID = FCID;
                pFlagDisplay.FeatureClassID = FID;
                pFlagDisplay.SubID = subID;
                pFlagDisplay.Geometry = snappedPoint;
                return edgeFlag as IEdgeFlag;
            }
            catch
            {
                return null;
            }
            finally
            {
                pDS = null;
                pointToEID = null;

                netElements = null;
            }


        }
        public static IEdgeFlag GetEdgeFlag(double x, double y, ref  IMap map, ref List<IGeometricNetwork> gnList, double snapTol, ref int gnIdx, out IPoint snappedPoint, out int EID, out double distanceAlong, out  IFlagDisplay pFlagDisplay, bool Flag)
        {

            //Initialize output variables
            snappedPoint = null; EID = -1; distanceAlong = -1;
            pFlagDisplay = null;

            IPoint point = null;
            IGeometricNetwork gn = null;

            IEdgeFlag edgeFlag = null;
            try
            {
                point = new PointClass() as IPoint;
                point.X = x;
                point.Y = y;
                point.SpatialReference = map.SpatialReference;

                for (int i = 0; i < gnList.Count; i++)
                {
                    gn = gnList[i] as IGeometricNetwork;

                    edgeFlag = GetEdgeFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint,
                                                 out EID, out distanceAlong, out  pFlagDisplay, Flag);


                    if (edgeFlag != null)
                    {
                        gnIdx = i;
                        return edgeFlag;

                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
            finally
            {

                point = null;
                gn = null;

            }



        }
        public static IEdgeFlag GetEdgeFlag(ref IPoint point, ref IMap map, ref List<IGeometricNetwork> gnList, double snapTol, ref int gnIdx, out IPoint snappedPoint, out int EID, out double distanceAlong, out  IFlagDisplay pFlagDisplay, bool Flag)
        {

            //Initialize output variables
            snappedPoint = null; EID = -1; distanceAlong = -1;
            pFlagDisplay = null;

            IGeometricNetwork gn = null;

            IEdgeFlag edgeFlag = null;
            try
            {

                for (int i = 0; i < gnList.Count; i++)
                {
                    gn = gnList[i] as IGeometricNetwork;

                    edgeFlag = GetEdgeFlagWithGN(ref point, ref map, ref gn, snapTol, out snappedPoint,
                                                 out EID, out distanceAlong, out  pFlagDisplay, Flag);


                    if (edgeFlag != null)
                    {
                        gnIdx = i;
                        return edgeFlag;

                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
            finally
            {


                gn = null;

            }



        }

        public static void AddFlagToTraceSolver(INetFlag netFlag, ref ITraceFlowSolverGEN traceFlowSolver, out IJunctionFlag junctionFlag, out IEdgeFlag edgeFlag)
        {
            IJunctionFlag[] junctionFlags = null;
            IEdgeFlag[] edgeFlags = null;
            junctionFlag = null;
            edgeFlag = null;
            try
            {
                //Add the flag to the trace solver
                junctionFlag = netFlag as IJunctionFlag;
                edgeFlag = netFlag as IEdgeFlag;
                if (junctionFlag != null)
                {
                    junctionFlags = new IJunctionFlag[1];
                    junctionFlags[0] = junctionFlag as IJunctionFlag;
                    try
                    {

                        traceFlowSolver.PutJunctionOrigins(ref junctionFlags);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if (edgeFlag != null)
                {
                    edgeFlags = new IEdgeFlag[1];
                    edgeFlags[0] = edgeFlag as IEdgeFlag;
                    try
                    {
                        traceFlowSolver.PutEdgeOrigins(ref edgeFlags);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch
            {

            }
            finally
            {
                junctionFlags = null;
                edgeFlags = null;
            }
        }
        public static void AddFlagsToTraceSolver(INetFlag[] netFlag, ref ITraceFlowSolverGEN traceFlowSolver, out IJunctionFlag[] junctionFlags, out IEdgeFlag[] edgeFlags)
        {
            ArrayList juncFlags = new ArrayList();
            ArrayList edFlags = new ArrayList();
            IJunctionFlag junctionFlag = null;
            IEdgeFlag edgeFlag = null;
            junctionFlags = null;
            edgeFlags = null;

            try
            {

                for (int i = 0; i < netFlag.Length; i++)
                {
                    //Add the flag to the trace solver
                    junctionFlag = netFlag[i] as IJunctionFlag;
                    edgeFlag = netFlag[i] as IEdgeFlag;
                    if (junctionFlag != null)
                    {
                        juncFlags.Add(junctionFlag);
                    }
                    else if (edgeFlag != null)
                    {
                        edFlags.Add(edgeFlag);
                    }
                }
                if (juncFlags.Count > 0)
                {

                    junctionFlags = new IJunctionFlag[juncFlags.Count];
                    for (int j = 0; j < juncFlags.Count; j++)
                    {
                        junctionFlags[j] = juncFlags[j] as IJunctionFlag;
                    }

                    traceFlowSolver.PutJunctionOrigins(ref junctionFlags);

                }
                else
                {
                    junctionFlags = null;
                }
                if (edFlags.Count > 0)
                {
                    edgeFlags = new IEdgeFlag[edFlags.Count];
                    for (int j = 0; j < edFlags.Count; j++)
                    {
                        edgeFlags[j] = edFlags[j] as IEdgeFlag;
                    }
                    traceFlowSolver.PutEdgeOrigins(ref edgeFlags);


                }
                else
                {
                    edgeFlags = null;
                }

            }
            catch { }
            finally
            {
                if (juncFlags != null)
                {
                    // Marshal.ReleaseComObject(juncFlags);

                }
                juncFlags = null;

                if (edFlags != null)
                {
                    // Marshal.ReleaseComObject(edFlags);

                }
                edFlags = null;

                if (junctionFlag != null)
                {
                    // Marshal.ReleaseComObject(junctionFlag);

                }
                junctionFlag = null;

                if (edgeFlag != null)
                {
                    //  Marshal.ReleaseComObject(edgeFlag);

                }
                edgeFlag = null;

            }

        }
        public static void AddBarriersToSolver(ref ITraceFlowSolverGEN traceFlowSolver, ref INetElementBarriers pEdgeElementBarriers,
            ref INetElementBarriers pJunctionElementBarriers, ref ISelectionSetBarriers pSelectionSetBarriers)//, INetElementBarriers closeValveBarr
        {
            INetSolver pNetSolver = null;
            try
            {
                pNetSolver = (INetSolver)traceFlowSolver;
                //if (closeValveBarr != null)
                //{
                //    pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, closeValveBarr);
                //}
                //else if (pEdgeElementBarriers != null)
                //{
                //    pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, pEdgeElementBarriers);
                //}
                if (pEdgeElementBarriers != null)
                {
                    pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, pEdgeElementBarriers);
                }
                if (pJunctionElementBarriers != null)
                    pNetSolver.set_ElementBarriers(esriElementType.esriETJunction, pJunctionElementBarriers);

                if (pSelectionSetBarriers != null)
                    pNetSolver.SelectionSetBarriers = pSelectionSetBarriers;

                // pNetSolver.set_ElementBarriers(esriElementType.esriETJunction, closeVal);

            }
            catch
            {

            }
            finally
            {
                pNetSolver = null;
            }
        }
        public static void AddFlagToGN(ref INetworkAnalysisExt pNetworkAnalysisExt, ref ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, IFlagDisplay pFlagDsiplay)
        {
            INetworkAnalysisExtFlags pNetworkAnalysisExtFlags = null;

            try
            {
                if (pNetworkAnalysisExt.CurrentNetwork != pGeomNet)
                {
                    pNetworkAnalysisExt.CurrentNetwork = pGeomNet;
                }


                pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetworkAnalysisExt;



                if (pFlagDsiplay is IEdgeFlagDisplay)
                {

                    pNetworkAnalysisExtFlags.AddEdgeFlag(pFlagDsiplay as IEdgeFlagDisplay);

                }
                else
                {

                    pNetworkAnalysisExtFlags.AddJunctionFlag(pFlagDsiplay as IJunctionFlagDisplay);
                }
            }
            catch
            {

            }
            finally
            {
                pNetworkAnalysisExtFlags = null;
            }


        }
        public static void AddFlagToGN(ref INetworkAnalysisExt pNetworkAnalysisExt, ref ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, ref  IFlagDisplay pFlagDsiplay)
        {
            INetworkAnalysisExtFlags pNetworkAnalysisExtFlags = null;

            try
            {
                if (pNetworkAnalysisExt.CurrentNetwork != pGeomNet)
                {
                    pNetworkAnalysisExt.CurrentNetwork = pGeomNet;
                }


                pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetworkAnalysisExt;



                if (pFlagDsiplay is IEdgeFlagDisplay)
                {

                    pNetworkAnalysisExtFlags.AddEdgeFlag(pFlagDsiplay as IEdgeFlagDisplay);

                }
                else
                {

                    pNetworkAnalysisExtFlags.AddJunctionFlag(pFlagDsiplay as IJunctionFlagDisplay);
                }
            }
            catch
            {

            }
            finally
            {
                pNetworkAnalysisExtFlags = null;
            }


        }

        public static void AddBarrierToGN(INetworkAnalysisExt pNetworkAnalysisExt, ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, IFlagDisplay pFlagDsiplay)
        {
            INetworkAnalysisExtBarriers pNetworkAnalysisExtBar = null;
            try
            {
                if (pNetworkAnalysisExt.CurrentNetwork != pGeomNet)
                {
                    pNetworkAnalysisExt.CurrentNetwork = pGeomNet;
                }


                pNetworkAnalysisExtBar = (INetworkAnalysisExtBarriers)pNetworkAnalysisExt;



                if (pFlagDsiplay is IEdgeFlagDisplay)
                {

                    pNetworkAnalysisExtBar.AddEdgeBarrier(pFlagDsiplay as IEdgeFlagDisplay);

                }
                else
                {

                    pNetworkAnalysisExtBar.AddJunctionBarrier(pFlagDsiplay as IJunctionFlagDisplay);
                }

            }
            catch { }
            finally
            {
                pNetworkAnalysisExtBar = null;
            }

        }
        public static void AddBarrierToGN(INetworkAnalysisExt pNetworkAnalysisExt, ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, ref IFlagDisplay pFlagDsiplay)
        {
            INetworkAnalysisExtBarriers pNetworkAnalysisExtBar = null;
            try
            {
                if (pNetworkAnalysisExt.CurrentNetwork != pGeomNet)
                {
                    pNetworkAnalysisExt.CurrentNetwork = pGeomNet;
                }


                pNetworkAnalysisExtBar = (INetworkAnalysisExtBarriers)pNetworkAnalysisExt;



                if (pFlagDsiplay is IEdgeFlagDisplay)
                {

                    pNetworkAnalysisExtBar.AddEdgeBarrier(pFlagDsiplay as IEdgeFlagDisplay);

                }
                else
                {

                    pNetworkAnalysisExtBar.AddJunctionBarrier(pFlagDsiplay as IJunctionFlagDisplay);
                }

            }
            catch { }
            finally
            {
                pNetworkAnalysisExtBar = null;
            }

        }
        public static void RemoveFlagBarrierAtLocation(double x, double y, ref INetworkAnalysisExt pNetworkAnalysisExt, double snapTol)
        {
            IPoint pPnt;
            try
            {
                pPnt = new PointClass();
                pPnt.X = x;
                pPnt.Y = y;
                RemoveFlagBarrierAtLocation(ref pPnt, ref pNetworkAnalysisExt, snapTol);
            }
            catch
            { }
            finally
            {
                pPnt = null;
            }
        }
        public static void RemoveFlagBarrierAtLocation(ref IPoint point, ref  INetworkAnalysisExt pNetworkAnalysisExt, double snapTol)
        {
            IProximityOperator pProx = null;
            INetworkAnalysisExtBarriers pNetworkAnalysisExtBarriers = null;
            INetworkAnalysisExtFlags pNetworkAnalysisExtFlags = null;
            IFlagDisplay pFlagDisplay = null;
            int lngFlagCount = 0;
            List<IFlagDisplay> pFlagsBarJunction = null;
            List<IFlagDisplay> pFlagsBarEdge = null;
            bool FlagFound = false;

            try
            {


                pProx = (IProximityOperator)point;





                pNetworkAnalysisExtBarriers = (INetworkAnalysisExtBarriers)pNetworkAnalysisExt;

                lngFlagCount = pNetworkAnalysisExtBarriers.JunctionBarrierCount;

                //only execute this next bit if there are junction flags
                if (lngFlagCount != 0)
                {  //redimension the array to hold the correct number of junction flags
                    pFlagsBarJunction = new List<IFlagDisplay>();

                    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                    for (int i = 0; i < lngFlagCount; i++)
                    {

                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_JunctionBarrier(i);
                        double distance;
                        //double distance = pProx.ReturnDistance(pFlagDisplay.Geometry);
                        IGeometry pTempGeo = pFlagDisplay.Geometry;
                        pTempGeo.Project(point.SpatialReference);

                        distance = pProx.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        if (distance > snapTol)
                            pFlagsBarJunction.Add(pFlagDisplay);
                        else
                            FlagFound = true;


                    }

                }

                lngFlagCount = pNetworkAnalysisExtBarriers.EdgeBarrierCount;

                //only execute this next bit if there are junction flags
                if (lngFlagCount != 0)
                {  //redimension the array to hold the correct number of junction flags
                    pFlagsBarEdge = new List<IFlagDisplay>();

                    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                    for (int i = 0; i < lngFlagCount; i++)
                    {

                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_EdgeBarrier(i);

                        double distance;
                        IGeometry pTempGeo = pFlagDisplay.Geometry;
                        pTempGeo.Project(point.SpatialReference);

                        distance = pProx.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        //double distance = pProx.ReturnDistance(pFlagDisplay.Geometry);

                        if (distance > snapTol)
                            pFlagsBarEdge.Add(pFlagDisplay);
                        else
                            FlagFound = true;

                    }

                }

                if (FlagFound)
                {
                    pNetworkAnalysisExtBarriers.ClearBarriers();
                    if (pFlagsBarEdge != null)
                    {
                        foreach (IFlagDisplay pFg in pFlagsBarEdge)
                        {
                            pNetworkAnalysisExtBarriers.AddEdgeBarrier((IEdgeFlagDisplay)pFg);
                        }
                    }
                    if (pFlagsBarJunction != null)
                    {

                        foreach (IFlagDisplay pFg in pFlagsBarJunction)
                        {
                            pNetworkAnalysisExtBarriers.AddJunctionBarrier((IJunctionFlagDisplay)pFg);
                        }

                    }

                }








                pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetworkAnalysisExt;

                lngFlagCount = pNetworkAnalysisExtFlags.JunctionFlagCount;
                FlagFound = false;

                //only execute this next bit if there are junction flags
                if (lngFlagCount != 0)
                {  //redimension the array to hold the correct number of junction flags
                    pFlagsBarJunction = new List<IFlagDisplay>();

                    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                    for (int i = 0; i < lngFlagCount; i++)
                    {

                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_JunctionFlag(i);
                        double distance;
                        //double distance = pProx.ReturnDistance(pFlagDisplay.Geometry);
                        IGeometry pTempGeo = pFlagDisplay.Geometry;
                        pTempGeo.Project(point.SpatialReference);

                        distance = pProx.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        if (distance > snapTol)
                            pFlagsBarJunction.Add(pFlagDisplay);
                        else
                            FlagFound = true;


                    }

                }

                lngFlagCount = pNetworkAnalysisExtFlags.EdgeFlagCount;

                //only execute this next bit if there are junction flags
                if (lngFlagCount != 0)
                {  //redimension the array to hold the correct number of junction flags
                    pFlagsBarEdge = new List<IFlagDisplay>();

                    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                    for (int i = 0; i < lngFlagCount; i++)
                    {

                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_EdgeFlag(i);
                        double distance;
                        //double distance = pProx.ReturnDistance(pFlagDisplay.Geometry);
                        IGeometry pTempGeo = pFlagDisplay.Geometry;
                        pTempGeo.Project(point.SpatialReference);

                        distance = pProx.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        if (distance > snapTol)
                            pFlagsBarEdge.Add(pFlagDisplay);
                        else
                            FlagFound = true;

                    }

                }

                if (FlagFound)
                {
                    pNetworkAnalysisExtFlags.ClearFlags();
                    if (pFlagsBarEdge != null)
                    {
                        foreach (IFlagDisplay pFg in pFlagsBarEdge)
                        {
                            pNetworkAnalysisExtFlags.AddEdgeFlag((IEdgeFlagDisplay)pFg);
                        }
                    }
                    if (pFlagsBarJunction != null)
                    {
                        foreach (IFlagDisplay pFg in pFlagsBarJunction)
                        {
                            pNetworkAnalysisExtFlags.AddJunctionFlag((IJunctionFlagDisplay)pFg);
                        }
                    }
                }
            }
            catch
            { }
            finally
            {

                pProx = null;
                pNetworkAnalysisExtBarriers = null;
                pNetworkAnalysisExtFlags = null;
                pFlagDisplay = null;

                pFlagsBarJunction = null;
                pFlagsBarEdge = null;
                FlagFound = false;
            }

        }
        public static IJunctionFlag[] junctionsToFlags(ref IEnumNetEID juncEIDs, ref IGeometricNetwork gn, ref IMap pMap, out  IFlagDisplay[] pFlagDisplay, bool Flag, double snapTol)
        {
            IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;
            IJunctionFlag junFlag = null;
            IJunctionFlag[] junctionFlags = null;
            IPoint pPnt = null;
            IPoint pRetPnt = null;
            pFlagDisplay = null;
            try
            {
                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.OutputSpatialReference = pMap.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = true;

                enumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);

                junctionFlags = new IJunctionFlag[juncEIDs.Count - 1];
                pFlagDisplay = new IFlagDisplay[juncEIDs.Count - 1];
                enumEidInfo.Reset();
                eidInfo = enumEidInfo.Next();
                int i = 0;
                while (eidInfo != null)
                {
                    pPnt = (IPoint)eidInfo.Geometry;
                    pRetPnt = null;
                    double SnapTol = 1.0;

                    int pOutEID;
                    junFlag = GetJunctionFlagWithGN(ref pPnt, ref pMap, ref gn, snapTol, out pRetPnt, out pOutEID, out  pFlagDisplay[i], Flag);
                    junctionFlags[i] = junFlag;


                    eidInfo = enumEidInfo.Next();
                    i++;
                }
            }
            catch
            { }
            finally
            {
                eidHelper = null;
                enumEidInfo = null;
                eidInfo = null;
                junFlag = null;
                junctionFlags = null;
                pPnt = null;
                pRetPnt = null;
            }


            return junctionFlags;


        }

        public static void AddTwoJunctionFlagsToTraceSolver(ref ITraceFlowSolverGEN traceFlowSolver, INetFlag netFlag1, INetFlag netFlag2)
        {
            IEdgeFlag EdgeFlag1 = null;
            IEdgeFlag EdgeFlag2 = null;
            IEdgeFlag[] EdgeFlags = null;
            IJunctionFlag JunctionFlag1 = null;
            IJunctionFlag JunctionFlag2 = null;
            IJunctionFlag[] JunctionFlags = null;
            try
            {
                if (netFlag1 is EdgeFlag && netFlag2 is EdgeFlag)
                {
                    EdgeFlag1 = netFlag1 as IEdgeFlag;

                    EdgeFlag2 = netFlag2 as IEdgeFlag;

                    try
                    {            //Add the flag to the trace solver

                        if (EdgeFlag1 != null && EdgeFlag2 != null)
                        {
                            EdgeFlags = new IEdgeFlag[2];
                            EdgeFlags[0] = EdgeFlag1 as IEdgeFlag;
                            EdgeFlags[1] = EdgeFlag2 as IEdgeFlag;
                            traceFlowSolver.PutEdgeOrigins(ref EdgeFlags);

                        }
                    }
                    catch { }
                    finally
                    {

                        //if (EdgeFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(EdgeFlags);

                        //}
                        //EdgeFlags = null;

                        //if (EdgeFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag1);

                        //}
                        //EdgeFlag1 = null;
                        //if (EdgeFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag2);

                        //}
                        //EdgeFlag2 = null;
                    }
                }
                else if (netFlag1 is EdgeFlag && netFlag2 is JunctionFlag)
                {
                    EdgeFlag1 = netFlag1 as IEdgeFlag;

                    JunctionFlag2 = netFlag2 as IJunctionFlag;

                    try
                    {            //Add the flag to the trace solver

                        if (EdgeFlag1 != null)
                        {
                            EdgeFlags = new IEdgeFlag[1];
                            EdgeFlags[0] = EdgeFlag1 as IEdgeFlag;

                            traceFlowSolver.PutEdgeOrigins(ref EdgeFlags);

                        }
                        if (JunctionFlag2 != null)
                        {
                            JunctionFlags = new IJunctionFlag[1];
                            JunctionFlags[0] = JunctionFlag2 as IJunctionFlag;

                            traceFlowSolver.PutJunctionOrigins(ref JunctionFlags);

                        }
                    }
                    catch { }
                    finally
                    {

                        //if (EdgeFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(EdgeFlags);

                        //}
                        //EdgeFlags = null;

                        //if (EdgeFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag1);

                        //}
                        //EdgeFlag1 = null;
                        //if (EdgeFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag2);

                        //}
                        //EdgeFlag2 = null;
                        //if (JunctionFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(junctionFlags);

                        //}
                        //JunctionFlags = null;

                        //if (JunctionFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag1);

                        //}
                        //JunctionFlag1 = null;
                        //if (JunctionFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag2);

                        //}
                        //JunctionFlag2 = null;
                    }

                }
                else if (netFlag1 is JunctionFlag && netFlag2 is EdgeFlag)
                {
                    EdgeFlag1 = netFlag2 as IEdgeFlag;

                    JunctionFlag2 = netFlag1 as IJunctionFlag;

                    try
                    {            //Add the flag to the trace solver

                        if (EdgeFlag1 != null)
                        {
                            EdgeFlags = new IEdgeFlag[1];
                            EdgeFlags[0] = EdgeFlag1 as IEdgeFlag;

                            traceFlowSolver.PutEdgeOrigins(ref EdgeFlags);

                        }
                        if (JunctionFlag2 != null)
                        {
                            JunctionFlags = new IJunctionFlag[1];
                            JunctionFlags[0] = JunctionFlag2 as IJunctionFlag;

                            traceFlowSolver.PutJunctionOrigins(ref JunctionFlags);

                        }
                    }
                    catch { }
                    finally
                    {

                        //if (EdgeFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(EdgeFlags);

                        //}
                        //EdgeFlags = null;

                        //if (EdgeFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag1);

                        //}
                        //EdgeFlag1 = null;
                        //if (EdgeFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(EdgeFlag2);

                        //}
                        //EdgeFlag2 = null;
                        //if (JunctionFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(junctionFlags);

                        //}
                        //JunctionFlags = null;

                        //if (JunctionFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag1);

                        //}
                        //JunctionFlag1 = null;
                        //if (JunctionFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag2);

                        //}
                        //JunctionFlag2 = null;
                    }
                }
                else
                {
                    JunctionFlag1 = netFlag1 as IJunctionFlag;
                    JunctionFlag2 = netFlag2 as IJunctionFlag;

                    try
                    {            //Add the flag to the trace solver

                        if (JunctionFlag1 != null && JunctionFlag2 != null)
                        {
                            JunctionFlags = new IJunctionFlag[2];
                            JunctionFlags[0] = JunctionFlag1 as IJunctionFlag;
                            JunctionFlags[1] = JunctionFlag2 as IJunctionFlag;
                            traceFlowSolver.PutJunctionOrigins(ref JunctionFlags);

                        }
                    }
                    catch { }
                    finally
                    {

                        //if (JunctionFlags != null)
                        //{
                        //    //  Marshal.ReleaseComObject(junctionFlags);

                        //}
                        //JunctionFlags = null;

                        //if (JunctionFlag1 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag1);

                        //}
                        //JunctionFlag1 = null;
                        //if (JunctionFlag2 != null)
                        //{
                        //    Marshal.ReleaseComObject(JunctionFlag2);

                        //}
                        //JunctionFlag2 = null;
                    }
                }
            }
            catch
            { }
            finally
            { }
        }

        public static Hashtable GetEIDInfoListByFC(int featureClassId, IEnumNetEID juncEIDs, IEIDHelper eidHelper)
        {
            Hashtable outputEIDInfoHT = new Hashtable();
            IEnumEIDInfo allEnumEidInfo = null;
            try
            {


                allEnumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);

                IEIDInfo testEidInfo = allEnumEidInfo.Next();
                while (testEidInfo != null)
                {
                    if (testEidInfo.Feature.Class.ObjectClassID == featureClassId)
                    {
                        outputEIDInfoHT.Add(testEidInfo.Feature.OID, testEidInfo);
                    }
                    testEidInfo = allEnumEidInfo.Next();
                }

                return outputEIDInfoHT;
            }
            catch (Exception ex)
            {

                return outputEIDInfoHT;
            }
            finally
            {
                if (allEnumEidInfo != null)
                {
                    Marshal.ReleaseComObject(allEnumEidInfo);
                }
                allEnumEidInfo = null;
                GC.Collect();
                GC.WaitForFullGCComplete(300);
            }

        }
        public static List<int> GetFeatureOIDList(int featureClassId, IEnumNetEID juncEIDs, IEIDHelper eidHelper)
        {
            List<int> sourceFeatureArrayList = new List<int>();
            IEnumEIDInfo sourceEnumEidInfo = null;
            try
            {

                sourceEnumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);
                IEIDInfo eidInfo = sourceEnumEidInfo.Next();

                while (eidInfo != null)
                {
                    if (eidInfo.Feature.Class.ObjectClassID == featureClassId)
                    {
                        sourceFeatureArrayList.Add(eidInfo.Feature.OID);
                    }
                    eidInfo = sourceEnumEidInfo.Next();
                }
                return sourceFeatureArrayList;
            }
            catch (Exception ex)
            {

                return sourceFeatureArrayList;
            }
            finally
            {
                if (sourceEnumEidInfo != null)
                {
                    Marshal.ReleaseComObject(sourceEnumEidInfo);
                }
                sourceEnumEidInfo = null;
                GC.Collect();
                GC.WaitForFullGCComplete(300);

            }
        }
        public static void getFlagsBarriers(IApplication app, out List<ESRI.ArcGIS.Geometry.IPoint> Flags, out List<ESRI.ArcGIS.Geometry.IPoint> Barriers)
        {
            Flags = new List<ESRI.ArcGIS.Geometry.IPoint>();
            Barriers = new List<ESRI.ArcGIS.Geometry.IPoint>();

            IMap map = null;
            List<IGeometricNetwork> gnList = null;

            IGeometricNetwork gn = null;
            INetworkAnalysisExt pNetAnalysisExt = null;
            IUtilityNetwork pUtilityNetwork = null;

            UID pID = null;

            INetworkAnalysisExtBarriers pNetworkAnalysisExtBarriers = null;
            INetworkAnalysisExtFlags pNetworkAnalysisExtFlags = null;

            IFlagDisplay pFlagDisplay = null;

            IClone pCl = null;

            int i;

            try
            {
                map = ((IMxDocument)app.Document).FocusMap;

                gnList = Globals.GetGeometricNetworksCurrentlyVisible(ref map);
                if (gnList.Count == 0)
                    return;

                if (gnList.Count > 0)
                    gn = gnList[0] as IGeometricNetwork;


                if (app != null)
                {


                    pID = new UID();

                    pID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                    pNetAnalysisExt = (INetworkAnalysisExt)app.FindExtensionByCLSID(pID);

                    pUtilityNetwork = (IUtilityNetwork)gn.Network;

                    pNetworkAnalysisExtBarriers = (INetworkAnalysisExtBarriers)pNetAnalysisExt;
                    //get the element barriers
                    if (pNetworkAnalysisExtBarriers.JunctionBarrierCount != 0)
                    {

                        for (i = 0; i < pNetworkAnalysisExtBarriers.JunctionBarrierCount; i++)
                        {
                            //assign to a local IFlagDisplay and IEdgeFlagDisplay variables
                            pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_JunctionBarrier(i);

                            pCl = (IClone)pFlagDisplay.Geometry;



                            Barriers.Add((IPoint)pCl.Clone());

                        }


                    }
                    if (pNetworkAnalysisExtBarriers.EdgeBarrierCount != 0)
                    {

                        for (i = 0; i < pNetworkAnalysisExtBarriers.EdgeBarrierCount; i++)
                        {
                            //assign to a local IFlagDisplay and IEdgeFlagDisplay variables
                            pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_EdgeBarrier(i);

                            pCl = (IClone)pFlagDisplay.Geometry;
                            Barriers.Add((IPoint)pCl.Clone());

                        }


                    }



                    pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetAnalysisExt;
                    //only execute this next bit if there are any edge flags
                    if (pNetworkAnalysisExtFlags.EdgeFlagCount != 0)
                    {

                        for (i = 0; i < pNetworkAnalysisExtFlags.EdgeFlagCount; i++)
                        {
                            //assign to a local IFlagDisplay and IEdgeFlagDisplay variables
                            pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_EdgeFlag(i);
                            pCl = (IClone)pFlagDisplay.Geometry;
                            Flags.Add((IPoint)pCl.Clone());

                        }


                    }

                    //next, get the junction flags

                    //only execute this next bit if there are junction flags
                    if (pNetworkAnalysisExtFlags.JunctionFlagCount != 0)
                    {  //redimension the array to hold the correct number of junction flags

                        //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                        for (i = 0; i < pNetworkAnalysisExtFlags.JunctionFlagCount; i++)
                        {
                            //assign to a local IFlagDisplay variable
                            pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_JunctionFlag(i);
                            pCl = (IClone)pFlagDisplay.Geometry;
                            Flags.Add((IPoint)pCl.Clone());

                        }

                    }

                }
            }
            catch
            {

            }
            finally
            {
                gnList = null;

                gn = null;
                pNetAnalysisExt = null;
                pID = null;


            }

        }
        public static ITraceFlowSolverGEN CreateTraceFlowSolverFromToolbar(ref INetworkAnalysisExt pNetworkAnalysisExt, out List<IEdgeFlag> pEdgeFlags, out List<IJunctionFlag> pJunctionFlags,
             out INetElementBarriers pEdgeElementBarriers, out INetElementBarriers pJunctionElementBarriers, out ISelectionSetBarriers pSelectionSetBarriers)
        {
            //    return CreateTraceFlowSolverFromToolbar(ref  pNetworkAnalysisExt, out pEdgeFlags, out  pJunctionFlags,
            //     out  pEdgeElementBarriers, out  pJunctionElementBarriers, out pSelectionSetBarriers,null);
            //}

            //public static ITraceFlowSolverGEN CreateTraceFlowSolverFromToolbar(ref INetworkAnalysisExt pNetworkAnalysisExt, out List<IEdgeFlag> pEdgeFlags, out List<IJunctionFlag> pJunctionFlags,
            //        out INetElementBarriers pEdgeElementBarriers, out INetElementBarriers pJunctionElementBarriers, out ISelectionSetBarriers pSelectionSetBarriers, INetElementBarriers inBarr = null)
            //{
            //out List<IEdgeFlag> pEdgeFlagsBar, out List<IJunctionFlag> pJunctionFlagsBar
            pEdgeFlags = new List<IEdgeFlag>();
            pJunctionFlags = new List<IJunctionFlag>();

            //pJunctionFlags = null;
            pEdgeElementBarriers = null;
            pJunctionElementBarriers = null;
            pSelectionSetBarriers = null;

            // pEdgeFlagsBar = null;
            //  pJunctionFlagsBar = null;

            IGeometricNetwork pGeometricNetwork = null;
            IUtilityNetwork pUtilityNetwork = null;
            INetSolver pNetSolver = null;
            INetworkAnalysisExtBarriers pNetworkAnalysisExtBarriers = null;
            INetworkAnalysisExtFlags pNetworkAnalysisExtFlags = null;

            IFeatureLayer pFeatureLayer = null;
            IFlagDisplay pFlagDisplay = null;
            IEdgeFlagDisplay pEdgeFlagDisplay = null;
            //IEdgeFlag[] pEdgeFlags= null;
            //IJunctionFlag[] pJunctionFlags= null;
            INetFlag pNetFlag = null;
            IEdgeFlag pEdgeFlag = null;
            ITraceFlowSolverGEN pTraceFlowSolver = null;
            ITraceTasks pTraceTasks = null;
            INetworkAnalysisExtWeightFilter pNetworkAnalysisExtWeightFilter = null;
            INetSchema pNetSchema = null;
            INetWeight pNetWeight = null;
            esriWeightFilterType eWeightFilterType = esriWeightFilterType.esriWFNone;
            INetSolverWeightsGEN pNetSolverWeights = null;
            System.Object[] lngFromValues = null;
            System.Object[] lngToValues = null;
            int lngFeatureLayerCount;
            int lngEdgeFlagCount;
            int lngJunctionFlagCount;
            bool binFeatureLayerDisabled;
            bool blnApplyNotOperator;
            int lngFilterRangeCount = 0;
            int i = 0;
            try
            {
                //get a reference to the IUtilityNetwork interface corresponding to the current network
                //QI for the INetworkAnalysisExt interface using IUtilityNetworkAnalysisExt

                // pNetworkAnalysisExt =(INetworkAnalysisExt)m_pUtilityNetworkAnalysisExt;

                //assign the current geometric network to a local IGeometricNetwork variable
                pGeometricNetwork = pNetworkAnalysisExt.CurrentNetwork;
                //assign the network to a local IUtilityNetwork variable
                //the Network property returns an INetwork interface, but this step performs
                //a QI for the IUtilityNetwork interface using INetwork
                pUtilityNetwork = (IUtilityNetwork)pGeometricNetwork.Network;


                //initialize the trace flow solver
                //co-create a new TraceFlowSolver object
                //this statement simultaneously co-creates a TraceFlowSolver object and performs
                //QI for the INetSolver interface
                pNetSolver = new TraceFlowSolverClass();
                // the source network for the solver
                pNetSolver.SourceNetwork = pUtilityNetwork;

                //get the barriers for the network, using the barriers that have been added
                //using the user interface
                //QI for the INetworkAnalysisExtBarriers interface using IUtilityNetworkAnalysisExt
                pNetworkAnalysisExtBarriers = (INetworkAnalysisExtBarriers)pNetworkAnalysisExt;
                //get the element barriers
                pNetworkAnalysisExtBarriers.CreateElementBarriers(out pJunctionElementBarriers, out  pEdgeElementBarriers);


                //get the selection set barriers
                pNetworkAnalysisExtBarriers.CreateSelectionBarriers(out pSelectionSetBarriers);
                //if (pSelectionSetBarriers == null)
                //{
                //    pSelectionSetBarriers = new SelectionSetBarriersClass();

                //}

                pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, pEdgeElementBarriers);

                //pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, inBarr);
                //pEdgeElementBarriers = inBarr;

                pNetSolver.set_ElementBarriers(esriElementType.esriETJunction, pJunctionElementBarriers);
                pNetSolver.SelectionSetBarriers = pSelectionSetBarriers;

                //lngEdgeFlagCount = pNetworkAnalysisExtBarriers.EdgeBarrierCount;
                //if (lngEdgeFlagCount != 0)
                //{
                //    //redimension the array to hold the correct number of edge flags
                //    // pEdgeFlags = new IEdgeFlag[lngEdgeFlagCount ];
                //    pEdgeFlagsBar = new List<IEdgeFlag>();

                //    for (i = 0; i < lngEdgeFlagCount; i++)
                //    {
                //        //assign to a local IFlagDisplay and IEdgeFlagDisplay variables
                //        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_EdgeBarrier(i);
                //        pEdgeFlagDisplay = (IEdgeFlagDisplay)pFlagDisplay;
                //        //co-create a new EdgeFlag object
                //        pNetFlag = new EdgeFlagClass();
                //        pEdgeFlag = (IEdgeFlag)pNetFlag;
                //        //assign the properties of the EdgeFlagDisplay object to the EdgeFlag object
                //        pEdgeFlag.Position = (float)pEdgeFlagDisplay.Percentage;
                //        pNetFlag.UserClassID = pFlagDisplay.FeatureClassID;
                //        pNetFlag.UserID = pFlagDisplay.FID;
                //        pNetFlag.UserSubID = pFlagDisplay.SubID;
                //        //add the new EdgeFlag object to the array
                //        pEdgeFlagsBar.Add((IEdgeFlag)pNetFlag);
                //    }
                //    ////add the edge flags to the network solver
                //    //IEdgeFlag[] pEF = pEdgeFlags.ToArray();

                //pNetSolver.set_ElementBarriers(esriElementType.esriETEdge, pEdgeElementBarriers);
                //}


                //lngJunctionFlagCount = pNetworkAnalysisExtBarriers.JunctionBarrierCount;
                ////only execute this next bit if there are junction flags
                //if (lngJunctionFlagCount != 0)
                //{  //redimension the array to hold the correct number of junction flags
                //    pJunctionFlagsBar = new List<IJunctionFlag>();

                //    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                //    for (i = 0; i < lngJunctionFlagCount; i++)
                //    {
                //        //assign to a local IFlagDisplay variable
                //        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtBarriers.get_JunctionBarrier(i);
                //        //co-create a new JunctionFlag object
                //        pNetFlag = new JunctionFlagClass();
                //        //assign the properties of the JunctionFlagDisplay object to the JunctionFlag object
                //        pNetFlag.UserClassID = pFlagDisplay.FeatureClassID;
                //        pNetFlag.UserID = pFlagDisplay.FID;
                //        pNetFlag.UserSubID = pFlagDisplay.SubID;
                //        //add the new junction flag to the array of junction flags
                //        pJunctionFlagsBar.Add( (IJunctionFlag)pNetFlag);
                //    }
                //  //  IJunctionFlag[] pJF = pJunctionFlags.ToArray();
                //    pNetSolver.set_ElementBarriers(esriElementType.esriETJunction, pJunctionElementBarriers);
                //    //add the junction flags to the network solver
                //   // pTraceFlowSolver.PutJunctionOrigins(ref pJF);
                //}







                //set up the disabled layers for the network solver
                //for each feature layer belonging to this network, determine if it is enabled
                //or disabled; if it//s disabled, then notify the network solver
                //determine the number of feature layers belonging to the current network
                lngFeatureLayerCount = pNetworkAnalysisExt.FeatureLayerCount;

                for (i = 0; i < lngFeatureLayerCount; i++)
                {
                    //get the next feature layer, and determine if it is disabled
                    //assign the feature layer to a local IFeatureLayer variable
                    pFeatureLayer = pNetworkAnalysisExt.get_FeatureLayer(i);
                    //determine if the feature layer is disabled
                    binFeatureLayerDisabled = pNetworkAnalysisExtBarriers.GetDisabledLayer(pFeatureLayer);
                    //if the feature layer is disabled, then notify the network solver
                    if (binFeatureLayerDisabled)
                        pNetSolver.DisableElementClass(pFeatureLayer.FeatureClass.FeatureClassID);

                }

                //set up the weight filters for the network
                //QI for the INetworkAnalysisExtWeightFilter interface using
                //IUtilityNetworkAnalysisExt
                pNetworkAnalysisExtWeightFilter = (INetworkAnalysisExtWeightFilter)pNetworkAnalysisExt;
                //QI for the INetSolverWeights interface using INetSolver
                pNetSolverWeights = (INetSolverWeightsGEN)pNetSolver;
                //QI for the INetSchema interface using IUtilityNetwork
                pNetSchema = (INetSchema)pUtilityNetwork;


                //create the junction weight filter
                lngFilterRangeCount = pNetworkAnalysisExtWeightFilter.get_FilterRangeCount(esriElementType.esriETJunction);
                if (lngFilterRangeCount > 0)
                {
                    //get a NetWeight object from the INetSchema interface
                    pNetWeight = pNetSchema.get_WeightByName(pNetworkAnalysisExtWeightFilter.JunctionWeightFilterName);
                    //get the type and Not operator status from the INetworkAnalysisExtWeightFilter interface
                    pNetworkAnalysisExtWeightFilter.GetFilterType(esriElementType.esriETJunction, out eWeightFilterType, out blnApplyNotOperator);
                    //add the weight filter to the network solver
                    pNetSolverWeights.JunctionFilterWeight = pNetWeight;
                    pNetSolverWeights.SetFilterType(esriElementType.esriETJunction, eWeightFilterType, blnApplyNotOperator);
                    //redimension the weight filter ranges arrays and get the ranges
                    lngFromValues = new System.Object[lngFilterRangeCount - 1];

                    lngToValues = new System.Object[lngFilterRangeCount - 1];
                    for (i = 0; i < lngFilterRangeCount; i++)
                    {
                        pNetworkAnalysisExtWeightFilter.GetFilterRange(esriElementType.esriETJunction, i, out lngFromValues[i], out lngToValues[i]);
                    }
                    //add the filter ranges to the network solver
                    pNetSolverWeights.SetFilterRanges(esriElementType.esriETJunction, ref lngFromValues, ref lngToValues);
                }

                //create the edge weight filters
                lngFilterRangeCount = pNetworkAnalysisExtWeightFilter.get_FilterRangeCount(esriElementType.esriETEdge);
                if (lngFilterRangeCount > 0)
                {
                    //get the type and Not operator status from the INetworkAnalysisExtWeightFilter interface
                    pNetworkAnalysisExtWeightFilter.GetFilterType(esriElementType.esriETEdge, out eWeightFilterType, out blnApplyNotOperator);

                    //get a NetWeight object from the INetSchema interface
                    pNetWeight = pNetSchema.get_WeightByName(pNetworkAnalysisExtWeightFilter.FromToEdgeWeightFilterName);
                    //add the weight filter to the network solver
                    pNetSolverWeights.FromToEdgeFilterWeight = pNetWeight;

                    //get a NetWeight object from the INetSchema interface
                    pNetWeight = pNetSchema.get_WeightByName(pNetworkAnalysisExtWeightFilter.ToFromEdgeWeightFilterName);
                    //add the weight filter to the network solver
                    pNetSolverWeights.ToFromEdgeFilterWeight = pNetWeight;

                    //get the filter ranges and apply them to the network solver
                    //redimension the weight filter ranges arrays and get the ranges
                    lngFromValues = new System.Object[lngFilterRangeCount - 1];

                    lngToValues = new System.Object[lngFilterRangeCount - 1];
                    for (i = 0; i < lngFilterRangeCount; i++)
                    {
                        pNetworkAnalysisExtWeightFilter.GetFilterRange(esriElementType.esriETEdge, i, out  lngFromValues[i], out lngToValues[i]);
                    }
                    pNetSolverWeights.SetFilterType(esriElementType.esriETEdge, eWeightFilterType, blnApplyNotOperator);
                    pNetSolverWeights.SetFilterRanges(esriElementType.esriETEdge, ref lngFromValues, ref lngToValues);


                }

                //assign the flags to the network solver
                //first, get the edge flags
                //QI for the ITraceFlowSolver interface using INetSolver interface
                pTraceFlowSolver = (ITraceFlowSolverGEN)pNetSolver;
                //QI for the INetworkAnalysisExtFlags interface using IUtilityNetworkAnalysisExt
                pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetworkAnalysisExt;
                //determine the number of edge flags on the current network
                lngEdgeFlagCount = pNetworkAnalysisExtFlags.EdgeFlagCount;
                //only execute this next bit if there are any edge flags
                if (lngEdgeFlagCount != 0)
                {
                    //redimension the array to hold the correct number of edge flags
                    // pEdgeFlags = new IEdgeFlag[lngEdgeFlagCount ];

                    for (i = 0; i < lngEdgeFlagCount; i++)
                    {
                        //assign to a local IFlagDisplay and IEdgeFlagDisplay variables
                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_EdgeFlag(i);
                        pEdgeFlagDisplay = (IEdgeFlagDisplay)pFlagDisplay;
                        //co-create a new EdgeFlag object
                        pNetFlag = new EdgeFlagClass();
                        pEdgeFlag = (IEdgeFlag)pNetFlag;
                        //assign the properties of the EdgeFlagDisplay object to the EdgeFlag object
                        pEdgeFlag.Position = (float)pEdgeFlagDisplay.Percentage;
                        pNetFlag.UserClassID = pFlagDisplay.FeatureClassID;
                        pNetFlag.UserID = pFlagDisplay.FID;
                        pNetFlag.UserSubID = pFlagDisplay.SubID;
                        //add the new EdgeFlag object to the array
                        pEdgeFlags.Add((IEdgeFlag)pNetFlag);
                    }
                    //add the edge flags to the network solver
                    IEdgeFlag[] pEF = pEdgeFlags.ToArray();


                    pTraceFlowSolver.PutEdgeOrigins(ref pEF);

                }

                //next, get the junction flags
                //determine the number of junction flags on the network
                lngJunctionFlagCount = pNetworkAnalysisExtFlags.JunctionFlagCount;
                //only execute this next bit if there are junction flags
                if (lngJunctionFlagCount != 0)
                {  //redimension the array to hold the correct number of junction flags

                    //ReDim pJunctionFlags(0 To lngJunctionFlagCount - 1)
                    for (i = 0; i < lngJunctionFlagCount; i++)
                    {
                        //assign to a local IFlagDisplay variable
                        pFlagDisplay = (IFlagDisplay)pNetworkAnalysisExtFlags.get_JunctionFlag(i);
                        //co-create a new JunctionFlag object
                        pNetFlag = new JunctionFlagClass();
                        //assign the properties of the JunctionFlagDisplay object to the JunctionFlag object
                        pNetFlag.UserClassID = pFlagDisplay.FeatureClassID;
                        pNetFlag.UserID = pFlagDisplay.FID;
                        pNetFlag.UserSubID = pFlagDisplay.SubID;
                        //add the new junction flag to the array of junction flags
                        pJunctionFlags.Add((IJunctionFlag)pNetFlag);
                    }
                    IJunctionFlag[] pJF = pJunctionFlags.ToArray();

                    //add the junction flags to the network solver
                    pTraceFlowSolver.PutJunctionOrigins(ref pJF);
                }

                //set the option for tracing on indeterminate flow
                //QI for the ITraceTasks interface using IUtilityNetworkAnalysisExt
                pTraceTasks = (ITraceTasks)pNetworkAnalysisExt;
                pTraceFlowSolver.TraceIndeterminateFlow = pTraceTasks.TraceIndeterminateFlow;

                //pass the TraceFlowSolver object back the network solver
                return pTraceFlowSolver;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

                pGeometricNetwork = null;
                pUtilityNetwork = null;
                pNetSolver = null;
                pNetworkAnalysisExtBarriers = null;
                pNetworkAnalysisExtFlags = null;
                //  pEdgeElementBarriers = null;
                //  pJunctionElementBarriers = null;
                // pSelectionSetBarriers = null;
                pFeatureLayer = null;
                pFlagDisplay = null;
                pEdgeFlagDisplay = null;
                //  pEdgeFlags = null;
                //  pJunctionFlags = null;
                pNetFlag = null;
                pEdgeFlag = null;
                pTraceFlowSolver = null;
                pTraceTasks = null;
                pNetworkAnalysisExtWeightFilter = null;
                pNetSchema = null;
                pNetWeight = null;

                pNetSolverWeights = null;
                lngFromValues = null;
                lngToValues = null;

            }

        }

        public static string SelectJunctions(ref IMap map, ref  IGeometricNetwork gn, ref IEnumNetEID juncEIDs, ref IJunctionFlag[] junctionFlag, string MeterName, string MeterCritField, string MeterCritValue, bool selectJunc)
        {
            List<int> pOIDs = null;
            IFeatureLayer fLayer = null;
            IDataset dataset = null;
            IFeatureSelection featSel = null;
            ISelectionSet selectionSet = null;
            IActiveView av = null;
            IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;
            UID pId = null;
            IEnumLayer enumLayer = null;
            ILayer layer = null;
            try
            {
                pOIDs = new List<int>();
                av = (IActiveView)map;
                if (junctionFlag != null)
                {
                    foreach (IJunctionFlag pNF in junctionFlag)
                    {
                        pOIDs.Add(((INetFlag)pNF).UserID);

                    }
                }


                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = false;

                enumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);

                pId = new UIDClass();
                pId.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(pId, true);

                enumLayer.Reset();
                layer = enumLayer.Next();
                int MeterCount = 0;
                int MeterCritical = 0;
                int MeterFldLoc = -1;
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    dataset = (IDataset)fLayer;
                    //&& fLayer.Selectable
                    if (fLayer.Valid &&
                         IsInNetwork(fLayer.FeatureClass.FeatureClassID, gn, false))
                    {
                        if (selectJunc)
                            selectionSet = fLayer.FeatureClass.Select(null, esriSelectionType.esriSelectionTypeHybrid, esriSelectionOption.esriSelectionOptionEmpty, dataset.Workspace);

                        enumEidInfo.Reset();
                        eidInfo = enumEidInfo.Next();
                        if (fLayer.Name == MeterName)
                        {
                            MeterFldLoc = fLayer.FeatureClass.Fields.FindField(MeterCritField);

                        }
                        while (eidInfo != null)
                        {
                            if (eidInfo.Feature.Class.ObjectClassID == fLayer.FeatureClass.FeatureClassID)
                            {
                                if (junctionFlag == null)
                                {
                                    if (fLayer.Name == MeterName)
                                    {
                                        MeterCount = MeterCount + 1;
                                        if (MeterFldLoc > 0)
                                        {
                                            string meterVal = eidInfo.Feature.get_Value(MeterFldLoc).ToString();

                                            if (MeterCritValue == meterVal)
                                            {
                                                MeterCritical = MeterCritical + 1;
                                            }
                                        }
                                    }
                                    if (selectJunc)
                                        selectionSet.Add(eidInfo.Feature.OID);
                                }
                                else if (pOIDs.Contains(eidInfo.Feature.OID) == false)
                                {
                                    if (fLayer.Name == MeterName && MeterFldLoc > 0)
                                    {
                                        MeterCount = MeterCount + 1;
                                        string meterVal = eidInfo.Feature.get_Value(MeterFldLoc).ToString();

                                        if (MeterCritValue == meterVal)
                                        {
                                            MeterCritical = MeterCritical + 1;
                                        }
                                    }
                                    if (selectJunc)
                                        selectionSet.Add(eidInfo.Feature.OID);
                                }
                            }

                            eidInfo = enumEidInfo.Next();
                        }
                        if (selectJunc)
                        {
                            featSel = (IFeatureSelection)fLayer;
                            if (selectionSet != null && selectionSet.Count > 0)
                            {
                                featSel.SelectionSet = selectionSet;

                            }
                        }
                    }
                    layer = enumLayer.Next();
                }
                return MeterCount.ToString() + "_" + MeterCritical.ToString();
                //if (refreshNeeded && DrawResults)
                //{
                //    av.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                //}
            }
            catch
            {
                return "";
            }
            finally
            {
                if (enumLayer != null)
                    Marshal.ReleaseComObject(enumLayer);
                enumLayer = null;
                pOIDs = null;
                fLayer = null;
                dataset = null;
                featSel = null;
                selectionSet = null;
                av = null;
                eidHelper = null;
                enumEidInfo = null;
                eidInfo = null;
                pId = null;

                layer = null;
            }


        }
        public static void SelectEdges(ref IMap map, ref IGeometricNetwork gn, ref IEnumNetEID edgeEIDs)
        {
            List<int> pOIDs = null;
            IFeatureLayer fLayer = null;
            IDataset dataset = null;
            IFeatureSelection featSel = null;
            ISelectionSet selectionSet = null;
            IActiveView av = null;
            IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;
            UID pId = null;
            IEnumLayer enumLayer = null;
            ILayer layer = null;
            try
            {

                pOIDs = new List<int>();
                av = (IActiveView)map;

                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = false;

                enumEidInfo = eidHelper.CreateEnumEIDInfo(edgeEIDs);


                pId = new UIDClass();
                pId.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(pId, true);

                enumLayer.Reset();
                layer = enumLayer.Next();
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    dataset = (IDataset)fLayer;
                    if (fLayer.Valid &&
                         fLayer.Visible &&
                         fLayer.Selectable &&
                         IsInNetwork(fLayer.FeatureClass.FeatureClassID, gn, false))
                    {

                        selectionSet = fLayer.FeatureClass.Select(null, esriSelectionType.esriSelectionTypeHybrid, esriSelectionOption.esriSelectionOptionEmpty, dataset.Workspace);

                        enumEidInfo.Reset();
                        eidInfo = enumEidInfo.Next();
                        while (eidInfo != null)
                        {
                            if (eidInfo.Feature.Class.ObjectClassID == fLayer.FeatureClass.FeatureClassID)
                            {

                                selectionSet.Add(eidInfo.Feature.OID);
                            }
                            eidInfo = enumEidInfo.Next();
                        }
                        featSel = (IFeatureSelection)fLayer;
                        if (selectionSet != null && selectionSet.Count > 0)
                        {
                            featSel.SelectionSet = selectionSet;

                        }

                    }
                    layer = enumLayer.Next();
                }

            }
            catch
            { }
            finally
            {
                pOIDs = null;
                fLayer = null;
                dataset = null;
                featSel = null;
                selectionSet = null;
                av = null;
                eidHelper = null;
                enumEidInfo = null;
                eidInfo = null;
                pId = null;
                enumLayer = null;
                layer = null;
            }

        }
        public static IEnvelope DrawEdges(ref IMap map, ref  IGeometricNetwork gn, ref IEnumNetEID edgeEIDs)
        {

            IEnvelope env = null;
            IActiveView av = null;
            IGraphicsContainer graphics = null;
            ILineSymbol symb = null;
            ILineElement lineElem = null;
            IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;
            IGeometry geom = null;
            IElement elem = null;
            IElementProperties elemProp = null;
            IRelationalOperator relOp = null;


            try
            {

                env = new EnvelopeClass();
                av = (IActiveView)map;
                graphics = (IGraphicsContainer)map;

                symb = new SimpleLineSymbolClass();
                lineColors = ConfigUtil.GetConfigValue("traceResultsLineColor", "255,0,0");
                string[] strColors = lineColors.Split(',');

                symb.Color = GetColor(Convert.ToInt32(strColors[0]), Convert.ToInt32(strColors[1]), Convert.ToInt32(strColors[2]));
                symb.Width = 2;
                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = true;
                eidHelper.PartialComplexEdgeGeometry = true;

                enumEidInfo = eidHelper.CreateEnumEIDInfo(edgeEIDs);



                enumEidInfo.Reset();
                eidInfo = enumEidInfo.Next();
                while (eidInfo != null)
                {
                    geom = eidInfo.Geometry;
                    if (geom != null)
                    {

                        //geom = eidInfo.Feature.ShapeCopy;
                        env.Union(geom.Envelope);

                        elem = new LineElementClass();
                        elem.Geometry = geom;
                        lineElem = (ILineElement)elem;
                        lineElem.Symbol = symb;
                        elemProp = (IElementProperties)elem;
                        elemProp.Name = "TraceResults";
                        graphics.AddElement(elem, 0);

                    }
                    eidInfo = enumEidInfo.Next();
                }
                env.Expand(1.1, 1.1, true);

                return env;
            }
            catch
            {
                return null;
            }
            finally
            {
                env = null;
                av = null;
                graphics = null;
                symb = null;
                lineElem = null;
                eidHelper = null;
                enumEidInfo = null;
                eidInfo = null;
                geom = null;
                elem = null;
                elemProp = null;
                relOp = null;
            }

        }
        public static IPolyline MergeEdges(ref IMap map, ref  IGeometricNetwork gn, ref IEnumNetEID edgeEIDs, ref IFeatureLayer mainsFL, out List<int> oids)
        {


            IGeometry geom = null;
            IGeometryCollection geometryBag = new GeometryBagClass();
            object Missing = Type.Missing;
            ESRI.ArcGIS.Geometry.ITopologicalOperator topologicalOperator = null;
            IEIDHelper eidHelper = null;
            IEnumEIDInfo enumEidInfo = null;
            IEIDInfo eidInfo = null;
            oids = new List<int>();
            try
            {

                eidHelper = new EIDHelperClass();
                eidHelper.GeometricNetwork = gn;
                eidHelper.OutputSpatialReference = map.SpatialReference;
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = true;
                eidHelper.PartialComplexEdgeGeometry = true;

                enumEidInfo = eidHelper.CreateEnumEIDInfo(edgeEIDs);
                enumEidInfo.Reset();
                eidInfo = enumEidInfo.Next();
                topologicalOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator)new PolylineClass();
                while (eidInfo != null)
                {

                    //if (eidInfo.Feature != null)
                    //{
                    //    if (mainsFL.FeatureClass.ObjectClassID.ToString() == eidInfo.Feature.Class.ObjectClassID.ToString() &&
                    //   mainsFL.FeatureClass.CLSID.Value.ToString() == eidInfo.Feature.Class.CLSID.Value.ToString())
                    //    {
                    //        if (oids.Contains(eidInfo.Feature.OID) == false)
                    //        {
                    //            oids.Add(eidInfo.Feature.OID);
                    //            if (topologicalOperator == null)
                    //            {
                    //                topologicalOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator)eidInfo.Feature.ShapeCopy;
                    //            }
                    //            else {
                    //                topologicalOperator.Union(eidInfo.Feature.ShapeCopy as IGeometry);
                    //            }

                    //            //geometryBag.AddGeometry(eidInfo.Feature.ShapeCopy as IGeometry, ref Missing, ref Missing);
                    //        }
                    //    }
                    //}

                    geom = eidInfo.Geometry;
                    if (geom != null)
                    {
                        if (mainsFL.FeatureClass.ObjectClassID.ToString() == eidInfo.Feature.Class.ObjectClassID.ToString() &&
                           mainsFL.FeatureClass.CLSID.Value.ToString() == eidInfo.Feature.Class.CLSID.Value.ToString())
                        {
                            if (oids.Contains(eidInfo.Feature.OID) == false)
                            {
                                oids.Add(eidInfo.Feature.OID);
                            }
                            geometryBag.AddGeometry(copyGeometry(geom, true), ref Missing, ref Missing);
                            //if (topologicalOperator == null)
                            //{

                            //    topologicalOperator = (ESRI.ArcGIS.Geometry.ITopologicalOperator)copyPolyline((IPolyline)eidInfo.Geometry);
                            //}
                            //else
                            //{
                            //    topologicalOperator.Union(copyPolyline((IPolyline)eidInfo.Geometry));
                            //}

                        }
                    }
                    eidInfo = enumEidInfo.Next();
                }
                IEnumGeometry enumGeometry = geometryBag as IEnumGeometry;

                topologicalOperator.ConstructUnion(enumGeometry);
                //topologicalOperator.Simplify();

                IPolyline pline = (IPolyline)topologicalOperator;

                return pline;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine(Ex.Message);

                System.Diagnostics.Trace.WriteLine(Ex.Message);

                return null;
            }
            finally
            {

                eidHelper = null;
                enumEidInfo = null;
                eidInfo = null;
                geom = null;

            }

        }
        public static IPolyline copyPolyline(IPolyline pPoly)
        {


            IObjectCopy objectCopy;
            IPolyline newPolyline;
            objectCopy = new ObjectCopy();
            newPolyline = (IPolyline)objectCopy.Copy(pPoly);
            return newPolyline;

        }
        public static IGeometry copyGeometry(IGeometry pGeo, bool removeMZ)
        {
            IZAware pZAware;
            IMAware pMAware;
            //IClone pClone = null;
            //pClone = (IClone)geom;
            //pZAware = (IZAware)pClone.Clone();
            //pZAware.ZAware = false;
            //pMAware = (IMAware)pZAware;
            //pMAware.MAware = false;

            IObjectCopy objectCopy;
            IGeometry newGeo;
            objectCopy = new ObjectCopy();
            newGeo = (IGeometry)objectCopy.Copy(pGeo);
            if (removeMZ)
            {
                pZAware = (IZAware)newGeo;
                pZAware.ZAware = false;
                pMAware = (IMAware)pZAware;
                pMAware.MAware = false;
            }
            return newGeo;

        }
        # endregion

        #region IOSerializationXML
        public static StreamWriter createTextFile(string sFileName, FileMode flMode)
        {
            // create the filename object - the while loop allows
            // us to keep trying with different filenames until
            // we succeed
            StreamWriter sw = null;
            string origname = sFileName;
            System.IO.FileStream fs = null;
            try
            {

                // open file for writing; throw an exception if the
                // file already exists:
                //   FileMode.CreateNew to create a file if it
                //                   doesn't already exist or throw
                //                   an exception if file exists
                //   FileMode.Append to create a new file or append
                //                   to an existing file
                //   FileMode.Create to create a new file or
                //                   truncate an existing file

                //   FileAccess possibilities are:
                //                   FileAccess.Read,
                //                   FileAccess.Write,
                //                   FileAccess.ReadWrite
                fs = File.Open(sFileName,
                                          flMode,
                                          FileAccess.Write);


                // generate a file stream with UTF8 characters
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine("Edit Log Started at: " + DateTime.Now);
                sw.WriteLine("-----------------------------------------");

                // read one string at a time, outputting each to the
                // FileStream open for writing
                // Console.WriteLine("Enter text; enter blank line to stop");
                return sw;


            }
            catch (IOException fe)
            {
                MessageBox.Show("Unable to create the log file\r\n" + fe.Message);
                return null;
            }
            finally
            {
                fs = null;
            }

        }
        public static string SerializeObject(object pObject, Type type)
        {


            MemoryStream memoryStream = null;

            XmlSerializer xs = null;

            XmlTextWriter xmlTextWriter = null;

            try
            {

                string XmlizedString = null;

                memoryStream = new MemoryStream();

                xs = new XmlSerializer(type);

                xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);



                xs.Serialize(xmlTextWriter, pObject);

                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

                return XmlizedString;

            }

            catch (Exception e)
            {

                System.Console.WriteLine(e);

                return null;

            }
            finally
            {
                memoryStream = null;

                xs = null;

                xmlTextWriter = null;
            }

        }
        public static object DeserializeObject(string pXmlizedString, Type type)
        {
            XmlSerializer xs = null;
            MemoryStream memoryStream = null;
            XmlTextWriter xmlTextWriter = null;
            try
            {
                xs = new XmlSerializer(type);

                memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));

                xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);



                return xs.Deserialize(memoryStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading the xml config file- DeserializeObject(string)\n" + ex.InnerException.ToString());
                return null;
            }
            finally
            {
                xs = null;
                memoryStream = null;
                xmlTextWriter = null;
            }

        }
        public static object DeserializeObject(XmlNode XMLNode, Type type)
        {
            try
            {

                XmlSerializer xs = new XmlSerializer(type);

                return xs.Deserialize(new XmlNodeReader(XMLNode));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading the xml config file - DeserializeObject \n" + ex.InnerException.ToString());
                return null;
            }

        }
        private static string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = null;
            try
            {
                encoding = new UTF8Encoding();

                string constructedString = encoding.GetString(characters);

                return (constructedString);
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                encoding = null;
            }
        }
        private static byte[] StringToUTF8ByteArray(String pXmlString)
        {

            UTF8Encoding encoding = null;
            try
            {
                encoding = new UTF8Encoding();
                byte[] byteArray = encoding.GetBytes(pXmlString);

                return byteArray;
            }
            catch (Exception ex)
            {
                return null;

            }
            finally
            {
                encoding = null;
            }


        }
        public static XmlDocument returnToXML(string XMLMessage)
        {
            XmlDocument xmld = default(XmlDocument);
            try
            {
                //Create the XML Document
                xmld = new XmlDocument();

                //Load the Xml file
                //xmld.Load(Globals.getFileAtDLLLocation("LayerViewerConfig.xml"));
                try
                {

                    // xmld.Load(Globals.getFileAtDLLLocation("ESRI.WaterUtilitiesTemplate.DesktopFunctions.config"));
                    // XmlDocument xmlDoc = new XmlDocument();

                    xmld.Load(new StringReader(XMLMessage));

                    // xmld.Load();
                    return xmld;


                }
                catch (Exception ex)
                {
                    //  System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                    return null;
                }
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {



            }
        }
        public static string getFileAtDLLLocation(string configFileName)
        {
            try
            {
                string AppPath = null;
                AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                if ((AppPath.IndexOf("file:\\") >= 0))
                {
                    AppPath = AppPath.Replace("file:\\", "");
                }
                string pConfigFiles = "";

                string fullAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                string assemblyName = fullAssemblyName.Split('.').GetValue(fullAssemblyName.Split('.').Length - 1).ToString();
                //assemblyName & ".Config"
                if ((System.IO.File.Exists(System.IO.Path.Combine(AppPath.ToString(), configFileName))))
                {
                    pConfigFiles = System.IO.Path.Combine(AppPath.ToString(), configFileName);
                }
                else if ((System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), configFileName))))
                {
                    pConfigFiles = System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), configFileName);
                }
                else if ((System.IO.File.Exists(System.IO.Path.Combine(AppPath, configFileName))))
                {
                    pConfigFiles = System.IO.Path.Combine(AppPath, configFileName);
                }
                return pConfigFiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - getFileAtDLLLocation" + Environment.NewLine + ex.Message);
                return "";


            }
            finally
            {
            }
        }


        //public static object DeserializeObject(string  strXML, Type type)
        //{
        //    try
        //    {

        //        XmlSerializer xs = new XmlSerializer(type);

        //        return xs.Deserialize(new StringReader(strXML));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error Reading the xml config file\n" + ex.InnerException.ToString());
        //        return null;
        //    }

        //}

        #endregion

        #region RegistryTools
        public static string GetRegistryValueLM(string key, string name)
        {
            string retVal = "";
            RegistryKey rootkey = Registry.LocalMachine;
            RegistryKey subkey = rootkey.OpenSubKey(key);

            // If the RegistryKey doesn't exist return null
            if (subkey == null)
            {
                return retVal;
            }
            else
            {
                try
                {

                    RegistryValueKind valKind = subkey.GetValueKind(name);
                    if (valKind == RegistryValueKind.DWord || valKind == RegistryValueKind.String)
                        retVal = subkey.GetValue(name).ToString();
                }
                catch
                {

                }
            }
            return retVal;
        }
        public static string GetRegistryValueCU(string key, string name)
        {
            string retVal = "";

            RegistryKey rootkey = Registry.CurrentUser;
            RegistryKey subkey = rootkey.OpenSubKey(key);
            // If the RegistryKey doesn't exist return null
            if (subkey == null)
            {
                return retVal;
            }
            else
            {
                try
                {
                    //       Debug.WriteLine("Subkey: " + name);
                    // Check for proper registry value data type
                    RegistryValueKind valKind = subkey.GetValueKind(name);
                    if (valKind == RegistryValueKind.DWord || valKind == RegistryValueKind.String)
                        retVal = subkey.GetValue(name).ToString();
                }
                catch
                {

                }
            }
            return retVal;
        }
        #endregion

        #region EditingTools
        public static void ValidateFeature(IFeature Feature)
        {
            IValidate pVal = null;
            IFields pInvFlds = null;
            //IField pFld = null;
            IEnumRule pEnumRules = null;
            IRule pRul = null;
            try
            {
                pVal = Feature as IValidate;
                pInvFlds = pVal.GetInvalidFields();
                if (pInvFlds.FieldCount > 0)
                {
                    for (int i = 0; i < pInvFlds.FieldCount; i++)
                    {
                        MessageBox.Show(pInvFlds.get_Field(i).AliasName);
                        pEnumRules = pVal.GetInvalidRulesByField(pInvFlds.get_Field(i).Name);

                        while ((pRul = pEnumRules.Next()) != null)
                        {
                            MessageBox.Show(pRul.Type.ToString());
                        }

                    }

                }
                pEnumRules = pVal.GetInvalidRules();


                while ((pRul = pEnumRules.Next()) != null)
                {
                    MessageBox.Show(pRul.Type.ToString());
                }

            }
            catch
            {
            }
            finally
            {
                pVal = null;
                pInvFlds = null;
                pEnumRules = null;
                pRul = null;
            }
        }

        public static void InitDefaults(this object o)
        {
            System.Reflection.PropertyInfo[] props = o.GetType().GetProperties(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            for (int i = 0; i < props.Length; i++)
            {
                System.Reflection.PropertyInfo prop = props[i];

                if (prop.GetCustomAttributes(true).Length > 0)
                {
                    object[] defaultValueAttribute =
                        prop.GetCustomAttributes(typeof(DefaultValueAttribute), true);

                    if (defaultValueAttribute != null)
                    {
                        DefaultValueAttribute dva =
                            defaultValueAttribute[0] as DefaultValueAttribute;

                        if (dva != null)
                            prop.SetValue(o, dva.Value, null);
                    }
                }
            }
        }
        public static string PromptForSave()
        {
            SaveFileDialog saveFileDialog1 = null;
            try
            {
                // Displays a SaveFileDialog so the user can save the a log file

                saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "log file|*.txt";
                saveFileDialog1.Title = "Save an Log File";
                saveFileDialog1.FileName = "AADebug.txt";

                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    if (saveFileDialog1.FileName == "AADebug.txt")
                    {

                        return "";
                    }
                    else
                        return saveFileDialog1.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {

                return "";
            }
            finally
            {
                if (saveFileDialog1 != null)
                {
                    saveFileDialog1.Dispose();

                }
                saveFileDialog1 = null;



            }
        }
        public static string getDebugPath()
        {
            string tmp = ConfigUtil.GetConfigValue("AttributeAssistant_Debug_Path", "!none");

            if (tmp == "!none")
            {
                tmp = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\ArcGISSolutions";

                if (System.IO.Directory.Exists(tmp))
                {
                    return tmp + "\\AALogFile.txt";
                }
                else
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(tmp);
                        return tmp + "\\AALogFile.txt";
                    }
                    catch
                    {
                        return PromptForSave();
                    }
                }

            }
            else
            {
                if (System.IO.Directory.Exists(tmp))
                {
                    return tmp + "\\AALogFile.txt";
                }
                else
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(tmp);
                        return tmp + "\\AALogFile.txt";
                    }
                    catch
                    {
                        tmp = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\ArcGISSolutions";

                        if (System.IO.Directory.Exists(tmp))
                        {
                            return tmp + "\\AALogFile.txt";
                        }
                        else
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(tmp);
                                return tmp + "\\AALogFile.txt";
                            }
                            catch
                            {
                                return PromptForSave();
                            }
                        }

                    }
                }
                return tmp;
            }


        }

        //public static IEditTemplateManager GetTemplateManager(IFeatureLayer featLayer)
        //{
        //    IEditTemplateManager editTemplateMgr = null;
        //    ILayerExtensions layerExtensions;
        //    try {
        //        layerExtensions = featLayer as ILayerExtensions;

        //        //Find the EditTemplateManager extension for the layer
        //        for (int i = 0; i < layerExtensions.ExtensionCount; i++)
        //        {
        //            object extension = layerExtensions.get_Extension(i);

        //            if (extension is IEditTemplateManager)
        //                editTemplateMgr = extension as IEditTemplateManager;
        //        }
        //        return editTemplateMgr;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        layerExtensions = null;
        //    }
        //}
        public static int GetEditTemplateCount(IFeatureLayer Layer)
        {
            IEditTemplateManager ipEditTemplateMgr = null;
            try
            {



                ipEditTemplateMgr = GetEditTemplateManager(Layer);

                return ipEditTemplateMgr.Count;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }

        public static IEditTemplate GetEditTemplateByIndex(IFeatureLayer Layer, int Index)
        {
            IEditTemplateManager ipEditTemplateMgr = null;
            try
            {



                ipEditTemplateMgr = GetEditTemplateManager(Layer);

                return ipEditTemplateMgr.get_EditTemplate(Index);
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public static IEditTemplate GetEditTemplate(string TemplateName, IFeatureLayer Layer)
        {
            try
            {


                IEditTemplateManager ipEditTemplateMgr;
                ipEditTemplateMgr = GetEditTemplateManager(Layer);
                if (ipEditTemplateMgr == null)
                {
                    return null;
                }
                for (int kdx = 0; kdx < ipEditTemplateMgr.Count; kdx++)
                {
                    IEditTemplate pEditTemp = ipEditTemplateMgr.get_EditTemplate(kdx);
                    if (ipEditTemplateMgr.get_EditTemplate(kdx).Name == TemplateName)
                    {
                        return pEditTemp;

                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Editing Template \r\n" + ex.Message);
                return null;
            }
        }
        public static IEditTemplate PromptAndGetEditTemplate(IFeatureLayer Layer, string DefaultTemplate, string caption)
        {

            IList<string> strTemplateNames = Globals.GetEditTemplateNames(Layer);
            if (strTemplateNames.Count == 0)
                return null;
            if (strTemplateNames.Contains(DefaultTemplate) && DefaultTemplate != "")
            {
                return Globals.GetEditTemplate(DefaultTemplate, Layer);
            }
            else
            {
                string SelectedTemplate = Globals.showValuesOptionsForm(strTemplateNames, Layer.Name, caption, ComboBoxStyle.DropDownList);
                if (SelectedTemplate == "")
                    return null;
                return Globals.GetEditTemplate(SelectedTemplate, Layer);
            }

        }

        public static IEditTemplate PromptAndGetEditTemplate(IFeatureLayer Layer, string DefaultTemplate)
        {

            return PromptAndGetEditTemplate(Layer, DefaultTemplate, "Select a template for " + Layer.Name);
        }

        public static IEditTemplate PromptAndGetEditTemplateGraphic(IFeatureLayer Layer, string DefaultTemplate)
        {
            SelectTemplateFormGraphic pForm = null;
            DialogResult result;
            IEditTemplate pEditTemp = null;
            try
            {
                if (DefaultTemplate != "")
                {
                    pEditTemp = Globals.GetEditTemplate(DefaultTemplate, Layer);
                }

                // "Select a template for " + Layer.Name
                if (pEditTemp == null)
                {
                    int idx = Globals.GetEditTemplateCount(Layer);

                    if (idx == 0)
                    {
                        return null;
                    }
                    else if (idx == 1)
                    {

                        return Globals.GetEditTemplateByIndex(Layer, 0);

                    }
                    else
                    {
                        pForm = new SelectTemplateFormGraphic(Layer);
                        pForm.lblLayer.Text = "Select a template for " + Layer.Name;
                        //pForm.LoadListView();

                        result = pForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            return pForm.GetSelected();

                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return pEditTemp;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("PromptAndGetEditTemplateGraphic: " + ex.Message);
                return null;
            }
            finally
            {
                pForm = null;
            }
        }


        public static string showValuesOptionsForm(IList<string> values, string LabelValue, string FormCaption, ComboBoxStyle cboSt)
        {
            try
            {
                if (values.Count == 1)
                {
                    return values[0];
                }

                SelectTemplateForm tmpForm = new SelectTemplateForm();
                tmpForm.lblLayer.Text = LabelValue;
                tmpForm.Text = FormCaption;
                tmpForm.cboSelectTemplate.DataSource = values;
                tmpForm.setComboType(cboSt);
                Graphics g = tmpForm.cboSelectTemplate.CreateGraphics();
                int frmWidth = Globals.getLongestText(values, tmpForm.cboSelectTemplate.Font, ref g);
                g = null;
                tmpForm.setWidth(frmWidth);
                tmpForm.FormClosing += new FormClosingEventHandler(tmpForm_FormClosing);
                tmpForm.StartPosition = FormStartPosition.CenterScreen;
                tmpForm.ShowDialog();
                return tmpForm.cboSelectTemplate.Text;
            }
            catch
            {
                return "";
            }

            //return "";
        }

        public static DomSubList showValuesOptionsForm(IList<DomSubList> values, string LabelValue, string FormCaption, ComboBoxStyle cboSt)
        {
            try
            {
                if (values.Count == 1)
                {
                    return values[0];
                }

                SelectTemplateForm tmpForm = new SelectTemplateForm();
                tmpForm.lblLayer.Text = LabelValue;
                tmpForm.Text = FormCaption;
                tmpForm.cboSelectTemplate.DataSource = values;
                tmpForm.cboSelectTemplate.ValueMember = "Value";
                tmpForm.cboSelectTemplate.DisplayMember = "Display";

                tmpForm.setComboType(cboSt);
                Graphics g = tmpForm.cboSelectTemplate.CreateGraphics();

                int frmWidth = getLongestText(values, tmpForm.cboSelectTemplate.Font, ref g);
                g = null;
                tmpForm.setWidth(frmWidth);
                tmpForm.FormClosing += new FormClosingEventHandler(tmpForm_FormClosing);
                tmpForm.StartPosition = FormStartPosition.CenterScreen;
                tmpForm.ShowDialog();
                return (DomSubList)tmpForm.cboSelectTemplate.SelectedItem;
            }
            catch
            {
                return null;
            }

            //return "";
        }
        public static OptionsToPresent showOptionsForm(IList<OptionsToPresent> features, string LayerName, string caption, ComboBoxStyle dropDownStyle)
        {
            try
            {
                if (features.Count == 1)
                {
                    return features[0];
                }

                SelectOptionForm tmpForm = new SelectOptionForm();
                tmpForm.lblLayer.Text = caption;
                Graphics g = tmpForm.cboSelectTemplate.CreateGraphics();
                int frmWidth = getLongestText(features, tmpForm.cboSelectTemplate.Font, ref g);
                g = null;
                tmpForm.setWidth(frmWidth);
                tmpForm.cboSelectTemplate.DataSource = features;
                tmpForm.cboSelectTemplate.ValueMember = "OID";
                tmpForm.cboSelectTemplate.DisplayMember = "Display";
                tmpForm.cboSelectTemplate.DropDownStyle = dropDownStyle;
                tmpForm.FormClosing += new FormClosingEventHandler(tmpForm_FormClosing);
                tmpForm.StartPosition = FormStartPosition.CenterScreen;
                tmpForm.ShowDialog();
                return (OptionsToPresent)tmpForm.cboSelectTemplate.SelectedItem;
            }
            catch
            {
                return null;
            }

            //return "";
        }

        public static int getLongestText(IList<string> options, System.Drawing.Font fnt, ref Graphics g)
        {
            int maxLen = 0;


            foreach (string val in options)
            {
                SizeF tmpF = g.MeasureString(val, fnt);
                if (Convert.ToInt32(tmpF.Width) > maxLen)
                    maxLen = Convert.ToInt32(tmpF.Width);
            }
            return maxLen;

        }
        public static int getLongestText(IList<OptionsToPresent> options, System.Drawing.Font fnt, ref Graphics g)
        {
            int maxLen = 0;


            foreach (OptionsToPresent val in options)
            {
                SizeF tmpF = g.MeasureString(val.Display, fnt);
                if (Convert.ToInt32(tmpF.Width) > maxLen)
                    maxLen = Convert.ToInt32(tmpF.Width);
            }
            return maxLen;

        }
        public static int getLongestText(IList<DomSubList> options, System.Drawing.Font fnt, ref Graphics g)
        {
            int maxLen = 0;


            foreach (DomSubList val in options)
            {
                SizeF tmpF = g.MeasureString(val.Display, fnt);
                if (Convert.ToInt32(tmpF.Width) > maxLen)
                    maxLen = Convert.ToInt32(tmpF.Width);
            }
            return maxLen;

        }
        public static string showOptionsForm(List<string> options, string caption, ComboBoxStyle dropDownStyle)
        {
            try
            {
                //if (options.Count == 1)
                //{
                //    return options[0];
                //}

                SelectOptionForm tmpForm = new SelectOptionForm();

                tmpForm.lblLayer.Text = caption;
                Graphics g = tmpForm.cboSelectTemplate.CreateGraphics();
                int frmWidth = getLongestText(options, tmpForm.cboSelectTemplate.Font, ref g);
                g = null;
                tmpForm.setWidth(frmWidth);
                tmpForm.showCancelButton();
                options.Sort();
                tmpForm.cboSelectTemplate.DataSource = options;
                tmpForm.cboSelectTemplate.DropDownStyle = dropDownStyle;
                //tmpForm.cboSelectTemplate.Sorted = true;

                //tmpForm.cboSelectTemplate.ValueMember = "OID";
                //tmpForm.cboSelectTemplate.DisplayMember = "Display";
                tmpForm.FormClosing += new FormClosingEventHandler(tmpForm_FormClosing);
                tmpForm.StartPosition = FormStartPosition.CenterScreen;
                tmpForm.ShowDialog();
                if (tmpForm.Cancel == true)
                {
                    return "||Cancelled||";
                }
                else
                {
                    return (string)tmpForm.cboSelectTemplate.SelectedItem;
                }
            }
            catch
            {
                return null;
            }

            //return "";
        }
        private static void tmpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //instance.whatever = ((TestForm)sender).whatever2;
        }
        public static IList<string> GetEditTemplateNames(IFeatureLayer Layer)
        {
            IEditTemplateManager ipEditTemplateMgr = null;
            try
            {

                List<string> templates = new List<string>();



                ipEditTemplateMgr = GetEditTemplateManager(Layer);

                for (int kdx = 0; kdx < ipEditTemplateMgr.Count; kdx++)
                {
                    templates.Add(ipEditTemplateMgr.get_EditTemplate(kdx).Name);
                }

                return templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Template Names: \r\n" + ex.Message);
                return null;
            }
            finally
            {

                ipEditTemplateMgr = null;
            }
        }
        public static IList<IEditTemplate> GetEditTemplates(IApplication App, IFeatureLayer Layer)
        {
            try
            {
                IMap pMap;
                IMxDocument ipDoc;
                ipDoc = App.Document as IMxDocument;

                pMap = ipDoc.FocusMap;
                List<IEditTemplate> templates = new List<IEditTemplate>();

                ILayerExtensions ipLayerExt;
                ipLayerExt = Layer as ILayerExtensions;
                //loop through looking for the editTemplateManager

                for (int jdx = 0; jdx < ipLayerExt.ExtensionCount; jdx++)
                {
                    object obj = ipLayerExt.get_Extension(jdx);
                    if (obj is IEditTemplateManager)
                    {
                        IEditTemplateManager ipEditTemplateMgr;
                        ipEditTemplateMgr = obj as IEditTemplateManager;

                        for (int kdx = 0; kdx < ipEditTemplateMgr.Count; kdx++)
                        {
                            templates.Add(ipEditTemplateMgr.get_EditTemplate(kdx));
                        }
                    }

                }
                return templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Template: \r\n" + ex.Message);
                return null;
            }
        }
        public static IEditTemplateManager GetEditTemplateManager(IFeatureLayer Layer)
        {
            ILayerExtensions ipLayerExt = null;

            try
            {


                ipLayerExt = Layer as ILayerExtensions;
                //loop through looking for the editTemplateManager

                for (int jdx = 0; jdx < ipLayerExt.ExtensionCount; jdx++)
                {
                    object obj = ipLayerExt.get_Extension(jdx);
                    if (obj is IEditTemplateManager)
                    {
                        IEditTemplateManager ipEditTemplateMgr;
                        ipEditTemplateMgr = obj as IEditTemplateManager;
                        return ipEditTemplateMgr;

                    }

                }
                return null;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Manager: \r\n" + ex.Message);
                return null;
            }
            finally
            {
                ipLayerExt = null;
            }
        }
        public static bool FeatureIsValidTemplate(IEditTemplate pEdTempl, IFeature inFeature, string[] args)
        {
            try
            {
                // IEditTemplate pEdTempl = pEdTmpManager.get_EditTemplate(kdx);
                foreach (string Fld in args)
                {
                    int fldDx = inFeature.Fields.FindField(Fld);

                    if (fldDx >= 0)
                    {
                        if (inFeature.get_Value(fldDx) != null && inFeature.get_Value(fldDx) != DBNull.Value && pEdTempl.get_DefaultValue(Fld) != null && pEdTempl.get_DefaultValue(Fld) != DBNull.Value)
                        {

                            if (inFeature.get_Value(fldDx).ToString() != pEdTempl.get_DefaultValue(Fld).ToString())
                            {
                                return false;

                            }
                        }
                    }
                    else
                    {

                        fldDx = inFeature.Fields.FindFieldByAliasName(Fld);

                        if (fldDx >= 0)
                        {
                            if (inFeature.get_Value(fldDx) != null && inFeature.get_Value(fldDx) != DBNull.Value && pEdTempl.get_DefaultValue(inFeature.Fields.get_Field(fldDx).Name) != null && pEdTempl.get_DefaultValue(inFeature.Fields.get_Field(fldDx).Name) != DBNull.Value)
                            {
                                if (inFeature.get_Value(fldDx).ToString() != pEdTempl.get_DefaultValue(inFeature.Fields.get_Field(fldDx).Name).ToString())
                                {
                                    return false;

                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in FeatureIsValid Function: \r\n" + ex.Message);
                return false;
            }
        }
        public static bool FeatureIsValidTemplate(IEditTemplateManager pEdTmpManager, IFeature inFeature, string[] args)
        {

            for (int kdx = 0; kdx < pEdTmpManager.Count; kdx++)
            {
                if (Globals.FeatureIsValidTemplate(pEdTmpManager.get_EditTemplate(kdx), inFeature, args) == true)
                {
                    return true;

                }


            }
            return false;
        }

        public static bool IsEditable(ref IFeature pFeature, ref IEditor Editor)
        {
            IEditLayers pEditLayers = default(IEditLayers);
            ILayer Layer = FindLayerByClassID(Editor.Map, pFeature.Class.ObjectClassID.ToString());
            if (Layer == null)
                return false;

            try
            {
                pEditLayers = (IEditLayers)Editor;

                return pEditLayers.IsEditable(Layer as IFeatureLayer);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - IsEditable" + Environment.NewLine + ex.Message);
                return false;
            }
            finally
            {
                pEditLayers = null;


            }
        }

        public static bool IsEditable(ref IFeatureLayer Layer, ref IEditor Editor)
        {
            IEditLayers pEditLayers = default(IEditLayers);

            try
            {
                pEditLayers = (IEditLayers)Editor;

                return pEditLayers.IsEditable(Layer);

            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error in Global Functions - IsEditable" + Environment.NewLine + ex.Message);
                return false;
            }
            finally
            {
                pEditLayers = null;


            }
        }
        public static bool IsEditable(string LayerName, ref IEditor Editor)
        {
            IEditLayers pEditLayers = default(IEditLayers);

            try
            {
                pEditLayers = (IEditLayers)Editor;
                bool FCorLayer = true;
                return pEditLayers.IsEditable((IFeatureLayer)FindLayer(Editor.Map, LayerName, ref FCorLayer));

            }
            catch (Exception ex)
            {
                //      MessageBox.Show("Error in Global Functions - IsEditable" + Environment.NewLine + ex.Message);
                return false;
            }
            finally
            {
                pEditLayers = null;


            }
        }

        public static IEditor getEditor(ref IApplication pApp)
        {
            UID editorUID = null;
            try
            {
                editorUID = new UIDClass();
                editorUID.Value = "esriEditor.Editor";
                return pApp.FindExtensionByCLSID(editorUID) as IEditor;
            }
            catch
            {
                return null;


            }
            finally
            { editorUID = null; }


        }
        public static IEditor getEditor(IApplication pApp)
        {
            UID editorUID = null;
            try
            {
                editorUID = new UIDClass();
                editorUID.Value = "esriEditor.Editor";
                return pApp.FindExtensionByCLSID(editorUID) as IEditor;
            }
            catch
            {
                return null;


            }
            finally
            { editorUID = null; }


        }
        public static IEditTask getEditTask(ref IEditor Editor, string TaskName)
        {

            for (int i = 0; i < Editor.TaskCount; i++)
            {
                if (Editor.get_Task(0).Name == TaskName)
                    return Editor.get_Task(0);
            }

            return null;

        }
        public static void DeleteFeatures(IEnumFeature enumFeatures)
        {
            IFeature feature = null;
            ISet set = null;
            IFeatureEdit2 featureEdit = null;
            try
            {
                set = new Set();
                enumFeatures.Reset();

                //Takes features and writes them out to an ISet object
                while ((feature = (IFeature)enumFeatures.Next()) != null)
                {
                    featureEdit = feature as IFeatureEdit2;
                    set.Add(feature);
                }

                //Calls the deleteset method from IFeatureEdit to delete the selected set of records
                if (featureEdit != null)
                    featureEdit.DeleteSet(set);


            }
            catch
            { }
            finally
            {
                if (set != null)
                    //Release objects implementing ISet
                    Marshal.ReleaseComObject(set);

                feature = null;
                set = null;
                featureEdit = null;
            }


        }
        public static IFeature CreateFeature(IGeometry geo, IEditTemplate pEditTemplate, IEditor Editor, IApplication app, bool checkForExisting, bool ClearSelected, bool SelectFeature)
        {
            IFeatureLayer pfeatureLayer = null;
            IFeatureClass pfeatureClass = null;
            ISpatialFilter pSpatFilt = null;
            IFeature feature = null;
            IGeometryDef pGeometryDefTest = null;
            IFields pFieldsTest = null;
            IField pFieldTest = null;
            IPoint pN = null;
            IZ pZ = null;
            IGeoDataset pDS = null;
            IZAware pZAware = null;
            try
            {
                if (pEditTemplate == null)
                {
                    MessageBox.Show("Please select an edit template to continue");
                    return null;
                }
                pfeatureLayer = pEditTemplate.Layer as IFeatureLayer;
                pfeatureClass = pfeatureLayer.FeatureClass;

                if (checkForExisting)
                {
                    pSpatFilt = new SpatialFilterClass();
                    pSpatFilt.Geometry = geo;
                    pSpatFilt.GeometryField = pfeatureLayer.FeatureClass.ShapeFieldName;
                    pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;




                    if (pfeatureClass.FeatureCount(pSpatFilt) > 0)
                    {
                        return null;
                    }
                }

                //create point for the current template
                feature = pfeatureClass.CreateFeature();

                //if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                //{




                //    IGeometryDef pGeometryDefTest;
                //    string sShpName = pfeatureClass.ShapeFieldName;

                //    IFields pFieldsTest = pfeatureClass.Fields;
                //    int lGeomIndex = pFieldsTest.FindField(sShpName);

                //    IField pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                //    pGeometryDefTest = pFieldTest.GeometryDef;
                //    bool bZAware;
                //    bool bMAware;
                //    //Determine if M or Z aware
                //    bZAware = pGeometryDefTest.HasZ;
                //    bMAware = pGeometryDefTest.HasM;

                //    if (bZAware)
                //    {

                //        // IGeometry pGeo = new PolylineClass();
                //        IZAware pZAware = geo as IZAware;
                //        if (pZAware.ZAware)
                //        {

                //        }
                //        else
                //        {
                //            pZAware.ZAware = true;

                //            //pZAware.DropZs();
                //        }
                //        // pZAware.DropZs();
                //        IZ pZ = geo as IZ;
                //        pZ.SetConstantZ(0);
                //    }
                //    if (bMAware)
                //    {

                //    }
                //    IGeoDataset pDS = pfeatureClass as IGeoDataset;

                //    geo.SpatialReference = pDS.SpatialReference;
                //}
                //else if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                //{





                string sShpName = pfeatureClass.ShapeFieldName;

                pFieldsTest = pfeatureClass.Fields;
                int lGeomIndex = pFieldsTest.FindField(sShpName);

                pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                pGeometryDefTest = pFieldTest.GeometryDef;
                bool bZAware;
                bool bMAware;
                //Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ;
                bMAware = pGeometryDefTest.HasM;

                if (bZAware)
                {

                    // IGeometry pGeo = new PolylineClass();
                    pZAware = geo as IZAware;
                    if (pZAware.ZAware)
                    {

                    }
                    else
                    {
                        pZAware.ZAware = true;

                        //pZAware.DropZs();
                    }
                    // pZAware.DropZs();
                    if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        // pZAware.InitDefaults();
                        pN = geo as IPoint;
                        pN.Z = 0;
                        geo = pN as IGeometry;

                    }
                    else
                    {
                        pZ = geo as IZ;

                        pZ.SetConstantZ(0);
                    }
                }
                if (bMAware)
                {

                }

                pDS = pfeatureClass as IGeoDataset;

                geo.SpatialReference = pDS.SpatialReference;


                //else
                //{
                //    feature.Shape = geo;
                //}
                feature.Shape = geo;
                pEditTemplate.SetDefaultValues(feature);
                //  feature.Store();

                if (ClearSelected)

                    Editor.Map.ClearSelection();
                //if (feature is INetworkFeature)
                //{
                //    feature.Store();
                //    INetworkFeature pNF = (INetworkFeature)feature;

                //    pNF.Connect();
                //}
                if (SelectFeature)
                    Editor.Map.SelectFeature(pfeatureLayer, feature);
                // MessageBox.Show(Editor.Map.SelectionCount.ToString());

                //Invalidate the area around the new feature
                //IEnvelope pEnv = null;
                //m_editor.Map.AreaOfInterest().QueryEnvelope(pEnv);
                //IEnvelope pEnv = m_editor.Map.AreaOfInterest;

                //Editor.Display.Invalidate((app.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);
                return feature;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating a feature with template\r\nFeature Template: " + pEditTemplate.Name + "\r\n" + ex.Message);
                return null;

            }
            finally
            {

                pfeatureLayer = null;
                pfeatureClass = null;
                pSpatFilt = null;

                pGeometryDefTest = null;
                pFieldsTest = null;
                pFieldTest = null;
                pN = null;
                pZ = null;
                pDS = null;
                pZAware = null;
            }
        }
        public static IFeature CreateFeature(IGeometry geo, IFeatureLayer FeatureLay, IEditor Editor, IApplication app, bool checkForExisting, bool ClearSelected, bool SelectFeature)
        {

            IFeatureClass pfeatureClass = null;
            ISpatialFilter pSpatFilt = null;
            IFeature feature = null;
            IGeometryDef pGeometryDefTest = null;
            IFields pFieldsTest = null;
            IField pFieldTest = null;
            IPoint pN = null;
            IZ pZ = null;
            IGeoDataset pDS = null;
            IZAware pZAware = null;
            IRowSubtypes rowSubtype = null;
            ISubtypes pSub = null;
            try
            {
                pfeatureClass = FeatureLay.FeatureClass;
                if (checkForExisting)
                {
                    pSpatFilt = new SpatialFilterClass();
                    pSpatFilt.Geometry = geo;
                    pSpatFilt.GeometryField = FeatureLay.FeatureClass.ShapeFieldName;
                    pSpatFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;




                    if (pfeatureClass.FeatureCount(pSpatFilt) > 0)
                    {
                        return null;
                    }
                }
                //create point for the current template
                feature = pfeatureClass.CreateFeature();

                //if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                //{




                //    IGeometryDef pGeometryDefTest;
                //    string sShpName = pfeatureClass.ShapeFieldName;

                //    IFields pFieldsTest = pfeatureClass.Fields;
                //    int lGeomIndex = pFieldsTest.FindField(sShpName);

                //    IField pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                //    pGeometryDefTest = pFieldTest.GeometryDef;
                //    bool bZAware;
                //    bool bMAware;
                //    //Determine if M or Z aware
                //    bZAware = pGeometryDefTest.HasZ;
                //    bMAware = pGeometryDefTest.HasM;

                //    if (bZAware)
                //    {

                //        // IGeometry pGeo = new PolylineClass();
                //        IZAware pZAware = geo as IZAware;
                //        if (pZAware.ZAware)
                //        {

                //        }
                //        else
                //        {
                //            pZAware.ZAware = true;

                //            //pZAware.DropZs();
                //        }
                //        // pZAware.DropZs();
                //        IZ pZ = geo as IZ;
                //        pZ.SetConstantZ(0);
                //    }
                //    if (bMAware)
                //    {

                //    }
                //    IGeoDataset pDS = pfeatureClass as IGeoDataset;

                //    geo.SpatialReference = pDS.SpatialReference;
                //}


                string sShpName = pfeatureClass.ShapeFieldName;

                pFieldsTest = pfeatureClass.Fields;
                int lGeomIndex = pFieldsTest.FindField(sShpName);

                pFieldTest = pFieldsTest.get_Field(lGeomIndex);
                pGeometryDefTest = pFieldTest.GeometryDef;
                bool bZAware;
                bool bMAware;
                //Determine if M or Z aware
                bZAware = pGeometryDefTest.HasZ;
                bMAware = pGeometryDefTest.HasM;

                if (bZAware)
                {

                    // IGeometry pGeo = new PolylineClass();
                    pZAware = geo as IZAware;
                    if (pZAware.ZAware)
                    {

                    }
                    else
                    {
                        pZAware.ZAware = true;

                        //pZAware.DropZs();
                    }
                    // pZAware.DropZs();
                    if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        // pZAware.InitDefaults();
                        pN = geo as IPoint;
                        pN.Z = 0;
                        geo = pN as IGeometry;

                    }
                    else
                    {
                        pZ = geo as IZ;

                        pZ.SetConstantZ(0);
                    }
                }
                if (bMAware)
                {

                }
                pDS = pfeatureClass as IGeoDataset;

                geo.SpatialReference = pDS.SpatialReference;
                feature.Shape = geo;
                rowSubtype = feature as IRowSubtypes;
                pSub = pfeatureClass as ISubtypes;
                if (pSub.HasSubtype)
                    rowSubtype.SubtypeCode = pSub.DefaultSubtypeCode;
                rowSubtype.InitDefaultValues();
                //if (feature is INetworkFeature)
                //{
                //    ((INetworkFeature)feature).Connect();
                //}

                // feature.Store();
                if (ClearSelected)
                    Editor.Map.ClearSelection();

                if (SelectFeature)
                    Editor.Map.SelectFeature(FeatureLay, feature);
                // Editor.Map.SelectFeature(FeatureLay, feature);
                //Invalidate the area around the new feature
                //IEnvelope pEnv = null;
                //m_editor.Map.AreaOfInterest().QueryEnvelope(pEnv);
                //IEnvelope pEnv = m_editor.Map.AreaOfInterest;

                Editor.Display.Invalidate((app.Document as IMxDocument).ActiveView.Extent, true, (short)esriScreenCache.esriAllScreenCaches);
                return feature;

                //            m_editor.Display.Invalidate(feature.Extent, true, (short)esriScreenCache.esriAllScreenCaches);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Create Feature: " + ex.Message);
                return null;

            }
            finally
            {
                pfeatureClass = null;
                pSpatFilt = null;

                pGeometryDefTest = null;
                pFieldsTest = null;
                pFieldTest = null;
                pN = null;
                pZ = null;
                pDS = null;
                pZAware = null;
                rowSubtype = null;
                pSub = null;
            }
        }
        public static bool IsShapeConstructorOkay(IShapeConstructor m_csc)
        {
            if (m_csc == null)
                return false;

            if (m_csc.Enabled != true)
                return false;

            if (m_csc.Active)
                return true;
            else
                m_csc.Activate();
            return true;
        }
        #endregion

        #region General_ComparasionTools
        public static bool IsNumeric(string PossibleNumber)
        {
            if (Information.IsNumeric(PossibleNumber))
            {
                double tstDbl;

                double.TryParse(PossibleNumber, out  tstDbl);
                if (Double.IsNaN(tstDbl))

                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }

            ////@"^\d+$"
            //Regex objNotWholePattern = new Regex(@"^\d+$");//"[^0-9]");
            //return !objNotWholePattern.IsMatch(PossibleNumber)
            //     && (PossibleNumber != "");
        }
        public static IRgbColor GetColor(int r, int g, int b)
        {
            //Create color
            IRgbColor color;
            color = new RgbColorClass();

            color.Red = r;  //TODO: UserConfig
            color.Green = g;  //TODO: UserConfig
            color.Blue = b;  //TODO: UserConfig
            return color;
        }
        public static bool IsDouble(string theValue)
        {
            try
            {
                Convert.ToDouble(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        }  //IsDecimal

        public static bool IsInteger(string theValue)
        {
            try
            {
                Convert.ToInt32(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsInteger
        public static bool IsBoolean(string theValue)
        {
            try
            {
                Convert.ToBoolean(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsBoolean
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
        public static bool IsOdd(double value)
        {
            return Convert.ToInt32(value) % 2 != 0;
        }

        public enum EvenOdd { Even, Odd };

        public static double RoundToEvenOdd(EvenOdd evenOdd, double value)
        {
            if (evenOdd == EvenOdd.Odd)
            {
                double rdNum = Convert.ToInt32(Math.Round(value, 0));
                if (Globals.IsOdd(rdNum))
                {
                    return rdNum;
                }
                else
                {
                    if (rdNum > value)
                    {
                        return rdNum - 1;

                    }
                    else
                    {
                        return rdNum + 1;
                    }
                }




            }
            else
            {

                double rdNum = Convert.ToInt32(Math.Round(value, 0));
                if (Globals.IsOdd(rdNum) == false)
                {
                    return rdNum;
                }
                else
                {
                    if (rdNum > value)
                    {
                        return rdNum - 1;

                    }
                    else
                    {
                        return rdNum + 1;
                    }
                }


            }
        }
        public static double ConvertPixelsToMap(double pixelUnits, IMap pMap)
        {
            // convert pixels to map coordinates
            double realWorldDisplayExtent;
            long pixelExtent;
            double sizeOfOnePixel;
            IDisplayTransformation pDT = null;
            tagRECT deviceRECT;
            IEnvelope pEnv = null;
            IActiveView pActiveView = null;

            try
            {    // Get the width of the display extents in Pixels
                // and get the extent of the displayed data
                // work out the size of one pixel and then return
                /// the pixels units passed in mulitplied by that value
                pActiveView = (IActiveView)pMap;
                pDT = pActiveView.ScreenDisplay.DisplayTransformation;
                deviceRECT = pDT.get_DeviceFrame();
                pixelExtent = deviceRECT.right - deviceRECT.left;
                pEnv = pDT.VisibleBounds;

                realWorldDisplayExtent = pEnv.Width;
                sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
                return pixelUnits * sizeOfOnePixel;
            }
            catch
            {
                return 0;
            }
            finally
            {
                pDT = null;

                pEnv = null;
                pActiveView = null;
            }



        }
        #endregion

        #region SearchDataTools
        public static IGeometry GetClosestFeatureIgnoreExistingLineFeature(int searchDistance, IGeometry searchShape, IFeatureLayer layer, IFeatureLayer layerToLookFromExistingConnection, bool bSelectedOnly)
        {
            ISpatialFilter sFilter = new SpatialFilterClass();
            ISpatialFilter sExistingFilter = new SpatialFilterClass();
            IEnvelope searchEnvelope;
            IFeatureCursor fCursor = null;
            IFeature sourceFeature = null;
            // IFeature nearestFeature = null;
            IProximityOperator proxOp = null;
            double lastDistance, distance;
            //         EesrSRI.ArcGIS.ADF.ComReleaser comReleaser;

            //_comReleaser = new ESRI.ArcGIS.ADF.ComReleaser();
            //   _comReleaser.ManageLifetime(fCursor);
            IGeometry pGeo = null;
            try
            {
                if (searchDistance > 0)
                {
                    searchEnvelope = searchShape.Envelope;
                    searchEnvelope.Expand(searchDistance, searchDistance, false);
                    sFilter.Geometry = searchEnvelope;


                }
                else
                    sFilter.Geometry = searchShape;


                sExistingFilter.GeometryField = layerToLookFromExistingConnection.FeatureClass.ShapeFieldName;
                sExistingFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                sFilter.GeometryField = layer.FeatureClass.ShapeFieldName;
                sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                if (bSelectedOnly)
                {
                    IFeatureSelection pFeatSel = layer as IFeatureSelection;
                    ISelectionSet pSelSet = pFeatSel.SelectionSet;
                    ICursor pCur = null;
                    pSelSet.Search(sFilter, true, out  pCur);
                    fCursor = (IFeatureCursor)pCur;

                }
                else
                {
                    fCursor = layer.FeatureClass.Search(sFilter, false);
                }

                sourceFeature = fCursor.NextFeature();
                //  nearestFeature = null;

                proxOp = (IProximityOperator)searchShape;
                lastDistance = searchDistance;
                int nearestOID;

                while (!(sourceFeature == null))
                {
                    if (!sourceFeature.Shape.Equals(searchShape))
                    {

                        IGeometry pTempGeo = sourceFeature.ShapeCopy;
                        pTempGeo.Project(searchShape.SpatialReference);

                        distance = proxOp.ReturnDistance(pTempGeo);
                        pTempGeo = null;
                        // distance = proxOp.ReturnDistance(sourceFeature.Shape);

                        if (distance != 0 && distance <= lastDistance)
                        {

                            IPolyline pNewPoly = new PolylineClass();
                            pNewPoly.FromPoint = searchShape as IPoint;
                            pNewPoly.ToPoint = sourceFeature.ShapeCopy as IPoint;
                            sExistingFilter.Geometry = pNewPoly;
                            if (layerToLookFromExistingConnection.FeatureClass.FeatureCount(sExistingFilter) == 0)
                            {
                                //nearestFeature = sourceFeature;
                                pGeo = sourceFeature.ShapeCopy;
                                nearestOID = sourceFeature.OID;

                                lastDistance = distance;
                            }
                            else
                            {
                                // MessageBox.Show("Overlap");

                            }
                        }
                    }

                    sourceFeature = fCursor.NextFeature();
                }
                return pGeo;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);
                if (sourceFeature != null)
                    Marshal.ReleaseComObject(sourceFeature);
            }
        }
        public static IFeature GetClosestFeature(IGeometry sourceGeo, IFeatureLayer searchFeatureLayer, double maxDistance, bool searchOnLayer, bool checkSelection)
        {
            if (searchFeatureLayer == null)
                return null;
            if (sourceGeo == null)
                return null;

            ITopologicalOperator topoOp = null;
            IFeature nearestFeature = null;
            IFeature lineFeature = null;
            IProximityOperator proxOp = null;
            IPolygon poly = null;
            ISpatialFilter sFilter = null;
            IFeatureCursor lineCursor = null;
            ICursor cCursor = null;

            try
            {




                sFilter = new SpatialFilterClass();
                if (maxDistance == 0)
                {
                    sFilter.Geometry = sourceGeo;
                }
                else
                {
                    topoOp = sourceGeo as ITopologicalOperator;
                    poly = topoOp.Buffer(maxDistance) as IPolygon;
                    if (poly == null)
                    {
                        poly = topoOp.Buffer(maxDistance * 10) as IPolygon;
                        if (poly == null)
                        {
                            sFilter.Geometry = sourceGeo;
                        }
                        else
                            sFilter.Geometry = poly;
                    }
                    else
                        sFilter.Geometry = poly;
                }
                sFilter.GeometryField = searchFeatureLayer.FeatureClass.ShapeFieldName;
                sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;


                if (searchOnLayer)
                    if (((IFeatureSelection)searchFeatureLayer).SelectionSet.Count > 0 && checkSelection)
                    {
                        ((IFeatureSelection)searchFeatureLayer).SelectionSet.Search(sFilter, true, out cCursor);
                        lineCursor = (IFeatureCursor)cCursor;

                    }
                    else
                    {
                        lineCursor = searchFeatureLayer.Search(sFilter, true);
                    }
                else
                    lineCursor = searchFeatureLayer.FeatureClass.Search(sFilter, true);
                lineFeature = lineCursor.NextFeature();

                double distance;
                double lastDistance = maxDistance;
                if (lastDistance == 0)
                {
                    lastDistance = 9999;
                }
                proxOp = (IProximityOperator)sourceGeo;
                int closestOID = -1;
                while (!(lineFeature == null))
                {
                    IGeometry pTempGeo = lineFeature.Shape;
                    pTempGeo.Project(sourceGeo.SpatialReference);

                    distance = proxOp.ReturnDistance(pTempGeo);
                    pTempGeo = null;

                    //distance = proxOp.ReturnDistance(lineFeature.Shape);
                    if (distance < lastDistance)
                    {
                        closestOID = lineFeature.OID;
                        lastDistance = distance;
                    }
                    lineFeature = lineCursor.NextFeature();
                }
                proxOp = null; lineFeature = null;
                if (closestOID != -1)
                    nearestFeature = searchFeatureLayer.FeatureClass.GetFeature(closestOID);
                else
                    nearestFeature = null;
                return nearestFeature;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in the Get Nearest Feature\n" + ex.Message, ex.Source);
                return null;
            }

            finally
            {
                if (lineCursor != null)
                    Marshal.ReleaseComObject(lineCursor);
                if (cCursor != null)
                    Marshal.ReleaseComObject(cCursor);
                topoOp = null;
                nearestFeature = null;
                lineFeature = null;
                proxOp = null;
                poly = null;
                sFilter = null;
                lineCursor = null;
                cCursor = null;

            }
        }


        public static IQueryFilter createQueryFilter()
        {
            IQueryFilter sFilter = null;
            try
            {
                sFilter = new QueryFilterClass();


                return sFilter;
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        public static IGeometry bufferTillNotEmpty(IGeometry inGeo, double searchDistance)
        {
            ITopologicalOperator pTopo = null;
            IPolygon ptempPoly = null;
            IGeometry pGeo = null;
            try
            {
                pTopo = inGeo as ITopologicalOperator;
                pGeo = pTopo.Buffer(searchDistance);
                ptempPoly = pGeo as IPolygon;
                if (ptempPoly.IsEmpty == true)
                {
                    pGeo = bufferTillNotEmpty(inGeo, searchDistance * 2);
                }
                return pGeo;
            }
            catch
            {
                return inGeo;
            }



        }
        public static ISpatialFilter createSpatialFilter(IFeatureLayer sourceLayer, IGeometry inGeo, double searchDistance, bool useCentroid, ISpatialReference mapSpatRef)
        {
            IGeometry pGeo = null;
            ISpatialFilter sFilter = null;
            IGeometry pSourceGeo = null;
            ISpatialReferenceResolution pSRResolution = null;
            IEnvelope searchEnvelope = null;
            ITopologicalOperator pTopo = null;
            try
            {
                sFilter = new SpatialFilterClass();

                sFilter.GeometryField = sourceLayer.FeatureClass.ShapeFieldName;
                sFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;


                if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference != null)
                    sFilter.set_OutputSpatialReference(sourceLayer.FeatureClass.ShapeFieldName, mapSpatRef);//(sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                //sFilter.set_OutputSpatialReference(sourceLayer.FeatureClass.ShapeFieldName, AAState._editor.Map.SpatialReference);
                if (inGeo.GeometryType == esriGeometryType.esriGeometryPoint)
                {

                    try
                    {
                        //double dblTol = .001;
                        pSourceGeo = inGeo as IPoint;
                        try
                        {
                            if (searchDistance != 0.0)
                            {
                                pSourceGeo = bufferTillNotEmpty(inGeo, searchDistance);
                                //pTopo = inGeo as ITopologicalOperator;
                                //pSourceGeo = pTopo.Buffer(searchDistance);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        //pSourceGeo.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;
                        if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference != null)
                        {
                            //pSourceGeo.Project((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                            pSourceGeo.Project(mapSpatRef);
                            //pSourceGeo.Project(AAState._editor.Map.SpatialReference);
                        }

                        pGeo = pSourceGeo; // Use the point, not the envelope

                        //dblTol = GetXYTolerance(sourceLayer);
                        //searchEnvelope = new EnvelopeClass();
                        //searchEnvelope.XMin = 0 - dblTol;
                        //searchEnvelope.YMin = 0 - dblTol;
                        //searchEnvelope.XMax = 0 + dblTol;
                        //searchEnvelope.YMax = 0 + dblTol;
                        //searchEnvelope.CenterAt(pSourceGeo as IPoint);
                        //if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference != null)
                        //{
                        //    searchEnvelope.SpatialReference = ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                        //}
                        //searchEnvelope.SpatialReference = (AAState._editor.Map.SpatialReference);


                        //searchEnvelope.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;
                        //searchEnvelope.SnapToSpatialReference();
                        //if (AAState._editor.Map.SpatialReference != ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference)
                        //{

                        //  searchEnvelope.Project((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);

                        //searchEnvelope.Project(AAState._editor.Map.SpatialReference);
                        //}
                        //if (searchDistance == 0.0)
                        //{

                        //    pGeo = pSourceGeo;
                        //}
                        //else
                        //{

                        //pGeo = Globals.Env2Polygon(searchEnvelope);


                        // }
                        //searchEnvelope.Expand(.1, .1, true);
                        //searchEnvelope.Expand(searchDistance, searchDistance, false);
                    }
                    catch
                    {
                        pGeo = inGeo;
                        //pGeo.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;
                        if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference != null)
                        {
                            pGeo.Project((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                        }
                        //pGeo.Project(AAState._editor.Map.SpatialReference);


                    }
                }
                else
                {
                    if (useCentroid)
                        pGeo = GetGeomCenter(inGeo)[0];
                    else
                        pGeo = inGeo;
                    try
                    {
                        if (searchDistance != 0.0)
                        {
                            pGeo = bufferTillNotEmpty(pGeo, searchDistance);
                            //pTopo = pGeo as ITopologicalOperator;
                            //pGeo = pTopo.Buffer(searchDistance);
                            //IPolygon ptempPoly = pGeo as IPolygon;
                            //if (ptempPoly.IsEmpty == true)
                            //{
                            //    pGeo = pTopo.Buffer(searchDistance * 2);
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    //pGeo.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;
                    if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference != null)
                    {
                        if (pGeo.SpatialReference != null)
                        {
                            try
                            {
                                if ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference.FactoryCode != pGeo.SpatialReference.FactoryCode)
                                {
                                    pGeo.Project((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                                }
                            }
                            catch
                            {
                                pGeo.Project((sourceLayer.FeatureClass as IGeoDataset).SpatialReference);
                            }
                        }

                    }
                    // pGeo.Project(AAState._editor.Map.SpatialReference);

                    // sFilter.Geometry = pGeo;

                    // sFilter.FilterOwnsGeometry = true;

                }

                sFilter.Geometry = pGeo;

                return sFilter;
            }
            catch
            {
                return null;
            }
            finally
            {
                pGeo = null;
                // sFilter = null;
                pSourceGeo = null;
                pSRResolution = null;
                searchEnvelope = null;
                pTopo = null;
            }
        }
        public static ISpatialFilter createSpatialFilter(IFeatureLayer sourceLayer, IFeature inFeature, double searchDistance, bool useCentroid, ISpatialReference mapSpatRef)
        {
            return createSpatialFilter(sourceLayer, inFeature.ShapeCopy, searchDistance, useCentroid, mapSpatRef);

        }

        public static ISpatialFilter createSpatialFilter(IFeatureLayer sourceLayer, IFeature inFeature, bool useCentroid, ISpatialReference mapSpatRef)
        {
            return createSpatialFilter(sourceLayer, inFeature, 0.0, useCentroid, mapSpatRef);

        }
        public static ISpatialFilter createSpatialFilter(IFeatureLayer sourceLayer, IGeometry inGeo, bool useCentroid, ISpatialReference mapSpatRef)
        {
            return createSpatialFilter(sourceLayer, inGeo, 0.0, useCentroid, mapSpatRef);

        }
        public enum statsType
        {
            Mean, Max, Sum, Min, Count, StandardDev

        };

        public static string GetFieldStats(IFeatureClass inClass, string FldName, statsType statType)
        {
            ICursor cursor = null;
            IDataStatistics dataStatistics = null;
            //System.Collections.IEnumerator enumerator = null;
            ESRI.ArcGIS.esriSystem.IStatisticsResults statisticsResults = null;
            try
            {
                cursor = (ICursor)inClass.Search(null, false);

                dataStatistics = new DataStatisticsClass();
                dataStatistics.Field = FldName;
                dataStatistics.Cursor = cursor;
                dataStatistics.SampleRate = -1;

                statisticsResults = dataStatistics.Statistics;
                switch (statType)
                {
                    case statsType.Sum:
                        {
                            return statisticsResults.Sum.ToString();
                            break;
                        }
                    case statsType.Count:
                        {
                            return statisticsResults.Count.ToString();
                            break;
                        }
                    case statsType.Max:
                        {
                            return statisticsResults.Maximum.ToString();
                            break;
                        }
                    case statsType.Mean:
                        {
                            return statisticsResults.Mean.ToString();
                            break;
                        }
                    case statsType.Min:
                        {
                            return statisticsResults.Minimum.ToString();
                            break;
                        }

                    case statsType.StandardDev:
                        {
                            return statisticsResults.StandardDeviation.ToString();
                            break;
                        }

                }
                return "Error";
                //enumerator = dataStatistics.UniqueValues;
                //enumerator.Reset();

                //while (enumerator.MoveNext())
                //{
                //    object myObject = enumerator.Current;
                //    Console.WriteLine("Value - {0}", myObject.ToString());
                //}

                //cursor = (ICursor)featureClass.Search(null, false);
                //dataStatistics.Cursor = cursor;


            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                if (cursor != null)
                    Marshal.ReleaseComObject(cursor);

                cursor = null;
                dataStatistics = null;
                //System.Collections.IEnumerator enumerator = null;
                statisticsResults = null;
            }
        }
        #endregion

        #region MXTools
        public static void CenterMapOnFeatureWithScale(IFeature Feature, IApplication app)
        {
            CenterMapOnFeatureWithScale(Feature, app, 0.0);

        }
        public static void CenterMapOnFeatureWithScale(IFeature Feature, IApplication app, string scale)
        {
            CenterMapOnFeatureWithScale(Feature, app, Convert.ToDouble(scale));
        }
        public static void CenterMapOnFeatureWithScale(IFeature Feature, IApplication app, double scale)
        {
            IPoint pPnt = null;
            IEnvelope pEnv = null;
            try
            {

                if (scale != 0.0)
                {
                    ((IMxDocument)app.Document).FocusMap.MapScale = scale;


                }
                pPnt = GetGeomCenter(Feature)[0];
                pPnt.SpatialReference = (Feature.Class as IGeoDataset).SpatialReference;
                pPnt.Project((app.Document as IMxDocument).FocusMap.SpatialReference);


                pEnv = ((IMxDocument)app.Document).ActiveView.Extent;
                pEnv.CenterAt(pPnt);


                ((IMxDocument)app.Document).ActiveView.Extent = pEnv;



                ((IMxDocument)app.Document).ActiveView.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - CenterMapOnFeatureWithScale" + Environment.NewLine + ex.Message);

            }
            finally
            {
                pPnt = null;
                pEnv = null;
            }
        }
        public static ESRI.ArcGIS.Geometry.IPoint GetMapCoordinatesFromScreenCoordinates(ESRI.ArcGIS.Geometry.IPoint screenPoint, IApplication app)
        {
            ESRI.ArcGIS.Display.IScreenDisplay screenDisplay = null;
            ESRI.ArcGIS.Display.IDisplayTransformation displayTransformation = null;
            try
            {
                if (screenPoint == null || screenPoint.IsEmpty)
                {
                    return null;
                }

                screenDisplay = ((IMxDocument)app.Document).ActiveView.ScreenDisplay;
                displayTransformation = screenDisplay.DisplayTransformation;

                return displayTransformation.ToMapPoint(Convert.ToInt32(screenPoint.X), Convert.ToInt32(screenPoint.Y));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Project Costing Tools - Globals.Functions: GetMapCoordinatesFromScreenCoordinates" + Environment.NewLine + ex.Message);
                return null;
            }
            finally
            {
                screenDisplay = null;
            }

        }
        public static System.Drawing.Point GetScreenCoordinatesFromMapCoordinates(ESRI.ArcGIS.Geometry.IPoint MapPoint, IApplication app)
        {
            ESRI.ArcGIS.Display.IScreenDisplay screenDisplay = null;
            ESRI.ArcGIS.Display.IDisplayTransformation displayTransformation = null;
            try
            {

                if (MapPoint == null || MapPoint.IsEmpty)
                {
                    return new System.Drawing.Point();
                }

                screenDisplay = ((IMxDocument)app.Document).ActiveView.ScreenDisplay;
                displayTransformation = screenDisplay.DisplayTransformation;
                System.Drawing.Point pScPnt = new System.Drawing.Point();
                int X, Y;

                displayTransformation.FromMapPoint(MapPoint, out X, out Y);
                pScPnt.X = X;
                pScPnt.Y = Y;
                return pScPnt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Project Costing Tools - Globals.Functions: GetScreenCoordinatesFromMapCoordinates" + Environment.NewLine + ex.Message);
                return new System.Drawing.Point();

            }
            finally
            {
                screenDisplay = null;
                displayTransformation = null;
            }

        }
        public static ICommandBar CreateContextMenu(string MultiItemRef, IApplication app)
        {



            ICommandBar pContextMenu = default(ICommandBar);
            ESRI.ArcGIS.esriSystem.UID pUID = default(ESRI.ArcGIS.esriSystem.UID);

            try
            {
                pContextMenu = ((IDocument)app.Document).CommandBars.Create("StylePopup", ESRI.ArcGIS.SystemUI.esriCmdBarType.esriCmdBarTypeShortcutMenu);
                pUID = new ESRI.ArcGIS.esriSystem.UID();
                pUID.Value = MultiItemRef;
                object menuIdx = 0;
                pContextMenu.Add(pUID, ref menuIdx);
                return pContextMenu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - CreateContext" + Environment.NewLine + ex.Message);
                return null;

            }
            finally
            {
                pUID = null;
                pContextMenu = null;

            }

        }
        public static ESRI.ArcGIS.Framework.ICommandItem GetCommand(string CommandReference, IApplication app)
        {
            return GetCommand(CommandReference, app, -1);

        }
        public static ESRI.ArcGIS.Framework.ICommandItem GetCommand(string CommandReference, IApplication app, int subtype)
        {
            ESRI.ArcGIS.esriSystem.UID pUID = new ESRI.ArcGIS.esriSystem.UID();

            try
            {
                pUID.Value = CommandReference;
                if (subtype != -1)
                {
                    pUID.SubType = subtype;
                }
                return ((IDocument)app.Document).CommandBars.Find(pUID, false, false);

            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error in Global Functions - GetCommand" + Environment.NewLine + ex.Message);
                return null;

            }
            finally
            {
                pUID = null;
            }


        }
        public static void FindCommandAndExecute(ESRI.ArcGIS.Framework.IApplication application, System.String commandName, int subtype)
        {
            ICommandItem commandItem = null;
            try
            {
                commandItem = GetCommand(commandName, application, subtype);

                if (commandItem != null)
                    commandItem.Execute();
            }
            catch
            {
            }
            finally
            {
                commandItem = null;
            }
        }
        public static void FindCommandAndExecute(ESRI.ArcGIS.Framework.IApplication application, System.String commandName)
        {
            ICommandItem commandItem = null;
            try
            {
                commandItem = GetCommand(commandName, application);

                if (commandItem != null)
                    commandItem.Execute();
            }
            catch
            {
            }
            finally
            {
                commandItem = null;
            }
        }
        public static ESRI.ArcGIS.Framework.IDockableWindow GetDockableWindow(ESRI.ArcGIS.Framework.IApplication application, System.String windowName)
        {
            ESRI.ArcGIS.Framework.IDockableWindowManager dockWindowManager = null;
            ESRI.ArcGIS.esriSystem.UID windowID = null;

            try
            {
                dockWindowManager = application as ESRI.ArcGIS.Framework.IDockableWindowManager;
                windowID = new ESRI.ArcGIS.esriSystem.UIDClass();
                windowID.Value = windowName; // example: "esriGeoprocessingUI.GPCommandWindow"
                return dockWindowManager.GetDockableWindow(windowID);
            }
            catch
            {
                return null;
            }
            finally
            {
                windowID = null;
                dockWindowManager = null;
            }

        }
        public static ESRI.ArcGIS.Framework.IDockableWindow GetDockableWindow(IMap map, System.String windowName)
        {
            ESRI.ArcGIS.Framework.IDockableWindowManager dockWindowManager = null;
            ESRI.ArcGIS.esriSystem.UID windowID = null;
            try
            {
                dockWindowManager = map as ESRI.ArcGIS.Framework.IDockableWindowManager;
                windowID = new ESRI.ArcGIS.esriSystem.UIDClass();
                windowID.Value = windowName; // example: "esriGeoprocessingUI.GPCommandWindow"
                return dockWindowManager.GetDockableWindow(windowID);
            }
            catch
            {
                return null;
            }
            finally
            {
                windowID = null;
                dockWindowManager = null;
            }


        }
        public static void IdentifySelectedDockable(IApplication app)
        {

            IdentifyWindow pIdWin = null;
            IDockableWindow pID = null;
            IMap map = null;
            IActiveView av = null;
            IScreenDisplay sd = null;

            IIdentifyDialog identifyDialog = null;
            IIdentifyDialog2 identifyDialog2 = null;
            IIdentifyDisplay Idf = null;
            UID pId = null;
            IEnumLayer enumLayer = null;
            ILayer layer = null;
            IFeatureLayer fLayer = null;
            IFeatureSelection fSel = null;
            IEnumIDs enumIds = null;
            try
            {
                pID = GetDockableWindow(app, "ArcMapUI.IdentifyWindow");
                pIdWin = pID as IdentifyWindow;


                map = ((IMxDocument)app.Document).FocusMap;


                if (map.SelectionCount < 1) return;

                pID.Show(true);
                av = map as IActiveView;
                sd = av.ScreenDisplay;

                identifyDialog = pID.UserData as IIdentifyDialog;//new IdentifyDialogClass();
                identifyDialog2 = pID.UserData as IIdentifyDialog2; //identifyDialog as IIdentifyDialog2;
                Idf = pID.UserData as IIdentifyDisplay;

                identifyDialog.Map = map;
                identifyDialog.Display = sd;

                //Clear the dialog
                identifyDialog.ClearLayers();

                pId = new UIDClass();
                pId.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(pId, true);
                enumLayer.Reset();
                layer = enumLayer.Next();
                int id; bool found = false;
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.Valid && fLayer.Visible)
                    {
                        fSel = (IFeatureSelection)fLayer;

                        //Get the ids of all selected features for this layer
                        enumIds = fSel.SelectionSet.IDs;

                        //Step through each and add to identify window
                        enumIds.Reset();
                        for (int i = 1; i <= fSel.SelectionSet.Count; i++)
                        {
                            id = enumIds.Next();
                            found = true;

                            identifyDialog.AddLayerIdentifyOID(layer, id);

                        }
                    }
                    layer = enumLayer.Next();
                }

                //Open Identify Dialog
                if (found)
                {
                    identifyDialog.Show();

                }
            }
            catch
            {
            }
            finally
            {
                pIdWin = null;
                pID = null;
                map = null;
                av = null;
                sd = null;

                identifyDialog = null;
                identifyDialog2 = null;
                Idf = null;
                pId = null;
                enumLayer = null;
                layer = null;
                fLayer = null;
                fSel = null;
                enumIds = null;
            }

        }
        public static void IdentifySelected(IMap map)
        {
            if (map == null)
                return;

            if (map.SelectionCount < 1) return;

            IActiveView av = null;
            IScreenDisplay sd = null;
            IIdentifyDialog identifyDialog = null;
            UID pId = null;

            IEnumLayer enumLayer = null;
            ILayer layer = null;
            IFeatureLayer fLayer = null;
            IFeatureSelection fSel = null;
            IEnumIDs enumIds = null;
            try
            {
                av = map as IActiveView;
                sd = av.ScreenDisplay;

                //Create a new IdentifyDialog
                identifyDialog = new IdentifyDialogClass();
                identifyDialog.Map = map;
                identifyDialog.Display = sd;

                //Clear the dialog
                identifyDialog.ClearLayers();

                pId = new UIDClass();
                pId.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(pId, true);
                enumLayer.Reset();
                layer = enumLayer.Next();
                int id; bool found = false;
                while (layer != null)
                {
                    fLayer = (IFeatureLayer)layer;
                    if (fLayer.Valid && fLayer.Visible)
                    {
                        fSel = (IFeatureSelection)fLayer;

                        //Get the ids of all selected features for this layer
                        enumIds = fSel.SelectionSet.IDs;

                        //Step through each and add to identify window
                        enumIds.Reset();
                        for (int i = 1; i <= fSel.SelectionSet.Count; i++)
                        {
                            id = enumIds.Next();
                            found = true;

                            identifyDialog.AddLayerIdentifyOID(layer, id);

                        }
                    }
                    layer = enumLayer.Next();
                }

                //Open Identify Dialog
                if (found)
                {
                    identifyDialog.Show();

                }
            }
            catch
            { }
            finally
            {

                av = null;
                sd = null;
                identifyDialog = null;
                pId = null;

                enumLayer = null;
                layer = null;
                fLayer = null;
                fSel = null;
                enumIds = null;
            }

        }
        public static void ClearSelected(IMap map, bool Refresh, List<esriGeometryType> geoType)
        {



            IEnumLayer pEnumLayer = null;

            ILayer pLay = null;
            try
            {



                pEnumLayer = map.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    try
                    {
                        if (pLay is IFeatureLayer)
                        {
                            if (geoType.Contains(((IFeatureLayer)pLay).FeatureClass.ShapeType))
                            {
                                ((IFeatureSelection)((IFeatureLayer)pLay)).Clear();
                            }
                        }
                    }
                    catch
                    {
                    }
                    pLay = pEnumLayer.Next();
                }




            }
            catch //()//Exception ex)
            {

            }
            finally
            {
                Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;

                pLay = null;
            }
        }
        public static void ClearSelected(IMap map, bool Refresh)
        {
            IActiveView av = null;
            try
            {
                if (map.SelectionCount > 0)
                {
                    if (Refresh)
                    {
                        av = (IActiveView)map;
                        av.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        map.ClearSelection();

                        av.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    }
                    else
                    {
                        map.ClearSelection();
                    }

                }
            }
            catch { }
            finally
            {
                av = null;
            }
        }
        public static void ClearSelected(IApplication app, bool Refresh)
        {
            ClearSelected(((IMxDocument)app.Document).FocusMap, Refresh);

        }
        public static void ClearSelected(IApplication app, bool Refresh, List<esriGeometryType> geoType)
        {
            ClearSelected(((IMxDocument)app.Document).FocusMap, Refresh, geoType);

        }
        public static void FlashGeometry(ESRI.ArcGIS.Geometry.IGeometry geometry, ESRI.ArcGIS.Display.IRgbColor color, ESRI.ArcGIS.Display.IDisplay display, System.Int32 delay)
        {
            ESRI.ArcGIS.Display.ISymbol symbol = null;
            ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = null;
            ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = null;
            ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = null;
            try
            {
                if (geometry == null || color == null || display == null)
                {
                    return;
                }

                display.StartDrawing(display.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache); // Explicit Cast


                switch (geometry.GeometryType)
                {
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                        {
                            //Set the flash geometry's symbol.
                            simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                            simpleFillSymbol.Color = color;
                            symbol = simpleFillSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                            symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                            //Flash the input polygon geometry.
                            display.SetSymbol(symbol);
                            display.DrawPolygon(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolygon(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolygon(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolygon(geometry);
                            break;
                        }

                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                        {
                            //Set the flash geometry's symbol.
                            simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
                            simpleLineSymbol.Width = 4;
                            simpleLineSymbol.Color = color;
                            symbol = simpleLineSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                            symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                            //Flash the input polyline geometry.
                            display.SetSymbol(symbol);
                            display.DrawPolyline(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolyline(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolyline(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPolyline(geometry);

                            break;
                        }

                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                        {
                            //Set the flash geometry's symbol.
                            simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                            simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;
                            simpleMarkerSymbol.Size = 12;
                            simpleMarkerSymbol.Color = color;
                            symbol = simpleMarkerSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                            symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                            //Flash the input point geometry.
                            display.SetSymbol(symbol);
                            display.DrawPoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawPoint(geometry);
                            break;
                        }

                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                        {
                            //Set the flash geometry's symbol.
                            simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                            simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;
                            simpleMarkerSymbol.Size = 12;
                            simpleMarkerSymbol.Color = color;
                            symbol = simpleMarkerSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                            symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                            //Flash the input multipoint geometry.
                            display.SetSymbol(symbol);
                            display.DrawMultipoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawMultipoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawMultipoint(geometry);
                            System.Threading.Thread.Sleep(delay);
                            display.DrawMultipoint(geometry);
                            break;
                        }
                }
                display.FinishDrawing();
            }
            catch
            { }
            finally
            { }

        }

        #endregion

        #region Symbols
        public static IMarkerSymbol FindMarkerSym(string stylename, string stylecategory, string symname, IMxDocument Doc)
        {
            // Usage: Set pMarkerSym = FindMarkerSym("Civic.style", "Default", "Pin Flag Square")

            IStyleGallery pStyGall = null;
            IStyleGalleryStorage pStyGallStor = null;
            IStyleGalleryItem pStyGallItem = null;

            IMarkerSymbol pMrkSym = null;
            IEnumStyleGalleryItem pEnumStyGallItm = null;

            try
            {

                pStyGall = Doc.StyleGallery;
                pStyGallStor = (IStyleGalleryStorage)pStyGall;

                //Set the styleset
                pEnumStyGallItm = pStyGall.get_Items("Marker Symbols", stylename, stylecategory);
                pEnumStyGallItm.Reset();


                //Make sure it contains markers
                pStyGallItem = pEnumStyGallItm.Next();
                if (pStyGallItem == null)
                {

                    return null;
                }
                pEnumStyGallItm.Reset();

                pStyGallItem = pEnumStyGallItm.Next();

                while (pStyGallItem != null)
                {
                    if (pStyGallItem.Name.ToUpper() == symname.ToUpper())
                    {
                        pMrkSym = (IMarkerSymbol)pStyGallItem.Item;
                        break;
                    }
                    pStyGallItem = pEnumStyGallItm.Next();

                }


                if (pMrkSym == null)
                {

                    return null;

                }

                return pMrkSym;
            }
            catch
            {
                return null;
            }
            finally
            {
                pStyGall = null;
                pStyGallStor = null;
                pStyGallItem = null;

                if (pEnumStyGallItm != null)
                    Marshal.ReleaseComObject(pEnumStyGallItm);
                pEnumStyGallItm = null;

            }


        }
        public static ESRI.ArcGIS.Display.ISimpleLineSymbol CreateSimpleLineSymbol(ESRI.ArcGIS.Display.IRgbColor rgbColor, System.Double inWidth, ESRI.ArcGIS.Display.esriSimpleLineStyle inStyle)
        {
            if (rgbColor == null)
            {
                return null;
            }

            ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
            simpleLineSymbol.Style = inStyle;
            simpleLineSymbol.Color = rgbColor;
            simpleLineSymbol.Width = inWidth;

            return simpleLineSymbol;
        }
        public static ESRI.ArcGIS.Display.ISimpleMarkerSymbol CreateSimpleMarkerSymbol(ESRI.ArcGIS.Display.IRgbColor rgbColor, ESRI.ArcGIS.Display.esriSimpleMarkerStyle inputStyle)
        {

            ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Color = rgbColor;
            simpleMarkerSymbol.Style = inputStyle;

            return simpleMarkerSymbol;
        }
        public static ESRI.ArcGIS.Display.IPictureMarkerSymbol CreatePictureMarkerSymbol(ESRI.ArcGIS.Display.esriIPictureType pictureType, System.String filename, System.Double markerSize)
        {


            // Set the Transparent background color for the Picture Marker symbol to white.
            ESRI.ArcGIS.Display.IRgbColor rgbColor = null;
            try
            {
                rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
                rgbColor.Red = 255;
                rgbColor.Green = 255;
                rgbColor.Blue = 255;

                // Create the Marker and assign properties.
                ESRI.ArcGIS.Display.IPictureMarkerSymbol pictureMarkerSymbol = new ESRI.ArcGIS.Display.PictureMarkerSymbolClass();
                pictureMarkerSymbol.CreateMarkerSymbolFromFile(pictureType, filename);
                pictureMarkerSymbol.Angle = 0;
                pictureMarkerSymbol.BitmapTransparencyColor = rgbColor;
                pictureMarkerSymbol.Size = markerSize;
                pictureMarkerSymbol.XOffset = 0;
                pictureMarkerSymbol.YOffset = 0;

                return pictureMarkerSymbol;
            }
            catch
            {
                return null;
            }
            finally
            {
                rgbColor = null;
            }
        }
        public static ISimpleMarkerSymbol CreateNetworkFlagBarrierSymbol(flagType flgType)
        {
            ISimpleMarkerSymbol pSymbolFlag = null;
            switch (flgType)
            {
                case flagType.EdgeFlag:
                    pSymbolFlag = new SimpleMarkerSymbolClass();
                    pSymbolFlag.Style = esriSimpleMarkerStyle.esriSMSSquare;
                    pSymbolFlag.Angle = 0;
                    pSymbolFlag.Color = GetColor(0, 255, 0);
                    pSymbolFlag.Outline = true;
                    pSymbolFlag.OutlineSize = 1;
                    pSymbolFlag.OutlineColor = GetColor(0, 0, 0);
                    pSymbolFlag.Size = 10; //TODO: UserConfig
                    break;
                case flagType.JunctionFlag:

                    pSymbolFlag = new SimpleMarkerSymbolClass();
                    pSymbolFlag.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSymbolFlag.Angle = 0;
                    pSymbolFlag.Color = GetColor(0, 255, 0);
                    pSymbolFlag.Outline = true;
                    pSymbolFlag.OutlineSize = 1;
                    pSymbolFlag.OutlineColor = GetColor(0, 0, 0);
                    pSymbolFlag.Size = 10; //TODO: UserConfig
                    break;
                case flagType.EdgeBarrier:


                    pSymbolFlag = new SimpleMarkerSymbolClass();
                    pSymbolFlag.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                    pSymbolFlag.Angle = 0;
                    pSymbolFlag.Color = GetColor(255, 0, 0);
                    pSymbolFlag.Outline = true;
                    pSymbolFlag.OutlineSize = 1;
                    pSymbolFlag.OutlineColor = GetColor(0, 0, 0);
                    pSymbolFlag.Size = 10; //TODO: UserConfig
                    break;
                case flagType.JunctionBarrier:

                    pSymbolFlag = new SimpleMarkerSymbolClass();
                    pSymbolFlag.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                    pSymbolFlag.Angle = 0;
                    pSymbolFlag.Color = GetColor(255, 0, 0);
                    pSymbolFlag.Outline = true;
                    pSymbolFlag.OutlineSize = 1;
                    pSymbolFlag.OutlineColor = GetColor(0, 0, 0);
                    pSymbolFlag.Size = 10; //TODO: UserConfig
                    break;
                default:
                    pSymbolFlag = new SimpleMarkerSymbolClass();
                    pSymbolFlag.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSymbolFlag.Angle = 0;
                    pSymbolFlag.Color = GetColor(0, 255, 0);
                    pSymbolFlag.Outline = true;
                    pSymbolFlag.OutlineSize = 1;
                    pSymbolFlag.OutlineColor = GetColor(0, 0, 0);
                    pSymbolFlag.Size = 10; //TODO: UserConfig
                    break;
            }
            return pSymbolFlag;

        }

        #endregion

        #region GeometryTools
        public static ISet splitLineWithPoint(IFeature lineFeature, IPoint SplitPoint, double SnapTol, IList<MergeSplitFlds> pFldsNames, string SplitFormatString, IApplication app)
        {
            IHitTest hitTest = null;
            IFeatureEdit2 featureEdit = null;
            IPoint pHitPnt = null;
            ITopologicalOperator pTopoOp = null;
            IPolygon pPoly = null;
            IRelationalOperator pRelOp = null;
            ICurve pCurve = null;
            ISet pSet = null;
            IFeature pSplitResFeat = null;
            try
            {


                featureEdit = lineFeature as IFeatureEdit2;
                hitTest = lineFeature.ShapeCopy as IHitTest;
                pHitPnt = new PointClass();
                double pHitDist = -1;
                int pHitPrt = -1;
                int pHitSeg = -1;
                bool pHitSide = false;
                bool hit = hitTest.HitTest(SplitPoint, SnapTol, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPnt, pHitDist, pHitPrt, pHitSeg, pHitSide);
                double dblMidVal = -1;
                if (hit)
                {

                    pCurve = lineFeature.ShapeCopy as ICurve;

                    //Split feature
                    pTopoOp = pHitPnt as ITopologicalOperator;

                    pPoly = pTopoOp.Buffer(Globals.ConvertFeetToMapUnits(.1, app) * 2) as IPolygon;
                    pRelOp = pPoly as IRelationalOperator;


                    if (pRelOp != null)
                    {

                        if ((pRelOp.Contains(pCurve.FromPoint)) ||
                                (pRelOp.Contains(pCurve.ToPoint)))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        pRelOp = pHitPnt as IRelationalOperator;
                        if ((pRelOp.Contains(pCurve.FromPoint)) ||
                              (pRelOp.Contains(pCurve.ToPoint)))
                        {
                            return null;
                        }
                    }

                    //Split feature
                    //Globals.FlashGeometry(pHitPnt, Globals.GetColor(255, 0, 0), mxdoc.ActiveView.ScreenDisplay, 150);


                    double dblHighVal = 0;
                    double dblLowVal = 0;
                    int intHighIdx = -1;
                    int intLowIdx = -1;
                    if (pFldsNames != null)
                    {
                        foreach (MergeSplitFlds FldNam in pFldsNames)
                        {
                            FldNam.Value = lineFeature.get_Value(FldNam.FieldIndex).ToString();
                            if (FldNam.SplitType.ToUpper() == "MAX")
                            {
                                if (FldNam.Value != null)
                                {
                                    if (FldNam.Value != "")
                                    {

                                        dblHighVal = Convert.ToDouble(FldNam.Value);
                                        intHighIdx = FldNam.FieldIndex;
                                    }
                                }
                            }
                            else if (FldNam.SplitType.ToUpper() == "MIN")
                            {
                                if (FldNam.Value != null)
                                {
                                    if (FldNam.Value != "")
                                    {

                                        dblLowVal = Convert.ToDouble(FldNam.Value);
                                        intLowIdx = FldNam.FieldIndex;
                                    }
                                }
                            }


                        }
                    }
                    if (intHighIdx > -1 && intLowIdx > -1)
                    {
                        double len = ((ICurve)(lineFeature.Shape as IPolyline)).Length;

                        double splitDist = Globals.PointDistanceOnLine(pHitPnt, lineFeature.Shape as IPolyline, 2, out pHitPnt);
                        double percentSplit = splitDist / len;

                        if (SplitFormatString == "")
                        {
                            dblMidVal = dblLowVal + ((dblHighVal - dblLowVal) * percentSplit);
                        }
                        else
                        {
                            dblMidVal = Convert.ToDouble(string.Format(SplitFormatString, dblLowVal + ((dblHighVal - dblLowVal) * percentSplit)));

                        }

                    }
                    //Split feature
                    try
                    {
                        pSet = featureEdit.SplitWithUpdate(pHitPnt);
                    }
                    catch
                    {
                        try
                        {
                            pSet = featureEdit.Split(pHitPnt);
                        }
                        catch
                        {
                            return null;
                        }
                    }


                    if (intHighIdx > -1 && intLowIdx > -1)
                    {
                        if (pSet.Count == 1)
                        {
                            while ((pSplitResFeat = pSet.Next() as IFeature) != null)
                            {
                                if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                                {
                                    pSplitResFeat.set_Value(intHighIdx, dblMidVal);
                                }
                                else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                                {
                                    pSplitResFeat.set_Value(intLowIdx, dblMidVal);

                                }
                            }
                            if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            {
                                lineFeature.set_Value(intHighIdx, dblMidVal);
                            }
                            else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            {
                                lineFeature.set_Value(intLowIdx, dblMidVal);

                            }
                        }
                        else if (pSet.Count == 2)
                        {
                            while ((pSplitResFeat = pSet.Next() as IFeature) != null)
                            {
                                if ((pSplitResFeat.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                                {
                                    pSplitResFeat.set_Value(intHighIdx, dblMidVal);
                                }
                                else if ((pSplitResFeat.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (pSplitResFeat.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                                {
                                    pSplitResFeat.set_Value(intLowIdx, dblMidVal);

                                }
                            }
                            //if ((lineFeature.ShapeCopy as IPolyline).FromPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).FromPoint.Y == pHitPnt.Y)
                            //{
                            //    lineFeature.set_Value(intHighIdx, dblMidVal);
                            //}
                            //else if ((lineFeature.ShapeCopy as IPolyline).ToPoint.X == pHitPnt.X && (lineFeature.ShapeCopy as IPolyline).ToPoint.Y == pHitPnt.Y)
                            //{
                            //    lineFeature.set_Value(intLowIdx, dblMidVal);

                            //}
                        }
                    }


                }
                return pSet;
            }
            catch (Exception ex)
            {
                return null;

            }
            finally
            {
                hitTest = null;
                featureEdit = null;
                pHitPnt = null;
                pTopoOp = null;
                pPoly = null;
                pRelOp = null;
                pCurve = null;
                //pSet = null;
                pSplitResFeat = null;
            }
        }

        public static double GetXYTolerance(IFeatureLayer FeatureLayer)
        {

            bool hasXY;
            ISpatialReferenceResolution pSRResolution = null;
            try
            {
                if (FeatureLayer == null)
                    return .000001;
                if (FeatureLayer.FeatureClass == null)
                    return .000001;
                if ((FeatureLayer.FeatureClass as IGeoDataset).SpatialReference == null)
                    return .000001;

                hasXY = ((FeatureLayer.FeatureClass as IGeoDataset).SpatialReference).HasXYPrecision();

                if (hasXY)
                {


                    pSRResolution = ((FeatureLayer.FeatureClass as IGeoDataset).SpatialReference) as ISpatialReferenceResolution;
                    double dblTol;

                    dblTol = pSRResolution.get_XYResolution(false);
                    if (dblTol < .0000000001)
                    {
                        dblTol = .00000001;
                    }
                    return dblTol;
                }
                else
                {
                    return .000001;

                }
            }
            catch
            {
                return .000001;
            }
            finally
            {
                pSRResolution = null;
            }
        }
        public static Boolean pointscoincident(IPoint pntOne, IPoint pntTwo)
        {

            double tol = GetXYTolerance(pntOne);
            tol = tol * 2;
            if ((Math.Abs(pntOne.X - pntTwo.X) <= tol) &&
                Math.Abs(pntOne.Y - pntTwo.Y) <= tol)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static double GetXYTolerance(IFeature Feature)
        {

            bool hasXY;
            ISpatialReferenceResolution pSRResolution = null;
            try
            {
                if (Feature == null)
                    return .000001;

                if (Feature.Shape.SpatialReference == null)
                    return .000001;

                hasXY = (Feature.Shape.SpatialReference).HasXYPrecision();

                if (hasXY)
                {


                    pSRResolution = (Feature.Shape.SpatialReference) as ISpatialReferenceResolution;
                    double dblTol;

                    dblTol = pSRResolution.get_XYResolution(false);
                    if (dblTol < .0000000001)
                    {
                        dblTol = .00000001;
                    }
                    return dblTol;
                }
                else
                {
                    return .000001;

                }
            }
            catch
            {
                return .000001;
            }
            finally
            {
                pSRResolution = null;
            }
        }

        public static double GetXYTolerance(IGeometry Geo)
        {

            bool hasXY;
            ISpatialReferenceResolution pSRResolution = null;
            try
            {
                if (Geo == null)
                    return .000001;

                if (Geo.SpatialReference == null)
                    return .000001;

                hasXY = (Geo.SpatialReference).HasXYPrecision();

                if (hasXY)
                {


                    pSRResolution = (Geo.SpatialReference) as ISpatialReferenceResolution;
                    double dblTol;

                    dblTol = pSRResolution.get_XYResolution(false);
                    if (dblTol < .0000000001)
                    {
                        dblTol = .00000001;
                    }
                    return dblTol;
                }
                else
                {
                    return .000001;

                }
            }
            catch
            {
                return .000001;
            }
            finally
            {
                pSRResolution = null;
            }
        }


        public static IGeometry PointToGeometry(IPoint pf)
        {
            return pf as IGeometry;

        }
        public static List<IPoint> GetGeomCenter(IFeature Feature)
        {

            try
            {
                return GetGeomCenter(Feature.ShapeCopy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - GetGeomCenter" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<IPoint> GetGeomCenter(IGeometry geo)
        {


            IPoint pCenterPoint = null;
            List<IPoint> pCenterPoints = new List<IPoint>();
            IArea pArea = null;
            IPolyline pl = null;
            IPolygon4 pPoly = null;
            IEnumGeometry enumGeometry = null;
            IGeometry geometry = null;
            IGeometryBag pGeoBag = null;
            if (geo == null)
            {
                return pCenterPoints;
            }
            try
            {



                switch (geo.GeometryType)
                {

                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                        //pCenterPoint = geo as ESRI.ArcGIS.Geometry.IPoint;
                        pCenterPoints.Add(geo as ESRI.ArcGIS.Geometry.IPoint);

                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:

                        pl = (ESRI.ArcGIS.Geometry.IPolyline)geo;
                        pCenterPoint = new ESRI.ArcGIS.Geometry.Point();
                        //int prtIdx;
                        //int segIdx;
                        //bool bSplit;

                        //pl.SplitAtDistance(50, true, false, out bSplit, out prtIdx, out segIdx);


                        pl.QueryPoint(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, .5, true, pCenterPoint);
                        pCenterPoints.Add(pCenterPoint);
                        pl = null;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                        pPoly = geo as ESRI.ArcGIS.Geometry.IPolygon4;
                        pPoly.SimplifySpaghetti();

                        if (pPoly.ExteriorRingCount > 1)
                        {
                            //Comps = new IPolygon[pPoly.ExteriorRingCount];
                            pGeoBag = pPoly.ConnectedComponentBag;
                            enumGeometry = pGeoBag as IEnumGeometry;

                            enumGeometry.Reset();
                            geometry = enumGeometry.Next();

                            while (geometry != null)
                            {
                                pArea = geometry as ESRI.ArcGIS.Geometry.IArea;
                                pCenterPoint = pArea.Centroid;
                                pCenterPoints.Add(pCenterPoint);
                                geometry = enumGeometry.Next();

                            }

                        }
                        else
                        {
                            pArea = geo as ESRI.ArcGIS.Geometry.IArea;
                            pCenterPoint = pArea.Centroid;
                            pCenterPoints.Add(pCenterPoint);
                            pArea = null;
                        }
                        break;
                }

                return pCenterPoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - GetGeomCenter" + Environment.NewLine + ex.Message);
                return null;
            }
            finally
            {
                pCenterPoint = null;

                pArea = null;
                pl = null;
                pPoly = null;
                enumGeometry = null;
                geometry = null;
                pGeoBag = null;

            }
        }

        //private static  midPointOfLine(Polyline geo)
        //{
        //    // Gets the selected geometry as a polyline
        //    IPolyline polyline = geo as Polyline;

        //    // Gets the polyline's part zero (0)
        //    CoordinateCollection coordinateCollection = polyline.Parts[0];

        //    // Gets the coordinate count in the collection minus 1
        //    int count = coordinateCollection.Count - 1;

        //    // Creates a array of double to that it will contain the distance
        //    // between each segment of the polyline
        //    double[] sumDistance = new double[count];

        //    // Contains the summation of all the segments in the polyline
        //    double sum = 0;

        //    // Loops throught the coordinates and calculates the
        //    // distance between each pair of coordinates
        //    for (int i = 0; i < count; i++)
        //    {
        //        // Calculates the delta in x
        //        double dx = (coordinateCollection[i + 1].X - coordinateCollection[i].X);
        //        dx *= dx;

        //        // Calculates the delta in y
        //        double dy = (coordinateCollection[i + 1].Y - coordinateCollection[i].Y);
        //        dy *= dy;

        //        // Calculates the square distance of the deltas
        //        // and sums the real distance of the segment
        //        sum += Math.Sqrt(dx + dy);

        //        // Stores the value in an array
        //        sumDistance[i] = sum;
        //    }

        //    // Calculates the mid distance of the polyline
        //    double mid = sum / 2;

        //    double min = 0;
        //    int index = 0;
        //    sum = 0;

        //    // Finds the last distance of the polyline right
        //    // before where the mid distance is located.
        //    for (int i = 0; i < count; i++)
        //    {
        //        if (sumDistance[i] < mid)
        //        {
        //            // Gets the minimun distance
        //            min = sumDistance[i];

        //            // Gets the coordinate index where the mid length is located
        //            index = i + 1;
        //        }
        //        else
        //            break;
        //    }

        //    // Calculates the distance of the segment between the
        //    // pair of coordinate that contains the mid distance
        //    // of the polyline
        //    double distance = mid - min;

        //    // Gets the coordinate
        //    Coordinate coordinate = coordinateCollection[index];

        //    // Calculates the angle of the segment
        //    double angle = Math.Atan2((coordinateCollection[index + 1].Y - coordinate.Y),
        //      (coordinateCollection[index + 1].X - coordinate.X));

        //    // Calculate the sine and cosine of the rotation angle.
        //    double sine = Math.Sin(angle);
        //    double cosine = Math.Cos(angle);

        //    // Rotate the coordinate
        //    double xn = (cosine * distance) + coordinate.X;
        //    double yn = (sine * distance) + coordinate.Y;

        //    // Creates a point that shows the mid coordinate
        //    // of the polyline after invalidating the map
        //    m_geometry = new ESRI.ArcGIS.Mobile.Geometries.Point(xn, yn);

        //    // Redraw the client area of the map
        //    // calls Map.Paint() event
        //    map1.Invalidate();
        //}

        public static IEnvelope CalcSearchExtent(IFeatureLayer sourceLayer, IFeature inFeature, double SearchTolerence)
        {
            IEnvelope searchEnvelope = null;

            try
            {
                ISpatialReferenceResolution pSRResolution;

                pSRResolution = ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference) as ISpatialReferenceResolution;

                //  sFilter = new SpatialFilterClass();
                double dblTol;
                if (SearchTolerence != 0.0)
                {
                    dblTol = SearchTolerence;
                }
                else
                {
                    dblTol = pSRResolution.get_XYResolution(false);
                }
                // bool hasXY = ((sourceLayer.FeatureClass as IGeoDataset).SpatialReference).HasXYPrecision();

                searchEnvelope = inFeature.ShapeCopy.Envelope;
                searchEnvelope.XMin = searchEnvelope.XMin - dblTol;
                searchEnvelope.YMin = searchEnvelope.YMin - dblTol;
                searchEnvelope.XMax = searchEnvelope.XMax + dblTol;
                searchEnvelope.YMax = searchEnvelope.YMax + dblTol;
                searchEnvelope.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;



                //searchEnvelope.Expand(.1, .1, true);
                //searchEnvelope.Expand(searchDistance, searchDistance, false);
            }
            catch
            {
                double dblTol;

                if (SearchTolerence != 0)
                {
                    dblTol = SearchTolerence;
                }
                else
                {
                    dblTol = .01;
                }

                searchEnvelope = inFeature.ShapeCopy.Envelope;
                searchEnvelope.XMin = searchEnvelope.XMin - dblTol;
                searchEnvelope.YMin = searchEnvelope.YMin - dblTol;
                searchEnvelope.XMax = searchEnvelope.XMax + dblTol;
                searchEnvelope.YMax = searchEnvelope.YMax + dblTol;
                searchEnvelope.SpatialReference = ((inFeature.Class as IFeatureClass) as IGeoDataset).SpatialReference;


            }
            return searchEnvelope;
        }
        public static IPolygon Env2Polygon(IEnvelope pEnv)
        {
            IPointCollection pPointColl = default(IPointCollection);
            IPolygon pEnvPoly = default(IPolygon);
            try
            {

                pPointColl = new Polygon();
                object before = null;
                object after = null;
                pPointColl.AddPoint(pEnv.LowerLeft);
                pPointColl.AddPoint(pEnv.UpperLeft);
                pPointColl.AddPoint(pEnv.UpperRight);
                pPointColl.AddPoint(pEnv.LowerRight);
                pPointColl.AddPoint(pEnv.LowerLeft);

                //pPointColl.AddPoint(pEnv.LowerLeft, ref before, ref after);
                //pPointColl.AddPoint(pEnv.UpperLeft, ref before, ref after);
                //pPointColl.AddPoint(pEnv.UpperRight, ref before, ref after);
                //pPointColl.AddPoint(pEnv.LowerRight, ref before, ref after);
                //pPointColl.AddPoint(pEnv.LowerLeft, ref before, ref after);
                pEnvPoly = (IPolygon)pPointColl;
                pEnvPoly.SpatialReference = pEnv.SpatialReference;
                pEnvPoly.Close();

                return pEnvPoly;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - Env2Polygon" + Environment.NewLine + ex.Message);
                return null;
            }
            finally
            {
                pPointColl = null;
                pEnvPoly = null;

            }
        }

        public static List<IFeature> GetIntersectingFeatures(IGeometry pSourceGeo, IFeatureLayer pLayerToSearch, bool boolSearchLayer, bool boolRecycle, int IgnoreOID, ISpatialReference mapSpatRef)
        {

            ISpatialFilter pSpatFilt = null;
            IFeatureCursor pFeatCurs = null;
            List<IFeature> pFeatRet = null;
            IFeature pFeat = null;
            try
            {
                pSpatFilt = createSpatialFilter(pLayerToSearch, pSourceGeo, false, mapSpatRef);
                if (boolSearchLayer)
                    pFeatCurs = pLayerToSearch.Search(pSpatFilt, boolRecycle);
                else
                    pFeatCurs = pLayerToSearch.FeatureClass.Search(pSpatFilt, boolRecycle);

                pFeatRet = new List<IFeature>();
                pFeat = pFeatCurs.NextFeature();
                while (pFeat != null)
                {
                    pFeatRet.Add(pFeat);
                    pFeat = pFeatCurs.NextFeature();

                }
                return pFeatRet;
            }
            catch
            {
                return null;
            }
            finally
            {
                pSpatFilt = null;
                if (pFeatCurs != null)
                {
                    Marshal.ReleaseComObject(pFeatCurs);
                }
                pFeatCurs = null;

                pFeat = null;
            }



        }
        public static List<IGeometry> GetIntersectingGeometry(IGeometry pSourceGeo, IFeatureLayer pLayerToSearch, bool boolSearchLayer, bool boolRecycle, int IgnoreOID, ISpatialReference mapSpatRef)
        {

            ISpatialFilter pSpatFilt = null;
            IFeatureCursor pFeatCurs = null;
            List<IGeometry> pGeoRet = null;
            IFeature pFeat = null;
            try
            {
                pSpatFilt = createSpatialFilter(pLayerToSearch, pSourceGeo, false, mapSpatRef);
                if (boolSearchLayer)
                    pFeatCurs = pLayerToSearch.Search(pSpatFilt, boolRecycle);
                else
                    pFeatCurs = pLayerToSearch.FeatureClass.Search(pSpatFilt, boolRecycle);

                pGeoRet = new List<IGeometry>();
                pFeat = pFeatCurs.NextFeature();
                while (pFeat != null)
                {
                    pGeoRet.Add(pFeat.ShapeCopy);
                    pFeat = pFeatCurs.NextFeature();

                }
                return pGeoRet;
            }
            catch
            {
                return null;
            }
            finally
            {
                pSpatFilt = null;
                if (pFeatCurs != null)
                {
                    Marshal.ReleaseComObject(pFeatCurs);
                }
                pFeatCurs = null;

                pFeat = null;
            }



        }

        public static List<int> GetIntersectingFeaturesOIDs(IGeometry pSourceGeo, IFeatureLayer pLayerToSearch, bool boolSearchLayer, int IgnoreOID, ISpatialReference mapSpatRef)
        {

            ISpatialFilter pSpatFilt = null;
            IFeatureCursor pFeatCurs = null;
            List<int> pFeatRet = null;
            IFeature pFeat = null;
            try
            {
                pSpatFilt = createSpatialFilter(pLayerToSearch, pSourceGeo, false, mapSpatRef);

                //   bool fit = pSpatFilt.FilterOwnsGeometry;




                if (boolSearchLayer)
                    pFeatCurs = pLayerToSearch.Search(pSpatFilt, true);
                else
                {
                    //  int tempCnt = pLayerToSearch.FeatureClass.FeatureCount(pSpatFilt);

                    pFeatCurs = pLayerToSearch.FeatureClass.Search(pSpatFilt, true);
                }
                pFeatRet = new List<int>();


                while ((pFeat = pFeatCurs.NextFeature()) != null)
                {
                    if (pFeat.OID != IgnoreOID)
                    {
                        pFeatRet.Add(pFeat.OID);
                    }

                }
                return pFeatRet;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                pSpatFilt = null;
                if (pFeatCurs != null)
                {
                    Marshal.ReleaseComObject(pFeatCurs);
                }
                pFeatCurs = null;

                pFeat = null;
            }



        }
        public static IGeometry GetIntersection(IGeometry pIntersect, IPolyline pOther)
        {
            ITopologicalOperator2 pTopoOp = null;
            ITopologicalOperator2 pTopoInt = null;
            IPolyline pBoundary = null;
            IGeometry pGeomResult = null;
            IPointCollection pPointColl = null;
            esriGeometryDimension d;
            try
            {
                //IClone pClone = null;
                //pClone = pIntersect.SpatialReference;

                //If Not pClone.IsEqual(pOther.SpatialReference) Then
                //  pOther.Project pIntersect.SpatialReference
                //End If

                // Check the input line is simple.


                pTopoInt = pOther as ITopologicalOperator2;
                pTopoInt.IsKnownSimple_2 = false;
                pTopoInt.Simplify();

                // We assume the feature geometry is simple as it belongs to a feature class.
                if (pIntersect is IPolygon)
                {

                    pTopoOp = pIntersect as ITopologicalOperator2;
                    pBoundary = (IPolyline)pTopoOp.Boundary;
                    pTopoOp = pBoundary as ITopologicalOperator2;


                }
                else
                    pTopoOp = pIntersect as ITopologicalOperator2;

                // Perform the intersection.
                pTopoOp.IsKnownSimple_2 = false;
                pTopoOp.Simplify();

                if (pIntersect.SpatialReference != pOther.SpatialReference)
                {
                    pOther.Project(pIntersect.SpatialReference);

                }
                pOther.SnapToSpatialReference();
                pIntersect.SnapToSpatialReference();

                d = esriGeometryDimension.esriGeometry0Dimension;

                pGeomResult = pTopoOp.Intersect((IGeometry)pTopoInt, d);
                if (pGeomResult == null)
                    return null;
                if (pGeomResult.IsEmpty == true)
                    return null;
                // If there is more than one point of intersection, the first point will be used.
                if (pGeomResult is IPointCollection)
                {

                    pPointColl = pGeomResult as IPointCollection;
                    if (pPointColl.PointCount >= 1)
                        pGeomResult = pPointColl.get_Point(0);
                    else
                        return null;

                }
                if (!(pGeomResult.GeometryType == esriGeometryType.esriGeometryPoint))
                {
                    // Some problem with the result of the intersection.
                    return null;
                }

                return pGeomResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Get Intersection: " + ex.Message);
                return null;
            }
            finally
            {
                pTopoOp = null;
                pTopoInt = null;
                pBoundary = null;
                pGeomResult = null;
                pPointColl = null;

            }
        }

        //public static IGeometry GetIntersection(IGeometry pIntersect, IPolyline pOther)
        //{
        //    //IClone pClone = null;
        //    //pClone = pIntersect.SpatialReference;

        //    //If Not pClone.IsEqual(pOther.SpatialReference) Then
        //    //  pOther.Project pIntersect.SpatialReference
        //    //End If

        //    // Check the input line is simple.
        //    ITopologicalOperator pTopoOp;
        //    pTopoOp = pOther as ITopologicalOperator;
        //    pTopoOp.Simplify();

        //    // We assume the feature geometry is simple as it belongs to a feature class.
        //    if (pIntersect is IPolygon)
        //    {
        //        IPolyline pBoundary;
        //        pTopoOp = pIntersect as ITopologicalOperator;
        //        pBoundary = (IPolyline)pTopoOp.Boundary;
        //        pTopoOp = pBoundary as ITopologicalOperator;


        //    }
        //    else
        //        pTopoOp = pIntersect as ITopologicalOperator;

        //    // Perform the intersection.
        //    IGeometry pGeomResult;

        //    pGeomResult = pTopoOp.Intersect(pOther, esriGeometryDimension.esriGeometry0Dimension);
        //    if (pGeomResult == null)
        //        return null;
        //    if (pGeomResult.IsEmpty == true)
        //        return null;
        //    // If there is more than one point of intersection, the first point will be used.
        //    if (pGeomResult is IPointCollection)
        //    {
        //        IPointCollection pPointColl;
        //        pPointColl = pGeomResult as IPointCollection;
        //        if (pPointColl.PointCount >= 1)
        //            pGeomResult = pPointColl.get_Point(0);
        //        else
        //            return null;

        //    }
        //    if (!(pGeomResult.GeometryType == esriGeometryType.esriGeometryPoint))
        //    {
        //        // Some problem with the result of the intersection.
        //        return null;
        //    }

        //    return pGeomResult;
        //}

        public static double GetGeometryLength(IFeature pFeature)
        { //helper function to get the area/length/perimeter of a feature

            IFeatureClass pFC = null;
            IFields pvFlds = null;
            try
            {
                pFC = (IFeatureClass)pFeature.Class;

                pvFlds = pFC.Fields;

                if (pFC.ShapeType == esriGeometryType.esriGeometryNull)
                    return 0;
                else
                    return (double)pFeature.get_Value(pvFlds.FindField(pFC.LengthField.Name));
            }
            catch
            {
                return 0;
            }
            finally
            {
                pFC = null;
                pvFlds = null;
            }



        }
        public static double GetLineLength(IPolyline pPolyline)
        {
            ICurve iCurv = null;
            IMAware iAw = null;
            //IMSegmentation iMegSeg = null;

            try
            {
                if (pPolyline == null)
                    return 0;
                if (pPolyline.IsEmpty)
                    return 0;

                iCurv = pPolyline as ICurve;
                iAw = pPolyline as IMAware;

                if (iAw.MAware)
                {

                    //iMegSeg = iCurv as IMSegmentation;
                    //return iMegSeg.MMax;
                    return iCurv.Length;

                }
                else
                {
                    return iCurv.Length;
                }

            }
            catch
            {
                return 0;
            }
            finally
            {
                iCurv = null;
                iAw = null;

            }



        }
        public static IPoint GetPointOnLine(IGeometry inPoint, IGeometry InLine, double snapTol, out bool RightSide)
        {
            if (inPoint.GeometryType != esriGeometryType.esriGeometryPoint && InLine.GeometryType != esriGeometryType.esriGeometryPolyline)
            {
                RightSide = false;

                return null;

            }
            else
            {
                return GetPointOnLine(inPoint as IPoint, InLine as IPolyline, snapTol, out RightSide);

            }
        }
        public static IPoint GetPointOnLine(IPoint inPoint, IPolyline InLine, double snapTol, out bool RightSide)
        {
            RightSide = true;
            IHitTest pHt = null;
            IPoint pHitPnt = null;

            try
            {
                pHt = (IHitTest)InLine;
                pHitPnt = new PointClass();

                double hitDistance = 0;
                int hitPartIndex = 0;
                int hitSegmentIndex = 0;
                bool foundGeometry = pHt.HitTest(inPoint, snapTol,
                    ESRI.ArcGIS.Geometry.esriGeometryHitPartType.esriGeometryPartBoundary,
                    pHitPnt, ref hitDistance, ref hitPartIndex, ref hitSegmentIndex, ref
                                RightSide);

                if (foundGeometry)
                {
                    return pHitPnt;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the PointOnLine" + ex.Message);

                return null;
            }
            finally
            {
                pHt = null;
                //pHitPnt = null;

            }







        }
        public static double GetDistanceBetweenPoints(IPoint onePoint, IPoint twoPoint)
        {
            IProximityOperator pProx = null;
            try
            {
                pProx = onePoint as IProximityOperator;
                return pProx.ReturnDistance(twoPoint);

            }
            catch
            {
                return -99999.9;
            }
            finally
            {
                pProx = null;
            }
        }

        public static IPoint CreatePointFromDistanceOnLine(double distance, IPolyline line, bool reverse)
        {
            IConstructPoint2 pConstructPoint = null;
            IPoint pPoint = null;
            ICurve pCurve = null;
            try
            {
                pCurve = (ICurve)line;
                pPoint = new PointClass();
                pConstructPoint = (IConstructPoint2)pPoint;
                if (reverse)
                {
                    distance = pCurve.Length - distance;

                }
                pConstructPoint.ConstructAlong(pCurve, esriSegmentExtension.esriNoExtension, distance, false);
                return pPoint;
            }
            catch
            {
                return null;
            }
            finally
            {
                pConstructPoint = null;
                pCurve = null;
            }
        }
        public static double PointDistanceOnLine(IPoint inPoint, IPolyline InLine, int DecimalPlaces, out IPoint SnapPnt)
        {
            SnapPnt = new PointClass();

            if (inPoint == null)
                return -1;
            if (inPoint.IsEmpty)
                return -1;
            if (InLine == null)
                return -1;
            if (InLine.IsEmpty)
                return -1;

            ICurve iCurv = null;
            IMAware iAw = null;
            IMSegmentation iMegSeg = null;


            try
            {


                double outDistAlongCurve = 0.0;
                double outDistFromCurve = 0.0;
                iCurv = InLine as ICurve;

                iCurv.QueryPointAndDistance(esriSegmentExtension.esriExtendAtFrom, inPoint, false, SnapPnt, ref outDistAlongCurve, ref outDistFromCurve, false);
                iAw = InLine as IMAware;

                if (iAw.MAware && outDistAlongCurve >= 0)
                {

                    iMegSeg = iCurv as IMSegmentation;

                    if (Globals.IsNumeric(iMegSeg.MMax.ToString()) == false || Globals.IsNumeric(iMegSeg.MMin.ToString()) == false)
                    {
                        return Convert.ToDouble(outDistAlongCurve.ToString(string.Format("N", DecimalPlaces)));
                    }
                    else if (iMegSeg.MMax != iMegSeg.MMin)
                    {
                        object iMs = null;
                        string[] pStr;
                        double[] pDbl;
                        iMs = iMegSeg.GetMsAtDistance(outDistAlongCurve, false);
                        pStr = iMs as string[];
                        pDbl = iMs as double[];
                        if (pDbl.Length > 0)
                        {
                            if (Globals.IsNumeric(pDbl[0].ToString()) == false)

                                //return outDistAlongCurve;
                                return Convert.ToDouble(outDistAlongCurve.ToString(string.Format("N", DecimalPlaces)));
                            else
                                return Convert.ToDouble(pDbl[0].ToString(string.Format("N", DecimalPlaces)));
                            //return pDbl[0];
                        }
                        else
                            return Convert.ToDouble(outDistAlongCurve.ToString(string.Format("N", DecimalPlaces)));
                        //return outDistAlongCurve;


                    }
                    else
                    {


                        return Convert.ToDouble(outDistAlongCurve.ToString(string.Format("N", DecimalPlaces)));
                        //  return outDistAlongCurve;

                    }
                }
                else
                    return Convert.ToDouble(outDistAlongCurve.ToString(string.Format("N", DecimalPlaces)));
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error in the PointDistanceOnLine" + Ex.Message);

                return -1;
            }
            finally
            {


                iCurv = null;
                iAw = null;
                iMegSeg = null;
            }


        }
        public static IPolyline CreatePolylineFromPoints(params IPoint[] points)
        {
            object Missing = null;
            IPolyline newPolyLine = null;
            IPointCollection newPointColl = null;
            try
            {
                Missing = Type.Missing;
                newPolyLine = new PolylineClass();
                newPointColl = (IPointCollection)newPolyLine;

                foreach (IPoint pnt in points)
                {
                    if (pnt != null && !pnt.IsEmpty)
                    {
                        pnt.Z = 0;
                        pnt.M = 0;
                        newPointColl.AddPoint(pnt, ref Missing, ref Missing);
                    }
                }
                if (newPointColl.PointCount < 2)
                    newPolyLine = new PolylineClass();

                return newPolyLine;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in CreatePolylineFromPoints: " + ex.Message);
                return null;
            }
            finally
            {

                Missing = null;

                newPointColl = null;
            }

        }

        public static IPolyline CreatePolylineFromPointsNewTurn(IPoint fromPoint, IPoint turnPoint, IPoint toPoint, ref IFeatureLayer pMainLayer, ref IFeature lineFeature, bool SearchOnLayer, double angle, ISpatialReference mapSpat)
        {
            object Missing = null;
            IPolyline newPolyLine = null;
            IPointCollection newPointColl = null;

            ILine straightLine = null;
            ILine angleLine = null;
            IPoint pNewPnt = null;
            IConstructPoint2 pConsPoint = null;
            IClone pCl = null;

            IPoint pStraightLinePoint = null;
            List<IGeometry> pIntGeo = null;
            IPoint pIntPnt = null;
            //ITopologicalOperator pTopoOptr = null;
            //IGeometryCollection pGeomColl = null;
            try
            {
                Missing = Type.Missing;
                newPolyLine = new PolylineClass();
                newPointColl = (IPointCollection)newPolyLine;
                if (turnPoint == null)
                {
                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);

                    newPointColl.AddPoint(toPoint, ref Missing, ref Missing);

                }
                else
                {

                    double dblTol = 1.0;

                    lineFeature = Globals.GetClosestFeature(toPoint as IGeometry, pMainLayer, dblTol, SearchOnLayer, false);

                    ESRI.ArcGIS.Geodatabase.esriFlowDirection lineFlow = esriFlowDirection.esriFDIndeterminate;
                    if (lineFeature == null)
                    {
                        lineFeature = Globals.GetClosestFeature(fromPoint, pMainLayer, dblTol, SearchOnLayer, false);
                        lineFlow = Globals.GetFlowDirectionAtLocation(lineFeature, pMainLayer, toPoint, dblTol);

                    }
                    else
                    {
                        lineFlow = Globals.GetFlowDirectionAtLocation(lineFeature, pMainLayer, fromPoint, dblTol);

                    }

                    straightLine = new LineClass();
                    straightLine.FromPoint = fromPoint;
                    straightLine.ToPoint = turnPoint;

                    angleLine = new LineClass();
                    angleLine.FromPoint = turnPoint;
                    angleLine.ToPoint = toPoint;

                    pNewPnt = new PointClass();
                    pConsPoint = (IConstructPoint2)pNewPnt;
                    double degs = angleLine.Angle * (180 / Math.PI);
                    if (degs < 0)
                        degs = 360 + degs;

                    double rads = (degs + angle) * (Math.PI / 180);

                    pCl = (IClone)turnPoint;

                    pStraightLinePoint = (IPoint)pCl.Clone();

                    pConsPoint.ConstructAngleDistance(turnPoint, rads, angleLine.Length * 2);


                    angleLine.FromPoint = turnPoint;
                    angleLine.ToPoint = pNewPnt;

                    double pHitDistOne = -1;
                    double pHitDistTwo = -1;
                    IPoint snapPnt = null;

                    pHitDistOne = Globals.PointDistanceOnLine(pStraightLinePoint, lineFeature.ShapeCopy as IPolyline, 2, out snapPnt);
                    pHitDistTwo = Globals.PointDistanceOnLine(pNewPnt, lineFeature.ShapeCopy as IPolyline, 2, out snapPnt);
                    snapPnt = null;

                    if (lineFlow == esriFlowDirection.esriFDUninitialized || lineFlow == esriFlowDirection.esriFDIndeterminate)
                    {

                    }
                    else
                    {
                        if (pHitDistOne > pHitDistTwo && lineFlow == esriFlowDirection.esriFDAgainstFlow)
                        {
                            angleLine.FromPoint = turnPoint;
                            angleLine.ToPoint = pNewPnt;

                        }
                        else if (pHitDistOne < pHitDistTwo && lineFlow == esriFlowDirection.esriFDAgainstFlow)
                        {
                            if (angle > 0)
                            {
                                //rads = (degs + angle) * (Math.PI / 180);
                                angle = 0 - Math.Abs(angle);
                            }
                            else if (angle < 0)
                            {
                                angle = Math.Abs(angle);
                            }

                            rads = (degs + angle) * (Math.PI / 180);
                            if (degs < 0)
                                degs = 360 + degs;

                            pConsPoint.ConstructAngleDistance(pStraightLinePoint, rads, angleLine.Length * 2);

                        }
                        else if (pHitDistOne > pHitDistTwo && lineFlow == esriFlowDirection.esriFDWithFlow)
                        {
                            if (angle > 0)
                            {
                                //rads = (degs + angle) * (Math.PI / 180);
                                angle = 0 - Math.Abs(angle);
                            }
                            else if (angle < 0)
                            {
                                angle = Math.Abs(angle);
                            }

                            rads = (degs + angle) * (Math.PI / 180);
                            if (degs < 0)
                                degs = 360 + degs;

                            pConsPoint.ConstructAngleDistance(pStraightLinePoint, rads, angleLine.Length * 2);


                            //angleLine.FromPoint = turnPoint;
                            //angleLine.ToPoint = pNewPnt;
                        }
                        else if (pHitDistOne < pHitDistTwo && lineFlow == esriFlowDirection.esriFDWithFlow)
                        {
                            //angleLine.FromPoint = turnPoint;
                            //angleLine.ToPoint = pNewPnt;

                        }
                        else
                        {
                            //angleLine.FromPoint = turnPoint;
                            //angleLine.ToPoint = pNewPnt;

                        }
                    }
                    newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);

                    newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
                    //// return newPolyLine;

                    newPolyLine.SpatialReference = ((IGeoDataset)pMainLayer).SpatialReference;


                    if (lineFeature != null)
                    {

                        //pTopoOptr = (ITopologicalOperator)newPolyLine;

                        //pGeomColl = (IGeometryCollection)pTopoOptr.Intersect(lineFeature.Shape, esriGeometryDimension.esriGeometry0Dimension);
                        pIntPnt = Globals.GetIntersection(lineFeature.Shape, newPolyLine) as IPoint;



                        if (pIntPnt != null)
                        {

                            newPolyLine = new PolylineClass();
                            newPointColl = (IPointCollection)newPolyLine;
                            newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                            if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                            {

                            }
                            else
                            {
                                newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                            }
                            newPointColl.AddPoint(pIntPnt, ref Missing, ref Missing);

                        }
                        else
                        {
                            pIntGeo = Globals.GetIntersectingGeometry(newPolyLine, pMainLayer, false, true, -1, mapSpat);

                            if (pIntGeo.Count > 0)
                            {

                                //pTopoOptr = (ITopologicalOperator)newPolyLine;

                                //pGeomColl = (IGeometryCollection)pTopoOptr.Intersect(pIntFeat[0].ShapeCopy, esriGeometryDimension.esriGeometry0Dimension);
                                pIntPnt = Globals.GetIntersection(pIntGeo[0], newPolyLine) as IPoint;

                                if (pIntPnt != null)
                                {

                                    newPolyLine = new PolylineClass();
                                    newPointColl = (IPointCollection)newPolyLine;
                                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                    if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                    {

                                    }
                                    else
                                    {
                                        newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                    }
                                    newPointColl.AddPoint(pIntPnt, ref Missing, ref Missing);

                                }
                                else
                                {

                                    newPolyLine = new PolylineClass();
                                    newPointColl = (IPointCollection)newPolyLine;
                                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                    if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                    {

                                    }
                                    else
                                    {
                                        newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                    }

                                    newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
                                }

                            }
                            else
                            {

                                pIntGeo = Globals.GetIntersectingGeometry(newPolyLine, pMainLayer, false, true, -1, mapSpat);

                                if (pIntGeo.Count > 0)
                                {

                                    //pTopoOptr = (ITopologicalOperator)newPolyLine;

                                    //pGeomColl = (IGeometryCollection)pTopoOptr.Intersect(pIntFeat[0].ShapeCopy, esriGeometryDimension.esriGeometry0Dimension);
                                    pIntPnt = Globals.GetIntersection(pIntGeo[0], newPolyLine) as IPoint;

                                    if (pIntPnt != null)
                                    {

                                        newPolyLine = new PolylineClass();
                                        newPointColl = (IPointCollection)newPolyLine;
                                        newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                        if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                        {

                                        }
                                        else
                                        {
                                            newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                        }
                                        newPointColl.AddPoint(pIntPnt, ref Missing, ref Missing);

                                    }
                                    else
                                    {

                                        newPolyLine = new PolylineClass();
                                        newPointColl = (IPointCollection)newPolyLine;
                                        newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                        if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                        {

                                        }
                                        else
                                        {
                                            newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                        }

                                        newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
                                    }


                                }
                                else
                                {

                                    newPolyLine = new PolylineClass();
                                    newPointColl = (IPointCollection)newPolyLine;
                                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                    if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                    {

                                    }
                                    else
                                    {
                                        newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                    }

                                    newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
                                }

                                //newPolyLine = new PolylineClass();
                                //newPointColl = (IPointCollection)newPolyLine;
                                //newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                                //if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                                //{

                                //}
                                //else
                                //{
                                //    newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                                //}

                                //newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
                            }
                        }
                    }
                    else
                    {
                        newPolyLine = new PolylineClass();
                        newPointColl = (IPointCollection)newPolyLine;

                        newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
                        if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
                        {

                        }
                        else
                        {
                            newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
                        }

                        newPointColl.AddPoint(toPoint, ref Missing, ref Missing);
                    }



                }

                if (newPointColl.PointCount < 2)
                    newPolyLine = new PolylineClass();

                return newPolyLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in CreatePolylineFromPointsNewTurn: " + ex.Message);
                return null;
            }
            finally
            {
                Missing = null;

                newPointColl = null;

                straightLine = null;
                angleLine = null;
                pNewPnt = null;
                pConsPoint = null;
                pCl = null;

                pStraightLinePoint = null;
                pIntGeo = null;
                pIntPnt = null;
                //pTopoOptr = null;
                //pGeomColl = null;
            }



        }

        //public static IPolyline CreatePolylineFromPointsNewTurn(IPoint fromPoint, IPoint turnPoint, IPoint toPoint, ref IFeatureLayer pMainLayer, ref IFeature lineFeature, bool SearchOnLayer, double angle)
        //{
        //    object Missing = null;
        //    IPolyline newPolyLine = null;
        //    IPointCollection newPointColl = null;

        //    ILine straightLine = null;
        //    ILine angleLine = null;
        //    IPoint pNewPnt = null;
        //    IConstructPoint2 pConsPoint = null;
        //    IClone pCl = null;

        //    IPoint pStraightLinePoint = null;
        //    ITopologicalOperator pTopoOptr = null;
        //    IGeometryCollection pGeomColl = null;
        //    try
        //    {
        //        Missing = Type.Missing;
        //        newPolyLine = new PolylineClass();
        //        newPointColl = (IPointCollection)newPolyLine;
        //        if (turnPoint == null)
        //        {
        //            newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);

        //            newPointColl.AddPoint(toPoint, ref Missing, ref Missing);

        //        }
        //        else
        //        {

        //            double dblTol = 1.0;

        //            lineFeature = Globals.GetClosestFeature(fromPoint as IGeometry, pMainLayer, dblTol, SearchOnLayer, false);
        //            ESRI.ArcGIS.Geodatabase.esriFlowDirection lineFlow = esriFlowDirection.esriFDIndeterminate;
        //            if (lineFeature == null)
        //            {
        //                lineFeature = Globals.GetClosestFeature(toPoint, pMainLayer, dblTol, SearchOnLayer, false);
        //                lineFlow = Globals.GetFlowDirectionAtLocation(lineFeature, pMainLayer, toPoint, dblTol);

        //            }
        //            else
        //            {
        //                lineFlow = Globals.GetFlowDirectionAtLocation(lineFeature, pMainLayer, fromPoint, dblTol);

        //            }

        //            straightLine = new LineClass();
        //            straightLine.FromPoint = fromPoint;
        //            straightLine.ToPoint = turnPoint;

        //            angleLine = new LineClass();
        //            angleLine.FromPoint = turnPoint;
        //            angleLine.ToPoint = toPoint;

        //            pNewPnt = new PointClass();
        //            pConsPoint = (IConstructPoint2)pNewPnt;
        //            double degs = angleLine.Angle * (180 / Math.PI);
        //            if (degs < 0)
        //                degs = 360 + degs;

        //            double rads = (degs + angle) * (Math.PI / 180);
        //            pCl = (IClone)turnPoint;

        //            pStraightLinePoint = (IPoint)pCl.Clone();

        //            pConsPoint.ConstructAngleDistance(turnPoint, rads, angleLine.Length * 2);


        //            angleLine.FromPoint = turnPoint;
        //            angleLine.ToPoint = pNewPnt;

        //            double pHitDistOne = -1;
        //            double pHitDistTwo = -1;
        //            IPoint snapPnt = null;

        //            pHitDistOne = Globals.PointDistanceOnLine(pStraightLinePoint, lineFeature.ShapeCopy as IPolyline, 2, out snapPnt);
        //            pHitDistTwo = Globals.PointDistanceOnLine(pNewPnt, lineFeature.ShapeCopy as IPolyline, 2, out snapPnt);
        //            snapPnt = null;

        //            if (lineFlow == esriFlowDirection.esriFDUninitialized || lineFlow == esriFlowDirection.esriFDIndeterminate)
        //            {

        //            }
        //            else
        //            {
        //                if (pHitDistOne > pHitDistTwo && lineFlow == esriFlowDirection.esriFDAgainstFlow)
        //                {
        //                    angleLine.FromPoint = turnPoint;
        //                    angleLine.ToPoint = pNewPnt;

        //                }
        //                else if (pHitDistOne < pHitDistTwo && lineFlow == esriFlowDirection.esriFDAgainstFlow)
        //                {
        //                    if (angle > 0)
        //                    {
        //                        //rads = (degs + angle) * (Math.PI / 180);
        //                        angle = 0 - Math.Abs(angle);
        //                    }
        //                    else if (angle < 0)
        //                    {
        //                        angle = Math.Abs(angle);
        //                    }

        //                    rads = (degs + angle) * (Math.PI / 180);
        //                    if (degs < 0)
        //                        degs = 360 + degs;

        //                    pConsPoint.ConstructAngleDistance(pStraightLinePoint, rads, angleLine.Length * 2);

        //                }
        //                else if (pHitDistOne > pHitDistTwo && lineFlow == esriFlowDirection.esriFDWithFlow)
        //                {
        //                    if (angle > 0)
        //                    {
        //                        //rads = (degs + angle) * (Math.PI / 180);
        //                        angle = 0 - Math.Abs(angle);
        //                    }
        //                    else if (angle < 0)
        //                    {
        //                        angle = Math.Abs(angle);
        //                    }

        //                    rads = (degs + angle) * (Math.PI / 180);
        //                    if (degs < 0)
        //                        degs = 360 + degs;

        //                    pConsPoint.ConstructAngleDistance(pStraightLinePoint, rads, angleLine.Length * 2);


        //                    //angleLine.FromPoint = turnPoint;
        //                    //angleLine.ToPoint = pNewPnt;
        //                }
        //                else if (pHitDistOne < pHitDistTwo && lineFlow == esriFlowDirection.esriFDWithFlow)
        //                {
        //                    //angleLine.FromPoint = turnPoint;
        //                    //angleLine.ToPoint = pNewPnt;

        //                }
        //                else
        //                {
        //                    //angleLine.FromPoint = turnPoint;
        //                    //angleLine.ToPoint = pNewPnt;

        //                }
        //            }
        //            newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);

        //            newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
        //            //// return newPolyLine;

        //            newPolyLine.SpatialReference = ((IGeoDataset)pMainLayer).SpatialReference;


        //            if (lineFeature != null)
        //            {

        //                pTopoOptr = (ITopologicalOperator)newPolyLine;

        //                pGeomColl = (IGeometryCollection)pTopoOptr.Intersect(lineFeature.Shape, esriGeometryDimension.esriGeometry0Dimension);


        //                newPolyLine = new PolylineClass();
        //                newPointColl = (IPointCollection)newPolyLine;

        //                if (pGeomColl.GeometryCount > 0)
        //                {

        //                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
        //                    if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
        //                    }
        //                    newPointColl.AddPoint((IPoint)pGeomColl.Geometry[0], ref Missing, ref Missing);

        //                }
        //                else
        //                {
        //                    newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
        //                    if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
        //                    }

        //                    newPointColl.AddPoint(pNewPnt, ref Missing, ref Missing);
        //                }
        //            }
        //            else
        //            {
        //                newPolyLine = new PolylineClass();
        //                newPointColl = (IPointCollection)newPolyLine;

        //                newPointColl.AddPoint(fromPoint, ref Missing, ref Missing);
        //                if (fromPoint.X == turnPoint.X && fromPoint.Y == turnPoint.Y)
        //                {

        //                }
        //                else
        //                {
        //                    newPointColl.AddPoint(turnPoint, ref Missing, ref Missing);
        //                }

        //                newPointColl.AddPoint(toPoint, ref Missing, ref Missing);
        //            }



        //        }

        //        if (newPointColl.PointCount < 2)
        //            newPolyLine = new PolylineClass();

        //        return newPolyLine;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in CreatePolylineFromPointsNewTurn: " + ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        Missing = null;

        //        newPointColl = null;

        //        straightLine = null;
        //        angleLine = null;
        //        pNewPnt = null;
        //        pConsPoint = null;
        //        pCl = null;

        //        pStraightLinePoint = null;
        //        pTopoOptr = null;
        //        pGeomColl = null;
        //    }



        //}

        #endregion

        #region GraphicTools
        public static IGeometry GetShapeFromGraphic(string Tag, string searchPhrase, IApplication app)
        {
            IElement pElem = default(IElement);
            IElementProperties pElProp = default(IElementProperties);
            try
            {
                ((IMxDocument)app.Document).ActiveView.GraphicsContainer.Reset();
                pElem = ((IMxDocument)app.Document).ActiveView.GraphicsContainer.Next();
                while (!(pElem == null))
                {
                    pElProp = (IElementProperties)pElem;
                    if (pElProp.CustomProperty != null)
                    {

                        if (pElProp.CustomProperty.ToString().Contains(searchPhrase))
                        {
                            string strEl = pElProp.CustomProperty.ToString();

                            if (strEl == Tag)
                            {
                                return pElem.Geometry;
                            }
                        }
                    }
                    pElem = ((IMxDocument)app.Document).ActiveView.GraphicsContainer.Next();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetShapeFromGraphic" + Environment.NewLine + ex.Message);

                return null;
            }
            finally
            {
                pElem = null;
                pElProp = null;
            }


        }
        public static void AddPointGraphic(IMap map, IPoint snappedPoint, bool Display)
        {
            IActiveView av = null;
            IGraphicsContainer gc = null;
            IElement element = null;
            IMarkerElement markerelem = null;
            ISimpleMarkerSymbol pSymbolJunctionFlag = null;
            IElementProperties3 elemProperties = null;
            try
            {
                av = map as IActiveView;

                pSymbolJunctionFlag = CreateNetworkFlagBarrierSymbol(flagType.JunctionFlag);

                //Add start graphic
                gc = map as IGraphicsContainer;
                element = null;
                markerelem = new MarkerElementClass();
                pSymbolJunctionFlag = CreateNetworkFlagBarrierSymbol(flagType.JunctionFlag);
                if (Display)
                    pSymbolJunctionFlag.Size = 10; //TODO: UserConfig
                else
                    pSymbolJunctionFlag.Size = .5; //TODO: UserConfig

                markerelem.Symbol = pSymbolJunctionFlag;


                element = (IElement)markerelem;
                element.Geometry = snappedPoint;
                elemProperties = element as IElementProperties3;

                elemProperties.Name = "TraceFlag";
                elemProperties.ReferenceScale = map.ReferenceScale;

                gc.AddElement(element, 0);

            }
            catch
            { }
            finally
            {

                av = null;
                gc = null;
                element = null;
                markerelem = null;
                pSymbolJunctionFlag = null;
                elemProperties = null;
            }

        }

        #endregion

        #region SpatRefTools_UnitTools
        public static ISpatialReference GetLayersCoordinateSystem(IFeatureClass inFeatClass)
        {
            if (!(inFeatClass is IGeoDataset))
                return null;

            IGeoDataset iGDs = (IGeoDataset)inFeatClass;
            return iGDs.SpatialReference;


        }
        public static ISpatialReference CreateSpatRef(int WKID)
        {

            ISpatialReferenceFactory3 spatRefFact = new SpatialReferenceEnvironmentClass();
            return spatRefFact.CreateSpatialReference(WKID);


            //IGeographicCoordinateSystem srGeo = spatRefFact.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            //IProjectedCoordinateSystem srPrj;
            //    return srGeo;

        }
        public static string GetSpatRefUnitName(ISpatialReference inSpatRef, bool clean)
        {
            //ISpatialReference3 iSpat3 = null;
            // ISpatialReferenceInfo pSpatialReferenceInfo;

            if (inSpatRef is IProjectedCoordinateSystem)
            {
                ILinearUnit pLinearUnit;
                IProjectedCoordinateSystem iProj;
                iProj = inSpatRef as IProjectedCoordinateSystem;

                pLinearUnit = iProj.CoordinateUnit as ILinearUnit;
                IUnit pUnit;

                pUnit = pLinearUnit as IUnit;
                if (clean)
                {
                    return pLinearUnit.Name.ToString().Split('_')[0];

                }
                else
                    return pLinearUnit.Name.ToString();
                // pSpatialReferenceInfo = pLinearUnit    as ISpatialReferenceInfo;

            }
            else if (inSpatRef is IGeographicCoordinateSystem)
            {
                IAngularUnit pAngUnit;
                IGeographicCoordinateSystem iGeo;
                iGeo = inSpatRef as IGeographicCoordinateSystem;

                pAngUnit = iGeo.CoordinateUnit as IAngularUnit;
                IUnit pUnit;

                pUnit = pAngUnit as IUnit;
                if (clean)
                {
                    return pAngUnit.Name.ToString().Split('_')[0];

                }
                else
                    return pAngUnit.Name.ToString();
                //pSpatialReferenceInfo = pAngUnit as ISpatialReferenceInfo;

            }
            return null;

        }
        public static double ConvertFeetToMapUnits(double unitsFeet, IApplication app)
        {
            IUnitConverter pUnitConverter = default(IUnitConverter);
            IProjectedCoordinateSystem pPrjCoord = default(IProjectedCoordinateSystem);
            IGeographicCoordinateSystem pGeoCoord = default(IGeographicCoordinateSystem);

            try
            {

                pUnitConverter = new UnitConverter();
                if (((IMxDocument)app.Document).FocusMap.SpatialReference is IProjectedCoordinateSystem)
                {
                    pPrjCoord = (IProjectedCoordinateSystem)((IMxDocument)app.Document).FocusMap.SpatialReference;
                    return pUnitConverter.ConvertUnits(unitsFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, ConvertUnitType(pPrjCoord.CoordinateUnit));
                }
                else
                {
                    pGeoCoord = (IGeographicCoordinateSystem)((IMxDocument)app.Document).FocusMap.SpatialReference;
                    return pUnitConverter.ConvertUnits(unitsFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, ConvertUnitType(pGeoCoord.CoordinateUnit));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - ConvertFeetToMapUnits" + Environment.NewLine + ex.Message);
                return -1.0;
            }
            finally
            {
                pUnitConverter = null;
                pGeoCoord = null;
                pPrjCoord = null;

            }
        }
        public static double ConvertSpatRefToFeet(double unitsSpatRef, ISpatialReference spatialRef, IApplication app)
        {
            IUnitConverter pUnitConverter = default(IUnitConverter);
            IProjectedCoordinateSystem pPrjCoord = default(IProjectedCoordinateSystem);
            IGeographicCoordinateSystem pGeoCoord = default(IGeographicCoordinateSystem);

            try
            {

                pUnitConverter = new UnitConverter();
                if (spatialRef is IProjectedCoordinateSystem)
                {
                    pPrjCoord = (IProjectedCoordinateSystem)spatialRef;
                    return pUnitConverter.ConvertUnits(unitsSpatRef, ConvertUnitType(pPrjCoord.CoordinateUnit), ESRI.ArcGIS.esriSystem.esriUnits.esriFeet);
                }
                else
                {
                    pGeoCoord = (IGeographicCoordinateSystem)spatialRef;
                    return pUnitConverter.ConvertUnits(unitsSpatRef, ConvertUnitType(pPrjCoord.CoordinateUnit), ESRI.ArcGIS.esriSystem.esriUnits.esriFeet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - ConvertSpatRefToFeet" + Environment.NewLine + ex.Message);
                return -1.0;
            }
            finally
            {
                pUnitConverter = null;
                pGeoCoord = null;
                pPrjCoord = null;

            }
        }
        public static ESRI.ArcGIS.esriSystem.esriUnits ConvertUnitType2(ESRI.ArcGIS.Geometry.ILinearUnit linearUnit)
        {

            try
            {
                switch ((linearUnit.FactoryCode))
                {
                    case 109006:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters;
                    case 9102:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees;
                    case 109005:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimeters;
                    case 9003:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet;
                    case 109008:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriInches;
                    case 9036:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers;
                    case 9001:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMeters;
                    case 9035:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMiles;
                    case 109007:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMillimeters;
                    case 9030:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriNauticalMiles;
                    case 109002:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriYards;
                }
                return esriUnits.esriUnknownUnits;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - ConvertUnitType2" + Environment.NewLine + ex.Message);
                return esriUnits.esriUnknownUnits;
            }
            finally
            {
            }

        }
        public static ESRI.ArcGIS.esriSystem.esriUnits ConvertUnitType(ESRI.ArcGIS.Geometry.IUnit Unit)
        {
            try
            {
                switch ((Unit.FactoryCode))
                {
                    case 109006:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters;
                    case 9102:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees;
                    case 109005:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriDecimeters;
                    case 9003:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet;
                    case 109008:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriInches;
                    case 9036:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers;
                    case 9001:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMeters;
                    case 9035:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMiles;
                    case 109007:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriMillimeters;
                    case 9030:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriNauticalMiles;
                    case 109002:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriYards;
                    default:
                        return ESRI.ArcGIS.esriSystem.esriUnits.esriFeet;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - ConvertUnitType" + Environment.NewLine + ex.Message);
                return esriUnits.esriUnknownUnits;
            }
            finally
            {
            }
        }
        public static double ConvertUnits(double distanceInFeet, ESRI.ArcGIS.esriSystem.esriUnits mapUnits)
        {
            ESRI.ArcGIS.esriSystem.IUnitConverter iuc = default(ESRI.ArcGIS.esriSystem.IUnitConverter);

            try
            {
                iuc = new ESRI.ArcGIS.esriSystem.UnitConverterClass();
                double convertedValue = 0;
                convertedValue = iuc.ConvertUnits(distanceInFeet, ESRI.ArcGIS.esriSystem.esriUnits.esriFeet, mapUnits);
                iuc = null;
                return convertedValue;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - ConvertUnits" + Environment.NewLine + ex.Message);
                return 0.0;
            }
            finally
            {
                iuc = null;


            }
        }

        #endregion

        #region DomainSubtypeTools

        public static List<DomSubList> DomainToList(IDomain Dom)
        {
            ICodedValueDomain CodedValue = default(ICodedValueDomain);
            List<DomSubList> pArrList = null;
            DomSubList pDSL = null;
            try
            {
                CodedValue = null;

                if (Dom is ICodedValueDomain)
                {
                    CodedValue = (ICodedValueDomain)Dom;
                }
                else
                {
                    return null;
                }

                pArrList = new List<DomSubList>();
                for (int i = 0; i <= CodedValue.CodeCount - 1; i++)
                {
                    pDSL = new DomSubList(CodedValue.get_Value(i).ToString(), CodedValue.get_Name(i).ToString());
                    //pDSL.Value = CodedValue.Value(i)
                    //pDSL.Display = CodedValue.Name(i)
                    pArrList.Add(pDSL);


                }

                return pArrList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - DomainToList" + Environment.NewLine + ex.Message);
                return null;
            }
            finally
            {
                CodedValue = null;
                pDSL = null;

            }
        }
        public static List<DomSubList> SubtypeToList(ISubtypes pSubTypes)
        {
            IEnumSubtype pEnumSubTypes = default(IEnumSubtype);
            List<DomSubList> pArrList = null;
            DomSubList pDSL = null;
            try
            {
                //  get  the enumeration of all of the subtypes for this feature class

                int lSubT = 0;
                string sSubT = null;
                pEnumSubTypes = pSubTypes.Subtypes;

                // loop through all of the subtypes and bring up a message
                // box with each subtype's code and name
                // Indicate when sFeature is found (a passed in string var)
                sSubT = pEnumSubTypes.Next(out lSubT);
                pArrList = new List<DomSubList>();

                while (sSubT != null)
                {
                    pDSL = new DomSubList(lSubT.ToString(), pSubTypes.get_SubtypeName(lSubT).ToString());
                    //pDSL.Value = lSubT
                    //pDSL.Display = pSubTypes.SubtypeName(lSubT)
                    pArrList.Add(pDSL);

                    sSubT = pEnumSubTypes.Next(out lSubT);

                }
                pEnumSubTypes = null;

                return pArrList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - SubtypeToList" + Environment.NewLine + ex.Message);
                return null;
            }
            finally
            {
                pEnumSubTypes = null;
                pDSL = null;

            }
        }
        public static string GetDomainDisplay(object Value, ICodedValueDomain CodedValue)
        {
            try
            {
                if (CodedValue == null)
                    return "";
                //if (object.ReferenceEquals(Value, DBNull.Value))
                //{
                //    return CodedValue.get_Name(0);
                //}
                for (int i = 0; i <= CodedValue.CodeCount - 1; i++)
                {
                    if (CodedValue.get_Value(i).ToString() == Value.ToString())
                        return CodedValue.get_Name(i);


                }
                return Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetDomainDisplay" + Environment.NewLine + ex.Message);
                return null;

            }
            finally
            {


            }
        }
        public static string GetDomainDisplay(object Value, IObject Feature, IField Field)
        {
            try
            {

                ISubtypes pSub = Feature.Class as ISubtypes;
                IDomain pDom;
                ICodedValueDomain pCVDom;
                if (pSub.HasSubtype)
                {
                    if (pSub.SubtypeFieldName.ToUpper() == Field.Name.ToUpper())
                    {
                        return GetSubtypeDisplay(pSub, Value.ToString());
                    }
                    else
                    {
                        pDom = pSub.get_Domain(Convert.ToInt32(Feature.get_Value(pSub.SubtypeFieldIndex)), Field.Name);
                        if (pDom == null)
                        {
                            return Value.ToString();
                        }
                        else if (!(pDom is ICodedValueDomain))
                        {
                            return Value.ToString();
                        }
                        else
                        {
                            pCVDom = (ICodedValueDomain)pDom;
                            return GetDomainDisplay(Value, pCVDom);

                        }
                    }
                }
                else
                {
                    pDom = Field.Domain;
                    if (pDom == null)
                    {
                        return Value.ToString();
                    }
                    else if (!(pDom is ICodedValueDomain))
                    {
                        return Value.ToString();
                    }
                    else
                    {
                        pCVDom = (ICodedValueDomain)pDom;
                        return GetDomainDisplay(Value, pCVDom);

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetDomainDisplay" + Environment.NewLine + ex.Message);
                return null;

            }
            finally
            {


            }
        }
        public static string GetDomainValue(object Display, ICodedValueDomain CodedValue)
        {
            try
            {
                if (CodedValue == null)
                    return "";
                for (int i = 0; i <= CodedValue.CodeCount - 1; i++)
                {
                    if (CodedValue.get_Name(i).ToString() == Display.ToString())
                        return (string)CodedValue.get_Value(i);


                }
                return (string)Display;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetDomainValue" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static int GetSubtypeValue(object Display, ISubtypes pSubtypes)
        {
            IEnumSubtype pEnumSubTypes = default(IEnumSubtype);
            try
            {
                //  get  the enumeration of all of the subtypes for this feature class

                int lSubT = 0;
                string sSubT = null;
                pEnumSubTypes = pSubtypes.Subtypes;

                // loop through all of the subtypes and bring up a message
                // box with each subtype's code and name
                // Indicate when sFeature is found (a passed in string var)
                sSubT = pEnumSubTypes.Next(out lSubT);
                ArrayList pArrList = new ArrayList();
                if (sSubT == null) return -99999;
                while (sSubT.Length > 0)
                {
                    if (Display.ToString() == pSubtypes.get_SubtypeName(lSubT).ToString())
                    {
                        pEnumSubTypes = null;
                        return lSubT;
                    }


                    sSubT = pEnumSubTypes.Next(out lSubT);
                    if (sSubT == null) break;
                }
                pEnumSubTypes = null;

                return -99999;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetSubtypeValue" + Environment.NewLine + ex.Message);
                return -99999;
            }
            finally
            {
                pEnumSubTypes = null;

            }
        }
        public static int SubtypeCount(ISubtypes pSubTypes)
        {
            IEnumSubtype pEnumSubTypes = default(IEnumSubtype);
            try
            {
                //  get  the enumeration of all of the subtypes for this feature class

                int lSubT = 0;
                string sSubT = null;
                pEnumSubTypes = pSubTypes.Subtypes;

                // loop through all of the subtypes and bring up a message
                // box with each subtype's code and name
                // Indicate when sFeature is found (a passed in string var)
                sSubT = pEnumSubTypes.Next(out lSubT);
                int i = 0;
                if (sSubT == null) return 0;
                while (sSubT.Length > 0)
                {
                    i = i + 1;

                    sSubT = pEnumSubTypes.Next(out lSubT);
                    if (sSubT == null) break;
                }
                pEnumSubTypes = null;

                return i;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - SubtypeCount" + Environment.NewLine + ex.Message);

                return -99999;
            }
            finally
            {
                pEnumSubTypes = null;

            }
        }
        public static string GetSubtypeDisplay(ISubtypes pSubTypes, string Code)
        {
            IEnumSubtype pEnumSubTypes = default(IEnumSubtype);

            try
            {
                //  get  the enumeration of all of the subtypes for this feature class

                int lSubT = 0;
                string sSubT = null;
                pEnumSubTypes = pSubTypes.Subtypes;

                // loop through all of the subtypes and bring up a message
                // box with each subtype's code and name
                // Indicate when sFeature is found (a passed in string var)
                sSubT = pEnumSubTypes.Next(out lSubT);
                int i = 0;
                if (sSubT == null) return "";
                while (sSubT.Length > 0)
                {

                    if (Code == lSubT.ToString())
                    {

                        return pSubTypes.get_SubtypeName(lSubT);
                    }


                    sSubT = pEnumSubTypes.Next(out lSubT);
                    i = i + 1;
                    if (sSubT == null) break;
                }
                pEnumSubTypes = null;
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - GetSubtypeDisplay" + Environment.NewLine + ex.Message);
                return "";
            }
            finally
            {
                pEnumSubTypes = null;

            }

        }
        public static void SubtypeValuesAtIndex(int index, ISubtypes pSubTypes, ref string Code, ref string Display)
        {
            IEnumSubtype pEnumSubTypes = default(IEnumSubtype);

            try
            {
                //  get  the enumeration of all of the subtypes for this feature class

                int lSubT = 0;
                string sSubT = null;
                pEnumSubTypes = pSubTypes.Subtypes;

                // loop through all of the subtypes and bring up a message
                // box with each subtype's code and name
                // Indicate when sFeature is found (a passed in string var)
                sSubT = pEnumSubTypes.Next(out lSubT);
                int i = 0;
                if (sSubT == null) return;
                while (sSubT.Length > 0)
                {
                    if (i == index)
                    {
                        Code = lSubT.ToString();
                        Display = pSubTypes.get_SubtypeName(lSubT);
                        // sSubT
                        return;
                    }


                    sSubT = pEnumSubTypes.Next(out lSubT);
                    i = i + 1;
                    if (sSubT == null) break;
                }
                pEnumSubTypes = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - SubtypeValuesAtIndex" + Environment.NewLine + ex.Message);

            }
            finally
            {
                pEnumSubTypes = null;

            }

        }
        public static void DomainValuesAtIndex(int index, ICodedValueDomain CodedValue, ref string Code, ref string Display)
        {

            try
            {
                if (CodedValue == null)
                    return;
                for (int i = 0; i <= CodedValue.CodeCount - 1; i++)
                {
                    if (i == index)
                    {
                        Code = Convert.ToString(CodedValue.get_Value(i));
                        Display = Convert.ToString(CodedValue.get_Name(i));
                        return;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Global Functions - DomainValuesAtIndex" + Environment.NewLine + ex.Message);
            }
            finally
            {

            }

        }

        #endregion

        #region LocatorTools
        public static IReverseGeocoding OpenLocator(string path, string LocatorName)
        {
            try
            {
                ILocatorWorkspace2 pLocWork = null;
                if (path.Contains(";"))
                {
                    if (path.ToUpper().Contains("SERVER="))
                    {
                        pLocWork = OpenArcSDEDatabaseLocatorWorkspace(path, "t");
                    }
                    else
                    {
                        string[] strSDEStuff = path.Split(';');
                        if (strSDEStuff.Length == 5)
                        {
                            pLocWork = OpenArcSDEDatabaseLocatorWorkspace(strSDEStuff[0], strSDEStuff[1], strSDEStuff[2], strSDEStuff[3], strSDEStuff[4]);
                        }
                        else if (strSDEStuff.Length == 6)
                        {
                            pLocWork = OpenArcSDEDatabaseLocatorWorkspace(strSDEStuff[0], strSDEStuff[1], strSDEStuff[2], strSDEStuff[3], strSDEStuff[4], strSDEStuff[5]);
                        }
                    }

                }
                else if (path.Contains(".sde"))
                {
                    pLocWork = OpenArcSDEDatabaseLocatorWorkspace(path);
                }
                else if (path.Contains(".gdb"))
                {
                    pLocWork = OpenFileGDBDatabaseLocatorWorkspace(path);
                }
                else if (path.Contains(".mdb"))
                {
                    pLocWork = OpenPersonalGDBDatabaseLocatorWorkspace(path);
                }
                else
                {
                    pLocWork = OpenLocatorWorkspace(path);
                }
                if (pLocWork == null) return null;
                if (LocatorName.Contains(".loc"))
                {
                    LocatorName = LocatorName.Replace(".loc", "");
                }

                IReverseGeocoding reverseGeocoding = (IReverseGeocoding)pLocWork.GetLocator(LocatorName);

                return reverseGeocoding;
            }
            catch
            {
                return null;
            }
        }
        public static ILocatorWorkspace2 OpenLocatorWorkspace(string path)
        {
            try
            {
                System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                    "esriLocation.LocatorManager"));
                ILocatorManager locatorManager = obj as ILocatorManager2;
                ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspaceFromPath(
                    path);
                ILocalLocatorWorkspace localLocatorWorkspace = (ILocalLocatorWorkspace)
                    locatorWorkspace;
                return localLocatorWorkspace as ILocatorWorkspace2;
            }
            catch
            {
                return null;
            }

        }
        public static ILocatorWorkspace2 OpenFileGDBDatabaseLocatorWorkspace(string path)
        {
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager locatorManager = obj as ILocatorManager2;

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.FileGDBWorkspaceFactory"));
            IWorkspaceFactory workspaceFactory = obj as IWorkspaceFactory;
            IWorkspace workspace = workspaceFactory.OpenFromFile(
                path, 0);
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;

            return databaseLocatorWorkspace as ILocatorWorkspace2;



        }
        public static ILocatorWorkspace2 OpenPersonalGDBDatabaseLocatorWorkspace(string path)
        {
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.AccessWorkspaceFactory"));
            IWorkspaceFactory workspaceFactory = obj as IWorkspaceFactory;
            IWorkspace workspace = workspaceFactory.OpenFromFile(
                path, 0);

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager locatorManager = obj as ILocatorManager;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;

            return databaseLocatorWorkspace as ILocatorWorkspace2;


        }
        public static ILocatorWorkspace2 OpenArcSDEDatabaseLocatorWorkspace(string server, string instance, string database, string AuthMode, string version)
        {

            // Open an ArcSDE workspace.
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("server", server);
            propertySet.SetProperty("instance", instance);
            propertySet.SetProperty("database", database);
            //  propertySet.SetProperty("user", user);
            propertySet.SetProperty("AUTHENTICATION_MODE", AuthMode);
            propertySet.SetProperty("version", version);

            // Get the workspace.
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.SdeWorkspaceFactory"));
            IWorkspaceFactory2 workspaceFactory = obj as IWorkspaceFactory2;
            IWorkspace workspace = workspaceFactory.Open(propertySet, 0);

            // Open the database locator workspace.

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager2 locatorManager = obj as ILocatorManager2;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;


            return databaseLocatorWorkspace as ILocatorWorkspace2;



        }
        public static ILocatorWorkspace2 OpenArcSDEDatabaseLocatorWorkspace(string connectionString, string other)
        {


            // Get the workspace.
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.SdeWorkspaceFactory"));
            IWorkspaceFactory2 workspaceFactory = obj as IWorkspaceFactory2;
            IWorkspace workspace = workspaceFactory.OpenFromString(connectionString, 0);

            // Open the database locator workspace.

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager2 locatorManager = obj as ILocatorManager2;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;


            return databaseLocatorWorkspace as ILocatorWorkspace2;



        }
        public static ILocatorWorkspace2 OpenArcSDEDatabaseLocatorWorkspace(string path)
        {

            // Open an ArcSDE workspace.
            IPropertySet propertySet = new PropertySetClass();
            //propertySet.SetProperty("server", server);
            //propertySet.SetProperty("instance", instance);
            //propertySet.SetProperty("database", database);
            //propertySet.SetProperty("user", user);
            //propertySet.SetProperty("password", password);
            //propertySet.SetProperty("version", version);

            // Get the workspace.
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.SdeWorkspaceFactory"));
            IWorkspaceFactory2 workspaceFactory = obj as IWorkspaceFactory2;
            IWorkspace workspace = workspaceFactory.OpenFromFile(path, 0);

            // Open the database locator workspace.

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager2 locatorManager = obj as ILocatorManager2;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;


            return databaseLocatorWorkspace as ILocatorWorkspace2;



        }

        public static ILocatorWorkspace2 OpenArcSDEDatabaseLocatorWorkspace(string server, string instance, string database, string user, string password, string version)
        {

            // Open an ArcSDE workspace.
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("server", server);
            propertySet.SetProperty("instance", instance);
            propertySet.SetProperty("database", database);
            propertySet.SetProperty("user", user);
            propertySet.SetProperty("password", password);
            propertySet.SetProperty("version", version);

            // Get the workspace.
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriDataSourcesGDB.SdeWorkspaceFactory"));
            IWorkspaceFactory2 workspaceFactory = obj as IWorkspaceFactory2;
            IWorkspace workspace = workspaceFactory.Open(propertySet, 0);

            // Open the database locator workspace.

            obj = Activator.CreateInstance(Type.GetTypeFromProgID(
                "esriLocation.LocatorManager"));
            ILocatorManager2 locatorManager = obj as ILocatorManager2;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetLocatorWorkspace
                (workspace);
            IDatabaseLocatorWorkspace databaseLocatorWorkspace = (IDatabaseLocatorWorkspace)
                locatorWorkspace;


            return databaseLocatorWorkspace as ILocatorWorkspace2;



        }
        #endregion

        #region LayerTools
        public static IEnumLayer GetLayers(IApplication app, string LayerType)
        {
            return GetLayers(((IMxDocument)app.Document).FocusMap, LayerType);

        }
        public static IEnumLayer GetLayers(IMap pMap, string LayerType)
        {
            ESRI.ArcGIS.esriSystem.UID uid = null;
            try
            {
                uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                switch (LayerType)
                {
                    case "RASTER":
                        uid.Value = "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}";
                        return pMap.get_Layers(uid, true);
                        break;
                    case "VECTOR":
                        uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                        return pMap.get_Layers(uid, true);
                        break;
                    case "BOTH":
                        uid.Value = "{34C20002-4D3C-11D0-92D8-00805F7C28B0}";
                        return pMap.get_Layers(uid, true);
                        break;
                    default:
                        //IEnumLayer pEn = new IEnumLayer();
                        return null;
                        //return FindLayer(pMap,LayerType);
                        break;
                }
                return null;
            }

            catch
            {
                return null;
            }
            finally
            {
                uid = null;
            }




        }
        public static ILayer FindLayer(IApplication app, string sLName, ref bool FoundAsFeatureLayer)
        {
            return FindLayer(((IMxDocument)app.Document).FocusMap, sLName, ref  FoundAsFeatureLayer);

        }
        public static IGroupLayer FindGroupLayer(IMap pMap, string sLName)
        {
            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;

            IEnumLayer pEnumLayer = default(IEnumLayer);

            try
            {

                ILayer pLay = default(ILayer);
                UID pUID = new UIDClass();
                pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

                pEnumLayer = pMap.get_Layers(pUID, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (pLay is IGroupLayer)
                    {
                        if (pLay.Name.ToUpper().Contains(sLName.ToUpper()))
                        {
                            return pLay as IGroupLayer;
                        }
                    }
                    pLay = pEnumLayer.Next();

                }

                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {

                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static List<IGroupLayer> FindGroupLayers(IMap pMap, string sLName)
        {
            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;

            IEnumLayer pEnumLayer = default(IEnumLayer);
            List<IGroupLayer> groupLayers = new List<IGroupLayer>();
            try
            {

                ILayer pLay = default(ILayer);
                UID pUID = new UIDClass();
                pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

                pEnumLayer = pMap.get_Layers(pUID, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (pLay is IGroupLayer)
                    {
                        if (pLay.Name.ToUpper().Contains(sLName.ToUpper()))
                        {
                            groupLayers.Add(pLay as IGroupLayer);
                        }
                    }
                    pLay = pEnumLayer.Next();

                }

                return groupLayers;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {

                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static ILayer FindLayerFromMapDataset(IMap pMap, string sLName, ref bool FoundAsFeatureLayer, IDataset dataset)
        {
            FoundAsFeatureLayer = false;
            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            IDataset pDataset = null;
            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                ILayer pLay = default(ILayer);
                pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay is IDataset)
                        {
                            pDataset = (IDataset)pLay;


                            if (pLay.Name.ToUpper() == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                            {
                                FoundAsFeatureLayer = true;
                                if (pLay is IBasemapSubLayer)
                                {
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    return pLay;
                                }
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();

                }
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay is IDataset)
                        {
                            pDataset = (IDataset)pLay;

                            if (pDataset.BrowseName.ToUpper() == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                            {
                                //functionReturnValue = pLay;

                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }
                            }
                            if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }


                            }
                            if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }

                            }
                            if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }

                            }
                        }
                        else if (pLay is IBasemapSubLayer)
                        {
                            if (((IBasemapSubLayer)pLay).Layer is IDataset)
                            {
                                pDataset = (IDataset)((IBasemapSubLayer)pLay).Layer;

                                if (pDataset.BrowseName.ToUpper() == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                                {
                                    //functionReturnValue = pLay;

                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }
                                }
                                if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }


                                }
                                if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }

                                }
                                if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper() && dataset.Workspace == pDataset.Workspace)
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }

                                }

                                //{
                                //    return ((IBasemapSubLayer)pLay).Layer;
                                //}
                                //else
                                //{
                                //return pLay;
                                //}
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                pDataset = null;
                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }

        public static ILayer FindLayer(IMap pMap, string sLName, ref bool FoundAsFeatureLayer)
        {
            FoundAsFeatureLayer = false;
            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            IDataset pDataset = null;
            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                ILayer pLay = default(ILayer);
                pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay.Name.ToUpper() == sLName.ToUpper())
                        {
                            FoundAsFeatureLayer = true;
                            if (pLay is IBasemapSubLayer)
                            {
                                return ((IBasemapSubLayer)pLay).Layer;
                            }
                            else
                            {
                                return pLay;
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();

                }
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay is IDataset)
                        {
                            pDataset = (IDataset)pLay;

                            if (pDataset.BrowseName.ToUpper() == sLName.ToUpper())
                            {
                                //functionReturnValue = pLay;

                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }
                            }
                            if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper())
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }


                            }
                            if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper())
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }

                            }
                            if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper())
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    FoundAsFeatureLayer = false;
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    FoundAsFeatureLayer = false;
                                    return pLay;
                                }

                            }
                        }
                        else if (pLay is IBasemapSubLayer)
                        {
                            if (((IBasemapSubLayer)pLay).Layer is IDataset)
                            {
                                pDataset = (IDataset)((IBasemapSubLayer)pLay).Layer;

                                if (pDataset.BrowseName.ToUpper() == sLName.ToUpper())
                                {
                                    //functionReturnValue = pLay;

                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }
                                }
                                if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper())
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }


                                }
                                if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper())
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }

                                }
                                if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper())
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {
                                        FoundAsFeatureLayer = false;
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        FoundAsFeatureLayer = false;
                                        return pLay;
                                    }

                                }

                                //{
                                //    return ((IBasemapSubLayer)pLay).Layer;
                                //}
                                //else
                                //{
                                //return pLay;
                                //}
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                pDataset = null;
                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static ILayer FindLayerInWorkspace(IMap pMap, string sLName, IWorkspace workspace)
        {

            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;


            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            IDataset pDataset = null;
            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                ILayer pLay = default(ILayer);
                pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay.Name.ToUpper() == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                        {
                            if (pLay is IBasemapSubLayer)
                            {
                                return ((IBasemapSubLayer)pLay).Layer;
                            }
                            else
                            {
                                return pLay;
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();

                }
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (!(pLay is IGroupLayer))
                    {
                        if (pLay is IDataset)
                        {
                            pDataset = (IDataset)pLay;

                            if (pDataset.BrowseName.ToUpper() == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                            {
                                //functionReturnValue = pLay;

                                if (pLay is IBasemapSubLayer)
                                {

                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {

                                    return pLay;
                                }
                            }
                            if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                            {
                                if (pLay is IBasemapSubLayer)
                                {

                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {

                                    return pLay;
                                }


                            }
                            if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                            {
                                if (pLay is IBasemapSubLayer)
                                {

                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {

                                    return pLay;
                                }

                            }
                            if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                            {
                                if (pLay is IBasemapSubLayer)
                                {
                                    return ((IBasemapSubLayer)pLay).Layer;
                                }
                                else
                                {
                                    return pLay;
                                }

                            }
                        }
                        else if (pLay is IBasemapSubLayer)
                        {
                            if (((IBasemapSubLayer)pLay).Layer is IDataset)
                            {
                                pDataset = (IDataset)((IBasemapSubLayer)pLay).Layer;

                                if (pDataset.BrowseName.ToUpper() == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                                {
                                    //functionReturnValue = pLay;

                                    if (pLay is IBasemapSubLayer)
                                    {
                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {
                                        return pLay;
                                    }
                                }
                                if (pDataset.FullName.NameString.ToUpper() == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {

                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {

                                        return pLay;
                                    }


                                }
                                if (pDataset.BrowseName.ToUpper().Substring(pDataset.BrowseName.LastIndexOf(".") + 1) == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {

                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {

                                        return pLay;
                                    }

                                }
                                if (pDataset.FullName.NameString.ToUpper().Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1) == sLName.ToUpper() && ((IWorkspace)pLay).Equals(workspace))
                                {
                                    if (pLay is IBasemapSubLayer)
                                    {

                                        return ((IBasemapSubLayer)pLay).Layer;
                                    }
                                    else
                                    {

                                        return pLay;
                                    }

                                }


                            }
                        }
                    }
                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                pDataset = null;
                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static ILayer FindLayerInGroup(ICompositeLayer pCompLayer, string sLName)
        {

            if (sLName == null)
                return null;

            if (sLName == "")
                return null;

            if (sLName.Trim() == "")
                return null;

            try
            {

                ILayer pLay = default(ILayer);

                for (int i = 0; i < pCompLayer.Count; i++)
                {
                    pLay = pCompLayer.get_Layer(i);

                    if (pLay.Name == sLName)
                    {
                        return pLay;
                    }

                }


                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {

            }

        }

        public static ILayer FindLayerByClassID(IMap pMap, string ClassID)
        {
            if (ClassID == null)
                return null;

            if (ClassID == "")
                return null;

            if (ClassID.Trim() == "")
                return null;
            if (IsNumeric(ClassID.Trim()) == false)
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            ILayer pLay = null;
            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                pLay = default(ILayer);
                // IDataset pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (pLay is IBasemapSubLayer)
                    {
                        pLay = ((IBasemapSubLayer)pLay).Layer;
                    }

                    if (pLay is IFeatureLayer)
                    {
                        IFeatureLayer pFlay = pLay as IFeatureLayer;
                        if (pFlay.FeatureClass != null)
                        {
                            if (pFlay.FeatureClass.FeatureClassID == Convert.ToInt32(ClassID))
                            {
                                return pLay;
                            }
                        }
                    }

                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static List<ILayer> FindLayersByClassID(IMap pMap, string ClassID)
        {
            if (ClassID == null)
                return null;

            if (ClassID == "")
                return null;

            if (ClassID.Trim() == "")
                return null;
            if (IsNumeric(ClassID.Trim()) == false)
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            List<ILayer> pLayLst = new List<ILayer>();

            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                ILayer pLay = default(ILayer);
                // IDataset pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (pLay is IBasemapSubLayer)
                    {
                        pLay = ((IBasemapSubLayer)pLay).Layer;
                    }
                    if (pLay is IFeatureLayer)
                    {
                        IFeatureLayer pFlay = pLay as IFeatureLayer;
                        if (pFlay.FeatureClass != null)
                        {
                            if (pFlay.FeatureClass.FeatureClassID == Convert.ToInt32(ClassID))
                            {
                                pLayLst.Add(pLay);
                            }
                        }
                    }

                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return pLayLst;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static List<ILayer> FindLayersByClassName(IApplication app, string ClassName)
        {
            return FindLayersByClassName(((IMxDocument)app.Document).FocusMap, ClassName);

        }
        public static List<ILayer> FindLayersByClassName(IMap pMap, string ClassName)
        {
            if (ClassName == null)
                return null;

            if (ClassName == "")
                return null;

            if (ClassName.Trim() == "")
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            List<ILayer> pLayLst = new List<ILayer>();
            IDataset pDs = null;
            try
            {
                //************************************************************************************
                //Produce by: Michael Miller *
                //Purpose: To return a refernece to a layer specified by sLName *
                //************************************************************************************


                ILayer pLay = default(ILayer);
                // IDataset pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);
                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();

                while (!(pLay == null))
                {
                    if (pLay is IBasemapSubLayer)
                    {
                        pLay = ((IBasemapSubLayer)pLay).Layer;
                    }
                    if (pLay is IFeatureLayer)
                    {
                        IFeatureLayer pFlay = pLay as IFeatureLayer;
                        pDs = pFlay.FeatureClass as IDataset;

                        if (pFlay.FeatureClass != null)
                        {
                            if (getClassName(pDs) == ClassName)
                            {
                                pLayLst.Add(pLay);
                            }
                        }
                    }

                    pLay = pEnumLayer.Next();
                }



                pLay = null;

                return pLayLst;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }
        public static List<ILayer> FindLayersByClassID(IMap pMap, int ClassID)
        {
            return FindLayersByClassID(pMap, ClassID.ToString());

        }
        public static string getClassName(IDataset Dataset)
        {
            try
            {
                if (Dataset.BrowseName != "" && Dataset.BrowseName.Contains("."))
                {
                    return Dataset.BrowseName.Substring(Dataset.BrowseName.LastIndexOf(".") + 1);
                }
            }
            catch
            { }


            try
            {
                if (Dataset.FullName != null)
                {
                    if (Dataset.FullName.NameString != "" && Dataset.FullName.NameString.Contains("."))
                    {
                        return Dataset.FullName.NameString.Substring(Dataset.FullName.NameString.LastIndexOf(".") + 1);
                    }
                }
            }
            catch
            { }
            try
            {
                if (Dataset.Name != "" && Dataset.Name.Contains("."))
                {
                    return Dataset.Name.Substring(Dataset.Name.LastIndexOf(".") + 1);
                }
            }
            catch
            { }
            try
            {
                if (Dataset.BrowseName != "")
                {
                    return Dataset.BrowseName;
                }
            }
            catch
            { }
            try
            {
                if (Dataset.FullName.NameString != "")
                {
                    return Dataset.FullName.NameString;
                }
            }
            catch
            { }
            return Dataset.Name;

        }
        public static string getClassName(ILayer Layer)
        {
            IDataset Dataset;
            try
            {
                Dataset = Layer as IDataset;
                return getClassName(Dataset);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errir in getClassName\n" + ex.Message);
                return "";
            }
            finally
            {
                Dataset = null;
            }
        }
        public static bool isVisible(ILayer pLayer, IMap pMap)
        {
            if (pLayer.Visible == false)
                return false;


            if (pLayer.MaximumScale != 0.0 && pLayer.MinimumScale != 0.0)
            {
                if (pMap.MapScale > pLayer.MaximumScale && pMap.MapScale < pLayer.MinimumScale)
                {

                }
                else
                    return false;
            }
            else if (pLayer.MaximumScale == 0.0 && pLayer.MinimumScale != 0.0)
            {
                if (pMap.MapScale > pLayer.MinimumScale)
                {
                    return false;
                }

            }
            else if (pLayer.MaximumScale != 0.0 && pLayer.MinimumScale == 0.0)
            {
                if (pMap.MapScale < pLayer.MaximumScale)
                {
                    return false;
                }

            }
            ILayer parLay = (ILayer)FindParentLayer(pLayer, pMap);
            if (parLay == null)
                return true;
            else
                return isVisible(parLay, pMap);



        }
        public static ArrayList FindAncestors(ILayer pLayer, IMap pMap)
        {
            ArrayList anc = null;
            ILayer pParentLayer = null;
            try
            {
                anc = new ArrayList();

                pParentLayer = (ILayer)FindParentLayer(pLayer, pMap);
                while (pParentLayer != null)
                {
                    anc.Add(pParentLayer);
                    pParentLayer = (ILayer)FindParentLayer(pParentLayer, pMap);
                }
                return anc;

            }
            catch
            {
                return null;
            }
            finally
            {
                anc = null;
                pParentLayer = null;
            }


        }
        public static ICompositeLayer FindParentLayer(ILayer pChildLayer, IMap pMap)
        {
            return FindParentLayer(pChildLayer, null, pMap);
        }
        public static ICompositeLayer FindParentLayer(ILayer pChildLayer, ICompositeLayer pCandidate, IMap pMap)
        {
            UID pUID = null;
            IEnumLayer pEnumLayer = null;
            ILayer pLayer = null;
            ILayer pParent = null;
            try
            {

                if ((pCandidate == null))
                {


                    pUID = new UID();
                    pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

                    pEnumLayer = pMap.get_Layers(pUID, true);
                    if (!(pEnumLayer == null))
                    {

                        pLayer = pEnumLayer.Next();
                        while (pLayer != null)
                        {

                            pParent = (ILayer)FindParentLayer(pChildLayer, (ICompositeLayer)pLayer, pMap);
                            if (!(pParent == null))
                            {
                                return (ICompositeLayer)pParent;

                            }
                            pLayer = pEnumLayer.Next();
                        }
                    }
                }
                else
                {
                    int l;
                    for (l = 0; (l < (pCandidate.Count)); l++)
                    {
                        if ((pCandidate.get_Layer(l) == pChildLayer))
                        {
                            return pCandidate;

                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                pUID = null;
                pEnumLayer = null;
                pLayer = null;

            }
        }
        public enum GNFlowDirection { AncillaryRole, Digitized }

        public static void EstablishFlow(GNFlowDirection flowDirection, IGeometricNetwork gn, IEnvelope env, IApplication app)
        {

            IEditor editor = null;
            IEditLayers eLayers = null;
            IMouseCursor appCursor = null;
            INetworkAnalysisExt netExt = null;
            UID pUID = null;
            IMap pMap = null;
            IMxDocument mxdoc = null;
            try
            {




                int calcCount = 0;

                //Get editor

                editor = Globals.getEditor(ref app);

                if (editor.EditState != esriEditState.esriStateEditing)
                {
                    MessageBox.Show("Must be editing.", "Desktop Tools");
                    return;
                }

                eLayers = editor as IEditLayers;

                //Change mouse cursor to wait - automatically changes back (ArcGIS Desktop only)
                appCursor = new MouseCursorClass();
                appCursor.SetCursor(2);

                ESRI.ArcGIS.esriSystem.IStatusBar statusBar = app.StatusBar;
                statusBar.set_Message(0, "Establishing flow direction. Please wait....");

                //Get NA Extension in order to update the current network with the first visible network
                pUID = new UIDClass();
                pUID.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
                netExt = app.FindExtensionByCLSID(pUID) as INetworkAnalysisExt;

                //Get Visible geometric networks
                pMap = editor.Map;




                bool editStarted = false;
                try
                {// Create an edit operation enabling undo/redo
                    editor.StartOperation();
                    editStarted = true;
                }
                catch
                {
                    editStarted = false;
                }

                IEnumFeatureClass enumFC = null;
                INetwork net = null;
                IUtilityNetworkGEN unet = null;
                IEnumNetEID edgeEIDs = null;
                //IFeatureLayer fLayer = null;
                try
                {


                    // fLayer = Globals.FindLayerByFeatureClass(pMap, gn.OrphanJunctionFeatureClass, false);
                    //if (fLayer == null)
                    //{
                    //    MessageBox.Show("Unable to set flow direction for " + gn.FeatureDataset.Name + ".  Add the " + gn.OrphanJunctionFeatureClass.AliasName + " to your map and try again, if needed", "Establish Flow Direction");
                    //    stepProgressor.Step();
                    //    continue;
                    //}
                    //if (!eLayers.IsEditable(fLayer))
                    //{
                    //    MessageBox.Show("Unable to set flow direction for " + gn.FeatureDataset.Name + ".  It is visible but not editable.", "Establish Flow Direction");
                    //    stepProgressor.Step();
                    //    continue;
                    //}
                    //Establish flow using AncillaryRole values
                    if (flowDirection == GNFlowDirection.AncillaryRole)
                    {
                        enumFC = gn.get_ClassesByNetworkAncillaryRole(esriNetworkClassAncillaryRole.esriNCARSourceSink);
                        if (enumFC.Next() == null)
                            MessageBox.Show("Flow direction for " + gn.FeatureDataset.Name + " not set.  No feature classes have source/sink capability." + Environment.NewLine +
                                            "You must recreate your geometric network to use this command on this network.", "Establish Flow");
                        else
                        {
                            gn.EstablishFlowDirection();
                            calcCount += 1;
                        }
                    }

                    //Establish flow direction based on digitized direction.
                    else
                    {
                        net = gn.Network;
                        unet = net as IUtilityNetworkGEN;

                        edgeEIDs = net.CreateNetBrowser(esriElementType.esriETEdge);
                        edgeEIDs.Reset(); int edgeEID;
                        for (long j = 0; j < edgeEIDs.Count; j++)
                        {
                            edgeEID = edgeEIDs.Next();
                            unet.SetFlowDirection(edgeEID, esriFlowDirection.esriFDWithFlow);
                        }
                        calcCount += 1;
                    }


                }

                catch (Exception ex)
                {
                    editor.AbortOperation();
                    MessageBox.Show("EstablishFlow\n" + ex.Message, ex.Source);
                }
                finally
                {

                    net = null;
                    unet = null;
                    edgeEIDs = null;
                    //fLayer = null;
                }
                if (editStarted)
                {   // Stop the edit operation
                    if (flowDirection == GNFlowDirection.AncillaryRole)
                        editor.StopOperation("Establish Flow");
                    else
                        editor.StopOperation("Establish Flow by Digitized Direction");
                }
                object Missing = Type.Missing;
                mxdoc = app.Document as IMxDocument;
                mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, Missing, mxdoc.ActiveView.Extent);

                if (app != null)
                    app.StatusBar.set_Message(2, "Flow direction established for " + calcCount + " geometric network(s).");

            }
            catch (Exception ex)
            {
                return;


            }
            finally
            {

                editor = null;
                eLayers = null;
                appCursor = null;
                netExt = null;
                pUID = null;
                pMap = null;

                mxdoc = null;
            }

        }
        public static IFeatureLayer FindLayerByFeatureClass(IMap map, IFeatureClass fc, bool mustBeVisible)
        {
            if (map == null)
                return null;

            IMapLayers mapLayers = null;

            UID geoFeatureLayerID = null;

            IEnumLayer enumLayer = null;
            IFeatureLayer testLayer = null;
            IDataset testDataset = null;
            try
            {

                mapLayers = (IMapLayers)map;

                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                // Step through each geofeature layer in the map
                enumLayer.Reset();
                testLayer = enumLayer.Next() as IFeatureLayer;
                while (testLayer != null)
                {
                    if (testLayer.Valid)
                    {
                        if ((!mustBeVisible) || (mustBeVisible && mapLayers.IsLayerVisible(testLayer)))
                        {
                            testDataset = testLayer.FeatureClass as IDataset;

                            if (testLayer.FeatureClass.FeatureClassID == fc.FeatureClassID && testLayer.FeatureClass.AliasName == fc.AliasName)
                            {
                                return testLayer as IFeatureLayer;
                            }
                        }

                    }
                    testLayer = enumLayer.Next() as IFeatureLayer;
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                mapLayers = null;

                geoFeatureLayerID = null;
                if (enumLayer != null)
                    Marshal.ReleaseComObject(enumLayer);
                enumLayer = null;

                testDataset = null;
            }


        }
        public static IFeatureLayer FindLayerByFeatureClass(ref IMap map, ref IFeatureClass fc, bool mustBeVisible)
        {
            IDataset ds = null;
            IMapLayers mapLayers = null;
            UID geoFeatureLayerID = null;
            IEnumLayer enumLayer = null;
            IFeatureLayer testLayer = null;
            IDataset testDataset = null;
            try
            {
                ds = fc as IDataset;
                mapLayers = (IMapLayers)map;
                //Get list of geofeature layers in the map
                geoFeatureLayerID = new UIDClass();
                geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);

                // Step through each geofeature layer in the map
                enumLayer.Reset();
                testLayer = enumLayer.Next() as IFeatureLayer;
                while (testLayer != null)
                {
                    if (testLayer.Valid)
                    {
                        if ((!mustBeVisible) | (mustBeVisible && mapLayers.IsLayerVisible(testLayer)))
                        {
                            testDataset = testLayer.FeatureClass as IDataset;

                            if (testLayer.FeatureClass.FeatureClassID == fc.FeatureClassID && testLayer.FeatureClass.AliasName == fc.AliasName)
                            {
                                return testLayer as IFeatureLayer;
                            }
                        }

                    }
                    testLayer = enumLayer.Next() as IFeatureLayer;
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                mapLayers = null;
                geoFeatureLayerID = null;
                enumLayer = null;
                testLayer = null;
                testDataset = null;
            }


        }
        public static bool LayerExist(IMap pMap, string sLName)
        {
            //************************************************************************************
            //Produce by: Michael Miller                                                         *
            //Purpose:    To return a refernece to a layer specified by sLName                   *
            //t************************************************************************************
            IEnumLayer pEnumLayer = default(IEnumLayer);
            ILayer pLay = default(ILayer);
            IDataset pDataset = default(IDataset);
            try
            {
                if (pMap == null) return false;


                if (pMap.LayerCount == 0)
                    return false;
                pEnumLayer = pMap.get_Layers(null, true);


                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();

                while (!(pLay == null))
                {

                    if ((pLay.Name).ToUpper() == (sLName).ToUpper())
                    {
                        return pLay.Valid;

                    }
                    if (pLay is IDataset)
                    {
                        pDataset = (IDataset)pLay;
                        if ((pDataset.BrowseName).ToUpper() == (sLName).ToUpper())
                        {
                            return pLay.Valid;

                        }
                        if ((pDataset.FullName.NameString).ToUpper() == (sLName).ToUpper())
                        {
                            return pLay.Valid;

                        }
                        if ((pDataset.FullName.NameString).Substring(pDataset.FullName.NameString.LastIndexOf(".") + 1).ToUpper() == (sLName).ToUpper())
                        {
                            return pLay.Valid;


                        }
                        if ((pDataset.BrowseName).Substring(pDataset.BrowseName.LastIndexOf(".") + 1).ToUpper() == (sLName).ToUpper())
                        {
                            return pLay.Valid;


                        }
                    }

                    pLay = pEnumLayer.Next();
                }



                return false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the Global Functions - LayerExist" + Environment.NewLine + ex.Message);
                return false;

            }
            finally
            {
                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
                pLay = null;
            }
        }
        public static System.String GetPathForALayer(ESRI.ArcGIS.Carto.ILayer layer)
        {
            if (layer == null || !(layer is ESRI.ArcGIS.Geodatabase.IDataset))
            {
                return null;
            }

            ESRI.ArcGIS.Geodatabase.IDataset dataset = null;
            IFeatureLayer pFLay = null;
            IFeatureDataset pFeatureDataset = null;

            try
            {
                dataset = (ESRI.ArcGIS.Geodatabase.IDataset)(layer);
                string strName = dataset.BrowseName;
                if (strName == "")
                {
                    strName = dataset.Name;

                }
                if (strName == "")
                {
                    strName = dataset.FullName.NameString.ToString();

                }
                if (layer is IFeatureLayer)
                {
                    pFLay = layer as IFeatureLayer;
                    if (pFLay.DataSourceType.ToLower().Contains("shapefile"))
                    {
                        strName += ".shp";
                    }
                    else if (pFLay.DataSourceType.ToLower().Contains("file geodatabase"))
                    {

                        if (pFLay.FeatureClass.FeatureDataset != null)
                        {
                            pFeatureDataset = pFLay.FeatureClass.FeatureDataset;
                            strName = pFeatureDataset.BrowseName + "\\" + strName;
                            //IWorkspace pWorkspace = pFeatureDataset.Workspace;
                        }


                    }
                    else if (pFLay.DataSourceType.ToLower().Contains("sde"))
                    {

                        if (pFLay.FeatureClass.FeatureDataset != null)
                        {
                            pFeatureDataset = pFLay.FeatureClass.FeatureDataset;
                            strName = pFeatureDataset.BrowseName + "\\" + strName;

                        }


                    }
                }
                strName = System.IO.Path.Combine(dataset.Workspace.PathName, strName);
                // strName = strName.Replace("\\\\", "\\");

                return (strName);
            }
            catch
            {
                return null;

            }
            finally
            {
                dataset = null;
                pFLay = null;
                pFeatureDataset = null;

            }

        }


        #endregion

        #region TableTools
        public static ITable FindTable(IApplication app, string sLName)
        {
            IStandaloneTable pStand = FindStandAloneTable(((IMxDocument)app.Document).FocusMap, sLName);
            if (pStand != null)

                return pStand.Table;

            else
                return null;
        }
        public static ITable FindTable(IMap pMap, string sLName)
        {

            IStandaloneTable pStand = FindStandAloneTable(pMap, sLName);
            if (pStand != null)

                return pStand.Table;

            else
                return null;
        }
        public static IStandaloneTable FindStandAloneTable(IApplication app, string sLName)
        {
            return FindStandAloneTable(((IMxDocument)app.Document).FocusMap, sLName);
        }
        public static IStandaloneTable FindStandAloneTable(IMap pMap, string sLName)
        {
            try
            {
                IDataset dataset;
                char[] c = new char[] { '.' };
                IStandaloneTable stTable;
                ITable table;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)pMap;
                long count = stTableColl.StandaloneTableCount;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        stTable = stTableColl.get_StandaloneTable(i);
                        if (stTable.Valid)
                        {
                            if (stTable.Name.ToLower() == sLName.ToLower())
                            {
                                return stTable;
                            }
                            table = stTable.Table;
                            dataset = table as IDataset;
                            string[] nameParts = dataset.BrowseName.Split(c);
                            //string[] nameParts = stTable.Name.Split(c);
                            string testName = nameParts[nameParts.Length - 1];

                            if (testName.ToLower() == sLName.ToLower())
                            {
                                return stTable;
                            }
                            nameParts = dataset.Name.Split(c);
                            //string[] nameParts = stTable.Name.Split(c);
                            testName = nameParts[nameParts.Length - 1];

                            if (testName.ToLower() == sLName.ToLower())
                            {
                                return stTable;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FindTable: " + ex.Message);
                return null;
            }

        }
        public static string FullPathOfLayer(object Item)
        {
            string fullPath = "";
            IFeatureLayer ifl = Item as IFeatureLayer;
            IDataset ds = ifl.FeatureClass as IDataset;
            fullPath += ds.Workspace.PathName;
            fullPath += @"\" + ds.Name;
            if (ifl.DataSourceType.ToLower().Contains("shapefile"))
            {
                fullPath += ".shp";
            }
            else
            {

            }
            return fullPath;
        }
        public static IFields createFeatureClassFields(ISpatialReference pSpatRef, esriGeometryType GeoType, string IDFieldName, string DateFieldName)
        {

            IFields pFields;
            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription;
            ESRI.ArcGIS.Geodatabase.IFieldsEdit pFieldsEdit;
            ESRI.ArcGIS.Geodatabase.IField pField;
            ESRI.ArcGIS.Geodatabase.IFieldEdit pFieldEdit;
            IGeometryDefEdit geomDefEdit;
            try
            {
                objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();

                pFields = objectClassDescription.RequiredFields;
                pFields = Globals.copyFields(pFields, null, null, IDFieldName, DateFieldName, false);
                pFieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)pFields; // Explicit Cast


                pField = pFields.get_Field(pFields.FindField("Shape"));
                pFieldEdit = (IFieldEdit)pField;
                geomDefEdit = (IGeometryDefEdit)pField.GeometryDef;
                geomDefEdit.GeometryType_2 = GeoType;
                geomDefEdit.SpatialReference_2 = pSpatRef;

                return pFields;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IFields createFeatureClassFieldsFromTableFields(IFields fields, ISpatialReference pSpatRef, esriGeometryType GeoType)
        {

            IFields pFields;


            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription;
            ESRI.ArcGIS.Geodatabase.IFieldsEdit pFieldsEdit;
            ESRI.ArcGIS.Geodatabase.IField pField;
            ESRI.ArcGIS.Geodatabase.IFieldEdit pFieldEdit;
            IGeometryDefEdit geomDefEdit;
            IClone pCln = null;
            try
            {
                objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();

                pFields = objectClassDescription.RequiredFields;
                pFieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)pFields; // Explicit Cast


                pField = pFields.get_Field(pFields.FindField("Shape"));
                pFieldEdit = (IFieldEdit)pField;
                geomDefEdit = (IGeometryDefEdit)pField.GeometryDef;
                geomDefEdit.GeometryType_2 = GeoType;
                geomDefEdit.SpatialReference_2 = pSpatRef;

                for (int i = 0; i < fields.FieldCount - 1; i++)
                {

                    pCln = (IClone)fields.get_Field(i);

                    pFieldsEdit.AddField((IField)pCln.Clone());

                }
                return pFields;

            }
            catch
            {
                return null;
            }
        }
        public static ITable createTableInMemory(string strName, IFields TableFields, IWorkspace pWS)
        {

            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;
            return pFWS.CreateTable(strName, TableFields, null, null, "");


        }
        public static IFeatureClass createFeatureClassInMemory(string strName, IFields FeatureFields, IWorkspace pWS, esriFeatureType featType)
        {
            ESRI.ArcGIS.esriSystem.UID CLSID = null;
            //ESRI.ArcGIS.esriSystem.UID CLSEXT = null;
            IFeatureWorkspace pFWS = null;
            IFeatureClass newFeat = null;
            ESRI.ArcGIS.Geodatabase.IFieldChecker fieldChecker = null;
            ESRI.ArcGIS.Geodatabase.IEnumFieldError enumFieldError = null;
            ESRI.ArcGIS.Geodatabase.IFields validatedFields = null;
            try
            {
                //CLSEXT = null;

                pFWS = (IFeatureWorkspace)pWS;


                if (CLSID == null)
                {
                    CLSID = new ESRI.ArcGIS.esriSystem.UIDClass();
                    CLSID.Value = "esriGeoDatabase.Feature";
                }


                fieldChecker = new ESRI.ArcGIS.Geodatabase.FieldCheckerClass();
                enumFieldError = null;
                validatedFields = null;
                fieldChecker.ValidateWorkspace = pWS;
                try
                {
                    fieldChecker.Validate(FeatureFields, out enumFieldError, out validatedFields);

                }
                catch (Exception e)
                {
                    validatedFields = FeatureFields;
                }
                bool FCCreated = false;

                int loopCnt = 0;
                if (featType == esriFeatureType.esriFTComplexEdge)
                {
                    featType = esriFeatureType.esriFTSimple;
                }
                if (featType == esriFeatureType.esriFTComplexJunction)
                {
                    featType = esriFeatureType.esriFTSimple;
                }
                if (featType == esriFeatureType.esriFTSimpleJunction)
                {
                    featType = esriFeatureType.esriFTSimple;
                }
                while (FCCreated == false)
                {
                    try
                    {
                        if (loopCnt == 0)
                        {
                            loopCnt = loopCnt + 1;
                            newFeat = pFWS.CreateFeatureClass(strName, validatedFields, null, null, featType, "SHAPE", "");
                        }
                        else
                        {
                            loopCnt = loopCnt + 1;
                            newFeat = pFWS.CreateFeatureClass(strName + (loopCnt - 1).ToString(), validatedFields, null, null, featType, "SHAPE", "");
                        }
                        FCCreated = true;
                    }
                    catch (Exception ex)
                    {
                        FCCreated = false;
                    }
                    if (loopCnt == 5)
                        FCCreated = true;

                }
                return newFeat;

            }
            catch (Exception ex)
            {
                return null;

            }
            finally
            {
                CLSID = null;

                pFWS = null;

                fieldChecker = null;
                enumFieldError = null;
                validatedFields = null;
            }
        }
        #endregion

        #region Raster_TIN_Tools
        public static string GetCellValue(string layerName, IPoint pLoc, IMap pMap)
        {
            IPnt pBlockSize = null;
            IRasterLayer pLayer = null;
            IPixelBlock pPixelBlock = null;

            IRasterProps pRasterProps = null;
            IPnt pPixel = null;

            object vValue = null;
            double dXSize;
            double dYSize;

            try
            {
                pBlockSize = new DblPnt();

                pBlockSize.SetCoords(1.0, 1.0);


                pPixel = new DblPnt();

                pLayer = GetRasterLayer(pMap, layerName);
                pPixelBlock = pLayer.Raster.CreatePixelBlock(pBlockSize);
                pRasterProps = pLayer.Raster as IRasterProps;
                dXSize = pRasterProps.Extent.XMax - pRasterProps.Extent.XMin;
                dYSize = pRasterProps.Extent.YMax - pRasterProps.Extent.YMin;

                dXSize = dXSize / pRasterProps.Width;
                dYSize = dYSize / pRasterProps.Height;

                pPixel.X = (pLoc.X - pRasterProps.Extent.XMin) / dXSize;
                pPixel.Y = (pRasterProps.Extent.YMax - pLoc.Y) / dYSize;

                pLayer.Raster.Read(pPixel, pPixelBlock);

                for (int j = 0; j < pPixelBlock.Planes; j++)
                {
                    vValue = pPixelBlock.GetVal(j, 0, 0);

                    if (vValue.ToString() != "No Raster")
                        return vValue.ToString(); //= sPixelVals + ", ";

                }

                return "No Raster";
            }
            catch
            { return "No Raster"; }
            finally
            {
                pBlockSize = null;
                pLayer = null;
                pPixelBlock = null;

                pRasterProps = null;
                pPixel = null;

                vValue = null;
            }



        }
        public static IRasterLayer GetRasterLayer(IMap map, string layerName)
        {
            UID geoRasterLayerID = null;
            IEnumLayer enumLayer = null;
            IRasterLayer layer = null;
            try
            {

                if (layerName == "")
                    return null;
                //Get list of geofeature layers in the map
                geoRasterLayerID = new UIDClass();
                geoRasterLayerID.Value = "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}";
                enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoRasterLayerID), true);

                // Step through each geofeature layer in the map
                enumLayer.Reset();
                layer = enumLayer.Next() as IRasterLayer;

                while (layer != null)
                {
                    if (layer.Valid)
                    {


                        if ((layer.Name).ToUpper() == (layerName).ToUpper())
                        {
                            return layer;

                        }


                    }
                    layer = enumLayer.Next() as IRasterLayer;
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (enumLayer != null)
                    Marshal.ReleaseComObject(enumLayer);
                geoRasterLayerID = null;
                enumLayer = null;

            }

        }
        public static ISurface GetSurface(ILayer pLayer)
        {
            ITinAdvanced pTin = null;
            ITinLayer pActiveTinLayer = null;
            IRasterLayer pRasterLayer = null;
            IRasterBandCollection pRasterBands = null;
            IRasterBand pRasterBand = null;
            IRasterSurface pRasterSurface = null;
            //IGeoDataset pGeoDataset = null;
            //IRasterStatistics pRasterStats = null;
            try
            {
                if (pLayer is ITinLayer)
                {
                    // SELECTED LAYER IS A TIN LAYER

                    pActiveTinLayer = pLayer as ITinLayer;

                    pTin = pActiveTinLayer.Dataset as ITinAdvanced;

                    // SET SURFACE
                    return pTin as ISurface;

                }
                else
                {
                    // SELECTED LAYER IS A RASTER LAYER

                    pRasterLayer = pLayer as IRasterLayer;

                    pRasterBands = pRasterLayer.Raster as IRasterBandCollection;

                    pRasterBand = pRasterBands.Item(0);

                    pRasterSurface = new RasterSurface();
                    pRasterSurface.RasterBand = pRasterBand;

                    //pGeoDataset = (IGeoDataset)pRasterBand.RasterDataset;

                    // pRasterStats = pRasterBand.Statistics;

                    // SET SURFACE
                    return pRasterSurface as ISurface;
                }
            }
            catch
            {
                return null;
            }
            finally
            {


                pActiveTinLayer = null;
                pRasterLayer = null;
                pRasterBands = null;
                pRasterBand = null;

            }

        }
        #endregion

        #region WorkspaceTools
        public static IFields copyFields(IFields SourceFields, IField lenFld, IField areaField, string IDFieldName, string DateFieldName, bool removeMZ)
        {
            IFields pFields = null;
            IFieldsEdit pFieldsEdit = null;
            IField pField = null;
            IFieldEdit pFieldEdit = null;
            IField SourceField = null;
            IGeometryDefEdit pGeomDefEdit = null;
            IClone clone = null;
            try
            {

                pFields = new FieldsClass();
                pFieldsEdit = (IFieldsEdit)pFields;
                int totFlds = SourceFields.FieldCount;
                bool idFieldExist = false;
                bool dateFieldExist = false;
                int resFldCnt = totFlds;

                for (int i = 0; i < totFlds; i++)
                {
                    SourceField = SourceFields.get_Field(i);
                    if (IDFieldName != null)
                    {
                        if (SourceField.Name == IDFieldName)
                        {
                            idFieldExist = true;
                            IDFieldName = null;

                        }
                    }

                    if (DateFieldName != null)
                    {
                        if (SourceField.Name == DateFieldName)
                        {
                            dateFieldExist = true;
                            DateFieldName = null;

                        }
                    }
                    if (SourceField == lenFld)
                    {
                        resFldCnt = resFldCnt - 1;
                    }
                    if (SourceField == areaField)
                    {
                        resFldCnt = resFldCnt - 1;
                    }
                }

                if (idFieldExist == false && IDFieldName != null)
                {
                    resFldCnt++;

                }
                if (dateFieldExist == false && DateFieldName != null)
                {
                    resFldCnt++;

                }

                pFieldsEdit.FieldCount_2 = resFldCnt;
                int addFldIdx = 0;
                for (int i = 0; i < totFlds; i++)
                {
                    SourceField = SourceFields.get_Field(i);

                    if (SourceField == lenFld ||
                       SourceField == areaField)
                    {
                        //pField = new FieldClass();
                        //pFieldEdit = (IFieldEdit)pField;
                        //pFieldEdit.Editable_2 = SourceField.Editable;
                        //pFieldEdit.Name_2 = SourceField.Name;
                        //pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                        //pFieldEdit.Length_2 = SourceField.Length;
                        //pFieldEdit.Precision_2 = SourceField.Precision;
                        //pFieldEdit.Type_2 = SourceField.Type;
                        //pFieldsEdit.set_Field(addFldIdx, pField);
                        //addFldIdx++;
                    }
                    else if (SourceField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        clone = SourceField as IClone;
                        pField = clone.Clone() as IField;

                        if (removeMZ)
                        {
                            pGeomDefEdit = (IGeometryDefEdit)pField.GeometryDef;
                            pGeomDefEdit.HasM_2 = false;
                            pGeomDefEdit.HasZ_2 = false;
                        }
                        pFieldsEdit.set_Field(addFldIdx, pField);
                        addFldIdx++;
                    }

                    else if (SourceField.Type == esriFieldType.esriFieldTypeOID)
                    {
                        clone = SourceField as IClone;
                        pField = clone.Clone() as IField;
                        pFieldsEdit.set_Field(addFldIdx, pField);
                        addFldIdx++;

                    }
                    else if (SourceField.Type == esriFieldType.esriFieldTypeGlobalID)
                    {
                        pField = new FieldClass();
                        pFieldEdit = (IFieldEdit)pField;
                        pFieldEdit.Editable_2 = true;
                        pFieldEdit.Name_2 = SourceField.Name;
                        pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                        pFieldEdit.Length_2 = SourceField.Length;
                        pFieldEdit.Precision_2 = SourceField.Precision;
                        pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGUID;
                        pFieldsEdit.set_Field(addFldIdx, pField);
                        addFldIdx++;

                    }
                    else if (SourceField.Editable)
                    {
                        //IClone clone = SourceField as IClone;
                        //pField = clone.Clone() as IField;
                        //pFieldsEdit.set_Field(i, pField);

                        pField = new FieldClass();
                        pFieldEdit = (IFieldEdit)pField;
                        pFieldEdit.Editable_2 = SourceField.Editable;
                        pFieldEdit.Name_2 = SourceField.Name;
                        pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                        pFieldEdit.Length_2 = SourceField.Length;
                        pFieldEdit.Precision_2 = SourceField.Precision;
                        pFieldEdit.Type_2 = SourceField.Type;
                        pFieldsEdit.set_Field(addFldIdx, pField);
                        addFldIdx++;
                    }
                    else
                    {

                        pField = new FieldClass();
                        pFieldEdit = (IFieldEdit)pField;
                        pFieldEdit.Editable_2 = SourceField.Editable;
                        pFieldEdit.Name_2 = SourceField.Name;
                        pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                        pFieldEdit.Length_2 = SourceField.Length;
                        pFieldEdit.Precision_2 = SourceField.Precision;
                        pFieldEdit.Type_2 = SourceField.Type;
                        pFieldsEdit.set_Field(addFldIdx, pField);
                        addFldIdx++;

                    }
                    //else
                    //{
                    //    pField = new FieldClass();
                    //    pFieldEdit = (IFieldEdit)pField;
                    //    pFieldEdit.Editable_2 = true;
                    //    pFieldEdit.Name_2 = SourceField.Name;
                    //    pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                    //    pFieldEdit.Length_2 = SourceField.Length;
                    //    pFieldEdit.Precision_2 = SourceField.Precision;
                    //    pFieldEdit.Type_2 = SourceField.Type;
                    //}

                }
                if (IDFieldName != null)
                {
                    pField = new FieldClass();
                    pFieldEdit = (IFieldEdit)pField;
                    pFieldEdit.Editable_2 = true;
                    pFieldEdit.Name_2 = IDFieldName;
                    pFieldEdit.IsNullable_2 = true;
                    pFieldEdit.Length_2 = 50;
                    pFieldEdit.Precision_2 = 0;
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldsEdit.set_Field(addFldIdx, pField);
                    addFldIdx++;
                }
                if (DateFieldName != null)
                {
                    pField = new FieldClass();
                    pFieldEdit = (IFieldEdit)pField;
                    pFieldEdit.Editable_2 = true;
                    pFieldEdit.Name_2 = DateFieldName;
                    pFieldEdit.IsNullable_2 = true;
                    //pFieldEdit.Length_2 = 50;
                    //pFieldEdit.Precision_2 = 0;
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                    pFieldsEdit.set_Field(addFldIdx, pField);
                    addFldIdx++;
                }
                return pFields;

            }
            catch (Exception ex)
            {
                MessageBox.Show("copyFields: " + ex.Message);
                return null;
            }
            finally
            {
                pFields = null;
                pFieldsEdit = null;
                pField = null;
                pFieldEdit = null;
                SourceField = null;
                pGeomDefEdit = null;
                clone = null;
            }

        }
        public static IFields createFieldsFromSourceFields(IFields SourceFields)
        {
            // create fields
            IFields pFields;
            IFieldsEdit pFieldsEdit;
            IField pField;
            IFieldEdit pFieldEdit;

            pFields = new FieldsClass();
            pFieldsEdit = (IFieldsEdit)pFields;
            int iFldCnt = SourceFields.FieldCount;
            pFieldsEdit.FieldCount_2 = iFldCnt;

            for (int i = 0; i < iFldCnt; i++)
            {
                IField SourceField = SourceFields.get_Field(i);

                if (SourceField.Editable || SourceField.Type == esriFieldType.esriFieldTypeOID || SourceField.Type == esriFieldType.esriFieldTypeGeometry)
                {
                    IClone clone = SourceField as IClone;
                    pField = clone.Clone() as IField;
                }
                else
                {
                    pField = new FieldClass();
                    pFieldEdit = (IFieldEdit)pField;
                    pFieldEdit.Editable_2 = true;
                    pFieldEdit.Name_2 = SourceField.Name;
                    pFieldEdit.IsNullable_2 = SourceField.IsNullable;
                    pFieldEdit.Length_2 = SourceField.Length;
                    pFieldEdit.Precision_2 = SourceField.Precision;
                    pFieldEdit.Type_2 = SourceField.Type;
                }
                pFieldsEdit.set_Field(i, pField);
            }
            return pFields;
        }

        public static IWorkspace CreateInMemoryWorkspace()
        {
            IWorkspaceFactory workspaceFactory = null;
            IWorkspaceName workspaceName = null;
            IName name = null;
            IWorkspace workspace = null;
            try
            {
                // Create an InMemory workspace factory.
                workspaceFactory = new InMemoryWorkspaceFactoryClass();

                // Create an InMemory geodatabase.
                workspaceName = workspaceFactory.Create("", "MyWorkspace",
                 null, 0);

                // Cast for IName.
                name = (IName)workspaceName;

                //Open a reference to the InMemory workspace through the name object.
                workspace = (IWorkspace)name.Open();
                return workspace;
            }
            catch
            {
                return null;

            }
            finally
            {
                workspaceFactory = null;
                workspaceName = null;
                name = null;
            }
        }
        public static IWorkspace GetInMemoryWorkspaceFromTOC(IMap pMap)
        {


            if (pMap == null)
                return null;
            if (pMap.LayerCount == 0)
                return null;

            //Layer functionReturnValue = default(ILayer);
            IEnumLayer pEnumLayer = default(IEnumLayer);
            IDataset pDataset = null;
            ILayer pLay = null;
            try
            {

                pLay = default(ILayer);
                pDataset = default(IDataset);

                pEnumLayer = pMap.get_Layers(null, true);

                pEnumLayer.Reset();
                pLay = pEnumLayer.Next();
                while (!(pLay == null))
                {
                    if (pLay.Valid)
                    {
                        if (!(pLay is IGroupLayer))
                        {
                            if (pLay is IDataset)
                            {
                                pDataset = (IDataset)pLay;
                                if (pDataset.Workspace.WorkspaceFactory is InMemoryWorkspaceFactoryClass)
                                {
                                    return pDataset.Workspace;

                                }
                                else if (pDataset.Workspace.WorkspaceFactory.WorkspaceType.ToString() == "99")
                                {
                                    return pDataset.Workspace;

                                }



                            }
                            else if (pLay is IBasemapSubLayer)
                            {
                                if (((IBasemapSubLayer)pLay).Layer is IDataset)
                                {
                                    pDataset = (IDataset)((IBasemapSubLayer)pLay).Layer;

                                    pDataset = (IDataset)pLay;
                                    if (pDataset.Workspace.WorkspaceFactory is InMemoryWorkspaceFactory)
                                    {
                                        return pDataset.Workspace;

                                    }
                                }
                            }
                        }
                    }
                    pLay = pEnumLayer.Next();
                }





                return null;
            }
            catch //()//Exception ex)
            {
                //MessageBox.Show("Error in the Costing Tools - FindLayer" + Environment.NewLine  + ex.Message);
                return null;
            }
            finally
            {
                pLay = null;
                pDataset = null;
                if (pEnumLayer != null)
                    Marshal.ReleaseComObject(pEnumLayer);
                pEnumLayer = null;
            }

        }

        #endregion

        #region FieldTools
        public static IField GetField(IFeatureLayer pLayer, string pName)
        {
            if (pLayer == null)
                return null;
            if (pLayer.FeatureClass == null)
                return null;
            return GetField(pLayer.FeatureClass.Fields, pName);


        }
        public static int GetFieldIndex(IFeatureLayer pLayer, string pName)
        {
            if (pLayer == null)
                return -1;
            if (pLayer.FeatureClass == null)
                return -1;
            return GetFieldIndex(pLayer.FeatureClass.Fields, pName);


        }
        public static IField GetField(IFields pFields, string pName)
        {
            int index = GetFieldIndex(pFields, pName);



            if (index >= 0)
                return pFields.get_Field(index);
            else
                return null;


        }
        public static int GetFieldIndex(IFields pFields, string pName)
        {
            pName = pName.Trim();

            int index = -1;

            index = pFields.FindField(pName);

            if (index >= 0)
                return index;
            else
            {
                index = pFields.FindFieldByAliasName(pName);
                if (index >= 0)
                    return index;
                else
                    return -1;
            }

        }
        #endregion

        #region TraceTools




        public static int SelectJunctions(IFeatureLayer pFL, IGeometry Extent, int numberOfEdges, string comparsionAbbrev, ref  IProgressDialog2 progressDialog, ref IStepProgressor stepProgressor, ref ITrackCancel trackCancel)
        {
            if (pFL == null) return 0;

            IFeatureClass featureClass = null;
            ISpatialFilter spatialFilter = null;
            IFeatureCursor featureCursor = null;

            IFeature feature = null;
            IFeatureSelection featureSel = null;
            ISimpleJunctionFeature junctionFeature = null;
            int Count = 0;
            Label pLbl = null;
            IFeature feat1 = null;
            IFeature feat2 = null;
            try
            {
                featureClass = pFL.FeatureClass;



                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = Extent;
                spatialFilter.GeometryField = featureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                int totSel = pFL.FeatureClass.FeatureCount(spatialFilter);
                stepProgressor.StepValue = 0;

                pLbl = new Label();
                featureSel = (IFeatureSelection)pFL;

                featureCursor = featureClass.Search(spatialFilter, false);

                int k = 0;

                feature = featureCursor.NextFeature();
                while (feature != null)
                {
                    junctionFeature = (ISimpleJunctionFeature)feature;

                    k += 1;
                    //Update progress bar
                    progressDialog.Description = pFL.Name + ": " + A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_3") + k.ToString() + A4LGSharedFunctions.Localizer.GetString("Of") + totSel.ToString() + ".";
                    stepProgressor.Step();

                    //Check if the cancel button was pressed. If so, stop process
                    bool boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        break;
                    }

                    #region select features where edgecount meets request number
                    switch (comparsionAbbrev)
                    {
                        //special case
                        case "ORPHAN":
                            if (junctionFeature.EdgeFeatureCount == 0)
                            {
                                Count += 1;
                                featureSel.Add(feature);
                            }
                            else if (junctionFeature.EdgeFeatureCount == 2)
                            {
                                feat1 = junctionFeature.get_EdgeFeature(0) as IFeature;
                                feat2 = junctionFeature.get_EdgeFeature(1) as IFeature;
                                if ((feat1.OID == feat2.OID) && (feat1.Class.ObjectClassID == feat2.Class.ObjectClassID))
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                            }
                            break;
                        case "LT":
                            {
                                if (junctionFeature.EdgeFeatureCount < numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                    break;
                                }
                                break;
                            }
                        case "LE":
                            {
                                if (junctionFeature.EdgeFeatureCount <= numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }
                        case "GT":
                            {
                                if (junctionFeature.EdgeFeatureCount > numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }
                        case "GE":
                            {
                                if (junctionFeature.EdgeFeatureCount >= numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }
                        case "EQ":
                            {
                                if (junctionFeature.EdgeFeatureCount == numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }
                        case "NE":
                            {
                                if (junctionFeature.EdgeFeatureCount == numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }
                        default: //LT
                            {
                                if (junctionFeature.EdgeFeatureCount < numberOfEdges)
                                {
                                    Count += 1;
                                    featureSel.Add(feature);
                                }
                                break;
                            }

                    }
                    #endregion

                    feature = featureCursor.NextFeature();
                }


                return Count;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (featureCursor != null)
                {
                    Marshal.ReleaseComObject(featureCursor);
                }
                featureClass = null;
                spatialFilter = null;
                featureCursor = null;

                feature = null;
                featureSel = null;
                junctionFeature = null;

                pLbl = null;
                feat1 = null;
                feat2 = null;
            }


        }

        public static int SelectEdges(IFeatureLayer pFL, IGeometry Extent, ref  IProgressDialog2 progressDialog, ref IStepProgressor stepProgressor, ref ITrackCancel trackCancel, int OrphanObjectClassID)
        {
            if (pFL == null) return 0;
            IFeatureClass featureClass = null;
            ISpatialFilter spatialFilter = null;
            IFeatureCursor featureCursor = null;

            IFeature feature = null;
            IFeatureSelection featureSel = null;
            IEdgeFeature edgeFeature = null;
            Label pLbl = null;
            int Count = 0;
            try
            {
                featureClass = pFL.FeatureClass;
                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = Extent;
                spatialFilter.GeometryField = featureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                int totSel = pFL.FeatureClass.FeatureCount(spatialFilter);
                stepProgressor.StepValue = 0;

                pLbl = new Label();
                featureSel = (IFeatureSelection)pFL;

                featureCursor = featureClass.Search(spatialFilter, false);

                int k = 0;

                feature = featureCursor.NextFeature();
                while (feature != null)
                {
                    edgeFeature = (IEdgeFeature)feature;

                    k += 1;
                    //Update progress bar
                    progressDialog.Description = A4LGSharedFunctions.Localizer.GetString("SltByJctCountProc_3") + ": " + pFL.Name + " " + k.ToString() + A4LGSharedFunctions.Localizer.GetString("of") + totSel.ToString() + ".";
                    stepProgressor.Step();

                    //Check if the cancel button was pressed. If so, stop process
                    bool boolean_Continue = trackCancel.Continue();
                    if (!boolean_Continue)
                    {
                        break;
                    }


                    if ((edgeFeature.FromJunctionEID < 0) || (edgeFeature.ToJunctionEID < 0))
                    {
                        Count += 1;
                        featureSel.Add(feature);
                    }
                    else if (((edgeFeature.FromJunctionFeature as IFeature).Class.ObjectClassID == OrphanObjectClassID) || ((edgeFeature.ToJunctionFeature as IFeature).Class.ObjectClassID == OrphanObjectClassID))
                    {
                        Count += 1;
                        featureSel.Add(feature);
                    }
                    feature = featureCursor.NextFeature();
                }


                return Count;
            }
            catch { return 0; }
            finally
            {
                if (featureCursor != null)
                {
                    Marshal.ReleaseComObject(featureCursor);
                }
                featureClass = null;
                spatialFilter = null;
                featureCursor = null;

                feature = null;
                featureSel = null;
                edgeFeature = null;
                pLbl = null;
            }




        }

        public static void RemoveTraceGraphics(IMap map, bool PartialRefresh)
        {

            if (map == null)
                return;

            IActiveView av = null;
            IGraphicsContainer graphics = null;
            IElementProperties elemProp = null;
            IElement elem = null;
            int count = 0;
            try
            {
                av = (IActiveView)map;
                graphics = map as IGraphicsContainer;
                graphics.Reset();
                elem = graphics.Next();
                while (elem != null)
                {
                    elemProp = (IElementProperties)elem;
                    if (elemProp.Name.Contains("MoveFeatureFlag") || elemProp.Name == "TraceFlag" || elemProp.Name == "TraceResults" || elemProp.Name == "SewerProfileFlag" || elemProp.Name == "SewerProfileFlag1" || elemProp.Name == "SewerProfileFlag2" || elemProp.Name.Contains("ProfileGraph"))
                    {
                        count += 1;
                        graphics.DeleteElement(elem);
                    }
                    elem = graphics.Next();
                }
                if (count > 0 && PartialRefresh)
                {
                    av.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }

            }
            catch { }
            finally
            {
                av = null;
                graphics = null;
                elemProp = null;
                elem = null;
            }

        }

        public static void GetEIDInfoListByFCWithHT(ref Hashtable inHT, List<int> featureClassIds, string operableFieldNameSources, string[] opValues, IEnumNetEID juncEIDs, IEIDHelper eidHelper)
        {
            if (inHT == null)
                inHT = new Hashtable();
            IEnumEIDInfo allEnumEidInfo = null;
            IEIDInfo testEidInfo = null;

            try
            {

                allEnumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);

                testEidInfo = allEnumEidInfo.Next();
                while (testEidInfo != null)
                {
                    if (featureClassIds.Contains(testEidInfo.Feature.Class.ObjectClassID))
                    {
                        if (testEidInfo.Feature.Fields.FindField(operableFieldNameSources) > 0)
                        {
                            if (testEidInfo.Feature.get_Value(testEidInfo.Feature.Fields.FindField(operableFieldNameSources)) == null)
                            {
                                inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                            }
                            else
                            {
                                if (testEidInfo.Feature.get_Value(testEidInfo.Feature.Fields.FindField(operableFieldNameSources)).ToString() == opValues[0])
                                {
                                    //inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                                }
                                else
                                {
                                    inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                                }
                            }

                        }
                        else if (testEidInfo.Feature.Fields.FindFieldByAliasName(operableFieldNameSources) > 0)
                        {
                            if (testEidInfo.Feature.get_Value(testEidInfo.Feature.Fields.FindFieldByAliasName(operableFieldNameSources)) == null)
                            {
                                inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                            }
                            else
                            {
                                if (testEidInfo.Feature.get_Value(testEidInfo.Feature.Fields.FindFieldByAliasName(operableFieldNameSources)).ToString() == opValues[0])
                                {
                                    //inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                                }
                                else
                                {
                                    inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                                }
                            }

                        }
                        else
                            inHT.Add(testEidInfo.Feature.OID, testEidInfo);
                    }
                    testEidInfo = allEnumEidInfo.Next();
                }

                //return outputEIDInfoHT;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (allEnumEidInfo != null)
                {
                    Marshal.ReleaseComObject(allEnumEidInfo);
                }
                allEnumEidInfo = null;

                if (testEidInfo != null)
                {
                    Marshal.ReleaseComObject(testEidInfo);
                }

                testEidInfo = null;
                GC.Collect();
                GC.WaitForFullGCComplete(300);
            }

        }
        public static List<int[]> GetOperableValveOIDs(IFeatureClass[] valveFCs, string operableFieldNameValves, string[] opValues, string addSQL)
        {
            IFeatureCursor fCursor = null;

            List<int[]> userIds = new List<int[]>();
            IQueryFilter qf = null;
            IField opField = null;
            IFeature feat = null;
            try
            {
                foreach (IFeatureClass valveFC in valveFCs)
                {
                    //Find field
                    int[] userId;
                    int operableFieldPos = valveFC.FindField(operableFieldNameValves);
                    qf = new QueryFilterClass();
                    qf.SubFields = valveFC.OIDFieldName;
                    if (operableFieldPos > -1 && opValues.Length == 2 && opValues[0] != "")
                    {
                        opField = valveFC.Fields.get_Field(operableFieldPos);
                        if (opField.Type == esriFieldType.esriFieldTypeInteger || opField.Type == esriFieldType.esriFieldTypeSmallInteger)
                        {
                            qf.WhereClause = "(" + operableFieldNameValves + " <> " + opValues[0] + " or " + operableFieldNameValves + " Is Null )";
                        }
                        else
                        {
                            qf.WhereClause = "(" + operableFieldNameValves + " <> '" + opValues[0] + "' or " + operableFieldNameValves + " Is Null )";
                        }

                    }
                    if (addSQL != "")
                    {
                        if (qf.WhereClause == "")
                        {
                            qf.WhereClause = addSQL;
                        }
                        else
                        {
                            qf.WhereClause = qf.WhereClause + " AND " + addSQL;
                        }
                    }

                    fCursor = valveFC.Search(qf, true);
                    feat = fCursor.NextFeature();
                    int selCount = valveFC.FeatureCount(qf);
                    userId = new int[selCount];

                    if (selCount != 0)
                    {
                        for (int i = 0; i < selCount; i++)
                        {
                            //userId.Add(feat.OID);
                            userId.SetValue(feat.OID, i);
                            Marshal.ReleaseComObject(feat);
                            feat = fCursor.NextFeature();
                        }

                    }
                    userIds.Add(userId);
                    if (fCursor != null)
                    {
                        Marshal.ReleaseComObject(fCursor);
                    }
                    fCursor = null;
                    GC.Collect();
                    GC.WaitForFullGCComplete(300);


                }
                return userIds;//.ToArray();

                //return userIds;
            }
            catch (Exception ex)
            {

                return null;

            }
            finally
            {
                if (fCursor != null)
                {
                    Marshal.ReleaseComObject(fCursor);
                }
                fCursor = null;


                userIds = null;

                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                qf = null;

                if (opField != null)
                {
                    Marshal.ReleaseComObject(opField);
                }
                opField = null;

                if (feat != null)
                {
                    Marshal.ReleaseComObject(feat);
                }
                feat = null;
                GC.Collect();
                GC.WaitForFullGCComplete(300);

            }

        }

        public static string SelectValveJunctions(ref IMap map, ref Hashtable valveHT, ref List<IFeatureLayer> valvesFLayer, bool selectFeat)
        {
            IFeatureSelection featSel = null;
            IActiveView av = null;
            IEIDInfo valveEidInfo = null;
            try
            {
                if (selectFeat)
                {
                    av = (IActiveView)map;

                    if (valveHT.Count == 0) return "0";


                    foreach (IFeatureLayer pLay in valvesFLayer)
                    {
                        featSel = pLay as IFeatureSelection;


                        featSel.Clear();
                    }

                    foreach (DictionaryEntry entry in valveHT)
                    {
                        valveEidInfo = entry.Value as IEIDInfo;
                        foreach (IFeatureLayer pLay in valvesFLayer)
                        {
                            if (valveEidInfo.Feature.Class.ObjectClassID == pLay.FeatureClass.ObjectClassID)
                            {
                                featSel = pLay as IFeatureSelection;
                                featSel.Add(valveEidInfo.Feature);
                            }
                        }

                    }
                }
                return valveHT.Count.ToString();

            }
            catch
            {
                return "0";
            }
            finally
            {
                valveEidInfo = null;
                av = null;
                featSel = null;
            }



        }
        public static void SelectValveJunctionsByName(ref IMap map, ref Hashtable valveHT, string valveFLName, bool DrawResults)
        {
            IFeatureLayer valveFLayer = null;

            IFeatureSelection featSel = null;
            IActiveView av = null;
            IEIDInfo valveEidInfo = null;
            try
            {
                bool FCorLayer = true;

                valveFLayer = FindLayer(map, valveFLName, ref FCorLayer) as IFeatureLayer;

                featSel = valveFLayer as IFeatureSelection;
                av = (IActiveView)map;
                if (valveHT.Count == 0) return;
                if (featSel.SelectionSet.Count > 0)
                {

                    featSel.Clear();
                }

                foreach (DictionaryEntry entry in valveHT)
                {
                    valveEidInfo = entry.Value as IEIDInfo;
                    featSel.Add(valveEidInfo.Feature);
                }
                if (DrawResults)
                    av.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            }
            catch
            {

            }
            finally
            {

                valveFLayer = null;
                av = null;
                featSel = null;
                valveEidInfo = null;
            }




        }
        public static void SelectValveJunctionsLayer(ref IMap map, ref Hashtable valveHT, ref FeatureLayer valveFLayer, bool DrawResults)
        {
            IFeatureSelection featSel = null;
            IActiveView av = null;
            IEIDInfo valveEidInfo = null;
            try
            {

                featSel = valveFLayer as IFeatureSelection;
                av = (IActiveView)map;
                if (valveHT.Count == 0) return;
                if (featSel.SelectionSet.Count > 0)
                {

                    featSel.Clear();
                }

                foreach (DictionaryEntry entry in valveHT)
                {
                    valveEidInfo = entry.Value as IEIDInfo;
                    featSel.Add(valveEidInfo.Feature);
                }
                if (DrawResults)
                    av.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            }
            catch { }
            finally
            {
                featSel = null;
                av = null;
                valveEidInfo = null;
            }




        }
        //public static int getValveCount(ref Hashtable valveHT)
        //{
        //    return valveHT.Count;


        //}
        //public static int GetMeterCount(int meterFCNameID, string critFldName, ref IGeometricNetwork gn, ref IEnumNetEID juncEIDs, out int critMeterCount)
        //{
        //    int metCnt = 0;
        //    critMeterCount = 0;
        //    IEIDHelper eidHelper = null;
        //    IEnumEIDInfo enumEidInfo = null;

        //    IEIDInfo eidInfo = null;
        //    try
        //    {
        //        eidHelper = new EIDHelperClass();
        //        eidHelper.GeometricNetwork = gn;
        //        //eidHelper.OutputSpatialReference = map.SpatialReference;

        //        eidHelper.ReturnFeatures = true;
        //        eidHelper.ReturnGeometries = false;
        //        eidHelper.AddField(critFldName);
        //        enumEidInfo = eidHelper.CreateEnumEIDInfo(juncEIDs);


        //        enumEidInfo.Reset();
        //        eidInfo = enumEidInfo.Next();
        //        while (eidInfo != null)
        //        {
        //            if (eidInfo.Feature.Class.ObjectClassID == meterFCNameID)
        //            {

        //                metCnt++;
        //                try
        //                {
        //                    if (eidInfo.Feature.Fields.FindField(critFldName) > 0)
        //                    {
        //                        if (eidInfo.Feature.get_Value(eidInfo.Feature.Fields.FindField(critFldName)) != null)
        //                        {
        //                            int val = 1;

        //                            if (eidInfo.Feature.get_Value(eidInfo.Feature.Fields.FindField(critFldName)).ToString() == val.ToString())
        //                            {
        //                                critMeterCount++;
        //                            }
        //                        }

        //                    }

        //                }
        //                catch
        //                {
        //                }

        //            }
        //            eidInfo = enumEidInfo.Next();
        //        }
        //        return metCnt;

        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //    finally
        //    {
        //        eidHelper = null;
        //        enumEidInfo = null;

        //        eidInfo = null;
        //    }

        //}



        #endregion

        #region URLTools

        public static string FormatLocationRequest(string url, double x, double y, double distance)
        {
            url += "?location=" + x.ToString() + "," + y.ToString() + "&distance=" + distance + "&f=json";
            return url;

        }//Format Location Request
        public static bool IsUrl(string Url)
        {
            return true;

            string strRegex = "^(https?://)"
            + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
            + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
            + "|" // allows either IP or domain
            + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
            + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain
            + "[a-z]{2,6})" // first level domain- .com or .museum
            + "(:[0-9]{1,4})?" // port number- :80
            + "((/?)|" // a slash isn't required if there is no file name
            + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
            Regex re = new Regex(strRegex);

            if (re.IsMatch(Url))
                return (true);
            else
            {
                strRegex = "^(http?://)"
               + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
               + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
               + "|" // allows either IP or domain
               + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
               + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain
               + "[a-z]{2,6})" // first level domain- .com or .museum
               + "(:[0-9]{1,4})?" // port number- :80
               + "((/?)|" // a slash isn't required if there is no file name
               + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
                re = new Regex(strRegex);

                if (re.IsMatch(Url))
                    return (true);
                else
                    return (false);

            }

        } //Is Url


        #endregion

        #region
        public static IWin32Window GetWindowFromHost(int hwnd)
        {
            IWin32Window window = null;
            IntPtr handle = new IntPtr(hwnd);

            try
            {
                NativeWindow nativeWindow = new NativeWindow();
                nativeWindow.AssignHandle(handle);
                window = nativeWindow;
            }
            finally
            {
                handle = IntPtr.Zero;
            }

            return window;
        }
        #endregion



        //public void AddFlagToGN(INetworkAnalysisExt pNetworkAnalysisExt, ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, INetFlag pNetFlag)
        //{
        //    if (pNetworkAnalysisExt.CurrentNetwork != pGeomNet)
        //    {
        //        pNetworkAnalysisExt.CurrentNetwork = pGeomNet;
        //    }
        //    INetworkAnalysisExtFlags pNetworkAnalysisExtFlags;


        //    pNetworkAnalysisExtFlags = (INetworkAnalysisExtFlags)pNetworkAnalysisExt;

        //    IFlagDisplay flagDisplay;
        //    if (pNetFlag is IJunctionFlag)
        //    {
        //        flagDisplay = new JunctionFlagDisplayClass();
        //        //flagDisplay.Geometry = ((IJunctionFlag)pNetFlag)
        //    }
        //    else
        //    {
        //        flagDisplay = new EdgeFlagDisplayClass();
        //    }

        //    //pNetFlag as IFlagDisplay;
        //    flagDisplay.FeatureClassID = pNetFlag.UserClassID;
        //    flagDisplay.FID = pNetFlag.UserID;
        //    flagDisplay.SubID = pNetFlag.UserSubID;

        //    if (pNetFlag is IEdgeFlag)
        //    {

        //        pNetworkAnalysisExtFlags.AddEdgeFlag(flagDisplay as IEdgeFlagDisplay);
        //    }
        //    else
        //    {

        //        pNetworkAnalysisExtFlags.AddJunctionFlag(flagDisplay as IJunctionFlagDisplay);
        //    }

        //}

        //public void LoadJunctions(ref ITraceResult traceRes, ref ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, ref ESRI.ArcGIS.Carto.IMap pMap,
        //                                                            ref ESRI.ArcGIS.Geodatabase.IEnumNetEID pJuncEIDs, ref string MeterDSName)
        //{

        //    IFeature pFeat = null;
        //    IFeatureClass pFC = null;
        //    IDataset pDS = null;
        //    IEIDHelper eidHelper = null;
        //    IEnumEIDInfo enumEidInfo = null;
        //    IEIDInfo eidInfo = null;
        //    try
        //    {
        //        eidHelper = new EIDHelperClass();
        //        eidHelper.GeometricNetwork = pGeomNet;
        //        eidHelper.OutputSpatialReference = pMap.SpatialReference;
        //        eidHelper.ReturnFeatures = true;
        //        // eidHelper.ReturnGeometries = true;
        //        //   eidHelper.PartialComplexEdgeGeometry = true;

        //        enumEidInfo = eidHelper.CreateEnumEIDInfo(pJuncEIDs);


        //        enumEidInfo.Reset();
        //        eidInfo = enumEidInfo.Next();
        //        int i = 0;
        //        while (eidInfo != null)
        //        {
        //            pFeat = eidInfo.Feature;
        //            pFC = (IFeatureClass)pFeat.Class;
        //            pDS = (IDataset)pFC;
        //            string datasetName = pDS.BrowseName;
        //            if (datasetName == "")
        //            {
        //                datasetName = pDS.FullName.NameString;
        //            }
        //            if (datasetName.Contains(MeterDSName))
        //            {
        //                traceRes.add(pFC.FeatureClassID, datasetName, pFeat.ShapeCopy, pFeat.OID, eidInfo.EID, pFeat, true, "Meters", false);

        //            }
        //            else
        //            {
        //                if (valveFCClassIDs.Contains(pFC.FeatureClassID))
        //                {
        //                    traceRes.add(pFC.FeatureClassID, datasetName, pFeat.ShapeCopy, pFeat.OID, eidInfo.EID, pFeat, false, "Non Isolating Valve", false);
        //                }
        //                else if (sourceFCClassIDs.Contains(pFC.FeatureClassID))
        //                {
        //                    traceRes.add(pFC.FeatureClassID, datasetName, pFeat.ShapeCopy, pFeat.OID, eidInfo.EID, pFeat, true, "Source", false);
        //                }
        //                else
        //                {
        //                    traceRes.add(pFC.FeatureClassID, datasetName, pFeat.ShapeCopy, pFeat.OID, eidInfo.EID, pFeat, true, "Other", false);
        //                }
        //            }
        //            eidInfo = enumEidInfo.Next();
        //            i++;
        //        }


        //    }
        //    catch { }
        //    finally
        //    {
        //        if (pFeat != null)
        //        {
        //            Marshal.ReleaseComObject(pFeat);
        //        }
        //        pFeat = null;

        //        if (pFC != null)
        //        {
        //            Marshal.ReleaseComObject(pFC);
        //        }

        //        pFC = null;
        //        if (pDS != null)
        //        {
        //            Marshal.ReleaseComObject(pDS);
        //        }
        //        pDS = null;
        //        if (eidHelper != null)
        //        {
        //            Marshal.ReleaseComObject(eidHelper);
        //        }
        //        eidHelper = null;
        //        if (enumEidInfo != null)
        //        {
        //            Marshal.ReleaseComObject(enumEidInfo);
        //        }
        //        enumEidInfo = null;
        //        if (eidInfo != null)
        //        {
        //            Marshal.ReleaseComObject(eidInfo);
        //        }
        //        eidInfo = null;
        //    }

        //}
        //public void LoadEdges(ref ITraceResult traceRes, ref ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, ref ESRI.ArcGIS.Carto.IMap pMap,
        //                                                           ref ESRI.ArcGIS.Geodatabase.IEnumNetEID pEdgeEIDs)
        //{

        //    IFeature pFeat = null;
        //    IFeatureClass pFC = null;
        //    IDataset pDS = null;
        //    IEIDHelper eidHelper = null;
        //    IEnumEIDInfo enumEidInfo = null;
        //    IEIDInfo eidInfo = null;
        //    try
        //    {
        //        eidHelper = new EIDHelperClass();
        //        eidHelper.GeometricNetwork = pGeomNet;
        //        eidHelper.OutputSpatialReference = pMap.SpatialReference;
        //        eidHelper.ReturnFeatures = true;
        //        eidHelper.ReturnGeometries = true;
        //        eidHelper.PartialComplexEdgeGeometry = true;

        //        enumEidInfo = eidHelper.CreateEnumEIDInfo(pEdgeEIDs);

        //        enumEidInfo.Reset();
        //        eidInfo = enumEidInfo.Next();

        //        int i = 0;
        //        //  IComplexEdgeFeature pComplexEdge = null;
        //        IGeometry pGeo = null;
        //        while (eidInfo != null)
        //        {

        //            pFeat = eidInfo.Feature;


        //            pFC = (IFeatureClass)pFeat.Class;
        //            pDS = (IDataset)pFC;
        //            string datasetName = pDS.BrowseName;
        //            if (datasetName == "")
        //            {
        //                datasetName = pDS.FullName.NameString;
        //            }

        //            if (eidInfo.Geometry == null)
        //            {
        //                pGeo = pFeat.Shape;
        //                traceRes.add(pFC.FeatureClassID, datasetName, pGeo, pFeat.OID, eidInfo.EID, pFeat, true, "Other", false);
        //            }

        //            else
        //            {
        //                pGeo = eidInfo.Geometry;
        //                traceRes.add(pFC.FeatureClassID, datasetName, pGeo, pFeat.OID, eidInfo.EID, pFeat, true, "Complex", true);
        //            }
        //            eidInfo = enumEidInfo.Next();
        //            i++;
        //        }
        //    }
        //    catch { }
        //    finally
        //    {
        //        if (pFeat != null)
        //        {
        //            Marshal.ReleaseComObject(pFeat);
        //        }
        //        pFeat = null;

        //        if (pFC != null)
        //        {
        //            Marshal.ReleaseComObject(pFC);
        //        }

        //        pFC = null;
        //        if (pDS != null)
        //        {
        //            Marshal.ReleaseComObject(pDS);
        //        }
        //        pDS = null;
        //        if (eidHelper != null)
        //        {
        //            Marshal.ReleaseComObject(eidHelper);
        //        }
        //        eidHelper = null;
        //        if (enumEidInfo != null)
        //        {
        //            Marshal.ReleaseComObject(enumEidInfo);
        //        }
        //        enumEidInfo = null;
        //        if (eidInfo != null)
        //        {
        //            Marshal.ReleaseComObject(eidInfo);
        //        }
        //        eidInfo = null;
        //    }
        //}
        //public void LoadValves(ref ITraceResult traceRes, ref ESRI.ArcGIS.Geodatabase.IGeometricNetwork pGeomNet, ref ESRI.ArcGIS.Carto.IMap pMap,
        //                                                            ref Hashtable valvesHT)
        //{
        //    IEIDInfo valveEidInfo = null;
        //    IFeature pFeat = null;
        //    IFeatureClass pFC = null;
        //    IDataset pDS = null;
        //    try
        //    {
        //        if (valvesHT != null)
        //        {
        //            foreach (DictionaryEntry entry in valvesHT)
        //            {
        //                valveEidInfo = entry.Value as IEIDInfo;
        //                pFeat = valveEidInfo.Feature;
        //                pFC = (IFeatureClass)pFeat.Class;
        //                pDS = (IDataset)pFC;
        //                string datasetName = pDS.BrowseName;


        //                traceRes.add(pFC.FeatureClassID, datasetName, pFeat.Shape, pFeat.OID, valveEidInfo.EID, pFeat, true, "Isolating Valve", false);


        //            }
        //        }
        //    }
        //    catch { }
        //    finally
        //    {
        //        if (pFeat != null)
        //        {
        //            Marshal.ReleaseComObject(pFeat);
        //        }
        //        pFeat = null;

        //        if (pFC != null)
        //        {
        //            Marshal.ReleaseComObject(pFC);
        //        }

        //        pFC = null;
        //        if (pDS != null)
        //        {
        //            Marshal.ReleaseComObject(pDS);
        //        }
        //        pDS = null;
        //        if (valveEidInfo != null)
        //        {
        //            Marshal.ReleaseComObject(valveEidInfo);
        //        }
        //        valveEidInfo = null;

        //    }
        //}

    }



}
