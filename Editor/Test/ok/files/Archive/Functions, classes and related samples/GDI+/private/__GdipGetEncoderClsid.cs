 /
function! $format GUID&clsid

int i n; str s1 s2

_hresult=GDIP.UnknownImageFormat
GDIP.GdipGetImageEncodersSize(&n &i); if(!i) ret
GDIP.ImageCodecInfo* ici=s1.all(i 2)
GDIP.GdipGetImageEncoders(n i ici)
for i 0 n
	s2.ansi(ici[i].MimeType)
	 out s2
	if(s2~format)
		clsid=ici[i].Clsid
		_hresult=0
		ret 1

 info: available only for bmp, jpeg, gif, png and tiff
