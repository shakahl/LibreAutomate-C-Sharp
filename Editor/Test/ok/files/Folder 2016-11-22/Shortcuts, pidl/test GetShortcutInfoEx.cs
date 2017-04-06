out
str s
 s="$desktop$\ClassicStartMenu.exe - Shortcut.lnk"
 s="c:\ProgramData\Microsoft\Windows\Start Menu\Programs\Accessories\Math Input Panel.lnk"
s="c:\Users\All Users\Microsoft\Windows\Start Menu\Programs\Microsoft Office Excel Viewer.lnk"
s="c:\ProgramData\Microsoft\Windows\Start Menu\Programs\NVDA\NVDA web site.lnk"
SHORTCUTINFO x
if GetShortcutInfoEx(s &x)
	out x.target
	out x.initdir
	out x.iconfile


#ret
INSTALLSTATE_NOTUSED The component being requested is disabled on the computer.
 
INSTALLSTATE_ABSENT The component is not installed.
 
INSTALLSTATE_INVALIDARG One of the function parameters is invalid.
 
INSTALLSTATE_LOCAL The component is installed locally.
 
INSTALLSTATE_SOURCE The component is installed to run from source.
 
INSTALLSTATE_SOURCEABSENT The component source is inaccessible.
 
INSTALLSTATE_UNKNOWN
