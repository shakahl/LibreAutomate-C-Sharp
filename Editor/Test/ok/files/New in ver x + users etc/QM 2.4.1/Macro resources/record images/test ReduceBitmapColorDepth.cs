out
str bmp="$my qm$\test3.bmp"
str bmp2="$my qm$\test3-2.bmp"
if(!CaptureImageOrColor(0 0 0 bmp)) ret
 _s.getfile(bmp); int nBmp=_s.len
 _s.encrypt(32); int nLzo=_s.len
 out "%i %i" nBmp nLzo
 run "$program files$\Riot\Riot.exe" F"''{bmp}''" "" "*"

	 #FreeImageFileTo4Bit $fileIn $fileOut nColors format
dll "qm.exe"
	#ReduceBitmapColorDepth str*sIn str*sOut nColors

str s1 s2
s1.getfile(bmp); out s1.len
_s.encrypt(32 s1); out F"lzo: {_s.len}"
 PF
int hr=ReduceBitmapColorDepth(&s1 &s2 16)
 PN;PO
if(hr) out F"error {hr}"; ret
s2.setfile(bmp2)
out F"reduced: {s2.len}"
_s.encrypt(32 s2); out F"reduced lzo: {_s.len}"
 outb s1 s1.len
 outb s2 s2.len

dll "freeimage"
	[_FreeImage_ZLibCompress@16]#FreeImage_ZLibCompress !*target targetSize !*source sourceSize
	[_FreeImage_ZLibUncompress@16]#FreeImage_ZLibUncompress !*target targetSize !*source sourceSize

str s3.all(s2.len*2 2) s4.all(s2.len*2 2)
int nbCompr nbUnc
PF
nbCompr=FreeImage_ZLibCompress(s3 s3.len s2 s2.len)
PN
nbUnc=FreeImage_ZLibUncompress(s4 s4.len s3 s3.len)
 PN;PO
out "%i %i" nbCompr nbUnc

 str k1 k2
 PF
 k1.encrypt(32 s2)
 PN
 k2.decrypt(32 k1)
 PN;PO
 out "%i %i" k1.len k2.len

 int hr=FreeImageFileTo4Bit(bmp bmp2 16 0)
 if(hr) out F"error {hr}"; ret
 _s.getfile(bmp2); out F"4-bit: {_s.len}"
 _s.encrypt(32); out F"lzo 4-bit: {_s.len}"
 zip "$my qm$\test3.zip" bmp2; _s.getfile("$my qm$\test3.zip"); out _s.len
 _s.getfile("$my qm$\test3-24bit.png"); out F"png 24-bit: {_s.len}"
 _s.getfile("$my qm$\test3-4bit.png"); out F"png 4-bit: {_s.len}"


 12054
 8-bit: 4794
 reduced: 2198
 reduced lzo: 880

 _______________________

 12054
 lzo: 2591
 4-bit: 2198, core 2154
 lzo 4-bit: 864, core 845

 png 24-bit: 2308
 png 8-bit: 1370
 png 4-bit: 692, core 713

 RIOT png: 669
 RIOT png from 4-bit: 668

 zip: 798, core 785
 8-bit: 5078
 lzo 8-bit: 2355

 out GetFileOrFolderSize(bmp)
  int hr=FreeImageFileTo4Bit(bmp bmp2 16 13)
 int hr=FreeImageFileTo4Bit(bmp bmp2 16 0)
 if(hr) out F"error {hr}"; ret
 _s.getfile(bmp2); out _s.len
 _s.encrypt(32); out _s.len
