out
#region Selenium, recorded 2014.09.13 23:11
#compile "__WebDriver"
WebDriver x.Init(__FUNCTION__ 0x100)
str baseURL="http://www.quickmacros.com"
x.StartIE(baseURL 1) ;;StartChrome, StartFirefox or StartIE

x.x.Run

/if(mes("Close browser?" "" "YN?2")='Y') x.Quit
1
#endregion


#ret
using System.Reflection;
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/index.html");
driver.FindElement(By.Id("m_forum")).Click();
}
