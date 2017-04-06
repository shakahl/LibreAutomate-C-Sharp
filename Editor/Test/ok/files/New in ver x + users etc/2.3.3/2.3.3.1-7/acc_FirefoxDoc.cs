 /
function'IAccessible hwnd

IAccessible a
if(AccessibleObjectFromWindow(hwnd OBJID_CLIENT IID_IAccessible &a)) ret
 g1
ret a.Navigate(0x1009 1) ;;with some pages, error if varStart 0. Eg http://www.w3schools.com/tags/tryit.asp?filename=tryhtml_frame_cols. Only FF 3.6; OK on FF 4 and 2. All OK if 1.
err 0.1; goto g1
