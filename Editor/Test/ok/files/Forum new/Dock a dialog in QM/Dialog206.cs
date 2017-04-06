str md=
 BEGIN MENU
 >&File
 	&Open :501 0x0 0x0 Co
 	&Save :502 0x0 0x0 Cs
 	>Submenu
 		Item1 :551
 		Item2 :552
 		<
 	-
 	E&xit :2
 	<
 >&Edit
 	Cu&t :601
 	&Copy :602
 	&Paste :603
 	<
 >&Help
 	&About :901
 	<
 END MENU

  menu bar example:
 if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 0 0 0 md)) ret
  popup menu example:
 int i=ShowMenu(md); out i

str dd=
 BEGIN DIALOG
 0 "" 0x90CC0AC8 0x8 0 0 224 136 "Dialog3"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 0 0 0 md)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_SetAutoSizeControls hDlg "1m 2m"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 501
	out "Open"
	
	case IDOK
	case IDCANCEL
ret 1
