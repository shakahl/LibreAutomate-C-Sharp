\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str sb3 si4 si5
sb3=":1 $qm$\bmp00004.bmp"
si4=":200 $qm$\target.ico"
si5="&:200 $qm$\target.ico"
 if(!ShowDialog("dlg_bmp_res" &dlg_bmp_res &controls)) ret
if(!ShowDialog("dlg_bmp_res" &dlg_bmp_res &controls 0 0 0 0 0 0 0 ":150 $qm$\keyboard.ico")) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 133 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x5400100E 0x20000 6 6 16 16 ""
 4 Static 0x54000003 0x0 6 48 16 16 ""
 5 Static 0x54000003 0x0 6 74 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

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
