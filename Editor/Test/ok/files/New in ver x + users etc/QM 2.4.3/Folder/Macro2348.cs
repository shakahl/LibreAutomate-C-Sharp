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
driver.FindElement(By.Id("m_Mforum")).Click();
driver.FindElement(By.LinkText("QM Extensions")).Click();
}
