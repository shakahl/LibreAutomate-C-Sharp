qmcs (CsScript) :run "$qm$\qmcs\qmcs.sln"
sqlite :run "$qm$\sqlite\sqlite.sln"
TextCapture :run "$qm$\TextCapture\TextCapture.sln"
qmshex :run "$qm$\qmshex\qmshex.sln" "" "" "" 0x10000 ;;need admin because runs regsvr32
zip :run "$qm$\zip\qmzip.sln"
libraries (pcre, lzo, profiling) :run "$qm$\qmcore\libraries.sln"
app_plus (ctoqm, qmexeresdll, qmsetup_dll, QuickMacrosPortable) :run "$qm$\app_plus\app_plus.sln"
 tccdll.sln :run "$qm$\tccqm\project\tccdll.sln"
tcc :run "$qm$\tcc\project\tccdll.sln"
qmphook :run "$qm$\qmphook\qmphook.sln"
xdiff :run "$qm$\xdiff\xdiff.sln"
-
app admin :run "%app%\app.sln" "" "" "*" 0x10000
-
VS15 (2017) :run "$program files$\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"
VS15 (2017) :run "$program files$\Microsoft Visual Studio\2017\Community"
VS14 (2015) :run "$program files$\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"
VC14 (2015) :run "$program files$\Microsoft Visual Studio 14.0\VC"
VS9 (2008) :run "$program files$\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
VS9 (2008) admin :run "$program files$\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe" "" "" "" 0x10000
VC9 (2008) :run "$program files$\Microsoft Visual Studio 9.0\VC"
Win7 SDK :run "$program files$\Microsoft SDKs\Windows\v7.1A"
Win10 SDK :run "Q:\SDK10"
-
>old
	VS Express 2013 :run "$program files$\Microsoft Visual Studio 12.0\Common7\IDE\WDExpress.exe"
	Microsoft Visual C++ 6.0 :run "$program files$\Microsoft Visual Studio\Common\MSDev98\Bin\MSDEV.EXE"
	Microsoft Visual Basic 6.0 :run "$program files$\Microsoft Visual Studio\VB98\VB6.EXE"
	<
-
TIF :mac+ "typeinfo.ini" * .ini
