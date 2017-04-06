#region Selenium, recorded 2014.09.13 09:20
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL 1) ;;StartIE, StartChrome or StartFirefox

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")
Assert.IsTrue(Regex.IsMatch(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$"))
x.x.c1
 C#:public void c1(){
 for (int second = 0;; second++) {
 if (second >= 60) Assert.Fail("timeout");
 }
 try { if (Regex.IsMatch(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$")) break; } catch(Exception) {  }
x.x.c2
 C#:public void c2(){
 Thread.Sleep(1000);
}
 }

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion
