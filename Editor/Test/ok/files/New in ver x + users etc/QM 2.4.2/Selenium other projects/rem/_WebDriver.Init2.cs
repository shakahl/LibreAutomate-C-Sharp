function $macro [flags] [$myUsing] [$myFunctions] ;;flags: 0x100 show C# code

 macro - TODO.
 myUsing - zero or more C# 'using' statements to add to the default statements, if you need it. Default C# code is stored in this function, you can open and see it.
 myFunctions - zero or more additional C# functions and variables, if you need it. They will be added to class User.Script that also contains code from macro.
   For example, if you use C# code, but the macro is encrypted, you can put the code in a variable and pass it as myFunctions.

 TODO: test encrypted.
 TODO: allow macro to be code in a variable.
 TODO: move myUsing and myFunctions to SetOptions.
 TODO: extract 'using' from macro and from myFunctions.


#region default C# code and options
if(!___wdGlobal.seleniumDir.len) ___wdGlobal.seleniumDir.expandpath("$qm$\Selenium")
str csDir.from(___wdGlobal.seleniumDir "\C#")

if(empty(___wdGlobal.csOptions)) ___wdGlobal.csOptions=F"references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium[]searchDirs={csDir}"

str sUsing
sUsing=
 using System;using System.Runtime.InteropServices;using System.Text;using System.Text.RegularExpressions;using System.Threading;
 using Selenium;using OpenQA.Selenium;using OpenQA.Selenium.Firefox;using OpenQA.Selenium.Chrome;using OpenQA.Selenium.IE;using OpenQA.Selenium.Support.UI;
if(FileExists(F"{csDir}\nunit.framework.dll")) sUsing+"using NUnit.Framework;" ;;else if it's registered in GAC, pass this string as myUsing
sUsing+myUsing
sUsing.replacerx("//.*"); sUsing.replacerx("[\r\n]")

str cClassDef=
 namespace User{
 [Guid("0AF98784-13FF-41A4-88AA-58182B4D5738")]
 public interface _ISScript{ ISelenium _ISelenium(); }
 public class Script :_ISScript { IWebDriver driver; ISelenium selenium; string baseURL; StringBuilder verificationErrors;
cClassDef.findreplace("[]")
 info: makes cClassDef 1 line and adds sFunctions at the end because then easier to manage errors in user functions.

lpstr sFunctions=
 public ISelenium _ISelenium(){ return selenium; }
 public void _Init(IWebDriver _driver, string _baseURL){ driver=_driver; baseURL=_baseURL; verificationErrors=new StringBuilder(); selenium=new WebDriverBackedSelenium(driver, baseURL); selenium.Start(); }
 public void _Quit(){ try { selenium.Stop(); } catch(Exception) { Console.Write("Warning: Quit() failed."); } }
 public String _VerificationErrors(){ String R=verificationErrors.ToString(); verificationErrors.Length=0; return R; }
 }}
#endregion
 public void _Init(IWebDriver _driver, string _baseURL){ driver=_driver; baseURL=_baseURL; verificationErrors=new StringBuilder(); if(%s) { selenium=new WebDriverBackedSelenium(driver, baseURL); selenium.Start(); } }
 public void _Quit(){ try { if(selenium!=null) selenium.Stop(); else driver.Quit(); } catch(Exception) { Console.Write("Warning: Quit() failed."); } }
 public String GetIID_ISelenium() { return typeof(ISelenium).GUID.ToString(); }

#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

str qmCode csCode line rx
int i j iid iLine iFunc nEmpty inCS isEmbeddedMultiline isAfterRet

csCode.from(sUsing cClassDef "[]")

 get text of macro or caller
iid=iif(empty(macro) getopt(itemid 1) qmitem(macro))
qmCode.getmacro(iid)
i=findl(qmCode 1)
if(i<0) goto g1 ;;eg macro encrypted

 erase QM code, format C# code
findrx "" "^\w+\.x\.(c\d+) *;;(.*)|[ ;]C#:(.*)|#ret\b.*" 0 128 rx
lpstr qmCode2=qmCode+i
foreach line qmCode2
	iLine+1
	lpstr s=line+strspn(line "[9],")
	if inCS
		sel s[0]
			case [32,';'] csCode.addline(s+1)
			case 0 csCode.addline(s)
			case else inCS=0
	if !inCS
		ARRAY(str) a
		if(findrx(s rx 0 0 a)<0) nEmpty+1; continue
		if a[1].len ;;x.x.c1 ;;C#
			csCode.formata("%.*mpublic void %s(){%s}[]" nEmpty 10 a[1] a[2])
			if(find(a[2] "//")>=0) csCode.set("[]}" csCode.len-3)
			iFunc=val(a[1]+1)
		else if a[3].len ;; C#:
			s+4; s+strspn(s " [9]")
			lpstr pub=0; if(!findrx(s "^[\w\.]+ +\w+ *\(")) pub="public "
			csCode.formata("%.*m%s%s[]" nEmpty 10 pub s)
			inCS=1
			isEmbeddedMultiline=1
		else ;;#ret
			_s.getl(qmCode2 iLine 2)
			csCode.formata("%.*m%s" nEmpty+1 10 _s)
			isAfterRet=1
			break
		nEmpty=0
 g1
if(!empty(myFunctions)) csCode.addline(myFunctions)
 csCode.formata(sFunctions iif(findrx(csCode "\bselenium\.")>=0 "true" "selenium!=null"))
csCode+sFunctions

if flags&0x100 ;;show C# code
	_s=csCode; _s.replacerx("\n{2,}")
	out F"<><Z 0xe080>C# code</Z>[]<code>{_s}</code>[]<Z 0xe080>C# options</Z>[]{___wdGlobal.csOptions}[]<Z 0xe080></Z>"

 compile C# code and create object
m_cs.SetOptions(___wdGlobal.csOptions)
m_cs.x.AddCode(csCode)

x=m_cs.CreateObject("User.Script")
