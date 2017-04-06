str sf; rget sf "file debug" "Software\GinDi\QM2\settings"
out sf
PF
FileCopy sf F"{sf}-backup"
PN;PO

 speed for "$my qm$\test\ok.qml":
 hard disk - 32 ms
 flash disk - 1.2 s
