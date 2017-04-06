int w=win("Sqlite Test" "#32770")
 clo w
PF
 but 100 w ;;checkpoint
PostMessage w WM_COMMAND 100 0
PN;PO
0.1

str sf="Q:\my qm\test\ok.qml"
str sf2="Q:\my qm\test\backup.qml"
int nFailed
rep 1
	PF
	if(!CopyFile(sf sf2 0)) out _s.dllerror; nFailed+1
	PN; PO
out nFailed

out Crc32("$my qm$\test\ok.qml" 0 1)
out Crc32("$my qm$\test\backup.qml" 0 1)
