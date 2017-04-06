\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid8 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 291 175 "QM_Grid"
 3 QM_Grid 0x54200001 0x200 0 0 294 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int g
	g=id(3 hDlg)
	 use first column as noneditable
	SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN -1
	 SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN|QG_AUTONUMBER -1
	 add columns
	 lpstr columns="read-only,80[]edit,50[]edit+button,80,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]"
	lpstr columns="read-only,20%[]edit,15%[]edit+button,30%,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]"
	SendMessage g LVM_QG_ADDCOLUMNS 0 columns
	 populate ICsv variable
	ICsv c
	c=CreateCsv(1)
	lpstr si=
 a1ccccccccccccc,b1,c1,d1,e1,yes
 a2,b2,c234567890,"iiiiiiii
 iiiiiiii
 iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  jjjjjjjj  jjjjjjjj",e2,
 ,,c3
	rep(1) _s.addline(si)
	c.FromString(_s)
	
	 populate grid control
	Q &q
	c.ToQmGrid(g 0)
	Q &qq; outq
	 rep(1000) (3000 items) - 4 ms.
	
	 out SendMessage(g LVM_QG_GETINFO GRID.QG_GETINFO_STYLE 0)
	 out SendMessage(g LVM_QG_GETINFO GRID.QG_GETINFO_COLUMNCOUNT 0)
	 out SendMessage(g LVM_QG_GETINFO GRID.QG_GETINFO_ROWCOUNT 0)
	 SendMessage(g LVM_QG_SETCELLTYPE 1 MakeInt(0 2))
	 for(_i 0 7) out "%i %i" SendMessage(g LVM_QG_GETINFO GRID.QG_GETINFO_COLUMNTYPE _i) SendMessage(g LVM_QG_GETINFO MakeInt(GRID.QG_GETINFO_CELLTYPE _i) 1)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
g=id(3 hDlg)
sel wParam
	case IDOK
	 get and show all cells
	Q &q
	c=CreateCsv(1)
	c.FromQmGrid(g)
	Q &qq; outq
	c.ToString(_s); ShowText "" _s
	
	case 4
	
	 SendMessage(g LVM_DELETEALLITEMS 0 0)
	 for(_i 5 -1 -1) out SendMessage(g LVM_DELETECOLUMN _i 0)
	  for(_i 0 6) out SendMessage(g LVM_DELETECOLUMN 0 0)
	 SendMessage g LVM_QG_ADDCOLUMNS 0 "a[]b"
	
	Q &q
	rep() if(!SendMessage(g LVM_DELETEITEM 0 0)) break
	 SendMessage(g LVM_DELETEALLITEMS 0 0)
	 int it
	 hid g
	 for it 0 1
		 TO_LvAdd g 0 0 0 "s1" "sub"
	 hid- g
	Q &qq; outq
	
ret 1
 messages3
NMHDR* nh=+lParam
 sel(nh.code) case [LVN_GETDISPINFOW,NM_CUSTOMDRAW,LVN_HOTTRACK,LVN_KEYDOWN] case else OutWinMsg message wParam lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
