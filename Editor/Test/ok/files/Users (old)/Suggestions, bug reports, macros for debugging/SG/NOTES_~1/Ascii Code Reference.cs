int i
str s ts
 Handy reference for those struggling with how to handle ASCII codes in Qm
 See also notes at the end

 1)	Output

for(i 32 128)				out "%i %c" i i


 2)	Set

s = " "
for(i 128 256)	s.set(i);	out "%i %s" i s


 3)	String array

s="abcdefghijklmnopqrstuvwxyz1234567890"
out s
for(i 0 len(s))				out "%i %s" s[i] ts.get(s i 1)


 4)	String constant

s="[168]";					out s
 note: CANT be a variable inside []. SHOOT! How to easily do it then?


 
 Also: In Documentation: in the ASCII chart, the strings for codes
 128-159 are not listed
