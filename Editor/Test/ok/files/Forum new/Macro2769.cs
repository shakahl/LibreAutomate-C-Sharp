OnScreenDisplay "Text" 0 0 0 "" 0 0 1 "get_bitmap"

int w=wait(0 WV win("get_bitmap" "QM_OSD_Class"))
RECT r; GetClientRect w &r
__Hdc dc.Init(w)
__MemBmp m.Create(r.right r.bottom dc)
dc.Release
OsdHide "get_bitmap"
if(!OpenClipboard(_hwndqm)) end "failed"
EmptyClipboard
if(SetClipboardData(CF_BITMAP m.bm)) m.bm=0
CloseClipboard

act "Paint"
key Cv
