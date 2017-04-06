\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_image_stretch" &dlg_image_stretch)) ret

 BEGIN DIALOG
 0 "" 0x92CF0AC8 0x0 0 0 257 47 "Dialog"
 3 Edit 0x54030080 0x200 8 10 96 14 ""
 4 Button 0x54012003 0x0 110 12 48 13 "Check"
 5 Static 0x54000000 0x0 166 12 48 13 "Text"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
__MemBmp-- t_mb
__GdiHandle-- t_hb
RECT r
sel message
	case WM_INITDIALOG
	t_hb=LoadPictureFile("$qm$\il_qm.bmp")
	SendMessage hDlg WM_SIZE 0 0
	
	case WM_SIZE
	GetClientRect hDlg &r
	t_mb.Attach(CopyImage(t_hb 0 r.right r.bottom 0)) ;;copy/stretch t_hb and attach to t_mb
	InvalidateRect hDlg 0 1
	
	case WM_ERASEBKGND
	GetClientRect hDlg &r
	BitBlt wParam 0 0 r.right r.bottom t_mb.dc 0 0 SRCCOPY
	ret DT_Ret(hDlg 1)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 antiflicker: set this style for dialog: WS_CLIPCHILDREN
