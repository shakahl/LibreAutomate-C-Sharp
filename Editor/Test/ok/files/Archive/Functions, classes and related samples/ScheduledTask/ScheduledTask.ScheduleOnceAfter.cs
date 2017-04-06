function $timeFromNow

 Adds a "Once" schedule to this task.

 timeFromNow - time relative to current time. Can include days, hours and minutes.
   Examples: "0:10" (after 10 minutes) "1:00" (after 1 hour), "2" (after 2 days), "1 12" (after 1 day and 12 hours), "1 2:30" (after 1 day, 2 hours and 30 minutes).

 REMARKS
 This function gets current time, adds timeFromNow, and creates schedule like ScheduleOnce.
 Scheduler precision is 1 minute.
 For small times instead use <help>tim</help>.


TASK_TRIGGER t; ITaskTrigger trigger=_Schedule(t TASK_TIME_TRIGGER_ONCE 0)

DateTime d.FromComputerTime
d.AddStr(timeFromNow)
int Y M D H m
d.GetParts(Y M D H m)
t.wBeginYear=Y
t.wBeginMonth=M
t.wBeginDay=D
t.wStartHour=H
t.wStartMinute=m

trigger.SetTrigger(&t)

err+ end _error
