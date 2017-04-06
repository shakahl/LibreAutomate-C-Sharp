function'SYSTEMTIME

 Returns date/time in SYSTEMTIME format.


SYSTEMTIME st
if(!FileTimeToSystemTime(+&t &st)) end ERR_FAILED
ret st
