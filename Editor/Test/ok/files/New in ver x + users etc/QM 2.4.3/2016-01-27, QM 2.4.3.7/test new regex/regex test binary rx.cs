out
str s="one two three"
s[3]=0
out RegexFind(s "two")
FROMTO f.to=s.len
out RegexFind(s "two" 0 0 0 f)

out RegexFind(s "\0two" 0 0 0 f)

out "----"
 IRegex x=CreateRegex("e[0]two")
IRegex x=CreateRegex("e[0]\w+" 0 0 5)
out x.MatchFromTo(s 0 s.len)
out x.Get(0 0 _i); out _i
