 /
function~ column [row] [column2] [row2]

 Converts Excel column/row index to "A1" or "A1:B1" string.

 column - 1-based column index.
 row - 1-based row index.
 column2, row2 - if used, creates string like "A1:B2", where B2 is from column2/row2.

 REMARKS
 Does not add omitted and 0 arguments to the string. For example, you can create strings like "A", "3:3", "C:E".

 See also: <Excel range strings>.
 Added in: QM 2.3.3.


rep
	if(column<=0) break
	column-1
	_s-" "; _s[0]='A'+(column%26)
	column/26

if(row) _s+row

if(column2 or row2) _s+":"; _s+ExcelRange(column2 row2)

ret _s
