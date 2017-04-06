Dir d
str s1 s2
int t=GetTickCount
int n
rep
	n+1
	 d.dir("$my qm$\test\ok.db3"); long tm=d.TimeModified(0 0 0 1)
	 FileCopy "$my qm$\test\ok.db3" "$my qm$\test\ok-2.db3"; err out _error.description
	if(!CopyFile(s1.expandpath("$my qm$\test\ok.db3") s2.expandpath("$my qm$\test\ok-2.db3") 0)) out _s.dllerror
	 d.dir("$my qm$\test\ok.db3"); out d.TimeModified(0 0 0 1)-tm
	0.01
	if(GetTickCount-t>2000) break

 out n
