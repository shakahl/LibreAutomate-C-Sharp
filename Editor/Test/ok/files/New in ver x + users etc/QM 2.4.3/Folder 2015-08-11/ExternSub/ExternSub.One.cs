 /ExternSub help
 out
 out 1
 deb
 Two

 deb
int u
 sub.OneSub()
 Function305
sub.OneSub()
subs.ThreeSub()
 subs.ThreeSub()
 subs.Sub4
 subEtc.InEtc()
 sub_A.Moko()
 sub_A.Doko
 subs.Dialog; ret

 mac "sub.LocalThread"; 1
 mac "subEtc.Thread3"; 1
 mac "sub_A.Doko"; 1
 mac "sub.OneSub"; 1 ;;error
 mac "subEtc.InEtc"; 1 ;;error
 #compile "sub.OneSub"
 #compile "subEtc.Thread3"
 #compile "sub_A.Moko"

 int+ g_set
  #set g_set 5
  #set g_set sub.Set
 #set g_set sub_A.Set
 out g_set
 ret

 sub.Sub4
 sub.Static
 ExternSub k; k.sub.ThreeSub

  Two
  sub.TwoSub
  #compile "ExternSub.__sub"


 ExcelSheet es.Init
 Excel.Application ap=es.ws.Application
 ap._setevents("sub.ap")
 mes 1


#sub OneSub c
function# [param] ;;comm
 Annotation of OneSub.

out 11
if(0) end "aaa"; sub.Errors
 out "before"
 sub.Move


 #sub ap_SheetSelectionChange
 function IDispatch'Sh Excel.Range'Target ;;Excel._Application'ap
 out 1


#sub Static
ExternSub k
k.sub.OneSub
k.subEtc.InEtc
 sub.OneSub ;;error
 subEtc.InEtc ;;error
 k.sub.LocalThread ;;error
 k.subEtc.Thread3 ;;error
 k.sub_A.Moko ;;error

 #sub Move
 out "move"


#sub LocalThread
mes 1


#sub Errors
act "ljkjh"


#sub Set
ret 3
