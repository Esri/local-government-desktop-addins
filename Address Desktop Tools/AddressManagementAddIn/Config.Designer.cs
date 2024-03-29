//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace A4LGAddressManagement {
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.esriSystem;
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.Desktop.AddIns;
    
    
    /// <summary>
    /// A class for looking up declarative information in the associated configuration xml file (.esriaddinx).
    /// </summary>
    internal static class ThisAddIn {
        
        internal static string Name {
            get {
                return "Address Management Tools";
            }
        }
        
        internal static string AddInID {
            get {
                return "{00d1d3d9-b338-4498-9585-00d2e81e1e0e}";
            }
        }
        
        internal static string Company {
            get {
                return "Esri., Inc.";
            }
        }
        
        internal static string Version {
            get {
                return "2021.6.18";
            }
        }
        
        internal static string Description {
            get {
                return "ArcMap Address Management Toolset";
            }
        }
        
        internal static string Author {
            get {
                return "Esri., Inc.";
            }
        }
        
        internal static string Date {
            get {
                return "6/18/2021";
            }
        }
        
        internal static ESRI.ArcGIS.esriSystem.UID ToUID(this System.String id) {
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = id;
            return uid;
        }
        
        /// <summary>
        /// A class for looking up Add-in id strings declared in the associated configuration xml file (.esriaddinx).
        /// </summary>
        internal class IDs {
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_AddressFlipLines', the id declared for Add-in Button class 'AddressFlipLines'
            /// </summary>
            internal static string AddressFlipLines {
                get {
                    return "A4LGAddressManagement_AddressFlipLines";
                }
            }
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_AddressFlipLinesNoAddress', the id declared for Add-in Button class 'AddressFlipLinesNoAddress'
            /// </summary>
            internal static string AddressFlipLinesNoAddress {
                get {
                    return "A4LGAddressManagement_AddressFlipLinesNoAddress";
                }
            }
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_CreateIntersectionPoints', the id declared for Add-in Button class 'AddressCreateIntersectionPoints'
            /// </summary>
            internal static string AddressCreateIntersectionPoints {
                get {
                    return "A4LGAddressManagement_CreateIntersectionPoints";
                }
            }
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_ShowConfigForm', the id declared for Add-in Button class 'ShowConfigForm'
            /// </summary>
            internal static string ShowConfigForm {
                get {
                    return "A4LGAddressManagement_ShowConfigForm";
                }
            }
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_CreateLineAndSplitIntersectingLines', the id declared for Add-in Tool class 'CreateLineAndSplitIntersectingLines'
            /// </summary>
            internal static string CreateLineAndSplitIntersectingLines {
                get {
                    return "A4LGAddressManagement_CreateLineAndSplitIntersectingLines";
                }
            }
            
            /// <summary>
            /// Returns 'A4LGAddressManagement_CreatePointAndRefPoint', the id declared for Add-in Tool class 'CreatePointAndRefPoint'
            /// </summary>
            internal static string CreatePointAndRefPoint {
                get {
                    return "A4LGAddressManagement_CreatePointAndRefPoint";
                }
            }
        }
    }
    
internal static class ArcMap
{
  private static IApplication s_app = null;
  private static IDocumentEvents_Event s_docEvent;

  public static IApplication Application
  {
    get
    {
      if (s_app == null)
      {
        s_app = Internal.AddInStartupObject.GetHook<IMxApplication>() as IApplication;
        if (s_app == null)
        {
          IEditor editorHost = Internal.AddInStartupObject.GetHook<IEditor>();
          if (editorHost != null)
            s_app = editorHost.Parent;
        }
      }
      return s_app;
    }
  }

  public static IMxDocument Document
  {
    get
    {
      if (Application != null)
        return Application.Document as IMxDocument;

      return null;
    }
  }
  public static IMxApplication ThisApplication
  {
    get { return Application as IMxApplication; }
  }
  public static IDockableWindowManager DockableWindowManager
  {
    get { return Application as IDockableWindowManager; }
  }
  public static IDocumentEvents_Event Events
  {
    get
    {
      s_docEvent = Document as IDocumentEvents_Event;
      return s_docEvent;
    }
  }
  public static IEditor Editor
  {
    get
    {
      UID editorUID = new UID();
      editorUID.Value = "esriEditor.Editor";
      return Application.FindExtensionByCLSID(editorUID) as IEditor;
    }
  }
}

namespace Internal
{
  [StartupObjectAttribute()]
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public sealed partial class AddInStartupObject : AddInEntryPoint
  {
    private static AddInStartupObject _sAddInHostManager;
    private List<object> m_addinHooks = null;

    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    public AddInStartupObject()
    {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool Initialize(object hook)
    {
      bool createSingleton = _sAddInHostManager == null;
      if (createSingleton)
      {
        _sAddInHostManager = this;
        m_addinHooks = new List<object>();
        m_addinHooks.Add(hook);
      }
      else if (!_sAddInHostManager.m_addinHooks.Contains(hook))
        _sAddInHostManager.m_addinHooks.Add(hook);

      return createSingleton;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void Shutdown()
    {
      _sAddInHostManager = null;
      m_addinHooks = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal static T GetHook<T>() where T : class
    {
      if (_sAddInHostManager != null)
      {
        foreach (object o in _sAddInHostManager.m_addinHooks)
        {
          if (o is T)
            return o as T;
        }
      }

      return null;
    }

    // Expose this instance of Add-in class externally
    public static AddInStartupObject GetThis()
    {
      return _sAddInHostManager;
    }
  }
}
}
