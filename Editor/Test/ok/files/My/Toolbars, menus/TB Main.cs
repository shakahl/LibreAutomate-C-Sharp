 /mov0 199 -1 /siz0 277 228 /ssiz0 277 3 /set0 0x648B 0xFCFF
	menu my folders :mac "my folders" * $Favorites$
	menu My Computer :mac "menu My Computer" * $17$
	Control Panel :run "$3$"
	Settings app :run "shell:AppsFolder\windows.immersivecontrolpanel_cw5n1h2txyewy!microsoft.windows.immersivecontrolpanel"
app :run "%app%"
Cat :run "%catkeys%"
 CatEd :run "%catkeys%\Editor"
 Tasks :run "%catkeys%\Tasks"
mq :run "$My QM$"
dl :run "%downloads%"
Test :run "Q:\Test"
-
	Notepad :run "notepad.exe" "" "" "$Desktop$"
 	PSPad editor :run "$program files$\PSPad editor\PSPad.exe"
	WordPad :run "$program files$\Windows NT\Accessories\wordpad.exe" "" "" "%HOMEDRIVE%%HOMEPATH%"
	Word :run "$program files$\Microsoft Office\OFFICE11\WINWORD.EXE"
	Excel :run "$program files$\Microsoft Office\OFFICE11\EXCEL.EXE"
	menu Office :mac "menu Office"
 	Calculator :run "calc.exe"
	Calculator :run "shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"
	Paint :run "mspaint.exe"
	picpick :run "$program files$\PicPick\picpick.exe"
 	IcoFX :run "$program files$\IcoFX 1.6\IcoFX.exe"
 	Gimp 2 :run "$program files$\Gimp-2.0\bin\gimp-2.6.exe" "" "" "%USERPROFILE%\Pictures"
-
	Spy++ :run "$program files$\Microsoft Visual Studio 9.0\Common7\Tools\spyxx.exe" "" "" "" 0x20000
	Dbgview :run "Q:\Programs\DebugView\Dbgview.exe" "" "" "" 0x10000
 	Character Map :run "$system$\charmap.exe"
 	Command Prompt :run "$system$\cmd.exe" "" "" "%HOMEDRIVE%%HOMEPATH%" 0x10000 * %SystemRoot%\system32\cmd.exe
	Regedit :sub.Regedit * C:\WINDOWS\REGEDIT.EXE
	Process Explorer :run "Q:\Programs\ProcessExplorer\procexp.exe" "" "" "" 0x10000
	Process Hacker :run "C:\Program Files\Process Hacker 2\ProcessHacker.exe" "" "" "*" 0x10000
 	ILSpy :run "Q:\Programs\ILSpy\ILSpy.exe"
	VM :mac "Virtual PC" *.vmx
	misc tools, macro programs :mac "misc tools"
-
	PAD :run "http://publisher.appvisor.com/"; OnScreenDisplay "login: gintarasdi[]password: *1"
	Inno Compiler - quickm21 :run "$program files$\Inno Setup 5\Compil32.exe" "%app%\quickm21.iss" "" "%app%"
 	TIF :mac+ "typeinfo.ini" * .ini
 	Microsoft Expression Web 4 :run "$program files$\Microsoft Expression\Web 4\ExpressionWeb.exe"
	Adobe Dreamweaver CS4 :run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe"
	HTML Help Workshop :run "$program files$\HTML Help Workshop\hhw.exe"
 	app :run "%app%\app.sln" "" "" "*" 0x10000 ;;something did not work if runs as User. Don't remember, now everything works.
 	app :run "%app%\app.sln" "" "" "*" 0x30000
	CatkeysHelp.sln :run "$qm$\Catkeys\Help\CatkeysHelp.sln"
	Sandcastle Help File Builder GUI :run "$program files$\EWSoftware\Sandcastle Help File Builder\SandcastleBuilderGUI.exe"
 	app :run "%app%\app.sln" "" "" "*"
	app :run "$program files$\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe" "%app%\app.sln"
 	Catkeys VS2015 :run "$qm$\Catkeys\Catkeys.sln"
	Catkeys VS2017 :run "$program files$\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe" "$qm$\Catkeys\Catkeys.sln"
	dev programs, folders, projects :mac "dev programs and projects"
 	Run Inkscape with icon :mac "Run Inkscape with icon" *"$program files$\Inkscape\inkscape.exe"
-
	Id :run "%doc%\Id.xls"
	QM Help :run "%app%\QM2Help.chm" "" "" "%app%"
	misc help :mac "menu Help"
 	VS 2003 :run "$common files$\microsoft shared\Help\dexplore.exe" "/helpcol ms-help://MS.MSDNQTR.2004JUL.1033"
7 :run "$common files$\microsoft shared\Help 9\dexplore.exe" "/helpcol ms-help://MS.W7SDK.1033 /LaunchNamedUrlTopic DefaultPage /usehelpsettings WindowsSDK.1.0"
 8 :run "$program files$\Microsoft Help Viewer\v2.2\HlpViewer.exe" "/catalogName VisualStudio14"
10 :run "$program files$\Microsoft Help Viewer\v2.3\HlpViewer.exe" "/catalogName VisualStudio15"
 life :mac "life ideas"
-
	FileZilla :run "$program files$\FileZilla FTP Client\filezilla.exe" "" "" "*"
	Firefox :run "$program files$\Mozilla Firefox\firefox.exe" "" "" "*"
	Thunderbird :run "$program files$\Mozilla Thunderbird\thunderbird.exe" "" "" "*"
	Google Chrome :run "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
	Internet Explorer :run "$program files$\Internet Explorer\IEXPLORE.EXE" "" "" "*"

 -
 	multimedia :mac "multimedia"
 	Screensaver :ScreenSaverRun * $system$\shell32.dll * 25
 	Screensaver :run "$system$\mystify.scr"
	Magnifier :run "$system$\magnify.exe"
-
 temporary items, used for testing something
Apps :mac "menu Apps"


#sub Regedit m
sel ShowMenu("1 Regedit 32-bit[]2 Regedit 64-bit")
	case 0 ret
	case 2 _i=0x4000
run "C:\WINDOWS\REGEDIT.EXE" "" "" "" 0x20000|_i
