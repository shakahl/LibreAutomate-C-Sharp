int w=win("Document1 - Microsoft Word" "OpusApp")
out
0.1
if(GetKeyState(VK_SCROLL)&1) out "new"; else out "old"
 if(GetKeyState(VK_NUMLOCK)&1) out "old2"; else out "new2"
 ARRAY(RECT) a
int i
for i 0 10
	RECT r
	 SetRect &r 1180
	 scan "color:0x40B8F0" w 0 0|2|16|0x1100 0
	if(!scan("image:h5958767F" w 0 16|0 0)) out "not found"; continue
	 out a.len
	0.01
	MapWindowPoints 0 w +&r 2;; outRECT r
	 if(i=1) OffsetRect &r 1 1

 old
 630 1300 2500 colorDiff
 470 1800

 new
 240 880
