 /
function# $name

 Restores size and position of several windows.

 name - registry value name.

 REMARKS
 The auto delay (spe) is applied.

 See also: <SaveMultiWinPos>.


str s ss; WINDOWPLACEMENT* wp; int n h
ARRAY(int) aw

if(!rget(ss name "\ArrangeWindows2" 0 "" REG_BINARY)) end "list of windows wasn't saved"
int& nn=+ss; wp=+(ss+4)
lpstr sw=+(wp+(nn*sizeof(WINDOWPLACEMENT))); if(sw-ss>=ss.len) end ERR_FAILED
int f(SWP_NOSIZE|SWP_NOMOVE) hh hdef=BeginDeferWindowPos(nn)
foreach s sw
	n+1; if(n>nn) break
	if(wp.Length)
		if(s.beg("+")) h=win("" s+1 "" 0x8000 &sub.EnumProc &aw)
		else h=win(s "" "" 0x8000 &sub.EnumProc &aw)
		if(h)
			SetWindowPlacement h wp
			hdef=DeferWindowPos(hdef h hh 0 0 0 0 f); if(!hdef) end ERR_FAILED
			hh=h; f|SWP_NOACTIVATE
	wp+sizeof(WINDOWPLACEMENT)
h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
act h
EndDeferWindowPos(hdef)
DestroyWindow(h)

wait -2


#sub EnumProc
function# hwnd ARRAY(int)&a
int i
for(i 0 a.len) if(hwnd=a[i]) ret 1
a[]=hwnd
