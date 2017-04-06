function action $windowlist

sel action
	case 6
	if(empty(windowlist)) end ERR_BADARG
	str s; int i h
	type ___ARRANGEWINDOWS @n @nn RECT'r[10] !state[10]
	___ARRANGEWINDOWS a
	for i 0 10
		if(s.getl(windowlist -i)<0) break
		h=iif(s.beg("+") win("" s+1) win(s))
		if(h=0) out "ArrangeWindows: '%s' not found" s; continue
		if(IsZoomed(h)) a.state[i]=3
		else if(IsIconic(h)) a.state[i]=2
		else
			a.state[i]=1
			GetWindowRect(h &a.r[i])
			a.n+1
	if(i) a.nn=i; rset a windowlist "\ArrangeWindows"

	case 7
	if(empty(windowlist)) end ERR_BADARG
	if(rget(a windowlist "\ArrangeWindows")=0) end "list of windows wasn't saved"
	int hh hfore=win
	int hdef=BeginDeferWindowPos(a.n)
	for i 0 a.nn
		if(s.getl(windowlist -i)<0) break
		if(a.state[i]=0) continue
		h=iif(s.beg("+") win("" s+1) win(s))
		if(h=0) out "ArrangeWindows: '%s' not found" s; if(a.state[i]!=1) continue
		sel a.state[i]
			case 1
			if(h=0)
				if(hh=0) hh=CreateWindowEx(0 +32770 0 0 0 0 0 0 0 0 _hinst 0)
				h=hh
			else if(IsZoomed(h) or IsIconic(h)) res h
			hdef=DeferWindowPos(hdef h 0 a.r[i].left a.r[i].top (a.r[i].right-a.r[i].left) (a.r[i].bottom-a.r[i].top) 0)
			if(hdef=0) bee; ret
			case 2 min h
			case 3 max h
	EndDeferWindowPos(hdef)
	SetWindowPos(hfore 0 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)
	if(hh) clo+ hh
