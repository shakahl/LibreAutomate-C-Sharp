 /Menu icons test
function hwnd hm item dc !isVistaTheme

 get item id from index
MENUITEMINFOW mi.cbSize=sizeof(mi); mi.fMask=MIIM_ID; if(!GetMenuItemInfoW(hm item 1 &mi)) ret ;;info: GetMenuItemID does not get submenu id
if(!mi.wID) ret

 id -> icon index
int ii iid=mi.wID
for(ii 0 m_map.len) if(m_map[ii].x=iid) break
if(ii=m_map.len) ret
ii=m_map[ii].y

 get item rect
RECT r
if(!GetMenuItemRect(hwnd hm item &r)) ret
MapWindowPoints 0 hwnd +&r 2
if(r.bottom-r.top<16 or r.right-r.left<16) ret

 problem with scrolling menus. Did not find a way to get correct y, but can prevent drawing in incorrect place.
POINT p.x=r.left+8; p.y=r.top+8; ClientToScreen hwnd &p
if(MenuItemFromPoint(hwnd hm p)!=item) ret

 get x y
int xIcon cxCheck
if(isVistaTheme) xIcon=3; cxCheck=22
else xIcon=0; cxCheck=16
int yIcon=r.top+(r.bottom-r.top/2)-8

 draw custom checkbox
if GetMenuState(hm iid 0)&MF_CHECKED
	RECT rr=r; rr.right=rr.left+cxCheck
	Rectangle dc rr.left rr.top rr.right rr.bottom

 draw icon ii from imagelist
ImageList_Draw(m_il ii dc r.left+xIcon yIcon 0)
