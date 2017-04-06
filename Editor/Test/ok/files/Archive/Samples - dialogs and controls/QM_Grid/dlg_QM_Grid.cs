\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_QM_Grid" &dlg_QM_Grid 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 349 175 "QM_Grid"
 3 QM_Grid 0x54200001 0x200 0 0 350 154 ""
 1 Button 0x54030001 0x4 2 158 48 14 "OK"
 2 Button 0x54030000 0x4 52 158 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
DlgGrid g
sel message
	case WM_INITDIALOG
	g.Init(hDlg 3)
	 use first column as noneditable
	g.GridStyleSet(1)
	 add columns
	g.ColumnsAdd("read-only,80[]edit,50[]edit+button,80,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]read-only,70,7")
	 add rows
	lpstr si=
	 a1,b1,c1,d1,e1,yes,g1
	 a2,b2,c2,d2,e2,
	 ,,c3
	g.FromCsv(si ",")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
g.Init(hDlg 3)
sel wParam
	case IDOK
	 get and show all cells
	g.ToCsv(_s ",")
	ShowText "" _s
	
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
