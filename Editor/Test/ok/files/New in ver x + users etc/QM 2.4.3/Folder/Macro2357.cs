#region Selenium, recorded 2014.09.13 12:04
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL 1) ;;StartIE, StartChrome or StartFirefox

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")
str st=selenium.GetTitle()
if(!(selenium.IsAlertPresent())) end "assertion"
if(findrx(selenium.GetAlert(), "^[\s\S]*mmmm$")>=0) end "assertion"
if(!(selenium.GetTitle()="Simple")) end "assertion"
if(selenium.GetTitle()="Simple") end "assertion"
if(!(findrx(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$")>=0)) end "assertion"
if(findrx(selenium.GetTitle(), "^[\s\S]*NNNN[\s\S]*$")>=0) end "assertion"
foreach(0.5 60 sub.Wait) if(findrx(selenium.GetTitle(), "^[\s\S]*Forum[\s\S]*$")>=0) break
foreach(0.5 60 sub.Wait) if(!(findrx(selenium.GetTitle(), "^[\s\S]*uuu[\s\S]*$")>=0)) break

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#sub Wait
function ^period ^timeout
double waited; if(waited>=timeout) end "timeout"
opt waitmsg -1; wait period; waited+period
ret 1
