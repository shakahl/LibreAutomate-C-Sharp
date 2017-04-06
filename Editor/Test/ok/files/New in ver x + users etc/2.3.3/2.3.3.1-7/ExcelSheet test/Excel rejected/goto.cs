out
ExcelSheet es.Init("" 16)

 es.ws.Application.Goto("R100C2") ;;go to B100
es.ws.Application.Goto(es._Range("B100")) ;;the same
 es.ws.Application.Goto("Sheet2!R100C2") ;;go to B100
