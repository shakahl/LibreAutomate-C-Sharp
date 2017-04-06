#region Selenium, recorded 2014.09.13 22:05
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartChrome(baseURL) ;;StartIE, StartChrome or StartFirefox

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=Clipboard copy triggers"); selenium.WaitForPageToLoad("30000")
selenium.Click("id=keywords")
selenium.Type("id=keywords", "C#")
selenium.Click("css=input.button2"); selenium.WaitForPageToLoad("30000")

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion
