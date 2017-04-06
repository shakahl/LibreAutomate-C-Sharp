out
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("Quick Macros")

TaskScheduler.IRegisteredTask t=folder.GetTask("QM - Macro2347")
out t

#ret

str td=
 <?xml version="1.0" encoding="UTF-16"?>
 <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
   <Actions Context="Author">
     <Exec>
       <Command>notepad.exe</Command>
     </Exec>
   </Actions>
 </Task>

VARIANT varEmpty

str name="QM - T1"
folder.DeleteTask(name 0); err

str sddl
sddl="D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;FA;;;S-1-5-21-364929558-101999248-426651109-1001)"
TaskScheduler.IRegisteredTask t=folder.RegisterTask(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN sddl)
t=0
t=folder.GetTask(name)
out t.Xml
   <RegistrationInfo>
     <Author>QM</Author>
   </RegistrationInfo>
   <Principals>
     <Principal id="Author">
       <LogonType>InteractiveToken</LogonType>
       <RunLevel>LeastPrivilege</RunLevel>
     </Principal>
   </Principals>
   <Triggers>
     <TimeTrigger>
       <StartBoundary>2014-07-07T07:16:30</StartBoundary>
     </TimeTrigger>
   </Triggers>
