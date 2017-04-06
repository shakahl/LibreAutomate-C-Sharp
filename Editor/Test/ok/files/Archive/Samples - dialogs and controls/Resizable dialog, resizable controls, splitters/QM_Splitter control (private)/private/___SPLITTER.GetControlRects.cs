function need2 ARRAY(RECT)&a

int i
ARRAY(int)& ah=iif(need2 m_a2 m_a1)
a.create(ah.len)
for(i 0 a.len) GetWinRect(ah[i] a[i] m_hParent)
