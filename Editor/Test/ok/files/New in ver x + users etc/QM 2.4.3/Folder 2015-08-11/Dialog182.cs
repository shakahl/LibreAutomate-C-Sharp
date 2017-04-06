 \Dialog_Editor

str md=
 BEGIN MENU
 >&File
 	&Open :501 0x0 0x0 Co
 	&Save :502 0x0 0x0 Cs
 	>Submenu
 		kjkjjkjk :551
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
 0 "" 0x90C80AC8 0x0 0 0 348 240 "Dialog" "2"
 1001 Static 0x54020000 0x4 112 4 48 13 "Page0"
 1002 Button 0x54012003 0x0 112 24 48 10 "Check"
 1101 Static 0x54020000 0x4 112 4 48 13 "Page1"
 1102 ComboBox 0x54230243 0x0 112 24 96 213 ""
 1201 Static 0x54020000 0x4 216 12 48 13 "Page2"
 1202 SysLink 0x54030000 0x0 216 32 96 12 "<a>Link1</a>, <a>link2</a>"
 1203 Button 0x54032000 0x0 216 52 48 14 "Button"
 1205 Static 0x54000000 0x0 8 148 80 32 "One[]Two[]"
 1204 Button 0x54020007 0x0 212 8 108 100 ""
 2 Button 0x54030000 0x4 188 140 48 14 "Cancel"
 4 Button 0x54032000 0x4 240 140 30 14 "?"
 5 Static 0x54000010 0x20004 0 132 800 2 ""
 3 ListBox 0x54230101 0x204 4 4 98 46 "" "tttttt"
 1 Button 0x54030006 0x4 64 72 48 14 "OK"
 6 Edit 0x54030080 0x200 85 104 102 16 "name"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "2" "" ""

str controls = "1002 1102 3 1 6"
str c1002Che cb1102 lb3 c1OK e6nam
lb3="&Page0[]Page1[]Page2"
if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 0 0 0 md)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	sub.SelectPage hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_EXITSIZEMOVE
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	sub.SelectPage hDlg
	case 1202 ;;link clicked. lParam is 0 for first link, 1 for second, and so on.
	
ret 1


#sub SelectPage
function hDlg
int page=LB_SelectedItem(id(3 hDlg))
DT_Page hDlg page
