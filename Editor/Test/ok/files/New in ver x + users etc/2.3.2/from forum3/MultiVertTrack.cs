\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("MultiVertTrack" &MultiVertTrack)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 268 402 "Reverb Control"
 3 msctls_trackbar32 0x54030003 0x0 3 3 24 399 ""
 4 msctls_trackbar32 0x54030003 0x0 30 3 24 399 ""
 5 msctls_trackbar32 0x54030003 0x0 57 3 24 399 ""
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "" "0" ""

ret
 messages
 int npages=60 ;;change this
sel message
	case WM_INITDIALOG
	SendMessage id(3 hDlg) TBM_SETRANGE 0 60<<16
	SendMessage id(4 hDlg) TBM_SETRANGE 0 60<<16
	SendMessage id(5 hDlg) TBM_SETRANGE 0 60<<16
	 SendMessage(id(3 hDlg) TBM_SETPOS 1 59);;example of loading position
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_VSCROLL
	int i3=SendMessage(id(3 hDlg) TBM_GETPOS 0 0)
	int i4=SendMessage(id(4 hDlg) TBM_GETPOS 0 0)
	int i5=SendMessage(id(5 hDlg) TBM_GETPOS 0 0)
	out
	out "i3 is: %i" i3
	out "i4 is: %i" i4
	out "i5 is: %i" i5
	out GetWinId(lParam)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1