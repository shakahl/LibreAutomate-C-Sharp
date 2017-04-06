 out
ARRAY(str) a
ExcelSheet es.Init
int r c
str s

Q &q
es.GetCells(a "")
Q &qq; outq
for r 0 a.len
	s.formata("-----Row %i-----[]" r+1)
	for c 0 a.len(1)
		s.addline(a[c r])
s.setmacro("GetCells (old)")
s.all

Q &q
es.CellsToArray(a "")
Q &qq; outq
for r 0 a.len
	s.formata("-----Row %i-----[]" r+1)
	for c 0 a.len(1)
		s.addline(a[c r])
s.setmacro("CellsToArray (new)")
