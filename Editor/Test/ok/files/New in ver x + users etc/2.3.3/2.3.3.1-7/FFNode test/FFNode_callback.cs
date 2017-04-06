 /FFNode test
 function# FFDOM.ISimpleDOMNode&x level &ntags FFNODEINFO&ni
function# FFNode&x level &ntags FFNODEINFO&ni

 out ni.numChildren
 x.Text
 out x.HTML
 out level
if(&ntags) ntags+1

 BSTR bn bv; int nc uid; word nsid nt
 x.node.get_nodeInfo(&bn &nsid &bv &nc &uid &nt)

ret 1
