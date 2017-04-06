\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3
qmg3="one,two,three[]four,five,six"
 qmg3="one,two[]four,five"
if(!ShowDialog("dlg_QM_Grid_easy" &dlg_QM_Grid_easy &controls)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x54030000 0x0 0 0 224 112 "1,1,0,2[]Col1,33%[]Col2,33%,1[]Col3,33%,2"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
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
