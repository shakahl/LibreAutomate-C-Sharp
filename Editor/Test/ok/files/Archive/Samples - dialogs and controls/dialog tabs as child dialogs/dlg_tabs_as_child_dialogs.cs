\Dialog_Editor

 This is an older example how to add child dialogs to a dialog.
 Maybe better would be to use ChildDialog class.
 This is also a tab control example.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_tabs_as_child_dialogs)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x10100 0 0 221 159 "Dialog"
 1 Button 0x54030001 0x4 120 140 48 14 "OK"
 2 Button 0x54030000 0x4 170 140 48 14 "Cancel"
 3 SysTabControl32 0x54010040 0x0 0 0 222 134 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
ARRAY(int)-- tabs.create(3)
sel message
	case WM_INITDIALOG
	 add tabs to the tab control
	str st="A[]B[]C" ;;tab labels
	int i htb=id(3 hDlg); TCITEM ti.mask=TCIF_TEXT; RECT r rr
	foreach(ti.pszText st) SendMessage htb TCM_INSERTITEMA i &ti; i+1
	 calculate y of child dialogs
	GetWindowRect htb &rr; ScreenToClient hDlg +&rr
	SendMessage htb TCM_GETITEMRECT 0 &r
	i=rr.top+r.bottom+2
	 create and initialize child dialogs
	tabs[0]=DTC_Tab0(0 0 hDlg i)
	tabs[1]=DTC_Tab1(0 0 hDlg i)
	tabs[2]=DTC_Tab2(0 0 hDlg i)
	
	_i=0; goto g11 ;;select first tab
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 get data of child dialogs
	DTC_Tab0(0 1 hDlg tabs[0])
	DTC_Tab1(0 1 hDlg tabs[1])
	DTC_Tab2(0 1 hDlg tabs[2])
	
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case TCN_SELCHANGE
	_i=SendMessage(id(3 hDlg) TCM_GETCURSEL 0 0)
	 g11
	for(i 0 tabs.len) hid tabs[i]
	hid- tabs[_i]
