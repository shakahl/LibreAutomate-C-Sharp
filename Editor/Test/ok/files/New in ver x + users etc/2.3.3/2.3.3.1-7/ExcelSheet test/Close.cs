int h=win("" "XLMAIN" "EXCEL")
 outw h
 ret

ExcelSheet es.Init("" 0 "" h)
 ret
1
es.Close(16)
 es.ws.Application.Quit
 PostMessage(h WM_SYSCOMMAND SC_CLOSE 0)
3
