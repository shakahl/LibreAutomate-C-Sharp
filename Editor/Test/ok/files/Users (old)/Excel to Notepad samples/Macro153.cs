act "Notepad"
spe 0 ;;maximum speed
ARRAY(str) row
foreach row "" FE_ExcelRow
	row[0].setsel ;;gets cell from column A. To get from other columns, use 1, 2, etc.
	key Y
