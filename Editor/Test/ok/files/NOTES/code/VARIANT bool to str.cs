 When converting VARIANT(bool) to str, should be option to convert to True instead of -1.

ExcelSheet es.Init
Excel.Range ra=es.ws.Application.Selection
ra.NumberFormat="General"
ARRAY(VARIANT) a
a=ra.Value
 ra.Value=a

int r c
for r a.lbound(1) a.ubound(1)+1
	for c a.lbound(2) a.ubound(2)+1
		out a[r c]
		