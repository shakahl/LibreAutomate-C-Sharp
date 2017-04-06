 /Selenium C# WebDriver2
function'IDispatch $code $browser $baseURL ;;browser: "firefox", "chrome", "ie"

#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1
#opt nowarnings 1

str so=
 searchDirs=$qm$\Selenium\C#
 references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium
SetOptions(so)

 AddCode("")
str ss sourceFile
code=_GetCode(code ss sourceFile _i)
x.AddCode(code 0 sourceFile)

IDispatch t=x.CreateObject("Selenium.Script")

sub.SetPath

sel browser 1
	case ""
	case "firefox" t.StartFirefox(baseURL)
	case "chrome" t.StartChrome(baseURL)
	case ["ie","internet explorer"] t.StartIE(baseURL)
	case else end "browser must be ''firefox'', ''chrome'', ''ie'', ''internet explorer'' or ''''"

ret t


#sub SetPath

 Adds Selenium folder path to the PATH environment variable for this process, if not already added.
 Alternatively you can add it in Control Panel for all processes. Then restart QM.
 Don't need this for Firefox.

str s1.expandpath("$qm$\\Selenium") s2
GetEnvVar("PATH" s2)
if(find(s2 s1 1)>=0) ret
s2-";"; s2-s1
SetEnvVar("PATH" s2)
