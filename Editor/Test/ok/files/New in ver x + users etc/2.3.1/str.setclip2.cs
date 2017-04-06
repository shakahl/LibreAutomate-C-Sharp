if(!OpenClipboard(0)) end ES_FAILED
EmptyClipboard
if(this.lpstr)
	_s.unicode(this)
	int h=GlobalAlloc(GMEM_MOVEABLE|GMEM_DDESHARE _s.len+2)
	memcpy GlobalLock(h) _s _s.len+2; GlobalUnlock h
	SetClipboardData CF_UNICODETEXT h
CloseClipboard
