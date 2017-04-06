function! $map $icons [flags] ;;flags: 0 list of icons, 1 bmp, 2 himagelist

 Adds icons to all menus in current thread.
 Returns: 1 success, 0 failed.

 map - string that maps menu item ids to icon indices. Format: "id=icon id=icon ...". Icon index is 0-based. Example: "5=0 10=1 3=2 4=1".
 icons - list of icons or imagelist. Depends on flags 1 and 2.
 flags:
   0 - icons is list of icon files, like "$qm$\mouse.ico[]shell32.dll,5". Slow, not recommended for big menus.
   1 - icons is imagelist file created with QM imagelist editor, like "$qm$\il_qm.bmp".
   2 - icons is imagelist handle. For example you can pass __ImageList variable.
      The imagelist must contain small icons (16x16).
      The function does not copy and does not destroy it.
      The imagelist will be used every time a menu is shown in current thread, therefore must not be destroyed too early.

 REMARKS
 Works with menu bar menus, popup menus (eg ShowMenu), and with system menu.
 Call once in thread, at any time before creating/showing menus.
 Fails if map syntax invalid or fails to load the bmp file. If fails to load an icon file, succeeds but does not display the icon.
 Tip: Use smaller icons for items that can be checked. On Windows XP there is not enough space for checkboxes. On other OS versions may be too.

 Added in: QM 2.4.2.


type ___TMICONS flags hh il pen ARRAY(POINT)map !isTheme ;;flags: 1 destroy imagelist
___TMICONS- ___t_mi

if(sub.AddIcons(___t_mi map icons flags)) ret 1
sub.Delete ___t_mi


#sub AddIcons
function! ___TMICONS&m $map $icons flags

if m.hh
	sub.Delete m
else
	m.hh=SetWindowsHookEx(WH_CBT &sub.HookCbtProc 0 GetCurrentThreadId)
	atend sub.Atend &m

int j; POINT p
rep
	p.x=val(map 0 j); if(!j) ret
	map+j
	if(map[0]='=') map+1; else ret
	p.y=val(map 0 j); if(!j) ret
	map+j
	m.map[]=p
	if(!map[0]) break

sel flags&3
	case 0 m.il=__ImageListLoad(icons 2); m.flags|1 ;;list of icons
	case 1 m.il=__ImageListLoad(icons); m.flags|1 ;;imagelist as bmp
	case 2 m.il=icons ;;imagelist as handle

if(!m.il) ret
m.pen=CreatePen(0 1 0xffa080)
ret 1


#sub Delete
function ___TMICONS&m
if(m.il and m.flags&1) ImageList_Destroy m.il; m.il=0
if(m.pen) DeleteObject m.pen; m.pen=0
m.map=0
m.flags=0


#sub Atend
function ___TMICONS&m
sub.Delete m
if(m.hh) UnhookWindowsHookEx m.hh; m.hh=0


#sub HookCbtProc
function# code wParam lParam
___TMICONS- ___t_mi
if code=HCBT_CREATEWND and GetClassLong(wParam GCW_ATOM)=32768
	SetWindowSubclass wParam &sub.SubclassProc 1 &___t_mi

ret CallNextHookEx(0 code wParam +lParam)


#sub SubclassProc
function# hWnd message wParam lParam uIdSubclass ___TMICONS&m

int R=DefSubclassProc(hWnd message wParam lParam)

 sel(message) case [MN_GETHMENU,WM_NCHITTEST] case else OutWinMsg message wParam lParam &_s; out "%s    <%i>" _s hwnd
sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hWnd &sub.SubclassProc uIdSubclass)
	
	case WM_PRINTCLIENT ;;sent after creation of main popup menu. Sent after creation of a submenu when invoked with mouse.
	sub.OnDraw(m hWnd wParam)
	
	case WM_PAINT ;;sent after creation of a submenu when invoked with keyboard
	sub.OnDraw(m hWnd)
	
	case 0x1E5 ;;undocumented NM_SELECTITEM, sent on mouse move, with wParam = item index if on item or -1 if not
	sub.OnDraw(m hWnd)
	
	case WM_KEYDOWN ;;keyboard navigation (arrows)
	sub.OnDraw(m hWnd)

ret R


#sub OnDraw
function ___TMICONS&m hwnd [dc]

int i hm=SendMessage(hwnd MN_GETHMENU 0 0)

 maybe does not have icon bar
MENUINFO mi.cbSize=sizeof(mi); mi.fMask=MIM_STYLE
if(!GetMenuInfo(hm &mi) or mi.dwStyle&MNS_NOCHECK) ret

int isTheme=sub.IsMenuTheme(m hm)

if(!dc) dc=GetDC(hwnd); int reldc=1

int op=SelectObject(dc m.pen)
int obr=SelectObject(dc GetStockObject(WHITE_BRUSH))

for(i 0 GetMenuItemCount(hm)) sub.DrawItem(m hwnd hm i dc isTheme)

SelectObject dc op
SelectObject dc obr

if(reldc) ReleaseDC hwnd dc


#sub DrawItem
function ___TMICONS&m hwnd hm item dc !isTheme

 get item id from index
MENUITEMINFOW mi.cbSize=sizeof(mi); mi.fMask=MIIM_ID; if(!GetMenuItemInfoW(hm item 1 &mi)) ret ;;info: GetMenuItemID does not get submenu id
if(!mi.wID) ret

 id -> icon index
int ii iid=mi.wID
for(ii 0 m.map.len) if(m.map[ii].x=iid) break
if(ii=m.map.len) ret
ii=m.map[ii].y

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
if(isTheme) xIcon=3; cxCheck=22
else xIcon=0; cxCheck=16
int yIcon=r.top+(r.bottom-r.top/2)-8

 draw custom checkbox
if GetMenuState(hm iid 0)&MFS_CHECKED
	RECT rr=r; rr.right=rr.left+cxCheck
	Rectangle dc rr.left rr.top rr.right rr.bottom

 draw icon ii from imagelist
ImageList_Draw(m.il ii dc r.left+xIcon yIcon 0)


#sub IsMenuTheme
function! ___TMICONS&m hm

if(_winnt<6) ret

 is classic theme or old common controls?
if(!m.isTheme) m.isTheme=1; int h=OpenThemeData(0 L"menu"); if(h) CloseThemeData h; m.isTheme=2
if(m.isTheme!2) ret

 is multicolumn?
int i
for i 0 GetMenuItemCount(hm)
	if(GetMenuState(hm i MF_BYPOSITION)&(MFT_MENUBARBREAK|MFT_MENUBREAK)) ret

ret 1
