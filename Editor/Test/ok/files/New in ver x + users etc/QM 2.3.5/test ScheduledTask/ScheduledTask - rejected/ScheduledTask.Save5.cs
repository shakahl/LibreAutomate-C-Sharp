
 Saves this scheduled task.
 The task is not active and not added to the Task Scheduler until you call this function.
 Clears this variable, so you cannot call ScheduleX functions after, but can call Create.


 Impersonate standard user. If saved as admin, nonadmins cannot delete, the program runs as admin, etc.
 However task will not work if QM is "run as", because then gets token from other user's shell window. Could not find a better way.
if _winnt>=6 and IsUserAdmin and GetWindowThreadProcessId(GetShellWindow &_i)
	__Handle ht hp=OpenProcess(PROCESS_QUERY_INFORMATION 0 _i)
	if(OpenProcessToken(hp TOKEN_QUERY|TOKEN_DUPLICATE &ht)) int imp=ImpersonateLoggedOnUser(ht)

IPersistFile iFile=+m_task
iFile.Save(0 1)
err
	if(_hresult&0xffff!ERROR_ACCESS_DENIED) end _error
	iFile.Save(0 1)
m_task=0

#if !EXE
__UpdateScheduledItems
#endif

err+ int e=1
if(imp) RevertToSelf
if(e) end _error

 Windows bug: Save fails if was admin task. Although Create2 deleted it, but it seems scheduler service needs some time between deleting and creating task with same name again.
 Workaround: Save again. Then does not fail. The task remains admin, the program runs as admin, but the task can be deleted always.
 Other way - in Create2 wait 1 s after deleting. It would fix all, but slow.
