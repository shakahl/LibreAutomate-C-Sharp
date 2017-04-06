function $HTML

 fix IE bug: when loading from memory, may recognize HTML as text. Only older IE 6 (XP SP0).
lpstr tmp
if _iever=0x600
	if !rget(_s "Version" "Software\Microsoft\Internet Explorer" HKEY_LOCAL_MACHINE) or StrCompare(_s "6.0.2900.")<0
		tmp="$temp qm$"

HtmlToWebBrowserControl 0 HTML 60 d tmp

err+ end _error
