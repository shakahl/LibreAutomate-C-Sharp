function il

IStream is
int hg=GlobalAlloc(GMEM_MOVEABLE 0)
CreateStreamOnHGlobal(hg 1 &is)
ImageList_Write il is
this.fromn(GlobalLock(hg) GlobalSize(hg))
this.encrypt(4)
GlobalUnlock hg
