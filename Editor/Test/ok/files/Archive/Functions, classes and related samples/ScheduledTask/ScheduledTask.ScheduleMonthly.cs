function $_time dayOfMonth [$months]

 Adds a "Monthly" schedule to this task.

 _time - start time. Examples: "20:00", "00:41".
 dayOfMonth - day of month.
 months - string like "1 3 5".
   1 is January, 2 February and so on.


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_MONTHLYDATE _time)

MONTHLYDATE& x=t.Type.MonthlyDate
x.rgfDays=1<<(dayOfMonth-1)
x.rgfMonths=_ParseMonths(months)

trigger.SetTrigger(&t)

err+ end _error
