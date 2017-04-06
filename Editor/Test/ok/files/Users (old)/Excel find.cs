ClearOutput

str s="33" ;;search for "33"

ExcelSheet es.Init
ARRAY(str) a
int sheet row col found
int nsheets=es.ws.Application.Worksheets.Count
for(sheet 1 nsheets+1)
	es.Init(sheet)
	es.GetCells(a)
	for row 0 a.len(2)
		for col 0 a.len(1)
			 out a[col row]
			if(a[col row]=s) found=1; goto g1
 g1
if(!found) mes "not found"; ret
mes "sheet %i, row %i, column %c" "" "i" sheet row+1 'A'+col

act "Excel"
es.ws.Activate
Excel.Range cell=es.ws.Cells.Item(row+1 col+1)
cell.Select
