out

 this code is just to create 2 2-dim arrays
ExcelSheet es1.Init("Sheet1")
ExcelSheet es2.Init("Sheet2")
ARRAY(str) a1 a2 a3
int i
es1.GetCells(a1)
es2.GetCells(a2)
 out "%i %i" a1.len(1) a1.len(2)
 out "%i %i" a2.len(1) a2.len(2)
 ____________________________

 combine: a3 = a1 + a2
 assuming that a1 and a2 have same number of columns and rows
a3.create(a1.len(1) a1.len(2))
int r c
for r 0 a1.len(2)
	for c 0 a1.len(1)
		a3[c r].from(a1[c r] a2[c r])
 _____________________________

 results
for r 0 a3.len(2)
	out "---- row %i ----" r
	for c 0 a3.len(1)
		out a3[c r]
