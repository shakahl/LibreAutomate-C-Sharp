rep
	GUITHREADINFO g.cbSize=sizeof(g)
	if(GetGUIThreadInfo(0 &g) and g.hwndCapture and wintest(g.hwndCapture "" "CLIPBRDWNDCLASS"))
		out "drag"
	0.5