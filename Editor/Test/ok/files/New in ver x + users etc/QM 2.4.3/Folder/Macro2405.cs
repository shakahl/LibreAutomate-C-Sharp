
#region Selenium, recorded 2014.09.14 09:38
#compile "__WebDriver"
WebDriver x.Init()
str baseURL="http://www.meteo.lt"
ISelenium selenium=x.StartChrome(baseURL) ;;StartChrome, StartFirefox or StartIE

selenium.Open("/skaitmenine_prog_lt_miest.php")
selenium.Click("link=Orų prognozė"); selenium.WaitForPageToLoad("30000");
selenium.Type("id=j_paieska_teks", "lietus")
selenium.Click("id=j_paieska_submit"); selenium.WaitForPageToLoad("30000");

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion
