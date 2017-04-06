function hbitmap

 Stores an existing bitmap into this variable without copying.
 Later will auto delete the bitmap.

 hbitmap - bitmap handle.


Delete

dc=CreateCompatibleDC(0)
bm=hbitmap
oldbm=SelectObject(dc bm)
