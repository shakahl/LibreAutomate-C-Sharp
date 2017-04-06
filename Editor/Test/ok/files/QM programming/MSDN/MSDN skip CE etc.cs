function hwnd idObject idChild

int hwin=GetAncestor(hwnd 2)
int h=child(mouse)
if(h and GetDlgCtrlID(h)=2420) ret
h=child("" "GenericPane" hwin 0 50 300)
str s.getwintext(h)
if(s!"Index") ret

bee "C:\WINDOWS\Media\Windows XP Hardware Insert.wav"

 0.1
 2
 10 P 20; err ret
 Acc a=acc("" "LIST" hwin "SysListView32" "" 0x0 0 0 "" 10)
Acc a.ObjectFromEvent(hwnd idObject idChild)
int n=a.a.ChildCount; if(n<3) ret
int found(0) nfound(0)
for a.elem 1 n
	s=a.Description
	if(!s.beg("Location: ")) break
	s.get(s 10)
	sel s 2
		case "*Windows CE *" continue
	
	 out a.Description
	if(!found) found=a.elem
	nfound+1

if(found)
	a.elem=found
	 if(a.State&STATE_SYSTEM_SELECTED=0)
	Acc aa=acc("" "CLIENT" hwin "DockingView" "" 0x1001); err ret
	s=a.Name
	str ss=aa.Name
	if(s!ss)
		if(nfound=1) a.Mouse(4); mou
		else a.Select(3);; a.Mouse
err+
1
