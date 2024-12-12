Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate the ComboBox with control panel options
        ComboBox1.Items.Add("WPFPanelcontrol")
        ComboBox1.Items.Add("WPFPanelnetwork")
        ComboBox1.Items.Add("WPFPanelpower")
        ComboBox1.Items.Add("WPFPanelregion")
        ComboBox1.Items.Add("WPFPanelsound")
        ComboBox1.Items.Add("WPFPanelsystem")
        ComboBox1.Items.Add("WPFPaneluser")
    End Sub

    Private Sub btnOpenPanel_Click(sender As Object, e As EventArgs) Handles btnOpenPanel.Click
        If ComboBox1.SelectedItem IsNot Nothing Then
            Dim selectedPanel = ComboBox1.SelectedItem.ToString
            InvokeWPFControlPanel(selectedPanel)
        Else
            MessageBox.Show("Please select a control panel from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' Opens the requested legacy control panel.
    ''' </summary>
    ''' <param name="panel">The panel to open.</param>
    Private Sub InvokeWPFControlPanel(panel As String)
        Select Case panel
            Case "WPFPanelcontrol"
                Process.Start("cmd.exe", "/c control")
            Case "WPFPanelnetwork"
                Process.Start("cmd.exe", "/c ncpa.cpl")
            Case "WPFPanelpower"
                Process.Start("cmd.exe", "/c powercfg.cpl")
            Case "WPFPanelregion"
                Process.Start("cmd.exe", "/c intl.cpl")
            Case "WPFPanelsound"
                Process.Start("cmd.exe", "/c mmsys.cpl")
            Case "WPFPanelsystem"
                Process.Start("cmd.exe", "/c sysdm.cpl")
            Case "WPFPaneluser"
                Process.Start("cmd.exe", "/c control userpasswords2") ''' this opens the legacy app for User Accounts.
            Case Else
                MessageBox.Show("Invalid panel specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Select
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class