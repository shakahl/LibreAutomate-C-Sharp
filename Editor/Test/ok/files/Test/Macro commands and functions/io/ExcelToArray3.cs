ClearOutput

ARRAY(str) a
 ExcelToArray a
 ExcelToArray a "" 1
 ExcelToArray a "2:2"
 ExcelToArray a "A:B"
ExcelToArray a "A1:B2"
 ExcelToArray2 a "sel"
int r c
for r 0 a.len
	out "-----Row %i-----" r+1
	for c 0 a.len(1)
		out a[c r]
