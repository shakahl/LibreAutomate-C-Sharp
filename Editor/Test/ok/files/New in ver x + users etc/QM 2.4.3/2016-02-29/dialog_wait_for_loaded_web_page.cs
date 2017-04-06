
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 402 298 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 402 270 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 Button 0x54032000 0x0 4 276 108 14 "Wait for loaded"
 5 Button 0x54032000 0x0 120 276 108 14 "Get notification when loaded"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str ax3SHD
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub.WaitForLoaded hDlg
	case 5 sub.GetNotificationWhenLoaded hDlg
ret 1


#sub WaitForLoaded
function hDlg
SHDocVw.WebBrowser we3
we3._getcontrol(id(3 hDlg))
we3.Navigate("http://www.quickmacros.com/forum")
opt waitmsg 1
rep() 0.01; if(!we3.Busy) break
OnScreenDisplay "loaded"

 note: 'web "http://www.quickmacros.com/forum" 1 id(3 hDlg)' does not work well in dialogs, unless called from another thread.


#sub GetNotificationWhenLoaded
function hDlg
SHDocVw.WebBrowser we3
we3._getcontrol(id(3 hDlg))
we3._setevents("sub.we3")
we3.Navigate("http://www.quickmacros.com/forum")


#sub we3_DocumentComplete
function IDispatch'pDisp `&URL SHDocVw.IWebBrowser2'we3
if(pDisp!=we3) ret ;;a frame document
OnScreenDisplay "DocumentComplete"
