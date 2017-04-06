 /
function# $name str&data

str xf="$Common AppData$\GinDi\Quick Macros\HelpIndex\chi.xml"
IXml xml=CreateXml
xml.FromFile(xf)
IXmlNode n=xml.RootElement
data=n.ChildValue(name)
ret 1
err+
data=""
