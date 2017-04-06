\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

type SHChangeNotifyEntry ITEMIDLIST*pidl fRecursive
 dll shell32
	 #SHChangeNotifyRegister hwnd fSources fEvents wMsg cEntries SHChangeNotifyEntry*pfsne
	 #SHChangeNotifyDeregister ulID
dll shell32
	[2]#SHChangeNotifyRegister hwnd fSources fEvents wMsg"cEntries SHChangeNotifyEntry*pfsne
	[4]#SHChangeNotifyDeregister ulID

if(!ShowDialog("ST_Dialog" &ST_Dialog)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Shell triggers"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010400 "*" ""

ret
 messages
int-- rid
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	
	SHChangeNotifyEntry e.fRecursive=1
	e.pidl=0
	e.pidl=PidlFromStr("c:\windows\tasks")
	out e.pidl
	rid=SHChangeNotifyRegister(hDlg 0xFF -1 WM_USER+10 1 &e)
	ret 1
	
	case WM_DESTROY
	SHChangeNotifyDeregister rid
	DT_DeleteData(hDlg)
	
	case WM_COMMAND goto messages2
	case WM_USER+10
	out "0x%X 0x%X" wParam lParam
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
