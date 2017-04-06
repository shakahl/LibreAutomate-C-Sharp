#region Selenium, recorded 2014.09.13 08:21
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL) ;;StartIE, StartChrome or StartFirefox

 selenium.Open("/index.html")
 selenium.Click("id=m_Mforum"); selenium.WaitForPageToLoad("30000")
 selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")

x.x.Run

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#ret
public void Run(){
selenium.Open("/index.html");
selenium.Click("id=m_Mforum"); selenium.WaitForPageToLoad("30000");
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000");
}
