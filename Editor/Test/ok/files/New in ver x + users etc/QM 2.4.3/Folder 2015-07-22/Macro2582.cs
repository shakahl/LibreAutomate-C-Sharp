ARRAY(str) master.create(1 3); master[0 0]="zero"; master[0 1]="one"; master[0 2]="two" ;;for testing

 find cell in column G that is not in master array
ExcelSheet es.Init
ARRAY(str) a
es.CellsToArray(a "G:G")
int i j
for i 0 a.len
	str& r=a[0 i]
	if(!r.len) continue
	for(j 0 master.len) if(r=master[0 j]) break
	if(j=master.len) break ;;r not found in master
if(i=a.len) out "all G cells exist in master"; ret

 get column B text in the found row
str B=es.Cell("B" i+1) ;;+1 because this func uses 1-based index
out B
 replace with correct data
es.SetCell("correct data" "B" i+1)
