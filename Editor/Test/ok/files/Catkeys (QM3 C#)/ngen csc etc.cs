out
SetCurDir("%catkeys%\tasks")

str ngen queue
ngen="install"
 queue=" /queue:1"
 ngen="uninstall"
 ngen="update"
 ngen="display" ;;don't need admin

str f name
 f="%catkeys%\tasks\catkeys.dll"; name="Catkeys, Version=1.0.0.0, Culture=neutral, PublicKeyToken=112db45ebd62e36d"
 f="%catkeys%\tasks\Microsoft.CodeAnalysis.CSharp.dll"
 f="%catkeys%\tasks\Microsoft.CodeAnalysis.CSharp.Scripting.dll"
 f="%catkeys%\tasks\System.Data.Sqlite.dll"; name="System.Data.SQLite, Version=1.0.102.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"
 f="%catkeys%\tasks\Sqlite.dll"; name="SQLite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=112db45ebd62e36d"

 f="%catkeys%\tasks\csc.exe"
 if 0 ;;don't add csc to GAC if want to unload compiler appdomain
	 name="csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	 sub.GAC(ngen "%catkeys%\tasks\Microsoft.CodeAnalysis.dll")
	 sub.GAC(ngen "%catkeys%\tasks\Microsoft.CodeAnalysis.CSharp.dll")
	 sub.GAC(ngen "%catkeys%\tasks\System.Reflection.Metadata.dll")
 f="%catkeys%\tasks\CatkeysTasks.exe"
 f="%catkeys%\tasks\Tests.exe"
 f="%catkeys%\tasks\catkeys.compiler.exe"
 f="%catkeys%\Test Projects\Wpf\Wpf.exe"
 f="Q:\app\Catkeys\Test Projects\WinForms\bin\Debug\WinForms.exe"
f="%catkeys%\Editor\CatEdit.exe"
 f="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\Roslyn\VBCSCompiler.exe"
 f="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
 f="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\PerfWatson2.exe"

 GAC
f.expandpath
if name.len and ngen.end("install")
	sub.GAC ngen f
	f=name

 NGEN
if ngen.len
	sub.NGEN ngen queue f


#sub GAC
function ~ngen ~f
str gacutil.expandpath("%catkeys%\GAC\gacutil.exe")
str iu.get(ngen 0 1)
if(iu="u") f.getfilename
RunConsole2(F"''{gacutil}'' /nologo /{iu} {f}" 0 "" 0x100)


#sub NGEN
function ~ngen ~queue ~f

int bits; str sb
 bits
RunConsole2 F"''C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\CorFlags.exe'' ''{f}'' /nologo" _s
if(find(_s "32BITREQ  : 1")>0 or find(_s "32BITPREF : 1")>0) bits=1
else if(f.endi(".exe")) bits=2
else bits=3
 out bits; ret

int i
for i 0 2
	if(1<<i&bits=0) continue
	str is64=iif(i=1 "64" "")
	 out is64; continue
	str ngenExe=F"C:\Windows\Microsoft.NET\Framework{is64}\v4.0.30319\ngen.exe"
	PF
	str cl=F"{ngenExe} {ngen}"
	if(ngen!="update") cl+F" ''{f}''"
	if(ngen="install") cl+queue
	cl+" /nologo"
	 out cl
	RunConsole2 cl
PN;PO
