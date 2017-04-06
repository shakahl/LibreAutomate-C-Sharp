IXml x._create
str sx=
 <x><a>aaa</a><b p="hh" /></x>
x.FromString(sx)

str sf="$my qm$\test\safe.xml"
PF
 x.Flags=0x300
x.ToFile(sf)
 x.ToFile(sf 0x100)
 x.ToFile(sf 0x300)
PN;PO
x=0

IXml xx._create
xx.FromFile(sf)
xx.ToString(_s); out _s.len
