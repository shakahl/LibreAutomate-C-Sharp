str s=
 Sub SINK_OnObjectReady(objObject, objAsyncContext)
     MsgBox (objObject.TargetInstance.Message)
 End Sub
 
 Set objWMIServices = GetObject("WinMgmts:{impersonationLevel=impersonate, (security)}") 
 
 ' Create the event sink object that receives the events
 Set sink = Wscript.CreateObject("WbemScripting.SWbemSink","SINK_")
  
 ' Set up the event selection. SINK_OnObjectReady is called when
 ' a Win32_NTLogEvent event occurs
 objWMIServices.ExecNotificationQueryAsync sink, _
 "SELECT * FROM __InstanceCreationEvent WHERE TargetInstance ISA 'Win32_NTLogEvent'"
 
 MsgBox "Waiting for events"

VbsExec2 s
