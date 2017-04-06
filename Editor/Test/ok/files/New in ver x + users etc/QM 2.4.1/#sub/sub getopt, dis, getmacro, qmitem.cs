/exe
opt clip 1
 out getopt(clip 6)

sub.Test
 atend sub.Test
 spe 10
 opt clip 0
 sub.Test

 mac "sub.Test"

#sub Test

 out getopt(clip 6)
int i=getopt(itemid)
 dis i

 out __FUNCTION__
 out _s.getmacro(i 1)

 #exe addtextof "<00003>Test"
 out _s.getmacro(i)

out i
out qmitem(i)
out qmitem("<00003>Test")
QMITEM q
if(qmitem(i 0 q 8)) out q.text


 BEGIN PROJECT
 main_function  sub getopt, dis, getmacro, qmitem
 exe_file  $my qm$\sub getopt, dis, getmacro, qmitem.qmm
 flags  6
 guid  {4578F6B6-6D52-42CD-92B8-18DFBAD84411}
 END PROJECT
