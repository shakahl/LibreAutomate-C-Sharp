out
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
PF
TaskScheduler.TaskScheduler ts._create
ts.Connect
PN;PO
rep 1
	PF
	int root=0
	str ss.all
	TaskScheduler.ITaskFolder folder=ts.GetFolder(iif(root "\" "\Quick Macros"))
	TaskScheduler.IRegisteredTask task
	foreach task folder.GetTasks(TASK_ENUM_HIDDEN)
		str name=task.Name
		ss.addline(name)
		 str xml=task.Xml
		 ss.addline(_s.from(Crc32(xml xml.len)))
	PN;PO
	out ss
	1
