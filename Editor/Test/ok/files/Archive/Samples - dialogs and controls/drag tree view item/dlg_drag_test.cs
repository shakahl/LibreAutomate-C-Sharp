\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_drag_test" &dlg_drag_test)) ret

 note: this dialog is Unicode and therefore received W versions of messages. But the function can work with ANSI dialogs too.

 BEGIN DIALOG
 1 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTreeView32 0x54030000 0x0 0 0 116 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int i htv=id(3 hDlg)
	str s ss="one[]two[]three[]βββ"
	foreach(s ss) i+1; TvAdd htv 0 s i
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 gett text of all
	htv=id(3 hDlg)
	int hi=SendMessage(htv TVM_GETNEXTITEM TVGN_ROOT 0)
	rep
		if(!hi) break
		TvGetItemText htv hi s
		ss.addline(s)
		hi=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT hi)
	out ss
	
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	NMTREEVIEW* nt=+nh
	if(nh.code=TVN_SELCHANGEDW or nh.code=TVN_SELCHANGED)
		i=nt.itemNew.lParam ;;was set by TvAdd
		out i
	else if(nh.code=TVN_BEGINDRAGW or nh.code=TVN_BEGINDRAG)
		type TVDRAG49 htv hidrag
		TVDRAG49 td.htv=nh.hwndFrom; td.hidrag=nt.itemNew.hItem
		Drag(hDlg &drag_test_proc &td)
		SendMessage td.htv TVM_SELECTITEM TVGN_DROPHILITE 0
