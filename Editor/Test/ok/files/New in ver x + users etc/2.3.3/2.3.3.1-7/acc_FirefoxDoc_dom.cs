 /Macro1490
function'IAccessible hwnd

IAccessible a aa
if(AccessibleObjectFromWindow(hwnd OBJID_CLIENT IID_IAccessible &a)) ret
 g1
 ret a.Navigate(0x1009 1) ;;with some pages, error if varStart 0. Eg http://www.w3schools.com/tags/tryit.asp?filename=tryhtml_frame_cols. Only FF 3.6; OK on FF 4 and 2. All OK if 1.
 err 0.1; goto g1

IServiceProvider sp=+a
 out sp
IDispatch d
sp.QueryService(uuidof("{1814ceeb-49e2-407f-af99-fa755a7d2607}") uuidof("{1814ceeb-49e2-407f-af99-fa755a7d2607}") &d)
 out d
sp=+d
sp.QueryService(uuidof(IAccessible) uuidof(IAccessible) &aa)
 out aa
Acc k.a=aa
k.Role(_s); out _s
