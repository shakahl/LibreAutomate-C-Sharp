#region Selenium, recorded 2014.09.13 18:47
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
x.StartIE(baseURL 1) ;;StartIE, StartChrome or StartFirefox

x.x.Run

_s=x.VerificationErrors; if(_s.len) out _s
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/index.html");
driver.FindElement(By.Id("m_forum")).Click();
driver.FindElement(By.LinkText("QM Extensions")).Click();
String st = driver.Title;
Assert.IsTrue(IsAlertPresent());
Assert.IsFalse(Regex.IsMatch(CloseAlertAndGetItsText(), "^[\\s\\S]*mmmm$"));
Assert.AreEqual("Simple", driver.Title);
Assert.AreNotEqual("Simple", driver.Title);
Assert.IsTrue(Regex.IsMatch(driver.Title, "^[\\s\\S]*Forum[\\s\\S]*$"));
Assert.IsFalse(Regex.IsMatch(driver.Title, "^[\\s\\S]*NNNN[\\s\\S]*$"));
for (int second = 0;; second++) {
    if (second >= 60) Assert.Fail("timeout");
    try
    {
        if (Regex.IsMatch(driver.Title, "^[\\s\\S]*Forum[\\s\\S]*$")) break;
    }
    catch (Exception)
    {}
    Thread.Sleep(1000);
}
for (int second = 0;; second++) {
    if (second >= 60) Assert.Fail("timeout");
    try
    {
        if (!Regex.IsMatch(driver.Title, "^[\\s\\S]*uuu[\\s\\S]*$")) break;
    }
    catch (Exception)
    {}
    Thread.Sleep(1000);
}
// ERROR: Caught exception [Error: locator strategy either id or name must be specified explicitly.]
// ERROR: Caught exception [Error: locator strategy either id or name must be specified explicitly.]
// ERROR: Caught exception [ERROR: Unsupported command [getAllLinks |  | ]]
driver.FindElement(By.LinkText("hh*")).Click();
for (int second = 0;; second++) {
    if (second >= 60) Assert.Fail("timeout");
    try
    {
        if (!IsAlertPresent()) break;
    }
    catch (Exception)
    {}
    Thread.Sleep(1000);
}
for (int second = 0;; second++) {
    if (second >= 60) Assert.Fail("timeout");
    try
    {
        if ("text" == driver.FindElement(By.TagName("BODY")).Text) break;
    }
    catch (Exception)
    {}
    Thread.Sleep(1000);
}
// ERROR: Caught exception [ERROR: Unsupported command [getAllLinks |  | ]]
// ERROR: Caught exception [ERROR: Unsupported command [getAllLinks |  | ]]
try
{
    Assert.AreEqual("msg", CloseAlertAndGetItsText());
}
catch (AssertionException e)
{
    verificationErrors.Append(e.Message);
}
try
{
    Assert.IsFalse(Regex.IsMatch(CloseAlertAndGetItsText(), "^[\\s\\S]*msg$"));
}
catch (AssertionException e)
{
    verificationErrors.Append(e.Message);
}
}

private bool IsAlertPresent() { try { driver.SwitchTo().Alert(); return true; } catch (NoAlertPresentException) { return false; } }

private bool acceptNextAlert = true;
private string CloseAlertAndGetItsText() { try { IAlert alert = driver.SwitchTo().Alert(); string alertText = alert.Text; if (acceptNextAlert) { alert.Accept(); } else { alert.Dismiss(); } return alertText; } finally { acceptNextAlert = true; } }
