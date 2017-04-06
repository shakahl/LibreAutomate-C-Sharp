 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
str controls = "3"
str qmg3
qmg3=
 q,b
 c,d
 e,f
if(!ShowDialog("dlg_QM_Grid_column_widths" &dlg_QM_Grid_column_widths &controls)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 446 135 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 446 48 "0x0,0,0,2[]A,20%,,[]B,30%,,[]C,50,,[]D,75,,[]E,100,,[]F,300,,[]"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Button 0x54032000 0x0 6 116 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""
 3 QM_Grid 0x56031041 0x0 0 0 446 86 "0,0,0,2[]A,,,[]B,,8,[]C,,1,[]D,,9,[]E,,2,[]F,,7,[]G,,16,[]H,,24,[]I,,17,[]J,,25,[]K,,18,[]L,,23,[]"
 3 QM_Grid 0x56031041 0x0 0 0 224 110 "0,0,0,2[]A,10,,[]B,20,,[]C,30,,[]D,40,,1[]"
 3 QM_Grid 0x56031041 0x0 0 0 448 106 "0x33,0,0,4[]A,,,[]"

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 4
	 g.ColumnsWidthAdjust
	 g.ColumnsWidthAdjust(2)
	 g.ColumnsWidthAdjust(0 50)
	 g.ColumnsWidthAdjust(0 -50)
	 out g.ColumnsAdd("")
ret 1
