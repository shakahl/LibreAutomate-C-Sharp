#region Selenium, recorded 2014.09.13 08:21
#compile "__WebDriver"
WebDriver x
x.SetOptions("$desktop$\Selenium")
x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL) ;;StartIE, StartChrome or StartFirefox

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion

 Example with IWebDriver functions called from C# code
#region Selenium, recorded 2014.09.13 08:21
#compile "__WebDriver"
WebDriver x2.Init("")
str baseURL2="http://www.quickmacros.com"
x2.StartIE(baseURL2) ;;StartIE, StartChrome or StartFirefox

x2.x.Run

if(mes("Close browser?" "" "YN?2")='Y') x2.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/index.html");
driver.FindElement(By.Id("m_forum")).Click();
driver.FindElement(By.LinkText("QM Extensions")).Click();
}
