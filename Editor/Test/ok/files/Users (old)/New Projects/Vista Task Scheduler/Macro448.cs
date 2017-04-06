typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
TaskScheduler.IRegisteredTask rt=folder.GetTask("QM - Macro446")
TaskScheduler.ITaskDefinition td=rt.Definition
 out td.Principal.RunLevel

 TaskScheduler.IPrincipal pr=td.Principal
 pr.RunLevel=0
 out rt.GetSecurityDescriptor(OWNER_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(GROUP_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(DACL_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(SACL_SECURITY_INFORMATION)

 out rt.GetSecurityDescriptor(PROTECTED_DACL_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(UNPROTECTED_DACL_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(PROTECTED_SACL_SECURITY_INFORMATION)
 out rt.GetSecurityDescriptor(UNPROTECTED_SACL_SECURITY_INFORMATION)
 pr.Id
 out td.Principal.RunLevel

rt.SetSecurityDescriptor("S-1-16-4096" TaskScheduler.TASK_DONT_ADD_PRINCIPAL_ACE)

 folder.RegisterTaskDefinition("QM - Macro446" td 4 @ @ 3) ;;should allow @ for nonoptional args
VARIANT v
folder.RegisterTaskDefinition("QM - Macro446" td 4 v v 3)
 folder.RegisterTaskDefinition("QM - Macro446" td 4 v v 3 "S-1-16-4096")


 Medium
 O:BA
 G:S-1-5-21-561171799-1882743267-3273394223-513
 D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)

 High
 O:S-1-5-21-561171799-1882743267-3273394223-1000
 G:S-1-5-21-561171799-1882743267-3273394223-513
 D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;S-1-5-21-561171799-1882743267-3273394223-1000)
