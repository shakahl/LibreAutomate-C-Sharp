out
ExcelSheet es.Init

  for each row, display cells in columns A, B and C
 ARRAY(str) a
 foreach a es FE_ExcelSheet_Row "A:C"
	 out "%s, %s, %s" a[0] a[1] a[2]

  display all sheet
 ARRAY(str) a; int r c
 foreach a es FE_ExcelSheet_Row
	 r+1; out "---- row %i ----" r
	 for c 0 a.len
		 out a[c]
