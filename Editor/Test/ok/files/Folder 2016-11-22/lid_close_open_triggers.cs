 stop if started second thread
if getopt(nthreads)>1
	clo win("lid_close_open_triggers" "#32770")
	ret

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "lid_close_open_triggers"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 128)) ret


#sub DlgProc
function# hDlg message wParam lParam

type __POWERBROADCAST_SETTING5219 GUID'PowerSetting DataLength Data

sel message
	case WM_INITDIALOG
	int-- t_isLidOpen=1
	int-- t_h=RegisterPowerSettingNotification(hDlg GUID_LIDSWITCH_STATE_CHANGE DEVICE_NOTIFY_WINDOW_HANDLE)
	if(t_h=0) end ERR_FAILED
	
	case WM_DESTROY
	UnregisterPowerSettingNotification t_h
	
	case WM_POWERBROADCAST
	sel wParam
		case PBT_POWERSETTINGCHANGE
		__POWERBROADCAST_SETTING5219& x=+lParam
		if !memcmp(&x.PowerSetting GUID_LIDSWITCH_STATE_CHANGE sizeof(GUID))
			if x.Data!=t_isLidOpen
				t_isLidOpen=x.Data
				out _s.timeformat(F"Lid {iif(x.Data `opened` `closed`)} at {{TT}.")
				
				 if !t_isLidOpen ;;lid closed
					 mac "lid closed"
		
		case PBT_APMSUSPEND
		out _s.timeformat("Suspended at {TT}.")
		
		case PBT_APMRESUMEAUTOMATIC
		out _s.timeformat("Resumed at {TT}.")
		
		case PBT_APMRESUMESUSPEND
		out _s.timeformat("User-resumed at {TT}.")
		
		case else
		out _s.timeformat(F"{wParam} at {{TT}.")
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
