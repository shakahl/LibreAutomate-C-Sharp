function'DateTime

 Returns date part of this variable, where all time components are 0.


SYSTEMTIME st=ToSYSTEMTIME
memset &st.wHour 0 8
DateTime d.FromSYSTEMTIME(st)
ret d
err+ end _error
