 connect to Excel
ExcelSheet es.Init

 get columns B, C and D from Excel. Use other string if need other range. For reference, click CellsToArray and press F1.
ARRAY(str) a
es.CellsToArray(a "B:D")

 repeat for each row
int r
for r 0 a.len
	 show message box that allows to continue macro (Yes), or skip current row (No), or end macro (Cancel)
	sel mes(F"Navigate and paste this value?[][]{a[0 r]}" "" "YNC")
		case 'N' continue
		case 'C' ret
	
	 navigate
	mou 1990 990
	lef
	wait 2
	mou 1970 265
	lef
	wait 5
	mou 3640 155
	lef
	wait 2
	
	 paste
	paste a[0 r]
	  ...
	  example pasting more columns of the specified range
	 paste a[1 r]
	 ...
	 paste a[2 r]
