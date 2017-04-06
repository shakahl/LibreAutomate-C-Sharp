function [detach]

 Finds adjacent controls.
 Stores left/top controls in m_a1; right/bottom in m_a2.

m_a1=0; m_a2=0; m_attached=0
if(detach) m_attached=1; ret ;;disable auto-attach

int i j h
RECT r rs
ARRAY(int) a

GetWinRect(m_hWnd rs)
child "" "" m_hParent 16 0 0 a

for i 0 a.len
	h=a[i]; if(h=m_hWnd) continue
	GetWinRect(h r)
	j=r.top+r.bottom/2; if(j<rs.top or j>rs.bottom) continue ;;matches vertically?
	if(r.left+r.right/2 <= rs.left) if(r.right>=rs.left-4 and r.right<=rs.right) m_a1[]=h ;;at left?
	else if(r.left<=rs.right+4 and r.left>=rs.left) m_a2[]=h ;;at right?

 out "a1"; for(i 0 m_a1.len) out GetWinId(m_a1[i])
 out "a2"; for(i 0 m_a2.len) out GetWinId(m_a2[i])

m_attached=m_a1.len+m_a2.len != 0
