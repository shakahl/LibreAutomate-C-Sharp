 Enumerates processes.
 This code is translated from VBScript to QM.

def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32
int IFlags = wbemFlagReturnImmediately|wbemFlagForwardOnly
IDispatch objWMIService._getfile("winmgmts:")
IDispatch colProcesses = objWMIService.ExecQuery("SELECT Name FROM Win32_Process",@,IFlags)

IDispatch objProcess
foreach objProcess colProcesses
	out objProcess.Name
