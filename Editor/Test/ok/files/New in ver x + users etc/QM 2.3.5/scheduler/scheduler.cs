lock schedule
str+ g_schedule=
 Macro1950,	2013.03.08 17:47, 5m, mac, "Macro1950", "command line"
 write,		2013.03.08 17:48, 0, run, "C:\Windows\write.exe"
 notepad,	2013.03.08 17:49, 1h, run, "C:\Windows\notepad.exe"
int+ g_scheduleUpdate=1
lock- schedule

 List of scheduled tasks in CSV format: name, date, action, path, arguments, flags
   name - task name. Must be unique in the list.
   date - start date. Any supported format. If in the past, anyway will run if interval is set.
   interval - repeat interval. Format like 1m, 1h, 1d, 1w, 1M. Use "" or 0 to run once.
   action - run (run program) or mac (start macro).
   path - program path or macro name or path.
   arguments (optional) - command line arguments for the program or macro.
   flags (optional): 1 disabled
 After updating schedule, run this function. If already running, it will catch the new schedule.
 After updating code, end thread and run again. Or move the loop body to other function, then will not need to end/run everytime, just compile.

 _____________________________________

if(getopt(nthreads)>1) ret ;;if already running, let just update schedule

rep
	scheduler_Loop
