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
     <CalendarTrigger>
       <StartBoundary>2014-07-03T13:41:00</StartBoundary>
       <Enabled>true</Enabled>
       <ScheduleByDay>
         <DaysInterval>1</DaysInterval>
       </ScheduleByDay>
     </CalendarTrigger>
   </Triggers>
   <Principals>
     <Principal id="Author">
       <LogonType>InteractiveToken</LogonType>
       <RunLevel>LeastPrivilege</RunLevel>
     </Principal>
   </Principals>
   <Settings>
     <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
     <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
     <IdleSettings>
       <Duration>PT600S</Duration>
       <WaitTimeout>PT3600S</WaitTimeout>
       <StopOnIdleEnd>false</StopOnIdleEnd>
       <RestartOnIdle>false</RestartOnIdle>
     </IdleSettings>
     <Enabled>true</Enabled>
     <Hidden>false</Hidden>
     <RunOnlyIfIdle>false</RunOnlyIfIdle>
     <WakeToRun>false</WakeToRun>
     <ExecutionTimeLimit>PT259200S</ExecutionTimeLimit>
     <Priority>5</Priority>
   </Settings>
   <Actions Context="Author">
     <Exec>
       <Command>Q:\app\qmcl.exe</Command>
       <Arguments>T MS "{5e5707bf-070a-4fd9-9eca-cc76bf60c2fc}Function294"</Arguments>
     </Exec>
   </Actions>
 </Task>

VARIANT varEmpty
 folder.RegisterTask("QM - XP User" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - XP Admin" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - Vista User" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - Vista Admin" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)

 BEGIN PROJECT
 main_function  TaskScheduler add task XML2
 exe_file  $my qm$\TaskScheduler add task XML2.qmm
 flags  6
 guid  {95BA589A-1501-4F19-95EF-C18B694ED144}
 END PROJECT
