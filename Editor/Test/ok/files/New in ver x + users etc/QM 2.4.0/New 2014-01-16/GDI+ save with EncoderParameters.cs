#compile "__Gdip"
GdipImage g
 ...

GDIP.EncoderParameters p
p.Count=1
memcpy &p.Parameter[0].Guid GDIP.EncoderQuality sizeof(GUID)
p.Parameter[0].Type=GDIP.EncoderParameterValueTypeLong
p.Parameter[0].NumberOfValues=1
int quality=0
p.Parameter[0].Value=&quality


g.Save2("file" &p)
