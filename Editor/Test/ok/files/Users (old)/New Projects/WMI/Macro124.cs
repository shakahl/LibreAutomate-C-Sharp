 $vbs_1
 'this will launch notepad through WMI
 set process = GetObject("winmgmts:{impersonationLevel=impersonate}!Win32_Process")
 result = process.Create ("notepad.exe",null,null,processid)
VbsExec vbs_1
