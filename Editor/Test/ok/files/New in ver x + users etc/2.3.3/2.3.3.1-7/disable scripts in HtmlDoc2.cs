out

Htm el=htm("HTML" "" "" win("" "IEFrame") 0 0 32)
str s=el.el.outerHTML

HtmlDoc h

h.InitFromText(s)
s=h.GetHtml
s.setmacro("Macro578")

h.SetOptions(1)
h.InitFromText(s)
s=h.GetHtml
s.setmacro("Macro581")

mac+ "Macro578"
