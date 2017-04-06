\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_QM_Grid_images2 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 291 175 "QM_Grid"
 3 SysListView32 0x54210041 0x200 0 0 294 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""
 3 QM_Grid 0x54210041 0x200 0 0 294 154 ""

ret
 messages
int g i; ICsv c
sel message
	case WM_INITDIALOG
	g=id(3 hDlg)
	 SendMessage g GRID.LVM_QG_SETSTYLE GRID.QG_NOEDITFIRSTCOLUMN -1
	SendMessage g LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_CHECKBOXES|LVS_EX_FULLROWSELECT LVS_EX_CHECKBOXES|LVS_EX_FULLROWSELECT
	TO_LvAddCols(g "one" 100 "two" 100)
	for i 0 4
		_s.RandomString(4 4 "A-Z")
		TO_LvAdd g i i*10 i+2 F"item{i}" F"{_s}{i}"
	
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	SendMessage g LVM_SETIMAGELIST LVSIL_SMALL il
	
	LVITEM lic
	 for(i 0 3)
		 lic.stateMask=LVIS_STATEIMAGEMASK; lic.state=(1<<12)
		 SendMessage(g LVM_SETITEMSTATE i &lic)
	
	out
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	
	case 4
	g=id(3 hDlg)
	
	lic.stateMask=LVIS_STATEIMAGEMASK; lic.state=0x2000
	 lic.stateMask=LVIS_OVERLAYMASK; lic.state=0x200
	
	SendMessage(g LVM_SETITEMSTATE i &lic)
	ret
	
	 for i 0 SendMessage(g LVM_GETITEMCOUNT 0 0)
		 LVITEMW lis.mask=LVIF_IMAGE|LVIF_PARAM
		 lis.iItem=i; lis.iImage=i+5; lis.lParam=i*100
		  lis.mask|LVIF_TEXT; lis.pszText="text"; lis.iSubItem=1
		 lis.mask|LVIF_STATE; lis.stateMask=-1; lis.state=LVIS_CUT
		 if(SendMessage(g LVM_SETITEM 0 &lis)) out "ok"
	 
	 for(i 0 SendMessage(g LVM_GETITEMCOUNT 0 0))
		 lic.stateMask=LVIS_STATEIMAGEMASK
		 lic.state=0x2000
		 SendMessage(g LVM_SETITEMSTATE i &lic)
	
	 SendMessage g LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_CHECKBOXES 0
	 SendMessage g LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_CHECKBOXES LVS_EX_CHECKBOXES
	for i 0 SendMessage(g LVM_GETITEMCOUNT 0 0)
		LVITEMW li.mask=LVIF_IMAGE|LVIF_PARAM|LVIF_STATE|LVIF_INDENT
		li.iItem=i
		li.mask|LVIF_STATE; li.stateMask=-1
		if(SendMessage(g LVM_GETITEM 0 &li)) out "%i %i 0x%X %i" li.iImage li.lParam li.state li.iIndent
	
	 SendMessage(g LVM_SORTITEMSEX g &lvsortproc)
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))

NMLISTVIEW* nlv=+nh
NMITEMACTIVATE* na=+nh
sel nh.code
	case LVN_ITEMCHANGED
	out "itemchanged: %s" _s.getstruct(*nlv 1)
	
	case NM_CLICK
	out "row click: %s" _s.getstruct(*na 1)
