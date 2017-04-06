deb
#region Selenium, recorded 2014.09.13 18:33
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL 1) ;;StartIE, StartChrome or StartFirefox
str verificationErrors
verificationErrors.all

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")
str st=selenium.GetTitle()
WdAssertTrue selenium.IsAlertPresent()
WdAssertFalse WdMatch(selenium.GetAlert(), "^[\s\S]*mmmm$")
WdAssertTrue WdEqual(selenium.GetTitle() "Simple")
WdAssertFalse WdEqual(selenium.GetTitle() "Simple")
WdAssertTrue WdMatch(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$")
WdAssertFalse WdMatch(selenium.GetTitle(), "^[\s\S]*NNNN[\s\S]*$")
foreach(0.5 60 WdWait) if(WdMatch(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$")) break
foreach(0.5 60 WdWait) if(!WdMatch(selenium.GetTitle(), "^[\s\S]*uuu[\s\S]*$")) break
WdAssertTrue WdEqual(selenium.GetAttribute("foo@bar") "avvvvvvvvvv")
WdAssertTrue WdMatch(selenium.GetAttribute("foo@bar"), "^[\s\S]*avvvvvvvvvv$")
WdAssertTrue WdMatch(WdJoin(",", selenium.GetAllLinks()), "^[\s\S]*whatsthis$")
selenium.Click("link=hh*")
foreach(0.5 60 WdWait) if(!selenium.IsAlertPresent()) break
foreach(0.5 60 WdWait) if(WdEqual(selenium.GetBodyText() "text")) break
foreach(0.5 60 WdWait) if(WdEqual(WdJoin(",", selenium.GetAllLinks()) "pattern")) break
foreach(0.5 60 WdWait) if(!WdMatch(WdJoin(",", selenium.GetAllLinks()), "^[\s\S]*pattern$")) break
WdAssertTrue WdEqual(selenium.GetAlert() "msg"); err WdVerifyErr verificationErrors
WdAssertFalse WdMatch(selenium.GetAlert(), "^[\s\S]*msg$"); err WdVerifyErr verificationErrors

if(verificationErrors.len) out verificationErrors
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion
