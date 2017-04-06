function IXml&xml [$defaultXML]

 Stores file data to xml.


str s
ToStr(s)
if(!s.len) s=defaultXML
xml.FromString(s)
err+ end _error
