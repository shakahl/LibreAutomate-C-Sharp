int hwndIE=win("Internet Explorer" "IEFrame")
Htm el=htm("HTML" "" "" hwndIE)
HtmlDoc d.InitFromHtm(el)
out d.GetText
