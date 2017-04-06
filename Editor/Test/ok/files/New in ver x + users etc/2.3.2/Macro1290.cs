ExcelSheet es.Init
 es.ws.Application.Run("Macro1")
 es.SelectCell(4 4)
 out es.NumRows
 es.GetCell(_s 1 1)
 out _s
 ARRAY(str) a
 es.GetCells(a)
 out a
 es.SetCell("test" 5 5)
 es.GetSelectedRange(_i); out _i
 es.GetUsedRange(_i); out _i
 out es.NumRows
 es.Save("$temp$\bbbb.xls")
 run "$temp$\bbbb.xls"
