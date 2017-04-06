 /
function hwndTB value [flags] ;;flags: 1 don't notify parent

 Sets trackbar control thumb position.
 value should be between min and max values of the trackbar control (use GetTrackBarInfo to get)

 EXAMPLE
 int hwndTB=id(1001 win("" "Volume Control"))
 int rmin rmax
 GetTrackbarInfo hwndTB rmin rmax ;;get range
 SetTrackbarPos hwndTB rmin+rmax/2 ;;set middle value
 bee
 1
 SetTrackbarPos hwndTB rmin ;;set min value (max sound)
 bee


SendMessage hwndTB TBM_SETPOS 1 value

if(flags&1=0)
	int hwnd=GetParent(hwndTB)
	int msg=iif(GetWinStyle(hwndTB)&TBS_VERT WM_VSCROLL WM_HSCROLL)
	value<<16
	SendMessage hwnd msg TB_THUMBTRACK|value hwndTB
	SendMessage hwnd msg TB_THUMBPOSITION|value hwndTB
	SendMessage hwnd msg TB_ENDTRACK hwndTB
