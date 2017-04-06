function# hWnd message wParam lParam
if(hWnd) goto messages

 New drive watcher function.
 Run it. It runs all the time until QM exits. If you run it again (while already running), you can end it.
 Whenever it gets a notification that a new removable drive inserted, it launches function NewDriveInserted. Edit it.
 Works with flash drives/cards and CD/DVD. Does not work with floppy. Gets drive type and other info in NewDriveInserted.


if(getopt(nthreads)>1)
	if(mes("The function is already running. Do you want to end it?" "QM - NewDriveWatcher" "YN?")='Y') shutdown -6 0 "NewDriveWatcher"
	ret

hWnd=CreateWindowEx(0 "#32770" 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
int- wp=SubclassWindow(hWnd &NewDriveWatcher)
MessageLoop
ret

 messages
sel message
	case WM_DEVICECHANGE
	sel wParam
		case DBT_DEVICEARRIVAL
		NDW_Arrival +lParam

ret CallWindowProc(wp hWnd message wParam lParam)
