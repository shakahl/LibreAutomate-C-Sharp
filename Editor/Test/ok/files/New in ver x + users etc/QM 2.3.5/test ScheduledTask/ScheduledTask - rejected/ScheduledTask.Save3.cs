function [flags] ;;flags: 1 admin

 Saves this scheduled task.
 The task is not active and not added to the Task Scheduler until you call this function.
 Clears this variable, so you cannot call ScheduleX functions after, but can call Create.


 __ImpersonateStandardUser imp.Impersonate
__ImpersonateStandardUser imp; if(!(flags&1)) imp.Impersonate
IPersistFile iFile=+m_task
m_task=0
iFile.Save(0 1)
err
	if(_hresult!E_ACCESSDENIED) end _error
	 workaround for Windows bug: if was admin task, first Save fails. Other way - in Create2 wait 1 s after deleting.
	iFile.Save(0 1)
	out "revert"

#if !EXE
__UpdateScheduledItems
#endif

 err+ end _error
