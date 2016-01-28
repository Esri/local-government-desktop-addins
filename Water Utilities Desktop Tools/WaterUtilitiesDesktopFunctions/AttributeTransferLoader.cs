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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.SystemUI;
using System.Collections;
using A4LGSharedFunctions;

namespace A4WaterUtilities
{
    public static class attributeTransferLoader
    {
       
        public static void LoadAttributeTransfer(IApplication app,List<AttributeTransferDetails> attTransferDetails)
        {
            IEditor editor = null;
            UID pUID = null;
            IAttributeTransfer pAttTrans = null;
            IAttributeTransferType pAttTransType = null;
            IFieldMap pFieldMap = null;
            AttributeTransferDetails attConfig = null;
            IFeatureLayer pSfl = null;
            IFeatureLayer pTfl = null;
            IField pSourceField = null;
            IField pTargetField = null;

            ICommandItem pCmdItem = null;
            IAttributeTransferDefaultSettings pATDS = null;

            try
            {
                editor = Globals.getEditor(app);

                if (editor.EditState == esriEditState.esriStateNotEditing)
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("MustBEditg") + " " + A4LGSharedFunctions.Localizer.GetString("StrtEditing"));
                    return;
                }

                if (attTransferDetails == null)
                    return;
                if (attTransferDetails.Count == 0)
                    return;


                pAttTransType = (IAttributeTransferType)editor;
                pAttTrans = pAttTransType.AttributeTransfer;





                for (int j = 0; j < attTransferDetails.Count; j++)
                {
                    attConfig = attTransferDetails[j];

                    string aSourceName = attConfig.SourceLayerName;
                    string aTargetName = attConfig.TargetLayerName;
                    bool FCorLayerSource = true;
                    bool FCorLayerTarget = true;
                    pSfl = Globals.FindLayer(app, aSourceName, ref FCorLayerSource) as IFeatureLayer;
                    pTfl = Globals.FindLayer(app, aTargetName, ref FCorLayerTarget) as IFeatureLayer;
                    if (pSfl == null || pTfl == null)
                    {
                        //MessageBox.Show("The source or target layer is not present");
                        //return;
                    }
                    else
                    {
                        pFieldMap = pAttTrans.FindFieldMap(pSfl.FeatureClass, pTfl.FeatureClass);
                        if (pFieldMap != null)
                        {

                            pAttTrans.DeleteFieldMap(pFieldMap);

                        }
                        pFieldMap = new FieldMapClass();

                        pFieldMap.SourceClass = pSfl.FeatureClass;
                        pFieldMap.TargetClass = pTfl.FeatureClass;



                        for (int i = 0; i < attConfig.FromToFields.Length; i++)
                        {
                            if (attConfig.FromToFields[i] == null || attConfig.FromToFields[i] == null)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttrTrasftLoadRError_1"));
                                return;

                            }
                            if (attConfig.FromToFields[i].SourceField == null || attConfig.FromToFields[i].SourceField == null)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttrTrasftLoadRError_1"));
                                return;

                            }
                            pSourceField = Globals.GetField(pSfl.FeatureClass.Fields, attConfig.FromToFields[i].SourceField);
                            pTargetField = Globals.GetField(pTfl.FeatureClass.Fields, attConfig.FromToFields[i].TargetField);
                            if (pSourceField != null && pTargetField != null)
                                pFieldMap.SetFieldMap(pSourceField, pTargetField);



                        }


                        pAttTrans.FieldMap = pFieldMap;

                        pATDS = (IAttributeTransferDefaultSettings)pAttTrans;

                        pATDS.SourceName = aSourceName;
                        pATDS.TargetName = aTargetName;
                    }
                }


                pUID = new UIDClass();
                pUID.Value = "esriEditorExt.FieldMappingCommand";


                app.CurrentTool = null;


                pCmdItem = app.Document.CommandBars.Find(pUID);
                pCmdItem.Execute();

            }
            catch(Exception ex)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("ErrorInThe") + "LoadAttributeTransfer: " + ex.Message);
            }
            finally
            {

                editor = null;
                pUID = null;
                pAttTrans = null;
                pAttTransType = null;
                pFieldMap = null;
                attConfig = null;
                pSfl = null;
                pTfl = null;
                pSourceField = null;
                pTargetField = null;

                pCmdItem = null;
                pATDS = null;
            }



        }
    
    }
}
