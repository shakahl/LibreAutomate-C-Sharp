 /
function# str&s $tabNameRX

rep
	Acc a.FromMouse
	int r=a.Role
	if(r=ROLE_SYSTEM_TEXT) break
	 outAcc a;; ret
	if(r!=ROLE_SYSTEM_APPLICATION) break ;;sometimas role APPLICATION, maybe when web page acc objects disconnected
	0.1

 try to get parent LINK, because the link may contain several TEXT that should be joined, eg TEXT WHITESPACE TEXT
Acc ap; a.Navigate("parent" ap)
ap.Role(_s); if(_s="LINK") a=ap

s=a.Name
s.trim
 out s; ret

if(s.begi("namas ")) s.remove(0 6)
else if(s.begi("sodyba ")) s.remove(0 7)
else if(s.begi("sodo namas ")) s.remove(0 11)

if(s.end("ai")) s.replace("ų k." s.len-2)
else if(s.end("ys")) s.replace("ių k." s.len-2)
else if(s.end("is")) s.replace("io k." s.len-2)
else if(s.end("ės")) s.replace("ių k." s.len-3)


int w=win("" "MozillaWindowClass")

 aruodas.lt now no city, let's get it from window name
str k.getwintext(w)
if k.beg("Namai ") and findrx(k "^Namai (\S+) -" 0 0 k 1)
	s+", "; s+k

a.Find(w "PAGETAB" tabNameRX "" 0x1002 2)
err
	w=win("maps.lt - Mozilla Firefox" "MozillaWindowClass") ;;maybe the tab is in separate window
	if(w) act w; ret w
	ret
a.DoDefaultAction
ret w
