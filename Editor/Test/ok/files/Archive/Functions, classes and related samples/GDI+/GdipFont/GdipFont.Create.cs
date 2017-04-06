function'GDIP.GpFont* $name ^emSize [style] ;;style (flags): 1 bold, 2 italic, 4 underline, 8 strikeout

 Creates this font.

 name - font name.
   Can be list of names (multiline). If failed to craete first font, tries next, and so on.
   Finally tries several commonly used fonts, so this function should never fail.
 emSize - font size. Normal values for small font are 8-12.


if(!GdipInit) ret
Delete

GDIP.GpFontFamily* ff
str s slist=name
slist.addline("Segoe UI[]Tahoma[]Arial")
foreach s slist
	_hresult=GDIP.GdipCreateFontFamilyFromName(@s 0 &ff); if(_hresult) continue
	_hresult=GDIP.GdipCreateFont(ff emSize style GDIP.UnitPoint &m_f)
	GDIP.GdipDeleteFontFamily ff
	if(!_hresult) ret m_f
