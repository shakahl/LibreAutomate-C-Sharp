 This class allows to automate Internet Explorer, Firefox and Chrome using Selenium WebDriver.
 Selenium is a web recorder and API for web page automation, eg for automated testing. Free, open-source.
 Selenium downloads: http://docs.seleniumhq.org/download/
 Selenium scripts can be recorded/edited with the Selenium IDE or created manually in QM.
 To export current script (Test Case) from Selenium IDE to QM, use the small toolbar with QM icon that QM adds to the IDE.
 In QM macros the scripts can be executed in 2 ways:
   1. Call ISelenium interface functions directly. The QM Selenium toolbar button creates such code if in Selenium IDE menu Options -> Clipboard Format is selected C# Remote Control.
   2. Call a user-defined C# function that uses functions of IWebDriver and other Selenium classes/interfaces available in C#. The QM Selenium toolbar button creates such code if in Selenium IDE menu Options -> Clipboard Format is selected C# WebDriver.
 You can find the Selenium API reference in the Selenium website or in the help file downloaded with C# language bindings.

 Selenium setup:
 Create folder for Selenium files. It should be QM folder's subfolder Selenium ($qm$\Selenium); if not, will need to specify it with SetOptions().
 Selenium consists of multiple components without a common installer. Download what you need.
 From Selenium website you need:
	 Selenium IDE. Need it only to record web actions, don't need to run Selenium scripts.
	 Selenium WebDriver bindings for C#. Create folder C# in the Selenium folder ($qm$\Selenium\C#) and add there all files from downloaded folder net35 (WinXP/7) or net40 (Win8/10).
	 If you'll use IE, download Internet Explorer Driver Server. Add IEDriverServer.exe to the Selenium folder.
	 If you'll use Chrome, download Chrome browser driver. Add chromedriver.exe to the Selenium folder.
 Also you need:
	 Windows XP SP2 or later.
	 .NET framework 3.5 or 4.x. Windows 7 and 8 have it. If your Windows is older and the .NET framework is not installed, download .NET 3.5 from Microsoft.
	 Firefox. Only for Selenium IDE, because it is a Firefox plugin.
 Also you may need:
	 NUnit. Only if you'll use Assert.IsTrue etc in C#. Don't need for ISelenium functions. Install NUnit eg in Program Files, and copy nunit.framework.dll to the Selenium folder.

 Selenium problems:
 There are no ways to automate an existing browser instance.
   You can reuse only the browser instance started by StartChrome() etc without flag 1 in same QM process.
   Also it means that if QM is running as admin, browser processes also run as admin. It is not good for security.
 Manually closing a Selenium-started browser does not close its driver process and does not delete temporary files (eg a Firefox profile may be 30 MB).
   The correct way to close browser - call Quit in the macro. Or use flag 1 with StartChrome() etc, then Quit is called automatically.
   Without flag 1, StartChrome() etc correctly closes the browser that was previously started by the same function (StartChrome() etc) without flag 1 in same QM process.
   Another correct way - use flag 2 with StartChrome() etc. It shows a driver console window if the browser is Chrome or IE. Let the user close the console after closing its controlled browser.
 If using IE, may need to change Protected Mode in IE Options -> Security. Must be the same in all zones.
 The IE driver process may crash when an ISelenium function does not find an element.
 Selenium-started IE does not respond to user clicks.
 On one of my computers did not work correctly with IE if Java is not installed. You can download Java 32-bit from Oracle. Look in Control Panel, maybe you already have it.
 Does not support high DPI. Clicks in wrong place. Tested only with IE.
 Does not give the browser window handle. If you need it, use win() after StartChrome() etc.
 WebDriver 2.42 did not work with Firefox 32 and later. Version 2.44 works.


 EXAMPLES
 Example with ISelenium functions called from QM macro
#region Selenium, recorded 2014.09.13 08:21
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartChrome(baseURL) ;;StartChrome, StartFirefox or StartIE

selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
selenium.Click("link=QM Extensions"); selenium.WaitForPageToLoad("30000")

if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion

 Example with IWebDriver functions called from C# code
#region Selenium, recorded 2014.09.13 08:21
#compile "__WebDriver"
WebDriver x2.Init(__FUNCTION__)
str baseURL2="http://www.quickmacros.com"
x2.StartChrome(baseURL2) ;;StartChrome, StartFirefox or StartIE

x2.x.Run

if(mes("Close browser?" "" "YN?2")='Y') x2.Quit
#endregion


#ret
public void Run(){
driver.Navigate().GoToUrl(baseURL + "/index.html");
driver.FindElement(By.Id("m_forum")).Click();
driver.FindElement(By.LinkText("QM Extensions")).Click();
}
