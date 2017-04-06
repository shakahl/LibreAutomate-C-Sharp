function'DateTime [FILETIME&ft] [SYSTEMTIME&st] [DATE&dt] [utc]

 Gets last access time in various formats.

 ft, st, dt - variables that receive time. Can be 0.
 utc - if nonzero, gets UTC times. Default: local times.

 REMARKS
 File access time auto-updating on most computers is disabled.


if(!fd) end ERR_INIT

FILETIME t
if(utc) t=fd.ftLastAccessTime; else FileTimeToLocalFileTime &fd.ftLastAccessTime &t
if(getopt(nargs))
	if(&ft) ft=t
	if(&st) FileTimeToSystemTime(&t &st)
	if(&dt) dt.fromfiletime(&t)
DateTime& R=+&t; ret R
