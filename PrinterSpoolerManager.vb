Imports System.Diagnostics

Public Class PrintSpoolerManager
    ' Method to restart the Print Spooler service
    Public Sub RestartPrintSpooler()
        Try
            ' Create a new process to run the PowerShell command
            Dim process As New Process()
            process.StartInfo.FileName = "powershell.exe"
            process.StartInfo.Arguments = "-Command ""Restart-Service -Name Spooler -Force"""
            process.StartInfo.Verb = "runas" ' Run as administrator
            process.StartInfo.UseShellExecute = True
            process.StartInfo.RedirectStandardOutput = False
            process.StartInfo.RedirectStandardError = False
            process.StartInfo.CreateNoWindow = True

            process.Start()
            process.WaitForExit()

            Console.WriteLine("Print Spooler service restarted successfully.")
        Catch ex As Exception
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try
    End Sub
End Class
