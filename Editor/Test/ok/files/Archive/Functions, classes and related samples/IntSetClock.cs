 Sets correct computer time, retrieved from the Internet ("http://tycho.usno.navy.mil").
 Returns 1 on success, 0 on failure.
 Does not change year, because there is no year in the web page.
 Not completely reliable because will stop working when the web page is moved or modified. During last ~5 years it happened 1 time.


if(!IntPing("http://tycho.usno.navy.mil")) ret
 download file
Http h; str s
if(!h.GetUrl("http://tycho.usno.navy.mil/cgi-bin/timer.pl" &s)) ret
 get time string
ARRAY(str) a
if(findrx(s "(\w{3})\. (\d\d), (\d\d):(\d\d):(\d\d)" 0 2 a)<0) goto error
 get current time (year is missing in web file)
SYSTEMTIME st; GetSystemTime &st
 fill structure
lpstr m="JanFebMarAprMayJunJulAugSepOctNovDec"
int i=find(m a[1]); if(i<0) goto error
st.wMonth=i/3+1
st.wDay=val(a[2])
st.wHour=val(a[3])
st.wMinute=val(a[4])
st.wSecond=val(a[5])
st.wMilliseconds=999
 set system time
if(!SetPrivilege(SE_SYSTEMTIME_NAME)) goto error
if(!SetSystemTime(&st)) goto error
ret 1

 error
out "IntSetClock error"
