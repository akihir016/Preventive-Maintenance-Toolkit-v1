Imports System.Windows.Forms

Public Class PUpPowerlog
    Inherits Form

    Public Sub New()
        Me.Text = "Windows Event Viewer"
        Me.Width = 400
        Me.Height = 200

        Dim label As New Label
        label.Text = "To check for power related issues, please follow these steps:"
        label.AutoSize = True
        label.Location = New Point(10, 10)
        Me.Controls.Add(label)

        Dim step1 As New Label
        step1.Text = "1. On Event Viewer, select filter current log under Actions/System on right most panel."
        step1.AutoSize = True
        step1.Location = New Point(10, 30)
        Me.Controls.Add(step1)

        Dim step2 As New Label
        step2.Text = "2. Under filter, change <All Event IDs> to 41."
        step2.AutoSize = True
        step2.Location = New Point(10, 50)
        Me.Controls.Add(step2)

        Dim step3 As New Label
        step3.Text = "3. This shall filter to only show errors concerning sudden power off."
        step3.AutoSize = True
        step3.Location = New Point(10, 70)
        Me.Controls.Add(step3)

        Dim step4 As New Label
        step4.Text = "4. Search for the Most recent or the time of shutdown you want to investigate."
        step4.AutoSize = True
        step4.Location = New Point(10, 90)
        Me.Controls.Add(step4)

        Dim step5 As New Label
        step5.Text = "5. You can now, check for details and start googling :D."
        step5.AutoSize = True
        step5.Location = New Point(10, 110)
        Me.Controls.Add(step5)

        Dim button As New Button
        button.Text = "Open Event Viewer"
        button.Location = New Point(10, 120)
        AddHandler button.Click, AddressOf Button_Click
        Me.Controls.Add(button)
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs)
        Process.Start("eventvwr.exe", "/c:system")
        Me.Close()
    End Sub
End Class