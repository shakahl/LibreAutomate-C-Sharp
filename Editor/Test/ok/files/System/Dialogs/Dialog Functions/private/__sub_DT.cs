#sub SetControl
function# hDlg controlId str&s [flags] [__DIALOG*d] ;;flags: 0x80000000 called by DT_SetControls, 0x40000000 called by ShowDialog->DT_SetControls

 Sets dialog control data like ShowDialog does.
 Returns 1. Returns 0 if the control does not exist.

 hDlg - dialog handle. If controlId 0 - control handle.
 controlId - control id, or 0 if hDlg is control handle.


opt noerrorshere

str cls
int j h stl handle img v2 calledbyShowDialog=flags&0x40000000

h=iif(controlId GetDlgItem(hDlg controlId) hDlg); if(!h) ret

if flags&0x80000000=0
	if(!controlId) hDlg=GetParent(h)
	if(GetClassLong(hDlg GCW_ATOM)=32770) flags=GetWindowContextHelpId(hDlg)
v2=flags&4

sel cls.getwinclass(h) 1
	case "Button"
	stl=GetWinStyle(h)&BS_TYPEMASK
	if (stl>1 and stl<7) or stl=9
		j=val(s)
		if(j or !calledbyShowDialog) SendMessage(h BM_SETCHECK iif(v2 j j!0) 0)
		ret 1
	
	case "ComboBox"
	TO_CBFill h s
	ret 1
	
	case "ListBox"
	stl=GetWinStyle(h)&LBS_MULTIPLESEL
	TO_CBFill h s iif(stl 2 1)
	ret 1
	
	case "Static"
	if(!calledbyShowDialog) ret 1
	img=0
	sel GetWinStyle(h)&SS_TYPEMASK
		case SS_ICON
		int gfi=s.beg("&")
		if(s.endi(".cur") or s.endi(".ani")) gfi|4 ;;later DestroyIcon anyway, it's the same
		handle=GetFileIcon(s+(gfi&1) 0 gfi); if(!handle) ret 1
		SendMessage(h STM_SETICON handle 0)
		img=1
		case SS_BITMAP
		_i=LoadPictureFile(s); if(!_i) ret 1
		SendMessage(h STM_SETIMAGE IMAGE_BITMAP _i)
		handle=SendMessage(h STM_GETIMAGE IMAGE_BITMAP 0)
		if(handle!=_i) DeleteObject _i ;;copies if transparent
		img=2
	
	if img
		__DIALOGHANDLE& dh=d.ah[]; dh.handle=handle; dh.flags=img
		ret 1
	
	case ["RichEdit20A","RichEdit20W","RichEdit50W"]
	SendMessage h EM_LIMITTEXT 0x7FFFFFFE 0
	if(s.beg("&") and RichEditLoad(h s+1)) ret 1
	
	case "QM_Grid"
	SendMessage(h GRID.LVM_QG_SETGETALL_CSVSTRING 1 s)
	ret 1
	
	case "msctls_hotkey32"
	if v2
		int vk mod
		TO_HotkeyFromQmKeys(s mod vk)
		SendMessage h HKM_SETHOTKEY mod<<8|vk 0
		ret 1
	
	case else
	if QmSetWindowClassFlags(cls 0x80000000)&8
		SendMessageW(h RegisterWindowMessage("WM_QM_DIALOGCONTROLDATA") 1 &s)
		ret 1

s.setwintext(h)

ret 1


#sub GetControl
function! hDlg controlId str&s [flags] ;;flags: 0x80000000 called by DT_GetControls, 4 v2

 Gets dialog control data like ShowDialog does.
 Returns 1. Returns 0 if the control does not exist.

 hDlg - dialog handle. If controlId 0 - control handle.
 controlId - control id, or 0 if hDlg is control handle.


opt noerrorshere

int j m h v2
str t.flags=1

h=iif(controlId GetDlgItem(hDlg controlId) hDlg); if(!h) ret

if flags&0x80000000=0
	if(!controlId) hDlg=GetParent(h)
	if(GetClassLong(hDlg GCW_ATOM)=32770) flags=GetWindowContextHelpId(hDlg)
v2=flags&4

sel t.getwinclass(h) 1
	case "Button"
	sel(GetWinStyle(h)&BS_TYPEMASK) case [2,3,4,5,6,9] s=SendMessage(h BM_GETCHECK 0 0)
	
	case "ComboBox"
	j=SendMessage(h CB_GETCURSEL 0 0)
	if(v2 and GetWinStyle(h)&3!=CBS_DROPDOWNLIST) s.getwintext(h)
	else s=j; t.getwintext(h); if(t.len) s+" "; s+t
	
	case "ListBox"
	if GetWinStyle(h)&LBS_MULTIPLESEL
		m=SendMessage(h LB_GETCOUNT 0 0); if(m<0) m=0
		s.all(m 2 '0')
		j=SendMessage(h LB_GETSELCOUNT 0 0)
		if j>0 and j<=m
			t.all(j*4 2); int* selitems=+t
			SendMessage(h LB_GETSELITEMS j selitems)
			for(m 0 j) s[selitems[m]]='1'
	else
		j=SendMessage(h LB_GETCURSEL 0 0)
		s=j
		if(j>=0)
			sub_sys.CBLB_GetItemText(h j t 1)
			if(t.len) s+" "; s+t
	
	case ["Edit","RichEdit20A","RichEdit20W","RichEdit"]
	s.getwintext(h); if(GetWinStyle(h)&ES_PASSWORD) s.__set_secure
	
	case "QM_Grid"
	if(!SendMessage(h GRID.LVM_QG_SETGETALL_CSVSTRING 0 &s)) s=""
	
	case ["Static","ActiveX","QM_DlgInfo"]
	
	case "msctls_hotkey32"
	if(!v2) goto gSWT
	j=SendMessage(h HKM_GETHOTKEY 0 0)
	if j
		QmKeyCodeFromVK j&0xff s
		if(j&0x400) s-"A"
		if(j&0x100) s-"S"
		if(j&0x200) s-"C"
	else s.all
	
	case else
	if QmSetWindowClassFlags(t 0x80000000)&8
		if(!SendMessageW(h RegisterWindowMessage("WM_QM_DIALOGCONTROLDATA") 0 &s)) s.all
	else
		 gSWT
		s.getwintext(h)
ret 1


#sub ParseControlsVar
function! __DIALOG&d str*p ARRAY(int)&acid str&errStr [validateIds]

 Parses and validates string of control variable ids (p[0]) and control variables (p[n]).
 Gets the specified control ids to acid, which can be d.acid or another variable.
 If validateIds, uses d.acid (it contains control ids specified in dialog template) to find invalid ids specified in p[0]. Else does not search for invalid ids.
 Returns 1. If error, sets errStr and returns 0.
 Returns 1 if p is 0 or p[0] is empty.

errStr.all
ARRAY(lpstr) a
if(!p or !tok(p[0] a -1 "" 0x1000)) acid=0; ret 1
int i j cid

for i 0 a.len
	cid=val(a[i] 0 j); if(!j) ret
	str& s=p[i+1]
	if(s.len>s.nc or s.len<0 or (s.lpstr and IsBadWritePtr(s.lpstr 1)) or s.len!=len(s)) ret
	if validateIds and cid
		ARRAY(int)& r=d.acid
		for(j 0 r.len) if(r[j]=cid) break
		if(j=r.len) errStr=F" The dialog doesn't have a control with id {cid}"; ret
	a[i]=+cid

acid.create(a.len)
memcpy &acid[0] &a[0] a.len*4

ret 1
err+ errStr=_error.description


#sub CompileDialog
function# $dd str&so [cbFunc] [cbParam]

 Takes text that contains dialog definition, and returns binary dialog template in so.
 Returns nonzero if successful.

if(findrx(dd __S_RX_DD 0 0 _s 1)<0) ret
ret __CompileDialogDefinition(_s &so cbFunc cbParam)


#sub SetHook
 Sets WH_GETMESSAGE hook with sub.HookProc in current thread.
 Used by several dialog functions that need messages as they are retrieved in message loop.
 We cannot access default dialog message loop. It's better to use hook than to replace it. Works with modeless dialog too.
 This func sets hook once in thread. Does not unhook when dialog closed.


__WindowsHook-- hh=SetWindowsHookEx(WH_GETMESSAGE &sub.__HookProc 0 GetCurrentThreadId)


#sub __HookProc
function# code wParam MSG*m

if code=0 and wParam=1 and m.hwnd ;;HC_ACTION and PM_REMOVE
	sel m.message
		case [WM_KEYDOWN,WM_SYSKEYDOWN,WM_CHAR]
		int hDlg=GetAncestor(m.hwnd 2)
		__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
		if d and ((d.haccel2 and TranslateAccelerator(hDlg d.haccel2 m)) or (d.haccel and TranslateAccelerator(hDlg d.haccel m)))
			m.hwnd=0; m.message=0
			ret 1

ret CallNextHookEx(0 code wParam m)


#sub Colors
function'__DIALOGCOLORS* hDlg


__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
if(!d.colors) d.colors._new; d.colors.func=&sub.__DlgProcColors
ret d.colors


#sub __DlgProcColors
function# __DIALOGCOLORS*p hDlg message wParam lParam

int i; RECT r
 OutWinMsg message wParam lParam
sel message
	case WM_ERASEBKGND
	if(p.bkFlags&3) goto gErase
	
	case [WM_CTLCOLORSTATIC,WM_CTLCOLOREDIT,WM_CTLCOLORLISTBOX]
	for(i 0 p.a.len) if(p.a[i].x=lParam) SetTextColor wParam p.a[i].y; break
	if(i<p.a.len or (message=WM_CTLCOLORSTATIC and p.bkFlags&3)) goto gControl ;;when drawing background, all static must be transparent
	
	case WM_SIZE
	if GetCapture!hDlg ;;anti flicker
		InvalidateRect hDlg 0 1
	case WM_SIZING
	p.bkFlags|0x10000
	case WM_EXITSIZEMOVE
	if(p.bkFlags&0x10000) p.bkFlags~0x10000; InvalidateRect hDlg 0 1

ret

 gErase
GetClientRect hDlg &r
sel p.bkFlags&3
	case 3
	POINT wp wp0; GetViewportOrgEx wParam &wp; SetBrushOrgEx wParam wp.x wp.y &wp0
	FillRect wParam &r p.hBrush
	SetBrushOrgEx wParam wp0.x wp0.y 0
	
	case [1,2]
	TRIVERTEX x1 x2; int R G B
	memcpy &x2 &r.right 8
	ColorToRGB p.color R G B; x1.Red=R<<8; x1.Green=G<<8; x1.Blue=B<<8
	ColorToRGB p.color2 R G B; x2.Red=R<<8; x2.Green=G<<8; x2.Blue=B<<8
	GRADIENT_RECT r1.LowerRight=1
	GdiGradientFill wParam &x1 2 &r1 1 p.bkFlags&2!0
ret DT_Ret(hDlg 1)

 gControl
if(message!WM_CTLCOLORSTATIC) ret GetSysColorBrush(COLOR_WINDOW)
str cls.getwinclass(lParam)
if(cls~"Edit") SetBkColor wParam GetSysColor(COLOR_BTNFACE); ret GetSysColorBrush(COLOR_BTNFACE) ;;readonly edit. Would be problems with scrolling. Also, better to not draw gradient/image, or text may be unreadable.

int dc direct(1) color(GetTextColor(wParam))

if(cls~"Static" and GetWinStyle(lParam)&SS_TYPEMASK=SS_ICON) direct=0 ;;for static icon, null brush makes transparent area black, therefore cannot draw directly

GetClientRect lParam &r
if(direct) dc=wParam
else __MemBmp mb.Create(r.right r.bottom dc); dc=mb.dc

 draw dialog background under this control to dc
 This code replaces DrawThemeParentBackground, which is not on Win 2000. DrawThemeParentBackground also sends WM_PRINTCLIENT. This code is faster.
MapWindowPoints lParam hDlg +&r 2; SetViewportOrgEx dc -r.left -r.top &wp0
SendMessage hDlg WM_ERASEBKGND dc 0
SetViewportOrgEx dc wp0.x wp0.y 0

SetBkMode wParam 1
SetTextColor wParam color ;;because erasebkgnd may reset

if(direct) ret GetStockObject(NULL_BRUSH)

p.hBrushForControl.Delete; p.hBrushForControl=CreatePatternBrush(mb.bm)
ret p.hBrushForControl

 notes:
 Cannot simply return null brush without copying from dialog, because then does not redraw control background when changed text or size. Also, checkbox may be black on XP.
 If not direct, this code is slower ~150%, and all drawing ~20%.
