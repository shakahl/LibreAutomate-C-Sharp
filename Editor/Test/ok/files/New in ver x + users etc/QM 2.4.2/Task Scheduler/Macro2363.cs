out
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("\")
 TaskScheduler.IRegisteredTask taskU=folder.GetTask("QM - XP User")
 TaskScheduler.IRegisteredTask taskA=folder.GetTask("QM - XP Admin")
int k=DACL_SECURITY_INFORMATION
 int k=OWNER_SECURITY_INFORMATION
 out taskU.GetSecurityDescriptor(k)
 out taskA.GetSecurityDescriptor(k)


TaskScheduler.IRegisteredTask taskXU=folder.GetTask("QM - XP User")
TaskScheduler.IRegisteredTask taskXA=folder.GetTask("QM - XP Admin")
TaskScheduler.IRegisteredTask taskVU=folder.GetTask("QM - Vista User")
TaskScheduler.IRegisteredTask taskVA=folder.GetTask("QM - Vista Admin")
out "<><c 0xff>1</c>"
out taskXU.GetSecurityDescriptor(k)
out taskXU.Xml
out "<><c 0xff>2</c>"
out taskXA.GetSecurityDescriptor(k)
out taskXA.Xml
out "<><c 0xff>3</c>"
out taskVU.GetSecurityDescriptor(k)
out taskVU.Xml
out "<><c 0xff>4</c>"
out taskVA.GetSecurityDescriptor(k)
out taskVA.Xml
ret

 TaskScheduler.IRegisteredTask& rt=taskXA
 TaskScheduler.ITaskDefinition td=rt.Definition
 TaskScheduler.ITrigger trig=+td.Triggers.Item(1)
 trig.StartBoundary="2014-07-04T16:59:42"
 VARIANT varEmpty
 folder.RegisterTaskDefinition(rt.Name td TASK_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN)


#ret
DACL_SECURITY_INFORMATION
D:AI(A;;FX;;;SY)(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;S-1-5-21-364929558-101999248-426651109-1001)
D:AI(A;;FX;;;SY)(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)(A;ID;FA;;;S-1-5-21-364929558-101999248-426651109-1001)
D:AI(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;S-1-5-21-364929558-101999248-426651109-1001)
D:AI(A;;FR;;;S-1-5-21-364929558-101999248-426651109-1001)(A;ID;0x1f019f;;;BA)(A;ID;0x1f019f;;;SY)(A;ID;FA;;;BA)(A;ID;FA;;;S-1-5-21-364929558-101999248-426651109-1001)
in admin added (A;ID;FA;;;BA)
in XP added (A;;FX;;;SY)

OWNER_SECURITY_INFORMATION
O:S-1-5-21-364929558-101999248-426651109-1001
O:BA
O:S-1-5-21-364929558-101999248-426651109-1001
O:BA

 Others same, empty or error.
