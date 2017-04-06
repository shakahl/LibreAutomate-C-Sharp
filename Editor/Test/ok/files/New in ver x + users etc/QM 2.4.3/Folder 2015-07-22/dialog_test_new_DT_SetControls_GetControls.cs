\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54012003 0x0 16 16 48 10 "Check"
 4 Edit 0x54030080 0x200 68 16 96 12 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 5 Button 0x54030040 0x4 16 40 48 14 "Set"
 6 Button 0x54032080 0x0 16 60 48 14 "Get"
 7 Button 0x54032000 0x0 16 80 48 14 "Param"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3 4"
str c3Che e4
 c3Che=1
e4="aaa"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret
out c3Che
out e4


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
str controls
str c3Che e4
sel wParam
	case IDOK
	case 5
	 DT_SetControls hDlg
	
	 controls = "3 4"
	c3Che=1
	e4="bbb"
	DT_SetControls hDlg 0 &controls
	
	case 6
	 controls = "3 4"
	out DT_GetControls(hDlg &controls)
	out c3Che
	out e4
	
	case 7
	int d dd
	PF
	rep(1000) d=GetWindowLong(hDlg DWL_USER)
	PN
	rep(1000) d=GetProp(hDlg "dialogdata")
	PN
	rep(1000) dd=GetProp(hDlg +__atom_dialogdata)
	PN;PO
	out "%i %i" d dd
	
ret 1
