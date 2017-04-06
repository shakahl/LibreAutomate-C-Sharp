out
int w=win("Mozilla Firefox" "MozillaWindowClass")
ARRAY(FFNode) a
FFNode n.FindFF(w "span" "" "" 0 0 0 &sub.Callback_Acc_FindFF &a)

 results
int i
for i 0 a.len
	out "------"
	 out a[i].HTML(0)
	 out a[i].HTML(1)
	n=a[i].node.firstChild ;;<span> -> text
	str text=n.Text
	out text
	Acc m.FromFFNode(n)
	m.Mouse(0)
	0.3



#sub Callback_Acc_FindFF
function# FFNode&x level ARRAY(FFNode)&a FFNODEINFO&ni

 Callback function for Acc.FindFF or FFNode.FindFF.

 x - current node.
 level - level of x in the tree. It will be 0 for the root node, 1 for its children, 2 for their children, and so on. By default, the root is the #document node of web content; its children are such nodes as DOCTYPE and HTML; HTML childeren are HEAD and BODY.
 cbParam - cbParam passed to FindFF. Can be declared as reference to any type.
 ni - node info, as retrieved from FFDOM.ISimpleDOMNode.get_nodeInfo(). To use the strings easier, assign them to str variables, because they are BSTR.

 Return:
 0 - stop. Let x will be the found node.
 1 - continue.
 2 - continue, but skip children of x.

a[]=x
ret 1
