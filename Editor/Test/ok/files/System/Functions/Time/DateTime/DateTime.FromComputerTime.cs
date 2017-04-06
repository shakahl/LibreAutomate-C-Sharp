function [flags] ;;flags: 1 UTC

 Gets current computer time and initializes this variable.
 By default uses local time. Flag 1 - UTC time.


GetSystemTimeAsFileTime +&t
if(flags&1=0)
	long k
	FileTimeToLocalFileTime +&t +&k
	t=k
