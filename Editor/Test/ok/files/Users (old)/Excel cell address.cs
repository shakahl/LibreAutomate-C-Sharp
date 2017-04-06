ExcelSheet es.Init
Excel.Range ac=es.ws.Application.ActiveCell
str s=ac.Address(0 0 1)
 str s.format("%c%i" 'A'+ac.Column-1 ac.Row)
out s
