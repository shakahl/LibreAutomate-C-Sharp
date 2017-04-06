 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 msctls_progress32 0x54000000 0x4 8 10 136 12 ""
 4 Button 0x54032000 0x4 8 28 48 14 "Start"
 5 Static 0x54000000 0x0 160 10 48 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	
	case 4 ;;Start
	opt waitmsg 1
	int i; str s
	int hpb=id(3 hDlg)
	for i 0 100
		SendMessage hpb PBM_SETPOS i 0
		s.format("%i %%" i); s.setwintext(id(5 hDlg))
		0.02
	SendMessage hpb PBM_SETPOS 0 0
	s="Completed"; s.setwintext(id(5 hDlg))
	
	 Default range is 100. To change, SendMessage hpb PBM_SETRANGE 0 max<<16|min
	 PBM_DELTAPOS can be used insted of PBM_SETPOS to set increment instead of absolute value.

ret 1
