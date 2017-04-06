 /
function hwnd msg idevent ticks

KillTimer hwnd idevent

OSKVAR- v
int i=OSK_FindVK(idevent)
v.a[i].pressed=0

InvalidateRect hwnd 0 0
