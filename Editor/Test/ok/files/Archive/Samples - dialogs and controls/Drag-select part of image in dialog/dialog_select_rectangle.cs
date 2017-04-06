\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
rget sb3 "Wallpaper" "Control Panel\Desktop"
if(!ShowDialog("dialog_select_rectangle" &dialog_select_rectangle &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x5400000E 0x0 0 0 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_LBUTTONDOWN goto dragCrop
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 dragCrop
type DSR2387 hwnd RECT'r __Hdc'hdc !notFirstTime
DSR2387 d.hwnd=id(3 hDlg)
POINT p; xm p d.hwnd 1; memcpy &d.r &p 8
d.hdc.Init(d.hwnd)

int ok=Drag(hDlg &DSR_Drag &d)
if(!ok or IsRectEmpty(&d.r)) ret

__MemBmp mb.Create(d.r.right-d.r.left d.r.bottom-d.r.top d.hdc d.r.left d.r.top)
SaveBitmap mb.bm "$temp$\crop.bmp"
run "$temp$\crop.bmp"

ret
