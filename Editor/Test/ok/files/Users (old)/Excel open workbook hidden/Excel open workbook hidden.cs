str f.expandpath("$personal$\book1.xls")
Excel.Application app._create
Excel.Workbook wb=app.Workbooks.Open(f)
Excel.Worksheet ws=wb.ActiveSheet
str s=ws.Range("A2").Value
out s
s+"A"
ws.Range("A2").Value=s

wb.Save
app.Quit

err+
	app.Quit
	end _error

 All excel variables should be destroyed or set to 0.
 Then you can open the file in Excel.
 It also closes the hidden excel process started by this macro.
 Instead of setting all variables to 0, you can move the above code to a separate function. If all variables are local, they are destroyed when the function ends.
ws=0
wb=0
app=0

15 ;;try to open the file in Excel now
