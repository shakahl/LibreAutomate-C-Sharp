act "Notepad"
spe 0
ARRAY(str) row
int col
foreach row "" FE_ExcelRow
	for col 0 row.len
		row[col].setsel
		key T
	key Y
