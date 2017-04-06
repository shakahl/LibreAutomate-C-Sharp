str s=
 set processClass = GetObject ("winmgmts:Win32_Process")
 retVal = processClass.Create ("notepad.exe")
VbsExec s
