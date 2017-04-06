ARRAY(int) ab.create(2)
ab[0]=LoadPictureFile("$my qm$\Macro341.bmp" 0)
ab[1]=LoadPictureFile("$my qm$\Macro341 (2).bmp" 0)
int width(20) height(20)

 source dc
int dcs=CreateCompatibleDC(0)

 dest bitmap and dc
int dc0=GetDC(0)
int bm=CreateCompatibleBitmap(dc0 width*ab.len height)
ReleaseDC(0 dc0)
int dc=CreateCompatibleDC(0)
int oldbm=SelectObject(dc bm)

int i
for i 0 ab.len
	int oldbms=SelectObject(dcs ab[i])
	BitBlt(dc i*width 0 width height dcs 0 0 SRCCOPY)
	SelectObject(dcs oldbms)

SelectObject(dc oldbm)
DeleteDC(dc)
DeleteDC(dcs)

for(i 0 ab.len) DeleteObject(ab[i])

SaveBitmap(bm "$desktop$\test.bmp")
DeleteObject(bm)
run "$desktop$\test.bmp"
