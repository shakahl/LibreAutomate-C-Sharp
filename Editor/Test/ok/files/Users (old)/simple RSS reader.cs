 download xml to AJNR_RSS
str AJNR_RSS
IntGetFile "http://www.ajnr.org/rss/mfc.xml" AJNR_RSS
 out AJNR_RSS
 out

 load into IXml
IXml x=CreateXml
x.FromString(AJNR_RSS)

 get all <item> nodes
ARRAY(IXmlNode) a
x.Path("rdf:RDF/item" a)

 extract <title> and other nodes and format html page in s
int i
str s title link descr
for i 0 a.len
	IXmlNode& n=a[i]
	title=n.Child("title").ChildValue("![")
	link=n.ChildValue("link")
	descr=n.Child("description").ChildValue("![")
	s.formata("<h3><a href=''%s''>%s</a></h3><p>%s</p><hr>[]" link title descr)

s-"<html><head></head><body>[]"
s+"</body></html>"
 out s

 save and run
str temp="$temp$\rss.htm"
s.setfile(temp)
run temp
