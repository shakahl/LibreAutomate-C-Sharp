out
#compile "__Gdip"
GDIP.EncoderParameters p
p.Count=1
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx x._create
ChDir "$my qm$"
str ss=
 31C356D3.bmp
 1950FFE7.bmp
 4615FF5B.bmp
 38493BB3.bmp
 20189523.bmp
 B15B065E.bmp
 4A81F969.bmp
 7E4FA268.bmp
int tBmp tLzo tZip tGif tJpg tPng tJpg2 tPng2 tBest
int nBmp nLzo nJpg
str s
foreach s ss
	out s
	str sl=s; sl.set("lzo" s.len-3)
	str sz=s; sz.set("zip" s.len-3)
	str sg=s; sg.set("gif" s.len-3)
	str sj=s; sj.set("jpg" s.len-3)
	str sp=s; sp.set("png" s.len-3)
	str sj2=s; sj2.set("-x.jpg" s.len-4)
	str sp2=s; sp2.set("-x.png" s.len-4)
	str sb=s; sb.set("-8bit.bmp" s.len-4)
	
	_s.getfile(s); out "bmp: %i" _s.len; tBmp+_s.len; nBmp=_s.len
	 continue
	_s.encrypt(32)
	out "lzo: %i" _s.len; tLzo+_s.len; nLzo=_s.len
	_s.setfile(sl)
	
	zip sz s
	_s.getfile(sz); out "zip: %i" _s.len; tZip+_s.len
	
	GdipImage g
	g.FromFile(s)
	
	memcpy &p.Parameter[0].Guid GDIP.EncoderQuality sizeof(GUID)
	p.Parameter[0].Type=GDIP.EncoderParameterValueTypeLong
	p.Parameter[0].NumberOfValues=1
	int quality=50
	p.Parameter[0].Value=&quality
	 memcpy &p.Parameter[0].Guid GDIP.EncoderCompression sizeof(GUID)
	 p.Parameter[0].Type=GDIP.EncoderParameterValueTypeLong
	 p.Parameter[0].NumberOfValues=1
	 int compr=0
	 p.Parameter[0].Value=&compr
	
	g.Save(sg)
	 g.Save(sg &p)
	_s.getfile(sg); out "gif: %i" _s.len; tGif+_s.len
	
	g.Save(sj)
	 g.Save(sj &p)
	_s.getfile(sj); out "jpg: %i" _s.len; tJpg+_s.len; nJpg=_s.len
	
	 GDIP.PixelFormat4bppIndexed
	g.Save(sp)
	 g.Save(sp &p)
	_s.getfile(sp); out "png: %i" _s.len; tPng+_s.len
	
	x.LoadBitmap(s)
	
	x.SaveFormat=GflAx.AX_JPEG
	 x.SaveFormat=GflAx.AX_JPEG2000
	x.SaveJPEGQuality=50
	x.SaveJPEGProgressive=1
	x.SaveBitmap(sj2)
	_s.getfile(sj2); out "jpg2: %i" _s.len; tJpg2+_s.len
	
	x.SaveFormat=GflAx.AX_PNG
	 x.SavePNGCompression=8
	x.SaveBitmap(sp2)
	_s.getfile(sp2); out "png2: %i" _s.len; tPng2+_s.len
	
	 x.SaveFormat=GflAx.AX_JPEG2000
	 x.SaveBitmap(sp2)
	 _s.getfile(sp2); out "png2: %i" _s.len; tPng2+_s.len
	
	 _s.getfile(sb)
	 out "bm8: %i" _s.len
	
	int best
	if(nBmp/nLzo>=5) best=nLzo
	else best=iif(nJpg<nLzo nJpg nLzo)
	tBest+best

out F"total: bmp={tBmp} lzo={tLzo} zip={tZip} gif={tGif} jpg={tJpg} png={tPng} jpg2={tJpg2} png2={tPng2} best={tBest}"
 total: bmp=62640 lzo=18629 zip=15994 gif=15101 jpg=12208
