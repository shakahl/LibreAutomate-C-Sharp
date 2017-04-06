 /
function# WINDOWPOS*wp

if(wp.flags&SWP_NOMOVE) ret

int i k
RECT r ro rs

 get old (current) and new splitter rect
GetWinRect(m_hWnd ro m_hParent) ;;current
rs.left=wp.x; rs.top=wp.y; SwapPOINT(+&rs) ;;new x y
int dist=rs.left-ro.left; if(!dist) ret
rs.right=rs.left+(ro.right-ro.left); rs.bottom=rs.top+(ro.bottom-ro.top) ;;new cx cy (may be not in wp)

 if first time, find adjacent controls
if(!m_attached) Attach()

 get control rects
ARRAY(RECT) a1 a2
GetControlRects(0 a1)
GetControlRects(1 a2)

 limit
if(dist<0) k=GetMinMax2(0 a1 ro)-rs.left; if(k<0) k=0
else k=GetMinMax2(1 a2 ro)-rs.right; if(k>0) k=0
if k
	dist+k
	if(dist) if(m_horz) wp.y+k; else wp.x+k
	else wp.flags|SWP_NOMOVE; ret

  move controls
int fl=SWP_NOZORDER|SWP_NOACTIVATE
for i 0 a1.len
	r=a1[i]
	r.right+dist
	SwapRECT(r)
	SetWindowPos m_a1[i] 0 0 0 r.right-r.left r.bottom-r.top fl|SWP_NOMOVE
for i 0 a2.len
	r=a2[i]
	r.left+dist
	SwapRECT(r)
	SetWindowPos m_a2[i] 0 r.left r.top r.right-r.left r.bottom-r.top fl
