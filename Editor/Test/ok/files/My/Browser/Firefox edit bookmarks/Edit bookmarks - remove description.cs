int w=win("Library" "Mozilla*WindowClass" "" 0x804)
act w
Acc a1.Find(w "OUTLINE" "" "class=MozillaWindowClass" 0x1084)
Acc a2.Find(w "TEXT" "Description:" "class=MozillaWindowClass[]state=0x0 0x20000040" 0x1085)
a1.Select(3)
rep 100
	_s=a2.Value
	if _s.len
		a2.Select(3)
		key CaX
		a1.Select(3)
	key D
