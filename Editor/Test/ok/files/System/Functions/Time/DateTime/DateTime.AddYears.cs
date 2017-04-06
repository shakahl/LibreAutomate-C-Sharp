function years

 Adds years.
 To subtract, use negative years.
 Does not change month and day of month. However, if it would be too big for February in that year, sets it to 28.


SYSTEMTIME st=ToSYSTEMTIME
st.wYear+years
if(st.wMonth=2 and st.wDay=29 and DaysInMonth(st.wYear 2)<29) st.wDay-1
FromSYSTEMTIME(st)
err+ end _error
