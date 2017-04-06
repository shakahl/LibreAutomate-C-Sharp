function [str&tag] [str&text] [int&nodeType] [int&numChildren]

 Gets basic properties of this node.

 tag, text, nodeType, numChildren - variables that receive the properties. Can be 0.


if(!node) end ERR_INIT
FFNODEINFO n
node.get_nodeInfo(n.bName n._NamespaceId n.bValue n.numChildren n._EventId n.nodeType)

if(&tag) tag=n.bName
if(&text) text=n.bValue
if(&nodeType) nodeType=n.nodeType
if(&numChildren) numChildren=n.numChildren

err+ end ERR_FAILED
