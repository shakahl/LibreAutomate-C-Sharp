\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_recent_menu_sample" &dlg_recent_menu_sample 0 0 0 0 0 0 0 0 "" "dlg_recent_menu_sample")) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
#compile __CRecentMenu
CRecentMenu-- t_rm

sel message
	case WM_INITDIALOG
	t_rm.Init(hDlg 1000 4 "\test\test")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_INITMENUPOPUP
	if(t_rm.OnWmInitmenupopup(wParam))
		AppendMenuW t_rm.hsubmenu MF_SEPARATOR 0 0
		AppendMenuW t_rm.hsubmenu 0 99 @"Clear"
ret
 messages2
str sf
sel wParam
	case 101 ;;Open
	if(!OpenSaveDialog(0 sf)) ret
	 g1
	t_rm.AddFile(sf)
	
	case [1000,1001,1002,1003]
	if(!t_rm.GetSelectedFile(wParam sf)) ret
	out sf
	goto g1
	
	case 99 ;;Clear
	t_rm.RemoveAll
	
	case 901
	ARRAY(str) a
	t_rm.GetFiles(a)
	out a
	
	case IDOK
	case IDCANCEL
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0 0 Co
	 -
	 &Recent : 1000
	 -
	 E&xit : 2 0 0 AF4
	 <
 >&Edit
	 Empty : 201 0 1
	 <
 >&View
	 Empty : 301 0 1
	 <
 &Help : 401 0
 -
 >&Test
	 GetFiles : 901
	 <
 END MENU
