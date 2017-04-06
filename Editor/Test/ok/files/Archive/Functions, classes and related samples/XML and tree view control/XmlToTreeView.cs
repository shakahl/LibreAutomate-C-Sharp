 /
function $xml htv [htvi]

 Adds tree view control items from XML string.

 xml - XML string.
  All nodes must have "t" attribute with item text.
  Node names can be any.
  Ignores the root node.
 htv - control handle.
 htvi - parent item handle. Can be 0.


IXml x=CreateXml
x.FromString(xml)

IXmlNode n=x.RootElement
__XmlToTreeView htv htvi n

err+ end _error
