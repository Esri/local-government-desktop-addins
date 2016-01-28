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


using System;

using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Win32;

//using MSUtil;
//using LogQuery = Interop.MSUtil.LogQueryClass;
//using RegistryInputFormat = Interop.MSUtil.COMRegistryInputContextClass;
//using RegRecordSet = Interop.MSUtil.ILogRecordset;

namespace A4LGSharedFunctions
{

    public delegate void ReloadEventHandler(object sender, ReloadEventArgs e);
    public class ReloadEventArgs : System.EventArgs
    {
        // Provide one or more constructors, as well as fields and

        // accessors for the arguments.

    }
    public class ReloadMonitor
    {
        public static event ReloadEventHandler reloadConfig;

        public ReloadMonitor()
        {
        }


        public void Reload()
        {
            reloadConfig(this, new ReloadEventArgs());
        }
    }
    public class ConfigEntries
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string FullName { get; set; }
        public bool Loaded { get; set; }
        public string Name { get; set; }
    }
    public class MergeSplitFlds
    {
        public MergeSplitFlds(string fieldName, int fieldIndex, string value, string mergeType, string splitType)
        {
            FieldName = fieldName;
            FieldIndex = fieldIndex;
            Value = value;
            MergeType = mergeType;
            SplitType = splitType;
        }
        public string FieldName { get; set; }
        public int FieldIndex { get; set; }
        public int AvgCount { get; set; }
        public string Value { get; set; }
        public string MergeType { get; set; }
        public string SplitType { get; set; }
    }

    public static class ConfigUtil
    {
        // public delegate void ReloadEventHandler(object sender, EventArgs e);

        public static string fileName = "loaded";
        public static string type = "aa";

        public static string GetHelpFile()
        {
            string helpFile = "";
            string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (AppPath.IndexOf("file:\\") >= 0)
                AppPath = AppPath.Replace("file:\\", "");
            string fullAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string assemblyName = fullAssemblyName.Split('.').GetValue(fullAssemblyName.Split('.').Length - 1).ToString();

            if (File.Exists(Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), "help\\" + assemblyName + ".hlp")))
                helpFile = Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), "help\\" + assemblyName + ".hlp");
            else if (File.Exists(Path.Combine(AppPath, "help\\" + assemblyName + ".hlp")))
                helpFile = Path.Combine(AppPath, "help\\" + assemblyName + ".hlp");
            return helpFile;

        }
        //private static string findConfigFile()
        //{
        //    MSUtil.ILogRecordset rs = null;
        //    try
        //    {
        //        MSUtil.LogQueryClass qry = new MSUtil.LogQueryClass();
        //        MSUtil.COMRegistryInputContextClass registryFormat = new MSUtil.COMRegistryInputContextClass();
        //        string query = @"SELECT Path from \HKCR where Value='A4WaterUtilities.AddLateralsConstructTool'";

        //        rs = qry.Execute(query, registryFormat);
        //        for (; !rs.atEnd(); rs.moveNext())
        //            Debug.WriteLine(rs.getRecord().toNativeString(","));

        //        query = @"SELECT Path from \HKCR\TypeLib where Value='{523367B6-27D1-4554-AC78-95A610A54D23}'";
        //        rs = qry.Execute(query, registryFormat);
        //        for (; !rs.atEnd(); rs.moveNext())
        //            Debug.WriteLine(rs.getRecord().toNativeString(","));
        //    }
        //    catch (Exception ex) 
        //    {

        //    }
        //    finally
        //    {
        //        rs.close();
        //    }

        //    return "";

        //}
        public static string generateUserCachePath()
        {

            try
            {
                string pathToUserProf;



                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", null) != null)
                {
                    pathToUserProf = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", null) as string;

                }
                else if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", null) != null)
                {
                    pathToUserProf = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\ArcGISSolutions\DesktopTools", "ConfigLocation", null) as string;
                }

                else
                {
                    pathToUserProf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGISSolutions\\ConfigFiles");
                }
                if (System.IO.Directory.Exists(pathToUserProf) == false)
                {
                    System.IO.Directory.CreateDirectory(pathToUserProf);
                }

                return pathToUserProf;

            }
            catch (Exception ex)
            {
                MessageBox.Show("generateUserCachePath:  " + ex.Message);
                return "";
            }


        }
        public static List<string> GetAllConfigFiles(bool includeLoaded, string type)
        {
            try
            {

                string pathToUserProf = generateUserCachePath();

                string configFileName = fileName + "." + type + ".config";
                List<string> pPrevConfFiles = new List<string>(Directory.GetFiles(pathToUserProf, "loaded.config", System.IO.SearchOption.AllDirectories));

                List<string> pConfFiles = new List<string>(Directory.GetFiles(pathToUserProf, "*." + type + ".*onfig*", System.IO.SearchOption.AllDirectories));
                if (pConfFiles.Count == 0)
                {
                    if (pPrevConfFiles.Count > 0 && type != "gas")
                    {
                        getInstalledConfig(pathToUserProf, true);
                        File.Copy(pPrevConfFiles[0], Path.Combine(pathToUserProf, configFileName));               
                        pConfFiles.Add(Path.Combine(pathToUserProf, configFileName));
                    }
                    else
                    {
                        pConfFiles.Add(getInstalledConfig(generateUserCachePath(), false));
                    }
                }
                else if (File.Exists(Path.Combine(pathToUserProf, configFileName)) == false)
                {

                    if (pPrevConfFiles.Count > 0 && type != "gas")
                    {
                        getInstalledConfig(pathToUserProf, true);
                        File.Copy(pPrevConfFiles[0], Path.Combine(pathToUserProf, configFileName));
                        pConfFiles.Add(Path.Combine(pathToUserProf, configFileName));
                    }
                    else
                    {
                        pConfFiles.Add(getInstalledConfig(generateUserCachePath(), false));
                    }
                }
                if (includeLoaded == false)
                    pConfFiles.Remove(Path.Combine(pathToUserProf, configFileName));
                return pConfFiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetAllConfigFiles:  " + ex.Message);
                return null;
            }


        }
        public static List<ConfigEntries> GetAllConfigFilesNames(bool includeLoaded)
        {


            List<string> pConfFiles = null;
            List<ConfigEntries> pConfFileNames = null;
            XmlDocument oXml = null;
            XmlNodeList oList = null;
            ConfigEntries confEn = null;
            XmlNode oNode = null;
            try
            {
                string configFileName = fileName + "." + type + ".config";

                pConfFiles = GetAllConfigFiles(true, type);
                pConfFileNames = new List<ConfigEntries>();

                for (int i = 0; i < pConfFiles.Count; i++)
                {
                    oXml = new XmlDocument();
                    oXml.Load(pConfFiles[i]);

                    // XmlNode pXMLNode = oXml.FirstChild;

                    confEn = new ConfigEntries();
                    confEn.FullName = pConfFiles[i];
                    confEn.Path = Path.GetDirectoryName(pConfFiles[i]);
                    confEn.FileName = Path.GetFileName(pConfFiles[i]);
                    if (confEn.FileName.ToUpper() == configFileName.ToUpper())
                        confEn.Loaded = true;
                    else
                        confEn.Loaded = false;
                    oList = oXml.GetElementsByTagName("Name");
                    if (oList == null)
                    {
                        confEn.Name = "";
                    }
                    else if (oList.Count == 0)
                    {
                        confEn.Name = confEn.FileName;
                    }
                    else
                    {
                        oNode = oList.Item(0);
                        confEn.Name = oNode.InnerText;
                        oNode = null;
                    }
                    if (!(includeLoaded == false && confEn.Loaded == true))
                    {
                        pConfFileNames.Add(confEn);
                    }
                    oXml = null;

                }



                return pConfFileNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetAllConfigFilesNames:  " + ex.Message);

                return null;
            }
            finally
            {

                pConfFiles = null;
                pConfFileNames = null;
                oXml = null;

                oList = null;
                confEn = null;
                oNode = null;
            }
        }
        public static bool copyFileContents(string SourceFile, string TargetFile)
        {
            try
            {
                using (StreamReader sr = new StreamReader(SourceFile))
                {
                    String line = sr.ReadToEnd();



                    if (File.Exists(TargetFile))
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(TargetFile))
                        {
                            sw.Write(line);
                        }

                    }
                    else
                    {
                        string dir = Path.GetDirectoryName(TargetFile);
                        if (Directory.Exists(dir) == false)
                            Directory.CreateDirectory(dir);
                        //Directory.CreateDirectory()
                        using (StreamWriter sw = File.CreateText(TargetFile))
                        {
                            sw.Write(line);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ChangeConfig(ConfigEntries LoadedConfig, ConfigEntries ConfigToLoad)
        {
            try
            {
                string SourceFile = ConfigToLoad.Path + "\\" + LoadedConfig.FileName;
                string SourceCopyFile = ConfigToLoad.Path + "\\" + LoadedConfig.Name + "." + type + ".config";
                string TargetFile = LoadedConfig.Path + "\\" + ConfigToLoad.FileName;

                if (copyFileContents(SourceFile, SourceCopyFile))
                    if (copyFileContents(TargetFile, SourceFile))
                    {
                        //LoadedConfig.Name = ConfigToLoad.Name;
                        return true;
                    }
                    else
                        return false;

                else
                    return false;

                //File.Copy(LoadedConfig.FullName, LoadedConfig.Path + "\\" + LoadedConfig.Name + ".config", true);

                //File.Copy(ConfigToLoad.FullName, ConfigToLoad.Path + "\\" + "Config" + ".config", true);
                // NotifyBase pNot = new NotifyBase();
                // invokeEvent();


                //return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ChangeConfig:  " + ex.Message);
                return false;

            }
            finally
            {
                //LoadedConfig = null;
                //ConfigToLoad = null;
            }
        }
        public static string GetConfigFile()
        {
            try
            {


                string pathToUserProf = generateUserCachePath();
                string configFileName = fileName + "." + type + ".config";



                List<string> pPrevConfFiles = new List<string>(Directory.GetFiles(pathToUserProf, "loaded.config", System.IO.SearchOption.AllDirectories));

                if (pathToUserProf != "")
                {
                    if (File.Exists(Path.Combine(pathToUserProf, configFileName)))
                    {
                        return Path.Combine(pathToUserProf, configFileName);


                    }
                    else
                        if (pPrevConfFiles.Count > 0 && type != "gas")
                        {
                            getInstalledConfig(pathToUserProf, true);
                            File.Copy(pPrevConfFiles[0], Path.Combine(pathToUserProf, configFileName));
                            
                            return Path.Combine(pathToUserProf, configFileName);
                        }
                        else
                        {
                            return getInstalledConfig(pathToUserProf,false);
                        }
                }
                else
                    return "";



            }
            catch (Exception ex)
            {
                MessageBox.Show("GetConfigFile:  " + ex.Message);
                return "";
            }

        }
        public static string getInstalledConfig(string pathToUserProf, bool saveAsBackup)
        {
            try
            {
                string configFileName = fileName + "." + type + ".config";
                string sourceFileNaame = fileName + "." + type + ".config";
                string pConfigFiles = "";
                if (saveAsBackup && type != "gas")
                {
                    configFileName = "shipped" + "." + type + ".config";
                }
                string AppPath;
                AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);



                if (AppPath.IndexOf("file:\\") >= 0)
                    AppPath = AppPath.Replace("file:\\", "");



                if (File.Exists(Path.Combine(AppPath, sourceFileNaame)))
                {
                    pConfigFiles = Path.Combine(AppPath, sourceFileNaame);
                    if (File.Exists(Path.Combine(pathToUserProf, configFileName)) == false)
                    {
                        copyFileContents(pConfigFiles, Path.Combine(pathToUserProf, configFileName));

                        //   System.IO.File.Copy(pConfigFiles, Path.Combine(pathToUserProf, configFileName));
                    }
                    pConfigFiles = Path.Combine(pathToUserProf, configFileName);
                }
                else if (File.Exists(Path.Combine(AppPath, "Config.config")))
                {
                    pConfigFiles = Path.Combine(AppPath, "Config.config");
                    if (File.Exists(Path.Combine(pathToUserProf, configFileName)) == false)
                    {
                        copyFileContents(pConfigFiles, Path.Combine(pathToUserProf, configFileName));

                        //   System.IO.File.Copy(pConfigFiles, Path.Combine(pathToUserProf, configFileName));
                    }
                    pConfigFiles = Path.Combine(pathToUserProf, configFileName);
                }
                return pConfigFiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getInstalledConfig:  " + ex.Message);
                return "";
            }
        }

        public static double GetConfigValue(string keyname, double defaultValue)
        {
            XmlDocument oXml = null;
            XmlNodeList oList = null;
            try
            {
                string pConfigFiles = GetConfigFile();
                string keyvalue = "";



                if (File.Exists(pConfigFiles))
                {
                    oXml = new XmlDocument();
                    oXml.Load(pConfigFiles);

                    oList = oXml.GetElementsByTagName("appSettings");
                    if (oList == null) return defaultValue;

                    foreach (XmlNode oNode in oList)
                    {
                        foreach (XmlNode oKey in oNode.ChildNodes)
                        {
                            if ((oKey != null) && (oKey.Attributes != null))
                            {
                                if (oKey.Attributes["key"].Value.Equals(keyname))
                                {
                                    if (oKey.Attributes["value"].Value.Trim().Length > 0)
                                    {
                                        keyvalue = oKey.Attributes["value"].Value;

                                        //Try to convert to double
                                        if (Globals.IsDouble(keyvalue))
                                        {
                                            return (Convert.ToDouble(keyvalue));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    oXml = null;

                }


                return defaultValue;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                return defaultValue;
            }
            finally
            {
                oXml = null;
                oList = null;
            }
        }
        public static int GetConfigValue(string keyname, int defaultValue)
        {
            XmlDocument oXml = null;
            XmlNodeList oList = null;
            try
            {
                string pConfigFiles = GetConfigFile();
                string keyvalue = "";

                if (File.Exists(pConfigFiles))
                {
                    //NameValueCollection AppSettings = new NameValueCollection();
                    oXml = new XmlDocument();

                    oXml.Load(pConfigFiles);

                    oList = oXml.GetElementsByTagName("appSettings");
                    if (oList == null) return defaultValue;

                    //AppSettings = new NameValueCollection();
                    foreach (XmlNode oNode in oList)
                    {
                        foreach (XmlNode oKey in oNode.ChildNodes)
                        {
                            if ((oKey != null) && (oKey.Attributes != null))
                            {
                                if (oKey.Attributes["key"].Value.Equals(keyname))
                                {
                                    if (oKey.Attributes["value"].Value.Trim().Length > 0)
                                    {
                                        keyvalue = oKey.Attributes["value"].Value;

                                        //Try to convert to integer32
                                        if (Globals.IsInteger(keyvalue))
                                        {
                                            return (Convert.ToInt32(keyvalue));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    oXml = null;

                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                return defaultValue;
            }
            finally
            {
                oXml = null;
                oList = null;
            }
        }
        public static bool GetConfigValue(string keyname, bool defaultValue)
        {
            XmlDocument oXml = null;
            XmlNodeList oList = null;
            try
            {
                string pConfigFile = GetConfigFile();
                string keyvalue = "";
                if (File.Exists(pConfigFile))
                {
                    //NameValueCollection AppSettings = new NameValueCollection();
                    oXml = new XmlDocument();

                    oXml.Load(pConfigFile);

                    oList = oXml.GetElementsByTagName("appSettings");
                    if (oList == null) return defaultValue;

                    //AppSettings = new NameValueCollection();
                    foreach (XmlNode oNode in oList)
                    {
                        foreach (XmlNode oKey in oNode.ChildNodes)
                        {
                            if ((oKey != null) && (oKey.Attributes != null))
                            {
                                if (oKey.Attributes["key"].Value.Equals(keyname))
                                {
                                    if (oKey.Attributes["value"].Value.Trim().Length > 0)
                                    {
                                        keyvalue = oKey.Attributes["value"].Value;

                                        //Try to convert to integer32
                                        if (Globals.IsBoolean(keyvalue))
                                        {
                                            return (Convert.ToBoolean(keyvalue));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    oXml = null;
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                return defaultValue;
            }
            finally
            {
                oXml = null;
                oList = null;
            }
        }
        public static bool compareConfigValue(string keyname, string value)
        {
            try
            {
                if (string.Compare(GetConfigValue(keyname), value) == 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("compareConfigValue:  " + ex.Message);
                return false;
            }
        }
        public static string GetConfigValue(string keyname)
        {
            XmlDocument oXml = null;
            XmlNodeList oList = null;
            try
            {
                string pConfigFile = GetConfigFile();
                string keyvalue = "";

                if (File.Exists(pConfigFile))
                {
                    //NameValueCollection AppSettings = new NameValueCollection();
                    oXml = new XmlDocument();

                    oXml.Load(pConfigFile);

                    oList = oXml.GetElementsByTagName("appSettings");
                    if (oList == null) return "";

                    //AppSettings = new NameValueCollection();
                    foreach (XmlNode oNode in oList)
                    {
                        foreach (XmlNode oKey in oNode.ChildNodes)
                        {
                            if ((oKey != null) && (oKey.Attributes != null))
                            {
                                if (oKey.Attributes["key"].Value.Equals(keyname))
                                {
                                    if (oKey.Attributes["value"].Value.Trim().Length > 0)
                                    {
                                        keyvalue = oKey.Attributes["value"].Value;

                                        //Try to convert to integer32
                                        return keyvalue;
                                    }
                                }
                            }
                        }
                    }
                    oXml = null;
                }


                return "";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                return "";
            }
            finally
            {
                oXml = null;
                oList = null;
            }
        }
        public static string GetConfigValue(string keyname, string defaultValue)
        {

            try
            {
                string strConfigVal = GetConfigValue(keyname);
                if (strConfigVal == "")
                    return defaultValue;
                else
                    return strConfigVal;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("GetConfigValue:" + ex.Message);
                return defaultValue;
            }
        }
        private static bool KeyExists(XmlDocument xmlDoc, string strKey)
        {
            XmlNode appSettingsNode = null;
            try
            {
                appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

                // Attempt to locate the requested setting.
                foreach (XmlNode childNode in appSettingsNode)
                {
                    if (childNode != null)
                    {
                        if (childNode.NodeType == XmlNodeType.Element)
                        {
                            if (childNode.Attributes.Count > 0)
                            {
                                if (childNode.Attributes["key"] != null)
                                {
                                    if (childNode.Attributes["key"].Value != null)
                                    {
                                        if (childNode.Attributes["key"].Value == strKey)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("KeyExists:  " + ex.Message);
                return false;
            }
            finally
            {
                appSettingsNode = null;
            }
        }
        private static XmlDocument getConfigAsXMLDoc()
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
                    string confFiles = GetConfigFile();
                    if (confFiles != null)
                    {
                        if (confFiles.Trim() != "")
                        {
                            // xmld.Load(Globals.getFileAtDLLLocation("ESRI.WaterUtilitiesTemplate.DesktopFunctions.config"));
                            xmld.Load(confFiles);
                            return xmld;

                        }


                    }
                    return null;

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compromised by a change you made.");
                    return null;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("getConfigAsXMLDoc:  " + ex.Message);
                return null;

            }
            finally
            {



            }
        }


        //public static List<MergeSplitGeoNetFeatures> GetMergeSplitConfig()

        //{


        //    XmlDocument xmld = getConfigAsXMLDoc();
        //    if (xmld == null) return null;
        //    XmlNodeList nodelist = null;
        //    XmlNode node = null;
        //    List<MergeSplitGeoNetFeatures> pEntries = null;
        //    MergeSplitGeoNetFeatures pSingleEntries = null;
        //    try
        //    {



        //        //Get the list of name nodes 
        //        //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
        //        nodelist = xmld.SelectNodes("configuration/MergeSplitGeoNetFeatures");
        //        if (nodelist == null) { return null; }
        //        //Loop through the nodes 
        //        pEntries = new List<MergeSplitGeoNetFeatures>();


        //        for (int i = 0; i < nodelist.Count; i++)
        //        {
        //            node = nodelist.Item(i);
        //            pSingleEntries = ((MergeSplitGeoNetFeatures)Globals.DeserializeObject(node, typeof(MergeSplitGeoNetFeatures)));
        //            if (pSingleEntries != null)

        //                pEntries.Add(pSingleEntries);
        //        }


        //        return pEntries;
        //    }
        //    catch //(Exception ex)
        //    {


        //        return null;

        //    }
        //    finally
        //    {

        //        xmld = null;
        //        nodelist = null;
        //        node = null;


        //        pSingleEntries = null;
        //    }
        //}

        public static List<MergeSplitGeoNetFeatures> GetMergeSplitConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = null;
            XmlNode node = null;
            List<MergeSplitGeoNetFeatures> pEntries = null;
            MergeSplitGeoNetFeatures pSingleEntries = null;
            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/MergeSplitGeoNetFeatures");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<MergeSplitGeoNetFeatures>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);
                    pSingleEntries = ((MergeSplitGeoNetFeatures)Globals.DeserializeObject(node, typeof(MergeSplitGeoNetFeatures)));
                    if (pSingleEntries != null)

                        pEntries.Add(pSingleEntries);
                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;


                pSingleEntries = null;
            }
        }

        public static LayerViewerConfig GetLayerViewerConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            //XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);

            try
            {

                node = xmld.SelectSingleNode("configuration/LayerViewerConfig");
                if (node == null) { return null; }

                LayerViewerConfig pSingleEntries;
                pSingleEntries = (LayerViewerConfig)Globals.DeserializeObject(node, typeof(LayerViewerConfig));



                return pSingleEntries;
            }
            catch //(Exception ex)
            {

                return null;
            }
            finally
            {
                xmld = null;
                //  nodelist = null;
                node = null;

            }
        }
        public static List<CreatePointWithReferenceDetails> GetCreatePointWithRefConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = null;
            XmlNode node = null;
            List<CreatePointWithReferenceDetails> pEntries = null;
            CreatePointWithReferenceDetails pSingleEntries = null;
            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");

                nodelist = xmld.SelectNodes("configuration/AddressManagement/CreatePointWithReference/CreatePointWithReferenceDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<CreatePointWithReferenceDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);
                    pSingleEntries = ((CreatePointWithReferenceDetails)Globals.DeserializeObject(node, typeof(CreatePointWithReferenceDetails)));
                    if (pSingleEntries != null)

                        pEntries.Add(pSingleEntries);
                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;


                pSingleEntries = null;
            }
        }
        public static List<ConstructLineWithPointsDetails> GetLinePointAtEndsConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = null;
            XmlNode node = null;
            List<ConstructLineWithPointsDetails> pEntries = null;
            ConstructLineWithPointsDetails pSingleEntries = null;
            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/ConstructLineWithPoints/ConstructLineWithPointsDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<ConstructLineWithPointsDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);
                    pSingleEntries = ((ConstructLineWithPointsDetails)Globals.DeserializeObject(node, typeof(ConstructLineWithPointsDetails)));
                    if (pSingleEntries != null)

                        pEntries.Add(pSingleEntries);
                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;


                pSingleEntries = null;
            }
        }

        public static List<AttributeTransferDetails> GetAttributeTransferConfig()
        {

            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<AttributeTransferDetails> pEntries = null;
            AttributeTransferDetails pSingleEntries = null;
            try
            {
                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/AttributeTransfer/AttributeTransferDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<AttributeTransferDetails>();
                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);


                    pSingleEntries = (AttributeTransferDetails)Globals.DeserializeObject(node, typeof(AttributeTransferDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);
                }
                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<AddLateralDetails> GetAddLateralsConfig()
        {



            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<AddLateralDetails> pEntries = null;
            AddLateralDetails pSingleEntries = null;
            try
            {


                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/AddLateralsLayers/AddLateralDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<AddLateralDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);




                    // Globals.InitDefaults((Object)pSingleEntries);

                    pSingleEntries = (AddLateralDetails)Globals.DeserializeObject(node, typeof(AddLateralDetails));
                    if (pSingleEntries != null)
                    {
                        if (pSingleEntries.Hook_Angle == 0)
                            pSingleEntries.Hook_Angle = 45;
                        pEntries.Add(pSingleEntries);
                    }

                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<AddLateralFromMainPointDetails> GetAddLateralsFromMainConfig()
        {



            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<AddLateralFromMainPointDetails> pEntries = null;
            AddLateralFromMainPointDetails pSingleEntries = null;
            try
            {


                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/AddLateralFromMainPointLayers/AddLateralFromMainPointDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<AddLateralFromMainPointDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);




                    // Globals.InitDefaults((Object)pSingleEntries);

                    pSingleEntries = (AddLateralFromMainPointDetails)Globals.DeserializeObject(node, typeof(AddLateralFromMainPointDetails));
                    if (pSingleEntries != null)
                    {

                        pEntries.Add(pSingleEntries);
                    }

                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<TapPointDetails> GetCreateTapPointsOnMainConfig()
        {



            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<TapPointDetails> pEntries = null;
            TapPointDetails pSingleEntries = null;
            try
            {


                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/CreateTapPointsOnMain/CreateTapPointsOnMainDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<TapPointDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);




                    // Globals.InitDefaults((Object)pSingleEntries);

                    pSingleEntries = (TapPointDetails)Globals.DeserializeObject(node, typeof(TapPointDetails));
                    if (pSingleEntries != null)
                    {

                        pEntries.Add(pSingleEntries);
                    }

                }


                return pEntries;
            }
            catch //(Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<ConnectClosestDetails> GetConnectClosestConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);

            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/ConnectClosest/ConnectClosestDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                List<ConnectClosestDetails> pEntries = new List<ConnectClosestDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);



                    ConnectClosestDetails pSingleEntries;
                    pSingleEntries = (ConnectClosestDetails)Globals.DeserializeObject(node, typeof(ConnectClosestDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);

                }


                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;

            }
        }
        public static List<ProfileGraphDetails> GetProfileGraphConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<ProfileGraphDetails> pEntries = null;
            ProfileGraphDetails pSingleEntries = null;

            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/ProfileGraph/ProfileGraphDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<ProfileGraphDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);



                    pSingleEntries = (ProfileGraphDetails)Globals.DeserializeObject(node.OuterXml.ToString(), typeof(ProfileGraphDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);

                }


                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<MoveConnectionsDetails> GetMoveConnectionsConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<MoveConnectionsDetails> pEntries = null;
            MoveConnectionsDetails pSingleEntries = null;

            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/MoveConnections/MoveConnectionsDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<MoveConnectionsDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);



                    pSingleEntries = (MoveConnectionsDetails)Globals.DeserializeObject(node, typeof(MoveConnectionsDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);

                }


                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<AddressCenterlineDetails> GetAddressCenterlineConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();

            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<AddressCenterlineDetails> pEntries = null;
            AddressCenterlineDetails pSingleEntries = null;

            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/AddressManagement/AddressCenterlineDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<AddressCenterlineDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);



                    pSingleEntries = (AddressCenterlineDetails)Globals.DeserializeObject(node, typeof(AddressCenterlineDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);

                }


                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }
        public static List<FlowLayerDetails> GetFlowAccumConfig()
        {


            XmlDocument xmld = getConfigAsXMLDoc();
            if (xmld == null) return null;
            XmlNodeList nodelist = default(XmlNodeList);
            XmlNode node = default(XmlNode);
            List<FlowLayerDetails> pEntries = null;
            FlowLayerDetails pSingleEntries = null;

            try
            {



                //Get the list of name nodes 
                //nodelist = xmld.SelectNodes("LayerViewerConfig/Layers/Layer");
                nodelist = xmld.SelectNodes("configuration/FlowAccumulation/FlowLayerDetails");
                if (nodelist == null) { return null; }
                //Loop through the nodes 
                pEntries = new List<FlowLayerDetails>();


                for (int i = 0; i < nodelist.Count; i++)
                {
                    node = nodelist.Item(i);



                    pSingleEntries = (FlowLayerDetails)Globals.DeserializeObject(node, typeof(FlowLayerDetails));
                    if (pSingleEntries != null)
                        pEntries.Add(pSingleEntries);

                }


                return pEntries;
            }
            catch// (Exception ex)
            {


                return null;

            }
            finally
            {

                xmld = null;
                nodelist = null;
                node = null;
                pSingleEntries = null;
            }
        }

    }
}

