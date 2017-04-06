
 ___________________________________

 out

 compile C# code and create C# object
CsScript x
str so=
 searchDirs=$qm$\Selenium\C#
 references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium
x.SetOptions(so)
x.AddCode("")
IDispatch t=x.CreateObject("Selenium.Script")

 some settings
sub.SetPath
str baseURL="http://www.quickmacros.com" ;;change this

 to start browser, use one of these:
t.StartIE(baseURL)
 t.StartChrome(baseURL)
 t.StartFirefox(baseURL)

 to run script, use either t.Run or t.C1;t.C2...
t.Run
  or:
 t.C1
 t.C2
 if mes("C1 and C2 passed. Continue script?" "" "YNi")='Y'
	 t.C3
	 t.C4

if(mes("The Selenium script finished successfully.[][]Close browser and end Selenium session?" "" "YNi")='Y') t.End


#sub SetPath

 Adds Selenium folder path to the PATH environment variable for this process, if not already added.
 Alternatively you can add it in Control Panel for all processes. Then restart QM.
 Don't need this for Firefox.

str s1.expandpath("$qm$\\Selenium") s2
GetEnvVar("PATH" s2)
if(find(s2 s1 1)>=0) ret
s2-";"; s2-s1
SetEnvVar("PATH" s2)


#ret
//C# code
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace Selenium
{
public class Script
{
private IWebDriver driver;
private StringBuilder verificationErrors = new StringBuilder();
private string baseURL;
private bool acceptNextAlert = true;

//You can place the script code in a single function.
//If you use Selenium IDE (web recorder plugin for Firefox), export the recorded script as C# WebDriver. Then copy the recorded commands from the exported file and paste here.
//Replace code in the functions below with your code. The initial code is just an example of a Selenium script in C#.
public void Run()
{
	driver.Navigate().GoToUrl(baseURL + "/index.html");
	driver.FindElement(By.Id("m_download")).Click();
	driver.FindElement(By.LinkText("Forum/Resources")).Click();
	driver.FindElement(By.LinkText("Collected QM apps, functions, samples")).Click();
}

//Or you can create functions for each command or for groups of commands.
//Then you can insert QM code in QM macro between calls to these functions.
//Also then easier to find failed commands.
public void C1() { driver.Navigate().GoToUrl(baseURL + "/index.html"); }
public void C2() { driver.FindElement(By.Id("m_download")).Click(); }
public void C3() { driver.FindElement(By.LinkText("Forum/Resources")).Click(); }
public void C4() { driver.FindElement(By.LinkText("Collected QM apps, functions, samples")).Click(); }


//Don't need to modify these functions, but you can.

public void StartFirefox(string _baseURL)
{
	driver = new FirefoxDriver();
	baseURL = _baseURL;
}

public void StartChrome(string _baseURL)
{
	driver = new ChromeDriver();
	baseURL = _baseURL;
}

public void StartIE(string _baseURL)
{
	driver = new InternetExplorerDriver();
	baseURL = _baseURL;
}

public void End()
{
	try { driver.Quit(); } catch (Exception) {} // Ignore errors if unable to close the browser
	//Assert.AreEqual("", verificationErrors.ToString());
}


//These also have been added by the Selenium IDE C# Export. Delete if don't need.

private bool IsElementPresent(By by) { try { driver.FindElement(by); return true; } catch (NoSuchElementException) { return false; } }
private bool IsAlertPresent() { try { driver.SwitchTo().Alert(); return true; } catch (NoAlertPresentException) { return false; } }
private string CloseAlertAndGetItsText() { try { IAlert alert = driver.SwitchTo().Alert(); string alertText = alert.Text; if (acceptNextAlert) { alert.Accept(); } else { alert.Dismiss(); } return alertText; } finally { acceptNextAlert = true; } }
}
}
