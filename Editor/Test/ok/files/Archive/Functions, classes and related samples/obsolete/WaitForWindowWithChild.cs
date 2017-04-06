 /
function# ^waitmax $childtext $childclass [childflags] [$wintext] [$winclass] [$exename] [winflags] [&hwndchild] ;;waitmax: 0 is infinite.

 Waits for window that contains specified control.
 Returns window handle.
 QM 2.3.4. You can instead use wait() with win(). Now win() can find window containing specified control.

 waitmax - max number of seconds to wait. 0 is infinite.
 childtext, childclass, childflags - same as with function <help "::/functions/IDP_CHILD.html">child</help>.
 other arguments - same as with function <help "::/functions/IDP_WIN.html">win</help>.

 See also: <WinC>

 EXAMPLE
 int h=WaitForWindowWithChild(120 "Are you sure you want to sign this report?" "Static" 0 "SampleApp" "#32770")
 out _s.getwintext(h)


if(waitmax<0 or waitmax>2000000) end ES_BADARG
opt waitmsg -1

int wt(waitmax*1000) t1(GetTickCount)
rep
	0.01
	
	int h=WinC(childtext childclass childflags wintext winclass exename winflags hwndchild)
	if(h) ret h
	
	if(wt>0 and GetTickCount-t1>=wt) end "wait timeout"
