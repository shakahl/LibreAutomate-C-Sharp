#compile "IVirtualDesktopManager"
IVirtualDesktopManager_Raw m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
ARRAY(int) a; int i
out
outw GetDesktopWindow
opt hidden 1
win "" "" "" 0 "" a
for i 0 a.len
	int w=a[i]
	int wo1=GetAncestor(w 3)
	int wo2=GetWindowRootOwner(w)
	if(wo1!wo2)
		out "---"
		outw w
		outw wo1
		 outw wo2
		
		int is e
		e=m.IsWindowOnCurrentVirtualDesktop(w &is); if(e) is=-1
		out is
		GUID g; e=m.GetWindowDesktopId(w g)
		if(!e) out _s.FromGUID(g)
