typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("Quick Macros")
str name="Test"
TaskScheduler.IRegisteredTask rt=folder.GetTask(name)
TaskScheduler.ITaskDefinition td=rt.Definition

TaskScheduler.ITrigger t=td.Triggers.Item(1)
t.ExecutionTimeLimit="PT12H"

VARIANT varEmpty
str sddl
sddl="D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;FA;;;S-1-5-21-364929558-101999248-426651109-1001)"
rt=folder.RegisterTaskDefinition(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN sddl)
