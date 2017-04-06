 /
function# $taskName [DATE&nextRunTime] [DATE&lastRunTime] [str&statusString] [int&lastExitCode] [int&isRunning]

 Gets some information about a scheduled task.
 Returns 1 if successful (although some variables may be emty if fais to retrieve), or 0 if task does not exist or cannot be opened.

 nextRunTime - receives next run time or 0 if not scheduled.
 lastRunTime - receives last run time or 0.
 statusString - if the task could not be started, receives error description. If ok, empty.
 lastExitCode - receives program's exit code or 0.
 isRunning - receives 1 if the task is currently running, 0 if not.

 Use 0 for arguments you don't need.

 Reference: search for ITask in http://msdn.microsoft.com/library
 Interfaces, constants etc are defined in WINAPI.

 EXAMPLE
 DATE nextrun lastrun; str statusstr; int exitcode isrunning
 GetScheduledTaskStatus "QM - Macro508" nextrun lastrun statusstr exitcode isrunning

 
ITaskScheduler ts._create(CLSID_CTaskScheduler)
ITask task

BSTR bName=taskName
ts.Activate(bName uuidof(ITask) &task); err ret

SYSTEMTIME st
if(&lastRunTime)
	task.GetMostRecentRunTime(&st); err
	if(_hresult) lastRunTime=0; else lastRunTime.fromsystemtime(st)

if(&nextRunTime)
	task.GetNextRunTime(&st); err
	if(_hresult) nextRunTime=0; else nextRunTime.fromsystemtime(st)

if(&lastExitCode or &statusString)
	if(!&lastExitCode) &lastExitCode=_i
	lastExitCode=0
	task.GetExitCode(&lastExitCode); err
	if(&statusString)
		sel(_hresult)
			case [0,SCHED_S_TASK_HAS_NOT_RUN] statusString.all
			case else statusString.dllerror("" "" _hresult)

if(&isRunning)
	task.GetStatus(&isRunning)
	isRunning=isRunning=SCHED_S_TASK_RUNNING

ret 1
