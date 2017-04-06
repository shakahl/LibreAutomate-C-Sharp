def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32
int IFlags = wbemFlagReturnImmediately|wbemFlagForwardOnly
IDispatch objWMIService._getfile("winmgmts:")
IDispatch colProcesses = objWMIService.ExecQuery("SELECT Name,ProcessId,ExecutablePath,CommandLine FROM Win32_Process",@,IFlags)

IDispatch objProcess
foreach objProcess colProcesses
	int pid=objProcess.ProcessId
	str name=objProcess.Name
	if(!pid or name~"System") continue
	str path=objProcess.ExecutablePath; err path=""
	str cl=PathGetArgs(objProcess.CommandLine); err cl=""
	cl.trim("''")
	out "------------[]name: %s[]path: %s[]pid:  %i[]cl:   %s" name path pid cl
