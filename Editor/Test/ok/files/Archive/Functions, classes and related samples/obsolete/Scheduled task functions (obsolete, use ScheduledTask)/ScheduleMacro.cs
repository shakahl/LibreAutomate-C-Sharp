 /
function $macro $_time [$_date] [$_enddate] [flags] [scheduletype] [daysinterval] [WEEKLY&weekly] [MONTHLYDATE&monthlydate] [MONTHLYDOW&monthlydow] [idleminutes] [idlewatchminutes] ;;scheduletype: 0 once, 1 daily, 2 weekly, 3 monthlydate, 4 monthlydow, 5 idle, 7 logon.  flags: 2 delete task if not scheduled to run again, 4 disabled, 0x10 start only if idle, 0x20 end macro if computer ceases to be idle, 0x40 don't start if on batteries, 0x80 end macro if battery mode begins, 0x200 hidden, 0x800 start again when idle state resumes, 0x1000 wake computer

 Obsolete, use ScheduledTask, it's in Archive too.

 Creates scheduled task for a macro.
 If the task already exists, at first deletes it.
 Throws error if the task cannot be created or changed.
 Note: Schedule of the task is not displayed in QM until QM is restarted or file reloaded or task properties dialog is opened through macro Properties dialog.
 Requires QM 2.2.0 or later.

 macro - QM item name or +id. It should be function, but also can be macro or item of other type.
 _time - start time. Examples: "20:00", "00:41".
 _date - start date. Example: "12/31/2007". Default or "": today. Note: date string format depends on your computer locale settings.
 _enddate - end date. Default or "": has no end date.
 flags, scheduletype - see above and below.
 daysinterval - use if scheduletype is 1 (daily). Default or 0: every day.
 weekly - variable that defines weekly schedule. Required if scheduletype is 2 (weekly). Otherwise should be 0.
 monthlydate - variable that defines monthly schedule. Required if scheduletype is 3 (monthlydate). Otherwise should be 0.
 monthlydow - variable that defines monthly schedule. Required if scheduletype is 4 (monthlydow). Otherwise should be 0.
 idleminutes, idlewatchminutes - values specified in task properties Settings tab. Used if scheduletype is 5 or flags includes 0x10.

 WEEKLY, MONTHLYDATE and MONTHLYDOW types, as well as other scheduler types, interfaces, constants, etc are documented in the MSDN library on the internet.

 EXAMPLES
 ScheduleMacro "Macro477" "09:00" ;;run once, today
 ScheduleMacro "Macro477" "15:30" "12/31/2007" ;;run once, at specified date
 ScheduleMacro "Macro477" "00:45" "" "" 2 ;;run once, today, delete when done
 ScheduleMacro "Macro477" "17:00" "" "" 0 1 ;;run every day
 ScheduleMacro "Macro477" "" "" "" 0 5 0 0 0 0 1 ;;run when idle for 1 minute
 WEEKLY w; w.WeeksInterval=1; w.rgfDaysOfTheWeek=TASK_MONDAY|TASK_FRIDAY
 ScheduleMacro "Macro477" "17:00" "" "" 0 2 0 w ;;run every week at monday and friday


str s ss; BSTR b; int retry
ITaskScheduler ts._create(CLSID_CTaskScheduler)
ITask task

 create new task
if(macro<0x10000) _i=macro; macro=s.getmacro(_i 1)
b=s.from("QM - " macro)
 g1
ts.NewWorkItem(b CLSID_CTask IID_ITask &task)
err
	if(retry or _hresult&0xff!=ERROR_FILE_EXISTS) end _error
	retry=1; ts.Delete(b); goto g1

 set properties
b=s.expandpath("$qm$\qmcl.exe"); task.SetApplicationName(b)
b=s.format("T MS ''%s''" macro); task.SetParameters(b)

flags|TASK_FLAG_RUN_ONLY_IF_LOGGED_ON
task.SetFlags(flags)

GetUserComputer s ss; b=s.from(ss "\" s)
task.SetAccountInformation(b 0); task.SetCreator(b)

if(idleminutes) task.SetIdleWait(idleminutes iif(idlewatchminutes idlewatchminutes 60))

 set trigger (schedule)
ITaskTrigger trigger; TASK_TRIGGER t
task.CreateTrigger(+&_i &trigger)
trigger.GetTrigger(&t)
t.rgFlags=0
t.TriggerType=scheduletype
sel scheduletype
	case TASK_TIME_TRIGGER_DAILY t.Type.Daily.DaysInterval=iif(daysinterval>0 daysinterval 1)
	case TASK_TIME_TRIGGER_WEEKLY t.Type.Weekly=weekly
	case TASK_TIME_TRIGGER_MONTHLYDATE t.Type.MonthlyDate=monthlydate
	case TASK_TIME_TRIGGER_MONTHLYDOW t.Type.MonthlyDOW=monthlydow

DATE d; SYSTEMTIME st
if(!empty(_time))
	d=_time; err end "invalid time string"
	d.tosystemtime(st)
	t.wStartHour=st.wHour
	t.wStartMinute=st.wMinute
if(!empty(_date))
	d=_date; err end "invalid date string"
	d.tosystemtime(st)
	t.wBeginYear=st.wYear
	t.wBeginMonth=st.wMonth
	t.wBeginDay=st.wDay
if(!empty(_enddate))
	d=_enddate; err end "invalid date string"
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
