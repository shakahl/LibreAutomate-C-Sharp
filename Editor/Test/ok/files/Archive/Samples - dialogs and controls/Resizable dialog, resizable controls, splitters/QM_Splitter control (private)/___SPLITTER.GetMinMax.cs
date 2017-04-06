function# needMax

if(!m_attached) Attach()

RECT rs
GetWinRect(m_hWnd rs m_hParent)

ARRAY(RECT) a
GetControlRects(needMax a)

ret GetMinMax2(needMax a rs)
