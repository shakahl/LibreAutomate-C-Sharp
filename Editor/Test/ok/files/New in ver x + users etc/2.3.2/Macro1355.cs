out

HtmlDoc d

 d.InitFromWeb("http://www.google.com")
 d.InitFromWeb("http://www.quickmacros.com")

 IntGetFile "http://www.google.com" _s; _s.setfile("$temp$\qm.html")
 d.InitFromFile("$temp$\qm.html")

 d.InitFromInternetExplorer(win("" "IEFrame"))

 Htm el=htm("DIV" "iGoogle | Paieškos nustatymai | Registruotis" "" win("Google - Windows Internet Explorer" "IEFrame") 0 5 0x21)
 d.InitFromHtm(el)

str ht=
 <html><head>
 </head><body>
 abcd ąčęž ب₪ abcd
 </body></html>
d.InitFromText(ht)

str s=d.GetText
out s

 <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
