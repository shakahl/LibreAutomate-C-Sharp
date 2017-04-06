 get new splitter rect
POINT ps
xm ps m_hWnd; SwapPOINT(ps)
int dist=ps.x-m_ptDrag.x; if(!dist) ret
RECT rs
GetWinRect(m_hWnd rs m_hParent)
OffsetRect &rs dist 0

 move splitter
SwapRECT(rs)
SetWindowPos m_hWnd 0 rs.left rs.top 0 0 SWP_NOZORDER|SWP_NOACTIVATE|SWP_NOSIZE
