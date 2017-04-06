act "Notepad"
spe 0 ;;maximum speed
ARRAY(str) row
foreach row "" FE_ExcelRow
	key "*" T
	row[0].setsel
	key TT
	row[1].setsel
	key T
	row[2].setsel
	key Y
