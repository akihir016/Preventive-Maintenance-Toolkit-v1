﻿Imports System.IO
Imports System.Management
Imports System.Xml
Imports System.Diagnostics
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading

Public Class Form1
    Private Sub Form1_Load(ender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs)
        ' Check which key was pressed and update the input sequence
        Select Case e.KeyCode
            Case Keys.Up
                inputSequence += "U"
            Case Keys.Down
                inputSequence += "D"
            Case Keys.Left
                inputSequence += "L"
            Case Keys.Right
                inputSequence += "R"
            Case Keys.A
                inputSequence += "A"
            Case Keys.B
                inputSequence += "B"
            Case Else
                Return ' Ignore other keys
        End Select

        ' Debug: Log the current input sequence
        Console.WriteLine("Current Input Sequence: " & inputSequence)

        ' Check if the input sequence matches the Konami Code
        If inputSequence.Length > konamiCode.Length Then
            inputSequence = inputSequence.Substring(1) ' Keep the last N characters
        End If

        If inputSequence = konamiCode Then
            EnableToolStripMenu()
            inputSequence = "" ' Reset the input sequence
        End If
    End Sub
    Private Sub nfoScan_Click(sender As Object, e As EventArgs) Handles nfoScan.Click
        Try
            ' Collect system information
            Dim systemInfoXml As String = GenerateSystemInfoXml()

            ' Display XML in TreeView
            PopulateTreeViewWithXmlData(systemInfoXml)
            MessageBox.Show("Scanning Complete.")
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub
    Private Function GenerateSystemInfoXml() As String
        Dim xmlDoc As New XmlDocument()
        Dim xmlDeclaration As XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        xmlDoc.AppendChild(xmlDeclaration)

        Dim rootNode As XmlNode = xmlDoc.CreateElement("SystemInformation")
        xmlDoc.AppendChild(rootNode)

        ' Add collected information to XML
        AddXmlElement(xmlDoc, rootNode, "OperatingSystem", GetOperatingSystemInfo())
        AddXmlElement(xmlDoc, rootNode, "InstallDate", GetOperatingSystemInstallDate())
        AddXmlElement(xmlDoc, rootNode, "CPU", GetCpuInfo())
        AddXmlElement(xmlDoc, rootNode, "RAM", GetRamInfo()) ' Ensure this captures the numeric value
        AddXmlElement(xmlDoc, rootNode, "GPU", GetGpuInfo())
        AddXmlElement(xmlDoc, rootNode, "ComputerName", Environment.MachineName)
        AddXmlElement(xmlDoc, rootNode, "StorageDevices", GetStorageInfo()) ' Changed to StorageDevices

        Return xmlDoc.OuterXml
    End Function
    Private Sub PopulateTreeViewWithXmlData(xmlData As String)
        treeViewXML.Nodes.Clear()
        If Not String.IsNullOrEmpty(xmlData) Then
            Dim xmlDoc As New XmlDocument()
            xmlDoc.LoadXml(xmlData)
            Dim rootNode As TreeNode = treeViewXML.Nodes.Add(xmlDoc.DocumentElement.Name)
            AddNode(xmlDoc.DocumentElement, rootNode)
            treeViewXML.ExpandAll()
        Else
            MessageBox.Show("XML data is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub AddNode(xmlNode As XmlNode, treeNode As TreeNode)
        For Each childNode As XmlNode In xmlNode.ChildNodes
            Dim childTreeNode As TreeNode

            ' Handle text nodes directly
            If childNode.NodeType = XmlNodeType.Text Then
                treeNode.Text &= ": " & childNode.Value
            Else
                childTreeNode = treeNode.Nodes.Add(childNode.Name)
                If childNode.HasChildNodes Then
                    AddNode(childNode, childTreeNode)
                ElseIf childNode.Name = "StorageDevices" Then
                    Dim storages() As String = childNode.InnerText.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
                    For Each storage As String In storages
                        childTreeNode.Nodes.Add(storage)
                    Next
                Else
                    childTreeNode.Text = childNode.Name & ": " & childNode.InnerText
                End If
            End If
        Next
    End Sub
    Private Sub ExportScanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportScanToolStripMenuItem.Click
        Try
            Dim saveDialog As New SaveFileDialog()
            saveDialog.Filter = "XML Files (*.xml)|*.xml"  ' Change filter to XML files
            saveDialog.Title = "Save XML File"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Dim xmlPath As String = saveDialog.FileName

                ' Check if the file already exists
                If System.IO.File.Exists(xmlPath) Then
                    Dim overwriteResult As DialogResult = MessageBox.Show("The file already exists. Do you want to overwrite it?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If overwriteResult = DialogResult.No Then
                        Return ' Exit if the user chooses not to overwrite
                    End If
                End If

                Dim xmlContent As String = GetXmlStringFromTreeView(treeViewXML)

                If Not String.IsNullOrEmpty(xmlContent) Then
                    ' Save XML content as XML file with line breaks for readability
                    System.IO.File.WriteAllText(xmlPath, FormatXml(xmlContent), System.Text.Encoding.UTF8)
                    MessageBox.Show("XML file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("XML content is empty or invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("No file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred while saving the XML file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Function FormatXml(xmlContent As String) As String
        Dim doc As New XmlDocument()
        doc.LoadXml(xmlContent)
        Using stringWriter As New StringWriter()
            Using xmlWriter As XmlWriter = XmlWriter.Create(stringWriter, New XmlWriterSettings() With {.Indent = True, .NewLineOnAttributes = False})
                doc.Save(xmlWriter)
                Return stringWriter.ToString()
            End Using
        End Using
    End Function
    Private Function GetXmlStringFromTreeView(treeView As TreeView) As String
        Dim xmlDoc As New XmlDocument()
        Dim xmlDeclaration As XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        xmlDoc.AppendChild(xmlDeclaration)

        Dim rootNode As XmlNode = xmlDoc.CreateElement("SystemInformation")
        xmlDoc.AppendChild(rootNode)

        ' Add treeViewXML content to XML
        For Each treeNode As TreeNode In treeView.Nodes
            AddXmlNodeFromTreeView(xmlDoc, rootNode, treeNode)
        Next

        Return xmlDoc.OuterXml
    End Function
    Private Sub AddXmlNodeFromTreeView(xmlDoc As XmlDocument, parentNode As XmlNode, treeNode As TreeNode)
        Dim newNode As XmlNode = xmlDoc.CreateElement(SanitizeXmlName(treeNode.Text.Split(":")(0).Trim()))

        If treeNode.Nodes.Count > 0 Then
            For Each childNode As TreeNode In treeNode.Nodes
                AddXmlNodeFromTreeView(xmlDoc, newNode, childNode)
            Next
        Else
            newNode.InnerText = treeNode.Text.Split(":")(1).Trim()
        End If

        parentNode.AppendChild(newNode)
    End Sub
    Private Function SanitizeXmlName(name As String) As String
        ' Remove invalid characters from the name
        Dim sanitized As String = System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_.-]", "_")

        ' Ensure the name starts with a letter or underscore
        If Not String.IsNullOrEmpty(sanitized) AndAlso Not Char.IsLetter(sanitized(0)) And Not sanitized.StartsWith("_") Then
            sanitized = "_" & sanitized
        End If

        Return sanitized
    End Function
    Private Sub AddXmlElement(xmlDoc As XmlDocument, parentNode As XmlNode, elementName As String, elementValue As String)
        Dim element As XmlElement = xmlDoc.CreateElement(elementName)
        element.InnerText = elementValue ' Ensure that this captures the RAM value as a string
        parentNode.AppendChild(element)
    End Sub
    Private Function GetOperatingSystemInfo() As String
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
            For Each obj As ManagementObject In searcher.Get()
                Return $"{obj("Caption")} {obj("Version")}"
            Next
        End Using
        Return String.Empty
    End Function
    Private Function GetOperatingSystemInstallDate() As String
        Dim installDate As String = String.Empty
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
            For Each obj As ManagementObject In searcher.Get()
                Dim dateStr As String = obj("InstallDate").ToString()
                installDate = ManagementDateTimeConverter.ToDateTime(dateStr).ToString("yyyy-MM-dd")
            Next
        End Using
        Return installDate
    End Function
    Private Function GetCpuInfo() As String
        Dim cpuInfo As String = String.Empty
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_Processor")
            For Each obj As ManagementObject In searcher.Get()
                cpuInfo = obj("Name").ToString()
            Next
        End Using
        Return cpuInfo
    End Function
    Private Function GetRamInfo() As String
        Dim totalRam As Long = 0
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory")
            For Each obj As ManagementObject In searcher.Get()
                totalRam += CLng(obj("Capacity"))
            Next
        End Using

        Dim totalRamInGB As Double = Math.Round(totalRam / 1024 / 1024 / 1024, 2)
        Return totalRamInGB.ToString() ' Return only the numeric value in GB
    End Function
    Private Function GetStorageInfo() As String
        Dim storageInfo As New StringBuilder()
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive")
            For Each obj As ManagementObject In searcher.Get()
                Dim model As String = obj("Model").ToString()
                Dim size As Long = CLng(obj("Size"))
                Dim sizeInGB As Double = Math.Round(size / 1024 / 1024 / 1024, 2)
                storageInfo.AppendLine($"{model} ({sizeInGB} GB)")
            Next
        End Using
        Return storageInfo.ToString().Trim()
    End Function
    Private Function GetGpuInfo() As String
        Dim gpuInfo As String = String.Empty
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_VideoController")
            For Each obj As ManagementObject In searcher.Get()
                Dim gpuName As String = obj("Name").ToString()
                Dim gpuMemory As Long = CLng(obj("AdapterRAM"))
                Dim gpuMemoryInMB As Double = Math.Round(gpuMemory / 1024 / 1024, 2)
                gpuInfo &= gpuName & " (" & gpuMemoryInMB.ToString() & " MB)" & Environment.NewLine
            Next
        End Using
        Return gpuInfo.Trim()
    End Function
    Private Sub RunSystemPropertiesProtection()
        Try
            Dim system32Path As String = Environment.GetFolderPath(Environment.SpecialFolder.System)
            Dim systemPropertiesProtectionPath As String = Path.Combine(system32Path, "SystemPropertiesProtection.exe")

            If File.Exists(systemPropertiesProtectionPath) Then
                Process.Start(systemPropertiesProtectionPath)
            Else
                MessageBox.Show("SystemPropertiesProtection.exe not found in System32 folder.")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub
    Private Sub StartCommand(command As String)
        Process.Start("cmd.exe", $"/c start {command}")
    End Sub

    Private Sub btnWindowsSecurity_Click(sender As Object, e As EventArgs) Handles btnWindowsSecurity.Click
        StartCommand("windowsdefender:")
    End Sub

    Private Sub btnWindowsUpdate_Click(sender As Object, e As EventArgs) Handles btnWindowsUpdate.Click
        StartCommand("ms-settings:windowsupdate")
    End Sub

    Private Sub btnSystemRestore_Click(sender As Object, e As EventArgs) Handles btnSystemRestore.Click
        RunSystemPropertiesProtection()
    End Sub

    Private Sub btnTaskManager_Click(sender As Object, e As EventArgs) Handles btnTaskManager.Click
        StartCommand("taskmgr")
    End Sub

    Private Sub btnDiskDefragmenter_Click(sender As Object, e As EventArgs) Handles btnDiskDefragmenter.Click
        StartCommand("dfrgui")
    End Sub

    Private Sub btnOpenAddOrRemovePrograms_Click(sender As Object, e As EventArgs) Handles btnOpenAddOrRemovePrograms.Click
        RunAppwizCpl()
    End Sub
    Private Sub RunAppwizCpl()
        Try
            ' Use the Shell function to run appwiz.cpl
            Shell("appwiz.cpl", AppWinStyle.NormalFocus)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub
    Private Sub btnPing_Click(sender As Object, e As EventArgs) Handles BtnPing.Click
        Dim address As String = IpBox.Text.Trim()

        ' If no address is entered, ping Google.com
        If String.IsNullOrEmpty(address) Then
            address = "google.com"
        End If

        If Not String.IsNullOrEmpty(address) Then
            LstPingResults.Items.Clear() ' Clear the ListBox before performing the ping

            Dim processInfo As New ProcessStartInfo("ping", "-n 4 " & address) ' Ping the address 4 times
            processInfo.RedirectStandardOutput = True
            processInfo.UseShellExecute = False
            processInfo.CreateNoWindow = True

            Dim process As Process = Process.Start(processInfo)
            process.WaitForExit()

            Dim output As String = process.StandardOutput.ReadToEnd()

            ' Split the output by line and add it to the list box
            For Each line As String In output.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                LstPingResults.Items.Add(line)
            Next
        Else
            MessageBox.Show("Please enter an address to ping.")
        End If
    End Sub
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Process.Start("DISMCheckhealth.bat")
    End Sub

    Private Sub DocumentationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DocumentationToolStripMenuItem.Click
        Dim pdfPath As String = Path.Combine(Application.StartupPath, "Resources", "PMT.pdf")
        Dim edgePath As String = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"

        ' Check if the PDF file exists
        If Not File.Exists(pdfPath) Then
            MessageBox.Show("The documentation file could not be found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Check if Edge is installed
        If Not File.Exists(edgePath) Then
            MessageBox.Show("Microsoft Edge is not installed at the expected location.", "Edge Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Start the process to open the PDF
        Process.Start(New ProcessStartInfo(edgePath, pdfPath) With {.UseShellExecute = True})
    End Sub

    Private Sub MASActivationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MASActivationToolStripMenuItem.Click
        Dim result As DialogResult = MessageBox.Show("Do you want to run the activation script from the URL?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                Dim psi As New ProcessStartInfo() With {
                .FileName = "powershell.exe",
                .Arguments = "-NoProfile -ExecutionPolicy Bypass -Command ""Invoke-Expression (New-Object Net.WebClient).DownloadString('https://get.activated.win')""",
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

                Dim process As Process = Process.Start(psi)
                process.WaitForExit()

                Dim output As String = process.StandardOutput.ReadToEnd()
                Dim errorOutput As String = process.StandardError.ReadToEnd()

                If Not String.IsNullOrEmpty(output) Then
                    MessageBox.Show(output, "Output", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                If Not String.IsNullOrEmpty(errorOutput) Then
                    MessageBox.Show(errorOutput, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
    Private Sub WinActivationScanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WinActivationScanToolStripMenuItem.Click
        Process.Start("AutoCreateRestorePoint.bat")
    End Sub

    Private Sub WinUtilToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WinUtilToolStripMenuItem.Click
        Dim result As DialogResult = MessageBox.Show("By proceeding to use this tool, The Developer will held no liablities for the results. ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        ' If the user clicks Yes, run the PowerShell command
        If result = DialogResult.Yes Then
            Try
                Dim psi As New ProcessStartInfo()
                psi.FileName = "powershell.exe"
                psi.Arguments = "-NoProfile -ExecutionPolicy Bypass -Command ""Invoke-Expression (New-Object Net.WebClient).DownloadString('https://christitus.com/win')"""
                psi.RedirectStandardOutput = True
                psi.RedirectStandardError = True
                psi.UseShellExecute = False
                psi.CreateNoWindow = True

                ' Start the process
                Dim process As Process = Process.Start(psi)
                process.WaitForExit() ' Optional: Wait for the process to exit

                ' Optionally read output
                Dim output As String = process.StandardOutput.ReadToEnd()
                Dim errorOutput As String = process.StandardError.ReadToEnd()

                ' Show output (if needed)
                If Not String.IsNullOrEmpty(output) Then
                    MessageBox.Show(output, "Output", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                If Not String.IsNullOrEmpty(errorOutput) Then
                    MessageBox.Show(errorOutput, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        ' Confirm before exiting
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            ' Exit the application
            Application.Exit()
        End If
    End Sub

    Private ReadOnly konamiCode As String = "UUDDLRLRBA"
    Private inputSequence As String = ""

    Public Sub New()
        InitializeComponent()
        Me.KeyPreview = True ' Ensure the form receives key events
        AddHandler Me.KeyDown, AddressOf Form1_KeyDown ' Correct way to add the event handler
    End Sub

    Private Sub EnableToolStripMenu()
        ' Enable the ToolStrip button or menu item
        ToolsToolStripMenuItem.Visible = True ' Ensure this is the correct reference
        MessageBox.Show("Advance Tools enabled.")
    End Sub


    Private Sub btnGenerateReport_Click(sender As Object, e As EventArgs) Handles btnGenerateReport.Click
        If CheckBox1.Checked Then
            ' Define the folder path
            Dim folderPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Battery-Report")

            ' Check if the folder exists, if not, create it
            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            ' Get the computer name and define the file path for the battery report
            Dim computerName As String = Environment.MachineName
            Dim reportFileName As String = $"{computerName}_battery_report.html"
            Dim reportFilePath As String = Path.Combine(folderPath, reportFileName)

            ' Create the process to run the powercfg command
            Dim process As New Process()
            process.StartInfo.FileName = "cmd.exe"
            process.StartInfo.Arguments = $"/c powercfg /batteryreport /output ""{reportFilePath}"""
            process.StartInfo.UseShellExecute = False
            process.StartInfo.RedirectStandardOutput = True
            process.StartInfo.RedirectStandardError = True
            process.StartInfo.CreateNoWindow = True

            Try
                ' Start the process
                process.Start()
                process.WaitForExit()

                ' Inform the user and provide an option to open the containing folder
                Dim result As DialogResult = MessageBox.Show("Battery report generated successfully at: " & reportFilePath & vbCrLf & "Do you want to open the containing folder?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                ' Open the containing folder if the user clicks Yes
                If result = DialogResult.Yes Then
                    Process.Start("explorer.exe", folderPath)
                End If

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        Try

        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        If CheckBox2.Checked Then
            Dim licenseDescription As String = "OEM_DM" ' Replace with actual license description
            Dim licenseType As New WindowsLicenseType(licenseDescription)
        End If

        If CheckBox3.Checked Then
            Process.Start("cmd.exe", "/c control userpasswords2") ''' this opens the legacy app for User Accounts.
        End If

        If CheckBox4.Checked Then
            Dim dialog As New PUpPowerlog
            dialog.ShowDialog(Me)
        End If

        If CheckBox6.Checked Then
            Dim modifier As New FbBlocker()
            modifier.AddEntriesToHostsFile()
        End If

        If CheckBox7.Checked Then
            Dim cleaner As New fbUnblocker()
            cleaner.RemoveEntriesFromHostsFile()
        End If

        If CheckBox8.Checked Then
            Dim spooler As New PrintSpoolerManager()
            spooler.RestartPrintSpooler()
        End If

        If CheckBox9.Checked Then
            Dim restorer As New HostsFileRestorer()
            restorer.RestoreHostsFile()
        End If

        If CheckBox5.Checked Then
            Dim backup As New HostsFileBackup()
            backup.CreateBackup(overwrite:=False)
        End If

    End Sub

    Private Sub RecoverWindowsLicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RecoverWindowsLicToolStripMenuItem.Click
        Form2.Show()
    End Sub

End Class

