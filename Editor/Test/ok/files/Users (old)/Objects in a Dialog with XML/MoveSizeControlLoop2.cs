 /Drag_drop_Dialog
function hDlg hctrl action mouseButton ;;action: 1 move, 2 resize.  mouseButton: 1 left, 2 right

MSG m; int mb;

RECT r; GetWindowRect hctrl &r
sel action
	case 1
	POINT po; xm po; po.x-r.left; po.y-r.top ;;get mouse offset in control
	case 2
	mou r.right r.bottom ;;move mouse to right-bottom

int hcur=LoadCursor(0 +IDC_CROSS)

SetCapture(hDlg);
rep
	if(GetCapture()!=hDlg || GetMessage(&m, 0, 0, 0)<=0) break
	if(m.message==WM_KEYDOWN && m.wParam==VK_ESCAPE) ReleaseCapture(); break

	mb=0;
	sel(m.message)
		case WM_MOUSEMOVE:
		POINT p; xm p
		sel action
			case 1
			ScreenToClient hDlg &p
			mov p.x-po.x p.y-po.y hctrl
			case 2
			siz p.x-r.left p.y-r.top hctrl
		SetCursor(hcur);
		continue;
		case WM_LBUTTONUP: mb=MK_LBUTTON
		case WM_RBUTTONUP: mb=MK_RBUTTON

	if(mb==mouseButton)
		ReleaseCapture();

	DispatchMessage(&m);
