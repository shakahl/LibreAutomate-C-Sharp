 /
function# hlv ARRAY(str)&a [ncolumns]

 Writes contents of list view control hlv (hlv is
 list view control handle) to array a. If control
 has report style, returned array has 2 dimensions:
 first for items (rows) and second for columns.
 Function returns number of items.

 EXAMPLE
  Get messages in list view of Outlook Express
 ARRAY(str) a
 GetListViewItems2(id(10372796 "Outlook Express") a)
 int i j
 for i 0 a.len(1)
	 for j 0 a.len(2)
		 out a[i j]


def LVM_GETHEADER LVM_FIRST+31
def HDM_FIRST 0x1200
def HDM_GETITEMCOUNT HDM_FIRST
def LVS_REPORT 1
if(!hlv) end ERR_BADARG

LVITEM* lip=share ;;shared memory address in QM context
LVITEM* lip2=share(hlv) ;;shared memory address in hwnd context
lpstr txt=lip+sizeof(LVITEM)+100
lip.pszText=lip2+sizeof(LVITEM)+100
lip.cchTextMax=800
lip.mask=LVIF_TEXT

int nitems=SendMessage(hlv LVM_GETITEMCOUNT 0 0); if(!nitems) ret
if(GetWinStyle(hlv)&LVS_REPORT)
	if(ncolumns<1) ncolumns=SendMessage(SendMessage(hlv LVM_GETHEADER 0 0) HDM_GETITEMCOUNT 0 0); if(!ncolumns) ncolumns=1
	a.create(nitems ncolumns)
	for lip.iItem 0 nitems
		for lip.iSubItem 0 ncolumns
			SendMessage(hlv LVM_GETITEM 0 lip2)
			a[lip.iItem lip.iSubItem]=txt
else
	a.create(nitems)
	for lip.iItem 0 nitems
		SendMessage(hlv LVM_GETITEM 0 lip2)
		a[lip.iItem]=txt
ret nitems
