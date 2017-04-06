str s
ARRAY(str) a
str* p
type EMBEDDEDARRAY x str'a[4]; EMBEDDEDARRAY e
type BASEIMPLICIT str's; BASEIMPLICIT bi
type ARRBASE :ARRAY(str)'a x y; ARRBASE ab
type ARRBASEIMPLICIT ARRAY(str)'a; ARRBASEIMPLICIT abi

s.all
s.LoadUnicodeFile("")
STRINT f.s.ucase; f.s.ConvertEncoding
bi.ansi
Http ht.Get; ht.lasterror; ht.SetProgressCallback

a[0].fix
p[0].lcase
e.a[0].ucase
a[-(0+0) "ffff"].fix

ab.a.create(2); ab.create(2)
ab.a[1].addline; ab[1].beg
abi.a.create(2); abi.create(2)
abi.a[1].addline; abi[1].beg

p._delete
p.addline; p.len

 _____________________________

Excel.Application ap
ap._create
ap.Range(1).Application.Range(1).Activate

IXml x._create
x.RootElement.FirstChild.Child("").Next.Attribute.Move

SHDocVw.WebBrowser b
