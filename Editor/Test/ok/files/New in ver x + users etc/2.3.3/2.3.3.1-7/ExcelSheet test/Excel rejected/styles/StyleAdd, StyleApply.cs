ExcelSheet es.Init
Excel.Style st=es.StyleAdd("style6" "A5")
 Excel.Style st=es.StyleAdd("style3" "A9")
 Excel.Style st=es.StyleAdd("style4")
 st.Font.Italic=1
 st.Font.Name="Courier New"
 Excel.Workbook wb=st.Parent; out wb.Name
out st.IncludeFont
 VARIANT v.vt=VT_BOOL; v.boolVal=-1
st.IncludeFont=-1
st.Font.Color=0xff
 es.StyleApply("G:G" "style5")

