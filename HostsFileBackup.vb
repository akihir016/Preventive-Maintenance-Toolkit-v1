Imports System.IO

Public Class HostsFileBackup
    Private ReadOnly hostsFilePath As String = "C:\Windows\System32\drivers\etc\hosts"
    Private ReadOnly backupFilePath As String = $"{hostsFilePath}.bak"

    ' Method to create a backup of the hosts file
    Public Sub CreateBackup(overwrite As Boolean)
        Try
            ' Check if the backup file already exists
            If File.Exists(backupFilePath) Then
                If Not overwrite Then
                    Console.WriteLine("Backup file already exists. Use 'overwrite' parameter to replace it.")
                    Return
                End If
            End If

            ' Create a backup of the hosts file
            File.Copy(hostsFilePath, backupFilePath, True) ' Overwrite if the backup already exists
            Console.WriteLine("Backup of the hosts file created successfully.")

        Catch ex As UnauthorizedAccessException
            Console.WriteLine("You need to run this application as an administrator.")
        Catch ex As Exception
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try
    End Sub
End Class