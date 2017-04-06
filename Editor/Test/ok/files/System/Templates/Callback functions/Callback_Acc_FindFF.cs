 /
function# FFNode&x level cbParam FFNODEINFO&ni

 Callback function for Acc.FindFF or FFNode.FindFF.

 x - current node.
 level - level of x in the tree. It will be 0 for the root node, 1 for its children, 2 for their children, and so on. By default, the root is the #document node of web content; its children are such nodes as DOCTYPE and HTML; HTML childeren are HEAD and BODY.
 cbParam - cbParam passed to FindFF. Can be declared as reference to any type.
 ni - node info, as retrieved from FFDOM.ISimpleDOMNode.get_nodeInfo(). To use the strings easier, assign them to str variables, because they are BSTR.

 Return:
 0 - stop. Let x will be the found node.
 1 - continue.
 2 - continue, but skip children of x.


 <add your code here>

ret 1

 ________________________

  Example of calling FindFF with callback function and user-defined type
 
 int w1=win("Firefox")
 type MYFFTYPE str's ARRAY(FFNode)a
 MYFFTYPE y
 y.s="m" ;;find links containing "m" in HTML
 FFNode fn.FindFF(w1 "A" "" "" 0 0 0 &sub.Callback_Acc_FindFF &y)
  results
 int i
 for(i 0 y.a.len) out y.a[i].HTML
 
 
 #sub Callback_Acc_FindFF
 function# FFNode&x level MYFFTYPE&y FFNODEINFO&ni
  Adds all "A" nodes that contain y.s to y.a.
 
 str s=x.HTML
 if(find(s y.s 0 1)>=0) y.a[]=x
 ret 1
