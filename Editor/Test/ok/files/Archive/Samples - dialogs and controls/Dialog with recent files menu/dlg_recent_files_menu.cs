\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

def X48_RFM_MAXITEMS 2 ;;max number of recent files in menu. Change this.
def X48_RFM_MENUFIRSTID 10000 ;;change only if conflicts with some id, eg control id
def X48_RFM_REGKEY "\mycompany\myprogram" ;;change to your registry key
def X48_RFM_REGVALUE "recent" ;;change to your registry value

str controls = "3"
str rea3
if(!ShowDialog("dlg_recent_files_menu" &dlg_recent_files_menu &controls 0 0 0 0 0 0 0 "dialog.ico" "dlg_recent_files_menu")) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 294 190 "Dialog"
 3 RichEdit20A 0x54233044 0x200 0 0 294 190 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 get handles of all submenus that we'll need (eg with WM_INITMENUPOPUP)
	type X48_POPUPMENUS menubar file file_recent ;;popup menu handles. Add more if needed.
	X48_POPUPMENUS- pm
	pm.menubar=GetMenu(hDlg)
	pm.file=MenuFindSubMenu(pm.menubar "&File" 1)
	pm.file_recent=MenuFindSubMenu(pm.file "&Recent" 1)
	
	case WM_INITMENUPOPUP
	if(wParam=pm.file_recent)
		RfmCreateMenu wParam X48_RFM_REGKEY X48_RFM_REGVALUE X48_RFM_MENUFIRSTID
	
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	siz r.right r.bottom id(3 hDlg)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	 menu items:
	case 101 ;; Menu File>>Open
		str s fileTxt
		if OpenSaveDialog(0 s "Text files[]*.txt[]Image files[]*.bmp;*.gif[]All Files[]*.*[]")
			 open
			fileTxt.getfile(s)
			fileTxt.setwintext(id(3 hDlg))
			
			RfmAddFile s X48_RFM_REGKEY X48_RFM_REGVALUE X48_RFM_MAXITEMS
	case 103 out "Cut"
	case 104 out "Select All"
	case 105 out "Help"
	
	case else
	if(wParam>=X48_RFM_MENUFIRSTID and wParam<X48_RFM_MENUFIRSTID+X48_RFM_MAXITEMS)
		MenuGetString pm.file_recent wParam &s
		if(dir(s)) goto open
	
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
	 <
 >&Edit
	 Cu&t : 103 0 0 Cx
	 -
	 Select &All : 104 0 0 Ca
	 <
 &Help : 105 0
 END MENU
