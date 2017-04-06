DATE d;;.tt
out d.getclock

SYSTEMTIME st
d.tosystemtime(&st)
out "%i.%i.%i %i:%i:%i" st.wYear st.wMonth st.wDay st.wHour st.wMinute st.wSecond

st.wDay+=2
out d.fromsystemtime(&st)

FILETIME f
d.tofiletime(&f)

long& l=+&f; l+=10000000
out d.fromfiletime(&f)
