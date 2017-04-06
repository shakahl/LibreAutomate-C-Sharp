\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_2_trackbars" &dlg_2_trackbars)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 msctls_trackbar32 0x54030001 0x0 6 4 94 22 ""
 4 msctls_trackbar32 0x54030001 0x0 114 4 92 22 ""
 5 Static 0x54000000 0x0 6 30 48 12 ""
 6 Static 0x54000000 0x0 114 30 48 12 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SendMessage id(3 hDlg) TBM_SETRANGE 0 MakeInt(20 30)
	SendMessage id(4 hDlg) TBM_SETRANGE 0 MakeInt(20 30)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_HSCROLL ;;lParam is scrollbar handle
	int pos staticId
	sel(GetWinId(lParam)) case 3 staticId=5; case 4 staticId=6; case else ret
	pos=SendMessage(lParam TBM_GETPOS 0 0)
	str s=pos
	s.setwintext(id(staticId hDlg))
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
