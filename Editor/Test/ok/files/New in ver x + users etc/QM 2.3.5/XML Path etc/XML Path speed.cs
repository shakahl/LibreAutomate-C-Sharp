out
lpstr sx=
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
 	<elem3 attr="16">JKL</elem3>
 	<elem1 id="test3">ABC3</elem1>
 </root>

IXml x._create
x.FromString(sx)
IXmlNode n
ARRAY(IXmlNode) a; int i

 n=x.Path("root/elem1[@id='test3']" a 1)
 n=x.Path("root/elem1[@id*='test*']" a 1)
 n=x.Path("root/elem3[@attr#='0x10']" a 1)
 n=x.Path("root/elem3[@attr!=0x10]" a 1)
 n=x.Path("root/elem3[@attr>0x10]" a 1)
 n=x.Path("root/elem3[@attr>=0x10]" a 1)
 n=x.Path("root/elem3[@attr<0x11]" a 1)
 n=x.Path("root/elem3[@attr<=0x10]" a 1)
 n=x.Path("root/elem3[@attr&1]" a 1)
 n=x.Path("root/elem3[@attr>=-10]" a 1)
 n=x.Path("root/elem2[elem4]" a 1)
 n=x.Path("root/elem1[@id]" a 1)


 PF
  rep(1000) n=x.Path("root/elem1") ;;old 780, new 570
 rep(1000) n=x.Path("root/elem1[@id='test']") ;;old 2150, new 1000
 PN; PO

if(!n) out "NOT FOUND"; ret
out "-----------------[]%s=%s" n.Name n.Value
if(a.len) out "---- array:"; for(i 0 a.len) n=a[i]; out "%s=%s" n.Name n.Value
