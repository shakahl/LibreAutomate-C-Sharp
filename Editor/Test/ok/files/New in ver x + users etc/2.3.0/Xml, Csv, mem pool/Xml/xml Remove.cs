out
IXml x=CreateXml
IXmlNode e
x.FromFile("$my qm$\test.xml")

ARRAY(IXmlNode) a
int i
for i 0 10
	x.Root.GetAll(1 a)
	if(!a.len) break ;;already deleted all from root
	e=a[RandomInt(0 a.len-1)]
	XmlOutItem e
	x.Delete(e)

out "-----------"
 ret
 XmlOut x 1
 
str s
x.ToString(s)
out s
