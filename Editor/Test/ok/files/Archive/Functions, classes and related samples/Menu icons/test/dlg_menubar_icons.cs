\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

SetThreadMenuIcons "101=0 103=1 104=2 120=3 61456=16 61440=17" "$qm$\il_qm.bmp" 1

str controls = "3"
str rea3
rea3="Try to:[]Click menubar.[]Right click here.[]Right click title bar."
if(!ShowDialog("dlg_menubar_icons" &dlg_menubar_icons &controls 0 0 0 0 0 0 0 "dialog.ico" "dlg_menubar_icons")) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 294 190 "Dialog"
 3 RichEdit20A 0x54233044 0x200 0 0 294 190 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	siz r.right r.bottom id(3 hDlg)
	case WM_COMMAND goto messages2
	case WM_CONTEXTMENU
	if(wParam=id(3 hDlg))
#ifdef ShowMenu
		_i=ShowMenu("103 Cut[]104 Select All" hDlg)
#else
		mes "You need function ShowMenu. Download from QM forum."
#endif
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
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0 0 Co
	 -
	 >&Recent : 120
		 Empty : 102 0 3
		 <
	 <
 >&Edit
	 Cu&t : 103 0 0 Cx
	 -
	 Select &All : 104 0 0 Ca
	 <
 &Help : 105 0
 END MENU
