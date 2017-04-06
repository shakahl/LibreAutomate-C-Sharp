out
#compile "__Regex"
str s rx; int fC fM R
 rx="(?m)^"
 rx="(?<=.)"
rx="(?=.)"
 s="[]"
s="abc"

Regex x.Compile(rx)

 if x.Match(s)
	 out x.Get(0 _i); out _i

 out x.MatchAll(s)

out x.Replace(s "," 0x100)
out s
