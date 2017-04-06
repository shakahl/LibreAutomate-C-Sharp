 Creates Windows Task Scheduler task to run a macro or a program.
 You can add one or more schedules of any type to the task.
 Also you can Delete a task.
 Creates/deletes tasks configured for Windows XP/2000/2003, in the same way as QM when you manually create/delete a task using the Properties dialog in QM.
 If after creating a task the schedule is not displayed in QM until it restarts, install QM 2.3.5.3 or later.

 EXAMPLE

#compile "__ScheduledTask"
ScheduledTask x
x.Create("Macro1998")
x.ScheduleOnce("9:00")
x.Save
