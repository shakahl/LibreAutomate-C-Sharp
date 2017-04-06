DATE d="4/1/2003"

SYSTEMTIME st
 add 2 days:
st.wDay=2
out d.add(&st)
 subtract 1 hour:
st.wDay=0; st.wHour=1
out d.sub(&st)
 get difference:
out d.diff("4.1.2003" &st 1)
out "%i/%i/%i %i:%i:%i" st.wMonth st.wDay st.wYear st.wHour st.wMinute st.wSecond
