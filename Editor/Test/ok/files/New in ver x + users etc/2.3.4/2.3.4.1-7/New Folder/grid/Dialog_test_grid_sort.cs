\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x=
 <9>one,a
 <8>two,b
 <7>three,c
 <6>four,d
if(!ShowDialog("" &Dialog_test_grid_sort &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x56035041 0x0 0 0 192 100 "0x27,0,0,14,0x0[]A,,,[]B,,,"
 4 Button 0x54032000 0x0 42 106 48 14 "Sort"
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
	case 4
	DlgGrid g.Init(hDlg 3)
	g.Sort(0)
	
ret 1
