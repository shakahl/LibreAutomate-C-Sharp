str s="a ąčę"
IRegex x=CreateRegex("." PCRE2_UTF)
x.ReplaceAll(s "M")
 x.Match(s -1)
out s
