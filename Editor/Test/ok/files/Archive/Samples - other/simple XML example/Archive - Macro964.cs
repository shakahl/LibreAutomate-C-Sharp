 this macro shows how to get data from the XML file

str xmlfile="$desktop$\pa.xml"
if(!dir(xmlfile)) ret

IXml xml=CreateXml
xml.FromFile(xmlfile)
ARRAY(IXmlNode) a
xml.Path("payments/i" a) ;;populates array a with all i tags
int i
for i 0 a.len
	IXmlNode& n=a[i]
	str s1=n.ChildValue("date")
	str s2=n.ChildValue("x")
	out s1
	out s2
	out "------"
