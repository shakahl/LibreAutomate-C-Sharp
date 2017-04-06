\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_tb" &dialog_tb)) ret

 BEGIN DIALOG
 0 "" 0x90CF0A44 0x100 0 0 409 284 "Dialog"
 1 Button 0x54030001 0x4 4 272 48 14 "OK"
 2 Button 0x54030000 0x4 56 272 48 14 "Cancel"
 3 ToolbarWindow32 0x54030001 0x0 0 0 415 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ARRAY(int) aw ai
	str s; int i
	GetWindowList s 0 17 0 0 aw ;;note: currently GetWindowList has bug: flag 16 does not work for names
	 out numlines(s)
	 out aw.len
	 out s
	ai.create(aw.len)
	for(i 0 aw.len) ai[i]=GetWindowIcon(aw[i])
	DT_TbAddButtons id(3 hDlg) 100 s 0 TBSTYLE_LIST|TBSTYLE_WRAPABLE 0 0 ai
	 SetWindowPos id(3 hDlg) 0 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOZORDER|SWP_FRAMECHANGED
	 SendMessage id(3 hDlg) WINAPI.TB_AUTOSIZE 0 0
	for(i 0 aw.len) DestroyIcon(ai[i])
	PostMessage hDlg WM_SIZE 0 0
	
	case WM_DESTROY
	DT_TbOnDestroy id(3 hDlg)
	case WM_SIZE
	SendMessage id(3 hDlg) WINAPI.TB_AUTOSIZE 0 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
