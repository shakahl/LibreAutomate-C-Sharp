\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out

str controls = "5"
str e5="HHH"
if(!ShowDialog("QMTC_SameThread" &QMTC_SameThread &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000000 0x0 68 10 48 12 "Text"
 4 Button 0x54032000 0x0 6 8 48 14 "Button"
 5 Edit 0x54030080 0x200 68 32 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 goto g1
ret 1
 g1
WTI* ta
int tf
int i n=CaptureWindowText(ta hDlg tf)
WT_ResultsOut ta n
