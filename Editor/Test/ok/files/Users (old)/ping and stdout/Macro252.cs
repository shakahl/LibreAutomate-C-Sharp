Wsh.WshShell objShell._create
Wsh.WshExec objScriptExec = objShell.Exec("ping -n 3 -w 1000 www.quickmacros.com")
str s=objScriptExec.StdOut.ReadAll
s.replacerx("[[]]+" "[]")
mes s
