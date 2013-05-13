
' | Version 10.2
' | Copyright 2013 Esri
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


Public Class CustomPanel
    Inherits Windows.Forms.Panel

    Private m_Pen As System.Drawing.Pen = New System.Drawing.Pen(System.Drawing.Color.FromArgb(127, 157, 185))


    'A custom panel used for two domain field.  This is requred to set the border color of a panel
    Public Sub New()
        MyBase.new()


    End Sub
    Public Property BorderColor() As System.Drawing.Pen
        Get
            Return m_Pen
        End Get
        Set(ByVal value As System.Drawing.Pen)
            m_Pen = value

        End Set
    End Property

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        e.Graphics.DrawRectangle(m_Pen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1)


        MyBase.OnPaint(e)
    End Sub
End Class
