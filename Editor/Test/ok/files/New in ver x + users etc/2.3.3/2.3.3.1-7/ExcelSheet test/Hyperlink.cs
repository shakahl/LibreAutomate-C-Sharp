ExcelSheet es.Init
 es.SetCell("link" "A15")
es.Hyperlink("A15" "#Sheet1!C1")
 out es.Cell("A15")
 2
 es.Hyperlink("A15") ;;delete

es.Hyperlink("A18" "#Sheet1!C1" "test link" "h")

es.Activate(4)
