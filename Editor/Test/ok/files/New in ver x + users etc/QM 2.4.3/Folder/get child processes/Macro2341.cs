out
CreateProcessSimple "notepad.exe"
CreateProcessSimple "calc.exe"
ARRAY(int) a
ARRAY(str) as
PF
GetChildProcesses(0 a as)
PN;PO
int i
for(i 0 a.len) out a[i]
for(i 0 as.len) out as[i]
