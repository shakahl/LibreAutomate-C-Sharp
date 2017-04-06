 /
function $date ;;date: "" - restore

if(!IsUserAdmin) mes- "Must be admin" "" "x"

type DATEYMD y m d
DATEYMD d
str so

if(!empty(date))
	DATE x=date; SYSTEMTIME st; x.tosystemtime(st)
	Date st.wYear st.wMonth st.wDay d.y d.m d.d
	rset d "Win7RcDate"
	so.format("date changed to %s" date)
else
	if(!rget(d "Win7RcDate")) ret
	Date d.y d.m d.d
	IntSetClock
	so="date restored"
	rset 0 "Win7RcDate" "" 0 -1

out so
OnScreenDisplay so 0 0 0 0 0 0xff 7
