 copies bitmaps of specified properties from f1 to f2

str f1="$desktop$\bmp new3"
str f2="$desktop$\bmp new3\16"

int cymin=16
int cymax=16
int bcmin=8
int bcmax=32

 ---------------

out
mkdir f2

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
	int bc cx cy
	if(c.bcSize<sizeof(c)) out "failed: %s" d.FileName; continue
	if(c.bcSize=sizeof(c)) bc=c.bcBitCount; cx=c.bcWidth; cy=c.bcHeight
	else bc=b.biBitCount; cx=b.biWidth; cy=b.biHeight
	 out "%i-bit, cx=%i, cy=%i, %s" bc cx cy d.FileName
	if(cy>=cymin and cy<=cymax and bc>=bcmin and bc<=bcmax)
		FileCopy sPath _s.from(f2 "\" d.FileName)
	
run f2
