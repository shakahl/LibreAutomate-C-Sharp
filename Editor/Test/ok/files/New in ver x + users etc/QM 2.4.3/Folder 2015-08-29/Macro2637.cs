out
PerfFirst
str sFolder="C:\*"
str sPath
Dir d
int n
 foreach(d sFolder FE_Dir 5)
foreach(d sFolder FE_Dir 6)
	sPath=d.FileName(1)
	out sPath
	n+1
PerfNext;PerfOut
out n
 n: 500974 files+folders, 92897 folders

 FindFirstFile:
 speed: 31608175  after reboot.   for files+folders (63 micros/file)
 speed: 11242001  later

 FindFirstFileEx(...FindExInfoBasic..FIND_FIRST_EX_LARGE_FETCH):
 speed: 19987875  after reboot
 speed: 6496472  later
