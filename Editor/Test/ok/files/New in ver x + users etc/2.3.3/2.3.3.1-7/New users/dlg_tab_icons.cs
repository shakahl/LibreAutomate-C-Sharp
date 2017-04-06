\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_tab_icons)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTabControl32 0x54030040 0x0 0 0 224 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__ImageList-- il.Load("$qm$\il_qm.bmp") ;;load an imagelist craeted with QM imagelist editor
	int htb=id(3 hDlg)
	SendMessage htb TCM_SETIMAGELIST 0 il
	TCITEM ti.mask=TCIF_TEXT|TCIF_IMAGE
	ti.pszText="A"; ti.iImage=2; SendMessage htb TCM_INSERTITEMA 0 &ti
	ti.pszText="B"; ti.iImage=3; SendMessage htb TCM_INSERTITEMA 0 &ti
	ti.pszText="C"; ti.iImage=15; SendMessage htb TCM_INSERTITEMA 0 &ti
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
