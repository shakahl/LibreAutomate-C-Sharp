function $_time [daysInterval]

 Adds a "Daily" schedule to this task.

 _time - start time. Examples: "20:00", "00:41".
 daysInterval - run every daysInterval days. Default or 0: 1 (every day).


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_DAILY _time)

t.Type.Daily.DaysInterval=iif(daysInterval>0 daysInterval 1)

trigger.SetTrigger(&t)

err+ end _error
