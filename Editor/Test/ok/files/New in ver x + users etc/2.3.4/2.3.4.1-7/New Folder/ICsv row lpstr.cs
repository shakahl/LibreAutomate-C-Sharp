
ICsv icsv1=CreateCsv(1)
str s1=
 a1,b1,c1
 a2,b2,c2
 a3,b3,c3
icsv1.FromString(s1)

int nr=icsv1.RowCount
int nc=icsv1.ColumnCount

ICsv icsv2=CreateCsv(1)

int r c
for r 0 nr
	;;.......
	lpstr ms=icsv1.Cell(r 0) ;;ms points to data in icsv1 memory; rows are stored as multistrings
	;;.......
	icsv2.AddRowMS(-1 nc ms)
	;;.......

 results
str s2; icsv2.ToString(s2); out s2
