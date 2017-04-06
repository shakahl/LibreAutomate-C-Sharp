\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
if(!ShowDialog("dialog_ComboBox_AutoWidth" &dialog_ComboBox_AutoWidth &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ComboBox 0x54230242 0x0 6 6 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 __Font-- t_font.Create("Arial" 14)
	 t_font.SetDialogFont(hDlg)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_DROPDOWN<<16|3 ;;before combo box dropdown
	int hcb=lParam ;;lParam is combo box control handle
	 create list of random strings for testing
	str s
	rep(2) s.addline(_s.RandomString(1 60))
	 add items to the control
	SendMessage hcb CB_RESETCONTENT 0 0
	foreach(_s s) CB_Add hcb _s ;;add item for each line
	 calc max width
	SIZE z
	CalculateTextSize s z SendMessage(hcb WM_GETFONT 0 0) DT_NOPREFIX ;;this function is somewhere in Archive
	 set combo box list width
	SendMessage hcb CB_SETDROPPEDWIDTH z.cx+10 0
	
	case IDOK
	case IDCANCEL
ret 1
