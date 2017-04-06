def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32
int IFlags = wbemFlagReturnImmediately|wbemFlagForwardOnly
IDispatch objWMIService._getfile("winmgmts:")
IDispatch colProcesses = objWMIService.ExecQuery("SELECT ProcessId,CommandLine FROM Win32_Process WHERE Name='devenv.exe'",@,IFlags)

IDispatch objProcess
foreach objProcess colProcesses
	int pid=objProcess.ProcessId
	str cl=PathGetArgs(objProcess.CommandLine)
	cl.trim("''")
	out cl
