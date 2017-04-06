str name="QM - Macro458" ;;high
 str name="QM - Macro458" ;;medium

typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
TaskScheduler.IRegisteredTask rt=folder.GetTask(name)
TaskScheduler.ITaskDefinition td=rt.Definition

 out td.Principal.RunLevel
td.Principal.RunLevel=0

 str sddl="O:BAG:S-1-5-21-561171799-1882743267-3273394223-513D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)"
 str sddl="O:BAG:S-1-5-21-561171799-1882743267-3273394223-513D:(A;;FA;;;SY)(A;;FA;;;WD)(A;ID;0x1f019f;;;WD)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)"
 str sddl="O:BAG:S-1-5-21-561171799-1882743267-3273394223-513D:(A;;FA;;;WD)(A;;FA;;;SY)(A;;FA;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)"
str sddl="D:(A;;FA;;;WD)"
rt.SetSecurityDescriptor(sddl 0)
 rt.SetSecurityDescriptor(sddl TaskScheduler.TASK_DONT_ADD_PRINCIPAL_ACE)
 
  folder.RegisterTaskDefinition(name td 4 @ @ 3) ;;should allow @ for nonoptional args
VARIANT v
folder.RegisterTaskDefinition(name td 4 v v 3)
 
 
  Medium
  O:BA
  G:S-1-5-21-561171799-1882743267-3273394223-513
  D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)
  full without sacl
  O:BAG:S-1-5-21-561171799-1882743267-3273394223-513D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)

  High
  O:S-1-5-21-561171799-1882743267-3273394223-1000
  G:S-1-5-21-561171799-1882743267-3273394223-513
  D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;S-1-5-21-561171799-1882743267-3273394223-1000)
  full without sacl
  O:BAG:S-1-5-21-561171799-1882743267-3273394223-513D:(A;;FX;;;SY)(A;;FR;;;S-1-5-21-561171799-1882743267-3273394223-1000)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)
