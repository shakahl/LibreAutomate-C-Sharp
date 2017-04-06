 \
function $solutionPath

 Activates main window of specified Visual Studio solution.

 solutionPath - solution path, like "Q:\app\app.sln".
   It must be command line of the solution process.


def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32
int IFlags = wbemFlagReturnImmediately|wbemFlagForwardOnly
IDispatch objWMIService._getfile("winmgmts:")
IDispatch colProcesses = objWMIService.ExecQuery("SELECT ProcessId,CommandLine FROM Win32_Process WHERE Name='devenv.exe'",@,IFlags)

IDispatch objProcess
foreach objProcess colProcesses
	str cl=PathGetArgs(objProcess.CommandLine)
	cl.trim("''")
	if cl~solutionPath
		int pid=objProcess.ProcessId
		act win("Microsoft Visual" "wndclass_desked_gsk" pid)
		break
