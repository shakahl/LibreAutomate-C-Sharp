 start Excel and create worksheet
Excel.Application xlApp._create
Excel.Worksheet xlSheet=xlApp.Workbooks.Add(Excel.xlWBATWorksheet).ActiveSheet ;;add workbook and get worksheet
 get cells
Excel.Range allcells=xlSheet.Cells
Excel.Range cell1=+allcells.Item(1 1)
Excel.Range cell2=+allcells.Item(2 1)
Excel.Range cell3=+allcells.Item(3 1)
 set cell values and calculate
cell1.Value=5
cell2.Value=2
cell3.Formula="=R1C1 + R2C1"
out cell3.Value
 make visible, wait, save and exit
xlApp.Visible = 1
3
xlApp.DisplayAlerts=0
xlSheet.SaveAs(_s.expandpath("$desktop$\abc.xls"))
xlApp.Quit
