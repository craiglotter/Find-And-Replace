Imports System.IO
Imports System.Text


Public Class Worker

    Inherits System.ComponentModel.Component

    ' Declares the variables you will use to hold your thread objects.

    Public WorkerThread As System.Threading.Thread

    Public searchstring As String
    Public replacestring As String
    Public basefolder As String

    Public stopcheck As Boolean = False

    Public filequeue1, filequeue2, filequeue3, filequeue4, filequeue5 As String
    Public filetypes As ArrayList

    Private foldercount As Long
    Private renamecount As Long
    Private filecount As Long

    Public Event WorkerFileProcessing(ByVal filename As String, ByVal queue As Integer)
    Public Event WorkerStatusMessage(ByVal message As String, ByVal statustag As Integer)
    Public Event WorkerError(ByVal Message As Exception)
    Public Event WorkerFolderCount(ByVal Result As Long)
    Public Event WorkerFolderRename(ByVal Result As Long, ByVal oftotal As Long)
    Public Event WorkerFileCount(ByVal Result As Long)
    Public Event WorkerComplete(ByVal queue As Integer)
    Public Event WorkerStepAnnounce(ByVal stepnumber As String)
    Public Event WorkerFileRenames()
    Public Event WorkerContentChanges()





#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
        filetypes = New ArrayList
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        filetypes = New ArrayList
    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

    Private Sub Error_Handler(ByVal message As Exception)
        Try
            If (Not WorkerThread.ThreadState.ToString.IndexOf("Aborted") > -1) And (Not WorkerThread.ThreadState.ToString.IndexOf("AbortRequested") > -1) Then
                RaiseEvent WorkerError(message)
            End If
        Catch ex As Exception
            MsgBox("An error occurred in Find And Replace's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub



    Public Sub ChooseThreads(ByVal threadNumber As Integer)
        Try
            If stopcheck = False Then
                ' Determines which thread to start based on the value it receives.
                Select Case threadNumber
                    Case 1
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFolderCount_Routine)
                        WorkerThread.Start()
                        RaiseEvent WorkerStepAnnounce("1")
                    Case 2
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileCount_Routine)
                        WorkerThread.Start()
                        RaiseEvent WorkerStepAnnounce("2")
                    Case 31
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck1_Routine)
                        Try
                            WorkerThread.Start()
                        Catch ex As System.Threading.ThreadStateException
                            Error_Handler(ex)
                            WorkerThread.Abort()
                            WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck1_Routine)
                            WorkerThread.Start()
                        End Try
                        RaiseEvent WorkerStepAnnounce("3")
                    Case 32
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck2_Routine)
                        Try
                            WorkerThread.Start()
                        Catch ex As System.Threading.ThreadStateException
                            Error_Handler(ex)
                            WorkerThread.Abort()
                            WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck2_Routine)
                            WorkerThread.Start()
                        End Try
                        RaiseEvent WorkerStepAnnounce("3")
                    Case 33
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck3_Routine)
                        Try
                            WorkerThread.Start()
                        Catch ex As System.Threading.ThreadStateException
                            Error_Handler(ex)
                            WorkerThread.Abort()
                            WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck3_Routine)
                            WorkerThread.Start()
                        End Try
                        RaiseEvent WorkerStepAnnounce("3")
                    Case 34
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck4_Routine)
                        Try
                            WorkerThread.Start()
                        Catch ex As System.Threading.ThreadStateException
                            Error_Handler(ex)
                            WorkerThread.Abort()
                            WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck4_Routine)
                            WorkerThread.Start()
                        End Try
                        RaiseEvent WorkerStepAnnounce("3")
                    Case 35
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck5_Routine)
                        Try
                            WorkerThread.Start()
                        Catch ex As System.Threading.ThreadStateException
                            Error_Handler(ex)
                            WorkerThread.Abort()
                            WorkerThread = New System.Threading.Thread(AddressOf WorkerFileContentCheck5_Routine)
                            WorkerThread.Start()
                        End Try
                        RaiseEvent WorkerStepAnnounce("3")
                    Case 4
                        WorkerThread = New System.Threading.Thread(AddressOf WorkerFolderRename_Routine)
                        WorkerThread.Start()
                        RaiseEvent WorkerStepAnnounce("4")
                End Select
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

   

    Private Sub FileContentSearchAndReplace(ByVal filename As String, ByVal thread As Integer)
        Try
            If filename.StartsWith("_") = False Then
                If filetypes.Contains(filename.Substring(filename.Length - 4, 4)) = True Then
                    Dim apptorun As String = ""
                    Dim finfo As FileInfo = New FileInfo((Application.StartupPath & "\Text File String Replacer.exe").Replace("\\", "\"))
                    Dim result As String
                    If finfo.Exists = True Then
                        Dim search() As String = searchstring.Split(";;")
                        Dim replace() As String = replacestring.Split(";;")
                        Dim i As Integer = 0

                        For Each str As String In search
                            'MsgBox(search.Length & " - " & replace.Length)
                            'MsgBox(search(i) & " - " & replace(i))
                            If Not (search(i) = "" Or search(i) = Nothing) Then
                                Log_Handler(filename & """ """ & search(i) & """ """ & replace(i), thread)
                                apptorun = """" & (Application.StartupPath & "\Text File String Replacer.exe").Replace("\\", "\") & """ """ & filename & """ """ & search(i) & """ """ & replace(i) & """ ""minimal"""

                                result = DosShellCommand(apptorun)
                                'MsgBox(result)
                            End If
                            i = i + 1
                        Next
                    Else
                        result = "Failure. Script Executable cannot be found"
                    End If
                End If

                Try
                    Dim path As String
                    Dim linetocheck As String
                    Dim ext As String
                    linetocheck = filename
                    Dim c1 As FileInfo = New FileInfo(filename)
                    If c1.Exists Then
                        path = c1.DirectoryName
                        linetocheck = c1.Name



                        Dim search() As String = searchstring.Split(";;")
                        Dim replace() As String = replacestring.Split(";;")
                        Dim i As Integer = 0

                        For Each str As String In search

                            If Not (search(i) = "" Or search(i) = Nothing) Then
                                'linetocheck = linetocheck.Replace(search(i), replace(i))
                                linetocheck = FastReplace(linetocheck, search(i), replace(i))
                            End If
                            i = i + 1
                        Next



                        linetocheck = (path & "\" & linetocheck).Replace("\\", "\")
                        If Not linetocheck = filename Then
                            Dim finfo As FileInfo = New FileInfo(linetocheck)
                            If finfo.Exists = False Then

                                Dim ginfo = New FileInfo(filename)
                                If ginfo.Exists = True Then
                                    ginfo.MoveTo(linetocheck)
                                End If
                                ginfo = Nothing
                                finfo = Nothing
                            End If
                        End If
                    End If
                    c1 = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub WorkerFileContentCheck1_Routine()
        Try
            If stopcheck = False Then
                RaiseEvent WorkerFileProcessing(filequeue1, 1)
                Try
                    Dim finfo As FileInfo = New FileInfo(filequeue1)
                    If finfo.Name.StartsWith("_") = False Then
                        FileContentSearchAndReplace(filequeue1, 1)
                    End If
                    finfo = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
                RaiseEvent WorkerComplete(1)
            Else
                    Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
    Private Sub WorkerFileContentCheck2_Routine()
        Try
            If stopcheck = False Then
                RaiseEvent WorkerFileProcessing(filequeue2, 2)
                Try
                    Dim finfo As FileInfo = New FileInfo(filequeue2)
                    If finfo.Name.StartsWith("_") = False Then
                        FileContentSearchAndReplace(filequeue2, 2)
                    End If
                    finfo = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
                RaiseEvent WorkerComplete(2)
            Else
                    Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
    Private Sub WorkerFileContentCheck3_Routine()
        Try
            If stopcheck = False Then
                RaiseEvent WorkerFileProcessing(filequeue3, 3)
                Try
                    Dim finfo As FileInfo = New FileInfo(filequeue3)
                    If finfo.Name.StartsWith("_") = False Then
                        FileContentSearchAndReplace(filequeue3, 3)
                    End If
                    finfo = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
                RaiseEvent WorkerComplete(3)
            Else
                    Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
    Private Sub WorkerFileContentCheck4_Routine()
        Try
            If stopcheck = False Then
                RaiseEvent WorkerFileProcessing(filequeue4, 4)
                Try
                    Dim finfo As FileInfo = New FileInfo(filequeue4)
                    If finfo.Name.StartsWith("_") = False Then
                        FileContentSearchAndReplace(filequeue4, 4)
                    End If
                    finfo = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
                RaiseEvent WorkerComplete(4)
            Else
                    Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
    Private Sub WorkerFileContentCheck5_Routine()
        Try
            If stopcheck = False Then
                RaiseEvent WorkerFileProcessing(filequeue5, 5)
                Try
                    Dim finfo As FileInfo = New FileInfo(filequeue5)
                    If finfo.Name.StartsWith("_") = False Then
                        FileContentSearchAndReplace(filequeue5, 5)
                    End If
                    finfo = Nothing
                Catch ex As Exception
                    Error_Handler(ex)
                End Try
                RaiseEvent WorkerComplete(5)
            Else
                    Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub WorkerFolderCount_Routine()
        RaiseEvent WorkerStatusMessage("Running Folder Count", 1)
        foldercount = 0
        RaiseEvent WorkerFolderCount(foldercount)
        Try
            If stopcheck = False Then
                FolderCountRunner(basefolder)
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
        RaiseEvent WorkerFolderCount(foldercount)
        RaiseEvent WorkerStatusMessage("", 2)
        RaiseEvent WorkerStatusMessage("Folder Count Completed", 1)
        RaiseEvent WorkerComplete(0)
    End Sub

    Private Sub WorkerFileCount_Routine()
        RaiseEvent WorkerStatusMessage("Running File Count", 1)
        filecount = 0
        RaiseEvent WorkerFileCount(filecount)
        Try
            If stopcheck = False Then
                FileCountRunner(basefolder)
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
        RaiseEvent WorkerFileCount(filecount)
        RaiseEvent WorkerStatusMessage("", 2)
        RaiseEvent WorkerStatusMessage("File Count Completed", 1)
        RaiseEvent WorkerComplete(0)
    End Sub


    Private Sub FolderCountRunner(ByVal targetDirectory As String)
        Dim tinfo As DirectoryInfo
        Dim dinfo As DirectoryInfo = New DirectoryInfo(targetDirectory)
        If dinfo.Name.StartsWith("_") = False Then
            RaiseEvent WorkerStatusMessage("Examining: " & targetDirectory, 2)
            RaiseEvent WorkerFolderCount(foldercount)
            Try
                Dim subdirectoryEntries As String() = Directory.GetDirectories(targetDirectory)
                Dim subdirectory As String
                For Each subdirectory In subdirectoryEntries
                    If stopcheck = False Then
                        tinfo = New DirectoryInfo(subdirectory)
                        If tinfo.Name.StartsWith("_") = False Then
                            foldercount = foldercount + 1
                            tinfo = Nothing
                            FolderCountRunner(subdirectory)
                        Else
                            tinfo = Nothing
                        End If
                    Else
                        Exit Sub
                    End If
                Next subdirectory
            Catch ex As Exception
                Error_Handler(ex)
            End Try
        End If
        dinfo = Nothing
    End Sub

    Private Sub FileCountRunner(ByVal targetDirectory As String)
        Try
            Dim tinfo As DirectoryInfo
            Dim dinfo As DirectoryInfo = New DirectoryInfo(targetDirectory)
            If dinfo.Name.StartsWith("_") = False Then
                Dim fileEntries As String() = Directory.GetFiles(targetDirectory)
                Dim fileName As String
                For Each fileName In fileEntries
                    If stopcheck = False Then
                        Dim finfo As FileInfo = New FileInfo(fileName)
                        If finfo.Name.StartsWith("_") = False Then
                            filecount = filecount + 1
                            RaiseEvent WorkerStatusMessage("Examining: " & fileName, 2)
                            RaiseEvent WorkerFileCount(filecount)
                            finfo = Nothing
                        Else
                            finfo = Nothing
                        End If
                    Else
                        Exit Sub
                    End If
                Next fileName

                Dim subdirectoryEntries As String() = Directory.GetDirectories(targetDirectory)
                Dim subdirectory As String
                For Each subdirectory In subdirectoryEntries
                    If stopcheck = False Then
                        tinfo = New DirectoryInfo(subdirectory)
                        If tinfo.Name.StartsWith("_") = False Then
                            tinfo = Nothing
                            FileCountRunner(subdirectory)
                        Else
                            tinfo = Nothing
                        End If
                    Else
                        Exit Sub
                    End If
                Next subdirectory
            End If
            dinfo = Nothing
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub 'ProcessDirectory

    Private Function DosShellCommand(ByVal AppToRun As String) As String
        Dim s As String = ""
        Try
            Dim myProcess As Process = New Process

            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.UseShellExecute = False

            Dim sErr As StreamReader
            Dim sOut As StreamReader
            Dim sIn As StreamWriter


            myProcess.StartInfo.CreateNoWindow = True

            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            myProcess.StartInfo.FileName = AppToRun

            myProcess.Start()
            sIn = myProcess.StandardInput
            sIn.AutoFlush = True

            sOut = myProcess.StandardOutput()
            sErr = myProcess.StandardError

            sIn.Write(AppToRun & System.Environment.NewLine)
            sIn.Write("exit" & System.Environment.NewLine)
            s = sOut.ReadToEnd()

            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If



            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()


        Catch ex As Exception
            Error_Handler(ex)
        End Try
        Return s
    End Function


    Private Sub Log_Handler(ByVal identifier_msg As String, ByVal thread As Integer)
        Try
            Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs")
            If dir.Exists = False Then
                dir.Create()
            End If
            Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs\" & Format(Now(), "yyyyMMdd") & "_Activity_Log_Thread_" & thread & ".txt", True)

            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy HH:mm:ss") & " - " & identifier_msg)


            filewriter.Flush()
            filewriter.Close()

        Catch exc As Exception
            'Error_Handler(exc, "Activity Logger")
            Error_Handler(exc)
        End Try
    End Sub

    Private Function FastReplace(ByVal Expr As String, ByVal Find As String, ByVal Replacement As String) As String



        Dim builder As System.Text.StringBuilder
        Dim upCaseExpr, upCaseFind As String
        Dim lenOfFind, lenOfReplace As Integer
        Dim currentIndex, prevIndex As Integer

        builder = New System.Text.StringBuilder
        Try
            upCaseExpr = Expr.ToUpper()
            upCaseFind = Find.ToUpper()
            lenOfFind = Find.Length
            lenOfReplace = Replacement.Length
            currentIndex = upCaseExpr.IndexOf(upCaseFind, 0)
            lenOfReplace = 0
            Do While currentIndex >= 0
                'finds = finds + 1
                builder.Append(Expr, prevIndex, currentIndex - prevIndex)
                builder.Append(Replacement)
                prevIndex = currentIndex + lenOfFind
                currentIndex = upCaseExpr.IndexOf(upCaseFind, prevIndex)
            Loop
            If prevIndex < Expr.Length Then
                builder.Append(Expr, prevIndex, Expr.Length - prevIndex)
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
        Return builder.ToString()
    End Function

    Private Sub WorkerFolderRename_Routine()
        RaiseEvent WorkerStatusMessage("Renaming Folders", 1)
        renamecount = 0
        RaiseEvent WorkerFolderRename(renamecount, foldercount)
        Try
            If stopcheck = False Then
                FolderRenameRunner(basefolder)
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
        RaiseEvent WorkerFolderRename(renamecount, foldercount)
        RaiseEvent WorkerStatusMessage("", 2)
        RaiseEvent WorkerStatusMessage("Folder Renaming Complete", 1)
        RaiseEvent WorkerComplete(0)
    End Sub

    Private Sub FolderRenameRunner(ByVal targetDirectory As String)
        Dim tinfo As DirectoryInfo = New DirectoryInfo(targetDirectory)

        Dim dinfo As DirectoryInfo
        Try
            If tinfo.Name.StartsWith("_") = False Then
                Dim subdirectoryEntries As String() = Directory.GetDirectories(targetDirectory)
                Dim subdirectory As String
                For Each subdirectory In subdirectoryEntries

                    If stopcheck = False Then
                        dinfo = New DirectoryInfo(subdirectory)
                        If dinfo.Name.StartsWith("_") = False Then
                            renamecount = renamecount + 1
                            dinfo = Nothing
                            FolderRenameRunner(subdirectory)
                        Else
                            dinfo = Nothing
                        End If
                    Else
                        Exit Sub
                    End If

                Next subdirectory
                RaiseEvent WorkerStatusMessage("Examining: " & targetDirectory, 2)
                Dim dir As DirectoryInfo = New DirectoryInfo(targetDirectory)

                Dim dirparent As String = dir.Parent.FullName
                Dim dirname As String = dir.Name
                Dim newname = dirname


                Dim search() As String = searchstring.Split(";;")
                Dim replace() As String = replacestring.Split(";;")
                Dim i As Integer = 0

                For Each str As String In search

                    If Not (search(i) = "" Or search(i) = Nothing) Then
                        'linetocheck = linetocheck.Replace(search(i), replace(i))
                        newname = FastReplace(newname, search(i), replace(i))
                    End If
                    i = i + 1
                Next

                newname = (dirparent & "\" & newname).Replace("\\", "\")
                If Not newname = targetDirectory Then
                    dir.MoveTo(newname)
                End If

            End If
            tinfo = Nothing
            RaiseEvent WorkerFolderRename(renamecount, foldercount)

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub
End Class
