\Dialog_Editor


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

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000000 0x0 8 8 48 12 "Text"
 4 Edit 0x54030080 0x200 8 28 96 13 "t"
 5 Button 0x54012003 0x0 8 48 48 10 "Check"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "4 5"
str e4t c5Che
e4t="kk"
c5Che=1
if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 0 0 0 md)) ret
out e4t
out c5Che


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 DT_SetBackgroundColor hDlg 1 0xffeeee 0x8000
	 DT_SetTextColor hDlg 0xff0000 "3 4"
	DT_SetBackgroundImage hDlg "$my qm$\Copy.jpg"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 501 ;;&Open
	out "open"
	case 502 ;;&Save
	out "save"
ret 1
