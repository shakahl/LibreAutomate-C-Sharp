\Dialog_Editor

 Shows how to make dialog resizable, and how to auto resize/move controls when user resizes dialog.
 To make dialog resizable, in dialog editor add WS_THICKFRAME style. Also optionally add WS_MAXIMIZEBOX and WS_MINIMIZEBOX.
 To auto resize/move controls, use function DT_AutoSizeControls.


function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6"
str e3 c4Che e5 e6
if(!ShowDialog("dlg_autosize_controls" &dlg_autosize_controls &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 219 132 "Dialog"
 1 Button 0x54030001 0x4 118 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 46 6 170 14 ""
 4 Button 0x54012003 0x0 4 102 48 12 "Check"
 5 Edit 0x54231044 0x200 120 56 96 48 ""
 6 Edit 0x54030080 0x200 16 116 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
DT_AutoSizeControls hDlg message "1m 2m 3sh 4mv 5s 6mv 6sh"
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
