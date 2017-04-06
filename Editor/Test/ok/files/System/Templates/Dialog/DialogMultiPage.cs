
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 264 160 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 96 80 ""
 1001 Static 0x54020000 0x4 112 4 48 13 "Page0"
 1101 Static 0x54020000 0x4 112 4 48 13 "Page1"
 1201 Static 0x54020000 0x4 112 4 48 13 "Page2"
 1 Button 0x54030001 0x4 136 140 48 14 "OK"
 2 Button 0x54030000 0x4 188 140 48 14 "Cancel"
 4 Button 0x54032000 0x4 240 140 18 14 "?"
 5 Static 0x54000010 0x20004 0 132 800 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "0" "" ""

str controls = "3"
str lb3
lb3="&Page0[]Page1[]Page2"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	sub.SelectPage hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	sub.SelectPage hDlg
ret 1


#sub SelectPage
function hDlg
int page=LB_SelectedItem(id(3 hDlg))
DT_Page hDlg page
