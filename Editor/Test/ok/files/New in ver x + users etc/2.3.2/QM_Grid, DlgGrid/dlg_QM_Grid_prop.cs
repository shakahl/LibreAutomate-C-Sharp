\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str qmg3
qmg3=
 a,b
 c,d
 e,f
 g,h
 i,j
 k,l
if(!ShowDialog("dlg_QM_Grid_prop" &dlg_QM_Grid_prop &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 307 135 "Dialog"
 1 Button 0x54030001 0x4 204 118 48 14 "OK"
 2 Button 0x54030000 0x4 256 118 48 14 "Cancel"
 3 QM_Grid 0x56031049 0x0 0 0 308 112 "7,0,0,2[]A,,[]B,,[]"
 4 Button 0x54032000 0x0 4 116 36 14 "Prop set"
 8 Button 0x54032000 0x0 42 116 38 14 "Prop get"
 5 Button 0x54032000 0x0 88 116 32 14 "Sel set"
 6 Button 0x54032000 0x0 122 116 32 14 "Sel rem"
 7 Button 0x54032000 0x0 156 116 32 14 "Sel get"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
__ImageList-- il
DlgGrid g.Init(hDlg 3)
int i
sel message
	case WM_INITDIALOG
	il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	g.SetImagelist(il il)
	 g.SetImagelist(il)
	 g.SetImagelist(il 1)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
SetFocus g
sel wParam
	case 4
	for i 0 g.RowsCountGet
		g.RowPropSet(i 31 i+10 i+2 i&1+1 i+5 i) ;;all props
		 g.RowPropSet(i 2 i+10 i+2 i&1+1 i+5 i) ;;single prop
		 g.RowPropSet(i 31 i+10 i+2 i&1+1 i&1+1 i) ;;checkboxes
	
	case 8
	int param image ovImage stImage indent
	for i 0 g.RowsCountGet
		g.RowPropGet(i param image ovImage stImage indent) ;;all props
		 g.RowPropGet(i 0 0 0 0 indent) ;;single prop
		out "%i %i %i %i %i" param image ovImage stImage indent
	
	case 5
	g.RowSelect(2)
	g.RowSelect(3 1)
	g.RowSelect(1 4) ;;cut
	g.RowSelect(4 4|1) ;;cut
	
	case 6
	g.RowSelect(-1)
	g.RowSelect(-1 4) ;;cut
	
	case 7
	ARRAY(int) a
	out g.RowsSelectedGet()
	out g.RowsSelectedGet(a)
	out "---"
	for(i 0 a.len) out a[i]
	out "---"
	for(i 0 g.RowsCountGet) out g.RowStateGet(i)
	out "---"
	
	int i1(g.RowSelectedGet) i2(g.RowSelectedGet(i1))
	out "%i %i" i1 i2
	
	case IDOK
	case IDCANCEL
ret 1
