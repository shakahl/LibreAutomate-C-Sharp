function $_time [$_date]

 Adds a "Once" schedule to this task.

 _time - time. Examples: "20:00", "00:41".
 _date - date. Example: "12/31/2013". Default: today.


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_ONCE _time)

if(!empty(_date)) _SetDate(t _date)

trigger.SetTrigger(&t)

err+ end _error
