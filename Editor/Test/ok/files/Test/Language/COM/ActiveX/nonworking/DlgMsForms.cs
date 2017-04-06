\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib MSForms {AC2DE821-36A2-11CF-8053-00AA006009FA} 2.0

if(!ShowDialog("DlgMsForms" &DlgMsForms 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x4 0 0 224 114 "MSForms.UserForm"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 MSForms.UserForm f
	 f._getcontrol(id(3 hDlg))
	 VARIANT v; v.lVal=0xffff; v.vt=VT_BOOL
	 f.Controls.Add("ARGradientControl.ArGradient" "name" v) ;;error
	
	ret 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
