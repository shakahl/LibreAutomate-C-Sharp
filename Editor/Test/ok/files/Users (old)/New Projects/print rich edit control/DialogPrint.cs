\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
if(!ShowDialog("DialogPrint" &DialogPrint &controls 0 0 0 0 0 0 0 "dialog.ico" "DialogPrint")) ret

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 431 282 "Dialog"
 3 RichEdit20A 0x54233044 0x200 0 0 294 190 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	PAGESETUPDLG+ g_psd9
	PREC_PageSetup hDlg 0
	
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	siz r.right r.bottom id(3 hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	 menu items:
	case 101 out "Open"
	case 103 out "Cut"
	case 104 out "Select All"
	case 105 out "Help"
	case 106 PREC_PageSetup hDlg 1
	case 107 PrintRichEditControl hDlg id(3 hDlg) "print test"
	
	case IDOK
	case IDCANCEL
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0 0 Co
	 -
	 >&Recent
		 Empty : 102 0 3
		 <
	 -
	 Pa&ge Setup : 106 0 0
	 &Print : 107 0 0
	 <
 >&Edit
	 Cu&t : 103 0 0 Cx
	 -
	 Select &All : 104 0 0 Ca
	 <
 &Help : 105 0
 END MENU
