function hStr str&s

_i=DdeQueryStringW(m_idinst hStr 0 0 CP_WINUNICODE)
BSTR b.alloc(_i)
DdeQueryStringW(m_idinst hStr b _i+1 CP_WINUNICODE)
s.ansi(b)
