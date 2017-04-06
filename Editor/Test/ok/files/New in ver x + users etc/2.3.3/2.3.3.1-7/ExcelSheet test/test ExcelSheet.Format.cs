ExcelSheet es.Init

 es.WidthHeight("A1" 10 0.8)
 es.WidthHeight("A1" -1 -1)
 es.WidthHeight("A1" -1)
 es.WidthHeight("C:E" 20) ;;set width of columns C-E to the width of 20 characters of Normal style
 es.WidthHeight("5:5" 0 1.5) ;;set height of row 5 to 1.5 of standard height

EXCELFORMAT x
x.style="Normal"
 es.Format("<all>" x)
x.numberFormat="@"
x.fontBold=1
x.fontItalic=1
x.fontUnderline=1
 x.fontNormal=1
 x.cellColor=0xE0E0FF
x.fontColor=0x008000
x.cellColor=0x80000000|24
x.indentLevel=1
x.alignVert=3
x.fontSize=8
x.fontName="Comic Sans MS"
x.borders=0xf|0x300
 x.borders=128|0x100
 x.borderStyle=Excel.xlDouble
x.borderStyle=Excel.xlSlantDashDot
 x.borderStyle=Excel.xlDash
 x.borderThickness=4
 x.borderColor=0xff0000
x.textWrap=1

 x.borders=0x101

 Q &q
__ExcelState _es.Init(es 2) ;;temporarily disable Excel screen updating. Faster and less flickering.
es.Format("B2:C8" x)
 _es.UpdatingRestore
 Q &qq; outq

 EXCELFORMAT y
 y.borders=0x30
 y.borderStyle=1
 y.borderThickness=2
 y.borderColor=0xff8080
 es.Format("B2:C8" y)
