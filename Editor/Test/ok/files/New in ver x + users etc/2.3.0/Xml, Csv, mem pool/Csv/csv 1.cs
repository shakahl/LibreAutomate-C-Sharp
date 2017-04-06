out
ICsv v=CreateCsv

v.Separator=";"

Q &q
v.FromFile("$my qm$\test.csv")
Q &qq; outq

out v.RowCount
out v.ColumnCount

str ss
 v.ToString(ss)
 out ss

v.Separator=","
 v.Separator=""

v.ToFile("$my qm$\test_5.csv" 0)

ss.getfile("$my qm$\test_5.csv")
out ss
