out

str s=
 Hello,
 
 I have a text in a string, like 400 characters, i want this splitted in 3 strings of 150 characters. So i have finally 2 strings of 150 characters and 1 of 100.
 And if it's possible, the split must be done before the whole word (a space character) so the length of a string can vary but never more than 150 characters.
 
 i don't know how to do this, is somebody here who can help me out?

int x=150
ARRAY(str) a
 if(!findrx(s _s.format("(?s)(.{1,%i})(?:\s+|$)" x) 0 4 a)) end "failed"
if(!findrx(s F"(?s)(.{{1,{x}})(?:\s+|$)" 0 4 a)) end "failed"
int i
for i 0 a.len
	out a[1 i].len
	out a[1 i]
