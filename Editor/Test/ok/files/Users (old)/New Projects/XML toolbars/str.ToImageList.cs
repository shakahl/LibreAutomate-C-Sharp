function#

str s.decrypt(4 this)
IStream is
int hg=GlobalAlloc(GMEM_MOVEABLE s.len)
memcpy(GlobalLock(hg) s s.len)
GlobalUnlock(hg)
CreateStreamOnHGlobal(hg 1 &is)
ret ImageList_Read(is)
