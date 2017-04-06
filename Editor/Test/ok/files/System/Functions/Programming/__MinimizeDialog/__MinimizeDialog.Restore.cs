
 Restores window minimized by Minimize().
 Optional. Called implicitly when destroying variable.


if(!m_a.len) ret

int h=m_a[m_a.len-1]
if(IsIconic(h)) SetWindowState h 9 1
act m_a[0]

err+
m_a=0
