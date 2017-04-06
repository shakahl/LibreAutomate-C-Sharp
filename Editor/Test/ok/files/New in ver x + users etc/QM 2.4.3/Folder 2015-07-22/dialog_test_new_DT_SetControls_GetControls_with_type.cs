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
 END DIALOG
 DIALOG EDITOR: "TYYYYYY" 0x2040301 "*" "" "" ""

type TYYYYYY ~controls ~c3Che ~e4
TYYYYYY d.controls="3 4"
d.c3Che=1
d.e4="aaa"
if(!ShowDialog(dd &sub.DlgProc &d)) ret
out d.c3Che
out d.e4


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 5
	 DT_SetControls hDlg
	
	 controls = "3 4"
	TYYYYYY d.controls="3 4"
	d.c3Che=0
	d.e4="bbbbb"
	DT_SetControls hDlg 0 &d
	
	case 6
	TYYYYYY& r=DT_GetControls(hDlg)
	
	 TYYYYYY r;;.controls = "3 4"
	 DT_GetControls(hDlg &r)
	
	out r.c3Che
	out r.e4
	
ret 1
