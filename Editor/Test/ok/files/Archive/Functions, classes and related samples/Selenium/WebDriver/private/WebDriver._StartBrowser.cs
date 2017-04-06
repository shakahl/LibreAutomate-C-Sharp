function'ISelenium browser $baseURL flags ;;browser: 1 FF, 2 Chrome, 3 IE.  flags: 1 always start new browser instance, 2 show driver console

#if EXE
if(flags&3=0) end "In exe use flag 1 or 2. Or make sure that Quit is always called at the end. Else driver processes will not be closed and temporary files will not be deleted." 8|2
#endif

opt noerrorshere 1
#opt nowarnings 1
if(!x) end ERR_INIT
if(m_started) Quit

lock
if !___wdGlobal.cs.x
	str so=
	F
	 references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium
	 searchDirs={___wdGlobal.seleniumDir}\C#
	___wdGlobal.cs.SetOptions(so)
	___wdGlobal.cs.AddCode("")
	___wdGlobal.cs.Call("Init" ___wdGlobal.seleniumDir)

lpstr fn; sel(browser) case 1 fn="StartFirefox"; case 2 fn="StartChrome"; case 3 fn="StartIE"
IUnknown driver=___wdGlobal.cs.Call(fn flags&3)
x._Init(driver baseURL)
 selenium=x._ISelenium ;;unreliable because uses IID (to QI from IDispatch) which may change in the future
___ISeleniumScript _is=x; m_selenium=_is._ISelenium

 BSTR guidISelenium
 IUnknown _selenium=x._Init(driver baseURL &guidISelenium)
 _s=guidISelenium; _s-"{"; _s+"}"
 GUID guid; IIDFromString(@_s &guid)
 if(_selenium.QueryInterface(&guid &selenium)) end ERR_FAILED

m_started=iif(flags&1 -browser browser)
ret m_selenium

 SHOULDDO:
 From FAQ: WebDriver is not thread-safe. If you can serialise access to the underlying driver instance, you can share a reference in more than one thread.
 Difficult. Assume the user will never run 2 macros that automate the same browser instance simultaneously.


#ret
using System;using OpenQA.Selenium;using OpenQA.Selenium.Firefox;using OpenQA.Selenium.Chrome;using OpenQA.Selenium.IE;
public class Script
{
static IWebDriver driverFirefox, driverChrome, driverIE;
static string driverPath;

public static void Init(string _driverPath)
{
driverPath=_driverPath;
}

public static IWebDriver StartFirefox(int flags)
{
bool reuse=(flags&1)==0 ? true : false;
if(reuse && driverFirefox!=null && _IsValid(ref driverFirefox)) return driverFirefox;
IWebDriver driver=new FirefoxDriver();
if(reuse) driverFirefox=driver;
return driver;
}

public static IWebDriver StartChrome(int flags)
{
bool reuse=(flags&1)==0 ? true : false;
if(reuse && driverChrome!=null && _IsValid(ref driverChrome)) return driverChrome;
ChromeDriverService ds=ChromeDriverService.CreateDefaultService(driverPath); ds.HideCommandPromptWindow=(flags&2)==0;
ChromeOptions options=new ChromeOptions();options.AddArgument("--test-type");
IWebDriver driver=new ChromeDriver(ds, options);
if(reuse) driverChrome=driver;
return driver;
}

public static IWebDriver StartIE(int flags)
{
bool reuse=(flags&1)==0 ? true : false;
if(reuse && driverIE!=null && _IsValid(ref driverIE)) return driverIE;
InternetExplorerDriverService ds=InternetExplorerDriverService.CreateDefaultService(driverPath); ds.HideCommandPromptWindow=(flags&2)==0;
IWebDriver driver=new InternetExplorerDriver(ds, new InternetExplorerOptions());
if(reuse) driverIE=driver;
return driver;
}

static bool _IsValid(ref IWebDriver driver)
{
try{string s=driver.Url;return true;}catch(Exception){}
try{driver.Quit();}catch(Exception){}
driver=null;
return false;
}

public static void OnQuit(int browser)
{
switch(browser){
case 1: driverFirefox=null; break;
case 2: driverChrome=null; break;
case 3: driverIE=null; break;
}
}
}
