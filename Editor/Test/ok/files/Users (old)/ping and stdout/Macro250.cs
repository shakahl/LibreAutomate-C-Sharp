str strComputer="."
IDispatch objWMIService._getfile("winmgmts:\\.\root\cimv2")
IDispatch colPings = objWMIService.ExecQuery("Select * From Win32_PingStatus WHERE (Address = 'www.quickmacros.com') OR (Address = 'www.yahoo.com') OR (Address = 'www.microsoft.com')")
IDispatch objStatus
foreach objStatus colPings
	_s=objStatus.Address
	out "-- %s --" _s
	VARIANT sc=objStatus.StatusCode
	if(sc.vt=VT_NULL or sc.lVal) out "did not respond"; continue
	out objStatus.Timeout
	