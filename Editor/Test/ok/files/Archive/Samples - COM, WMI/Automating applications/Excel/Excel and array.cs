 start Excel and create worksheet
Excel.Application xlApp._create
Excel.Worksheet xlSheet=xlApp.Workbooks.Add(Excel.xlWBATWorksheet).ActiveSheet ;;add workbook and get worksheet
 get cells
Excel.Range cells=xlSheet.Range("b1:f10")
 create array and set cell values
ARRAY(VARIANT) a.createlb(10 1 5 1)
int x y
for(x 1 a.ubound(1)+1)
	for(y 1 a.ubound(2)+1)
		a[x y]=x*y
cells.Value=a
 make visible
xlApp.Visible = 1
xlApp.UserControl=1
