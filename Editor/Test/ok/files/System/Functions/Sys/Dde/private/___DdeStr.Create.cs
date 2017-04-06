function# $string idinst

m_idinst=idinst
if(hs) DdeFreeStringHandle(m_idinst hs)
hs=DdeCreateStringHandleW(m_idinst @string CP_WINUNICODE)
ret hs
