 /
function $taskName [$_time] [$_date] [$_enddate]

 Changes the start time or/and begin date or/and end date of a scheduled task.
 Error if task does not exist or time/date format is invalid.
 Note: Names of scheduled tasks that are created by QM consist of "QM - " and macro name.

 Reference: search for ITask in http://msdn.microsoft.com/library
 Interfaces, constants etc are defined in WINAPI.

 EXAMPLES
 ChangeScheduledTaskTime "QM - Macro508" "17:00" "08/03/07"
 ChangeScheduledTaskTime "QM - Macro508" "17:00"
 ChangeScheduledTaskTime "QM - Macro508" "" "08/03/07"


ITaskScheduler ts._create(CLSID_CTaskScheduler)

ITask task
BSTR bName=taskName
ts.Activate(bName uuidof(ITask) &task)

ITaskTrigger trigger
task.GetTrigger(0 &trigger)

TASK_TRIGGER t
trigger.GetTrigger(&t)

DATE d
SYSTEMTIME st

if(len(_time))
	d=_time; err goto ge1
	d.tosystemtime(st)
	t.wStartHour=st.wHour
	t.wStartMinute=st.wMinute

if(len(_date))
	d=_date; err goto ge1
	d.tosystemtime(st)
	t.wBeginYear=st.wYear
	t.wBeginMonth=st.wMonth
	t.wBeginDay=st.wDay

if(len(_enddate))
	d=_enddate; err goto ge1
	d.tosystemtime(st)
	t.wEndYear=st.wYear
	t.wEndMonth=st.wMonth
	t.wEndDay=st.wDay
	t.rgFlags|TASK_TRIGGER_FLAG_HAS_END_DATE

trigger.SetTrigger(&t)

 save
IPersistFile iFile=+task
iFile.Save(0 1)
ret

err+ end _error
 ge1
end ES_BADARG
