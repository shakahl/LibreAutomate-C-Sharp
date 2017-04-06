typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
TaskScheduler.ITaskDefinition td=ts.NewTask(0)

IAction a=td.Actions.Create(TASK_ACTION_EXEC)
a.Id="id2"
TaskScheduler.IExecAction ea=+a
ea.Path="c:\windows\system32\notepad.exe"

folder.RegisterTaskDefinition("test" td 2 "" "" 3 "")
