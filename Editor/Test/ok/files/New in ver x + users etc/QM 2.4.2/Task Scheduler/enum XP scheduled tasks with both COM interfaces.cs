out
ITaskScheduler ts._create(CLSID_CTaskScheduler)
IEnumWorkItems en
ts.Enum(&en)
word** a
int i n
en.Next(20 &a &n)
out n
for i 0 n
	out _s.ansi(a[i])
	

out "----------"
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler tss._create
tss.Connect
TaskScheduler.ITaskFolder folder=tss.GetFolder("\")
TaskScheduler.IRegisteredTask task
foreach task folder.GetTasks(TASK_ENUM_HIDDEN)
	out task.Name
