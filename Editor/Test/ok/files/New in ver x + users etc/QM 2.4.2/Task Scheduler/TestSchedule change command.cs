out
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
 TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
TaskScheduler.ITaskFolder folder=ts.GetFolder("\Quick Macros")
TaskScheduler.IRegisteredTask task
VARIANT varEmpty
foreach task folder.GetTasks(TASK_ENUM_HIDDEN)
	str name=task.Name
	 if(!name.beg("QM - ")) continue
	TaskScheduler.ITaskDefinition td=task.Definition
	TaskScheduler.IExecAction ea=+td.Actions.Item(1)
	ea.Path="Q:\test\TestSchedule.exe"
	 out ea.Arguments
	ea.Arguments=F"M ''TestSchedule'' ''{name}''"
	
	TaskScheduler.ITimeTrigger trig=+td.Triggers.Item(1)
	DateTime dt.FromComputerTime()
	 dt.AddParts(0 0 0 10) ;;+10 s
	dt.AddParts(0 0 1) ;;+1 m
	trig.StartBoundary=_s.timeformat("{yyyy-MM-dd}T{HH:mm:ss}" dt)
	
	folder.RegisterTaskDefinition(name td TASK_UPDATE|TASK_DONT_ADD_PRINCIPAL_ACE varEmpty varEmpty td.Principal.LogonType)
