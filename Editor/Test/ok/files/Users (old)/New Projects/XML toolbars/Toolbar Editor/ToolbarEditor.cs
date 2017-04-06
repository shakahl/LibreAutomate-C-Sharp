\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
e4=
 $qm$\keyboard.ico
 -
 $qm$\lightning.ico
 $qm$\list.ico
 $qm$\macro.ico
 $qm$\menu.ico
 $qm$\mes.ico
if(!ShowDialog("ToolbarEditor" &ToolbarEditor &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 301 178 "Dialog"
 7 Edit 0x54030080 0x200 32 140 266 14 ""
 3 ToolbarWindow32 0x54030801 0x0 0 0 301 17 ""
 4 RichEdit20A 0x54233044 0x200 0 20 226 106 ""
 5 Button 0x54032000 0x0 48 160 48 14 "Open"
 6 Button 0x54032000 0x0 100 160 48 14 "Save"
 8 Button 0x54032000 0x0 250 160 48 14 "Browse..."
 9 Static 0x54000000 0x0 2 142 28 12 "Toolbar"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

 TBSTYLE_FLAT

ret
 messages
int- htb hed
sel message
	case WM_INITDIALOG
	htb=id(3 hDlg)
	hed=id(4 hDlg)
	
	SendMessage hed EM_SETOPTIONS ECOOP_OR ECO_SELECTIONBAR
	SendMessage hed EM_SETEVENTMASK 0 ENM_CHANGE
	QmRegisterDropTarget(htb hDlg 16)
	
	rget _s "te_file"
	if(_s.len) _s.setwintext(id(7 hDlg)); TE_Open
	TE_Update
	
	case WM_DESTROY
	ImageList_Destroy SendMessage(htb TB_SETIMAGELIST 0 0)
	
	case WM_COMMAND goto messages2
	
	case WM_QM_DRAGDROP
	str s ss
	ss.getwintext(hed)
	QMDRAGDROPINFO& di=+lParam
	foreach s di.files
		s.expandpath(s 2)
		ss.addline(s)
	EditReplaceSel hed 0 ss 3
ret
 messages2
sel wParam
	case EN_CHANGE<<16|4
	TE_Update
	case 5 TE_Open
	case 6 TE_Save
	case 8 if(TO_Browse3(hDlg 7 "te_dir" "$my qm$" "XML files[]*.xml[]" "xml" _s)) rset _s "te_file"
	case IDOK
	case IDCANCEL
ret 1

