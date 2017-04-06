out
int w1 x y cx cy; Acc a
w1=IsEditableTextControlFocused(x y cx cy a)
if(!w1) ret
out "%i %i %i %i" x y cx cy
outw w1
a.Role(_s); out _s
a.State(_s); out _s
out a.Name
out a.Value


#ret
Acc a.Focus
a.Role(_s); out _s
a.State(_s); out _s

#ret

out
int x1 y1
outw GetCaretXY(x1 y1)
out "%i %i" x1 y1
 ret


 int w1=child
 
 Acc a
 if(AccessibleObjectFromWindow(w1 OBJID_CARET IID_IAccessible &a.a)) ret
 int x y cx cy
 a.Location(x y cx cy)
 out "%i %i %i %i" x y cx cy
  a.State(_s); out _s
 
 
  a.Navigate("pa")
 
  a.Location(x y cx cy)
  out "%i %i %i %i" x y cx cy
  OnS