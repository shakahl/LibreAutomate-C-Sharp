 To run these 3 macros, QM must be running as admin.

str s=
 wbemFlagReturnImmediately = 16
 wbemFlagForwardOnly = 32
 IFlags = wbemFlagReturnImmediately + wbemFlagForwardOnly
 
 Set wmiServices = GetObject("winmgmts:root/cimv2") 
 Set colProc = wmiServices.ExecNotificationQuery("SELECT * FROM Win32_ProcessStartTrace",, IFlags)
 
 Do While (True)
 Set ProcEvent = colProc.NextEvent
 MsgBox ProcEvent.ProcessName
 Loop

VbsExec s


 Press Pause to stop.
