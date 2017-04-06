 /
function hour minute second [ms] [int&phour] [int&pminute] [int&psecond] [int&pms]

 Changes and/or gets computer time.
 Returns: 1 success, 0 failed.

 If hour is >= 0, changes computer time.
 Last 4 optional parameters can be int variables that receive current time.

 REMARKS
 To get number of milliseconds since Windows started, use function GetTickCount instead.

 EXAMPLE
 int h m s
 Time -1 0 0 0 h m s
 out "Hour %i, minute %i, second %i" h m s


SYSTEMTIME st
 g1
 Get current time:
GetLocalTime &st
if(st.wHour=23 and st.wMinute=59 and st.wSecond=59) wait 1.1; goto g1
if(&phour) phour = st.wHour
if(&pminute) pminute = st.wMinute
if(&psecond) psecond = st.wSecond
if(&pms) pms = st.wMilliseconds
 Set new time:
if(hour>=0)
	if(!SetPrivilege(SE_SYSTEMTIME_NAME)) ret
	st.wHour = hour
	st.wMinute = minute
	st.wSecond = second
	st.wMilliseconds = ms
	ret SetLocalTime(&st)
