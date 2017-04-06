out
#compile "__ConsoleProcess"
ConsoleProcess x
x.Exec("$my qm$\console3.exe")
str s
int i
for i 1 1000
	if(!x.ReadStdout(s 2)) break
	out F"<{s}>"
	x.WriteStdin(F"input={i}")
