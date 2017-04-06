rep
	1
	GUITHREADINFO g.cbSize=sizeof(g)
	if(!GetGUIThreadInfo(0 &g)) continue
	outw g.hwndCapture
