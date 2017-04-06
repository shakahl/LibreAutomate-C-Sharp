str filename="$temp$\data.csv"
filename.expandpath

str s=
 1,=SUM(A1+1)
 2,=A2+1
 note: must not be spaces between separator and =
s.setfile(filename)
 run "excel" F"''{filename}''"; ret

ExcelSheet es.Init("" 8 filename)
filename.timeformat("$desktop$\{yyyy}-{MM}.xls")
filename.expandpath
del- filename; err
 es.Save(filename) ;;does not change file format
es.ws.Application.ActiveWorkbook.SaveAs(filename Excel.xlNormal @ @ @ @ 1)

es.ws.Application.Quit
run filename
