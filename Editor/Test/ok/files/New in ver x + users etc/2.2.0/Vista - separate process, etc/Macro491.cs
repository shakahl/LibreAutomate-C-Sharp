spe
int hp
Q &q
 StartProcess 2 "regedit.exe" "" "" &hp
out StartProcess(1 "$common documents$\my qm\Dialog30.exe" "" "" &hp)
 out StartProcess(1 "c:\users\user\documents\my qm\Dialog30.exe" "" "" &hp)
 out StartProcess(2 "notepad.exe" "" "" &hp)
 run "$my qm$\Dialog30.exe" "" "" "" 0x20000
 hp=run("$qm$")
 hp=run("$qm$" "" "" "" 0x20000)
 hp=run("notepad.exe" "" "" "" 0x20000)
out hp
Q &qq
outq
wait 0 H hp
CloseHandle hp
