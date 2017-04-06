ExcelSheet es.Init ;;connect to Excel
ARRAY(str) a; es.CellsToArray(a "<all>")




 delete starting from bottom
int r c nc=a.len(1)
for r a.len-1 -1 -1
	for(c 0 nc) if(a[c r].len) break
	if(c<nc) continue
	Excel.Range R=es.ws.Rows.Item(r+1)
	R.Delete(Excel.xlUp)
