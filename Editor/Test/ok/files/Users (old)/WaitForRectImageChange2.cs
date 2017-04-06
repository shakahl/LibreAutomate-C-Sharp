 /
function ^wt RECT&r

 This version is compatible with QM < 2.2.1.

 TODO: window, flags

 Waits until something changes in the specified rectangle on the screen.

 wt - max wait time, s. Error on timeout. Use 0 to wait forever.
 r - variable that contains screen coordinates of the rectangle.

 EXAMPLE
 RECT r
 r.left=100; r.top=100; r.right=r.left+100; r.bottom=r.top+100
 WaitForRectImageChange 0 r
 mes "changed"


GdiObject bm
if(!CaptureImageOnScreen(r.left r.top r.right-r.left r.bottom-r.top "" bm)) end ES_FAILED
int t1=GetTickCount
rep
	0.1
	if(!scan(bm 0 r 0)) break
	if(wt>0 and GetTickCount-t1>=wt*1000) end "wait timeout"
