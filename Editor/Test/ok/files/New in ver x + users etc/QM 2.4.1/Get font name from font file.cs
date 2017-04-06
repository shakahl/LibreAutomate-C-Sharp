 /exe
str fontFile="$desktop$\calibri.ttf"

GDIP.GdiplusStartupInput gi.GdiplusVersion=1
GDIP.GdiplusStartup(&_i &gi 0)
atend GDIP.GdiplusShutdown _i
 out InitWindowsDll(1) ;;QM 2.4.1

int r
GDIP.GpFontCollection* c
r=GDIP.GdipNewPrivateFontCollection(&c); if(r) end F"error {r}"
r=GDIP.GdipPrivateAddFontFile(c @_s.expandpath(fontFile))
if r
	out F"failed to load font file, error {r}"
else
	int i n
	GDIP.GdipGetFontCollectionFamilyCount(c &n)
	ARRAY(GDIP.GpFontFamily*) a.create(n)
	GDIP.GdipGetFontCollectionFamilyList(c n &a[0] &n)
	
	BSTR b.alloc(100)
	for(i 0 n)
		if(GDIP.GdipGetFamilyName(a[i] b 0)) out "failed"
		else _s.ansi(b); out _s
	
	 for(i 0 n) GDIP.GdipDeleteFontFamily(a[i]) ;;don't

GDIP.GdipDeletePrivateFontCollection(&c)

 BEGIN PROJECT
 main_function  Macro2240
 exe_file  $my qm$\Macro2240.qmm
 flags  6
 guid  {5CC77F36-0742-4BCB-92DB-56D05C0A48F3}
 END PROJECT
