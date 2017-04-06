 /
function ARRAY(str)&a

 Stores Excel cells into two-dimensional array.

 EXAMPLE
 ARRAY(str) a
 ExcelSheetToArray a
 int r c
 for r 0 a.len
	 out "-----Row %i-----" r+1
	 for c 0 a.len(1)
		 out a[c r]


typelib Excel {00020813-0000-0000-C000-000000000046} 1.2

Excel.Application xlApp._getactive; err act; act; xlApp._getactive
Excel.Worksheet ws=xlApp.ActiveSheet
Excel.Range r=ws.UsedRange
int i j nr(r.Rows.Count) nc(r.Columns.Count)
a.create(nc nr)
for i 0 nr
	for j 0 nc
		a[j i]=r.Item(i+1 j+1)
