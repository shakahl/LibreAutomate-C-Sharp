\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str qmg3x
qmg3x="one,a[]<8>two,b"
if(!ShowDialog("" &Dialog_test_grid_get &controls)) ret
out qmg3x

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 136 80 "0x27,0,0,14,0x0[]A,,,[]B,,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	DlgGrid g.Init(hDlg 3)
	
	 out g.CellGet(1 0)
	
	GRID.QG_SGACB r.func=&FromToQmGrid_Callback; r.flags=10
	g.Send(GRID.LVM_QG_SETGETALL_CALLBACK 0 &r)
	
	str s
	g.Send(GRID.LVM_QG_GETALLCELLS 10 &s)
	outb s s.len 1
	
	case IDCANCEL
ret 1
