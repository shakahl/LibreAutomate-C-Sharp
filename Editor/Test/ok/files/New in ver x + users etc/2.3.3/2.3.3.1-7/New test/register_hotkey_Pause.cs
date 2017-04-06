__RegisterHotKey k
Q &q
k.Register(0 1 0 VK_PAUSE)
Q &qq
k.Unregister
Q &qqq
outq

#ret
MSG m
rep
	if(!GetMessage(&m 0 0 0)) break
	sel m.message
		case WM_HOTKEY
		out 2