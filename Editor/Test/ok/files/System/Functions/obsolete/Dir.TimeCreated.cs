function'DateTime [FILETIME&ft] [SYSTEMTIME&st] [DATE&dt] [utc]

 Gets creation time in various formats.

 ft, st, dt - variables that receive time. Can be 0.
 utc - if nonzero, gets UTC times. Default: local times.


if(!fd) end ERR_INIT

FILETIME t
if(utc) t=fd.ftCreationTime; else FileTimeToLocalFileTime &fd.ftCreationTime &t
if(getopt(nargs))
	if(&ft) ft=t
	if(&st) FileTimeToSystemTime(&t &st)
	if(&dt) dt.fromfiletime(&t)
DateTime& R=+&t; ret R
