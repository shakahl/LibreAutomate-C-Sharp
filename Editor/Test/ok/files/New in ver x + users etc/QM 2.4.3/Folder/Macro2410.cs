#region Selenium, recorded 2014.09.15 11:48
#compile "__WebDriver"
WebDriver x.Init(__FUNCTION__)
str baseURL="http://www.meteo.lt"
x.StartChrome(baseURL) ;;StartChrome, StartFirefox or StartIE

x.x.Run

_s=x.x.VerificationErrors; if(_s.len) out _s
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/skaitmenine_prog_lt_miest.php");
driver.FindElement(By.LinkText("Orų prognozė")).Click();
driver.FindElement(By.Id("j_paieska_teks")).Clear();
driver.FindElement(By.Id("j_paieska_teks")).SendKeys("lietus");
driver.FindElement(By.Id("j_paieska_submit")).Click();
try
{
    Assert.IsTrue(Regex.IsMatch(driver.Title, "^[\\s\\S]*r[\\s\\S]*$"));
}
catch (AssertionException e)
{
    verificationErrors.Append(e.Message);
}
Console.WriteLine("finished");
}
