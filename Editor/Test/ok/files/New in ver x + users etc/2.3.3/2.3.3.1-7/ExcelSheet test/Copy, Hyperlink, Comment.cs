ExcelSheet es.Init
act "Excel"
Excel.Range r=es._Range("B:B")
r.Select
 es.SelectRange("2:2" 1)
1
act _hwndqm
 out r.NumberFormat
r.Interior.Color=0x00e0ff
r.RowHeight=es.ws.StandardHeight*1.5
 r=es._Range("D:D")
 r.ColumnWidth=20 ;;20 * character width in Normal style
 r.Font.Italic=1
 r=es._Range("E:E")
 r.HorizontalAlignment=Excel.xlLeft
 r.BorderAround(Excel.xlContinuous Excel.xlMedium 7)
 es.ws.Hyperlinks.Add(es._Range("D4") "http://www.quickmacros.com" @ @ "Quick Macros")

 es.Copy("B2" "C20:E20")
 es.Copy("B9" "C20" 1)
 es.Copy("A2" "C3:E3" 1)

 es.StylesImport("$documents$\test.xls")
 es.StyleApply("3:3" "yellow")

 es.Hyperlink("D25" "http://www.quickmacros.com/help" "help" "tooltip2")
 es.Hyperlink("D6" "http://www.quickmacros.com" "" "tooltip")
 es.Hyperlink("D7" "http://www.quickmacros.com" "kkk")
 es.Hyperlink("sel" "http://www.quickmacros.com" "text")
 10
 es.Hyperlink("sel")
 out es.ws.Hyperlinks.Count

 es.Comment("G6" "one[]two")
 es.Comment("G6")

 Excel.Comment c=r.Comment
 out c.Author
  out c.Creator
  out c.Shape.AlternativeText
 out c.Text
 c.Shape.Fill.ForeColor.RGB=0xff ;;exception
 c.Shape.TextEffect.FontBold=0
 out c.Shape.TextEffect.Text
 c.Shape.TextFrame.Characters.Font.Bold=0
 out c.Shape.TextFrame

