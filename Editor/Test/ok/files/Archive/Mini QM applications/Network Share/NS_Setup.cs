\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Run this function (click the Run button) to set computer names and passwords.
 It is required before you can use NS_SendFile or NS_Copy.
 When you add a computer, creates shortcut in the Send To folder.
 Then you can right-click a file or folder in the Windows Explorer and use the Send To menu to send it to the computer.


_i=win("QM Network Share Setup"); if(_i) act _i; ret

str c p

str controls = "3 5 7"
str lb3 e5 e7
if(!ShowDialog("NS_Setup" &NS_Setup &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 158 119 "QM Network Share Setup"
 3 ListBox 0x54230101 0x200 6 44 92 70 ""
 2 Button 0x54030000 0x4 102 100 52 14 "Close"
 4 Static 0x54000000 0x0 6 8 48 12 "Computer"
 5 Edit 0x54030080 0x200 58 8 96 14 ""
 6 Static 0x54000000 0x0 6 24 48 12 "Password"
 7 Edit 0x54030080 0x200 58 24 96 14 ""
 8 Button 0x54032000 0x0 102 46 52 14 "Add/Change"
 9 Button 0x54032000 0x0 102 62 52 14 "Remove"
 10 Button 0x54032000 0x0 102 78 52 14 "Open SendTo"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	NS_FillListBox id(3 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3
	LB_GetItemText(lParam LB_SelectedItem(lParam) c)
	c.setwintext(id(5 hDlg))
	
	case 8 ;;Add
	c.getwintext(id(5 hDlg)) p.getwintext(id(7 hDlg))
	if(!c.len) ret
	rset p c "\NetShare"
	NS_FillListBox id(3 hDlg)
	
	SHORTCUTINFO si.target="qmcl.exe"
	si.param.format("M ''NS_SendFiles'' A %s" c) ;;file will be appended
	si.iconfile="explorer.exe"
	CreateShortcutEx _s.format("$SendTo$\Computer %s.lnk" c) &si
	
	case 9 ;;Remove
	int h=id(3 hDlg)
	LB_GetItemText(h LB_SelectedItem(h) c)
	rset "" c "\NetShare" 0 -1
	NS_FillListBox id(3 hDlg)
	
	del- _s.format("$SendTo$\Computer %s.lnk" c); err
	
	case 10 ;;Open SendTo
	run "$sendto$"
	
	case IDOK
	case IDCANCEL
ret 1
