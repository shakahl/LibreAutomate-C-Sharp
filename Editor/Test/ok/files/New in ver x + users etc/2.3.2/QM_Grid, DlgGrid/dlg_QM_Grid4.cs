\Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid4 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 291 175 "QM_Grid"
 3 QM_Grid 0x54210001 0x200 0 0 294 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""
 3 SysListView32 0x54200001 0x200 0 0 294 154 ""

ret
 messages
sel message
	case WM_INITDIALOG
	int g
	g=id(3 hDlg)
	 use first column as noneditable
	 SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN -1
	SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN -1
	 SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN|8 -1
	 add columns
	LvAddCol g 0 "read-only" 80
	LvAddCol g 1 "edit" 50
	LvAddCol g 2 "edit+button" 80
	 set column cell control default types
	SendMessage g LVM_QG_SETCOLUMNTYPE 2 QG_EDIT|QG_BUTTONATRIGHT
	 populate ICsv variable
	ICsv c
	c=CreateCsv(1)
	lpstr si1=
	 a1
	 b1
	 
	 c1
	c.FromString(si1)
	
	 populate grid control
	c.ToQmGrid(g 1)
	
	lpstr si2=
	 i,j
	 k,l
	 
	 m,n
	c.FromString(si2)
	c.ToQmGrid(g 2)
	
	 c.FromString("")
	 c.ToQmGrid(g 2)
	
	 si=
	  y,yy
	  z,zz
	 c.FromString(si)
	 c.ToQmGrid(g 2)
	
	 si=
	  y
	  z
	 c.FromString(si)
	 c.ToQmGrid(g 1)
	 
	 si=
	  y,yy
	  z,zz
	 c.FromString(si)
	 c.ToQmGrid(g 2)
	
	 str d; int i
	 for(i 0 6) _i=i+65; d.fromn(d d.len &_i 2)
	 out SendMessage(g LVM_QG_SETALLCELLS 0 &d)
	  out SendMessage(g LVM_QG_SETALLCELLS 1 &d)
	  for(i 0 6) _i=i+65; d.fromn(d d.len &_i 2)
	  out SendMessage(g LVM_QG_SETALLCELLS 2 &d)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
g=id(3 hDlg)
sel wParam
	case 4
	 out SendMessage(g LVM_QG_GETINFO 4 1)
	
	 str v
	 out SendMessage(g LVM_QG_GETALLCELLS 3 &v)
	 outb v v.len 1
	 ret
	
	 LVCOLUMN lc.mask=LVCF_SUBITEM
	 lc.iSubItem=2
	 LVCOLUMN lc.mask=LVCF_ORDER
	 lc.iOrder=2
	 out SendMessage(g LVM_SETCOLUMN 1 &lc)
	
	 LVITEM li
	 li.pszText="new"
	 li.iItem=0
	 out SendMessage(g LVM_SETITEMTEXT 0 &li)
	  li.mask=LVIF_TEXT; out SendMessage(g LVM_SETITEM 0 &li)
	
	 LVITEMW li
	 li.pszText=@"ąčęüöä"
	 li.iItem=0
	 out SendMessage(g LVM_SETITEMTEXTW 0 &li)
	  li.mask=LVIF_TEXT; out SendMessage(g LVM_SETITEMW 0 &li)
	
	 int- tc; tc+1
	  out TO_LvAdd(g -1 0 0 tc tc)
	 out TO_LvAdd(g 2 0 0 tc tc)
	 
	 ret
	 SendMessage(g LVM_DELETEALLITEMS 0 0)
	 LvAddCol g 1 "new" 50
	  SendMessage g LVM_DELETECOLUMN 1 0
	  out SendMessage(g LVM_QG_SETCOLUMNTYPE 1 QG_EDIT|QG_BUTTONATRIGHT)
	  out SendMessage(g LVM_SETITEMCOUNT 0 0)
	  ret
	 c=CreateCsv(1)
	 rep(10) _s.addline("one,two,three,four[]five")
	 c.FromString(_s)
	 c.ToQmGrid(g 0); err out "failed"
	
	 str v
	 out SendMessage(g LVM_QG_SETALLCELLS 2 &v)
	 out SendMessage(g LVM_QG_SETALLCELLS 2 0)
	
	case IDOK
	 get and show all cells
	c=CreateCsv(1)
	c.FromQmGrid(g)
	 c.FromQmGrid(g 3)
	c.ToString(_s); ShowText "" _s
	
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))

 BEGIN PROJECT
 main_function  dlg_QM_Grid3
 exe_file  $my qm$\dlg_QM_Grid3.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {3DF8143A-F0A4-4CF9-96B0-28876E0E4DBE}
 END PROJECT
