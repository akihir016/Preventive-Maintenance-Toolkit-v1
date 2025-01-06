Imports System.IO

Public Class fbUnblocker
    Private ReadOnly hostsFilePath As String = "C:\Windows\System32\drivers\etc\hosts"

    ' List of entries to remove from the hosts file
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

    ' Method to remove entries from the hosts file
    Public Sub RemoveEntriesFromHostsFile()
        Try
            ' Read all lines from the hosts file
            Dim lines As List(Of String) = File.ReadAllLines(hostsFilePath).ToList()

            ' Remove entries if they exist
            For Each entry In entries
                lines.RemoveAll(Function(line) line.Trim() = entry)
            Next

            ' Write the updated lines back to the hosts file
            File.WriteAllLines(hostsFilePath, lines)

            Console.WriteLine("Entries removed from hosts file successfully.")
        Catch ex As Exception
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try
    End Sub
End Class
