function [$myCode] [flags] ;;flags: 0x100 show C# code

 Initializes this WebDriver variable.
 Call before StartChrome etc.

 myCode - C# code to compile. When using IWebDriver, it must contain Selenium script code. If using ISelenium, it can be omitted or "".
   Can be:
     C# code in a variable.
     Name of macro where the code is. This function gets code after #ret line. Error if the macro does not exist or does not contain #ret or is encrypted.
   Can contain any C# functions, variables and 'using' lines in the code. The functions and variables will be added to class User.Script.
   To call the functions from QM macro, you'll use code like this: WebDriver x.Init(myCode) ... x.x.MyFunction().

 REMARKS
 Creates and compiles C# code that contains class User.Script that will call Selenium functions. Creates object of the class.
 Then you can start browser with StartChrome etc. You can call functions of the class through member variable x, like x.x.Func().


opt noerrorshere 1
#opt nowarnings 1

str userCode
int iid nEmpty
if !empty(myCode)
	if findc(myCode 10)<0 ;;assume macro
		iid=qmitem(myCode); if(!iid) end F"{ERR_MACRO}: {myCode}"
		lpstr s=_s.getmacro(iid)
		if(!_s.len) end F"macro {myCode} text is empty or failed to get it. If using in exe, add macro text to exe with #exe addtextof ''{myCode}''. If encrypted, store C# code in a variable."
		rep
			s=strchr(s 10)+1; if(s=1) end F"macro {myCode} does not contain #ret or is encrypted"
			if(!StrCompareN(s "#ret" 4)) s=strchr(s 10); break
			nEmpty+1
		userCode=s
	else userCode=myCode

#region default C# code and options
if(!___wdGlobal.seleniumDir.len) ___wdGlobal.seleniumDir.expandpath("$qm$\Selenium")
str csDir.from(___wdGlobal.seleniumDir "\C#")

if(empty(___wdGlobal.csOptions)) ___wdGlobal.csOptions=F"references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium[]searchDirs={csDir}"

str sUsing=
 using System;using System.Runtime.InteropServices;using System.Text;using System.Text.RegularExpressions;using System.Threading;
 using Selenium;using OpenQA.Selenium;using OpenQA.Selenium.Firefox;using OpenQA.Selenium.Chrome;using OpenQA.Selenium.IE;using OpenQA.Selenium.Support.UI;
if(FileExists(F"{csDir}\nunit.framework.dll")) sUsing+"using NUnit.Framework;"
sUsing.findreplace("[]")
if(userCode.len) REPLACERX rr.frepl=&sub.RxCallback; rr.paramr=&sUsing; userCode.replacerx("(?m)^([ \t]*using +[\w\.]+ *;)+" rr) ;;extract 'using' lines

str sClassDef=
 namespace User{
 [Guid("0AF98784-13FF-41A4-88AA-58182B4D5738")]
 public interface _ISScript{ ISelenium _ISelenium(); }
 public class Script :_ISScript { IWebDriver driver; ISelenium selenium; string baseURL; StringBuilder verificationErrors;
sClassDef.findreplace("[]") ;;make sClassDef 1 line and add sFunctions at the end because then easier to manage errors in user functions.

lpstr sFunctions=
 public ISelenium _ISelenium(){ return selenium; }
 public void _Init(IWebDriver _driver, string _baseURL){ driver=_driver; baseURL=_baseURL; verificationErrors=new StringBuilder(); selenium=new WebDriverBackedSelenium(driver, baseURL); selenium.Start(); }
 public void _Quit(){ try { selenium.Stop(); } catch(Exception) { Console.Write("Warning: Quit() failed."); } }
 public String VerificationErrors(){ String R=verificationErrors.ToString(); verificationErrors.Length=0; return R; }
 }}
#endregion

str cs.format("%s%s[]%.*m%s[]%s" sUsing sClassDef nEmpty 10 userCode sFunctions)

if flags&0x100 ;;show C# code
	_s=cs; _s.replacerx("(?:\r?\n){2,}")
	out F"<><Z 0xe080>C# code</Z>[]<code>{_s}</code>[]<Z 0xe080>C# options</Z>[]{___wdGlobal.csOptions}[]<Z 0xe080></Z>"

m_cs.SetOptions(___wdGlobal.csOptions)
m_cs.x.AddCode(cs)
err
	if(iid) _error.description.replacerx("\bC#\((\d+),\d+\)" F"<open ''{myCode} /L$1''>$0</open>")
	end _error

x=m_cs.CreateObject("User.Script")


#sub RxCallback
function# REPLACERXCB&x

str& sUsing=+x.rrx.paramr
sUsing+x.match
x.match.set(' ')
