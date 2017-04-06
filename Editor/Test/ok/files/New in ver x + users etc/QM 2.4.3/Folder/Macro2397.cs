#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
x.StartIE(baseURL 1) ;;or StartChrome, StartFirefox
x.x.c1 ;;selenium.Open("/download.html");
x.x.c2 ;;selenium.Click("id=m_support");selenium.WaitForPageToLoad("30000");
if(mes("Close browser?" "" "YN?2")='Y') x.Quit


IDispatch x=SeleniumScript
str baseURL="http://www.quickmacros.com"
x.StartIE(baseURL 1) ;;or StartChrome, StartFirefox
x.c1 ;;selenium.Open("/download.html");
x.c2 ;;selenium.Click("id=m_support");selenium.WaitForPageToLoad("30000");
if(mes("Close browser?" "" "YN?2")='Y') x.Quit


#compile "__WebDriver"
WebDriver x.Init
str baseURL="http://www.quickmacros.com"
x.StartIE(baseURL 1) ;;or StartChrome, StartFirefox
x.Open("/download.html");
x.Click("id=m_support");x.WaitForPageToLoad("30000");
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
