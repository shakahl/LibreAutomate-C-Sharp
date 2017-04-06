\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid_images 0 _hwndqm)) ret

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
	
	 __Font- f.Create("Tahoma" 12 2)
	 SendMessage g WM_SETFONT f 0
	
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
	 use first column as noneditable
	 SendMessage g LVM_QG_SETSTYLE QG_NOAUTOADD -1
	 SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN -1
	SendMessage g LVM_QG_SETSTYLE QG_NOAUTOADD|QG_NOEDITFIRSTCOLUMN -1
	SendMessage g LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_CHECKBOXES LVS_EX_CHECKBOXES
	 add columns
	lpstr columns="noedit,150[]edit,50[]edit+button,80,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]read-only,80,7"
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
	
	 for i 0 4
		  TO_LvAdd g i i*10 i+3 F"item{i}" F"subitem{i}"
		 _s.RandomString(3 3 "a-z")
		 TO_LvAdd g i i*10 i+3 F"item{i}" F"a {_s}" F"b {_s}" F"c {_s}" F"d {_s}" F"e {_s}" F"f {_s}"
	
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	ImageList_SetOverlayImage(il 0 1); ImageList_SetOverlayImage(il 1 2)
	SendMessage g LVM_SETIMAGELIST LVSIL_SMALL il
	
	for(i 0 SendMessage(g LVM_GETITEMCOUNT 0 0))
		 LVITEM lic.stateMask=LVIS_STATEIMAGEMASK|LVIS_OVERLAYMASK
		 lic.state=(i&1+1<<12)|(i&1+1<<8)
		 SendMessage(g LVM_SETITEMSTATE i &lic)
		LVITEM lic.mask=LVIF_IMAGE|LVIF_STATE|LVIF_PARAM
		lic.iItem=i
		lic.iImage=i+3
		lic.lParam=i*10
		lic.stateMask=LVIS_STATEIMAGEMASK|LVIS_OVERLAYMASK
		lic.state=(i&1+1<<12)|(i&1+1<<8)
		lic.mask|LVIF_INDENT; lic.iIndent=i
		SendMessage(g LVM_SETITEM 0 &lic)
	
	
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
	
	 for i 0 SendMessage(g LVM_GETITEMCOUNT 0 0)
		 LVITEMW lis.mask=LVIF_IMAGE|LVIF_PARAM
		 lis.iItem=i; lis.iImage=i+5; lis.lParam=i*100
		  lis.mask|LVIF_TEXT; lis.pszText="text"; lis.iSubItem=1
		 lis.mask|LVIF_STATE; lis.stateMask=LVIS_CUT|LVIS_STATEIMAGEMASK|LVIS_OVERLAYMASK
		 lis.state=LVIS_CUT|(2<<12)|(i&1^1+1<<8)
		 if(SendMessage(g LVM_SETITEM 0 &lis)) out "ok"
	 
	for i 0 SendMessage(g LVM_GETITEMCOUNT 0 0)
		LVITEMW li.mask=LVIF_IMAGE|LVIF_PARAM
		li.iItem=i
		li.mask|LVIF_STATE; li.stateMask=-1
		if(SendMessage(g LVM_GETITEM 0 &li)) out "%i %i 0x%X" li.iImage li.lParam li.state
		outx SendMessage(g LVM_GETITEMSTATE i -1)
	
	 SendMessage(g LVM_SORTITEMSEX g &lvsortproc)
	 SendMessage(g LVM_SORTITEMS g &lvsortproc)
	
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
