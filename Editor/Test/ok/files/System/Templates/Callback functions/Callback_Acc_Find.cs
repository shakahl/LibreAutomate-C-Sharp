 /
function# Acc&a level cbParam

 Callback function for Acc.Find or acc.
 Read more about <help #IDP_ENUMWIN>callback functions</help>.

 a - the found object.
 level - its level in the hierarchy. If class or id is specified, it is level beginning from that child window. When searching in web page (flag 0x2000), it is level from the root object of the web page (DOCUMENT or PANE).
 cbParam - cbParam passed to Acc.Find, or y passed to acc.

 Return:
 0 - stop. Let a will be the found object.
 1 - continue.
 2 - continue, but skip children of a.


 <add your code here>

ret 1

 ________________________

  Example of calling Acc.Find with callback function and user-defined type
 
 int w1=win("Internet Explorer")
 type MYACCTYPE str's ARRAY(Acc)a
 MYACCTYPE y
 y.s="a" ;;get links that contain "a" in text
 Acc a.Find(w1 "LINK" "" "" 0 0 0 "" &sub.Callback_Acc_Find &y)
  results
 int i
 for(i 0 y.a.len) out y.a[i].Name
 
 
 #sub Callback_Acc_Find
 function# Acc&a level MYACCTYPE&y
  Adds all "LINK" objects that contain y.s to y.a.
 
 str s=a.Name
 if(find(s y.s 0 1)>=0) y.a[]=a
 ret 1
