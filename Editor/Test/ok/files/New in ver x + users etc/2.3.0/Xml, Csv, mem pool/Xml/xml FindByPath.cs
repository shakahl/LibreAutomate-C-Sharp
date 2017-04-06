out
IXml x=CreateXml

x.FromFile("$my qm$\test.xml")

IXmlNode e
ARRAY(IXmlNode) a
int i

e=x.Path("styling/common/c" a)
 e=x.Path("styling/common/*" a)
 e=x.Path("styling/lang/styles/s32/@f" a)
 e=x.Path("styling/lang/styles/s32/@*" a) ;;first
 e=x.Path("styling/lang/styles/s32/@" a) ;;last

 e=x.Path("styling/default/font/../../lang/misc/mixed-content" a)

 e=x.Path("styling/lang/styles/s32/@*")
 e=e.Path(".." a)
 e=e.Path("../@fs" a)
 e=e.Path("../@*" a)
 e=e.Path("../../?pi1" a)
 e=e.Path("../../../../../!-" a)
 e=e.Path("../../../../![" a)
 e=e.Path("../../../../../?pi2" a)

 e=x.Path("?xml" a)
 e=x.Path("?xml/@encoding" a)
 e=x.Path("?xml/@*" a)
 e=x.Path("?xml/@" a)
 e=x.Path("!DOCTYPE" a)
 e=x.Path("?pi0" a)

 e=x.Path("styling/default/font")
 e=x.Path("styling/") ;;text
 e=x.Path("*" a)
 e=x.Path("*/unicode" a)
 e=x.Path("*/*" a)
 e=x.Path("styling/*" a)
 e=x.Path("styling/lang/*" a)
 e=x.Path("styling/lang/styles/*" a)
 e=x.Path("styling/lang/misc/*" a)
 e=x.Path("styling/lang/misc/mixed-content/*" a)

out "--------"
if(e) out "'%s' '%s'[]---" e.Name e.Value
for i 0 a.len
	e=a[i]
	out "'%s' '%s'" e.Name e.Value

 str s
 x.ToString(s)
 out s
