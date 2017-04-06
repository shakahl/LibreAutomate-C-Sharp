 After PC time changes, some files may have future mod time. Then Visual Studio does not work well.
 This macro sets current time for such files in app folder and subfolders.

out
DateTime dNow.FromComputerTime
Dir d
foreach(d "$QM$\*" FE_Dir 4)
	str path=d.FileName(1)
	DateTime tm=d.TimeModified
	if(tm<=dNow) continue
	out path
	out tm.ToStr
	_utime path 0
