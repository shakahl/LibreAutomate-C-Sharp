out

str html=
 03/03/2010 08:39:13 Updated by BOND, JAMES
 This is my update.
 Thanks.
 
 02/03/2010 12:53:14 Updated by Dr. No
 And this is my update..
 
 02/03/2010 12:21:11 Updated by Oddjob
 YAZZAH!

str pattern="(?ms)^[0-9]{2}\/[0-9]{2}\/[0-9]{4} (.*?)(?=[][]\d\d/\d\d/\d\d\d\d|\Z)"

ARRAY(str) a
findrx(html pattern 0 4 a)

int i
for(i 0 a.len)
	out "---- updated ----[]%s" a[1 i]
