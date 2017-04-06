 out

#compile "__Selenium"

PF
Selenium x
 x.Connect("firefox" "" 4)
x.Connect("chrome" "" 4)
PN
x.urlOpen("http://www.meteo.lt/skaitmenine_prog_lt_miest.php")
PN
PO;ret
int i

 ARRAY(str) a
  x.sessionsGet(a); out a.len; for(i 0 a.len) out a[i]
 x.sessionsGet(a "id"); out a.len; for(i 0 a.len) out a[i]

 _s=x.windowGetId
 out _s
 x.windowActivate(_s)

 out x.urlGet
 out x.sourceHtmlGet

 out x.element("link text" "Download")
 out x.element("link text" "Features")
 ARRAY(int) a
 x.elements("partial link text" "a" a)
 for(i 0 a.len) out a[i]

i=x.element("link text" "Download")
PN
x.elementClick(i)
PN;PO

 mes 1
2
 x.windowClose ;;it seems ends session too, because End fails
 mes 2
 x.End
