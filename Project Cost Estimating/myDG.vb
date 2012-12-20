
' | Version 10.1.1
' | Copyright 2012 Esri
' |
' | Licensed under the Apache License, Version 2.0 (the "License");
' | you may not use this file except in compliance with the License.
' | You may obtain a copy of the License at
' |
' |    http://www.apache.org/licenses/LICENSE-2.0
' |
' | Unless required by applicable law or agreed to in writing, software
' | distributed under the License is distributed on an "AS IS" BASIS,
' | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' | See the License for the specific language governing permissions and
' | limitations under the License.


Imports System.Windows.Forms

Public Class myDG
    Inherits DataGridView
    Public Event DataGridKeyIntercept(ByVal Key As Integer)

    Protected Overrides Function ProcessKeyMessage(ByRef m As System.Windows.Forms.Message) As Boolean
        'Return MyBase.ProcessKeyMessage(m)
        If CInt(m.WParam) = 13 Then
            RaiseEvent DataGridKeyIntercept(13)
            Return True
        ElseIf CInt(m.WParam) = 27 Then
            RaiseEvent DataGridKeyIntercept(27)
            Return True
        ElseIf CInt(m.WParam) = 34 Then
            Dim p As String = ""
        ElseIf CInt(m.WParam) = 39 Then
            RaiseEvent DataGridKeyIntercept(39)
            Return True
            'ElseIf m.WParam = 46 And m.Msg = 256 Then
            '    RaiseEvent DataGridKeyIntercept(46)
            '    Return False

            'ElseIf m.WParam = 96 Then
            '    RaiseEvent DataGridKeyIntercept(96)
            '    Return True
            'ElseIf m.WParam = 222 Then
            '    RaiseEvent DataGridKeyIntercept(222)
            '    Return True

        End If
        Return MyBase.ProcessKeyPreview(m)
    End Function

    Protected Overrides Function ProcessKeyPreview(ByRef m As System.Windows.Forms.Message) As Boolean
        If CInt(m.WParam) = 13 Then
            RaiseEvent DataGridKeyIntercept(13)
            Return True
        ElseIf CInt(m.WParam) = 27 Then
            RaiseEvent DataGridKeyIntercept(27)
            Return True

        ElseIf CInt(m.WParam) = 34 Then
            RaiseEvent DataGridKeyIntercept(34)
            Return True
        ElseIf CInt(m.WParam) = 39 And m.Msg = 258 Then
            RaiseEvent DataGridKeyIntercept(39)
            Return True
        ElseIf CInt(m.WParam) = 46 And m.Msg = 258 Then
            RaiseEvent DataGridKeyIntercept(46)
            Return True
            'ElseIf m.WParam = 96 Then
            '    RaiseEvent DataGridKeyIntercept(96)
            '    Return True
            'ElseIf m.WParam = 222 Then
            '    RaiseEvent DataGridKeyIntercept(222)
            '    Return True

        End If
        Return MyBase.ProcessKeyPreview(m)
    End Function
      


    'Protected Overloads Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
    '    Dim key As Keys = (keyData And Keys.KeyCode)
    '    If key = Keys.Enter Then
    '        RaiseEvent DataGridKeyIntercept(Keys.KeyCode)
    '        Return True

    '    ElseIf key = Keys.Escape Then

    '        RaiseEvent DataGridKeyIntercept(Keys.KeyCode)
    '        Return True


    '    End If
    '    Return MyBase.ProcessDialogKey(keyData)
    'End Function
    'Protected Overloads Overrides Function ProcessDataGridViewKey(ByVal e As KeyEventArgs) As Boolean
    '    If e.KeyCode = Keys.Enter Then
    '        e.Handled = True
    '        RaiseEvent DataGridKeyIntercept(e.KeyCode)

    '        Return True
    '    ElseIf e.KeyCode = Keys.Escape Then
    '        e.Handled = True
    '        RaiseEvent DataGridKeyIntercept(e.KeyCode)
    '        Return True
    '    End If

    '    Return MyBase.ProcessDataGridViewKey(e)
    'End Function
End Class
'Public Class MyCustomTextBox
'    Inherits System.Windows.Forms.TextBox
'    Protected Overrides Function ProcessKeyEventArgs(ByRef m As System.Windows.Forms.Message) As Boolean

'        Return MyBase.ProcessKeyEventArgs(m)
'    End Function
'    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
'        If keyData = Keys.Enter Then
'            Return True

'        End If
'        Return MyBase.ProcessCmdKey(msg, keyData)
'    End Function
'    Protected Overloads Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
'        If keyData = Keys.[Return] Then

'            Return True
'        Else
'            Return MyBase.ProcessDialogKey(keyData)
'        End If
'    End Function
'End Class