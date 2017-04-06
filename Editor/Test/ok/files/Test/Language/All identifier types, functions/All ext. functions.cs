QMITEM q
int v j i=qmitem("System contents")
int i2=qmitem("init")
str s ss sa
rep
	i=qmitem(-i 5 q 1); if(!i or i>=i2) break
	sel(q.itype)
		case 1 sa+q.name; sa+"[]"
		case 6
		j=findc(q.name '.'); if(j<0 or !q.name[j+1]) continue
		v+1
		ss.format(" v%i." v)
		s=q.name
		s.findreplace("." ss)
		sa+s; sa+"[]"
		
sa.setsel

MouseWheel
CapsLock
GetCaretXY
GetMod
Enclose
Outp
Replace
ShowText
OpenSaveDialog
BrowseForFolder
ShellDialog
ColorDialog
ReplaceDlg
ReplaceReplace
TextBox
LogFile
ChDir
CurDir
GetAttr
SetAttr
GetFileInfo
CreateShortcut
UniqueFileName
Dir v1.dir
Dir v2.FileName
Dir v3.FileSize
Dir v4.FileAttributes
Dir v5.IsFolder
Dir v6.TimeModified
Dir v7.TimeModified2
Dir v8.TimeCreated
Dir v9.TimeCreated2
Dir v10.TimeAccessed
Dir v11.TimeAccessed2
Dir v12.Enum
Dir v13.EnumSub
File v14.Open
File v15.Close
File v16.UpdateFile
File v17.WriteLine
File v18.ReadLine
File v19.Write
File v20.Read
File v21.ReadToStr
File v22.GetPos
File v23.SetPos
File v24.EOF
File v25.FileLen
FE_Dir
__ChDir
GetWinId
GetWinXY
GetWinStyle
SetWinStyle
GetClientTop
Transparent
WinC
Zorder
ArrangeWindows
SaveMultiWinPos
RestoreMultiWinPos
RegWinPos
TriggerWindow
WinCEnumProc
ArrWinEnumProc
ArrWinSD
CB_SelectedItem
CB_SelectItem
CB_SelectString
CB_FindItem
CB_GetCount
CB_GetItemText
LB_SelectedItem
LB_SelectItem
LB_SelectString
LB_FindItem
LB_GetCount
LB_GetItemText
SelectedItem
SelectItem
FindItem
GetItemText
SelectTab
GetListViewItemText
GetStatusBarText
Acc v26.DoDefaultAction
Acc v27.Select
Acc v28.Location
Acc v29.Name
Acc v30.Value
Acc v31.SetValue
Acc v32.Description
Acc v33.State
Acc v34.Role
Acc v35.Focus
Acc v36.Selection
Acc v37.ObjectFromPoint
Acc v38.Navigate
Acc v39.Mouse
Acc v40.Result
Htm v41.Click
Htm v42.SetFocus
Htm v43.Text
Htm v44.SetText
Htm v45.HTML
Htm v46.Tag
Htm v47.Attribute
Htm v48.Location
Htm v49.Mouse
Htm v50.Scroll
Htm v51.Checked
Htm v52.CbSelect
Htm v53.CbItem
Htm v54.DocText
Htm v55.DocURL
ExcelSheet v56.Init
ExcelSheet v57.GetCell
ExcelSheet v58.SetCell
ExcelSheet v59.GetCells
ExcelSheet v60.NumRows
FE_ExcelRow
ExcelSheet v61.GetRange
Date
Time
IntConnect
IntDisconnect
IntIsConnected
IntIsOnline
IntGoOnline
IntPing
IntDial
rasapi
IntConnectDlg
RasConnect
RasError
RasGetDefConn
RasGetConnections
RasGetEntries
SendMail
MailMessage
SmtpMail v62.AddMessage
SmtpMail v63.Send
SmtpMail v64.SetSaveFolder
SmtpMail v65.Save
SmtpMail v66.Clear
SmtpMail v67.GetMessage
SmtpMail v68.H
SmtpMail v69.SaveMessage
SmtpMailDlg
smtp_OnSendProgress
Pop3Mail v70.RetrieveMessages
Pop3Mail v71.Disconnect
Pop3Mail v72.SetSaveFolder
Pop3Mail v73.Save
Pop3MailDlg
p_OnReceiveStart
p_OnReceiveProgress
p_OnMessageComplete
MailGetAccounts
MailHeader
MailCWM
MailAddress
MAPI
Ftp v74.Connect
Ftp v75.Disconnect
Ftp v76.DirSet
Ftp v77.DirGet
Ftp v78.DirNew
Ftp v79.DirDel
Ftp v80.Dir
Ftp v81.FileGet
Ftp v82.FilePut
Ftp v83.FileRen
Ftp v84.FileDel
Ftp v85.Cmd
Http v86.Connect
Http v87.Disconnect
Http v88.GetUrl
Http v89.FileGet
Http v90.Post
Http v91.__GetUrl
Http_GetUrl_Thread2
IntGetFile
IntPost
IntSettings
__Wininet v92.Error
AddTrayIcon
RunFileAsMacro
RunTextAsMacro
RunTextAsFunction
DynamicMenu
DynamicToolbar
MenuFromQmFolder
PopupMenu
PopupMenuK
Exit
ClearOutput
ErrMsg
Deb
NewItem
NetSendMacro
Tray v93.AddIcon
Tray v94.Modify
Tray v95.Delete
TrayWndProc
Tray v96.Add
Tray_sample_simplest
Tray_sample_use_function
TrayProcSample
temp_function_template
MF_Proc
PopupMenuCreate
ScreenWidth
ScreenHeight
GetWorkArea
DisplaySettings
GetProcessId
ShutDownProcess
GetProcessCpuUsage
GetUserComputer
IsUserAdmin
SetDefaultPrinter
ColorFromRGB
ColorToRGB
Play
PlayStop
Dde v97.Connect
Dde v98.Disconnect
Dde v99.Execute
Dde v100.Execute2
Dde v101.Request
Dde v102.Request2
Dde v103.Poke
Dde v104.Poke2
__DdeStr v105.Create
DdeExecute
DdeRequest
DdePoke
FE_RegKey
RegGetSubkeys
RegGetValues
RegKey v106.Open
RegKey v107.Close
VbsExec
VbsEval
VbsAddCode
VbsFunc
JsExec
JsEval
JsAddCode
JsFunc
VbsError
MainWindow
RegWinClass
MessageLoop
CreateControl
GetIcon
InsertLine
RegExpMenu
TvAdd
QmHelp
GetWindowList
GetThreadWindows
GetProgList
DownloadComponent
RegisterComponent
EnumQmFolder
CompileAll
TB_ClientY
GWL_EnumProc
GTW_EnumProc
EQ_Rec
TvGetParam
GetSpecFolder
WaitEnabledWindow
WaitIEPage
ShutDown
EndMacro
HttpConnect
HttpDisconnect
HttpFileGet
IntError
FtpConnect
FtpDisconnect
FtpDirSet
FtpDirGet
FtpDirNew
FtpDirDel
FtpDir
FtpFileGet
FtpFilePut
FtpFileRen
FtpFileDel
FtpCmd
TrayIcon
GetWinFromPoint
ArrangeWindowsOld
RestoreTriggers
Checked
TimeDiff
IeNavigate
IeWait
WaitPixelColor
IntHangUp
IntConnected
IntGetUrl
Password
MkDir
TM_Main
GetCount
AccClick
AccSelect
AccSetValue
HtmlClick
HtmlSetFocus
HtmlSetText
HtmlLocation
HtmlDocFromHwnd
HtmlFind
HtmlFromToAcc
WaitFor
replacerx_callback
FFT_Window
FFT_One_Of_Windows
FFT_Window_Dispatcher
FFT_Mouse_Is_On_Control
FFT_Mouse_Is_On_Control2
FFT_Mouse_Is_In_Rectangle
FFT_Mouse_Is_In_Rectangle2
Dialog
DialogEx
DialogMultiPage
WndProc
ToolbarExProc

int i.Set
str s.Sort
