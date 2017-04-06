out
#compile "__ConsoleProcess"
ConsoleProcess x
x.Exec("$my qm$\console2.exe /ab cd")
str s u
rep
	if(!x.ReadStdout(s 2)) break
	 int r=x.ReadStdout(s 3); if(!r) break; else if(r=-1) 0.1; continue
	out F"<{s}>"
	 foreach(u s) out F"<{u}>"
out x.GetExitCode
