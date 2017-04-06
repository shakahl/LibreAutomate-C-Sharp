 int h=id(2202)
 Acc a=acc(h)
 a.Role(_s)
 out _s
 out child(a)

MSHTML.IHTMLElement el=htm("A" "Utilities & Drivers" "" win("Internet Explorer") 0 62 0x21)

Acc a
a=acc(el)
out a.Name

Acc an=acc(a "pa")
an.Role(_s)
out _s
