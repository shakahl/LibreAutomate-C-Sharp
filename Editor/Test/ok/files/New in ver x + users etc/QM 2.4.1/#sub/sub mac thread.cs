 /exe
 mac "sub.Test"

 out F"<{getopt(itemid)%%05i}>Test"
 mac qmitem(F"<{getopt(itemid)%%05i}>Test")

 int subId=sub.Test(1)
 mac subId

 sub.Test(1) ;;make exe: QM crashes if 0 parameters (error)
 mac sub.Test "" 0
mac "sub.Test" "" 0

1
#sub Test
function# getSubId
if(getSubId) ret getopt(itemid)
mes getSubId

 BEGIN PROJECT
 main_function  sub mac thread
 exe_file  $my qm$\sub mac thread.qmm
 flags  6
 guid  {F438BFD2-81D0-4F08-BDF9-124C184DB50D}
 END PROJECT
