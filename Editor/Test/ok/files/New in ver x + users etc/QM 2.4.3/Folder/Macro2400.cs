 /exe 1
#region Selenium, recorded 2014.09.13 22:42
#compile "__WebDriver"
WebDriver x.Init("")
str baseURL="http://www.quickmacros.com"
ISelenium selenium=x.StartIE(baseURL) ;;StartIE, StartChrome or StartFirefox
str verificationErrors
verificationErrors.all

selenium.Open("/index.html")
  comment
 ARRAY(str) a=selenium.GetAllLinks()
 out a
 out WdJoin("," selenium.GetAllLinks())
 WdAssertTrue WdEqual(selenium.GetTitle() "QM"); err WdVerifyErr verificationErrors
 WdAssertTrue WdMatch(selenium.GetTitle(), "^[\s\S]*Macros[\s\S]*$"); err WdVerifyErr verificationErrors
 WdAssertFalse WdEqual(selenium.GetTitle() "BAD")


selenium.Open("/index.html")
selenium.Click("id=m_forum"); selenium.WaitForPageToLoad("30000")
 foreach(0.5 60 WdWait) if(WdMatch(selenium.GetTitle(), "^[\s\S]*Download[\s\S]*$")) break

if(verificationErrors.len) out verificationErrors
if(mes("Close browser?" "" "YN?2")='Y') x.Quit
#endregion

 BEGIN PROJECT
 main_function  Macro2400
 exe_file  $my qm$\Macro2400.qmm
 flags  6
 guid  {2F83B508-433D-4ADD-9C69-1FCF0E591F91}
 END PROJECT
