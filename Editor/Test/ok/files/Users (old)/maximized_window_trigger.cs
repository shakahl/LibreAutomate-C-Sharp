int h=TriggerWindow
rep
	if(!IsWindow(h)) ret
	if(max(h))
		 wait until really maxed
		RECT r; GetWindowRect h &r
		if(r.right-r.left<ScreenWidth) 0.1; continue
		 restore at first
		res h
		 then minimize
		min h
		1
	0.5
