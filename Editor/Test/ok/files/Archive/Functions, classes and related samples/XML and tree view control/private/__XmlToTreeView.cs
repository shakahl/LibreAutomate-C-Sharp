 /
function htv htvi IXmlNode&n

ARRAY(IXmlNode) a
n.GetAll(2 a)
int i htvi2
for i 0 a.len
	IXmlNode& r=a[i]
	htvi2=TvAdd(htv htvi r.AttributeValue("t"))
	__XmlToTreeView htv htvi2 r

err+ end _error
