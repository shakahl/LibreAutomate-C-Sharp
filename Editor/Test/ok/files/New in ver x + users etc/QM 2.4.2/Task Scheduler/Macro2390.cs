out
typelib TaskScheduler {e34cb9f1-c7f7-424c-be29-027dcc09363a} 1.0
TaskScheduler.TaskScheduler ts._create
ts.Connect
TaskScheduler.ITaskFolder folder=ts.GetFolder("Quick Macros")

Dir d
foreach(d "$system$\Tasks\Quick Macros-\QM - *" FE_Dir)
	str path=d.FullPath
	out path
	str s.getfile(path)
	BSTR td.alloc(s.len/2-1); memcpy td.pstr s+2 s.len-2
	
	VARIANT varEmpty
	
	str name=d.FileName
	
	str sddl
	sddl="D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;FA;;;S-1-5-21-364929558-101999248-426651109-1001)"
	TaskScheduler.IRegisteredTask t=folder.RegisterTask(name td TASK_CREATE_OR_UPDATE varEmpty varEmpty TASK_LOGON_INTERACTIVE_TOKEN sddl)
	err out "Error: %s" name; continue
	out "OK: %s" name
