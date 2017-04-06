out
str html
IntGetFile "http://www.applefile.com" html
 IntGetFile "http://www.quickmacros.com" html
 IntGetFile "http://www.meteo.lt" html
 html.fix(300)
 html.ConvertEncoding(10003 _unicode)
 html.ConvertEncoding(949 _unicode)
 html.ConvertEncoding2("euc-kr" _unicode)

  html.RandomString(100 
 html="abc ąčęėįšųū"; html.ConvertEncoding(-1 "windows-1257")
 out html

str repl=
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
 html.findreplace(repl)

 IMultiLanguage2 m._create(CLSID_CMultiLanguage)
 int nb=html.len
 DetectEncodingInfo d; int nd=1
 PF
 m.DetectInputCodepage(0 0 html &nb &d &nd) ;;MLDETECTCP_HTML
 PN;PO
 out "%i %i  percent=%i confidence=%i _hresult=%i" d.nCodePage nd d.nDocPercent d.nConfidence _hresult

 html.ConvertEncoding("<detect>" -1)
html.ConvertEncoding("<html>" -1)
out html
