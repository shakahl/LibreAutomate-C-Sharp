\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll user32 #RegisterHotKey hWnd id fsModifiers vk
dll user32 #UnregisterHotKey hWnd id

if(getopt(nthreads)>1) ret
if(!ShowDialog("MyHotKeys" &MyHotKeys 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 107 0 "My Hot Keys"
 END DIALOG
 DIALOG EDITOR: "" 0x2010505 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	
	 <EDIT THIS>
	
	RegisterHotKey(hDlg 1 0 VK_MEDIA_PLAY_PAUSE)
	RegisterHotKey(hDlg 2 0 VK_MEDIA_STOP)
	RegisterHotKey(hDlg 3 0 VK_MEDIA_NEXT_TRACK)
	RegisterHotKey(hDlg 4 0 VK_MEDIA_PREV_TRACK)
	 You can register more keys, with id 5, 6, ...
	 Third argument can be modifier flags: 1 Alt, 2 Ctrl, 4 Shift, 8 Win
	 Example: register hot key Ctrl+Win+A:
	 RegisterHotKey(hDlg 5 2|8 'A')
	int+ nHotKeys=4 ;;set this to the id of the last registered hot key
	ret 1
	
	case WM_HOTKEY
	sel wParam
		case 1 mac "Macro1" ;;Replace "Macro1" etc to the real macro names
		case 2 mac "Macro2"
		case 3 mac "Macro3"
		case 4 mac "Macro4"
		 ... (other registered keys)
	
	 <END EDIT THIS>
	
	case WM_DESTROY
	DT_DeleteData(hDlg)
	for(_i 1 nHotKeys+1) UnregisterHotKey(hDlg _i)
	
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case IDCANCEL DT_Cancel hDlg
ret 1
