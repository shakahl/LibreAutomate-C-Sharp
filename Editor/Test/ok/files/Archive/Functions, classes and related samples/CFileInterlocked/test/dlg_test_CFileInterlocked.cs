\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Run this 2 times. Click Open on each dialog. It opens and locks the file for 5 s.
 To test on network, change fn to a shared file path and run this on 2 computers.


#compile __CFileInterlocked

if(!ShowDialog("" &dlg_test_CFileInterlocked 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 10 14 48 14 "Open"
 4 Static 0x54000000 0x0 10 36 48 13 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	EnableWindow lParam 0
	str fn="$desktop$\test dlg_test_CFileInterlocked.txt"
	 str fn="\\COMPUTER\shared\test dlg_test_CFileInterlocked.txt" //test on network
	opt waitmsg 1
	CFileInterlocked f
	_s="opening..."; _s.setwintext(id(4 hDlg))
	f.Open(fn)
	_s="OPEN"; _s.setwintext(id(4 hDlg))
	5
	f.Close
	_s="closed"; _s.setwintext(id(4 hDlg))
	EnableWindow lParam 1
	case IDOK
	case IDCANCEL
ret 1
