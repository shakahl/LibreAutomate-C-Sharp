function# hWnd message wParam lParam
if(hWnd) goto messages
if(getopt(nthreads)>1) ret

 Shows when battery mode begins and ends.
 Can be extended for other power events.
 Uses the same messages as in 'Power management triggers', but without a dialog.
 This function runs all the time. To end it, use Threads dialog or Running items pane.
 You'll have to end it to apply changes after editing.


MainWindow 0 "QM_Power" &power_events 0 0 0 0 WS_POPUP

ret
 messages
int-- AC
SYSTEM_POWER_STATUS p
sel message
	case WM_CREATE
	GetSystemPowerStatus &p
	AC=p.ACLineStatus
	
	case WM_POWERBROADCAST
	 out wParam
	 bee 200 1000
	sel wParam
		case PBT_APMPOWERSTATUSCHANGE
		GetSystemPowerStatus &p
		if(p.ACLineStatus!=AC)
			AC=p.ACLineStatus
			
			out iif(AC "AC" "battery")
			
			 To run a macro when battery mode begins or ends, use mac here. Example:
			 mac "BatteryMode" "" !AC ;;first argument will be 1 if battery mode began, 0 if ended.
			
		 This code also is executed on other power events.
		 And you can find more info in other p members.
		 Read more in MSDN Library, about PBT_APMPOWERSTATUSCHANGE and SYSTEM_POWER_STATUS.
		
	case WM_DESTROY
	PostQuitMessage 0

ret DefWindowProc(hWnd message wParam lParam)
