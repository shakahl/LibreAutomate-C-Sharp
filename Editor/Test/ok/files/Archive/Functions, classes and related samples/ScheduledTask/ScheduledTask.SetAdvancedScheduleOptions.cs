function [$startDate] [$endDate] [repeatEveryMinutes] [repeatDurationMinutes] [flags] ;;flags: 1 this schedule is disabled, 2 stop task after repeatDurationMinutes

 Sets options for the last added schedule, as in the Advanced Schedule Options dialog.

 startDate - start date. Example: "12/31/2013". Does not change if ""; initially it is today.
 endDate - end date. Default or "": has no end date.
 repeatEveryMinutes - repeatedly run task every repeatEveryMinutes minutes. Default or 0: don't repeat.
 repeatDurationMinutes - repeatedly run task until this time expires. Use when repeatEveryMinutes not 0. Must be >=repeatEveryMinutes.

 EXAMPLE
 T.ScheduleOnceAfter("0:1") ;;add "once" schedule
 T.SetAdvancedScheduleOptions("" "" 1 100) ;;set options for the "once" schedule


ITaskTrigger trigger; TASK_TRIGGER t.cbTriggerSize=sizeof(t)
word n; m_task.GetTriggerCount(&n); if(!n) end "no schedules added"
m_task.GetTrigger(n-1 &trigger)
trigger.GetTrigger(&t)

if(!empty(startDate)) _SetDate(t startDate)
if(!empty(endDate)) _SetDate(t endDate 1); t.rgFlags|TASK_TRIGGER_FLAG_HAS_END_DATE; else t.rgFlags~TASK_TRIGGER_FLAG_HAS_END_DATE

t.MinutesInterval=repeatEveryMinutes
t.MinutesDuration=repeatDurationMinutes

if(flags&1) t.rgFlags|TASK_TRIGGER_FLAG_DISABLED; else t.rgFlags~TASK_TRIGGER_FLAG_DISABLED
if(flags&2) t.rgFlags|TASK_TRIGGER_FLAG_KILL_AT_DURATION_END; else t.rgFlags~TASK_TRIGGER_FLAG_KILL_AT_DURATION_END

trigger.SetTrigger(&t)

err+ end _error
