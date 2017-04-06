function hwnd idObject idChild
int hp=GetAncestor(hwnd 2)
if(hp=_hwndqm) ret
 out idObject
Acc a.ObjectFromEvent(hwnd idObject idChild); err out "no ao"; ret
str s=a.Name
s.getl(s 0)
out s
bee "C:\WINDOWS\Media\Windows XP Hardware Insert.wav"
