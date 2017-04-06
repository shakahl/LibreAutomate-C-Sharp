function $timeSpanStr

 Adds or subtracts a time span.
 Error if timeSpanStr is invalid.

 timeSpanStr - string in one of the following formats.
   d
   d h
   d h:m
   d h:m:s
   d h:m:s.f
   h:m
   h:m:s
   h:m:s.f
   m:s.f
   s.f
   
   Here d h m s - days, hours, minutes, seconds. .f - fraction of second (.001 is 1 ms, .1 is 100 ms, .0000001 is 0.1 mcs).
   There is no m:s format. Use m:s.0
   Whatever format is, the first part can be any value. Other parts must be valid values (eg s 0-59).
   The string can begin with -. Then subtracts.


long i=TimeSpanFromStr(timeSpanStr); err end _error
i+t; if(i<0) end ERR_BADARG
t=i
