 /
function# iid FILTER&f

if wintest(f.hwnd "" "#32768")
	int hm=SendMessage(f.hwnd MN_GETHMENU 0 0)
	if hm
		POINT p; xm p
		int i=MenuItemFromPoint(0 hm p)
		int itemid=GetMenuItemID(hm i)
		if i>0
			out "system menu clic k: item position=%i, item id=0x%X" i itemid
			sel itemid
				case 666
				PostMessage win WM_CANCELMODE 0 0 ;;close menu
				out "my item"
				mac "my macro" "" f.hwnd itemid
				ret -1 ;;eat mouse event but don't run a macro
				
				case SC_MOVE
				PostMessage win WM_CANCELMODE 0 0 ;;close menu
				out "disabled Move"
				ret -1 ;;eat mouse event but don't run a macro

ret -2

 use GetMenuItemIdFromMouse
