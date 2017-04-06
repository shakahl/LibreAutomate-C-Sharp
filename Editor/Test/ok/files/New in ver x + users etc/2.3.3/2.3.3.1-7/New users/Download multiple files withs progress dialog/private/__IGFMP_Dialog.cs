 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 154 "Downloading"
 3 ListBox 0x54230101 0x200 0 0 224 96 ""
 8 Static 0x54000000 0x0 2 100 52 13 ""
 4 msctls_progress32 0x54030000 0x0 56 100 164 13 ""
 5 Static 0x44000000 0x0 2 116 52 13 ""
 6 msctls_progress32 0x44030000 0x0 56 116 164 13 ""
 2 Button 0x54030000 0x4 172 136 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "IntGetFileMultiProgress" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	PostMessage hDlg WM_APP 0 0
	
	case WM_APP
	opt waitmsg 1
	wait 0 H mac("__IGFMP_Thread" "" hDlg)
	clo hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
