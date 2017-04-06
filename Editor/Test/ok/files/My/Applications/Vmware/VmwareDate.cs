 /
function $date pid [flags] ;;date: "" - restore.  flags: 1 restore from vmware_date_on

if(flags&1) goto gOff

int on=!empty(date)

if !on
	rget _i "VmwarePid"
	if(_i!=pid) ret

if(!IsUserAdmin)
	if(on) ShutDownProcess pid 1
	mes- "Must be admin." F"QM - {__FUNCTION__}" "x"

type DATEYMD y m d
DATEYMD d
str so

if(on)
	DATE x=date; SYSTEMTIME st; x.tosystemtime(st)
	Date st.wYear st.wMonth st.wDay d.y d.m d.d
	rset d "VmwareDate"
	rset pid "VmwarePid"
	so.format("date changed to %s" date)
else
	 gOff
	rget d "VmwareDate"
	Date d.y d.m d.d
	IntSetClock
	so="date restored"
	rset 0 "VmwarePid" "" 0 -1

out so
OnScreenDisplay so 0 0 0 0 0 0xff 5
