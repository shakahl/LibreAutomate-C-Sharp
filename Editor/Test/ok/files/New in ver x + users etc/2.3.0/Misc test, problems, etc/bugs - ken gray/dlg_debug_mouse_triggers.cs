\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3=
 1 - disable hittest
 2 - disable mouse move mutex
 4 - disable mouse info
 8 - disable mouse triggers
 
if(!ShowDialog("dlg_debug_mouse_triggers" &dlg_debug_mouse_triggers &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "debug qm mouse hook"
 3 ListBox 0x54230109 0x200 4 4 216 128 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
int& d
int i n hlb
sel message
	case WM_INITDIALOG
	&d=share+1000
	hlb=id(3 hDlg)
	for(i 0 LB_GetCount(hlb)) if(d&(1<<i)) LB_SelectItem(hlb i 1)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3
	&d=share+1000
	d=0
	hlb=lParam
	n=SendMessage(hlb LB_GETSELCOUNT 0 0); if(!n) ret
	ARRAY(int) a.create(n); SendMessage(hlb LB_GETSELITEMS n &a[0])
	for(i 0 n) d|1<<a[i]
	
	case IDOK
	case IDCANCEL
ret 1
