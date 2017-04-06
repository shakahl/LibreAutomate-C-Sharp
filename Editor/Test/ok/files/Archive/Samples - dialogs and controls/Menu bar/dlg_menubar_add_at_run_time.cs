\Dialog_Editor

 Adds menu bar in third page only.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="&Page0[]Page1[]Page2"
if(!ShowDialog("dlg_menubar_add_at_run_time" &dlg_menubar_add_at_run_time &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 265 163 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 96 80 ""
 1001 Static 0x54020000 0x4 106 4 48 13 "Page0"
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 4 138 257 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "0" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto selectpage
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	 menu items:
	case 101 out "Open"
	case 103 out "Cut"
	case 104 out "Select All"
	case 105 out "Help"
	
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	 selectpage
	_i=LB_SelectedItem(id(3 hDlg))
	DT_Page hDlg _i
	
	 add menu bar if page is 2, and remove in other pages
	int hmenu haccel
	if(_i=2) hmenu=DT_CreateMenu("dlg_menubar_add_at_run_time" haccel)
	DT_SetMenu hDlg hmenu haccel
	
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0
	 -
	 >&Recent
		 Empty : 102 0 3
		 <
	 <
 >&Edit
	 Cu&t[9]Ctrl+X : 103 0 0 Cx
	 -
	 Select &All : 104 0
	 <
 &Help : 105 0
 END MENU
