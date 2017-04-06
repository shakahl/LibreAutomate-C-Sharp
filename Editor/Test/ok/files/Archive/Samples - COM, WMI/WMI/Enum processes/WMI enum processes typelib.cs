 Enumerates processes.
 This code uses type library.

typelib WbemScripting {565783C6-CB41-11D1-8B02-00600806D9B6} 1.1

WbemScripting.SWbemServices objWMIService._getfile("winmgmts:")
WbemScripting.SWbemObjectSet colProcesses = objWMIService.ExecQuery("SELECT Name FROM Win32_Process","WQL",WbemScripting.wbemFlagReturnImmediately|WbemScripting.wbemFlagForwardOnly,0)

WbemScripting.SWbemObject objProcess
foreach objProcess colProcesses
	out objProcess.Properties_.Item("Name" 0).Value
