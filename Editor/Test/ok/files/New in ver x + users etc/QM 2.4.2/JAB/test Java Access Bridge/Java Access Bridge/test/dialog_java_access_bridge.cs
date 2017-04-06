\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

JabInit

if(!ShowDialog("dialog_java_access_bridge" &dialog_java_access_bridge)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 8 68 16 "Java windows"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 goto gRefresh
ret 1
 gRefresh
ARRAY(int) a
win "" "" "" 0 "" a
int i w
for i 0 a.len
	w=a[i]
	if(!JAB.IsJavaWindow(w)) continue
	outw w
