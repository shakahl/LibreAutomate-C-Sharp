/exe 1

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
 TaskScheduler.ITaskFolder folder=ts.GetFolder("\Quick Macros")
 err folder=ts.GetFolder("\").CreateFolder("Quick Macros")

str td=
 <?xml version="1.0" encoding="UTF-16"?>
 <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <RegistrationInfo>
    <Author>Quick Macros</Author>
  </RegistrationInfo>
  <Triggers>
    <TimeTrigger>
      <StartBoundary>2014-07-02T16:18:42</StartBoundary>
      <Enabled>true</Enabled>
    </TimeTrigger>
  </Triggers>
  <Principals>
    <Principal id="Author">
      <RunLevel>LeastPrivilege</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>true</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
    <AllowHardTerminate>true</AllowHardTerminate>
    <StartWhenAvailable>true</StartWhenAvailable>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <IdleSettings>
      <StopOnIdleEnd>true</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
    <AllowStartOnDemand>true</AllowStartOnDemand>
    <Enabled>true</Enabled>
    <Hidden>false</Hidden>
    <RunOnlyIfIdle>false</RunOnlyIfIdle>
    <WakeToRun>false</WakeToRun>
    <ExecutionTimeLimit>P3D</ExecutionTimeLimit>
    <Priority>7</Priority>
  </Settings>
  <Actions Context="Author">
    <Exec>
      <Command>qmcl.exe</Command>
      <Arguments>M "TestSchedule" "test command"</Arguments>
    </Exec>
  </Actions>
 </Task>

td.findreplace("qmcl.exe" _s.expandpath("$qm$\qmcl.exe") 4) ;;does not work if no path

VARIANT varEmpty
 folder.RegisterTask("QM - XML" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
 folder.RegisterTask("QM - XML2" td TASK_CREATE_OR_UPDATE "Users" varEmpty TASK_LOGON_GROUP)
folder.RegisterTask("Quick Macros\XML" td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)
folder.RegisterTask("Quick Macros\XML Users" td TASK_CREATE_OR_UPDATE "Users" varEmpty TASK_LOGON_GROUP)


 BEGIN PROJECT
 main_function  TaskScheduler add task XML
 exe_file  $my qm$\TaskScheduler add task XML.qmm
 flags  6
 guid  {3AC8C1CA-255F-4B91-89F4-9137E00383C9}
 END PROJECT
