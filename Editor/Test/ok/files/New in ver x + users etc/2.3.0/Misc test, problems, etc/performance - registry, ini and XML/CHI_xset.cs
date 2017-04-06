 /
function# $name $data

str xf="$Common AppData$\GinDi\Quick Macros\HelpIndex\chi.xml"
if(!dir(xf)) _s="<chi></chi>"; _s.setfile(xf)
IXml xml=CreateXml
xml.FromFile(xf); err ret
IXmlNode n=xml.RootElement
n.SetChild(name data)
xml.ToFile(xf)
ret 1
err+
