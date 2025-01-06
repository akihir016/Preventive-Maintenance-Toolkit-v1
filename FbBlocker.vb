Imports System.Diagnostics
Imports System.IO
Public Class FbBlocker
    Private ReadOnly hostsFilePath As String = "C:\Windows\System32\drivers\etc\hosts"

    ' List of entries to add to the hosts file
    Private ReadOnly entries As String() = {
        "127.0.0.1 www.facebook.com",
        "127.0.0.1 facebook.com",
        "127.0.0.1 static.ak.fbcdn.net",
        "127.0.0.1 www.static.ak.fbcdn.net",
        "127.0.0.1 login.facebook.com",
        "127.0.0.1 www.login.facebook.com",
        "127.0.0.1 fbcdn.net",
        "127.0.0.1 www.fbcdn.net",
        "127.0.0.1 fbcdn.com",
        "127.0.0.1 www.fbcdn.com",
        "127.0.0.1 static.ak.connect.facebook.com",
        "127.0.0.1 www.static.ak.connect.facebook.com"
    }

    ' Method to add entries to the hosts file
    Public Sub AddEntriesToHostsFile()
        Try
            ' Backup the existing hosts file
            Dim backupFilePath As String = $"{hostsFilePath}.bak"
            If File.Exists(hostsFilePath) Then
                File.Copy(hostsFilePath, backupFilePath, True) ' Overwrite if the backup already exists
                Console.WriteLine("Backup of the hosts file created successfully.")
            End If

            Dim existingEntries As List(Of String) = New List(Of String)

            ' Read existing entries from the hosts file
            If File.Exists(hostsFilePath) Then
                existingEntries = File.ReadAllLines(hostsFilePath).ToList()
            End If

            ' Filter out entries that already exist
            Dim newEntries = entries.Where(Function(e) Not existingEntries.Contains(e)).ToArray()

            If newEntries.Length = 0 Then
                Console.WriteLine("No new entries to add. All entries already exist.")
                Return
            End If

            ' Create a PowerShell command to append new entries to the hosts file
            Dim command As String = String.Join(Environment.NewLine, newEntries.Select(Function(e) $"echo {e} >> {hostsFilePath}"))

            ' Run the PowerShell command
            Using process As New Process()
                process.StartInfo.FileName = "powershell.exe"
                process.StartInfo.Arguments = $"-Command ""{command}"""
                process.StartInfo.Verb = "runas" ' Run as administrator
                process.StartInfo.UseShellExecute = True
                process.StartInfo.RedirectStandardOutput = False
                process.StartInfo.RedirectStandardError = False
                process.StartInfo.CreateNoWindow = True

                process.Start()
                process.WaitForExit()
            End Using

            Console.WriteLine("New entries added to hosts file successfully.")
        Catch ex As UnauthorizedAccessException
            Console.WriteLine("You need to run this application as an administrator.")
        Catch ex As Exception
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try
    End Sub
End Class
