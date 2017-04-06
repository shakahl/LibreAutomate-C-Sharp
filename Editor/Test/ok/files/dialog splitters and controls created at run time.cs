\Dialog_Editor

 Resizable dialog with 2 splitters.
 Controls are created at run time.

str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 225 133 "Dialog"
 100 Button 0x54032000 0x0 8 116 72 14 "Create controls"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040500 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 100 ;;Create controls
	sub.CreateControls hDlg
ret 1


#sub CreateControls
function hDlg

 3 ListBox 0x54230101 0x200 0 0 96 110 ""
 4 Edit 0x54231044 0x200 100 0 126 70 ""
 5 Edit 0x54230844 0x20000 100 74 126 36 ""
 6 QM_Splitter 0x54000000 0x0 96 0 4 110 ""
 7 QM_Splitter 0x54000000 0x0 100 70 126 4 ""

RECT r; GetClientRect hDlg &r
int wid(r.right) hei(r.bottom*0.8) splitWid(6)
int c3=CreateControl(0 "ListBox" 0 0x54230101 0 0 wid/2-splitWid hei hDlg 3)
int c4=CreateControl(0 "Edit" 0 0x54231044 wid/2 0 wid/2 hei/2-splitWid hDlg 4)
int c5=CreateControl(0x20000 "Edit" 0 0x54230844 wid/2 hei/2 wid/2 hei/2 hDlg 5)
int c6=CreateControl(0 "QM_Splitter" 0 0x54000000 wid/2-splitWid 0 splitWid hei hDlg 6)
int c7=CreateControl(0 "QM_Splitter" 0 0x54000000 wid/2 hei/2-splitWid wid/2 splitWid hDlg 7)

DT_SetAutoSizeControls hDlg "3sv 4sh 5s 6sv 7sh 1m 2m 100mv"
