 /exe
 inp- _s "" "" "" 0 "" sub.InpCb
 out _s

ARRAY(str) a="fsdfsdf[]kjghkjsh[]gsf[]bxnbc[]ghds"
a.sort(0 sub.Sort)
out a

#sub InpCb
function# str&s
out s
 ret 1

#sub Sort
function# param str&a str&b

if(a.len<b.len) ret -1
if(a.len>b.len) ret 1

 BEGIN PROJECT
 main_function  sub inp, sort
 exe_file  $my qm$\sub inp, sort.qmm
 flags  6
 guid  {3C81DD11-25FF-4EC2-85B8-867870E42315}
 END PROJECT
