 /
function IXml&xml [withAttr]

 This function displays all XML nodes and their properties.

 EXAMPLE
 IXml x=CreateXml
 x.FromFile("$my qm$\test.xml")
 XmlOut x 1


lpstr st="root[]el[]a[]text[]xml[]DOC[]PI[]CD[]comm"
ARRAY(str) at=st

ARRAY(IXmlNode) a; int i
xml.Root.GetAll(withAttr!=0 a)

for(i 0 a.len)
	XMLNODE xi; a[i].Properties(&xi)
	out "%-15s %-4s F=0x%X L=%i V='%s'", xi.name, at[xi.xtype], xi.flags, xi.level, xi.value
out "-----"
