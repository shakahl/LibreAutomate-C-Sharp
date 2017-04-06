function! hBitmap

 Replaces tray icon.
 Returns 1 if successful, 0 if failed.

 hBitmap - bitmap handle. The bitmap must be 16x16.

 Info: AddIcon allows you to add one or more icons. This function replaces the first icon.


if(!m_a.len) ret

 create mask
ARRAY(word) a.create(16)
__GdiHandle hm=CreateBitmap(16 16 1 1 &a[0])

 craete icon
ICONINFO in.fIcon=1
in.hbmColor=CopyImage(hBitmap 0 0 0 0) ;;copy because may be selected into a DC
in.hbmMask=hm
int icNew=CreateIconIndirect(&in)
DeleteObject in.hbmColor
if(!icNew) ret

 replace icon in array
__Hicon& ic=m_a[0].hicon
DestroyIcon ic
ic=icNew

 replace icon in tray
this.Modify(1)

ret 1
