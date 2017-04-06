__GdiHandle h
if(!CaptureImageOrColor(&h 0)) ret
if OpenClipboard(_hwndqm)
	EmptyClipboard
	if(SetClipboardData(CF_BITMAP h)) h=0
	CloseClipboard
