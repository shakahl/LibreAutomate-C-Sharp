\Dialog_Editor

 Shows how to draw icons list box control when items are added later.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
if(!ShowDialog("dialog_od_combo2" &dialog_od_combo2 &controls)) ret
out lb3

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 204 164 "Dialog"
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 56 146 48 14 "Cancel"
 3 ListBox 0x54230151 0x200 4 104 96 36 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages

CB_DrawImages hDlg message wParam lParam 3 "mouse.ico[]keyboard.ico" 1|2

sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	LB_Add h "one" 0
	LB_Add h "two" 1
	LB_Add h "three" 0
	LB_Add h "four" 1
	 third argument of LB_Add is icon index in the list of icon files
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
