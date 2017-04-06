ExcelSheet es.Init
 ExcelSheet es.Init("" 16); 0.5
ARRAY(Excel.Range) a; int i
 __ExcelState _es.Init(es 2)
Q &q
es.Find("c" a 4|32)
 es.Find("c" a 4|8)
 es.Find("c" a 4|2|1)
 es.Find("c" a 4 "B2:C4")
 es.Find("c" a 4 "<all>")
 es.Find("3" a 4)
 VARIANT v.vt=VT_BOOL; v.boolVal=-1
 es.Find(v a 4)
 es.Find("True" a 4)
 DATE d="2000.01.01"; es.Find(d a 4)
 es.Find("2000.01.01" a 4)
 es.Find(100 a 4)
 es.Find(100 a 4|2)
 es.Find("100 Lt" a 4|0x100)
 CURRENCY cy=100; es.Find(cy a 4) ;;does not find
Q &qq
out
outq

 es.Activate(1|4)
for i 0 a.len
	Excel.Range& r=a[i]
	Excel.Worksheet w=r.Parent
	str sWS sAddr
	sWS=w.Name
	sAddr=r.Address(0 0 1)
	out "%s: %s (R%iC%i)" sWS sAddr r.Row r.Column
	 w.Activate
	 r.Activate
	 1

 Excel.CellFormat f.
 Excel.Application x.FindFormat
 x.