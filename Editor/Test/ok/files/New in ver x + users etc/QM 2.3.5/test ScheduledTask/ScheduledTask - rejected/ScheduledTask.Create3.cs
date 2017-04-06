function $name $program [$params] [flags] [idleMinutes] [idleWatchMinutes] [$runAs] [$comments] ;;flags: 2 delete task if not scheduled to run again, 4 disabled, 0x10 start only if idle, 0x20 end task if computer ceases to be idle, 0x40 don't start if on batteries, 0x80 end task if battery mode begins, 0x200 hidden, 0x800 start again when idle state resumes, 0x1000 wake computer

 Creates scheduled task to run a program.
 If the task already exists, at first deletes it.
 Does not set schedule and does not save the task. Use ScheduleDaily etc and Save later.
 Error if fails.

 name - task name.
 program - full path of the program.
 params - command line arguments.
 flags - see above.
 idleMinutes, idleWatchMinutes - as in task properties Settings tab.
 runAs, comments - as in task properties Task tab.
   Default runAs is current user.
   The task will run only if the user is logged on, therefore don't need a password.


Delete2(name)

ITaskScheduler ts._create(CLSID_CTaskScheduler)
m_task=0
ts.NewWorkItem(@name CLSID_CTask IID_ITask &m_task)

m_task.SetApplicationName(@_s.expandpath(program))
if(!empty(params)) m_task.SetParameters(@params)
if(!empty(comments)) m_task.SetComment(@comments)

if(empty(runAs)) GetUserInfo &_s; runAs=_s
m_task.SetAccountInformation(@runAs 0); m_task.SetCreator(@runAs)

if(idleMinutes) m_task.SetIdleWait(idleMinutes iif(idleWatchMinutes idleWatchMinutes 60)); flags|0x10
m_task.SetFlags(flags~1|TASK_FLAG_RUN_ONLY_IF_LOGGED_ON)

err+ end _error
