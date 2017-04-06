out

#compile "__Selenium"

 int w=Selenium_FindServer(3)
 if(!w) w=Selenium_StartServer("Q:\Downloads\selenium-server-standalone-2.42.2.jar")
 act w
 clo w; ret
PF
Selenium x
 x.SetOptions("http://localhost:6432/wd/hub")
 x.Start("firefox" "http://www.quickmacros.com" 3|4)
 x.Start("firefox" "" 3|4)
x.Start("internet explorer" "" 3|4)
 x.Start("chrome" "" 3|4)
 x.Start("{''desiredCapabilities'':{''browserName'':''chrome'',''chromeOptions'':{''args'':[''--test-type'']}}}" "" 3)
 x.Start("firefox" "http://www.meteo.lt/skaitmenine_prog_lt_miest.php" 3|4)
 ret
PN
x.urlOpen("http://www.quickmacros.com")
PN
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
