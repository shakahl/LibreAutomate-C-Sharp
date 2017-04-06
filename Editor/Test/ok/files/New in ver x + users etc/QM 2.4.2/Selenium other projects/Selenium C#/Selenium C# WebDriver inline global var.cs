/exe
out
CsScript g_x
IDispatch+ t
if(!t) t=g_x.SeleniumInline("" "chrome" "http://www.quickmacros.com" 0x100)
t.c1 ;;driver.Navigate().GoToUrl(baseURL + "/index.html");
if mes("c1 passed. Continue script?" "" "YNi")='Y'
	t.c2 ;;driver.FindElement(By.Id("m_download")).Click(); driver.FindElement(By.LinkText("Forum/Resources")).Click(); //comments
	t.c3 ;;driver.FindElement(By.LinkText("Collected QM apps, functions, samples")).Click();
if(mes("The Selenium script finished successfully.[][]Close browser and end Selenium session?" "" "YNi")='Y') t.End


 BEGIN PROJECT
 main_function  Selenium C# WebDriver2
 exe_file  $my qm$\Selenium C# WebDriver2.qmm
 flags  6
 guid  {2C1620E2-538C-4A7B-A313-3712C01B8C27}
 END PROJECT
