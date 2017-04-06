\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str qmg4x
qmg4x="test\file_1MB.txt"
if(!ShowDialog("dlg_download_progress_grid" &dlg_download_progress_grid &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 2 4 48 14 "Download"
 4 QM_Grid 0x56031041 0x0 4 46 216 66 "0x0,0,0,0,0x0[]File,,,[]Status,,7,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	 better if this code was in other thread, but for simplicity just add flag 32
	Http h.Connect("www.quickmacros.com")
	h.SetProgressCallback(&DDP_progress_grid hDlg)
	h.Get("test\file_1MB.txt" _s 32)
	
	case IDOK
	case IDCANCEL
ret 1
