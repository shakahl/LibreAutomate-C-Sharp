out
 str s="<c ''0xff0000''>simple</c> bbbb"
 str s="<c ''0xff0000''></c> bbbb"
 str s="<c>simple</c> bbbb"
 str s="<c 0xff0000>simple</c> bbbb"
 str s="<c ''0xff0000''>text <c ''0xff00''>color</c> text</c> bbbb"
 str s="<c 0xff0000>text <c ''0xff00''>color</c> text</c> bbbb"
 str s="<c ''0xff0000''><z ''zz''> </z></c> bbbb"
 str s="<c 0xff00>b</c> c</c>"

str s1="<i>A<i>B</i>C</i> more"
str s2="<i>A</i>B</i> more"

 str rx="(?s)^<(color|c|b|i|u|code|help|link|macro|open|tip|fa|_|z|Z)( ''.*?''| \w+)?>((?>.+?|(?R))*)</\1>"
str rx="(?s)^<(color|c|b|i|u|code|help|link|macro|open|tip|fa|_|z|Z)( ''.*?''| \w+)?>((?>.+?|(?R))*?)</\1>"

str rcx; findrx "" rx 0 128 rcx
ARRAY(str) a1 a2
 ARRAY(CHARRANGE) a
Q &q
findrx(s1 rcx 0 0 a1)
Q &qq
findrx(s2 rcx 0 0 a2)
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
