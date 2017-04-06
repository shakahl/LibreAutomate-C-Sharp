out
ref qmzip qmzip_def
ChDir "$my qm$"
str ss=
 Macro2198.bmp
 Macro2198 (2).bmp
 Macro2198 (3).bmp
 Macro2198 (4).bmp
 Macro2198 (5).bmp
str s s1 s2
foreach s ss
	out s
	str sl=s; sl.set("lzo" sl.len-3)
	str sz=s; sz.set("zip" sz.len-3)
	str sg=s; sg.set("gif" sg.len-3)
	str sj=s; sj.set("jpg" sj.len-3)
	str sb=s; sb.set("-8bit.bmp" sb.len-4)
	
	s1.getfile(sl)
	s2.getfile(sz)
	PF
	_s.decrypt(32 s1)
	PN
	int z=qmzip.OpenZip(s2 s2.len qmzip.ZIP_MEMORY)
	 qmzip.ZIPENTRY ze; outx qmzip.GetZipItem(z 0 &ze)
	 out "%i %i" ze.unc_size ze.comp_size
	_s.all(50000 2)
	 outb _s 100 1
	 _s.all(ze.unc_size 2)
	qmzip.UnzipItem(z 0 _s _s.len qmzip.ZIP_MEMORY)
	 outx qmzip.UnzipItem(z 0 L"unz.bmp" 0 qmzip.ZIP_FILENAME)
	 outb _s 100 1
	qmzip.CloseZip(z)
	PN;PO
