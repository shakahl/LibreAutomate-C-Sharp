 lpstr taskName="QM - set_time"
lpstr taskName="QM - Macro1184"

ITaskScheduler ts._create(CLSID_CTaskScheduler)
ITask task

BSTR bName=taskName
ts.Activate(bName uuidof(ITask) &task)

word* w
task.GetAccountInformation(&w)
out _s.ansi(w)
CoTaskMemFree w
