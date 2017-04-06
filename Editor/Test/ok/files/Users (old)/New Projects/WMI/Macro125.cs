typelib WbemScripting {565783C6-CB41-11D1-8B02-00600806D9B6} 1.1
 #opt dispatch 1
WbemScripting.SWbemLocator loc._create()
WbemScripting.SWbemServices se=loc.ConnectServer("" "" "" "" "" "" 0 0)
 WbemScripting.SWbemObject oo=se.Get("Win32_Process" 0 0)

 interface@ WmiObject :IDispatch Create {WbemScripting.ISWbemObject}
interface@ WmiObject :IDispatch Create(BSTR'a BSTR'd SWbemObject'n *i) {WbemScripting.ISWbemObject}
WmiObject o
o=+se.Get("Win32_Process" 0 0)
o.Create("notepad.exe" "" 0 &_i)

 WbemScripting.SWbemMethodSet ms=o.Methods_
 VARIANT v
 foreach v ms
	 WbemScripting.SWbemMethod m=v
	 out m.Name


 IUnknown d._getactive("winmgmts:{impersonationLevel=impersonate}!Win32_Process")
