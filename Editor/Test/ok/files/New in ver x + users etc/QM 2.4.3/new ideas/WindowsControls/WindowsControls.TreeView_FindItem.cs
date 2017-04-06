function# hwnd $itemText [htvi] [flags] ;;flags: 0x100 all descendants

 Finds tree view control item.
 Returns item handle (HTVI) that can be used with tree view control messages.

 hwnd - control handle.
 itemText - item text.
 htvi - parent item handle. Default: 0 - no parent.


opt noerrorshere 1

ret sub.Enum(hwnd itemText htvi flags)


#sub Enum c
function# hwnd $itemText htvi flags

opt noerrorshere 1

 TODO: 64-bit. If HTREEITEM can be 64-bit, use long for htvi and the return type.
type ___TVITEMW64 mask %hItem state stateMask %pszText cchTextMax iImage iSelectedImage cChildren %lParam

byte* m=_Mem(hwnd)

if(!m) end ERR_INIT
int stringOffs=sizeof(___TVITEMW64)

htvi=SendMessage(hwnd TVM_GETNEXTITEM TVGN_CHILD htvi)
rep
	if(!htvi) break
	 if m_is64Bit
		 ___TVITEMW64
	TVITEMW t.mask=TVIF_TEXT
	if(flags&0x100) t.mask|TVIF_CHILDREN
	t.hItem=htvi
	t.pszText=m+stringOffs; t.cchTextMax=1000
	m_pm.Write(&t sizeof(t))
	if(!SendMessage(hwnd TVM_GETITEMW 0 m)) continue
	m_pm.ReadStr(_s 2000 stringOffs 1)
	 out _s
	if(_s=itemText) ret htvi
	if flags&0x100
		TVITEMW tt; m_pm.Read(&tt sizeof(tt))
		if tt.cChildren
			int R=sub.Enum(hwnd itemText htvi flags)
			if(R) ret R
	htvi=SendMessage(hwnd TVM_GETNEXTITEM TVGN_NEXT htvi)
