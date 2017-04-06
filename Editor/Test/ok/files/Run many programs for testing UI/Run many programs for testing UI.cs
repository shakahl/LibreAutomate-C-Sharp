 Launches list of programs.
 Skips those that are currently running.

str sList=
 C:\Program Files (x86)\Notepad++\notepad++.exe
 C:\Program Files (x86)\HexEdit\HexEdit.exe
 C:\Users\G\AppData\Local\Programs\Python\Python35-32\pythonw.exe
 C:\Program Files (x86)\SQLite Expert\Personal 3\SQLiteExpertPers.exe
 C:\Program Files\LockHunter\LockHunter.exe
 C:\Program Files (x86)\Free YouTube Downloader\YouTubeDownloader.exe
 C:\Program Files\AutoHotkey\SciTE\SciTE.exe
 C:\Program Files (x86)\Audacity\audacity.exe
 C:\Program Files (x86)\EWSoftware\Sandcastle Help File Builder\SandcastleBuilderGUI.exe
 C:\Program Files (x86)\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe
 C:\Program Files (x86)\Inno Setup 5\Compil32.exe
 C:\Windows\System32\mmc.exe
 C:\Program Files (x86)\SyncBackFree\SyncBackFree.exe
 C:\Windows\SysWOW64\charmap.exe
 Q:\Programs\PEview\PEview.exe
 C:\Program Files (x86)\Resource Hacker\ResourceHacker.exe
 C:\Program Files (x86)\Fiddler2\Fiddler.exe
 C:\Program Files (x86)\VMware\VMware Player\vmplayer.exe
 C:\Program Files (x86)\PicPick\picpick.exe
 C:\Program Files (x86)\Microsoft Office\OFFICE11\WINWORD.EXE
 C:\Program Files (x86)\Microsoft Office\OFFICE11\EXCEL.EXE
 C:\Program Files (x86)\Windows NT\Accessories\wordpad.exe
 C:\Program Files (x86)\Microsoft Help Viewer\v2.3\HlpViewer.exe
 C:\Program Files (x86)\Common Files\Microsoft Shared\Help 9\dexplore.exe
 C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe
 C:\Program Files (x86)\FileZilla FTP Client\filezilla.exe
 C:\Program Files (x86)\Mozilla Firefox\firefox.exe

spe 500
str s sn
foreach s sList
	sn.getfilename(s)
	if(win("" "" sn)) continue
	 out s
	run s
