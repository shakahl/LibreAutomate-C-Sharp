out
ICsv c=CreateCsv(1)

int i j; str s1 s2
 for(i 0 100)
	 for(j 0 7) s1+s2.RandomString(4 4 "0-9a-z"); s1+", "
	 s1.replace("[]" s1.len-2)

s1.getmacro("Macro1425")

c.FromString(s1)

Q &q
c.Sort(4 0)
Q &qq
outq

c.ToString(s2)
out s2
