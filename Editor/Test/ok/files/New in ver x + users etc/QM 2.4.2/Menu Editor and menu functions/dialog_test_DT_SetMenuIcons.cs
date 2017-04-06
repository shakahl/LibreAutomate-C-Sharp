\Dialog_Editor

DT_SetMenuIcons "501=1 502=2 552=3" "$qm$\il_qm.bmp" 1

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

 example menu bar:
 if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 0 0 0 md)) ret
 example popup menu:
 int i=ShowMenu(md); out i

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 0 0 0 md)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
