int hwnd=val(_command)
Acc a.Find(hwnd "MENUPOPUP" "" "" 0x1001 1); err ret
if(a.ChildCount!2) ret
a.elem=1
str s=a.Name
if(!matchw(s "*.cpp:*{...}*" 1)) ret
a.DoDefaultAction

err+
