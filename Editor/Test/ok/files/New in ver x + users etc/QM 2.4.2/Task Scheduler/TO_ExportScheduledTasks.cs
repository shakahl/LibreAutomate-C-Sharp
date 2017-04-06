\Dialog_Editor

str controls = "5"
str e5
_s.timeformat("{yyyy-MM-dd}")
e5=F"Scheduled tasks {_s}"
if(!ShowDialog("" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 94 "Export QM scheduled tasks"
 3 QM_DlgInfo 0x54000000 0x20000 0 0 226 36 "Exports data of all QM scheduled tasks from Windows Task Scheduler to a macro. Creates or replaces the macro. Then you can import the tasks from the macro to Windows Task Scheduler on any Windows Vista/7/8 computer."
 4 Static 0x54000200 0x0 8 44 24 13 "Macro"
 5 Edit 0x54030080 0x200 32 44 184 13 ""
 1 Button 0x54030001 0x4 64 72 48 14 "OK"
 2 Button 0x54030000 0x4 116 72 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040108 "*" "" "" ""

out

str s
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
int root=1
TaskScheduler.ITaskFolder folder=ts.GetFolder(iif(root "\" "\Quick Macros"))
TaskScheduler.IRegisteredTask task
foreach task folder.GetTasks(TASK_ENUM_HIDDEN)
	str name=task.Name
	if(root and !name.beg("QM - ")) continue
	str xml=task.Xml
	s.formata("[]#region <%s>[]" name)
	s.addline(xml)
	s.formata("#endregion <%s>[]" name)
	
	 out task.Definition.Triggers.Item(1).Id

e5.ReplaceInvalidFilenameCharacters("-")
newitem(e5 s)
