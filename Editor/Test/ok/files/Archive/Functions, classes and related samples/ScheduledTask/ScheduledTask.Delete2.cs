function! $taskName

 Deletes a non-macro scheduled task (eg created with Create2).
 Returns 1 if successfully deleted, 0 if the task does not exist. Error if exists and failed to delete.

 taskName - scheduled task name.


ITaskScheduler ts._create(CLSID_CTaskScheduler)

ts.Delete(@taskName)
err
	if(_hresult&0xffff=ERROR_FILE_NOT_FOUND) ret
	end _error

ret 1
err+ end _error
