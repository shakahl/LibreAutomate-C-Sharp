 /VR_Main
 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 129 15 "QM Voice Triggers"
 5 Button 0x54032001 0x4 0 0 32 14 "Menu"
 3 Button 0x54030000 0x4 32 0 32 14 "Edit"
 4 Button 0x54032000 0x4 64 0 32 14 "Apply"
 6 Button 0x54032000 0x4 96 0 32 14 "Exit"
 END DIALOG

function# hDlg message wParam lParam

HSRLib.Vcommand- vr
int- vrmenu
str voicecommands
int+ _hwndvr
int-- active actwnd

sel message
	case WM_INITDIALOG
	Tray-- tray.AddIcon("Ex triggers.ico" "QM Voice Triggers" 3 hDlg)
	VR_Init; active=1
	ret 1
	 ----
	case WM_COMMAND
	sel wParam
		case 5
		rget voicecommands
		mac voicecommands
		case 3
		rget voicecommands
		mac+ voicecommands
		case 4
		men 2004 _hwndqm ;;save
		0.1
		VR_Init; active=1
		 ----
		case IDCANCEL min hDlg; hid hDlg
		case 6 DestroyWindow(hDlg)
		 ----
	ret 1
	 ----
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	case WM_DESTROY DT_DeleteData(hDlg)
	vr.ReleaseMenu(vrmenu)
	_hwndvr=0
	PostQuitMessage(hDlg)

	 message from tray icon:
	case (WM_USER+101)
	if(lParam=WM_LBUTTONUP) ifk(C) DestroyWindow(hDlg) else hid- hDlg; act hDlg
	else if(lParam=WM_MOUSEMOVE)
		int h=win; if(h and h!=win("" "Shell_TrayWnd")) actwnd=win
	else if(lParam=WM_RBUTTONUP)
		active^1
		if(active) vr.Activate(vrmenu); else vr.Deactivate(vrmenu)
		if(actwnd) act actwnd
