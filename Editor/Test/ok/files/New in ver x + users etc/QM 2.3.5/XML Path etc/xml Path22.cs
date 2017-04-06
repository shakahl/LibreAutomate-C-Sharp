out
lpstr sx=
 <?xml version="1.0" encoding="utf-8" ?>
 <root>
 	<elem1>ABC</elem1>
 	<elem1 id="test" kid="x">ABC2</elem1>
 	<elem2>
 		<xyz>XYZ1</xyz>
 		<xyz>XYZ2</xyz>
 	</elem2>
 	<xyz>XYZ22</xyz>
 	<elem2 id="u" kid="z">
 		<xyz>XYZ3</xyz>
 		<x.z>XYZ1</x.z>
	 	<elem4>
	 		<xyz id="test2">XYZ4</xyz>
	 		<xyz>XYZ5<xyz>XYZ6<xyz>XYZ7</xyz></xyz></xyz>
	 	</elem4>
 	</elem2>
 	<elem3 attr="35">GHI</elem3>
 	<elem3 attr="45">JKL</elem3>
 	<elem1 id="test3">ABC3</elem1>
 </root>

IXml x._create
x.FromString(sx)
IXmlNode n
ARRAY(IXmlNode) a; int i

 n=x.Path("//xyz" a)
 n=x.Path("//@id" a)
 n=x.Path("//@*" a)
 n=x.Path("//@version" a)
 n=x.Path("//elem2/@id" a)
 n=x.Path("//@id/.." a)
 n=x.Path("//@id[='uf']" a) ;;we don't support it
 n=x.Path("//elem2/xyz" a)
 n=x.Path("root/elem2/@*" a)

 n=x.Path("root/elem3" a)
 n=x.Path("./root/elem1" a)
 n=x.Path("root/./elem1" a)
 n=x.Path("*/elem2/xyz" a)
 n=x.Path("*/elem2[@id='u']/elem4/xyz" a)
 n=x.Path("*/elem2/elem4/xyz" a)
 n=x.Path("*/*/elem4/xyz" a)

 n=x.Path("root/elem2/../*" a)
 n=x.Path("root/elem1/@id/.." a)
 n=x.Path("root/elem2/@id/xyz" a)
 n=x.Path("root/elem2[@id*='*']/xyz" a)
 n=x.Path("root/*/.." a)
 n=x.Path("root/elem1/../elem2/elem4/xyz" a)
 n=x.Path("root/elem1/../elem2[@id='u']/elem4/xyz" a)
 n=x.Path("root/elem1").Path("../elem2/xyz" a)
 n=x.Path("root/elem1/../elem2/no" a)
 n=x.Path("root/*/../../..")
 n=x.Path("/root/elem2" a)
 n=x.Path("root/elem1").Path("/root/elem2" a)
 n=x.Path("/root/elem3[.='JKL']" a)

 n=x.Path("root/elem1/@id" a)
 n=x.Path("root/elem1/@id/more")
 n=x.Path("root/elem2[@id='u']/elem4/xyz")
 n=x.Path("*" a)
 n=x.Path("*/elem3" a)
 n=x.Path("?*" a)
 n=x.Path("?xml[@version='1.0']" a)

if(!n) out "NOT FOUND"; ret
out "-----------------[]%s=%s" n.Name n.Value
if(a.len) out "---- array:"; for(i 0 a.len) n=a[i]; out "%s=%s" n.Name n.Value

 n=x.RootElement.Child("elem1")
 n=x.RootElement.Child("elem1[@id='test']")
 n=x.RootElement.Child("*2")
 n=x.Root.Child("?xml")
