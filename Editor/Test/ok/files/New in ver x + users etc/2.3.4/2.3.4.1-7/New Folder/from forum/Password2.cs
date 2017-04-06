\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 57 "Passwords"
 3 Edit 0x54030080 0x200 100 6 120 12 ""
 4 Edit 0x54030080 0x200 100 22 120 13 ""
 5 Edit 0x54030080 0x200 100 38 120 12 ""
 7 Button 0x54032000 0x0 0 20 98 14 "Save"
 6 ComboBox 0x54230243 0x0 0 6 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

str- t_file="$desktop$\passwords447.ini"
str Data Name Web User Pass Hive

str controls = "3 4 5 6"
str e3 e4 e5 cb6
if(!ShowDialog("Password2" &Password2 &controls)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	RefreshCombo(id(6 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [7,IDOK] ;;Save
	Name.getwintext(id(3 hDlg))
	User.getwintext(id(4 hDlg))
	Pass.getwintext(id(5 hDlg))
	rset User "User" Name t_file 
	rset Pass "Pass" Name t_file
	RefreshCombo(id(6 hDlg) Name)
	
	case CBN_SELENDOK<<16|6
	CB_SelectedItem lParam Name
	rget User "User" Name t_file
	rget Pass "Pass" Name t_file
	Name.setwintext(id(3 hDlg))
	User.setwintext(id(4 hDlg))
	Pass.setwintext(id(5 hDlg))
ret 1
