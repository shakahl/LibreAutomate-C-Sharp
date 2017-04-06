\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_QM_Grid9 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 439 175 "QM_Grid"
 3 QM_Grid 0x54210049 0x200 0 0 440 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	g.GridStyleSet(GRID.QG_NOEDITFIRSTCOLUMN|GRID.QG_NOAUTOADD|GRID.QG_SETROWTYPE)
	 g.GridStyleSet(LVS_EX_GRIDLINES 2|4)
	 g.GridStyleSet(LVS_EX_FULLROWSELECT 0|4)
	 g.GridStyleSet(LVS_EX_TRACKSELECT 1|4)
	g.ColumnsAdd("noedit,10%[]edit,15%[]edit+button,20%,16[]edit multiline,20%,8[]combo,15%,1[]check,10%,2[]read-only,10%,7" 1)
	 g.ColumnTypeSet(1 1)
	lpstr data=
	 a1,b1,c1,d1,e1,yes,g1
	 a2,b2,c2,"line1
	 line2","1,2",,g2
	
	int i j; str s1 s2
	for(i 0 10)
		for(j 0 7) s1+s2.RandomString(4 4 "0-9a-z"); s1+", "
		s1.replace("[]" s1.len-2)
	g.FromCsv(s1 ",")
	 out "%i %i" g.ColumnsCountGet g.RowsCountGet
	
	 for(i 0 1)
		 Q &q
		 TO_LvAdd(g i 0 0 "tttt")
		 Q &qq
		 outq
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 get and show all cells
	g.ToCsv(_s ",")
	ShowText "" _s
	
	case 4
	 out g.IsChanged(1)
	
	 out g.RowAddSet(1 0 "one[0]two[0]three" 3 1 1)
	 str ss1("one") ss2("two"); out g.RowAddSet(1 2 &ss1 2 2 0)
	 out g.RowAddSetMS(-1 "one[0]two[0]three" 3)
	 out g.RowAddSetMS(1 "one[0]two[0]three" 3 0 1)
	 out g.RowAddSetMS(21 "one[0]two[0]three" 3 2)
	 ARRAY(str) a="one[]two[]three"
	 out g.RowAddSetSA(-1 &a[0] 3)
	
	 out g.RowGetMS(0 3 1 0 &_i)
	 out _i
	
	 ARRAY(str) a
	 out g.RowGetSA(a 0 3 1 2)
	 for(_i 0 a.len) out a[_i]
	
	 out g.CellSet(-1 3 "ttttttttttt")
	 out g.CellGet(1 3)
	
	 out g.RowsSelectedGet
	 ARRAY(int) a
	 out g.RowsSelectedGet(a)
	 for(_i 0 a.len) out a[_i]
	
	 g.RowSelect(4)
	 g.RowSelect(4 1)
	 g.RowSelect(24 2)
	 g.RowSelect(-1)
	
	 g.RowsDeleteAll(0)
	
	 out g.GridStyleGet
	 out g.ColumnTypeGet(2)
	 out g.CellTypeGet(2 3)
	 g.CellTypeSet(2 4 2)
	 g.RowTypeSet(5 7)
	
	 Q &q
	  g.RowsDeleteAll(0)
	 g.FromCsv("" ",")
	 Q &qq
	 outq
	
	 Q &q
	 TO_LvAdd(g -1 0 0 "tttt")
	  out g.Send(LVM_INSERTITEMW 0 0)
	 Q &qq
	 outq
	
	 Q &q
	 g.ToCsv(_s "," 4)
	 Q &qq
	 outq
	 ShowText "" _s
	
	 TO_LvAdd g 10 0 0 "new"
	
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3)
	 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
	ret DT_Ret(hDlg gridNotify(nh))
