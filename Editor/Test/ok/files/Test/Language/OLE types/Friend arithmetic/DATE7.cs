DATE d.getclock
DATE dd=d
SYSTEMTIME st.wDay=1
dd.add(st)
st.wDay=0
out d.diff(dd &st)
out st.wDay
 out d
