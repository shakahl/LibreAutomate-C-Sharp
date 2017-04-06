out
Htm el=htm("TABLE" ...)

MSHTML.IHTMLTable2 table=+el
MSHTML.IHTMLElement cell
MSHTML.IHTMLElementCollection cells=table.cells
int i c r ncol nrow
ncol=3 ;;assume you know it
nrow=cells.length/ncol
ARRAY(str) a.create(ncol nrow)
for c 0 ncol
	for r 0 nrow
		cell=cells.item(i)
		a[c r]=cell.innerText
		i+1

 out array
for r 0 a.len(2)
	out "--------- Row %i ---------" r
	for c 0 a.len(1)
		out "--- Column %i ---" c
		out a[c r]
		
