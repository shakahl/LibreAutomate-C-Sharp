out
IXml x=CreateXml
x.FromFile("$my qm$\test.xml")
IXmlNode n
ARRAY(IXmlNode) a
int i

 n=x.Path("styling/common/c" a)
 n=x.Path("styling/common/*" a)
 n=x.Path("styling/lang/styles/s32/@f" a)
 n=x.Path("styling/lang/styles/s32/@*" a) ;;first
 n=x.Path("styling/lang/styles/s32/@" a) ;;last
 n=x.Path("styling/lang/styles/*/@*" a)
 n=x.Path("styling/@a" a)

 n=x.Path("styling/default/font/../../lang/misc/line_numbers" a)

 n=x.Path("styling/lang/styles/s32/@*")
 n=n.Path(".." a)
 n=n.Path("../@fs" a)
 n=n.Path("../@*" a)
 n=n.Path("../../?pi1" a)
 n=n.Path("../../../../../!-" a)
 n=n.Path("../../../../![" a)
 n=n.Path("../../../../../?pi0" a)

 n=x.Path("?xml" a)
 n=x.Path("?xml/@encoding" a)
 n=x.Path("?xml/@*" a)
 n=x.Path("?xml/@" a)
 n=x.Path("!DOCTYPE" a)
 n=x.Path("?pi0" a)

 n=x.Path("styling/default/font")
 n=x.Path("styling/common/b/") ;;text
 n=x.Path("styling/common/b").Path("") ;;text
 n=x.Path("*" a)
 n=x.Path("*/lang" a)
 n=x.Path("*/*" a)
 n=x.Path("styling/*" a)
 n=x.Path("styling/lang/*" a)
 n=x.Path("styling/lang/styles/*" a)
 n=x.Path("styling/lang/styles/*[@u*='*']" a)
 n=x.Path("styling/lang/misc/*" a)
 n=x.Path("styling/common/b/*" a)
 n=x.Path("styling/common/c[@at='aa/bb']" a)

if(!n) out "NOT FOUND"; ret
out "-----------------[]%s=%s" n.Name n.Value
if(a.len) out "---- array:"; for(i 0 a.len) n=a[i]; out "%s=%s" n.Name n.Value

 str s
 x.ToString(s)
 out s
