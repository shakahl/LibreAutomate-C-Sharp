 /
function# $name $windowlist

 Saves size and position of several windows.
 See also: <RestoreMultiWinPos>.

 name - registry value name (some unique string).
 windowlist - list of window names or +classnames.

 EXAMPLE
 SaveMultiWinPos "NQ" "Notepad[]+QM_Editor"
 ...
 RestoreMultiWinPos "NQ"


str s ss.all(4 2); WINDOWPLACEMENT* wp; int n sl h
ARRAY(int) aw

foreach s windowlist
	n+1; sl=ss.len
	ss.all(sl+sizeof(WINDOWPLACEMENT) 3); wp=+(ss+sl)
	if(s.beg("+")) h=win("" s+1 "" 0x8000 &sub.EnumProc &aw)
	else h=win(s "" "" 0x8000 &sub.EnumProc &aw)
	if(h) wp.Length=sizeof(WINDOWPLACEMENT); GetWindowPlacement h wp
	else wp.Length=0; end F"'{s}' not found" 8
if(!n) ret
int& nn=+ss; nn=n
ss+windowlist
rset ss name "\ArrangeWindows2" 0 REG_BINARY


#sub EnumProc
function# hwnd ARRAY(int)&a
int i
for(i 0 a.len) if(hwnd=a[i]) ret 1
a[]=hwnd
