 Closes progress dialog, and ends worker thread if still running (if current thread is being ended from outside).

if hdlg
	DestroyWindow(hdlg)
	hdlg=0

if hthread
	wi.End
	if(WaitForSingleObject(hthread 50)) shutdown -6 8 "" hthread
	hthread=0
