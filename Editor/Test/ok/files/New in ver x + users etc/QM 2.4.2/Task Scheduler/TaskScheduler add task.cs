/exe 1

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
 TaskScheduler.ITaskFolder folder=ts.GetFolder("\Quick Macros")
 err folder=ts.GetFolder("\").CreateFolder("Quick Macros")

TaskScheduler.ITaskDefinition td=ts.NewTask(0)

TaskScheduler.IRegistrationInfo ri=td.RegistrationInfo
ri.Author="Quick Macros"

TaskScheduler.ITaskSettings sett=td.Settings
sett.StartWhenAvailable=TRUE

TaskScheduler.IExecAction ea=+td.Actions.Create(TaskScheduler.TASK_ACTION_EXEC)
ea.Path=_s.expandpath("$qm$\qmcl.exe")
ea.Arguments="M ''TestSchedule'' ''test command''"

TaskScheduler.ITimeTrigger trig=+td.Triggers.Create(TASK_TRIGGER_TIME)
trig.StartBoundary="2014-07-02T16:59:42"

 #ret


VARIANT varEmpty
folder.RegisterTaskDefinition("Quick Macros\G" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
folder.RegisterTaskDefinition("Quick Macros\Users" td TASK_CREATE_OR_UPDATE "Users" varEmpty TASK_LOGON_GROUP) ;;error Access is denied if QM as User
folder.RegisterTaskDefinition("Quick Macros\A Users" td TASK_CREATE_OR_UPDATE "Authenticated Users" varEmpty TASK_LOGON_GROUP) ;;error Access is denied if QM as User
folder.RegisterTaskDefinition("Quick Macros\Interactive" td TASK_CREATE_OR_UPDATE "INTERACTIVE" varEmpty TASK_LOGON_GROUP) ;;if QM is User, no error, but does not create task
folder.RegisterTaskDefinition("Quick Macros\SID Interactive" td TASK_CREATE_OR_UPDATE "S-1-5-4" varEmpty TASK_LOGON_GROUP) ;;if QM is User, no error, but does not create task
folder.RegisterTaskDefinition("Quick Macros\SID AU" td TASK_CREATE_OR_UPDATE "S-1-5-11" varEmpty TASK_LOGON_GROUP) ;;if QM is User, no error, but does not create task
folder.RegisterTaskDefinition("Quick Macros\SID Everyone" td TASK_CREATE_OR_UPDATE "S-1-1-0" varEmpty TASK_LOGON_GROUP) ;;if QM is User, no error, but does not create task
folder.RegisterTaskDefinition("Quick Macros\SID Users" td TASK_CREATE_OR_UPDATE "S-1-5-32-545" varEmpty TASK_LOGON_GROUP) ;;if QM is User, no error, but does not create task

 TaskScheduler.IPrincipal pr=td.Principal
 pr.GroupId="Users"
 pr.LogonType=TASK_LOGON_GROUP
 folder.RegisterTaskDefinition("QM - Test3" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_GROUP varEmpty) ;;error Access is denied if QM as User

 folder.RegisterTaskDefinition("QM - Test1" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTaskDefinition("QM - Test2" td TASK_CREATE_OR_UPDATE "Users" varEmpty TASK_LOGON_GROUP) ;;error Access is denied if QM as User

 BEGIN PROJECT
 main_function  Macro2359
 exe_file  $my qm$\Macro2359.qmm
 flags  6
 guid  {F22CA88D-DDA1-4D0A-922B-3CD02BE5A076}
 END PROJECT
