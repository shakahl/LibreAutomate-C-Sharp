Acc ac=acc(mouse)
str url=ac.Value
 out url
HtmlDoc d.InitFromWeb(url)
str s=d.GetText
 out s

ARRAY(str) a
if(!findrx(s "^(\d+)\s(\S+)" 0 4|8 a)) end "failed"
if(a.len<100) end F"Extracted {a.len} words"

s=""
int i
for i 0 a.len
	 out "%s %s" a[1 i] a[2 i]
	s.addline(a[2 i])

s.setfile("$desktop$\words.txt" -1)

OnScreenDisplay F"Extracted {a.len} words" 1
