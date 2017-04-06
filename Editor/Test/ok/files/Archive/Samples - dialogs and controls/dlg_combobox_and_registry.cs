\Dialog_Editor

 Dialog with a combo box.
 You can add/remove items.
 Saves all in the registry.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
rget cb3 "cb" "\test\test"
if(!ShowDialog("dlg_combobox_and_registry" &dlg_combobox_and_registry &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Name List"
 1 Button 0x54030001 0x4 2 110 48 14 "OK"
 2 Button 0x54030000 0x4 60 110 48 14 "Cancel"
 3 ComboBox 0x54230641 0x0 6 16 96 56 ""
 5 Button 0x54032000 0x0 104 16 18 14 "+"
 6 Button 0x54032000 0x0 104 32 18 14 "-"
 4 Button 0x54020007 0x0 2 2 126 96 "Employee Names "
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 5 ;;+
	str s.getwintext(id(3 hDlg))
	s.trim; if(!s.len) ret
	CB_Add id(3 hDlg) s
	 SendMessage id(3 hDlg) CB_ADDSTRING 0 s ;;use this instead of the above line if CB_Add is unavailable
	
	case 6 ;;-
	int i=CB_SelectedItem(id(3 hDlg))
	if(i<0) ret
	SendMessage id(3 hDlg) CB_DELETESTRING i 0
	
	case IDOK
	int j h=id(3 hDlg); str s1 s2
	for j 0 CB_GetCount(h)
		CB_GetItemText h j s1
		s2.addline(s1)
	rset s2 "cb" "\test\test"
	
	case IDCANCEL
ret 1
