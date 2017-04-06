 /Subs help
function [parentParam]
 out 1
 int sub
 out &this
int k=5
int- t=9
 sub.Sub1(10)
 this.sub.Sub1
 out sub.Sub2(2 3)
 Sub1
 this.Sub1
 this.sub.Sub1
 F2
 mac "sub.Timer1"
 mac "sub.Sub1"
 tim 1 sub.Timer1
 tim 1 sub.Sub1
 atend sub.Timer1
 atend sub.Sub1
 out &sub.Timer1
 out &sub.Sub1
sub.Udf(this)

 Subs r
 out r.pro
 out r.priv
 out r.pro_
 out r.priv_

#sub Sub1 cv
function [param]
 Description.

 deb
int local=5
out t
int- tt=4
k=6
int k=7
out "sub"
if(0) end "test error"
 this.F2
 F2
 m=7

#sub Sub2 cv
function# a b
 Description.

out k
ret a+b+m

#sub Timer1
out 1

#sub Udf v
function Subs&r

 deb
out t
t+1
 Subs r
 out r.pro
 out r.priv
 out r.pro_
  out r.priv_
 r.F2
 r.F3
r.sub.Sub1
 r.Sub1
