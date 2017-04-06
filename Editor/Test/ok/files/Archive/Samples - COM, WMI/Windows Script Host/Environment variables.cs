Wsh.IWshShell_Class shell._create
Wsh.IWshEnvironment env=shell.Environment

 Display value of Path variable
out env.Item("Path")

 Display all variables
str s
foreach s env
	out s

 Note: not all variables are included. Use this instead:
 lpstr s0(GetEnvironmentStrings) s(s0)
 rep
	 out s
	 s+len(s)+1
	 if(!s[0]) break
 FreeEnvironmentStrings(s0)
