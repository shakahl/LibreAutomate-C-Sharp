\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

hDlg=ShowDialog("dialog_fade" &dialog_fade 0 0 1 0 0 0 -1 -1)

AnimateWindow hDlg 2000 AW_BLEND ;;use this
 for(_i 0 256 2) Transparent hDlg _i; 0.015; if(!_i) hid- hDlg ;;or this

opt waitmsg 1
3

 AnimateWindow hDlg 2000 AW_BLEND|AW_HIDE ;;use this (does not work well on Windows 7)
for(_i 256 0 -2) 0.015; Transparent hDlg _i ;;or this
clo hDlg

 BEGIN DIALOG
 0 "" 0x80C808C8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

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
