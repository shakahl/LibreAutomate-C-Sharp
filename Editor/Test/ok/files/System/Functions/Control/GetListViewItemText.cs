 /
function# hwnd itemIndex str&text [column] [flags] [int&pitem]

 Gets text of a listview control item.
 Returns text length if successful, 0 if failed.

 hwnd - control handle. Error if 0 or invalid.
 itemIndex - 0-based item index. If flags used, must be -1 or index of item that starts searching (item itself is excluded).
 text - str variable that receives text.
 column - zero-based column index.
 flags - get selected item or item that has certain relationship to the specified item:
   1 focused, 2 selected, 4 cut-highlighted, 8 drop-highlighted, 0x100 above, 0x200 below, 0x400 toleft, 0x800 toright.
 pitem - int variable that receives item index if flags is not 0.

 REMARKS
 Fails if the window belongs to other user account. On Vista/7/8/10 fails if the process has higher integrity level than QM when QM runs as User.
 List view control class name is "SysListView32" or similar.

 EXAMPLES
  Display text of item 5 in Favorites folder:
 int hwnd=win("Favorites" "ExploreWClass")
 int hlv=child("" "SysListView32" hwnd)
 str s
 if(GetListViewItemText(hlv 5 &s)) out s

  Display text of all desktop icons:
 str s
 int hlv=id(1 "Program Manager")
 int i n=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
 for i 0 n
	 GetListViewItemText(hlv i &s)
	 out s

  Display text of all selected items:
 str s; int item=-1
 rep
	 if(GetListViewItemText(hlv item &s 0 2 &item)=0) break
	 out s


type ___LVITEMW64 mask iItem iSubItem state stateMask %pszText cchTextMax iImage %lParam iIndent iGroupId cColumns %puColumns
LVITEMW li; ___LVITEMW64 li2

if(!GetWindowThreadProcessId(hwnd &_i)) end ERR_HWND
text.all

if(flags)
	itemIndex=SendMessage(hwnd LVM_GETNEXTITEM itemIndex flags); if(itemIndex<0) ret
	if(&pitem) pitem=itemIndex

if(_i=GetCurrentProcessId)
	BSTR b.alloc(260)
	li.pszText=b
	li.cchTextMax=260
	li.iSubItem=column
	li.mask=LVIF_TEXT
	int tl=SendMessageW(hwnd LVM_GETITEMTEXTW itemIndex &li)
	if(tl) text.ansi(b)
else
	__ProcessMemory-- m; int-- pid
	if(_i!=pid or WaitForSingleObject(m.hprocess 0)!=WAIT_TIMEOUT)
		m.Alloc(hwnd 1000) ;;don't use _i!
		pid=_i
	int a=m.address
	
	if IsWindow64Bit(hwnd)
		li2.pszText=a+200
		li2.cchTextMax=260
		li2.iSubItem=column
		li2.mask=LVIF_TEXT
		m.Write(&li2 sizeof(li2))
	else
		li.pszText=+(a+200)
		li.cchTextMax=260
		li.iSubItem=column
		li.mask=LVIF_TEXT
		m.Write(&li sizeof(li))
	
	tl=SendMessageW(hwnd LVM_GETITEMTEXTW itemIndex a)
	if(tl) m.ReadStr(text tl*2 200 1)

ret tl
err+
