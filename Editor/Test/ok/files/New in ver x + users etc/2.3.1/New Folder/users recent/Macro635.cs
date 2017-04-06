int nitems=10
int ihei=16
int hei=nitems*ihei
__MemBmp m.Create(1 hei)
ARRAY(int) a.create(hei)
int i j
for i 0 hei/ihei
	int c=ColorAdjustLuma(RandomInt(0 0xffffff) 500 1)
	for j i*ihei i*ihei+ihei
		a[j]=c

BITMAPINFOHEADER bh.biSize=sizeof(bh)
bh.biBitCount=32
bh.biHeight=hei
bh.biWidth=1
bh.biPlanes=1
SetDIBits m.dc m.bm 0 hei &a[0] +&bh DIB_RGB_COLORS

str f="$temp$\my qm menu.bmp"
SaveBitmap m.bm f
 run f
mac "Menu22"
