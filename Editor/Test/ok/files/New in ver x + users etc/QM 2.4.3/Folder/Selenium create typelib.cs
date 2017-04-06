 Most interfaces are not COM-visible. ISelenium visible.

out
SetCurDir "Q:\Downloads\Selenium\C#"
str tlbexp="C:\Program Files\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\TlbExp.exe"
Dir d
foreach(d "Q:\Downloads\Selenium\C#\*.dll" FE_Dir)
	str path=d.FullPath
	out path
	RunConsole2 F"''{tlbexp}'' {path} /verbose"
