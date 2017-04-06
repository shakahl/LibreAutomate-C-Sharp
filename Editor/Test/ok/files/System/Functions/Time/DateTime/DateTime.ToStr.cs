function'str [flags] ;;flags: 1 only date, 2 only time, 4 with s, 8 with s and ms, 16 with s and ticks

 Creates short date/time string for current locale.


lpstr f
sel flags&3
	case 1 f="{D}"
	case 2 f=iif(flags&(4|8|16) "{TT}" "{T}")
	case else if(flags&(4|8|16)) f="{D} {TT}"

SYSTEMTIME st
if(!FileTimeToSystemTime(+&t &st)) end ERR_FAILED
str s.timeformat(f st); err end ERR_FAILED

str ms
if(flags&17=16) _i=t%10000000; ms.format(".%07i" _i)
else if(flags&9=8) ms.format(".%03i" st.wMilliseconds)

if ms.len ;;insert ms. Cannot simply append because may end with AM.
	strrev s
	int i=findcs(s "0123456789")
	strrev s
	if(i>0) s.insert(ms s.len-i); else s+ms

ret s
