 typelib Excel5 "C:\Documents and Settings\G\Desktop\XL5EN32.OLB"
typelib Excel5 "C:\MSOffice\Excel\XL5EN32.OLB"

Excel5.Application a._getactive("Excel.Application")
Excel5.Worksheet ws=a.ActiveSheet
Excel5.Range r=ws.UsedRange
int i j
for i 1 3
	for j 1 3
		out r.Cells(i j)


 Excel5._ExcelApplication a._getactive
 Excel5._ExcelApplication a._create
 Excel.Application aa=a.Application
 aa.Visible=1
