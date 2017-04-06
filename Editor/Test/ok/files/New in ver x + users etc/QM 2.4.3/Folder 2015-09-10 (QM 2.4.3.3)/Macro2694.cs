ARRAY(int) aw; GetMainWindows aw
ARRAY(__Hicon) ai.create(aw.len)
str s t
int i h w c
for i 0 aw.len
	w=aw[i]
	h=GetWindowIcon(w)
	ai[i]=h
	t.getwintext(w)
	t.findreplace(" :" " [91]58]") ;;escape :
	t.escape(1) ;;escape " etc
	s+F"{t} :act {w}; err * {ai[i]}[]"
 out s

mac "sub.SelectMenuItem"
i=DynamicMenu(s "" 1)
if(i) outw aw[i-1]


#sub SelectMenuItem
int w=wait(10 WV win("" "#32768" "qm"))
key DD ;;Down arrow 2 times
