 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 0 121 48 14 "Button"
 4 ComboBox 0x54230243 0x0 48 18 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x203000E "" "" ""

ret
 messages
#compile __CToolTip
sel message
	case WM_INITDIALOG
	CToolTip- tt.Create(hDlg)
	tt.AddTool(hDlg 1 "test")
	
	case WM_CLOSE
	 out SendMessage(_hwndqm WM_APP+567 0 0)
	
	case WM_DESTROY
	out "destr"
	case WM_COMMAND goto messages2
	
	case WM_SETCURSOR
	tt.OnWmSetcursor(wParam lParam)
	
	case WM_TIMER
	sel wParam
		case 1
		shutdown -7
	
	case WM_APP
	wait 60
ret
 messages2
sel wParam
	case 3
	rep() _i=0
	 out mac("Menu24")
	 out PopupMenu("a[]b")
	
	 mes 1
	MES m.hwndowner=hDlg
	out mes(1 "" m)
	
	 SetTimer hDlg 1 500 0
	 MessageBox hDlg "" "" 0
	
	case IDOK
	case IDCANCEL
ret 1
