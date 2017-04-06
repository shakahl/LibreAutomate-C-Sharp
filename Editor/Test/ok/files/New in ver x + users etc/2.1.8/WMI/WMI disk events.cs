IDispatch serv._getfile("Winmgmts:")
str q="Select * from __InstanceModificationEvent within 10 WHERE TargetInstance ISA 'Win32_LogicalDisk'"
IDispatch colMonitoredDisks=serv.ExecNotificationQuery(q)

 rep
	IDispatch strDiskChange = colMonitoredDisks.NextEvent
	 IDispatch i=strDiskChange.TargetInstance
	 out i.DriveType
	int i=strDiskChange.TargetInstance.DriveType
