 Extracts icons from bitmaps in f1 and saves in f2.
 Does not enum subfolders.

str f1="Q:\ico and bmp\bmp"
str f2="Q:\ico and bmp\bmp\ico"

 int cymin=16
 int cymax=32
 int cxmin=16
 int bcmin=4
 int bcmax=32

int cymin=16
int cymax=16
int cxmin=16
int bcmin=4
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
	
	BITMAPINFOHEADER* b=+sd+14
	int bc cx cy
	if(b.biSize<sizeof(b)) ;;eg UPX-compressed, or BITMAPCOREHEADER (never seen)
		 out "failed: %s" d.FileName
		continue
	bc=b.biBitCount; cx=b.biWidth; cy=b.biHeight
	
	if(cy>=cymin and cy<=cymax and bc>=bcmin and bc<=bcmax and cx>=cxmin)
		sel(bc) case [4,8,24,32] case else continue
		if(cx%cy or cy&1) continue
		
		 out "%i-bit, cx=%i, cy=%i, %s" bc cx cy d.FileName
		if(!__BitmapToIcons(sPath f2 -1 0))
			out "failed: %i-bit, cx=%i, cy=%i, %s" bc cx cy d.FileName
