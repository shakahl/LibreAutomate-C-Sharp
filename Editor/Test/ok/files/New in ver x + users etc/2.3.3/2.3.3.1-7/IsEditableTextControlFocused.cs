 /
function# [&caretX] [&caretY] [&caretW] [&caretH] [Acc&aFocus]

 If caret is in an editable text field, returns handle of the control.
 Else returns 0.
 This function works not with all windows/controls.

 caretX ... - optional variables that receive caret location and focused accessible object.


int x y cx cy
int w=GetCaretXY(x y cx cy)
if(!w) ret

Acc a=acc
if(!a.a or a.State&STATE_SYSTEM_READONLY) ret

if(&caretX) caretX=x
if(&caretY) caretY=y
if(&caretW) caretW=cx
if(&caretH) caretH=cy
if(&aFocus) aFocus=a

ret w
