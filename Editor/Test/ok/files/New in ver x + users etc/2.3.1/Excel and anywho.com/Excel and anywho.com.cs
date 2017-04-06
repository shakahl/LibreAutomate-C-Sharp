 /exe 1
out

 define variable type for search results, and create variable that will be populated by anywho_find
type ANYWHORESULT ~lastName ~firstName ~city ~state ~_zip
ARRAY(ANYWHORESULT) ar

 connect to Excel
ExcelSheet es.Init("Sheet1")
ExcelSheet es2.Init("Sheet2")

 get Sheet1 into array a
ARRAY(str) a
es.GetCells(a)

 for each row
int row col row2(2) i
for row 1 a.len ;;change 1 to 0 if need to include first row
	 out "-------------"
	 for(col 0 a.len(1)) out a[col row]
	
	 searches for the first contact. Would need some more code to repeat for all contacts in row.
	anywho_find ar a[1 row] a[2 row] a[3 row] a[4 row] a[5 row]
	
	 now all results are in ar. Write to Sheet2. Writes each result in separate row.
	for i 0 ar.len
		es2.SetCell(a[0 row] 1 row2) ;;PIN
		ANYWHORESULT& r=ar[i]
		es2.SetCell(r.lastName 2 row2)
		es2.SetCell(r.firstName 3 row2)
		es2.SetCell(r.city 4 row2)
		es2.SetCell(r.state 5 row2)
		es2.SetCell(r._zip 6 row2)
		row2+1

out "Finished"
