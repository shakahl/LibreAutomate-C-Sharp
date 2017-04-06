int w=wait(3 WV win("Passagens Aereas Promocionais - Submarino Viagens - Windows Internet Explorer" "IEFrame"))
Htm e=htm("INPUT" "chkSomenteIda" "" w 0 1 0x121 3)
e.Click

Htm e1=htm("INPUT" "txtOrigem" "" w 0 4 0x121 3)
e1.SetText("Rio de Janeiro / RJ, Brasil, Santos Dumont (SDU)")

Htm e2=htm("INPUT" "txtDestino" "" w 0 5 0x121 3)
e2.SetText("Sao Paulo / SP, Brasil, Congonhas (CGH)")

Htm e3=htm("INPUT" "txtDataIda" "" w 0 6 0x121 3)
e3.SetText("10/01/2013")

Htm e4=htm("INPUT" "btnPesquisar" "" w 0 8 0x121 3)
e4.Click

ARRAY(str) texto.create(1 1000) ;;1 columns, 10 rows
for(_i 0 texto.len 1)
	int w1=wait(3 WV win("Submarino Viagens - Windows Internet Explorer" "IEFrame"))
	Htm e5=htm("STRONG" "" "" w1 "" 115 0x21 3 _i)
	texto[0 _i]=e5.Text
	out texto[0 _i]

 ExcelSheet es.Init
 es.CellsFromArray(texto "A1")
