function hwnd idObject idChild

OnScreenDisplay F"Drag and drop detected: {_s.getwinexe(hwnd)}" -1 0 0 "" 0 0 8
rep
	1
	int tid=GetWindowThreadProcessId(hwnd 0); if(!tid) break
	GUITHREADINFO g.cbSize=sizeof(g)
	if(!GetGUIThreadInfo(tid &g)) break
	if(g.hwndCapture!hwnd) break
