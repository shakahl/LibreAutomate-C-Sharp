\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "6 4 3 5 9"
str e6 cb4 e3 qmg5x ax9SHD
cb4="one[]two"
e6="zero"
if(!ShowDialog("Dialog110" &Dialog110 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 6 Edit 0x50030080 0x0 8 30 12 4 ""
 4 ComboBox 0x54230242 0x0 8 16 46 213 ""
 3 Edit 0x54030080 0x200 96 18 34 14 ""
 5 QM_Grid 0x56031041 0x0 160 10 48 40 "0x0,0,0,0,0x0[]A,,,[]B,,," "trrrr"
 7 QM_DlgInfo 0x54000000 0x20000 180 54 34 42 ""
 8 msctls_trackbar32 0x54030005 0x0 48 46 92 18 ""
 9 ActiveX 0x54030000 0x0 78 64 96 48 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 4 Edit 0x54030080 0x200 168 42 36 14 ""
 3 msctls_updown32 0x54030014 0x0 208 42 12 14 ""

ret
 messages
TO_EditWithCombo hDlg 6 4
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
