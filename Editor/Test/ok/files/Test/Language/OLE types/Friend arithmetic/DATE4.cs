DATE d="2003.4.7"

SYSTEMTIME st
 add 1 day:
st.wDay=1
out d.add(&st)
 subtract 1 hour:
st.wDay=0; st.wHour=1
out d.sub(&st)
 get difference:
out d.diff("2003.4.7" &st 1)
out "%i.%i.%i %i:%i:%i" st.wYear st.wMonth st.wDay st.wHour st.wMinute st.wSecond
