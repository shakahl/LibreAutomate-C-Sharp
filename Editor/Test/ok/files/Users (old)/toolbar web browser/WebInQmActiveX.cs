 /
function# $url [flags] [hwnd] [$url2] [str&geturl2] [&gethwnd] ;;flags: 1 wait, 2 in existing window, 4 in new window, 8 in new IE, 12 in new IE7 tab, 16 url2 exact, 32 get IES hwnd, 64 don't activate, hiword max wait time

 Opens url in a web browser control that is in a QM dialog or other QM window.
 The same as the intrinsic web function, but does not fail if the control currently is not displaying a web page (it may be empty or display a document of other type).

 hwnd - handle of the ActiveX control host control or parent window.
 Other arguments - the same as with web.

 Returns 1 if successful, 0 on error.
 Unlike web, does not return IWebBrowser2 interface pointer. If you need it, use _getcontrol.


 g1
int retry
web url 0 hwnd url2 geturl2 gethwnd
err ;;probably could not connect because is not displaying a web page. Open blank page and retry.
	if(retry) ret
	retry=1
	if(!childtest(hwnd "" "ActiveX")) hwnd=child("" "ActiveX" hwnd); if(!hwnd) ret
	SHDocVw.WebBrowser b._getcontrol(hwnd)
	b.Navigate("about:blank")
	rep() if(b.Busy) wait 0.01; else goto g1
ret 1
