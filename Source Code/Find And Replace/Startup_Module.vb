Imports System.Diagnostics
Imports System.IO

Module Startup_Module

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message("The Application encountered the following problem: " & vbCrLf & ex.ToString)
                Display_Message1.ShowDialog()
                Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy HH:mm:ss") & " - " & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
            End If
        Catch exc As Exception

            MsgBox("An error occurred in Find And Replace's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Sub main()
        Dim ApplicationName As String
        ApplicationName = "Find And Replace"
        Try
            Dim aModuleName As String = Diagnostics.Process.GetCurrentProcess.MainModule.ModuleName
            Dim aProcName As String = System.IO.Path.GetFileNameWithoutExtension(aModuleName)

            If Process.GetProcessesByName(aProcName).Length > 3 Then
                Dim Display_Message1 As New Display_Message("Another Instance of " & ApplicationName & " is already running. Only three instances of " & ApplicationName & " is allowed to run at any time")
                Display_Message1.ShowDialog()
                Application.Exit()
            Else
                Dim Splash As New Splash_Screen
                Splash.Show()
                While Splash.dataloaded = False
                End While
                Dim monitor As New Main_Screen(Splash)
                monitor.ShowDialog()
                monitor.Visible = False
                While monitor.dataloaded = False
                End While
                monitor.Visible = True
                Application.Exit()
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
End Module
