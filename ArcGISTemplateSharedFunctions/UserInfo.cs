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
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;

namespace A4LGSharedFunctions
{
  public class CurrentUserInfo
  {
    private string _userName;
    private string _domainName;
    private string _databaseUser;
    private string _fullName;
    private string _testString;

    public CurrentUserInfo(IEditor editor)
    {
      _userName = Environment.UserName;
      _domainName = Environment.UserDomainName;
      _fullName = String.Format(@"{0}\{1}", _domainName, _userName);
      _databaseUser = GetDatabaseUser(editor);
    }

    public string UserName
    {
      get
      {
        return _userName;
      }
    }
    public string DomainName
    {
      get
      {
        return _domainName;
      }
    }
    public string FullName
    {
      get
      {
        return _fullName;
      }
    }
    public string DatabaseUser
    {
      get
      {
        return _databaseUser;
      }
    }

    private string GetDatabaseUser(IEditor editor)
    {
      IDatabaseConnectionInfo databaseConnectionInfo = editor.EditWorkspace as IDatabaseConnectionInfo;
      if (databaseConnectionInfo != null)
        return databaseConnectionInfo.ConnectedUser;
      else
	      return String.Empty;
    }

    //Current user is obtained edit workspace (if RDBMS) or from Windows
    public string GetCurrentUser(string mode, int targetFieldLength)
    {
      _testString = "";

      if (mode == null)
        mode = "";

      if (mode.Length > 1)
        mode = mode.Substring(0, 1).ToUpper();
      else
        mode = mode.ToUpper();

      switch (mode.Trim())
      {
        case "D":
          _testString = _databaseUser;
          break;

        case "U":
          _testString = _userName;
          break;

        case "W":
          _testString = _fullName;
          break;

        default:
          if (_databaseUser.ToUpper() != String.Empty &&
              _databaseUser.ToUpper() != "DBO")
            _testString = _databaseUser;
          else
            _testString = _fullName;
          break;
      }

      // If the user name is longer than the user field allows, shorten it.
      if (targetFieldLength < _testString.Length)
        _testString = _testString.Substring(0, targetFieldLength);

      return _testString;

    }
	}
}
       