Public Class LengthTip
    Private Sub LengthTip_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Sub SetLabel(text As String)

        mapTipLabel.Text = Format(Val(text), "0.00")
    End Sub

End Class