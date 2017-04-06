function [hwndSeleniumIDE] [flags] ;;hwndSeleniumIDE: finds if 0.  flags: 1 only script

 Copies recorded script from Selenium IDE, converts to QM and stores in clipboard.
 Called by the toolbar that QM adds to Selenium IDE.


#region copy from Selenium IDE
spe 10
int w=hwndSeleniumIDE
if(!w) w=win("*Selenium IDE *" "MozillaWindowClass" "" 0x5)
act w

str s baseURL errMesTitle="QM - failed to get code from Selenium"
Acc a
 get base URL
if(a.Find(w "COMBOBOX" "" "a:id=baseURL" 0x4 0 0 "first")) baseURL=a.Value; err
baseURL.rtrim("/\")
 select tab Table
a.FindFF(w "tab" "" "id=editorTab" 0x3084)
int state=a.State
if(state&STATE_SYSTEM_UNAVAILABLE) mes "The ''Table'' tab must be available." errMesTitle "x"; ret
if(state&STATE_SYSTEM_SELECTED=0) a.DoDefaultAction
 focus the table
a.FindFF(w "tree" "" "id=commands" 0x3084) ;;or a.Navigate("pa n f2")
a.Select(1)
 select all, and get code through clipboard
key Ca
s.getsel; if(!s.len) ret

 is a C# formatter?
int i
if(s[0]='<') i=-1 ;;HTML, default
else i=findrx(s "\b(selenium|driver).[A-Z]")
if i<0
	_s=
	 Please select this menu item in Selenium IDE, then retry:
	     Options -> Clipboard Format -> C# Remote Control or WebDriver.
	;
	 If that does not work, look in Selenium Options, must be:
	     Formats -> C# Remote Control -> Variable: selenium.
	     Formats -> C# WebDriver -> Variable: driver.
	     Plugins -> Selenium IDE C# formatters: enabled. Or enable it in Firefox Add-ins.
	mes _s errMesTitle "x"
	ret
int format=iif(s[i]='s' 1 2)
#endregion

#region convert C# to QM
str q ;;QM code converted from C#
int isVerify
lpstr selVarDecl
sel format
	case 1 ;;call ISelenium functions directly from macro
	selVarDecl="ISelenium selenium="
	 convert strings to QM. To make easier, put in as and replace with "[1]index[2]" in s.
	ARRAY(str) as
	REPLACERX rr.frepl=&sub.Callback_str_replacerx; rr.paramr=&as
	s.replacerx("''[^''\\]*(?:\\.[^''\\]*)*''" rr)
	 convert multiline code - verifyX and waitForX
	s.replacerx("(?m)^\s*try\s+\{\s+([^\r\n]+)\s+\}\s+catch\s*([^\r\n]+)\s+\{\s*([^\r\n]*)\s*\}" "try { $1 } catch$2 { $3 }")
	s.replacerx("(?m)^\s*for ([^\r\n]+)\s+([^\r\n]+)\s+([^\r\n]+)\s+([^\r\n]+)\s+}" "for$1 $2  $3  $4 }")
	 remove indentation
	s.replacerx("(?m)^\s+")
	 convert Regex.IsMatch etc
	s.replacerx("\bRegex.IsMatch\b" "WdMatch")
	s.replacerx("\bString.Join\b" "WdJoin")
	if(s.replacerx("try \{ (Assert\..+?) } catch[^\r\n]+" "$1 err WdVerifyErr verificationErrors")) isVerify=1
	
	ARRAY(str) ac=s
	for i 0 ac.len
		str& k=ac[i]
		sel k.rtrim(";") 6
			case "selenium.*"
			sel k 2
				case "selenium.Break()" k="if(mes(''Continue?'' '''' ''YN?'')!='Y') goto somewhere"
				case "selenium.SendKeys(*" k+" ;;if SendKeys is missing, try to replace with Type"
				 also missing GetCssCount, used with storeX, assertX etc
				
			q.formata("%s[]" k)
			if i<ac.len-1 and ac[i+1].beg("selenium.WaitForPageToLoad(") ;;add in same line
				i+1
				q.set("; " q.len-2)
				q.formata("%s[]" ac[i])
			
			case "Assert.*"
			sel k+7 2
				case "IsTrue*" k.replacerx("^Assert.IsTrue\(([^;]+)\)" "WdAssertTrue $1" 4)
				case "IsFalse*" k.replacerx("^Assert.IsFalse\(([^;]+)\)" "WdAssertFalse $1" 4)
				case "AreEqual*" k.replacerx("^Assert.AreEqual\(([^,]+), *([^;]+)\)" "WdAssertTrue WdEqual($2 $1)" 4)
				case "AreNotEqual*" k.replacerx("^Assert.AreNotEqual\(([^,]+), *([^;]+)\)" "WdAssertFalse WdEqual($2 $1)" 4)
			q.addline(k)
			
			case "for*" ;;waitForX
			k.replacerx("^.+?try \{ if *\((.+)\) break.+" "foreach(0.5 60 WdWait) if($1) break" 4)
			k.replacerx("if\(([1]\d+[2]) *(?:=|(!))= *(.+)\) break$" "if($2WdEqual($3 $1)) break" 4) ;;"string" == Func()
			q.addline(k)
			
			case "$^((?:String|Number|Boolean)(?:\[\])?) (\w+) = (.+)" ;;storeX
			ARRAY(str) ar
			findrx(k "^((?:String|Number|Boolean)(?:\[\])?) (\w+) = (.+)" 0 0 ar)
			str qmType=iif((ar[1][0]='S') "str" "int"); if(ar[1].end("]")) qmType-"ARRAY("; qmType+")"
			q.formata("%s %s=%s[]" qmType ar[2] ar[3])
			
			case "Console.WriteLine(*"
			k.replacerx("^Console.WriteLine\((.+)\)$" "out $1" 4)
			q.addline(k)
			
			case "Thread.Sleep(*"
			if(findrx(k "\d+" 0 0 _s)>0) k=F"wait {val(_s)/1000.0}"
			q.addline(k)
			
			case "//*"
			q.formata(" %s[]" k+2)
			
			case else q.formata(" CANNOT CONVERT: %s[]" k)
	
	 strings were stored in as, and in q replaced with "[1]index[2]". Now put back to q.
	for(i as.len-1 -1 -1) q.findreplace(F"[1]{i}[2]" as[i] 4)
	
	case 2 ;;C# function Run() that calls IWebDriver functions
	if(findw(s "verificationErrors")>0) isVerify=2
	q.from("public void Run(){[]" s "}[]")
	if(findw(q "IsElementPresent")>0) q+"[]private bool IsElementPresent(By by) { try { driver.FindElement(by); return true; } catch (NoSuchElementException) { return false; } }[]"
	if(findw(q "IsAlertPresent")>0) q+"[]private bool IsAlertPresent() { try { driver.SwitchTo().Alert(); return true; } catch (NoAlertPresentException) { return false; } }[]"
	if(findw(q "CloseAlertAndGetItsText")>0) q+"[]private bool acceptNextAlert = true;[]private string CloseAlertAndGetItsText() { try { IAlert alert = driver.SwitchTo().Alert(); string alertText = alert.Text; if (acceptNextAlert) { alert.Accept(); } else { alert.Dismiss(); } return alertText; } finally { acceptNextAlert = true; } }[]"

 out q;act _hwndqm;ret
#endregion

#region head and tail
str sInit=
F
 #region Selenium, recorded {_s.timeformat}
 #compile "__WebDriver"
 WebDriver x.Init({iif(format=2 "__FUNCTION__" "")})
 str baseURL="{baseURL}"
 {selVarDecl}x.StartFirefox(baseURL) ;;StartFirefox, StartChrome or StartIE
;
if(isVerify=1) sInit+"str verificationErrors[]verificationErrors.all[]"
str sEnd=
 if(mes("Close browser?" "" "YN?2")='Y') x.Quit
 #endregion
;
if(isVerify) sEnd-iif(isVerify=1 "if(verificationErrors.len) out verificationErrors[]" "_s=x.x.VerificationErrors; if(_s.len) out _s[]")

if flags&1
	s=q
else
	sel format
		case 1 s.from(sInit "[]" q "[]" sEnd)
		case 2 s.from(sInit "[]x.x.Run[][]" sEnd "[][]#ret[]" q)
 out s;ret
#endregion

 set clipboard, show info, activate QM
s.setclip
act _hwndqm
OnScreenDisplay "Paste in QM" 0 0 0 0 0 0 4

err+ mes _error.description errMesTitle "x"


#sub Callback_str_replacerx
function# REPLACERXCB&x

ARRAY(str)& as=+x.rrx.paramr
str& s=as[]; s=x.match
x.match.from("[1]" as.len-1 "[2]")
s.trim("''")
s.findreplace("\\" "\")
s.findreplace("\''" "''")
s.findreplace("\'" "'")
s.findreplace("\n" "[10]")
s.findreplace("\r" "[13]")
s.findreplace("\t" "	")
s.findreplace("\a" "[7]")
s.findreplace("\b" "[8]")
s.findreplace("\v" "[11]")
s.findreplace("\f" "[12]")
s.escape(1)
s-"''"; s+"''"
