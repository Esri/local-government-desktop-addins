Imports System.Drawing
Public Class LengthTip
    Private m_Pen As System.Drawing.Pen = New System.Drawing.Pen(System.Drawing.Color.FromArgb(127, 157, 185))
    Private Sub LengthTip_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        m_Pen = New Pen(Color.Black, 5)
    End Sub
    Public Sub SetLabel(text As String)

        mapTipLabel.Text = Format(Val(text), "0.00")
        Dim g As System.Drawing.Graphics
        Dim s As System.Drawing.SizeF
        g = mapTipLabel.CreateGraphics()

        s = g.MeasureString(mapTipLabel.Text, mapTipLabel.Font)
        Me.Width = s.Width + 7
        Me.Height = s.Height + 7

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        'e.Graphics.DrawRectangle(m_Pen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1)


        MyBase.OnPaint(e)
    End Sub
End Class