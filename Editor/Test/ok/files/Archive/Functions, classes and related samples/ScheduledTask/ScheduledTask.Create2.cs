function $name $program [$params] [flags] [$runAs] ;;flags: 2 delete task if not scheduled to run again, 4 disabled, 0x200 hidden

 Creates scheduled task to run a program.
 If the task already exists, at first deletes it.
 Does not set schedule and does not save the task. Use ScheduleDaily etc and Save later.
 Error if fails.

 name - task name.
 program - full path of the program.
 params - command line arguments.
 runAs - as in task properties Task tab. Default: current user.

 REMARKS
 If QM is running as administrator (UAC) when creating task, the program will run as administrator, else as standard user.


m_task=0
ITaskScheduler ts._create(CLSID_CTaskScheduler)

 create new task
 g1
int retry
ts.NewWorkItem(@name CLSID_CTask IID_ITask &m_task)
err
	if(retry or _hresult&0xff!=ERROR_FILE_EXISTS) end _error
	retry=1; ts.Delete(@name); goto g1

 set properties
m_task.SetApplicationName(@_s.expandpath(program))
if(!empty(params)) m_task.SetParameters(@params)

if(empty(runAs)) GetUserInfo &_s; runAs=_s
m_task.SetAccountInformation(@runAs 0); m_task.SetCreator(@runAs)

m_task.SetFlags(flags&0x206|TASK_FLAG_RUN_ONLY_IF_LOGGED_ON)

err+ end _error
