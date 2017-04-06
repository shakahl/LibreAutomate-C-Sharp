function [days] [hours] [minutes] [seconds] [ms] [^micros]

 Adds or subtracts a time span specified in days, hours etc.
 To subtract, use all negative values.
 Not error if arguments are out of range. For example, if hours is 25, adds 1 day and 1 hour.


long i=TimeSpanFromParts(days hours minutes seconds ms micros)
i+t; if(i<0) end ERR_BADARG
t=i
