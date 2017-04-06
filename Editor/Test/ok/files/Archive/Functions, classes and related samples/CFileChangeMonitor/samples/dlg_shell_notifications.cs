\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile "__CFileChangeMonitor"

if(!ShowDialog("dlg_shell_notifications" &dlg_shell_notifications 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 2 22 220 20 "Don't close this dialog. Open My Documents folder and change something (rename files, etc). Look in QM output."
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	CFileChangeMonitor- t_fcm ;;declare one thread variable for each folder you will monitor
	if(!t_fcm.Register(hDlg "$personal$")) ;;monitor My Documents folder
		mes "failed to set file change notifications" "" "!"
	
	case WM_DESTROY
	t_fcm.Unregister
	case WM_COMMAND goto messages2
	
	case WM_USER+145
	 wParam is SHNOTIFYSTRUCT containing pidl of the file or folder (depending on event). If renamed, also contains the new pidl.
	 lParam probably is wEventId of SHChangeNotify (documented in MSDN library)
	SHNOTIFYSTRUCT& n=+wParam
	str s1 s2
	if(n.dwItem1) PidlToStr +n.dwItem1 &s1
	if(n.dwItem2) PidlToStr +n.dwItem2 &s2
	out "0x%08X %s %s" lParam s1 s2
	
	 notes:
	 SHChangeNotifyRegister documentation says that sometimes multiple messages are combined into 1.
	 Also, sometimes more than 1 notification may be sent for the same event.
	 Sometimes notifications may be not sent. Eg if an app forgets to call SHChangeNotify after it makes a change, and OS does not call it too. Especially in older OS.
	 I don't know, are dwItem1 and dwItem2 always pidls. Maybe sometimes they can be paths or friendly names, like when calling SHChangeNotify with SHCNF_PATH etc.
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
