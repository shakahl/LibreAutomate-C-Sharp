 /
function [waitmax] [hwnd] [$urlmustcontain] [str&urlout] [int&hwndout]

 Waits while Internet Explorer is busy.
 Obsolete. Use <help>wait</help> or <help>web</help>.

 waitmax is max wait time (0 - infinite).
 hwnd can be handle of IE window, or 0.


int fl=1|(waitmax<<16)
if(&hwndout) fl|0x1000
if(hwnd)
	if(&urlout) web "" fl hwnd urlmustcontain urlout
	else web "" fl hwnd urlmustcontain
else
	if(&urlout) web "" fl 0 urlmustcontain urlout
	else web "" fl 0 urlmustcontain
if(&hwndout) hwndout=tls9
err+ end _error
