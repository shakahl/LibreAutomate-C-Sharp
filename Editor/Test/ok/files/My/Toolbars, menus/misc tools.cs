>Monitor
	perfmon.msc :run "perfmon.msc" "" "" "" 0x10000
	Process Monitor (registry/file/etc) :run "Q:\Programs\ProcessMonitor\Procmon.exe" "" "" "*" 0x10000
	SmartSniff (TCP monitor of network card) :run "C:\program files\SmartSniff\smsniff.exe" ;;"" "" ""  0x10000 ;;run with consent, or the required driver does not work for some reason
	SocketSniff (TCP monitor of a process) :run "$program files$\SmartSniff\socketsniff\SocketSniff.exe"
	Fiddler4 :run "$program files$\Fiddler2\Fiddler.exe" "" "" "*"
	<
>Debug
	Dbgview :run "Q:\Programs\DebugView\Dbgview.exe" "" "" "" 0x10000
	Heavyload.exe :run "$program files$\hload\Heavyload.exe"
	Standard User Analyzer :run "$program files$\Standard User Analyzer\SUA.exe" "" "" "*" 0x10000
	TreeSizeFree.exe :run "$program files$\hload\TreeSizeFree.exe"
	WinDbg :run "$program files$\Debugging Tools for Windows (x86)\windbg.exe" "" "" "*" 0x10000
	<
>Resources, icons
	IcoFX :run "$program files$\IcoFX 1.6\IcoFX.exe"
	Resource Hacker :run "$program files$\Resource Hacker\ResourceHacker.exe" "" "" "*"
	ResourcesExtract :run "$program files$\resourcesextract\ResourcesExtract.exe"
	 XNResourceEditor :run "$program files$\XN Resource Editor\XNResourceEditor.exe"
	<
>Rarely used
	AlternateStreamView :run "Q:\Programs\AlternateStreamView\AlternateStreamView.exe"
	Depends 32bit :run "Q:\Programs\Depends32\depends.exe"
	Depends 64bit :run "Q:\Programs\Depends64\depends.exe"
	PEview :run "Q:\Programs\PeView\PEview.exe" "" "" "*"
	<
Autoruns :run "Q:\programs\Autoruns\autoruns.exe" "" "" "" 0x10000
Character Map :run "$system$\charmap.exe"
 certmgr.msc :run "$system$\en-US\certmgr.msc" "" "" "*" 0x10000
Command Prompt :run "$system$\cmd.exe" "" "" "%HOMEDRIVE%%HOMEPATH%" 0x10000
Computer Management :run "$system$\compmgmt.msc" "/s" "" "%HOMEDRIVE%%HOMEPATH%" 0x10000
Disk Cleanup :run "$system$\cleanmgr.exe" "" "" "" 0x10000
ExamDiff :run "$program files$\ExamDiff\ExamDiff.exe"
ILSpy :run "Q:\Programs\ILSpy\ILSpy.exe"
IrfanView :run "$program files$\IrfanView\i_view32.exe"
oleview :run "C:\Program Files\Microsoft SDKs\Windows\v7.0\Bin\x64\Oleview.exe" "" "" "*" 0x10000
 perfmon3.msc :run "$programs$\Administrative Tools\perfmon3.msc" "" "" "$programs$\Administrative Tools"
Services :run "$system$\services.msc" "" "" "%HOMEDRIVE%%HOMEPATH%" 0x10000
SyncBack :run "$program files$\SyncBackFree\SyncBackFree.exe" "" "" "*" ;;very slow icon extraction, don't know why * $program files$\SyncBackFree\RemBlankPwd.exe
System Restore :run "$system$\SystemPropertiesProtection.exe" "" "" ""  0x14000
Task Scheduler :run "$system$\taskschd.msc" "/s" "" "" 0x10000
 TSLite3 :run "$program files$\Bradbury\TopStyle3\TSLite3.exe"
UI Spy (x64) :run "C:\Program Files\Microsoft SDKs\Windows\v7.0\Bin\x64\UISpy.exe" "" "" "*" 0x10000
 What's Running :run "$program files$\WhatsRunning\WhatsRunning.exe" "" "" "*" 3|0x10000
-
>macro programs
	AutoMate :run "$program files$\AutoMate 6\AMTA.exe" "" "" "" 0x10000
	AM6.chm :run "$program files$\AutoMate 6\AM6.chm"
	-
	AutoIt :run "$program files$\AutoIt3\SciTE\SciTE.exe" "" "" "*"
	AutoIt3.chm :run "$program files$\AutoIt3\AutoIt3.chm"
	UDFs3.chm :run "$program files$\AutoIt3\UDFs3.chm"
	-
	AutoHotkey.chm :run "$program files$\AutoHotkey\AutoHotkey.chm"
	-
	PhraseExpress :run "$program files$\PhraseExpress\phraseexpress.exe" "" "" "" 0x10000
	AllChars :run "C:\Program Files\AllChars\AllChars.exe" "" "" "" 0x10000
	<
