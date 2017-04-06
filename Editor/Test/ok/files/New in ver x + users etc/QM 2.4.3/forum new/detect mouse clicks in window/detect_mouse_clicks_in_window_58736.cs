int hwnd=TriggerWindow

rep
	wait 0 ML
	if(!IsWindow(hwnd)) ret
	if(win!hwnd) continue
	Acc a.FromMouse; err continue
	str name=a.Name; err continue
	out name
	