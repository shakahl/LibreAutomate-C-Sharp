 /
function# [str&itemText] [int&popupMenuHandle]

 Gets id of menu item from mouse.
 The id can be used with QM function men().
 Works only with standard menus.
 Returns 0 if fails, eg if mouse is not on a menu.


POINT p
GetCursorPos(&p)
int h=WindowFromPoint(p.x p.y)
if(GetClassLongW(h GCW_ATOM)!=32768) ret

 Acc a=acc(mouse)
 outw child(a)
GUITHREADINFO g.cbSize=sizeof(g)
if GetGUIThreadInfo(GetWindowThreadProcessId(h 0) &g)
	outw g.hwndMenuOwner
	 if(hm=GetSystemMenu
	if(g.flags&GUI_SYSTEMMENUMODE) out "<system menu>"

int hm=SendMessage(h MN_GETHMENU 0 0); if(!hm) ret
PhysicalToLogicalPoint hwnd &p
int i=MenuItemFromPoint(0 hm p); if(i=-1) ret
i=GetMenuItemID(hm i); if(i=-1) ret

if(&popupMenuHandle) popupMenuHandle=hm
if(&itemText) MenuGetString hm i &itemText

ret i
