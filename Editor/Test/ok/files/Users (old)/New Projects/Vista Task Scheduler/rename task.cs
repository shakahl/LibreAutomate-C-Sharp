 Works, but sets original trigger.

lpstr name="QM - Macro444"
lpstr name2="QM - Macro444-1"

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
str xml=folder.GetTask(name).Xml
folder.DeleteTask(name 0)

folder.RegisterTask(name2 xml 2 "" "" 3)
