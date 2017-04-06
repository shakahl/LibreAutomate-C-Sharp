\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x=
 one
 two
 </////1>three
 </40////1>four
 five
if(!ShowDialog("dlg_Grid_check_autoselect" &dlg_Grid_check_autoselect &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 12 118 48 14 "OK"
 2 Button 0x54030000 0x4 62 118 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 112 "0x27,0,0,4,0x8000[]A,,,[]A,,,[]RO,,7,"
 4 Button 0x54032000 0x0 122 118 48 14 "Check"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "*" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	g.SetImagelist(il)
	out
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case 4
	g.RowCheck(3 1)
	g.RowCheck(4 0)
	for(_i 0 g.RowsCountGet) out g.RowIsChecked(_i)
	
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))

NMLISTVIEW* nlv=+nh
NMITEMACTIVATE* na=+nh
sel nh.code
	case LVN_ITEMCHANGED
	out "itemchanged: %s" _s.getstruct(*nlv 1)
	int row isChecked
	if(g.RowIsCheckNotification(lParam row isChecked)) out "%schecked %i" iif(isChecked "" "un") row
	
	case NM_CLICK
	out "row click: %s" _s.getstruct(*na 1)
