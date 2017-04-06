\Dialog_Editor

function# hDlg message wParam lParam
if(hDlg) goto messages

InitSplitter

str controls = "4 5"
str e4 e5
if(!ShowDialog("dlg_ButtonsOnSplitter" &dlg_ButtonsOnSplitter &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 216 249 "Dialog"
 4 Edit 0x54231044 0x200 0 0 216 80 ""
 5 Edit 0x54230844 0x20000 0 94 216 114 ""
 1 Button 0x54030001 0x4 60 224 48 14 "OK"
 2 Button 0x54030000 0x4 124 224 48 14 "Cancel"
 3 Button 0x54032000 0x0 26 80 26 14 "▼"
 6 Button 0x54032000 0x0 0 80 26 14 "▲"
 7 QM_Splitter 0x54000000 0x0 0 80 216 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "*" "" "" ""

ret
 messages
DlgSplitter ds.Init(hDlg 7)
sel message
	case WM_INITDIALOG
	int hSplitter(id(7 hDlg)) hb6(id(6 hDlg)) hb3(id(3 hDlg))
	GetWinXY hb6 0 0 _i
	SetParent hb6 hSplitter; mov 0 0 hb6
	SetParent hb3 hSplitter; mov _i 0 hb3
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 3 ds.SetPos(ds.GetMaxPos)
	case 6 ds.SetPos(0)
ret 1