function'DateTime [FILETIME&ft] [SYSTEMTIME&st] [DATE&dt] [utc]

 Gets last write time in various formats.

 ft, st, dt - variables that receive time. Can be 0.
 utc - if nonzero, gets UTC times. Default: local times.

 REMARKS
 All 3 formats are used by Windows and QM:
   FILETIME - Windows stores file time in this format. It is the most precise format. QM uses this format in DateTime variables.
   SYSTEMTIME - contains members for date/time parts - year, day etc.
   DATE - often used with COM functions. Used with some QM functions. Less precise than other formats.
 In QM recommended to use DateTime variables. When precision is not important, can use DATE variables.
 You can find more info about these formats on the internet.
 For folders gets creation time.


if(!fd) end ERR_INIT

FILETIME t
if(utc) t=fd.ftLastWriteTime; else FileTimeToLocalFileTime &fd.ftLastWriteTime &t
if(getopt(nargs))
	if(&ft) ft=t
	if(&st) FileTimeToSystemTime(&t &st)
	if(&dt) dt.fromfiletime(&t)
DateTime& R=+&t; ret R
