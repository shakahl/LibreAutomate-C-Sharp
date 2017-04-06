IDispatch sv._getfile("winmgmts:")
IDispatch evs=sv.ExecNotificationQuery("SELECT * FROM Win32_ProcessStartTrace" @ 16|32)
IDispatch pr

rep
	pr=evs.NextEvent(-1)
	out pr.ProcessName


 Press Pause to stop.
