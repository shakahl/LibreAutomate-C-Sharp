int w1=win("Teksto vertimas - Windows Internet Explorer" "IEFrame")
Htm el=HtmlFind("A" "Apie" "" w1)
out el.Text
