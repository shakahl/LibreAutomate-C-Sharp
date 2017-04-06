\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(!ShowDialog("Dialog2" &Dialog2)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 8 80 14 "GWL_USERDATA"
 4 Button 0x54032000 0x0 8 26 80 14 "SetProp"
 5 Button 0x54032000 0x0 8 44 80 14 "SetProp atom"
 6 Button 0x54032000 0x0 8 62 80 14 "SetProp atom int"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
int i
sel wParam
	case IDOK
	case 3
	Q &q
	for(i 0 1000) GetWindowLong(hDlg GWL_USERDATA)
	Q &qq
	outq
	
	case 4
	SetProp(hDlg "ggtreehhjk" 1)
	Q &q
	for(i 0 1000) GetProp(hDlg "ggtreehhjk")
	Q &qq
	outq
	RemoveProp(hDlg "ggtreehhjk")
	
	case 5
	int atom=GlobalAddAtom("qm_test")
	 int atom=0x1155
	 int atom=GlobalAddAtom+0x1155)
	 int atom=0xff00
	 out GlobalFindAtom(+atom)
	 out atom
	SetProp(hDlg +atom 5)
	_i=GetProp(hDlg +atom)
	 out _i
	 _i=GetProp(hDlg "qm_test")
	 out _i
	
	Q &q
	for(i 0 1000) GetProp(hDlg +atom)
	Q &qq
	outq
	
	RemoveProp(hDlg +atom)
	SetLastError 0
	rep(1) GlobalDeleteAtom(atom)
	out GetLastError
	out GetProp(hDlg +atom)
	
	case 6
	atom=GlobalAddAtom(+0xA4A1)
	out atom
	SetProp(hDlg +atom 5)
	_i=GetProp(hDlg +atom)
	out _i
	
	Q &q
	for(i 0 1000) GetProp(hDlg +atom)
	Q &qq
	outq
	
	 RemoveProp(hDlg +atom)
	 GlobalDeleteAtom atom
	
	case IDCANCEL
ret 1
