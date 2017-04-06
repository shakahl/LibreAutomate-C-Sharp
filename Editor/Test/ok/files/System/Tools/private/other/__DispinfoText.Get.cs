function@* $s

if(!m_a.len) m_a.create(4)
BSTR& b=m_a[m_i]; m_i+1; m_i&3
b=s
ret b.pstr
