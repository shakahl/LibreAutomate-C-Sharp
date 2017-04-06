function $fromFile

 Imports styles to this workbook from other workbook file.

 fromFile - Excel file containing styles.

 Added in: QM 2.3.3.

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 Excel.Style st=es.StyleObject("my style")
 st.Font.Color=0xff0000
 st.Interior.Color=0x00ffff
 es.StyleApply("3:3" "my style")


Excel.Workbook wb wbt
Excel.Workbooks wbc=ws.Application.Workbooks
out wbc.Count
wbt=wbc.Open(_s.expandpath(fromFile) 2 -1)
out wbc.Count
wb=ws.Parent

wb.Styles.Merge(wbt)

wbt.Close
out wbc.Count

err+ E
