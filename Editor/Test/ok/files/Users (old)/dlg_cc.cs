\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int h=win("dlg_cc" "#32770")
if(h) clo h; ret

hDlg=ShowDialog("dlg_cc" &dlg_cc 0 0 1)
opt waitmsg 1
wait 0 WD hDlg

 BEGIN DIALOG
 0 "" 0x80000040 0x8000080 0 0 227 151 "dlg_cc"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 get rectangles
	int hc=child(mouse); if(!hc) hc=win(mouse)
	RECT rc; GetWindowRect hc &rc
	max hDlg ;;also makes visible
	RECT rd; GetWindowRect hDlg &rd
	RECT ri
	if(!IntersectRect(&ri &rd &rc)) ret
	
	 create hole
	int rw=CreateRectRgn(0 0 rd.right rd.bottom)
	int r2=CreateRectRgn(ri.left ri.top ri.right ri.bottom)
	CombineRgn(rw rw r2 RGN_DIFF)
	SetWindowRgn hDlg rw 1
	DeleteObject r2
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
