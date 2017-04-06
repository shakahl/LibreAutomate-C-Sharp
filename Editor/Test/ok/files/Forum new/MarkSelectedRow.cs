
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Grid 0x56031049 0x200 0 5 220 99 ""
 4 Button 0x54032000 0x0 7 115 48 14 "Action"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "3"
str qmg3
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam
DlgGrid g.Init(hDlg 3)

sel message
	case WM_INITDIALOG
	
	g.ColumnsAdd("Modified,30%,7[]File in Zip,70%" 1)
	
	str slo=
	 1,A
	 2,B
	 3,C
	 4,D
	 5,E
	 6,F
	 7,G
	 8,H
	g.FromCsv(slo "," 0)
	

	case WM_DESTROY
	case WM_COMMAND goto messages2
	
ret
 messages2

sel wParam
	case IDOK
	case IDCANCEL
ret 1