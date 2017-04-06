out
str s
s=".qml"
 s=".bmp"
s=".asp"

IEnumAssocHandlers en
int fl=ASSOC_FILTER_RECOMMENDED
if(SHAssocEnumHandlers(@s fl &en)) ret
 out en
IAssocHandler h
rep
	if(en.Next(1 &h &_i) or _i!1) break
	 out h
	word* w
	h.GetName(&w)
	_s.ansi(w)
	CoTaskMemFree w
	out _s
	