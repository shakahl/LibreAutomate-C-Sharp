#compile "IVirtualDesktopManager"

 IVirtualDesktopManager m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
 IVirtualDesktopManager_Raw m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
IVirtualDesktopManager_Raw m; if(!__GetVirtualDesktopManager(m)) ret

out;; 0.1
ARRAY(int) a
win "" "" "" 0 "" a
int i; str s ss
for i 0 a.len
	int w=a[i]
	outw2 w "" s; ss.addline(s+2)
	PF
	if(IsWindowCloaked(w)) ss.addline("<cloaked>")
	PN
	int is e
	e=m.IsWindowOnCurrentVirtualDesktop(w &is); if(e) is=-1
	ss.formata("%i[]" is)
	PN
	GUID g; e=m.GetWindowDesktopId(w g); if(e) ss+"[]"; else ss.addline(s.FromGUID(g))
	PN;PO 0 &s; ss.addline(s)
	 int wo=GetAncestor(w 3); if(wo!w) outw wo "								" s; ss.formata("<c 0x80C0>%s</c>" s)

out F"<>{ss}"

 note: for owned windows:
   IsWindowOnCurrentVirtualDesktop always gets TRUE, regardless of desktop.
   GetWindowDesktopId always gets {00000000-0000-0000-0000-000000000000}.
 tested: correct desktop for QM dialogs that are on other desktops.
