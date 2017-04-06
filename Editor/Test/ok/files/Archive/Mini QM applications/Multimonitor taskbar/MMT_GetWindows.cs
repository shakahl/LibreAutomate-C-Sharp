 /MMT_Main
function ARRAY(MMTWINMON)&aw

 Gets windows that should have taskbar button in monitor.


aw=0
int i h
ARRAY(int) a
win("" "" "" 0 0 0 a) ;;gets all visible window handles
for i 0 a.len
	h=a[i]
	 has taskbar button?
	if(!MMT_IsTaskbarWindow(h)) continue
	 monitor
	MMTWINMON& r=aw[]
	r.hwnd=h
	r.monitor=MonitorIndex(MonitorFromWindow(h 2))
