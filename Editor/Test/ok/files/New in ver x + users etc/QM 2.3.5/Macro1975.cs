ICsv c1._create c2._create
c1.FromFile("file1")
int i; str s0
for i 0 c1.RowCount
	s0=c1.Cell(i 0)
	out s0
	c2.AddRowSA(-1 1 &s0)
c2.ToFile("file2")
