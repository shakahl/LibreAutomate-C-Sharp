\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

type T21795 var1 str'var2 etc ;;add your variables to a type
T21795 _v ;;declare variable of the type

_v.var1=5

if(!ShowDialog("dlg_21795" &dlg_21795 0 0 0 0 0 &_v)) ret ;;pass address of the variable as 8-th argument

out _v.var1

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 12 12 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x203000A "" "" ""

ret
 messages
T21795& v=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	v.var1+1
	case IDOK
	case IDCANCEL
ret 1
