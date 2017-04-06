 /
function hDlg imageList $buttons

 Sets to draw icons on dialog buttons using an imagelist.

 hDlg - dialog handle.
 imageList - imagelist. Don't destroy too early. You can create imagelists with the imagelist editor (floating toolbar -> more tools) or at run time (call t_il.Create).
 icons - list of buttonId=imageIndex like "3=0 4=1 7=0".

 REMARKS
 Buttons must have space at the left side for icons.

 EXAMPLE
 	case WM_INITDIALOG
 	__ImageList- t_il.Load("$qm$\il_qm.bmp")
 	DT_SetButtonIcon hDlg t_il "1=25 2=32"


ARRAY(str) a
int i w x y
tok buttons a -1 " ="
for i 0 a.len/2
	x=val(a[i*2]) ;;id
	y=val(a[i*2+1]) ;;image index
	w=id(x hDlg 1); if(w=0) end F"control id {x} not found" 8; continue
	POINT* p
	if !GetWindowSubclass(w &sub.WndProc_Subclass 815476901 +&p)
		p._new
		SetWindowSubclass(w &sub.WndProc_Subclass 815476901 p)
	p.x=imageList; p.y=y
	if(IsWindowVisible(w)) InvalidateRect w 0 1


#sub WndProc_Subclass
function# hwnd message wParam lParam uIdSubclass POINT*p

int R=DefSubclassProc(hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hwnd &sub.WndProc_Subclass p)
	p._delete
	
	case WM_PAINT
	int dc=GetDC(hwnd)
	ImageList_Draw p.x p.y dc 4 4 ILD_TRANSPARENT
	ReleaseDC hwnd dc

ret R
