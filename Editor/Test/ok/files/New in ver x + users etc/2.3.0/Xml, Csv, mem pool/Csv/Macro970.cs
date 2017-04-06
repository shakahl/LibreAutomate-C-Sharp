out
ICsv c=CreateCsv
c.Separator=","
str s=
 one,   two
 three,   four, five, six, seven, eith, ten
 
c.FromString(s)
 c.RemoveRow(0)

str ss
c.ToString(ss)
out ss
