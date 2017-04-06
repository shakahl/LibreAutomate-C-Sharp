function minutes [hours] [days]

SYSTEMTIME st
long ft
GetLocalTime &st
SystemTimeToFileTime &st +&ft
ft+10000000L*60*minutes
ft+10000000L*60*60*hours
ft+10000000L*60*60*24*days
FileTimeToSystemTime +&ft &st
if(!SetPrivilege(SE_SYSTEMTIME_NAME)) ret
SetLocalTime &st
