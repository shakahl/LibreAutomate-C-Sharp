 /
function# htv $itemText [htvi] [flags] ;;flags: 0x100 all descendants

 Finds tree view control item.
 Returns item handle (HTVI) that can be used with tree view control messages.

 name - item text.
 itemText - control handle.
 htvi - parent item handle. Can be 0 to search all.


opt noerrorshere 1

__ProcessMemory pm.Alloc(htv 4096)

ret sub.Enum(pm htv itemText htvi flags)


#sub Enum
function# __ProcessMemory&pm htv $itemText htvi flags

opt noerrorshere 1

byte* m=pm.address
 TODO: 64-bit

htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_CHILD htvi)
rep
	if(!htvi) break
	TVITEMW t.mask=TVIF_TEXT
	if(flags&0x100) t.mask|TVIF_CHILDREN
	t.hItem=htvi
	t.pszText=m+sizeof(t); t.cchTextMax=1000
	pm.Write(&t sizeof(t))
	if(!SendMessage(htv TVM_GETITEMW 0 m)) continue
	pm.ReadStr(_s 2000 sizeof(t) 1)
	 out _s
	if(_s=itemText) ret htvi
	if flags&0x100
		TVITEMW tt; pm.Read(&tt sizeof(tt))
		if tt.cChildren
			int R=sub.Enum(pm htv itemText htvi flags)
			if(R) ret R
	htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT htvi)
