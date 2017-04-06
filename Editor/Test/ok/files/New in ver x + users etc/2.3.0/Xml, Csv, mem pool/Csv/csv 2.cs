out
ICsv v=CreateCsv
v.Separator=","

str s="one,two,''three,four''[][]''five[]four''"
 str s="one,two,''three,four''"
 Q &q
v.FromString(s)
 Q &qq; outq
 s.all
 s=""
 v.FromString(s 0)

out v.RowCount
out v.ColumnCount

 v.Separator="?"

str ss
v.ToString(ss)
out ss