Imports System.IO
Imports Microsoft.Win32

Public Class Main_Screen
    Inherits System.Windows.Forms.Form

    Dim WithEvents Worker1 As Worker

    Private workerbusy As Boolean = False
    Private steps As Integer = 0
    'steps 0: process not launched
    'steps 1: folder count
    'steps 2: file count (and file queue creation)
    'steps 3: file search and replace operation
    'steps 4: folder renaming

    Private thread1snapshot, thread2snapshot, thread3snapshot, thread4snapshot, thread5snapshot As Long


    Private queueselector As Integer = 1
    Private filequeue1, filequeue2, filequeue3, filequeue4, filequeue5 As Queue

    Public Delegate Sub WorkerComplete_h()
    Public Delegate Sub WorkerError_h(ByVal Message As Exception)
    Public Delegate Sub WorkerFolderCount_h(ByVal Result As Long)
    Public Delegate Sub WorkerFolderRename_h(ByVal Result As Long)
    Public Delegate Sub WorkerFileCount_h(ByVal Result As Long)
    Public Delegate Sub WorkerStatusMessage_h(ByVal message As String, ByVal statustag As Integer)
    Public Delegate Sub WorkerFileProcessing_h(ByVal filename As String, ByVal queue As Integer)
    Public Delegate Sub WorkerStepAnnounce_h(ByVal stepnumber As String)

    Public Delegate Sub WorkerContentChanges_h()
    Public Delegate Sub WorkerFileRenames_h()

    Private filetypes As ArrayList

    Private frmsearchstring, frmreplacestring, frmbasefolder As String

    Private splash_loader As Splash_Screen
    Public dataloaded As Boolean = False



#Region " Windows Form Designer generated code "

    Public Sub New(ByVal splash As Splash_Screen)
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        splash_loader = splash
        Worker1 = New Worker
        AddHandler Worker1.WorkerComplete, AddressOf WorkerCompleteHandler
        AddHandler Worker1.WorkerError, AddressOf WorkerErrorHandler
        AddHandler Worker1.WorkerFolderCount, AddressOf WorkerFolderCountHandler
        AddHandler Worker1.WorkerFolderRename, AddressOf WorkerFolderRenameHandler
        AddHandler Worker1.WorkerFileCount, AddressOf WorkerFileCountHandler
        AddHandler Worker1.WorkerStatusMessage, AddressOf WorkerStatusMessageHandler
        AddHandler Worker1.WorkerFileProcessing, AddressOf WorkerFileProcessingHandler
        AddHandler Worker1.WorkerStepAnnounce, AddressOf WorkerStepAnnounceHandler
        AddHandler Worker1.WorkerContentChanges, AddressOf WorkerContentChangesHandler
        AddHandler Worker1.WorkerFileRenames, AddressOf WorkerFileRenamesHandler

        filequeue1 = New Queue
        filequeue2 = New Queue
        filequeue3 = New Queue
        filequeue4 = New Queue
        filequeue5 = New Queue

        filetypes = New ArrayList

    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Worker1 = New Worker
        AddHandler Worker1.WorkerComplete, AddressOf WorkerCompleteHandler
        AddHandler Worker1.WorkerError, AddressOf WorkerErrorHandler
        AddHandler Worker1.WorkerFolderCount, AddressOf WorkerFolderCountHandler
        AddHandler Worker1.WorkerFolderRename, AddressOf WorkerFolderRenameHandler
        AddHandler Worker1.WorkerFileCount, AddressOf WorkerFileCountHandler
        AddHandler Worker1.WorkerStatusMessage, AddressOf WorkerStatusMessageHandler
        AddHandler Worker1.WorkerFileProcessing, AddressOf WorkerFileProcessingHandler
        AddHandler Worker1.WorkerStepAnnounce, AddressOf WorkerStepAnnounceHandler
        AddHandler Worker1.WorkerContentChanges, AddressOf WorkerContentChangesHandler
        AddHandler Worker1.WorkerFileRenames, AddressOf WorkerFileRenamesHandler

        filequeue1 = New Queue
        filequeue2 = New Queue
        filequeue3 = New Queue
        filequeue4 = New Queue
        filequeue5 = New Queue

        filetypes = New ArrayList


    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents txtSearchString As System.Windows.Forms.TextBox
    Friend WithEvents txtReplaceString As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ButtonFolderBrowse As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtBaseFolder As System.Windows.Forms.TextBox
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ButtonOperationLaunch As System.Windows.Forms.Button
    Friend WithEvents txtStatus As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents step1 As System.Windows.Forms.Label
    Friend WithEvents step2 As System.Windows.Forms.Label
    Friend WithEvents step3 As System.Windows.Forms.Label
    Friend WithEvents step4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtFolderRenames As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueueTotal5 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueueTotal4 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueueTotal3 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueueTotal2 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueueTotal1 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueue5 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueue4 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueue3 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueue2 As System.Windows.Forms.Label
    Friend WithEvents txtFileQueue1 As System.Windows.Forms.Label
    Friend WithEvents txtStatusBar5 As System.Windows.Forms.TextBox
    Friend WithEvents txtStatusBar4 As System.Windows.Forms.TextBox
    Friend WithEvents txtStatusBar3 As System.Windows.Forms.TextBox
    Friend WithEvents txtStatusBar2 As System.Windows.Forms.TextBox
    Friend WithEvents txtStatusBar1 As System.Windows.Forms.TextBox
    Friend WithEvents txtFileCount As System.Windows.Forms.Label
    Friend WithEvents txtFolderCount As System.Windows.Forms.Label
    Friend WithEvents txtProcessLaunched As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtContentChanges As System.Windows.Forms.Label
    Friend WithEvents txtFileRenames As System.Windows.Forms.Label
    Friend WithEvents txtProcessEnded As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents Button2 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(Main_Screen))
        Me.txtSearchString = New System.Windows.Forms.TextBox
        Me.txtReplaceString = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtBaseFolder = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.ButtonFolderBrowse = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ButtonOperationLaunch = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.txtStatus = New System.Windows.Forms.TextBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.step1 = New System.Windows.Forms.Label
        Me.step2 = New System.Windows.Forms.Label
        Me.step3 = New System.Windows.Forms.Label
        Me.step4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtFolderRenames = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtFileQueueTotal5 = New System.Windows.Forms.Label
        Me.txtFileQueueTotal4 = New System.Windows.Forms.Label
        Me.txtFileQueueTotal3 = New System.Windows.Forms.Label
        Me.txtFileQueueTotal2 = New System.Windows.Forms.Label
        Me.txtFileQueueTotal1 = New System.Windows.Forms.Label
        Me.txtFileQueue5 = New System.Windows.Forms.Label
        Me.txtFileQueue4 = New System.Windows.Forms.Label
        Me.txtFileQueue3 = New System.Windows.Forms.Label
        Me.txtFileQueue2 = New System.Windows.Forms.Label
        Me.txtFileQueue1 = New System.Windows.Forms.Label
        Me.txtStatusBar5 = New System.Windows.Forms.TextBox
        Me.txtStatusBar4 = New System.Windows.Forms.TextBox
        Me.txtStatusBar3 = New System.Windows.Forms.TextBox
        Me.txtStatusBar2 = New System.Windows.Forms.TextBox
        Me.txtStatusBar1 = New System.Windows.Forms.TextBox
        Me.txtFileCount = New System.Windows.Forms.Label
        Me.txtFolderCount = New System.Windows.Forms.Label
        Me.txtProcessLaunched = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Label13 = New System.Windows.Forms.Label
        Me.txtContentChanges = New System.Windows.Forms.Label
        Me.txtFileRenames = New System.Windows.Forms.Label
        Me.txtProcessEnded = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Button2 = New System.Windows.Forms.Button
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtSearchString
        '
        Me.txtSearchString.BackColor = System.Drawing.Color.White
        Me.txtSearchString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtSearchString.ForeColor = System.Drawing.Color.Black
        Me.txtSearchString.Location = New System.Drawing.Point(8, 56)
        Me.txtSearchString.Name = "txtSearchString"
        Me.txtSearchString.Size = New System.Drawing.Size(288, 20)
        Me.txtSearchString.TabIndex = 0
        Me.txtSearchString.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtSearchString, "The search string searched for during the Find And Replace operation")
        '
        'txtReplaceString
        '
        Me.txtReplaceString.BackColor = System.Drawing.Color.White
        Me.txtReplaceString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtReplaceString.ForeColor = System.Drawing.Color.Black
        Me.txtReplaceString.Location = New System.Drawing.Point(8, 96)
        Me.txtReplaceString.Name = "txtReplaceString"
        Me.txtReplaceString.Size = New System.Drawing.Size(288, 20)
        Me.txtReplaceString.TabIndex = 1
        Me.txtReplaceString.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtReplaceString, "The replace string used during the Find And Replace operation")
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(8, 80)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "SEARCH STRING"
        '
        'txtBaseFolder
        '
        Me.txtBaseFolder.BackColor = System.Drawing.Color.White
        Me.txtBaseFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtBaseFolder.ForeColor = System.Drawing.Color.Black
        Me.txtBaseFolder.Location = New System.Drawing.Point(8, 136)
        Me.txtBaseFolder.Name = "txtBaseFolder"
        Me.txtBaseFolder.ReadOnly = True
        Me.txtBaseFolder.Size = New System.Drawing.Size(224, 20)
        Me.txtBaseFolder.TabIndex = 4
        Me.txtBaseFolder.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtBaseFolder, "The base folder from which the search is initiated during the Find And Replace op" & _
        "eration")
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(8, 120)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 16)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "REPLACE STRING"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(8, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(280, 32)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "ENTER THE DESIRED SEARCH AND REPLACE STRINGS, SELECT THE OPERATION BASE FOLDER AN" & _
        "D CLICK ON THE PROCESS BUTTON TO LAUNCH THE FIND AND REPLACE OPERATION.  (DELIMI" & _
        "TER: ';;')"
        '
        'ButtonFolderBrowse
        '
        Me.ButtonFolderBrowse.BackColor = System.Drawing.Color.Orchid
        Me.ButtonFolderBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonFolderBrowse.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonFolderBrowse.Location = New System.Drawing.Point(240, 136)
        Me.ButtonFolderBrowse.Name = "ButtonFolderBrowse"
        Me.ButtonFolderBrowse.Size = New System.Drawing.Size(56, 20)
        Me.ButtonFolderBrowse.TabIndex = 7
        Me.ButtonFolderBrowse.Text = "BROWSE"
        Me.ToolTip1.SetToolTip(Me.ButtonFolderBrowse, "Launches the Folder Browser Dialog")
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(8, 160)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(136, 16)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "BASE FOLDER"
        '
        'FolderBrowserDialog1
        '
        Me.FolderBrowserDialog1.Description = "Select the base folder from which the search is initiated during the Find And Rep" & _
        "lace operation"
        Me.FolderBrowserDialog1.ShowNewFolderButton = False
        '
        'ButtonOperationLaunch
        '
        Me.ButtonOperationLaunch.BackColor = System.Drawing.Color.Orchid
        Me.ButtonOperationLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonOperationLaunch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonOperationLaunch.Location = New System.Drawing.Point(104, 182)
        Me.ButtonOperationLaunch.Name = "ButtonOperationLaunch"
        Me.ButtonOperationLaunch.Size = New System.Drawing.Size(88, 20)
        Me.ButtonOperationLaunch.TabIndex = 10
        Me.ButtonOperationLaunch.Text = "Process"
        Me.ToolTip1.SetToolTip(Me.ButtonOperationLaunch, "Launches Find and Replace Operation")
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Orchid
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(640, 176)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(88, 18)
        Me.Button1.TabIndex = 37
        Me.Button1.Text = "KILL PROCESSES"
        Me.ToolTip1.SetToolTip(Me.Button1, "Launches the Folder Browser Dialog")
        '
        'txtStatus
        '
        Me.txtStatus.BackColor = System.Drawing.Color.Plum
        Me.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatus.ForeColor = System.Drawing.Color.White
        Me.txtStatus.Location = New System.Drawing.Point(320, 368)
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.ReadOnly = True
        Me.txtStatus.Size = New System.Drawing.Size(416, 13)
        Me.txtStatus.TabIndex = 15
        Me.txtStatus.Text = ""
        Me.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 5000
        '
        'ListBox1
        '
        Me.ListBox1.BackColor = System.Drawing.Color.Thistle
        Me.ListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ListBox1.ForeColor = System.Drawing.Color.DimGray
        Me.ListBox1.Location = New System.Drawing.Point(640, 40)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(88, 132)
        Me.ListBox1.TabIndex = 31
        '
        'step1
        '
        Me.step1.ForeColor = System.Drawing.Color.Gray
        Me.step1.Location = New System.Drawing.Point(8, 224)
        Me.step1.Name = "step1"
        Me.step1.Size = New System.Drawing.Size(184, 16)
        Me.step1.TabIndex = 32
        Me.step1.Text = "Step 1: Retrieve Folder List"
        '
        'step2
        '
        Me.step2.ForeColor = System.Drawing.Color.Gray
        Me.step2.Location = New System.Drawing.Point(8, 240)
        Me.step2.Name = "step2"
        Me.step2.Size = New System.Drawing.Size(184, 16)
        Me.step2.TabIndex = 33
        Me.step2.Text = "Step 2: Retrieve File List"
        '
        'step3
        '
        Me.step3.ForeColor = System.Drawing.Color.Gray
        Me.step3.Location = New System.Drawing.Point(8, 256)
        Me.step3.Name = "step3"
        Me.step3.Size = New System.Drawing.Size(216, 16)
        Me.step3.TabIndex = 34
        Me.step3.Text = "Step 3: Process File Names and Content"
        '
        'step4
        '
        Me.step4.ForeColor = System.Drawing.Color.Gray
        Me.step4.Location = New System.Drawing.Point(8, 272)
        Me.step4.Name = "step4"
        Me.step4.Size = New System.Drawing.Size(184, 16)
        Me.step4.TabIndex = 35
        Me.step4.Text = "Step 4: Process Folder Names"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.Color.Thistle
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label5.Location = New System.Drawing.Point(8, 176)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(288, 32)
        Me.Label5.TabIndex = 36
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(640, 16)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(104, 16)
        Me.Label6.TabIndex = 38
        Me.Label6.Text = "FILE TYPES TO BE SCANNED"
        '
        'txtFolderRenames
        '
        Me.txtFolderRenames.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFolderRenames.ForeColor = System.Drawing.Color.Gray
        Me.txtFolderRenames.Location = New System.Drawing.Point(320, 208)
        Me.txtFolderRenames.Name = "txtFolderRenames"
        Me.txtFolderRenames.Size = New System.Drawing.Size(216, 16)
        Me.txtFolderRenames.TabIndex = 45
        Me.txtFolderRenames.Text = "Folder Renames:"
        Me.txtFolderRenames.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Gray
        Me.Label12.Location = New System.Drawing.Point(320, 112)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(192, 16)
        Me.Label12.TabIndex = 44
        Me.Label12.Text = "File Scan Progress:"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.Gray
        Me.Label11.Location = New System.Drawing.Point(336, 192)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(64, 16)
        Me.Label11.TabIndex = 43
        Me.Label11.Text = "Thread 5:"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.Gray
        Me.Label10.Location = New System.Drawing.Point(336, 176)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(64, 16)
        Me.Label10.TabIndex = 42
        Me.Label10.Text = "Thread 4:"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Gray
        Me.Label9.Location = New System.Drawing.Point(336, 160)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(64, 16)
        Me.Label9.TabIndex = 41
        Me.Label9.Text = "Thread 3:"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label8
        '
        Me.Label8.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Gray
        Me.Label8.Location = New System.Drawing.Point(336, 144)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(64, 16)
        Me.Label8.TabIndex = 40
        Me.Label8.Text = "Thread 2:"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.Gray
        Me.Label7.Location = New System.Drawing.Point(336, 128)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(64, 16)
        Me.Label7.TabIndex = 39
        Me.Label7.Text = "Thread 1:"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueueTotal5
        '
        Me.txtFileQueueTotal5.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueueTotal5.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueueTotal5.Location = New System.Drawing.Point(440, 192)
        Me.txtFileQueueTotal5.Name = "txtFileQueueTotal5"
        Me.txtFileQueueTotal5.Size = New System.Drawing.Size(80, 16)
        Me.txtFileQueueTotal5.TabIndex = 30
        Me.txtFileQueueTotal5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueueTotal4
        '
        Me.txtFileQueueTotal4.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueueTotal4.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueueTotal4.Location = New System.Drawing.Point(440, 176)
        Me.txtFileQueueTotal4.Name = "txtFileQueueTotal4"
        Me.txtFileQueueTotal4.Size = New System.Drawing.Size(80, 16)
        Me.txtFileQueueTotal4.TabIndex = 29
        Me.txtFileQueueTotal4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueueTotal3
        '
        Me.txtFileQueueTotal3.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueueTotal3.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueueTotal3.Location = New System.Drawing.Point(440, 160)
        Me.txtFileQueueTotal3.Name = "txtFileQueueTotal3"
        Me.txtFileQueueTotal3.Size = New System.Drawing.Size(80, 16)
        Me.txtFileQueueTotal3.TabIndex = 28
        Me.txtFileQueueTotal3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueueTotal2
        '
        Me.txtFileQueueTotal2.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueueTotal2.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueueTotal2.Location = New System.Drawing.Point(440, 144)
        Me.txtFileQueueTotal2.Name = "txtFileQueueTotal2"
        Me.txtFileQueueTotal2.Size = New System.Drawing.Size(80, 16)
        Me.txtFileQueueTotal2.TabIndex = 27
        Me.txtFileQueueTotal2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueueTotal1
        '
        Me.txtFileQueueTotal1.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueueTotal1.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueueTotal1.Location = New System.Drawing.Point(440, 128)
        Me.txtFileQueueTotal1.Name = "txtFileQueueTotal1"
        Me.txtFileQueueTotal1.Size = New System.Drawing.Size(80, 16)
        Me.txtFileQueueTotal1.TabIndex = 26
        Me.txtFileQueueTotal1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFileQueue5
        '
        Me.txtFileQueue5.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueue5.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueue5.Location = New System.Drawing.Point(400, 192)
        Me.txtFileQueue5.Name = "txtFileQueue5"
        Me.txtFileQueue5.Size = New System.Drawing.Size(40, 16)
        Me.txtFileQueue5.TabIndex = 25
        Me.txtFileQueue5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileQueue4
        '
        Me.txtFileQueue4.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueue4.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueue4.Location = New System.Drawing.Point(400, 176)
        Me.txtFileQueue4.Name = "txtFileQueue4"
        Me.txtFileQueue4.Size = New System.Drawing.Size(40, 16)
        Me.txtFileQueue4.TabIndex = 24
        Me.txtFileQueue4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileQueue3
        '
        Me.txtFileQueue3.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueue3.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueue3.Location = New System.Drawing.Point(400, 160)
        Me.txtFileQueue3.Name = "txtFileQueue3"
        Me.txtFileQueue3.Size = New System.Drawing.Size(40, 16)
        Me.txtFileQueue3.TabIndex = 23
        Me.txtFileQueue3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileQueue2
        '
        Me.txtFileQueue2.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueue2.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueue2.Location = New System.Drawing.Point(400, 144)
        Me.txtFileQueue2.Name = "txtFileQueue2"
        Me.txtFileQueue2.Size = New System.Drawing.Size(40, 16)
        Me.txtFileQueue2.TabIndex = 22
        Me.txtFileQueue2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileQueue1
        '
        Me.txtFileQueue1.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileQueue1.ForeColor = System.Drawing.Color.Gray
        Me.txtFileQueue1.Location = New System.Drawing.Point(400, 128)
        Me.txtFileQueue1.Name = "txtFileQueue1"
        Me.txtFileQueue1.Size = New System.Drawing.Size(40, 16)
        Me.txtFileQueue1.TabIndex = 21
        Me.txtFileQueue1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtStatusBar5
        '
        Me.txtStatusBar5.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtStatusBar5.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatusBar5.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStatusBar5.ForeColor = System.Drawing.Color.Gray
        Me.txtStatusBar5.Location = New System.Drawing.Point(320, 272)
        Me.txtStatusBar5.Name = "txtStatusBar5"
        Me.txtStatusBar5.ReadOnly = True
        Me.txtStatusBar5.Size = New System.Drawing.Size(296, 10)
        Me.txtStatusBar5.TabIndex = 20
        Me.txtStatusBar5.Text = ""
        '
        'txtStatusBar4
        '
        Me.txtStatusBar4.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtStatusBar4.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatusBar4.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStatusBar4.ForeColor = System.Drawing.Color.Gray
        Me.txtStatusBar4.Location = New System.Drawing.Point(320, 288)
        Me.txtStatusBar4.Name = "txtStatusBar4"
        Me.txtStatusBar4.ReadOnly = True
        Me.txtStatusBar4.Size = New System.Drawing.Size(296, 10)
        Me.txtStatusBar4.TabIndex = 19
        Me.txtStatusBar4.Text = ""
        '
        'txtStatusBar3
        '
        Me.txtStatusBar3.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtStatusBar3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatusBar3.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStatusBar3.ForeColor = System.Drawing.Color.Gray
        Me.txtStatusBar3.Location = New System.Drawing.Point(320, 304)
        Me.txtStatusBar3.Name = "txtStatusBar3"
        Me.txtStatusBar3.ReadOnly = True
        Me.txtStatusBar3.Size = New System.Drawing.Size(296, 10)
        Me.txtStatusBar3.TabIndex = 18
        Me.txtStatusBar3.Text = ""
        '
        'txtStatusBar2
        '
        Me.txtStatusBar2.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtStatusBar2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatusBar2.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStatusBar2.ForeColor = System.Drawing.Color.Gray
        Me.txtStatusBar2.Location = New System.Drawing.Point(320, 320)
        Me.txtStatusBar2.Name = "txtStatusBar2"
        Me.txtStatusBar2.ReadOnly = True
        Me.txtStatusBar2.Size = New System.Drawing.Size(296, 10)
        Me.txtStatusBar2.TabIndex = 17
        Me.txtStatusBar2.Text = ""
        '
        'txtStatusBar1
        '
        Me.txtStatusBar1.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtStatusBar1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatusBar1.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStatusBar1.ForeColor = System.Drawing.Color.Gray
        Me.txtStatusBar1.Location = New System.Drawing.Point(320, 336)
        Me.txtStatusBar1.Name = "txtStatusBar1"
        Me.txtStatusBar1.ReadOnly = True
        Me.txtStatusBar1.Size = New System.Drawing.Size(296, 10)
        Me.txtStatusBar1.TabIndex = 14
        Me.txtStatusBar1.Text = ""
        '
        'txtFileCount
        '
        Me.txtFileCount.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileCount.ForeColor = System.Drawing.Color.Gray
        Me.txtFileCount.Location = New System.Drawing.Point(320, 96)
        Me.txtFileCount.Name = "txtFileCount"
        Me.txtFileCount.Size = New System.Drawing.Size(192, 16)
        Me.txtFileCount.TabIndex = 16
        Me.txtFileCount.Text = "File Count:"
        Me.txtFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtFolderCount
        '
        Me.txtFolderCount.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFolderCount.ForeColor = System.Drawing.Color.Gray
        Me.txtFolderCount.Location = New System.Drawing.Point(320, 80)
        Me.txtFolderCount.Name = "txtFolderCount"
        Me.txtFolderCount.Size = New System.Drawing.Size(192, 16)
        Me.txtFolderCount.TabIndex = 13
        Me.txtFolderCount.Text = "Folder Count:"
        Me.txtFolderCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtProcessLaunched
        '
        Me.txtProcessLaunched.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtProcessLaunched.ForeColor = System.Drawing.Color.Gray
        Me.txtProcessLaunched.Location = New System.Drawing.Point(320, 64)
        Me.txtProcessLaunched.Name = "txtProcessLaunched"
        Me.txtProcessLaunched.Size = New System.Drawing.Size(192, 16)
        Me.txtProcessLaunched.TabIndex = 12
        Me.txtProcessLaunched.Text = "Launched:"
        Me.txtProcessLaunched.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label13)
        Me.Panel1.ForeColor = System.Drawing.Color.Gray
        Me.Panel1.Location = New System.Drawing.Point(312, 16)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(312, 344)
        Me.Panel1.TabIndex = 46
        '
        'Label13
        '
        Me.Label13.BackColor = System.Drawing.Color.Thistle
        Me.Label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(8, 8)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(296, 24)
        Me.Label13.TabIndex = 0
        Me.Label13.Text = "Operation Details"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtContentChanges
        '
        Me.txtContentChanges.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtContentChanges.ForeColor = System.Drawing.Color.Gray
        Me.txtContentChanges.Location = New System.Drawing.Point(530, 150)
        Me.txtContentChanges.Name = "txtContentChanges"
        Me.txtContentChanges.Size = New System.Drawing.Size(62, 16)
        Me.txtContentChanges.TabIndex = 48
        Me.txtContentChanges.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileRenames
        '
        Me.txtFileRenames.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtFileRenames.ForeColor = System.Drawing.Color.Gray
        Me.txtFileRenames.Location = New System.Drawing.Point(536, 168)
        Me.txtFileRenames.Name = "txtFileRenames"
        Me.txtFileRenames.Size = New System.Drawing.Size(62, 16)
        Me.txtFileRenames.TabIndex = 49
        Me.txtFileRenames.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtProcessEnded
        '
        Me.txtProcessEnded.BackColor = System.Drawing.Color.FromArgb(CType(224, Byte), CType(224, Byte), CType(224, Byte))
        Me.txtProcessEnded.ForeColor = System.Drawing.Color.Gray
        Me.txtProcessEnded.Location = New System.Drawing.Point(320, 224)
        Me.txtProcessEnded.Name = "txtProcessEnded"
        Me.txtProcessEnded.Size = New System.Drawing.Size(192, 16)
        Me.txtProcessEnded.TabIndex = 13
        Me.txtProcessEnded.Text = "Ended:"
        Me.txtProcessEnded.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.ForeColor = System.Drawing.Color.Thistle
        Me.Label14.Location = New System.Drawing.Point(8, 368)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(256, 16)
        Me.Label14.TabIndex = 50
        '
        'Timer2
        '
        Me.Timer2.Enabled = True
        Me.Timer2.Interval = 500
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Orchid
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(264, 368)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(15, 13)
        Me.Button2.TabIndex = 51
        Me.ToolTip1.SetToolTip(Me.Button2, "Force Application Status Check")
        '
        'Main_Screen
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackColor = System.Drawing.Color.Plum
        Me.ClientSize = New System.Drawing.Size(746, 392)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.txtFileRenames)
        Me.Controls.Add(Me.txtContentChanges)
        Me.Controls.Add(Me.txtFolderRenames)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.step4)
        Me.Controls.Add(Me.step3)
        Me.Controls.Add(Me.step2)
        Me.Controls.Add(Me.step1)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.txtFileQueueTotal5)
        Me.Controls.Add(Me.txtFileQueueTotal4)
        Me.Controls.Add(Me.txtFileQueueTotal3)
        Me.Controls.Add(Me.txtFileQueueTotal2)
        Me.Controls.Add(Me.txtFileQueueTotal1)
        Me.Controls.Add(Me.txtFileQueue5)
        Me.Controls.Add(Me.txtFileQueue4)
        Me.Controls.Add(Me.txtFileQueue3)
        Me.Controls.Add(Me.txtFileQueue2)
        Me.Controls.Add(Me.txtFileQueue1)
        Me.Controls.Add(Me.txtStatusBar5)
        Me.Controls.Add(Me.txtStatusBar4)
        Me.Controls.Add(Me.txtStatusBar3)
        Me.Controls.Add(Me.txtStatusBar2)
        Me.Controls.Add(Me.txtStatus)
        Me.Controls.Add(Me.txtStatusBar1)
        Me.Controls.Add(Me.txtBaseFolder)
        Me.Controls.Add(Me.txtReplaceString)
        Me.Controls.Add(Me.txtSearchString)
        Me.Controls.Add(Me.txtFileCount)
        Me.Controls.Add(Me.txtFolderCount)
        Me.Controls.Add(Me.txtProcessLaunched)
        Me.Controls.Add(Me.txtProcessEnded)
        Me.Controls.Add(Me.ButtonOperationLaunch)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.ButtonFolderBrowse)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Panel1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(752, 424)
        Me.MinimumSize = New System.Drawing.Size(752, 424)
        Me.Name = "Main_Screen"
        Me.Text = "Find And Replace (Build 20061128.4)"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                If identifier_msg.Length > 0 Then
                    identifier_msg = identifier_msg & ": "
                End If

                Dim Display_Message1 As New Display_Message("The Application encountered the following problem: " & vbCrLf & identifier_msg & ex.ToString)
                Display_Message1.ShowDialog()
                Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy HH:mm:ss") & " - " & identifier_msg & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
            End If
        Catch exc As Exception

            MsgBox("An error occurred in Find And Replace's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Error_Handler(ByVal identifier_msg As String)
        Try

            Dim Display_Message1 As New Display_Message("The Application encountered the following problem: " & vbCrLf & identifier_msg)
            Display_Message1.ShowDialog()
            Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
            If dir.Exists = False Then
                dir.Create()
            End If
            Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy HH:mm:ss") & " - " & identifier_msg)
            filewriter.Flush()
            filewriter.Close()

        Catch exc As Exception
            MsgBox("An error occurred in Find And Replace's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub


    Private Sub ButtonFolderBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonFolderBrowse.Click
        Try
            Dim result As DialogResult
            If Not txtBaseFolder.Text = "" And txtBaseFolder.Text Is Nothing = False Then


                Dim foldercheck As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(txtBaseFolder.Text)
                If foldercheck.Exists = True Then
                    FolderBrowserDialog1.SelectedPath = txtBaseFolder.Text
                End If
            End If
            result = FolderBrowserDialog1.ShowDialog()
            If result = DialogResult.OK Or result = DialogResult.Yes Then
                txtBaseFolder.Text = FolderBrowserDialog1.SelectedPath
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub




    Private Sub SendMessage(ByVal labelname As String, ByVal message As String)
        Try
            Dim controllist As ControlCollection = Me.Controls
            Dim cont As Control

            For Each cont In controllist
                If cont.Name = labelname Then
                    cont.Text = message
                    cont.Refresh()
                    Exit For
                End If
            Next
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub ButtonOperationLaunch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOperationLaunch.Click

        SendMessage("txtProcessLaunched", "Launched:")
        SendMessage("txtProcessEnded", "Ended:")
        SendMessage("txtStatus", "")
        SendMessage("txtStatusBar1", "")
        SendMessage("txtStatusBar2", "")
        SendMessage("txtStatusBar3", "")
        SendMessage("txtStatusBar4", "")
        SendMessage("txtStatusBar5", "")
        SendMessage("txtFolderCount", "Folder Count:")
        SendMessage("txtFileCount", "File Count:")
        SendMessage("txtFileQueue1", "")
        SendMessage("txtFileQueueTotal1", "")
        SendMessage("txtFileQueue2", "")
        SendMessage("txtFileQueueTotal2", "")
        SendMessage("txtFileQueue3", "")
        SendMessage("txtFileQueueTotal3", "")
        SendMessage("txtFileQueue4", "")
        SendMessage("txtFileQueueTotal4", "")
        SendMessage("txtFileQueue5", "")
        SendMessage("txtFileQueueTotal5", "")
        SendMessage("txtFolderRenames", "Folder Renames:")

        filequeue1.Clear()
        filequeue2.Clear()
        filequeue3.Clear()
        filequeue4.Clear()
        filequeue5.Clear()

        SendMessage("txtProcessLaunched", "Launched: " & Format(Now(), "dd/MM/yyyy HH:mm:ss"))

        Worker1.filetypes.Clear()
        Dim str As String
        For Each str In filetypes
            Worker1.filetypes.Add(str)
        Next
        Worker1.basefolder = txtBaseFolder.Text.Trim
        Worker1.searchstring = txtSearchString.Text
        Worker1.replacestring = txtReplaceString.Text

        Dim search() As String = txtSearchString.Text.Split(";;")
        Dim replace() As String = txtReplaceString.Text.Split(";;")
        If search.Length = replace.Length Then
            Worker1.stopcheck = False

            steps = 1
            statuslabel()
            Worker1.ChooseThreads(1)
            workerbusy = True

            ButtonOperationLaunch.Enabled = False
        Else
            MsgBox("The number of search items does not match up to the number of replace items as delimited using the ';;' delimiter string. Please go back ensure that the number of terms match up.", MsgBoxStyle.Information, "Input Error")
        End If


    End Sub

    Public Sub WorkerStepAnnounceHandler(ByVal stepnumber As String)
        Try
            Select Case stepnumber
                Case "0"
                    step1.ForeColor = Color.Gray
                    step2.ForeColor = Color.Gray
                    step3.ForeColor = Color.Gray
                    step4.ForeColor = Color.Gray
                Case "1"
                    step1.ForeColor = Color.Black
                    step2.ForeColor = Color.Gray
                    step3.ForeColor = Color.Gray
                    step4.ForeColor = Color.Gray
                Case "2"
                    step1.ForeColor = Color.Gray
                    step2.ForeColor = Color.Black
                    step3.ForeColor = Color.Gray
                    step4.ForeColor = Color.Gray
                Case "3"
                    step1.ForeColor = Color.Gray
                    step2.ForeColor = Color.Gray
                    step3.ForeColor = Color.Black
                    step4.ForeColor = Color.Gray
                Case "4"
                    step1.ForeColor = Color.Gray
                    step2.ForeColor = Color.Gray
                    step3.ForeColor = Color.Gray
                    step4.ForeColor = Color.Black
                Case "5"
                    step1.ForeColor = Color.Gray
                    step2.ForeColor = Color.Gray
                    step3.ForeColor = Color.Gray
                    step4.ForeColor = Color.Gray
            End Select
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerStatusMessageHandler(ByVal message As String, ByVal statustag As Integer)
        Try
            If statustag = 1 Then
                SendMessage("txtStatus", message)
            Else
                SendMessage("txtStatusBar1", message.Replace(txtBaseFolder.Text & "\", "...\").ToUpper)
                If steps = 2 And Not message = "" Then
                    Select Case queueselector
                        Case 1
                            filequeue1.Enqueue(message.Replace("Examining: ", ""))
                        Case 2
                            filequeue2.Enqueue(message.Replace("Examining: ", ""))
                        Case 3
                            filequeue3.Enqueue(message.Replace("Examining: ", ""))
                        Case 4
                            filequeue4.Enqueue(message.Replace("Examining: ", ""))
                        Case 5
                            filequeue5.Enqueue(message.Replace("Examining: ", ""))
                    End Select
                    queueselector = queueselector + 1
                    If queueselector > 5 Then
                        queueselector = 1
                    End If
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerErrorHandler(ByVal Message As Exception)
        Try
            Error_Handler(Message)
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerFolderCountHandler(ByVal Result As Long)
        Try
            SendMessage("txtFolderCount", "Folder Count: " & Result.ToString)
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerFolderRenameHandler(ByVal Result As Long, ByVal oftotal As Long)
        Try
            SendMessage("txtFolderRenames", "Folder Renames: " & Result.ToString & " (of " & oftotal.ToString & ")")

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerFileCountHandler(ByVal Result As Long)
        Try
            SendMessage("txtFileCount", "File Count: " & Result.ToString)
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerFileRenamesHandler()
        Try
            Dim lng As Long
            If txtFileRenames.Text = "" Then
                lng = 0
            Else
                lng = CLng(txtFileRenames.Text)
            End If
            lng = lng + 1
            SendMessage("txtFileRenames", lng.ToString)
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerContentChangesHandler()
        Try
            Dim lng As Long
            If txtContentChanges.Text = "" Then
                lng = 0
            Else
                lng = CLng(txtContentChanges.Text)
            End If
            lng = lng + 1
            SendMessage("txtContentChanges", lng.ToString)
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerCompleteHandler(ByVal queue As Integer)
        Try
            Dim eventhandled As Boolean = False
            workerbusy = False
            If steps = 1 And eventhandled = False Then
                steps = 2
                Worker1.ChooseThreads(2)
                workerbusy = True
                eventhandled = True
            End If
            If steps = 2 And eventhandled = False Then
                SendMessage("txtStatus", "Examining Files' Content")
                steps = 3
                txtFileQueue1.Text = 0
                txtFileQueue2.Text = 0
                txtFileQueue3.Text = 0
                txtFileQueue4.Text = 0
                txtFileQueue5.Text = 0
                txtFileQueueTotal1.Text = "(of " & filequeue1.Count & ")"
                txtFileQueueTotal2.Text = "(of " & filequeue2.Count & ")"
                txtFileQueueTotal3.Text = "(of " & filequeue3.Count & ")"
                txtFileQueueTotal4.Text = "(of " & filequeue4.Count & ")"
                txtFileQueueTotal5.Text = "(of " & filequeue5.Count & ")"

                thread1snapshot = 0
                thread2snapshot = 0
                thread3snapshot = 0
                thread4snapshot = 0
                thread5snapshot = 0

                If filequeue1.Count > 0 Then
                    Worker1.filequeue1 = CStr(filequeue1.Dequeue())
                    Try
                        Worker1.ChooseThreads(31)
                    Catch errt As Exception
                    End Try
                End If
                If filequeue2.Count > 0 Then
                    Worker1.filequeue2 = CStr(filequeue2.Dequeue())
                    Try
                        Worker1.ChooseThreads(32)
                    Catch errt As Exception
                    End Try
                End If
                If filequeue3.Count > 0 Then
                    Worker1.filequeue3 = CStr(filequeue3.Dequeue())
                    Try
                        Worker1.ChooseThreads(33)
                    Catch errt As Exception
                    End Try
                End If
                If filequeue4.Count > 0 Then
                    Worker1.filequeue4 = CStr(filequeue4.Dequeue())
                    Try
                        Worker1.ChooseThreads(34)
                    Catch errt As Exception
                    End Try
                End If
                If filequeue5.Count > 0 Then
                    Worker1.filequeue5 = CStr(filequeue5.Dequeue())
                    Try
                        Worker1.ChooseThreads(35)
                    Catch errt As Exception
                    End Try
                End If
                workerbusy = True
                eventhandled = True
            End If
            If steps = 3 And eventhandled = False Then
                Select Case queue
                    Case 1
                        If filequeue1.Count > 0 Then
                            Worker1.filequeue1 = CStr(filequeue1.Dequeue())
                            Try
                                Worker1.ChooseThreads(31)
                            Catch errt As Exception
                            End Try
                        End If
                    Case 2
                        If filequeue2.Count > 0 Then
                            Worker1.filequeue2 = CStr(filequeue2.Dequeue())
                            Try
                                Worker1.ChooseThreads(32)
                            Catch errt As Exception
                            End Try
                        End If
                    Case 3
                        If filequeue3.Count > 0 Then
                            Worker1.filequeue3 = CStr(filequeue3.Dequeue())
                            Try
                                Worker1.ChooseThreads(33)
                            Catch errt As Exception
                            End Try
                        End If
                    Case 4
                        If filequeue4.Count > 0 Then
                            Worker1.filequeue4 = CStr(filequeue4.Dequeue())
                            Try
                                Worker1.ChooseThreads(34)
                            Catch errt As Exception
                            End Try
                        End If
                    Case 5
                        If filequeue5.Count > 0 Then
                            Worker1.filequeue5 = CStr(filequeue5.Dequeue())
                            Try
                                Worker1.ChooseThreads(35)
                            Catch errt As Exception
                            End Try
                        End If
                End Select

                If filequeue1.Count = 0 And filequeue2.Count = 0 And filequeue3.Count = 0 And filequeue4.Count = 0 And filequeue5.Count = 0 Then
                    SendMessage("txtStatusBar1", "")
                    SendMessage("txtStatusBar2", "")
                    SendMessage("txtStatusBar3", "")
                    SendMessage("txtStatusBar4", "")
                    SendMessage("txtStatusBar5", "")
                    SendMessage("txtStatus", "Files' Content Examination Complete")
                    steps = 4
                    workerbusy = False

                Else
                    workerbusy = True
                End If
                eventhandled = True

            End If

            If steps = 4 And eventhandled = False Then
                SendMessage("txtStatusBar1", "")
                SendMessage("txtStatusBar2", "")
                SendMessage("txtStatusBar3", "")
                SendMessage("txtStatusBar4", "")
                SendMessage("txtStatusBar5", "")
                steps = 5
                Worker1.ChooseThreads(4)
                workerbusy = True
                eventhandled = True
            End If

            If steps = 5 And eventhandled = False Then
                WorkerStepAnnounceHandler(5)

                txtProcessEnded.Text = "Ended: " & Format(Now(), "dd/MM/yyyy HH:mm:ss")
                SendMessage("txtStatus", "Process Completed")
                workerbusy = False
                eventhandled = True
                txtSearchString.Select()
                ButtonOperationLaunch.Enabled = True
            End If

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerFileProcessingHandler(ByVal filename As String, ByVal queue As Integer)
        Try
            Select Case queue
                Case 1
                    SendMessage("txtStatusBar1", ("Processing: " & filename.Replace(txtBaseFolder.Text & "\", "...\")).ToUpper)
                    SendMessage("txtFileQueue1", CInt(txtFileQueue1.Text) + 1)
                Case 2
                    SendMessage("txtStatusBar2", ("Processing: " & filename.Replace(txtBaseFolder.Text & "\", "...\")).ToUpper)
                    SendMessage("txtFileQueue2", CInt(txtFileQueue2.Text) + 1)
                Case 3
                    SendMessage("txtStatusBar3", ("Processing: " & filename.Replace(txtBaseFolder.Text & "\", "...\")).ToUpper)
                    SendMessage("txtFileQueue3", CInt(txtFileQueue3.Text) + 1)
                Case 4
                    SendMessage("txtStatusBar4", ("Processing: " & filename.Replace(txtBaseFolder.Text & "\", "...\")).ToUpper)
                    SendMessage("txtFileQueue4", CInt(txtFileQueue4.Text) + 1)
                Case 5
                    SendMessage("txtStatusBar5", ("Processing: " & filename.Replace(txtBaseFolder.Text & "\", "...\")).ToUpper)
                    SendMessage("txtFileQueue5", CInt(txtFileQueue5.Text) + 1)
            End Select

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub Shutting_Down()
        Try
            Save_Registry_Values()
            Worker1.Dispose()
            splash_loader.Close()
            Me.Close()
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub Main_Screen_Close(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Shutting_Down()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        application_status_check()
    End Sub

    Private Sub statuslabel()
        Label14.Text = steps & " " & filequeue1.Count & " " & filequeue2.Count & " " & filequeue3.Count & " " & filequeue4.Count & " " & filequeue5.Count & " " & workerbusy.ToString
    End Sub

    Private Sub application_status_check()

        Try
            If steps = 3 Then
                If filequeue1.Count > 0 Or filequeue2.Count > 0 Or filequeue3.Count > 0 Or filequeue4.Count > 0 Or filequeue5.Count > 0 Then
                    If filequeue1.Count > 0 Then
                        If CLng(txtFileQueue1.Text) <= thread1snapshot Then
                            If Worker1.filequeue1 = "" Then
                                Worker1.filequeue1 = CStr(filequeue1.Dequeue())
                            End If
                            Try
                                SendMessage("txtFileQueue1", CLng(txtFileQueueTotal1.Text) - filequeue1.Count - 1)
                                Worker1.ChooseThreads(31)
                            Catch errt As Exception
                            End Try
                        End If
                        thread1snapshot = txtFileQueue1.Text
                    End If
                    If filequeue2.Count > 0 Then
                        If CLng(txtFileQueue2.Text) <= thread2snapshot Then
                            If Worker1.filequeue2 = "" Then
                                Worker1.filequeue2 = CStr(filequeue2.Dequeue())
                            End If
                            Try
                                SendMessage("txtFileQueue2", CLng(txtFileQueueTotal2.Text) - filequeue2.Count - 1)
                                Worker1.ChooseThreads(32)
                            Catch errt As Exception
                            End Try
                        End If
                        thread2snapshot = txtFileQueue2.Text
                    End If
                    If filequeue3.Count > 0 Then
                        If CLng(txtFileQueue3.Text) <= thread3snapshot Then
                            If Worker1.filequeue3 = "" Then
                                Worker1.filequeue3 = CStr(filequeue3.Dequeue())
                            End If
                            Try
                                SendMessage("txtFileQueue3", CLng(txtFileQueueTotal3.Text) - filequeue3.Count - 1)
                                Worker1.ChooseThreads(33)
                            Catch errt As Exception
                            End Try
                        End If
                        thread3snapshot = txtFileQueue3.Text
                    End If
                    If filequeue4.Count > 0 Then
                        If CLng(txtFileQueue4.Text) <= thread4snapshot Then
                            If Worker1.filequeue4 = "" Then
                                Worker1.filequeue4 = CStr(filequeue4.Dequeue())
                            End If
                            Try
                                SendMessage("txtFileQueue4", CLng(txtFileQueueTotal4.Text) - filequeue4.Count - 1)
                                Worker1.ChooseThreads(34)
                            Catch errt As Exception
                            End Try
                        End If
                        thread4snapshot = txtFileQueue4.Text
                    End If
                    If filequeue5.Count > 0 Then
                        If CLng(txtFileQueue5.Text) <= thread5snapshot Then
                            If Worker1.filequeue5 = "" Then
                                Worker1.filequeue5 = CStr(filequeue5.Dequeue())
                            End If
                            Try
                                SendMessage("txtFileQueue5", CLng(txtFileQueueTotal5.Text) - filequeue5.Count - 1)
                                Worker1.ChooseThreads(35)
                            Catch errt As Exception
                            End Try
                        End If
                        thread5snapshot = txtFileQueue5.Text
                    End If
                End If
            End If
            If steps = 4 And workerbusy = False Then
                WorkerCompleteHandler(0)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Restart Stalled Threads")
        End Try
    End Sub


    Private Function load_datatypes() As Boolean
        Dim result As Boolean = False
        Try
            Dim path1, path2, filetoread As String
            path1 = (Application.StartupPath & "\config.ini").Replace("\\", "\")
            path2 = (Application.StartupPath & "\default_config.ini").Replace("\\", "\")
            filetoread = ""
            Dim finfo As FileInfo = New FileInfo(path1)
            If finfo.Exists = True Then
                filetoread = path1
            Else
                finfo = New FileInfo(path2)
                If finfo.Exists = True Then filetoread = path2
            End If

            If filetoread.Length > 0 Then
                filetypes = New ArrayList
                filetypes.Clear()

                Dim config As StreamReader = New StreamReader(filetoread)

                While config.Peek > -1
                    filetypes.Add(config.ReadLine.Trim)

                End While
                config.Close()
                filetypes.Sort()
                Dim str As String
                For Each str In filetypes
                    ListBox1.Items.Add(str)
                Next
                result = True
            End If


        Catch ex As Exception
            Error_Handler(ex)
            result = False
        End Try
        Return result
    End Function


    Private Sub Main_Screen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Registry_Values()
        statuslabel()

        thread1snapshot = 0
        thread2snapshot = 0
        thread3snapshot = 0
        thread4snapshot = 0
        thread5snapshot = 0
        dataloaded = True
        splash_loader.Visible = False
        Me.Show()
        If load_datatypes() = False Then
            Error_Handler("Error reading required Config.ini input file. This application will now shut down.")
            Shutting_Down()
        End If
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If Worker1.WorkerThread Is Nothing = False Then
                Worker1.stopcheck = True
                WorkerStepAnnounceHandler(0)
                Worker1.WorkerThread.Abort()
                workerbusy = False
                SendMessage("txtStatus", "Process Terminated")
                ButtonOperationLaunch.Enabled = True
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub Load_Registry_Values()
        Try
            Dim configflag As Boolean
            configflag = False
            Dim str As String
            Dim keyflag1 As Boolean = False
            Dim oReg As RegistryKey = Registry.LocalMachine
            Dim keys() As String = oReg.GetSubKeyNames()
            System.Array.Sort(keys)

            For Each str In keys
                If str.Equals("Software\Find And Replace") = True Then
                    keyflag1 = True
                    Exit For
                End If
            Next str

            If keyflag1 = False Then
                oReg.CreateSubKey("Software\Find And Replace")
            End If

            keyflag1 = False

            Dim oKey As RegistryKey = oReg.OpenSubKey("Software\Find And Replace", True)

            str = oKey.GetValue("frmbasefolder")
            If Not IsNothing(str) And Not (str = "") Then
                frmbasefolder = str
            Else
                configflag = True
                oKey.SetValue("frmbasefolder", (Application.StartupPath))
                frmbasefolder = (Application.StartupPath)
            End If
            txtBaseFolder.Text = frmbasefolder


            str = oKey.GetValue("frmsearchstring")
            If Not IsNothing(str) And Not (str = "") Then
                frmsearchstring = str
            Else
                configflag = True
                oKey.SetValue("frmsearchstring", "String to Search for")
                frmsearchstring = ("String to Search for")
            End If
            txtSearchString.Text = frmsearchstring


            str = oKey.GetValue("frmreplacestring")
            If Not IsNothing(str) And Not (str = "") Then
                frmreplacestring = str
            Else
                configflag = True
                oKey.SetValue("frmreplacestring", "Replacement String")
                frmreplacestring = ("Replacement String")
            End If
            txtReplaceString.Text = frmreplacestring
            oKey.Close()
            oReg.Close()

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub Save_Registry_Values()
        Try
            Dim oReg As RegistryKey = Registry.LocalMachine
            Dim oKey As RegistryKey = oReg.OpenSubKey("Software\Find And Replace", True)

            oKey.SetValue("frmsearchstring", txtSearchString.Text)
            oKey.SetValue("frmreplacestring", txtReplaceString.Text)
            oKey.SetValue("frmbasefolder", txtBaseFolder.Text)

            oKey.Close()
            oReg.Close()
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub



    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        statuslabel()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        application_status_check()
    End Sub
End Class
