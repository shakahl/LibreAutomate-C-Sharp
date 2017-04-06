str s
 g1
inp- s "Find bookmark in visible bookmarks" "" s
int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)
act w
Acc ap.Find(w "OUTLINE" "" "class=MozillaWindowClass[]a:id=bookmarks-view" 0x1004)
ARRAY(Acc) a
ap.GetChildObjects(a 0 "OUTLINEITEM" "" "" 16)
int i
for i 0 a.len
	if(find(a[i].Name s 0 1)<0) continue
	err
	 out a[i].Name
	 a[i].WebScrollTo
	a[i].Select(3)
	key DU
	mes- "Find next?" "" "OC"
 OnScreenDisplay "no more"
goto g1
