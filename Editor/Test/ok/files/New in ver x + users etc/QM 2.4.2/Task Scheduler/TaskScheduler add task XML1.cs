/exe 1

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")

str td=
 <?xml version="1.0" encoding="UTF-16"?>
 <Task version="1.1" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
   <RegistrationInfo>
     <Author>QM</Author>
   </RegistrationInfo>
   <Triggers>
     <TimeTrigger>
       <StartBoundary>2014-07-07T07:16:30</StartBoundary>
     </TimeTrigger>
   </Triggers>
   <Principals>
     <Principal id="Author">
       <LogonType>InteractiveToken</LogonType>
       <RunLevel>LeastPrivilege</RunLevel>
     </Principal>
   </Principals>
   <Actions Context="Author">
     <Exec>
       <Command>notepad.exe</Command>
     </Exec>
   </Actions>
 </Task>

VARIANT varEmpty
 folder.RegisterTask("QM - XP User" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - XP Admin" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - Vista User" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - Vista Admin" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)

str name="QM - Macro2361"
folder.DeleteTask(name 0); err

str sddl
 sddl="D:AI(A;;FA;;;BU)O:S-1-5-21-364929558-101999248-426651109-1001"
sddl="D:AI(A;;FA;;;BU)"
TaskScheduler.IRegisteredTask t=folder.RegisterTask(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN sddl)
 TaskScheduler.IRegisteredTask t=folder.RegisterTask(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
__ScheduleUpdated

str s1 s2
s1=t.GetSecurityDescriptor(DACL_SECURITY_INFORMATION)
s2=t.GetSecurityDescriptor(OWNER_SECURITY_INFORMATION)
out s1
out s2


 BEGIN PROJECT
 main_function  TaskScheduler add task XML2
 exe_file  $my qm$\TaskScheduler add task XML2.qmm
 flags  6
 guid  {95BA589A-1501-4F19-95EF-C18B694ED144}
 END PROJECT
