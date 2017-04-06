out
str s1="<i>A<i>B</i>C</i> more"
str s2="<i>A</i>B</i> more"

 str rx="(?s)^<(color|c|b|i|u|code|help|link|macro|open|tip|fa|_|z|Z)( ''.*?''| \w+)?>((?>.+?|(?R))*)</\1>"
 str rx="(?s)^<(color|c|b|i|u|code|help|link|macro|open|tip|fa|_|z|Z)( ''.*?''| \w+)?>((?>.+?(?C1)|(?C2)(?R))*?)</\1>"
 str rx="(?s)^<(color|c|b|i|u|code|help|link|macro|open|tip|fa|_|z|Z)( ''.*?''| \w+)?>((?>.+?(?C1)|(?C2)(?R))*)</\1>"
 str rx="(?s)^<(i)>((?>.+?|(?R))*)</\1>"
 str rx="(?s)^(<(i)>(.+)(</\1>|(?R)"
str rx="(?s)(<(i)>([^<]+|<(?!(?R)))</\2>)"
 <div\b[^>]*>(?:(?> [^<]+ ) |<(?!div\b[^>]*>))*?</div>

FINDRX f.fcallout=&callout3

str rcx; findrx "" rx 0 128 rcx
ARRAY(str) a1 a2
 ARRAY(CHARRANGE) a
Q &q
findrx(s1 rcx f 0 a1)
Q &qq
 findrx(s2 rcx f 0 a2)
Q &qqq; outq

int i
for i 0 a1.len
	out a1[i]
	 out a1[i].cpMin
	break
 out "----------"
for i 0 a2.len
	out a2[i]
	 out a2[i].cpMin
	break
