#region Selenium, recorded 2014.09.15 11:49
#compile "__WebDriver"
WebDriver x.Init()
str baseURL="http://www.meteo.lt"
ISelenium selenium=x.StartChrome(baseURL) ;;StartChrome, StartFirefox or StartIE
str verificationErrors
verificationErrors.all

selenium.Open("/skaitmenine_prog_lt_miest.php")
selenium.Click("link=Orų prognozė"); selenium.WaitForPageToLoad("30000");
selenium.Type("id=j_paieska_teks", "lietus")
selenium.Click("id=j_paieska_submit"); selenium.WaitForPageToLoad("30000");
WdAssertTrue WdMatch(selenium.GetTitle(), "^[\s\S]*r[\s\S]*$"); err WdVerifyErr verificationErrors
out "finished"

if(verificationErrors.len) out verificationErrors
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion
