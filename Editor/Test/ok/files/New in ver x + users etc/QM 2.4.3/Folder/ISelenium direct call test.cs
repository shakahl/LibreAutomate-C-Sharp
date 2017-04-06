out
#region Selenium, recorded 2014.09.12 10:06
#compile "__WebDriver"
WebDriver x.Init("" 0x100)
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartChrome(baseURL 1) ;;or StartChrome, StartFirefox
 ret

 x.x.c1 ;;selenium.Open("/index.html");
selenium.Open("/index.html")
out x.VerificationErrors

 x.x.c2 ;;selenium.Click("id=m_download");selenium.WaitForPageToLoad("30000");
selenium.Click("id=m_download"); selenium.WaitForPageToLoad("30000");

 x.x.c3
 C#: void c3() { selenium.Click("id=m_mfeatures");selenium.WaitForPageToLoad("30000"); }
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


 ISelenium k=x.selenium
 k.Click("id=m_download")
 TODO: why on F1 shows help links for Click, not for ISelenium.Click?
