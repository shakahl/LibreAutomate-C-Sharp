function $_time whichWeek [dayOfWeek] [$months] ;;whichWeek: 1 first, 2 second, 3 third, 4 fourth, 5 last

 Adds a "Monthly" schedule to this task.

 _time - start time. Examples: "20:00", "00:41".
 whichWeek - week of month. See above.
 dayOfWeek:
   0,7 Sunday, 1-6 Monday-Saturday.
   Default: 1 (Monday).
 months - string like "1 3 5".
   1 is January, 2 February, and so on.


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_MONTHLYDOW _time)

MONTHLYDOW& x=t.Type.MonthlyDOW
x.wWhichWeek=whichWeek
if(getopt(nargs)<3) dayOfWeek=1; else if(dayOfWeek=7) dayOfWeek=0
x.rgfDaysOfTheWeek=1<<dayOfWeek
x.rgfMonths=_ParseMonths(months)

trigger.SetTrigger(&t)

err+ end _error
