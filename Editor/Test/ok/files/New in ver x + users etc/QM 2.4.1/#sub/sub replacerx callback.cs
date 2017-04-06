 /exe

str s="one two three"
REPLACERX r.frepl=&sub.Repl
s.replacerx("two" r)
out s

#sub Repl
function# REPLACERXCB&x
out x.match
x.match="YYYYY"

 BEGIN PROJECT
 main_function  sub replacerx callback
 exe_file  $my qm$\sub replacerx callback.qmm
 flags  6
 guid  {0CDDE278-F331-43F0-95CD-214925986178}
 END PROJECT
