PF
int n
Dir d

str s
 s="q:\app\*" ;;5615 files, speed 175/35
 s="C:\Program Files\*" ;;38733 files, speed 1160/347
 s="c:\windows\system32\*" ;;14976 files, speed 299/151
 s="E:\app\2.4.3.8\*" ;;(HDD) 5209 files, speed 2400/35
 s="E:\app\2.4.3.6\*" ;;(HDD) 3373 files, speed 3940/13

s="q:\test\ok\*" ;;13630 files, Avast 462/151, notebook 12s/360
 s="E:\test\ok\*" ;;(HDD) 13630 files, Avast 2592/166
 s="G:\test\ok\*" ;;(Sandisk flash) 13630 files, no AV 1605/131

foreach(d s FE_Dir 2|4)
	str path=d.FullPath
	 out path
	n+1
PN;PO
out n
