out
opt end 1
int n=100
str s="aakjskjdk ;Moo jdsdj ;gg shjhdj ;777 dsd"
 rep(6) s+s
 out s
str rx=";(\w+)"
int fl;;=16
int iSub=0
int one=fl&16 or iSub
ARRAY(str) as1 as2
 ARRAY(CHARRANGE) ac1 ac2
ARRAY(POINT) ac1 ac2
int i i1 i2 i3 i4
PF
rep(n) i1=findrx(s rx 0 fl as1 iSub)
PN
rep(n) i2=findrx(s rx 0 fl|4 as2 iSub)
PN
rep(n) i3=findrx(s rx 0 fl ac1 iSub)
PN
rep(n) i4=findrx(s rx 0 fl|4 ac2 iSub)
PN
PO

 ret
out i1; out "%s %s" as1[0] iif(one "" as1[1])
out "---"
out i2; for(i 0 as2.len) out "%s %s" as2[0 i] iif(one "" as2[1 i])
out "---"
out i3; if(one) out "%i %i" ac1[0].x ac1[0].y; else out "%i %i  %i %i" ac1[0].x ac1[0].y ac1[1].x ac1[1].y
out "---"
out i4
if(one) for(i 0 ac2.len) out "%i %i" ac2[0 i].x ac2[0 i].y
else for(i 0 ac2.len) out "%i %i  %i %i" ac2[0 i].x ac2[0 i].y ac2[1 i].x ac2[1 i].y

 speed: 125  247  83  160  
 speed: 185  12225  110  7124  
 10
 ;Moo Moo
 ---
 3
 ;Moo Moo
 ;gg gg
 ;777 777
 ---
 10
 10 14  11 14
 ---
 3
 10 14  11 14
 21 24  22 24
 32 36  33 36
