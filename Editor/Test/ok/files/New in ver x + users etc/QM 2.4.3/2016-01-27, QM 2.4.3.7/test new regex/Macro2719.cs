out
str s rx; int fC fM R
s="aa 7word bb 8kk cc"
 rep(2) s+s
 out s.len
rx="\d(?<na>\w+)"
str m
foreach m s FE_Regex rx +"na"
	out m
