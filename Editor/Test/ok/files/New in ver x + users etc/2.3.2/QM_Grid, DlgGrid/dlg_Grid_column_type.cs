\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str qmg3 e4
qmg3="one[]two[]three"
if(!ShowDialog("" 0 &controls)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog58"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x54030000 0x0 0 0 224 86 "0,0,0,2[]A,20%,[]B,20%,8[]c,20%,1[]d,10%,9[]e,10%,2[]f,10%,7[]g,10%,16[]"
 4 Edit 0x54030080 0x200 98 90 98 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""

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
	case IDCANCEL
ret 1
