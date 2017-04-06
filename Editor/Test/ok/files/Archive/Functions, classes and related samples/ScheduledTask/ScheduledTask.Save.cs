
 Saves this scheduled task.
 The task is not active and not added to the Task Scheduler until you call this function.
 Clears this variable, so you cannot call ScheduleX functions after, but can call Create.


IPersistFile iFile=+m_task
iFile.Save(0 1)
m_task=0

#ifdef ChangeFileSecurity
if _winnt>=6 and IsUserAdmin ;;grant file modify/delete rights to current user. By default only admins can.
	str sf su; word* f
	iFile.GetCurFile(&f); sf.ansi(f); CoTaskMemFree(f)
	GetUserInfo &su
	ChangeFileSecurity sf F"/E /G {su}:F"; err ;;quite slow, but several times faster than Save
	 Other way - use impersonation. But it creates more problems than solves.
#endif

PostMessage win("" "QM_Editor") WM_USER+60 0 0

err+ end _error
