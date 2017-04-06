\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid_readonly 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 399 175 "QM_Grid"
 3 QM_Grid 0x54210049 0x200 0 0 400 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
int g i; ICsv c
sel message
	case WM_INITDIALOG
	g=id(3 hDlg)
	SendMessage g LVM_QG_SETSTYLE QG_NOAUTOADD -1
	 add columns
	lpstr columns="one,,7[]two,,7[]three,,7"
	SendMessage g LVM_QG_ADDCOLUMNS 0 columns
	 populate ICsv variable
	c=CreateCsv(1)
	lpstr si=
	 a1ccccccccccccc,b1,c1,d1,e1,yes,aaa
	 a2,b2,c234567890,"iiiiiiii",,c3
	rep(5) _s.addline(si)
	c.FromString(_s)
	
	 populate grid control
	c.ToQmGrid(g 0)
	
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 get and show all cells
	c=CreateCsv(1)
	c.FromQmGrid(id(3 hDlg))
	c.ToString(_s); ShowText "" _s
	
	case 4
	g=id(3 hDlg)
	
	
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
