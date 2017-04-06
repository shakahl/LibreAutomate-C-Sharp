 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTreeView32 0x54010027 0x204 6 4 96 48 ""
 4 Edit 0x54231044 0x204 108 4 112 48 "Tex"
 5 Edit 0x54032000 0x200 188 80 32 14 "Ind"
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""

ret
 messages
ARRAY(MYTVITEMDATA)- t_a
int i htv hpar
sel message
	case WM_INITDIALOG
	
	 populate tree view with t_a array data
	if(t_a.len)
		htv=id(3 hDlg)
		for i 0 t_a.len
			if(t_a[i].issubitem)
				t_a[i].htvitem=TvAdd(htv hpar t_a[i].label i)
			else
				t_a[i].htvitem=TvAdd(htv 0 t_a[i].label i); hpar=t_a[i].htvitem
		SendMessage htv TVM_SELECTITEM TVGN_CARET t_a[0].htvitem
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	sel nh.code
		case TVN_SELCHANGED
		NMTREEVIEW* ntv=+nh
		i=ntv.itemNew.lParam
		SetDlgItemInt(hDlg 5 i 0)
		t_a[i].data.setwintext(id(4 hDlg))
