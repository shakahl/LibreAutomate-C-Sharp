 /
function button [flags] ;;button: 1 X1, 2 X2.  flags: 1 down, 2 up

 Clicks mouse X1 or X2 button.

 REMARKS
 The auto delay (spe) is applied.


INPUT in.mi.mouseData=button&3
in.mi.dwExtraInfo=1354291109

if(flags&2=0)
	in.mi.dwFlags=MOUSEEVENTF_XDOWN
	SendInput 1 &in sizeof(INPUT)
	if(flags&1=0) 0.02
if(flags&1=0)
	in.mi.dwFlags=MOUSEEVENTF_XUP
	SendInput 1 &in sizeof(INPUT)

wait -2
