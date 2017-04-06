str s=
 <x>
 <string name="Path" value="X:\AUDIO PROJECTS\TestFolder\"/>
 <z>
 <string name="Path" value="X:\AUDIO PROJECTS\TestFolder2\"/>
 </z>
 </x>

IXml x._create
x.FromString(s)

ARRAY(IXmlNode) a; int i
x.RootElement.GetAll(0 a)
for i 0 a.len
	IXmlNode n=a[i]
	if(StrCompare(n.Name "string")) continue
	s=n.AttributeValue("value")
	if(!s.begi("X:\")) continue
	s.findreplace("\" "/")
	s.replace("/Volumes/AUDIO SSD" 0 2)
	n.Attribute("value").Value=s

x.ToString(_s); out _s
