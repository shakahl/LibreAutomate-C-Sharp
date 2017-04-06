 /
function# year month day [int&pyear] [int&pmonth] [int&pday] [int&pdayofweek]

 Changes and/or gets computer date.
 Returns: 1 success, 0 failed.

 If year > 0, changes computer date.
 Last 4 optional parameters can be int variables that receive current date.

 EXAMPLE
 int y m d
 Date 0 0 0 y m d
 out "Year %i, month %i, day %i" y m d


SYSTEMTIME st
 Get current date:
GetLocalTime &st
if(st.wHour=23 and st.wMinute=59 and st.wSecond=59) wait 1.1; goto Get current date:
if(&pyear) pyear = st.wYear
if(&pmonth) pmonth = st.wMonth
if(&pday) pday = st.wDay
if(&pdayofweek) pdayofweek = st.wDayOfWeek
 Set new date:
if(year>0)
	if(!SetPrivilege(SE_SYSTEMTIME_NAME)) ret
	st.wYear = year
	st.wMonth = month
	st.wDay = day
	ret SetLocalTime(&st)
