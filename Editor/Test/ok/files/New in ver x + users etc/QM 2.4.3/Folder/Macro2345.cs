#region Selenium, recorded 2014.09.12 23:34
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://docs.seleniumhq.org"
x.StartIE(baseURL 1) ;;or StartChrome, StartFirefox

x.x.Run

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/download/");
driver.FindElement(By.XPath("(//a[contains(text(),'API docs')])[4]")).Click();
}
