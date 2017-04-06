\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 3"
str e4 qmg3x
qmg3x=
 1999.8.7,,4:5

if(!ShowDialog("dlg_Grid_datetime" &dlg_Grid_datetime &controls _hwndqm)) ret
out qmg3x

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 453 135 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 454 82 "0x0,0,0,2[]C,,3,[]D,,19,[]E,,11,[]F,,27,[]B,,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Edit 0x54030080 0x200 240 116 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
 __Font-- f.Create("Arial" 12)
sel message
	case WM_INITDIALOG
	 f.SetDialogFont(hDlg)
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3)
	 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
	ret DT_Ret(hDlg gridNotify(nh))
