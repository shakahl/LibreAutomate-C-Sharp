 /Dialog_Editor
function# Exstyle $Class $Text Style cx cy Id [flags] [x] [y] ;;flags: 1 re-create (not dlg units, no reposition-zorder), 2 no dlg units

if flags&3=0
	RECT r; SetRect &r x y x+cx y+cy; MapDialogRect(_hform &r)
	cx=r.right-r.left; cy=r.bottom-r.top; x=0

int h=CreateWindowExW(Exstyle @Class @Text (Style|WS_CHILD|WS_CLIPSIBLINGS|WS_VISIBLE) x y cx cy _hform Id _hinst 0)
if(!h) ret

if(flags&1=0) sub.ZorderControl(h Id)

SendMessage(h WM_SETFONT SendMessage(_hform WM_GETFONT 0 0) 0)
 note: SysIPAddress32 bug: deletes font when destroying.

ret h


#sub ZorderControl
function h Id

int i n hh(h) iAfter(-1) hAfter page(sub.IdToPage(Id)) p
ARRAY(POINT) a
rep
	hh=GetWindow(hh GW_HWNDPREV); if(!hh) break
	POINT& _p=a[]; _p.x=hh; _p.y=sub.IdToPage(GetDlgCtrlID(hh))
n=a.len; if(!n) ret

 zorder h to bottom of current page etc
if page>=0
	for(p page -1 -1) for(i 0 n) if(a[i].y=p) iAfter=i; goto g1 ;;find last control in this page or in a page before
	for(i n-1 -1 -1) if(a[i].y>=0) iAfter=iif(i<n-1 i+1 -1); break ;;first page, empty. Find first control of any page, and insert before; if not found - top.
	 g1
	if(iAfter>=0) hAfter=a[iAfter].x
else hAfter=HWND_BOTTOM

 zorder h above groupbox etc
if sub.IsZOrderBottomControl(h)
	if(page<0) ret
else if(hAfter)
	for i 0 n
		if(a[i].y!page) continue
		if(!sub.IsZOrderBottomControl(a[i].x)) break
		hAfter=iif(i<n-1 a[i+1].x 0)

 outw hAfter
SetWindowPos(h hAfter 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE)


#sub IdToPage
function# Id

ret iif(Id<1000 -1 Id-1000/100)


#sub IsZOrderBottomControl
function! h

 Returns 1 if controls of this type should be at bottom of Z order.

str s.getwinclass(h)
int st=GetWinStyle(h)
sel s 1
	case "Button" sel(st&BS_TYPEMASK) case BS_GROUPBOX ret 1; case else sel(GetDlgCtrlID(h)) case [IDOK,IDCANCEL] ret 1
	case "Static" sel(st&SS_TYPEMASK) case [SS_ETCHEDHORZ,SS_ETCHEDVERT] ret 1
	case "SysTabControl32" ret 1
