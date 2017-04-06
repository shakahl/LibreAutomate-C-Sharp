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

NOTIFYICONDATAW- t
__Hicon- hicon
sel message
	case WM_INITDIALOG
	hicon=GetFileIcon("$qm$\paste.ico")
	t.cbSize=sizeof(t)
	t.hWnd=hDlg
	t.uID=1
	t.hIcon=hicon
	 lstrcpynW &t.szTip L"tooltip" 128
	t.uCallbackMessage=WM_USER+101
	 t.uFlags=NIF_MESSAGE|NIF_ICON|NIF_TIP
	
	 t.uFlags=NIF_MESSAGE|NIF_ICON|NIF_TIP|NIF_INFO
	t.uFlags=NIF_MESSAGE|NIF_ICON|NIF_TIP
	lstrcpynW &t.szTip L"tooltip" 128
	lstrcpynW &t.szInfo L"balloon" 256
	lstrcpynW &t.szInfoTitle L"title" 64
	
	Shell_NotifyIconW NIM_ADD &t
	t.uVersion=NOTIFYICON_VERSION
	Shell_NotifyIconW(NIM_SETVERSION &t)
	
	case WM_USER+101
	 out lParam
	 sel lParam
		 case WM_MOUSEMOVE
		 sub.ShowTooltip2 "balloon" 3 xm ym 200 3 "Tooltip Title" "lightning.ico"
		
	case WM_DESTROY
	Shell_NotifyIconW NIM_DELETE &t
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	t.uFlags=NIF_INFO
	t.dwInfoFlags=NIIF_INFO
	t.uTimeout=5000
	out Shell_NotifyIconW(NIM_MODIFY &t)
	 sub.ShowTooltip2 "balloon" 3 xm ym 200 3 "Tooltip Title" "lightning.ico"
	
	case IDCANCEL
ret 1

