
 Saves this scheduled task.
 The task is not active and not added to the Task Scheduler until you call this function.
 Clears this variable, so you cannot call ScheduleX functions after, but can call Create.


__ImpersonateStandardUser imp.Impersonate ;;don't save as admin, because then nonadmin cannot delete, the program runs as admin, etc
IPersistFile iFile=+m_task
iFile.Save(0 1)
err
	if(_hresult&0xffff!ERROR_ACCESS_DENIED) end _error
	iFile.Save(0 1)
m_task=0

#if !EXE
__UpdateScheduledItems
#endif

err+ end _error

 Windows bug: Save fails if was admin task. Although Create2 deleted it, but it seems scheduler service needs some time between deleting and creating task with same name again.
 Workaround: Save again. Then does not fail. The task remains admin, the program runs as admin, but the task can be deleted always.
 Other way - in Create2 wait 1 s after deleting. It would fix all, but slow.
