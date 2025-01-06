Imports System.IO

Public Class HostsFileRestorer
    Private ReadOnly hostsFilePath As String = "C:\Windows\System32\drivers\etc\hosts"
    Private ReadOnly backupFilePath As String = $"{hostsFilePath}.bak"

    ' Method to restore the hosts file from backup
    Public Sub RestoreHostsFile()
        Try
            ' Check if the backup file exists
            If Not File.Exists(backupFilePath) Then
                Console.WriteLine("Backup file does not exist. Cannot restore the hosts file.")
                Return
            End If

            ' Restore the hosts file from the backup
            File.Copy(backupFilePath, hostsFilePath, True) ' Overwrite the existing hosts file
            Console.WriteLine("Hosts file restored successfully from backup.")

            ' Optionally, you can delete the backup after restoration
            ' File.Delete(backupFilePath)
        Catch ex As UnauthorizedAccessException
            Console.WriteLine("You need to run this application as an administrator.")
        Catch ex As Exception
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try
    End Sub
End Class