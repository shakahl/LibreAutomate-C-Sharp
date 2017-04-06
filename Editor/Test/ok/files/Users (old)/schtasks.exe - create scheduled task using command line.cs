/
function# $macro $SCschedule $STstarttime [$SDstartdate]

 HELP
 RunConsole "schtasks.exe" "/?" ;;run this for help
 RunConsole "schtasks.exe" "/create /?" ;;run this for /create help
 
 PROBLEMS
 Password must be specified in cl or at run time.
 'Run only if logged on' will be unchecked.


macro="Macro505"
SCschedule="ONCE"
STstarttime="20:19:00"
SDstartdate="09/13/2007"

str cl su
GetUserComputer su

 cl.format("/create /RU %s /RP %s /SC %s /TN QM - %s /TR qmcl.exe T MS '%s' /ST %s /SD %s" su sp SCschedule macro macro STstarttime SDstartdate)
cl.format("/create /RU %s /RP '''' /SC %s /TN ''QM - %s'' /TR ''qmcl.exe Q T MS %s'' /ST %s /SD %s" su SCschedule macro macro STstarttime SDstartdate)
out cl

RunConsole "schtasks.exe" cl

 I use RunConsole instead of run because I want to see help, errors, etc.
