\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog56" &Dialog56)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 18 24 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x203000E "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_CLOSE
	PostQuitMessage 0
	 out "close"
	 DestroyWindow hDlg
	 EndDialog hDlg 0
	 int+ kkkkkkk=1
	case WM_DESTROY
	out "destr"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	opt waitmsg 1
	 1000
	wait 0 WV "kkkkkkkkkk"
	
	 int+ g_testwaitvar=0
	 out wait(10 V g_testwaitvar)
	
	 zw wait(0 WV "Notepad")
	
	 kkkkkkk=0
	 out wait(0 V kkkkkkk)
	 0.1
	
	case IDOK
	case IDCANCEL
ret 1
