typelib WbemScripting {565783C6-CB41-11D1-8B02-00600806D9B6} 1.1
 #opt dispatch 1

interface@ WmiObject :IDispatch Create(BSTR'a BSTR'd SWbemObject'n *i) {WbemScripting.ISWbemObject}

WmiObject o._getactive("winmgmts:{impersonationLevel=impersonate}!Win32_Process")

o.Create("notepad.exe" "" 0 &_i)
