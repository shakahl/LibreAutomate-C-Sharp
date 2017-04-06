\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
cb3="&one[]two[]three[]four[]five[]six[]seven[]eight[]nine"
if(!ShowDialog("dlg_combo_autocomplete" &dlg_combo_autocomplete &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ComboBox 0x54230342 0x0 6 4 96 213 ""
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
	case CBN_EDITCHANGE<<16|3
	CB_AutoComplete lParam
	case IDOK
	case IDCANCEL
ret 1
