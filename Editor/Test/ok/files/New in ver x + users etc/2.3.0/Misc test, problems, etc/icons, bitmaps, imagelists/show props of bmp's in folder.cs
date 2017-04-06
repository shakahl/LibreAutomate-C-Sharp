 displays properties of bitmaps

 str f1="$desktop$\bmp new3"
str f1="$desktop$\bmp in pf"

 int cymin=16
 int cymax=32
 int cxmin=64
 int bcmin=1
 int bcmax=32

int cymin=256
int cymax=256
int cxmin=256
int bcmin=1
int bcmax=32

 ---------------

out

f1+"\*.bmp"
ARRAY(int) a; int i
Dir d
foreach(d f1 FE_Dir)
	str sPath=d.FileName(1)
	str sd.getfile(sPath); err out "failed: %s" d.FileName; continue
	
	 skip duplicates
	int crc=Crc32(sd sd.len)
	for(i 0 a.len) if(crc=a[i]) break
	if(i=a.len) a[]=crc; else continue
	
	BITMAPCOREHEADER* c=+sd+14
	BITMAPINFOHEADER* b=+c
	int bc cx cy core
	if(c.bcSize<sizeof(c)) out "failed: %s" d.FileName; continue
	if(c.bcSize=sizeof(c)) core=1; bc=c.bcBitCount; cx=c.bcWidth; cy=c.bcHeight
	else core=0; bc=b.biBitCount; cx=b.biWidth; cy=b.biHeight
	
	if(cy>=cymin and cy<=cymax and bc>=bcmin and bc<=bcmax and cx>=cxmin)
		out "%s%i-bit, cx=%i, cy=%i, %s" iif(core "CORE, " "") bc cx cy d.FileName
