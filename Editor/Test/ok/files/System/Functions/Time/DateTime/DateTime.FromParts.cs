function year month day [hour] [minute] [second] [ms] [^micros]

 Initializes this variable from date/time parts.
 Error if some parts are out of range.


SYSTEMTIME s
s.wYear=year
s.wMonth=month
s.wDay=day
s.wHour=hour
s.wMinute=minute
s.wSecond=second
s.wMilliseconds=ms
long k
if(!SystemTimeToFileTime(&s +&k) or micros<0 or micros>=1000) end ERR_BADARG
_i=micros*10; t=k+_i
