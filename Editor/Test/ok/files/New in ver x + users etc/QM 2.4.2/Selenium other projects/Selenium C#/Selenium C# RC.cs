 Selenium RC problems:
 Firefox: hidden browser window. Shows only the "log" window. Sometimes shows, especially if we activate the server console before. Workaround: find the hidden window and make visible. This maybe because of a popup blocker.
 IE: script error and blank page. The Selenium script does not run at all. Workaround: disable IE Protected Mode.
 IE: error in this script at some command, although the final page is correct.
 Starts ~9 Chrome processes.
 Chrome shows a 'unsupported command line' warning. There is a workaroud, implemented in QM.
 Does not activate the browser window.

 The Selenium server (java.exe with the .jar file) process must be running. You can use Selenium_StartServer() to start it.

 out
PerfFirst
CsScript x
str so=
 searchDirs=Q:\Downloads\selenium-dotnet-2.42.0\net40
 references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium
x.SetOptions(so)
x.AddCode("")
PerfNext
IDispatch test=x.CreateObject("SeleniumTests.Case")
PerfNext
test.SetupTest
PerfNext
test.TheCaseTest
PerfNext;PerfOut
mes 1
test.TeardownTest


#ret
//C# code
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using NUnit.Framework;
using Selenium;

namespace SeleniumTests
{
//[TestFixture]
public class Case
{
private ISelenium selenium;
private StringBuilder verificationErrors;

//[SetUp]
public void SetupTest()
{
//selenium = new DefaultSelenium("localhost", 4444, "*firefox", "http://www.quickmacros.com/");
selenium = new DefaultSelenium("localhost", 4444, "*googlechrome", "http://www.quickmacros.com/");
//selenium = new DefaultSelenium("localhost", 4444, "*iexplore", "http://www.quickmacros.com/");
selenium.Start();
verificationErrors = new StringBuilder();
}

//[TearDown]
public void TeardownTest()
{
try
{
selenium.Stop();
}
catch (Exception)
{
// Ignore errors if unable to close the browser
}
//Assert.AreEqual("", verificationErrors.ToString());
}

//[Test]
public void TheCaseTest()
{
			selenium.Open("/forum/viewforum.php?f=5");
			selenium.Select("id=st", "label=1 day");
			selenium.Click("name=sort");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Test");
			selenium.WaitForPageToLoad("30000");
}
}
}
