\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

Tray- t
sel message
	case WM_INITDIALOG
	t.AddIcon("$qm$\paste.ico" "" 1 hDlg)
	
	case WM_USER+101
	sel lParam
		case WM_MOUSEMOVE
		int- inTT
		if !inTT
			inTT=1
			ShowTooltip "balloon" 3 xm ym 200 1 "Tooltip Title" "lightning.ico"
			inTT=0
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2

type NOTIFYICONIDENTIFIER cbSize hWnd uID GUID'guidItem [pack1]
dll- shell32 #Shell_NotifyIconGetRect NOTIFYICONIDENTIFIER*identifier RECT*iconLocation
sel wParam
	case 3
	if _winver>=0x601 ;;Shell_NotifyIconGetRect added in Windows 7
		NOTIFYICONDATAW& nr=+&t
		NOTIFYICONIDENTIFIER k.cbSize=sizeof(k); k.hWnd=nr.hWnd; k.uID=nr.uID
		RECT r
		Shell_NotifyIconGetRect(&k &r)
		ShowTooltip "balloon" 3 r.left+r.right/2 r.top 200 1 "Tooltip Title" "lightning.ico"
	
	case IDCANCEL
ret 1
