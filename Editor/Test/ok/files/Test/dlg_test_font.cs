\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_test_font" &dlg_test_font 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 4 6 48 12 "Custom"
 4 Static 0x54000000 0x0 4 20 48 13 "Dialog"
 5 Static 0x54000000 0x0 4 34 48 12 "Icon"
 6 Static 0x54000000 0x0 4 48 48 12 "Menu"
 7 Static 0x54000000 0x0 4 62 48 12 "Statusbar"
 8 Static 0x54000000 0x0 4 76 48 13 "Messagebox"
 9 Static 0x54000000 0x0 4 90 48 12 "Caption"
 10 Static 0x54000000 0x0 4 104 48 12 "Smallcaption"
 11 Static 0x54000000 0x0 4 118 48 12 "Template"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Font-- f3 f4 f5 f6 f7 f8 f9 f10 f11
	f3.Create("Courier New"); f3.SetDialogFont(hDlg "3")
	f4.Create("" 12 0 0 1 2); f4.SetDialogFont(hDlg "4")
	f5.Create("" 12 0 0 2 2); f5.SetDialogFont(hDlg "5")
	f6.Create("" 0 0 0 3); f6.SetDialogFont(hDlg "6")
	f7.Create("" 0 0 0 4); f7.SetDialogFont(hDlg "7")
	f8.Create("" 0 0 0 5); f8.SetDialogFont(hDlg "8")
	f9.Create("" 0 0 0 6); f9.SetDialogFont(hDlg "9")
	f10.Create("" 0 0 0 7); f10.SetDialogFont(hDlg "10")
	f11.Create("" 0 0 0 f3); f11.SetDialogFont(hDlg "11")
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
