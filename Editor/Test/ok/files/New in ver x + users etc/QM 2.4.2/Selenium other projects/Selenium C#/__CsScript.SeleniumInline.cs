 /Selenium C# WebDriver inline
function'IDispatch $macro $browser $baseURL [flags] [$codeBefore] [$codeAfter] [$CsScriptOptions] ;;browser: "firefox", "chrome", "ie", "".  flags: 0x100 show C# code

 default C# strings
if(empty(codeBefore)) codeBefore="using System;using System.Text;using System.Text.RegularExpressions;using System.Threading;using OpenQA.Selenium;using OpenQA.Selenium.Firefox;using OpenQA.Selenium.Chrome;using OpenQA.Selenium.IE;using OpenQA.Selenium.Support.UI;namespace Selenium{public class Script{private IWebDriver driver;private string baseURL;"
else if(findc(codeBefore 10)>=0) _s=codeBefore; _s.replacerx("[\r\n]"); codeBefore=_s; if(find(_s "//")>=0) end "codeBefore should not contain //comments. Use /*comments*/ instead." 8
if(empty(codeAfter)) codeAfter="public void StartFirefox(string _baseURL){driver = new FirefoxDriver();baseURL = _baseURL;}public void StartChrome(string _baseURL){driver = new ChromeDriver();baseURL = _baseURL;}public void StartIE(string _baseURL){driver = new InternetExplorerDriver();baseURL = _baseURL;}public void End(){try { driver.Quit(); } catch (Exception) {}}}}"
if(empty(CsScriptOptions)) CsScriptOptions="references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium[]searchDirs=$qm$\Selenium\C#"

#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1
 #opt nowarnings 1

str qmcode cscode line rx
int i j iid iLine nEmpty
ARRAY(POINT) alf ;;map iLine to iFunc, for error link

 get text of macro or caller
iid=iif(empty(macro) getopt(itemid 1) qmitem(macro))
qmcode.getmacro(iid)
i=findl(qmcode 1); if(i<0) ret

 erase QM code, format C# code
cscode.from(codeBefore "[]")
findrx "" "^[\t,]*t\.(c\d+) *;;([^\r\n]+)" 0 128 rx
foreach line qmcode+i
	iLine+1
	ARRAY(str) a
	if(findrx(line rx 0 0 a)<0) nEmpty+1; continue
	cscode.formata("%.*mpublic void %s(){%s}[]" nEmpty 10 a[1] a[2])
	if(find(a[2] "//")>=0) cscode.set("[]}" cscode.len-3)
	nEmpty=0
	POINT& p=alf[]; p.x=iLine+1; p.y=val(a[1]+1)
if(!alf.len) end "The macro must contain one or more lines like t.c1 ;;C# code"
cscode+codeAfter

if flags&0x100
	_s=cscode; _s.replacerx("\n{2,}" "")
	out F"<><Z 0xe080>C# code</Z>[]<code>{_s}</code>[]<Z 0xe080>C# options</Z>[]{CsScriptOptions}[]<Z 0xe080></Z>"

 compile C# code
SetOptions(CsScriptOptions)
x.AddCode(cscode)
err sub.OnError iid alf; end _error

IDispatch t=x.CreateObject("Selenium.Script")

sub.SetPath

sel browser 1
	case ""
	case "firefox" t.StartFirefox(baseURL)
	case "chrome" t.StartChrome(baseURL)
	case ["ie","internet explorer"] t.StartIE(baseURL)
	case else end "browser must be ''firefox'', ''chrome'', ''ie'', ''internet explorer'' or ''''"

ret t


#sub OnError
function iid ARRAY(POINT)&alf

str& s=_error.description
 out s
str macro.getmacro(iid 1)
int i j k iLine iFunc iPos
ARRAY(CHARRANGE) a
if(findrx(s "^\t*C#\((\d+),(\d+)\)" 0 8 a)<0) ret
i=a[0].cpMin; j=a[0].cpMax-i
iLine=val(s+a[1].cpMin); iPos=val(s+a[2].cpMin)
for(k 0 alf.len) if(alf[k].x=iLine) iFunc=alf[k].y; break
if(iFunc) s.replace(F"[9]<macro ''Scripting_Link /{iLine} {iPos} 2 ''{macro}''>c{iFunc}</macro>" i j)
else if(iLine=1) s.replace(F"[9]codeBefore" i j)
else s.replace(F"[9]codeAfter" i j)


#sub SetPath

 Adds Selenium folder path to the PATH environment variable for this process, if not already added.
 Alternatively you can add it in Control Panel for all processes. Then restart QM.
 Don't need this for Firefox.

str s1.expandpath("$qm$\\Selenium") s2
GetEnvVar("PATH" s2)
if(find(s2 s1 1)>=0) ret
s2-";"; s2-s1
SetEnvVar("PATH" s2)
