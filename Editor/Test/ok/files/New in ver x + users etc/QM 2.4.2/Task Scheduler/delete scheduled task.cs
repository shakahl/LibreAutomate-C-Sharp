 /exe 1

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
 TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
TaskScheduler.ITaskFolder folder=ts.GetFolder("Quick Macros")
str name="QM - Macro2373"

TaskScheduler.IRegisteredTask t=folder.GetTask(name)
str s1 s2
s1=t.GetSecurityDescriptor(DACL_SECURITY_INFORMATION)
s2=t.GetSecurityDescriptor(OWNER_SECURITY_INFORMATION)
out s1
out s2
ret

folder.DeleteTask(name 0)
err mes _error.description

 BEGIN PROJECT
 main_function  delete scheduled task
 exe_file  $my qm$\delete scheduled task.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {E684A43E-EE2E-4ED2-9AAF-DEFB92975518}
 END PROJECT
