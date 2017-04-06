out
str slist=
 aaa123456789
 bbb\ccc
 bbb\ddd
 eee\fff
 eee\zzz\ggg
 +hhh
 bbb\iii
 eee\yyy
 +kkk\lll
 eee\zzz\jjj
 +kkk\mmm
 eee\zzz\xxx
 bbb\uuu
 eee\zzz\ggg\vvv

 aaa
 bbb\ccc
 bbb\ddd
 eee\fff
 eee\fff\ggg
 +hhh
 bbb\iii
 eee\fff\jjj
 +kkk\lll
 +kkk\mmm

 eee\zzz\ggg
 +hhh
 eee\zzz\jjj

dll "qm.exe" __SMT_Table2 str*as ns
ARRAY(str) as=slist
__SMT_Table2(&as[0] as.len)
