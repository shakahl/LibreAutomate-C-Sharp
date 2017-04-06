out
#region Selenium, recorded 2014.09.12 23:35
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://docs.seleniumhq.org"
ISelenium selenium=x.StartIE(baseURL 1) ;;or StartChrome, StartFirefox

selenium.Open("/download/")
selenium.Click("xpath=(//a[contains(text(),'API docs')])[91]4]"); selenium.WaitForPageToLoad("30000")

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion

 C#: void c3() { selenium.Click("id=m_mfeatures");selenium.WaitForPageToLoad("30000"); }
