 /
function# hWnd message wParam lParam

type OUTEXTAB ~name ~s hedit tt flags
type OUTEX ARRAY(OUTEXTAB)a itab __Font'font htab Tray'tray

OUTEX-- d
OUTEXTAB& t
int i j h
str s1 s2
TCITEMW ti

 OutWinMsg message wParam lParam
sel message
	case WM_CREATE
	d.htab=CreateControl(0 "SysTabControl32" 0 0x54030040 0 0 0 0 hWnd 3)
	d.itab=-1
	d.font.Create("Courier New" 8)
	d.tray.AddIcon("" s1.getwintext(hWnd) 5 hWnd)
	h=GetSystemMenu(hWnd 0)
	AppendMenuW h MF_SEPARATOR 0 0
	AppendMenuW h 0 1001 @"On Top"
	AppendMenuW h 0 1002 @"Exit"
	
	case WM_CLOSE
	if(wParam=0) min hWnd; hid hWnd; ret ;;when trying to close, instead hide
	
	case WM_DESTROY PostQuitMessage 0
	 case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	
	case WM_WINDOWPOSCHANGED
	 save/restore window pos when hiding/showing
	WINDOWPOS& p=+lParam
	sel(p.flags&(SWP_SHOWWINDOW|SWP_HIDEWINDOW)) case SWP_SHOWWINDOW i=1; case SWP_HIDEWINDOW i=2
	if(i) if(!RegWinPos(hWnd "QM outex windowpos" "" i-1) and i=1) mov 0 0 hWnd
	
	case WM_SIZE
	 autosize controls
	if(!min(hWnd))
		RECT r rt; GetClientRect hWnd &r
		if(d.a.len) SendMessageW d.htab TCM_GETITEMRECT 0 &rt; j=rt.bottom+2
		MoveWindow d.htab 0 0 r.right j 1; j+2
		for(i 0 d.a.len) MoveWindow d.a[i].hedit 0 j r.right r.bottom-j 1
	
	case WM_SETCURSOR
	if(d.itab<0) goto gDef
	 remove * from tab name
	&t=d.a[d.itab]; err goto gDef
	if(t.flags&1)
		t.flags~1
		ti.mask=TCIF_TEXT; ti.pszText=@t.name
		SendMessageW d.htab TCM_SETITEMW d.itab &ti
	
	case WM_USER+101 ;;tray icon
	sel lParam
		case WM_LBUTTONUP act hWnd; err
		case WM_RBUTTONUP sel(PopupMenu("Exit")) case 1 DestroyWindow hWnd
		case WM_MOUSEMOVE if(d.tray.param&1) d.tray.param~1; d.tray.Modify(0 s1.getwintext(hWnd)) ;;window text changed
	ret
	
	case WM_SYSCOMMAND ;;from system menu
	sel wParam
		case 1001 ;;ontop
		i=ont(hWnd)
		if(i) ont- hWnd; else ont hWnd
		CheckMenuItem GetSystemMenu(hWnd 0) 1001 iif(i 0 MF_CHECKED)
		ret
		
		case 1002 ;;exit
		DestroyWindow hWnd
		ret
	 
	case WM_CONTEXTMENU
	 show menu on right click
	i=GetWinId(wParam)
	if(i=3) ;;tab
		TCHITTESTINFO tht; xm tht.pt wParam 1
		j=SendMessageW(wParam TCM_HITTEST 0 &tht); if(j<0) ret
		i=PopupMenu("Delete tab" 0 0 0 0 hWnd)
		sel i
			case 1 ;;delete tab
			if(j=d.itab) 
				i=j+1; if(i=d.a.len) i=j-1
				SendMessageW(wParam TCM_SETCURFOCUS i 0)
			
			SendMessageW(wParam TCM_DELETEITEM j 0)
			DestroyWindow d.a[j].hedit
			d.a.remove(j)
			d.itab=SendMessageW(wParam TCM_GETCURSEL 0 0)
	else if(i>=100) ;;edit
		i=PopupMenu("Clear" 0 0 0 0 hWnd)
		sel i
			case 1 ;;clear
			d.a[d.itab].s=""
			_s.setwintext(wParam)
	ret
	
	case WM_SETTEXT
	 if wParam is 1, it is sent by outex
	sel wParam
		case 1 goto ontext
		case 0 d.tray.param|1
	
	case WM_TIMER
	if(wParam>=100) goto ontimer ;;async display t.s
	ret
 gDef
ret DefWindowProcW(hWnd message wParam lParam)

 _______________________________________________________

 messages3
NMHDR* nh=+lParam
sel nh.code
	case TCN_SELCHANGE
	 display selected tab
	hid d.a[d.itab].hedit; err
	d.itab=SendMessageW(d.htab TCM_GETCURSEL 0 0)
	hid- d.a[d.itab].hedit; err
ret

 _______________________________________________________

 ontext

word* w=+lParam ;;UTF-16
 get tab name
word* w2=wcschr(w ':')
int clear=!w2
str sth
if(clear) sth.ansi(w); else sth.ansi(w _unicode w2-w/2)

 find tab by name
for(i 0 d.a.len) if(sth~d.a[i].name) break

if(i=d.a.len) ;;not found. Add new tab.
	if(clear) ret
	
	ti.mask=TCIF_TEXT; ti.pszText=@sth
	SendMessageW d.htab TCM_INSERTITEMW i &ti
	
	&t=d.a[]
	t.name=sth
	t.hedit=CreateControl(0 "RichEdit20W" 0 ES_MULTILINE|WS_VSCROLL|WS_HSCROLL|ES_AUTOVSCROLL|ES_READONLY 0 0 0 0 hWnd 100+i d.font)
	if(d.a.len=1) d.itab=0; else hid t.hedit ;;create new tabs hidden
	outex_window_proc hWnd WM_SIZE 0 0 ;;autosize
	SendMessageW t.hedit EM_LIMITTEXT 1024*1024*50 0
else &t=d.a[i]
	
if(clear)
	t.s=""
	t.s.setwintext(t.hedit)
else
	int sync; if(!t.s.len) t.tt=GetTickCount; else sync=GetTickCount-t.tt>=50
	 accumulate new text in t.s
	t.s+_s.ansi(w2+2)
	 display t.s asynchronously or synchronously, depending on when displayed last time
	if(!sync) SetTimer hWnd 100+i 50 0
	else outex_window_proc hWnd WM_TIMER i+100 0
ret

 _______________________________________________________

 ontimer

KillTimer hWnd wParam
wParam-100
&t=d.a[wParam]; err ret
h=t.hedit

 limit edit control text length
GETTEXTLENGTHEX gtl.codepage=1200; i=SendMessageW(h EM_GETTEXTLENGTHEX &gtl 0)+t.s.len
if(i>OUTEX_MAX)
	s2="...[]"
	if(t.s.len<OUTEX_MAX)
		s1.getwintext(h)
		s2.setwintext(h)
		t.s-s1
	else
		s2.setwintext(h)
	i=t.s.len-(0.75*OUTEX_MAX)
	j=findc(t.s 10 i)+1; if(j=0) j=i
	t.s.get(t.s j)

 add t.s to the end of the edit control, clear t.s, and scroll to the end
SendMessageW h EM_SETSEL -1 -1
SendMessageW h EM_REPLACESEL 0 @t.s
t.s=""
SendMessageW h WM_VSCROLL SB_BOTTOM 0

 add * to tab name. Will remove on WM_SETCURSOR.
if(t.flags&1=0)
	ti.mask=TCIF_TEXT; ti.pszText=@sth.from(t.name "*")
	SendMessageW d.htab TCM_SETITEMW wParam &ti
	t.flags|1

ret
