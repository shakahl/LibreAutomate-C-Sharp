out
IXml x=CreateXml

str s=
 <root>
 <elem a="p">text</elem>
 </root>

x.FromString(s)

 XmlOut x 1
 
ARRAY(IXmlNode) a
IXmlNode n=x.Root
n.GetAll(1 a)

IXmlNode t=a[0]
out t.Name

 ARRAY(IXmlNode) a.create(2)
 a[0]=x.Root

str ss
x.ToString(ss)
out ss

