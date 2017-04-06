 /
function $urlFile $urlTarget

 Creates Internet shortcut (.url) file.
 Error if fails.

 urlFile - shortcut file.
 urlTarget - URL (Internet address).

 EXAMPLE
 CreateInternetShortcut "$desktop$\Quick Macros website.url" "http://www.quickmacros.com/"


Wsh.IWshShell_Class sh._create
Wsh.IWshURLShortcut_Class us=sh.CreateShortcut(_s.expandpath(urlFile))
us.TargetPath=urlTarget
us.Save
err+ end _error
