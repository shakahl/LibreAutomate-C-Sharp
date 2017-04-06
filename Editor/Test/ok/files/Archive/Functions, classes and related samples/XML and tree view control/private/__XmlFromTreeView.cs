 /
function htv htvi IXmlNode&n

htvi=SendMessage(htv TVM_GETNEXTITEM iif(htvi TVGN_CHILD TVGN_ROOT) htvi)
rep
	if(!htvi) break
	TVITEMW t.mask=TVIF_TEXT|TVIF_CHILDREN
	t.hItem=htvi
	BSTR b; if(!b) b.alloc(1000)
	t.pszText=b; t.cchTextMax=1001
	SendMessage(htv TVM_GETITEMW 0 &t)
	_s.ansi(b)
	IXmlNode nn=n.Add("x")
	nn.SetAttribute("t" _s)
	if t.cChildren
		__XmlFromTreeView htv htvi nn
	htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT htvi)

err+ end _error
