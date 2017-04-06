\Dialog_Editor

 Shows how to accept only mp3 files.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_tv_drop_files" &dlg_tv_drop_files 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTreeView32 0x54030000 0x0 0 0 116 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget(id(3 hDlg) hDlg 1)
	
	case WM_QM_DRAGDROP ;;we receive this message on drag enter (flag 1) and on drop (always)
	QMDRAGDROPINFO& di=+lParam
	di.effect=0
	ARRAY(str) a amp3
	if(di.GetFiles(a wParam=3) and GetMp3Files(a amp3))
		di.effect=4 ;;shortcut
		if(wParam=3) ;;on drop
			int i htv=id(3 hDlg)
			for i 0 amp3.len
				TvAdd(htv 0 amp3[i])
	
	ret DT_Ret(hDlg 1)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
