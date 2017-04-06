function [$SeleniumFolder] [$CsScriptOptions]

 Sets some global options for WebDriver class.

 SeleniumFolder - folder containing Selenium files - chromedriver.exe, IEDriverServer.exe and subfolder C#.
   If not set, WebDriver.Init sets it to "$qm$\Selenium".
   Once set, cannot be changed until restarting QM.
 CsScriptOptions - <help>CsScript.SetOptions</help> optionsList.
   If not set, WebDriver.Init uses F"references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium[]searchDirs={SeleniumFolder}\C#".

 REMARKS
 Call this function at startup or before WebDriver.Init.
 If an argument is omitted or "", does not change that option.


if(!empty(SeleniumFolder) and !___wdGlobal.seleniumDir.len) ___wdGlobal.seleniumDir.expandpath(SeleniumFolder)
if(!empty(CsScriptOptions)) ___wdGlobal.csOptions=CsScriptOptions
