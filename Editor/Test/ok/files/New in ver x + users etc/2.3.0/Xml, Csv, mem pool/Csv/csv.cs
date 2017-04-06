out
str s ss
ICsv v=CreateCsv
v.Separator=";"
v.FromFile("$my qm$\test.csv")

int nr=v.RowCount
int nc=v.ColumnCount
int r c
for r 0 nr
 for r 0 2
	out "--- row %i ---" r
	for c 0 nc
		s=v.Cell(r c)
		out s
		s.trim
		 s+s
		 if(s.len>2) s.fix(s.len/2)
		v.Cell(r c)=s

v.ToString(ss)
out ss
