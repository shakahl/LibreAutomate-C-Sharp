 Enumerates processes using VBScript.
 This code is taken from WMI documentation, and is slightly modified (Wscript.Echo replaced with MsgBox).

str s=
 wbemFlagReturnImmediately = 16
 wbemFlagForwardOnly = 32
 IFlags = wbemFlagReturnImmediately + wbemFlagForwardOnly
 set objWMIService = GetObject("winmgmts:")
 ' Query for all the Win32_Process objects on the 
 '     local machine and use forward-only enumerator
 set colProcesses = objWMIService.ExecQuery("SELECT Name FROM Win32_Process",,IFlags)
 ' Receive each object as it arrives
 For Each objProcess in colProcesses
     MsgBox objProcess.name
 Next

VbsExec s
