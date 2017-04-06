#compile "__Regex"

out
 WakeCPU
str s rx; int fC fM R i
s="aa 7word,bb 8kk , cc"
 rep(2) s+s
rx=" *, *"
PF
IRegex x=CreateRegex(rx)
 err out _error.description; out _error.code; ret
PN

ARRAY(str) as; ARRAY(OFFSETS) ao
 R=x.Split(s as 2 2 ao)
out RegexSplit(s rx as 0 0 ao)
 R=x.SplitFromTo(s 0 s.len/2 as 0 0 ao)
 PN;PO
out R
out "----"
for(i 0 as.len) out F"'{as[i]}'"
out "----"
for(i 0 ao.len) out "%i %i" ao[i].b ao[i].e
out "----"
outb s s.len 1
