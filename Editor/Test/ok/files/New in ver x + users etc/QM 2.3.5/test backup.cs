out
str backupFolder="Q:\My QM\test"
str filePath="Q:\My QM\test\test.txt"
str fileName="test"
filePath.lcase; int crc=Crc32(filePath -1)

del- F"{backupFolder}\*.txt"
str sd("ttt") sf
int t0=timeGetTime

int i k t pt
for i 0 40
 for i 0 3
	if(i&7=7) k=5; else k=50
	wait RandomNumber/k
	 if(k<10) 2
	 2
	 0.1
	 wait 0.02
	t=timeGetTime-t0
	sf.format("%s\%08X.%s.%04i.txt" backupFolder crc fileName t)
	sd.setfile(sf)
	ManageBackup sf t t-pt
	pt=t