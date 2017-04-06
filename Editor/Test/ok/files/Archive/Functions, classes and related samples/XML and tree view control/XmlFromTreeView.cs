 /
function str&xml htv [htvi]

 Gets tree view control items to XML string.

 xml - variable that receives XML.
 htv - control handle.
 htvi - parent item handle. Can be 0.


IXml x=CreateXml
IXmlNode n=x.Add("x")
__XmlFromTreeView htv htvi n
x.ToString(xml)

err+ end _error
