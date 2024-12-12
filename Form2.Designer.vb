<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        ComboBox1 = New ComboBox()
        btnOpenPanel = New Button()
        Button1 = New Button()
        SuspendLayout()
        ' 
        ' ComboBox1
        ' 
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(12, 33)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(182, 33)
        ComboBox1.TabIndex = 0
        ' 
        ' btnOpenPanel
        ' 
        btnOpenPanel.Location = New Point(12, 103)
        btnOpenPanel.Name = "btnOpenPanel"
        btnOpenPanel.Size = New Size(112, 34)
        btnOpenPanel.TabIndex = 1
        btnOpenPanel.Text = "Launch"
        btnOpenPanel.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(190, 103)
        Button1.Name = "Button1"
        Button1.Size = New Size(44, 34)
        Button1.TabIndex = 2
        Button1.Text = "X"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Form2
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(246, 158)
        ControlBox = False
        Controls.Add(Button1)
        Controls.Add(btnOpenPanel)
        Controls.Add(ComboBox1)
        Name = "Form2"
        ShowIcon = False
        Text = "Legacy Apps Launcher"
        ResumeLayout(False)
    End Sub

    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents btnOpenPanel As Button
    Friend WithEvents Button1 As Button
End Class
