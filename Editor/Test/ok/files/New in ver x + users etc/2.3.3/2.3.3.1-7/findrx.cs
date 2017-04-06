str s=
 01/01/2010 00:10 (A1A):
 ==Info==
 Text I'd like
 ==Info==
 01/01/2010 00:00 (A1A):
 ==Info==
 Text I'd like 2
 Text I'd like 2
 ==Info==
 02/01/2010 02:00 (A2B):
 ==Info==
 Text I'd like 3
 Text I'd like 3
 Text I'd like 3
 ==Info==

ARRAY(str) a
if(!findrx(s "^(\d\d/\d\d/\d{4} \d\d:\d\d) \(.+?\):[]==Info==[](?s)(.+?)[]==Info==" 0 4|8 a)) end "failed. Either s is empty, or the format changed."

 results
out
int i
for i 0 a.len
	out "<><color ''0xff0000''>%s</color>[]<color ''0x8000''>%s</color>" a[1 i] a[2 i]
