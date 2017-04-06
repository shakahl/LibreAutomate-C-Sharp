\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("DialogMultipageWithTrackbarVert" &DialogMultipageWithTrackbarVert)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 265 163 "Dialog"
 1001 Static 0x54020000 0x4 106 4 48 13 "Page0"
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 3 msctls_trackbar32 0x54030003 0x0 4 4 22 85 ""
 5 Static 0x54000010 0x20004 4 138 256 1 ""
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "0" ""

ret
 messages
int npages=3 ;;change this
sel message
	case WM_INITDIALOG
	SendMessage id(3 hDlg) TBM_SETRANGE 0 npages-1<<16
	DT_Page hDlg 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_VSCROLL
	if(wParam&0xffff=TB_ENDTRACK) ret ;;to change behavior, insert ! before =
	DT_Page hDlg SendMessage(id(3 hDlg) TBM_GETPOS 0 0)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

