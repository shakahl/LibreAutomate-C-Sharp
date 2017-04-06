#region Selenium, recorded 2014.09.13 22:01
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
x.StartFirefox(baseURL) ;;StartIE, StartChrome or StartFirefox

x.x.Run

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/index.html");
driver.FindElement(By.Id("m_forum")).Click();
driver.FindElement(By.LinkText("QM Extensions")).Click();
driver.FindElement(By.LinkText("Clipboard copy triggers")).Click();
driver.FindElement(By.Id("keywords")).Click();
driver.FindElement(By.Id("keywords")).Clear();
driver.FindElement(By.Id("keywords")).SendKeys("C#");
driver.FindElement(By.CssSelector("input.button2")).Click();
}
