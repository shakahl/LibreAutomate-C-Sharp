out
 typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0
typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 6.0

MSXML2.DOMDocument doc._create
 doc.preserveWhiteSpace=-1

MSXML2.IXMLDOMElement e=doc.createElement("root")
doc.appendChild(+e)
MSXML2.IXMLDOMNode e2=doc.createNode(MSXML2.NODE_ELEMENT "nnnn" "")
e.appendChild(e2)
e2.text="text"

str s=doc.xml
out s

 doc.save(_s.expandpath("$my qm$\msxml.xml"))
