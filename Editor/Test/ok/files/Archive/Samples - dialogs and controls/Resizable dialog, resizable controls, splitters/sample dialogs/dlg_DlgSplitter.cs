\Dialog_Editor

 DlgSplitter example.
 Shows how to get/set/save/restore splitter position, etc.


function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3 4 5"
str lb3 e4 e5
if(!ShowDialog("" &dlg_DlgSplitter &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 225 214 "Dialog"
 3 ListBox 0x54230101 0x200 18 4 78 102 ""
 4 Edit 0x54231044 0x200 100 4 106 66 ""
 5 Edit 0x54230844 0x20000 100 74 118 38 ""
 6 QM_Splitter 0x54030000 0x0 96 0 4 114 ""
 7 QM_Splitter 0x54030000 0x0 100 70 120 4 ""
 8 QM_Splitter 0x54030000 0x0 2 122 34 4 ""
 9 Button 0x54032000 0x0 2 132 48 14 "Disable"
 10 Button 0x54032000 0x0 52 132 48 14 "Enable"
 11 Button 0x54032000 0x0 2 150 48 14 "Detach"
 12 Button 0x54032000 0x0 52 150 48 14 "Attach"
 13 Button 0x54032000 0x0 2 168 48 14 "Getbounds"
 14 Button 0x54032000 0x0 52 186 48 14 "Getmaxpos"
 15 Button 0x54032000 0x0 2 186 48 14 "Getpos"
 16 Button 0x54032000 0x0 120 186 48 14 "mov"
 17 Button 0x54032000 0x0 120 132 64 14 "move to min"
 18 Button 0x54032000 0x0 120 150 64 14 "move to max"
 19 Button 0x54032000 0x0 120 168 64 14 "move to middle"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
DT_AutoSizeControls hDlg message "3sv 4sh 5s 6sv 7sh"
DlgSplitter ds.Init(hDlg 6)
sel message
	case WM_INITDIALOG
	RegWinPos hDlg "winpos" "\test\dlg_DlgSplitter" 0 ;;apply saved dialog pos
	if(rget(_i "splitter" "\test\dlg_DlgSplitter")) ds.SetPos(_i) ;;apply saved splitter pos
	
	case WM_DESTROY
	RegWinPos hDlg "winpos" "\test\dlg_DlgSplitter" 1 ;;save dialog pos
	rset ds.GetPos "splitter" "\test\dlg_DlgSplitter" ;;save splitter pos
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 9 ds.EnableSplitter(0)
	case 10 ds.EnableSplitter(1)
	case 11 ds.AttachControls(0)
	case 12 ds.AttachControls(1)
	case 13 int _min _max; ds.GetBounds(_min _max); out F"{_min} {_max}"
	case 14 out ds.GetMaxPos
	case 15 int pos maxpos; pos=ds.GetPos(maxpos); out F"{pos}/{maxpos}"
	
	case 17 ds.SetPos(0)
	case 18 ds.SetPos(100000)
	case 19 ds.SetPos(ds.GetMaxPos/2)
	
	case 16
	SetWindowPos ds 0 100 0 0 0 SWP_NOSIZE|SWP_NOZORDER
	 SetWindowPos ds 0 0 0 0 0 SWP_NOSIZE|SWP_NOZORDER
	 SetWindowPos ds 0 100000 0 0 0 SWP_NOSIZE|SWP_NOZORDER
	 SetWindowPos ds 0 100 0 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOSENDCHANGING ;;will not resize controls

ret 1
