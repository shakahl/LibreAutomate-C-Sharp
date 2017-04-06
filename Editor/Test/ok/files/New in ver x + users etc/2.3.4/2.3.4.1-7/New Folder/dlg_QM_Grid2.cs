 \Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid3 0)) ret

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
	 add columns
	LvAddCol g 0 "read-only" 80
	LvAddCol g 1 "edit" 50
	LvAddCol g 2 "edit+button" 80
	LvAddCol g 3 "edit multiline" 80
	LvAddCol g 4 "combo" 80
	LvAddCol g 5 "check" 50
	 set column cell control default types
	SendMessage g LVM_QG_SETCOLUMNTYPE 2 QG_EDIT|QG_BUTTONATRIGHT
	SendMessage g LVM_QG_SETCOLUMNTYPE 3 QG_EDIT|QG_EDIT_MULTILINE
	SendMessage g LVM_QG_SETCOLUMNTYPE 4 QG_COMBO
	SendMessage g LVM_QG_SETCOLUMNTYPE 5 QG_CHECK
	 populate ICsv variable
	ICsv c
	c=CreateCsv(1)
	lpstr si=
 a1ccccccccccccc,b1,c1,d1,e1,yes
 a2,b2,c234567890,"iiiiiiii
 iiiiiiii
 iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  jjjjjjjj  jjjjjjjj",e2,
 ,,c3
	rep(2) _s.addline(si)
	Q &q
	c.FromString(_s)
	Q &qq
	
	 Make sure that ncols is equal to the number of columns in the grid that you want to set (also depends on ToQmGrid flags).
	 If not, ToQmGrid will fail.
	 out "ncols=%i, nrows=%i" c.ColumnCount c.RowCount
	
	 populate grid control
	c.ToQmGrid(g 0)
	Q &qqq
	outq
	 31429 514163
	
	 c.FromString("")
	 c.ToQmGrid(g 0)
	
	 also can set cell control default types
	SendMessage g LVM_QG_SETCELLTYPE 2 MakeInt(1 QG_CHECK)
	
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
	 dll "qm.exe" #SendMessageQW hwnd message wParam lParam
	
	g=id(3 hDlg)
	Q &q
	rep(1000) LvGetItemText(g 1 2 _s)
	 rep(10) LvGetItemTextA(g 1 2 _s)
	Q &qq; outq
	out _s
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))

\Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid3 0)) ret

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
	 add columns
	LvAddCol g 0 "read-only" 80
	LvAddCol g 1 "edit" 50
	LvAddCol g 2 "edit+button" 80
	LvAddCol g 3 "edit multiline" 80
	LvAddCol g 4 "combo" 80
	LvAddCol g 5 "check" 50
	 set column cell control default types
	SendMessage g LVM_QG_SETCOLUMNTYPE 2 QG_EDIT|QG_BUTTONATRIGHT
	SendMessage g LVM_QG_SETCOLUMNTYPE 3 QG_EDIT|QG_EDIT_MULTILINE
	SendMessage g LVM_QG_SETCOLUMNTYPE 4 QG_COMBO
	SendMessage g LVM_QG_SETCOLUMNTYPE 5 QG_CHECK
	 populate ICsv variable
	ICsv c
	c=CreateCsv(1)
	lpstr si=
 a1ccccccccccccc,b1,c1,d1,e1,yes
 a2,b2,c234567890,"iiiiiiii
 iiiiiiii
 iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  iiiiiiii  jjjjjjjj  jjjjjjjj",e2,
 ,,c3
	rep(2) _s.addline(si)
	Q &q
	c.FromString(_s)
	Q &qq
	
	 Make sure that ncols is equal to the number of columns in the grid that you want to set (also depends on ToQmGrid flags).
	 If not, ToQmGrid will fail.
	 out "ncols=%i, nrows=%i" c.ColumnCount c.RowCount
	
	 populate grid control
	c.ToQmGrid(g 0)
	Q &qqq
	outq
	 31429 514163
	
	 c.FromString("")
	 c.ToQmGrid(g 0)
	
	 also can set cell control default types
	SendMessage g LVM_QG_SETCELLTYPE 2 MakeInt(1 QG_CHECK)
	
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
	 dll "qm.exe" #SendMessageQW hwnd message wParam lParam
	
	g=id(3 hDlg)
	Q &q
	rep(1000) LvGetItemText(g 1 2 _s)
	 rep(10) LvGetItemTextA(g 1 2 _s)
	Q &qq; outq
	out _s
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))

