out
str s rx
s="aa 7word bb 8kk cc"
 rx="(\d)(\w+)"
rx="(?<one>\d)(?<two>\w+)"
IRegex y=CreateRegex(rx)
y.ReplaceCallback(s &sub.Callback 8)
out s


#sub Callback
function# cbParam str&match REGEXREPLACECB&x



out match
match.ucase; match-"<<"; match+">>"
 match=x.repl

 out x.x.GetStr(2 _s); out _s
 int i1 i2 k1 k2; i1=x.x.Get(1 k1); i2=x.x.Get(2 k2); match.fromn(x.s+i2 k2 x.s+i1 k1)
 int i1 i2 k1 k2; i1=x.x.GetByName("one" k1); i2=x.x.GetByName("two" k2); match.fromn(x.s+i2 k2 x.s+i1 k1)

 x.t.addline(match)
 ret 1

 ret -1
 ret -2
 if(x.number=2) ret -1
 ret -100
