out
IStringMap m=CreateStringMap

str s=
 one,two
 three,"four
 kkkkkkkkkkk"

 s=
 one
 "two
 three"

s=
 one=two
 three="four
 kkkkkkkkkkk"

 s=
 one
 two

str sep=","
 sep="csv"
sep="csv="

m.AddList(s sep)

str s2
m.GetList(s2 sep)
out s2
