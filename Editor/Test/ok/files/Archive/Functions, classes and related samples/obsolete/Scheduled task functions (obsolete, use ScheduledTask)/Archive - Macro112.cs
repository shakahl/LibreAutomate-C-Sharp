ScheduleMacro "Macro477" "10:00" ;;run once, today
 ScheduleMacro "Macro477" "15:30" "12/31/2008" ;;run once, at specified date
 ScheduleMacro "Macro477" "00:45" "" "" 2 ;;run once, today, delete when done
 ScheduleMacro "Macro477" "17:00" "" "" 0 1 ;;run every day
 ScheduleMacro "Macro477" "" "" "" 0 5 0 0 0 0 1 ;;run when idle for 1 minute
 WEEKLY w; w.WeeksInterval=1; w.rgfDaysOfTheWeek=TASK_MONDAY|TASK_FRIDAY
 ScheduleMacro "Macro477" "17:00" "" "" 0 2 0 w ;;run every week at monday and friday
