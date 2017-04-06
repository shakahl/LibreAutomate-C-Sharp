out

str s
s.timeformat ;;same as s.timeformat("{D} {T}")
s.timeformat("{D} {TT}")
s.timeformat("Current date is {DD}")
s.timeformat("Current date is {MMM dd yyyy}, {HH 'hours and' mm} minutes")
s.timeformat("" 0 WINAPI.LANG_ENGLISH|WINAPI.SUBLANG_ENGLISH_US)

DATE d.getclock ;;get current time
d=d+1 ;;add 1 day
d=d+(60*MINUTE) ;;add 1 hour
s.timeformat("{DD} {TT}" d)

SYSTEMTIME st
GetLocalTime &st
s.format("%02i:%02i:%02i.%03i" st.wHour st.wMinute st.wSecond st.wMilliseconds)

out s
