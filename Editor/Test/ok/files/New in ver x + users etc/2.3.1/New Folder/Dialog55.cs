\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile "__crty"

 Deb 100
if(!ShowDialog("Dialog55" &Dialog55)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 12 18 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x203000E "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 atend Function90 1
	str- s="test"
	crty-- k
	case WM_DESTROY
	out "destr"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	 opt end 1
	 end
	out s
	out mac("Menu24")
	 out _error.description
	 act "ffff"; err
	case IDOK
	case IDCANCEL
ret 1
