\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_statusbar_images" &dlg_statusbar_images)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 102 48 14 "OK"
 2 Button 0x54030000 0x4 170 102 48 14 "Cancel"
 3 msctls_statusbar32 0x54030000 0x0 0 121 223 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int hsb=id(3 hDlg)
	int w1(70) w2(140) w3(-1)
	SendMessage hsb SB_SETPARTS 3 &w1
	
	SendMessage hsb SB_SETTEXTA 0 "one"
	SendMessage hsb SB_SETTEXTA 1 "two"
	SendMessage hsb SB_SETTEXTA 2 "three"
	
	ARRAY(__Hicon)- t_ic
	t_ic.redim
	t_ic[]=GetFileIcon("$qm$\cut.ico")
	t_ic[]=GetFileIcon("$qm$\copy.ico")
	t_ic[]=GetFileIcon("$qm$\paste.ico")
	SendMessage hsb SB_SETICON 0 t_ic[0]
	SendMessage hsb SB_SETICON 1 t_ic[1]
	SendMessage hsb SB_SETICON 2 t_ic[2]
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
