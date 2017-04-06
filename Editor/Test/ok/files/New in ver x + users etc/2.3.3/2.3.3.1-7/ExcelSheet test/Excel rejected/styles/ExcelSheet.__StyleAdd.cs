function'Excel.Style $name

 Adds new style to the workbook, or gets existing style.
 Returns style object.

 name - style name.

 Added in: QM 2.3.3.

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 Excel.Style st=es.StyleObject("my style")
 st.Font.Color=0xff0000
 st.Interior.Color=0x00ffff
 es.StyleApply("3:3" "my style")


Excel.Workbook wb=ws.Parent
Excel.Style st=wb.Styles.Item(name); err
if(!st) st=wb.Styles.Add(name)
ret st

err+ E

 Excel bug: if used basedOn, the style is invalid. Cannot set/get properties.
