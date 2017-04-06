function months

 Adds months.
 To subtract, use negative months.
 Does not change day of month. However, if it would be too big for that month, sets nearest valid day of that month.


SYSTEMTIME st=ToSYSTEMTIME
int y=months/12; months%12
int m=st.wMonth+months
if(m<=0) m+12; y-1
else if(m>12) m-12; y+1
st.wMonth=m
st.wYear+y
if(st.wDay>28) _i=DaysInMonth(st.wYear st.wMonth)-st.wDay; if(_i<0) st.wDay+_i
FromSYSTEMTIME(st)
err+ end _error
