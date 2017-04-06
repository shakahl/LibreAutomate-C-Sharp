 /
function __OnScreenRect&osr [Acc&get] [Acc&usethis]

___EA- dA
Acc a; POINT p; RECT r; int i haveRect

if(&usethis) a=usethis
else
	xm p
	int w=win(p.x p.y)
	if(!SendMessageTimeout(w 0 0 0 SMTO_ABORTIFHUNG 500 &i)) goto erase
	
	if(!&get and w!dA.hwndCapturing and sub.CaptureSpec(w)) dA.hwndCapturing=w
	
	i=EA_Smallest(2 0 p)
	if(i>=0) a=dA.ar[i].a; r=dA.ar[i].r; haveRect=1
	else a=acc(p.x p.y 0)

int osrFlags
if(&get) sub.Alt a r; get=a; osrFlags=2
else if(!haveRect and !EA_GetRect(a r 1)) goto erase

osr.Show(osrFlags &r)
ret
err+
 erase
osr.Show(3)


#sub Alt
function Acc&a RECT&r

Acc aa; a.Navigate("pa" aa)
 RECT rr; if(!EA_GetRect(aa rr)) ret
 if(memcmp(&r &rr sizeof(RECT))) ret
int ro=aa.Role; if(ro!ROLE_SYSTEM_LINK and ro!ROLE_SYSTEM_PUSHBUTTON) ret
 _s=a.Name; if(_s.len and _s!aa.Name) ret
a=aa
err+


#sub CaptureSpec
function# w

sel WinTest(w "Chrome*")
	case 1
	mac "sub.ChromeEnable" "" w
	
	case else
	 JAB workaround. May not give object from point if w is a Java window.
	POINT p; xm p w 1
	PostMessage w WM_MOUSEMOVE 0 MakeInt(p.x p.y)

err+
ret 1


#sub ChromeEnable
function w
Acc a.Find(w "DOCUMENT" "" "" 0x2000)
err
