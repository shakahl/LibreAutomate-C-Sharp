\Dialog_Editor

 Shows how to use __Settings in a dialog procedure.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x200 0 0 224 112 "0x7,0,0,0,0x0[]Name,,,[]Value,,,[]Check,,2,"
 4 Button 0x54032000 0x0 12 116 48 14 "Get"
 5 Button 0x54032000 0x0 62 116 48 14 "Set"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	str settingsCSV=
	 one,Onee,1,
	 two,Twoo,2,Yes
	 three,Threee,3,
	 four,Fourr,4,Yes
	__Settings-- x
	x.Init(settingsCSV "__Settings_dlgproc" "\test" 1)
	x.ToGrid(g)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	x.FromGrid(g)
	x.ToReg()
	
	case 4 ;;Get
	x.FromGrid(g)
	out "two: str=%s, int=%i, check(1)=%i" x.GetStr("two") x.GetInt("two") x.GetCheck("two" 1)
	
	case 5 ;;Set
	x.Set("two" 100); x.SetCheck("two" 1 1); x.SetCheck("three" 0 1)
	x.ToGrid(g)
ret 1
