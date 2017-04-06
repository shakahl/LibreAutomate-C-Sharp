 /
function# IXmlNode&xn

ARRAY(int) a; int i
IXml xml=xn.XmlDoc
IXmlNode r=xml.Root
r.GetAll(1 a)
for(i 0 a.len) if(xn=a[i]) ret 1
