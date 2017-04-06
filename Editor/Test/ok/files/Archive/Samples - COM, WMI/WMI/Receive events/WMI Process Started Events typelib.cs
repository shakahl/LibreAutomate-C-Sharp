typelib WbemScripting {565783C6-CB41-11D1-8B02-00600806D9B6} 1.1

WbemScripting.SWbemServices sv._getfile("winmgmts:")
WbemScripting.SWbemEventSource evs=sv.ExecNotificationQuery("SELECT * FROM Win32_ProcessStartTrace" "WQL" WbemScripting.wbemFlagReturnImmediately|WbemScripting.wbemFlagForwardOnly 0)
WbemScripting.SWbemObject pr

rep
	pr=evs.NextEvent(-1)
	out pr.Properties_.Item("ProcessName" 0).Value


 Press Pause to stop.
