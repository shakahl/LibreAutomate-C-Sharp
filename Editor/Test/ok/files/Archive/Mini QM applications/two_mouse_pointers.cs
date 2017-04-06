 /Keyboard_Detector

 Creates two virtual pointers to be used by two mouses.
 If there are two monitors, each pointer will be in its monitor.

 Requires Keyboard_Detector. If you don't have it: If this function is from Archive.qml, you'll find Keyboard_Detector there too. Else download from http://www.quickmacros.com/forum/viewtopic.php?f=2&t=776
 In Keyboard_Detector dialog must be enabled multiple mouses, and filter functions assigned to mouses. To open the dialog, run Keyboard_Detector or this function 2 times.
 Also need to edit function RI_Input. Before line " case RIM_TYPEHID" insert line "two_mouse_pointers" (tab-indented).

 You can change this variable:
   0 all cursors can be anywhere;
   1 each cursor must be in its monitor;
   2 cursors must be in left or right side of primary monitor. This is for testing on single monitor.
 After changing, click Compile button. Or assign a global variable, and then can change from other macro.
int clipCursor=1
 int+ g_clipCursor; clipCursor=g_clipCursor ;;with global variable

 _______________________________________

#compile Keyboard_Detector
type TWOMPDATA mouse_id POINT'p clipCursor RECT'r
TWOMPDATA-- t

POINT p; xm p

if g_ri.mouse_id=t.mouse_id  ;;same mouse
	if(clipCursor!=t.clipCursor) ClipCursor 0; goto g1
	if clipCursor
		if t.r.right>t.r.left and !PtInRect(&t.r p.x p.y)
			if(p.x<t.r.left) p.x=t.r.left
			if(p.x>=t.r.right) p.x=t.r.right-1
			if(p.y<t.r.top) p.y=t.r.top
			if(p.y>=t.r.bottom) p.y=t.r.bottom-1
			ClipCursor &t.r
			SetCursorPos p.x p.y; GetCursorPos &p
	ret

ClipCursor 0

if t.mouse_id
	SetCursorPos t.p.x t.p.y
	OnScreenDisplay "\" -1 p.x-3 p.y-6 "Courier New" 0 0xff 16 "two_mouse_pointers"
	atend OsdHide ""
	t.p=p
t.mouse_id=g_ri.mouse_id

 g1
t.clipCursor=clipCursor
SetRectEmpty &t.r
if clipCursor
	for _i 0 4
		if g_ri.mouse_id=g_rir.m[_i]
			if(t.clipCursor=1) MonitorFromIndex(_i+1 0 &t.r)
			else t.r.right=ScreenWidth; t.r.bottom=ScreenHeight; if(_i=0) t.r.right/2; else t.r.left=t.r.right/2
			ClipCursor &t.r
			atend ClipCursor 0
			break

 notes:
 ClipCursor is not necessary, but prevents cursor shaking.
 Cannot use only ClipCursor because it works temporarily.
