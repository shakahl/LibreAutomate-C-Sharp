/exe 1

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("Quick Macros")

str td=
 <?xml version="1.0" encoding="UTF-16"?>
 <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
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

str name="User"
folder.DeleteTask(name 0); err

str sddl
 sddl="D:AI(A;;FA;;;BU)O:S-1-5-21-364929558-101999248-426651109-1001"
 sddl="D:AI(A;;FA;;;BU)"
sddl="D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;FA;;;S-1-5-21-364929558-101999248-426651109-1001)"
TaskScheduler.IRegisteredTask t=folder.RegisterTask(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN sddl)
 __ScheduleUpdated

str s1 s2
s1=t.GetSecurityDescriptor(DACL_SECURITY_INFORMATION)
s2=t.GetSecurityDescriptor(OWNER_SECURITY_INFORMATION)
out s1
out s2

 Admin: D:AI(A;;FA;;;BA)(A;;FA;;;SY)(A;;0x1200a9;;;S-1-5-5-0-159969)(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)
 User:  D:AI(A;;FA;;;S-1-5-21-364929558-101999248-426651109-1001)(A;;FA;;;SY)(A;;0x1200a9;;;S-1-5-5-0-159969)(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)


 BEGIN PROJECT
 main_function  TaskScheduler add task XML2
 exe_file  $my qm$\TaskScheduler add task XML2.qmm
 flags  6
 guid  {95BA589A-1501-4F19-95EF-C18B694ED144}
 END PROJECT
