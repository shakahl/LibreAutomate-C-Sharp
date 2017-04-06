\Dialog_Editor
out
int flags=1|0x1000
int w=win("Registry Editor" "RegEdit_RegEdit")
 w=id(1 w)
RECT r
if(flags&1) GetClientRect w &r; else GetWindowRect w &r; OffsetRect &r -r.left -r.top
 if(r.right>100) r.right=100
 if(r.bottom>40) r.bottom=50
__MemBmp m.Create(r.right r.bottom)
int x y
for y 0 r.bottom
	out y
	if(!(flags&0x1000) and y=15) break
	for x 0 r.right
		SetPixel m.dc x y pixel(x y w flags)

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 368 230 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_PAINT
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	BitBlt ps.hDC 0 0 r.right r.bottom m.dc 0 0 SRCCOPY
	EndPaint hDlg &ps
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
