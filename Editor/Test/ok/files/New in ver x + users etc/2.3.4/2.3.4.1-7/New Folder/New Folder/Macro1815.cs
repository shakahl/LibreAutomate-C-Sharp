 select ThreadCount from Win32_Process WHERE name="process.ext"

Q &q
def wbemFlagReturnImmediately 16
def wbemFlagForwardOnly 32
int IFlags = wbemFlagReturnImmediately|wbemFlagForwardOnly
Q &qq
IDispatch objWMIService._getfile("winmgmts:")
Q &qqq
IDispatch colProcesses = objWMIService.ExecQuery("select ThreadCount from Win32_Process WHERE name=''qm.exe''",@,IFlags)

IDispatch x
foreach x colProcesses
	out x.ThreadCount
Q &qqqq
outq
