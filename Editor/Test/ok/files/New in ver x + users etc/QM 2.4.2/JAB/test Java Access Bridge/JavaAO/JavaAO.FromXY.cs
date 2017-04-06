function! x y

 Gets Java accessible object at specified point in screen.
 Returns: 1 success, 0 failed. Also may throw error, but not error if there is no accessible Java window there.


opt noerrorshere 1
JavaAO_JabInit
Clear
int w=win(x y)
JavaAO aw a2
if(!JAB.GetAccessibleContextFromHWND(w &aw.vmID &aw.a) or !aw.a) ret

a2.a=JAB.GetAccessibleChildFromContext(aw.vmID aw.a 0)
if(!a2.a) ret
a2.vmID=aw.vmID

if JAB.GetAccessibleContextAt(a2.vmID a2.a x y &a)
	vmID=aw.vmID
	ret 1
