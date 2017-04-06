 Shows dynamically created menu of windows.

 out
ARRAY(int) aw; GetMainWindows aw
 out "---"
ARRAY(__Hicon) ai.create(aw.len)
str s t
int i h w c
for i 0 aw.len
	w=aw[i]
	 outw w
	 PF
	 if wintest(w "" "ApplicationFrameWindow")
		  c=child("" "Windows.UI.Core.CoreWindow" w)
		 c=TO_GetWindowsStoreAppFrameChild(w)
		  outw c
		 if(!c) outw w; continue
		 if(!sub_sys.GetWindowsStoreAppId(c _s 1)) continue
		 h=GetFileIcon(_s)
	 else
		h=GetWindowIcon(w)
	 PN;PO
	ai[i]=h
	t.getwintext(w)
	t.findreplace(" :" " [91]58]") ;;escape :
	t.escape(1) ;;escape " etc
	s+F"{t} :act {w}; err * {ai[i]}[]"
 out s

i=DynamicMenu(s "" 1)
if(i) outw aw[i-1]

 s-" /isiz 32 32 /siz 500 50[]"
 int tb=DynamicToolbar(s)
 wait 0 -WC tb
