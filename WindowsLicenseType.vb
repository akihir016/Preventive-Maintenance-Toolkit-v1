Imports System.Windows.Forms

Public Enum LicenseType
    OEM_DM
    OEM_SLP
    OEM_COA
    Retail
    Volume
    Unknown
End Enum

Public Class WindowsLicenseType
    Private _licenseType As LicenseType

    Public Sub New(licenseDescription As String)
        _licenseType = GetLicenseType(licenseDescription)
        ShowLicenseTypeDialog()
    End Sub

    Private Function GetLicenseType(licenseDescription As String) As LicenseType
        If licenseDescription.Contains("OEM_DM") Then
            Return LicenseType.OEM_DM
        ElseIf licenseDescription.Contains("OEM_SLP") Then
            Return LicenseType.OEM_SLP
        ElseIf licenseDescription.Contains("OEM_COA") Then
            Return LicenseType.OEM_COA
        ElseIf licenseDescription.Contains("RETAIL") Then
            Return LicenseType.Retail
        ElseIf licenseDescription.Contains("VOLUME") Then
            Return LicenseType.Volume
        Else
            Return LicenseType.Unknown
        End If
    End Function

    Private Sub ShowLicenseTypeDialog()
        Dim dialog As New Form
        dialog.Text = "Windows License Type"
        dialog.Width = 500
        dialog.Height = 300

        Dim label As New Label
        label.Text = "Your Windows license type is:"
        label.AutoSize = True
        label.Location = New Point(10, 10)
        dialog.Controls.Add(label)

        Dim licenseTypeLabel As New Label
        licenseTypeLabel.Text = _licenseType.ToString()
        licenseTypeLabel.AutoSize = True
        licenseTypeLabel.Location = New Point(10, 30)
        dialog.Controls.Add(licenseTypeLabel)

        Dim descriptionLabel As New Label
        descriptionLabel.Text = GetLicenseDescription(_licenseType)
        descriptionLabel.AutoSize = True
        descriptionLabel.Location = New Point(10, 60)
        descriptionLabel.MaximumSize = New Size(450, 0)
        descriptionLabel.AutoScrollOffset = New Point(0, 0)
        dialog.Controls.Add(descriptionLabel)

        Dim button As New Button
        button.Text = "OK"
        button.Location = New Point(10, 250)
        AddHandler button.Click, AddressOf Button_Click
        dialog.Controls.Add(button)

        dialog.ShowDialog()
    End Sub
    Private Function GetLicenseDescription(licenseType As LicenseType) As String
        Select Case licenseType
            Case LicenseType.OEM_DM
                Return "OEM_DM (Device Manufacturer) license: This license is pre-installed on devices by the manufacturer. It is tied to the device and cannot be transferred to another device."
            Case LicenseType.OEM_SLP
                Return "OEM_SLP (System Locked Pre-installation) license: This license is pre-installed on devices by the manufacturer. It is tied to the device and cannot be transferred to another device. The license is also locked to the device's BIOS."
            Case LicenseType.OEM_COA
                Return "OEM_COA (Certificate of Authenticity) license: This license is pre-installed on devices by the manufacturer. It is tied to the device and cannot be transferred to another device. The license is also verified by a Certificate of Authenticity sticker on the device."
            Case LicenseType.Retail
                Return "Retail license: This license is purchased separately from a device. It can be installed on any device that meets the system requirements."
            Case LicenseType.Volume
                Return "Volume license: This license is purchased in bulk by organizations. It can be installed on multiple devices within the organization."
            Case Else
                Return "Unknown license type."
        End Select
    End Function

    Private Sub Button_Click(sender As Object, e As EventArgs)
        Dim dialog As Form = DirectCast(sender, Button).Parent
        dialog.Close()
    End Sub
End Class