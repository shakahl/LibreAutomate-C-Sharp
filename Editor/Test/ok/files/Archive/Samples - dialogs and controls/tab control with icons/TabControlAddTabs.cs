 /
function hwnd $tabs

 Adds tabs to control of SysTabControl32 class.

 hwnd - control handle.
 tabs - list of tabs in CSV format.
   Line format: label,imageIndex,lParam
   label - tab text.
   imageIndex - icon index in imagelist.
   lParam - an integer number to assign to the tab.
   imageIndex and lParam are optional, so it can be simply list of labels.

 EXAMPLE
 	case WM_INITDIALOG
 	__ImageList-- il.Load("$qm$\il_qm.bmp") ;;load an imagelist created with QM imagelist editor
 	int htb=id(3 hDlg)
 	SendMessage htb TCM_SETIMAGELIST 0 il
 	TabControlAddTabs htb "A,2[]B[]C,15"

ICsv x._create
x.FromString(tabs)
int i nc=x.ColumnCount
lpstr s
for i 0 x.RowCount
	TCITEMW ti.mask=TCIF_TEXT
	ti.pszText=@x.Cell(i 0)
	if nc>1
		s=x.Cell(i 1)
		if(!empty(s)) ti.iImage=val(s); ti.mask|TCIF_IMAGE
	if nc>2
		s=x.Cell(i 2)
		if(!empty(s)) ti.lParam=val(s); ti.mask|TCIF_PARAM
	SendMessageW hwnd TCM_INSERTITEMW i &ti
