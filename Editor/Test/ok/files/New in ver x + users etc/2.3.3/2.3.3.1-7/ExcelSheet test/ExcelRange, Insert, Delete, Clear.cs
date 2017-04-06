ExcelSheet es.Init
 es.Save
 es.ws.Columns.Insert
 es.ws.Columns.Insert

 out ExcelRange(3 4)
 out ExcelRange(3 0)
 out ExcelRange(0 4)
 out ExcelRange(3 4 5 6)
 out ExcelRange(3 0 5 0)
 out ExcelRange(0 4 0 6)
 es.ws.Range(ExcelRange(0 4 0 6)).Select

 es.Insert("3:3")
 es.Insert("C:C")
 es.Insert("3:7")
 es.Insert("C:E")
 es.Insert("sel")
 es.Insert("D2:D4")
 es.Insert("sel" 1)
 es.Insert(ExcelRow(10))

 es.Delete("4:4")
 es.Delete("C:C")
 es.Delete("15:100")

 es.Clear("sel")
 es.Clear
 es.Clear("sel" 1)
 es.Clear("sel" 4)

 es.ApplyStyle("D13" "red text")
 es.ApplyStyle("D2" "yellow[]red text")
 es.ApplyStyle("G3" "D4" 1)
 es.ApplyStyle("E10" "D2" 1)
 es.ApplyStyle("E11" "F16" 1)

Excel.Workbook wb=es.ws.Parent
Excel.Style st=wb.Styles.Add("blue text")
err st=wb.Styles.Item("blue text")
st.Font.Color=0xff0000
 ...
 es.ApplyStyle("A1" "blue text")
