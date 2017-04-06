#compile "IVirtualDesktopManager"

 IVirtualDesktopManager m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
IVirtualDesktopManager_Raw m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")

int w
 w=_hwndqm
w=win("Notepad")

PF
GUID g
g=m.GetWindowDesktopId(_hwndqm)
 outb &g sizeof(GUID)
PN
g=m.GetWindowDesktopId(w)
PN

int i1=m.IsWindowOnCurrentVirtualDesktop(_hwndqm)
PN
int i2=m.IsWindowOnCurrentVirtualDesktop(w)
PN
PO
out "%i %i" i1 i2

m.MoveWindowToDesktop(_hwndqm g)


 speed:
   When a function called first time in thread - 1500 us. Later ~80 us. For all-desktops windows ~170 us.
   When not using Run button (eg use Ctrl+R or add 0.1 s delay): 4500, 200 and 450 us.
 info: for windows that are on all desktops, error 0x8002802B, Element not found.
 note: passing 0 as window handle to functions of this interface crashes explorer. If invalid handle (closed window) - error 0x8002802B, Element not found.
