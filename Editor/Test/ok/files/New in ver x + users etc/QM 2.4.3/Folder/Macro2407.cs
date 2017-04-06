 /exe
#exe addtextof "Macro2407"
#region Selenium, recorded 2014.09.15 09:35
#compile "__WebDriver"
WebDriver x.Init(__FUNCTION__)
str baseURL="http://www.meteo.lt"
x.StartChrome(baseURL 2) ;;StartChrome, StartFirefox or StartIE

x.x.Run

_s=x.x.VerificationErrors; if(_s.len) out _s
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


 BEGIN PROJECT
 main_function  Macro2407
 exe_file  $my qm$\Macro2407.qmm
 flags  6
 guid  {BB955DF0-17A8-405C-AA8C-C52E28B7154D}
 END PROJECT

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
