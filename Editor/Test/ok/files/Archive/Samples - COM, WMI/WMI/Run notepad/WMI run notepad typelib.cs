typelib WbemScripting {565783C6-CB41-11D1-8B02-00600806D9B6} 1.1

WbemScripting.SWbemObject pr._getfile("winmgmts:Win32_Process")
IDispatch oInParams = pr.Methods_.Item("Create" 0).InParameters.SpawnInstance_(0)
oInParams.CommandLine = "Notepad.exe"
pr.ExecMethod_("Create" oInParams 0 0)
