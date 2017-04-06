 /test TreeViewFindItem
function# htv $text [flags] [htvi] ;;flags: 0x100 all descendants

 Finds tree view control item.
 Returns item handle that can be used with tree view control messages.

 hwnd - control handle.
 itemText - item text.
 htvi - parent item handle. Default: 0 - no parent.

 REMARKS
 Can be used with windows of any process.
 Does not work with 64-bit processes where tree item handles have more than 32 bits.


opt noerrorshere 1
type __TVFI_CONTEXT htv $text flags BSTR'b1000 str's __ProcessMemory'pm byte*_pm
__TVFI_CONTEXT c.htv=htv; c.text=text; c.flags=flags; c.s.flags=1
type ___TVITEMW64 mask %hItem state stateMask %pszText cchTextMax iImage iSelectedImage cChildren %lParam

if(!GetWindowThreadProcessId(htv &_i)) end ERR_FAILED
if _i!=GetCurrentProcessId
	c._pm=c.pm.Alloc(_i 4000 1)
	ret sub.EnumChildren(c htvi 1+(IsWindow64Bit(_i 1)))
else
	c.b1000.alloc(1000)
	ret sub.EnumChildren(c htvi 0)


#sub EnumChildren
function# __TVFI_CONTEXT&c htvi process ;;process: 0 this, 1 32-bit, 2 64-bit

htvi=SendMessageW(c.htv TVM_GETNEXTITEM TVGN_CHILD htvi)
TVITEMW t.mask=TVIF_TEXT; ___TVITEMW64 t64.mask=TVIF_TEXT
if(c.flags&0x100) t.mask|TVIF_CHILDREN; t64.mask|TVIF_CHILDREN
TVITEMW* pt
rep
	if(!htvi) break
	 out htvi
	sel process
		case 0
		t.hItem=htvi
		t.cchTextMax=1000
		t.pszText=c.b1000 ;;note: TVM_GETITEMW changes pszText (pointer)
		pt=&t
		
		case 1
		t.hItem=htvi
		t.cchTextMax=1000
		pt=c._pm
		t.pszText=pt+1000
		c.pm.Write(&t sizeof(t))
		
		case 2
		t64.hItem=htvi
		t64.cchTextMax=1000
		pt=c._pm
		t64.pszText=pt+1000
		c.pm.Write(&t64 sizeof(t64))
	
	if(!SendMessageW(c.htv TVM_GETITEMW 0 pt)) end ERR_FAILED ;;if fails, probably htvi is truncated (64-bit process)
	if(process) c.pm.ReadStr(c.s 2000 1000 1); else c.s.ansi(c.b1000)
	out c.s
	if(c.s=c.text) ret htvi
	 TODO: insens, wildcard, rx etc
	
	if c.flags&0x100
		if(process) c.pm.Read(&t.cChildren 4 iif(process=1 32 44)) ;;fast
		if t.cChildren
			int R=sub.EnumChildren(c htvi process)
			if(R) ret R
	
	htvi=SendMessageW(c.htv TVM_GETNEXTITEM TVGN_NEXT htvi)
