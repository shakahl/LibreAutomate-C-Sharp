str f.expandpath("$personal$\book1.xls")
Excel.Workbook wb._getfile(f) ;;open file in background
Excel.Worksheet ws=wb.ActiveSheet
str s=ws.Range("A2").Value
out s
s+"A"
ws.Range("A2").Value=s

wb.Windows.Item(1).Visible=TRUE ;;without this the workbook would be hidden in Excel
wb.Save
wb.Application.Quit

ws=0
wb=0

15 ;;try to open the file in Excel now
