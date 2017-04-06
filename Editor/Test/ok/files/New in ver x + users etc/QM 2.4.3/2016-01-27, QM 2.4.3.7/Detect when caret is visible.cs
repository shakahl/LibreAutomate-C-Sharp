rep
	1
	GUITHREADINFO g.cbSize=sizeof(GUITHREADINFO)
	if GetGUIThreadInfo(0 &g) and g.hwndCaret
		out "yes"
	else
		out "no"
