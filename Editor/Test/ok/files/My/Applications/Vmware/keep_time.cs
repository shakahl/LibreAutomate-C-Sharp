 Keeps time specified in _command, in format "yyyy-MM-dd".
 On virtual PC create exe from this. Then create a scheduled task that runs at system startup as SYSTEM.
 Every 1 s gets time, and restores if it is not the specified date.
 Need it because vmware sets vitual PC time to the host time eg when host time changes.

lpstr s=_command
int y=val(s 0 _i); s+_i+1
int m=val(s 0 _i); s+_i+1
int d=val(s 0 _i)
 out "%i %i %i" y m d

rep
	SYSTEMTIME t
	GetSystemTime &t
	if t.wYear!y or t.wMonth!m or t.wDay!d
		t.wYear=y; t.wMonth=m; t.wDay=d
		SetSystemTime &t
	1

 BEGIN PROJECT
 main_function  keep_time
 exe_file  c:\keep_time.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  0
 guid  {9BF30AA9-8549-4CA8-AC91-8DB0EBDD0A6E}
 END PROJECT
