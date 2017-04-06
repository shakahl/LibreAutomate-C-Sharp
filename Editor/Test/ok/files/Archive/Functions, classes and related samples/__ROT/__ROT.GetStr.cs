function~ i

word* w
m_a[i].GetDisplayName(m_bind 0 &w); err ret
str s.ansi(w)
CoTaskMemFree(w)
ret s
