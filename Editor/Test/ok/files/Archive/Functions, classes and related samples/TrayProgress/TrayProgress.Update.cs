function percent [$customText]

 Updates tray icon.

 percent - a value 1 to 100.
 customText - if not empty, displays it in tray icon instead of percent.


RECT rb rp
rb.right=16; rb.bottom=16
rp.bottom=16; rp.top=10; rp.right=percent*16/100


FillRect m_mb.dc &rb m_hback
FillRect m_mb.dc &rp m_hprogress

if(empty(customText)) customText=_s.from(percent "%")
int oldfont=SelectObject(m_mb.dc m_font)
DrawTextW m_mb.dc @customText -1 &rb 0
SelectObject(m_mb.dc oldfont)

this.ReplaceIconHB(m_mb.bm)
