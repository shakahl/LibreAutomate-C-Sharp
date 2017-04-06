\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
if(!ShowDialog("" &dlg_SetStaticIcon &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000003 0x0 6 8 16 16 ""
 4 Button 0x54032000 0x0 2 42 48 14 "Button"
 5 Button 0x54032000 0x0 54 42 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetStaticIcon id(3 hDlg) "$qm$\copy.ico"
	
	case WM_DESTROY
	SetStaticIcon id(3 hDlg)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	SetStaticIcon id(3 hDlg) "$qm$\paste.ico"
	
	case 5
	SetStaticIcon id(3 hDlg) "shell32.dll,3" 1
	
	case IDOK
	case IDCANCEL
ret 1
