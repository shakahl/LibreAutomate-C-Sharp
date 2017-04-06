 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

type TYDV ~controls ~qmdi3 ~c5Che ~c6Che ~c7Che ~c8Che ~qmdi1001
TYDV d.controls="3 5 6 7 8 1001"
if(!ShowDialog("dialog_test_minimize" &dialog_test_minimize &d _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_DlgInfo 0x54000000 0x20000 10 16 96 48 ""
 5 Button 0x54012003 0x0 118 36 48 13 "Check"
 6 Button 0x54012003 0x0 132 56 48 12 "Check"
 7 Button 0x54012003 0x0 140 74 48 12 "Check"
 8 Button 0x54012003 0x0 130 90 48 12 "Check"
 1001 QM_DlgInfo 0x54000000 0x20000 0 87 96 48 ""
 END DIALOG
 DIALOG EDITOR: "TYDV" 0x2030400 "*" "0" "0 1" "3 1001"

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_LBUTTONDOWN
	out
	Q &q
	__IdStringParser p.Parse(hDlg "1 2-4 -7-9")
	Q &qq
	outq ;;60, was 47
	out p.warnings
	int i
	for i 0 p.a.len
		out p.a[i].flags
	
	 __MinimizeDialog m
	 m.Minimize
	 TO_Mouse 0 0 0 hDlg
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
