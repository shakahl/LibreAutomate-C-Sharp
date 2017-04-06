 /
function# $url [waits] [hwnd] [str&urlout] [int&hwndout]

 Obsolete. Use <help>web</help>.


if(waits<0) waits=1; else if(waits) waits=1|(waits<<16)
if(&hwndout) waits|0x1000
if(hwnd)
	if(&urlout) web(url waits hwnd "" urlout)
	else web(url waits hwnd)
else
	if(&urlout) web(url waits 0 "" urlout)
	else web(url waits 0)
if(&hwndout) hwndout=GetAncestor(tls9 2)

err+ ret 1
