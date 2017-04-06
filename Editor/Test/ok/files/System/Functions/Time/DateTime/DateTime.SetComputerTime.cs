function [flags] ;;flags: 1 UTC

 Sets computer time.
 By default uses local time. Flag 1 - UTC time.
 Error if time is invalid or QM is not running as administrator.


SYSTEMTIME st=ToSYSTEMTIME; err end _error
int r
if flags&1
	r=SetSystemTime(&st)
else
	r=SetLocalTime(&st) and SetLocalTime(&st)
if(!r) end "" 16
