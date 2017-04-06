function $_time [$daysOfWeek] [weeksInterval]

 Adds a "Weekly" schedule to this task.

 _time - start time. Examples: "20:00", "00:41".
 daysOfWeek - days of week, like "1 3 5".
   0,7 Sunday, 1-6 Monday-Saturday.
   Default: "1" (Monday).
 weeksInterval - run every weeksInterval week. Default or 0: 1 (every week).


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_WEEKLY _time)

WEEKLY& x=t.Type.Weekly
x.rgfDaysOfTheWeek=_ParseDays(daysOfWeek)
x.WeeksInterval=iif(weeksInterval weeksInterval 1)

trigger.SetTrigger(&t)

err+ end _error
