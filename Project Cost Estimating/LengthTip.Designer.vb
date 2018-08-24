<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LengthTip
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.mapTipLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'mapTipLabel
        '
        Me.mapTipLabel.AutoSize = True
        Me.mapTipLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapTipLabel.Location = New System.Drawing.Point(0, 0)
        Me.mapTipLabel.Name = "mapTipLabel"
        Me.mapTipLabel.Size = New System.Drawing.Size(39, 13)
        Me.mapTipLabel.TabIndex = 0
        Me.mapTipLabel.Text = "Label1"
        '
        'LengthTip
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.Yellow
        Me.ClientSize = New System.Drawing.Size(172, 49)
        Me.ControlBox = False
        Me.Controls.Add(Me.mapTipLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "LengthTip"
        Me.Text = "LengthTip"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents mapTipLabel As Windows.Forms.Label
End Class
