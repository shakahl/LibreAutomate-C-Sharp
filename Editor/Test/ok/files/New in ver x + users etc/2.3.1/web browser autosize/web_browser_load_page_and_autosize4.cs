 /dlg_web_browser_autosize
function hwb $url

out

siz 300 0 hwb 2

opt waitmsg 1
SHDocVw.WebBrowser b=web(url 3 hwb)
 out "loaded"

MSHTML.IHTMLDocument2 d=b.Document
MSHTML.IHTMLWindow2 w=d.parentWindow
 w.scrollBy(100 0)
 0.1
 out d.body.offsetLeft

 MSHTML.IHTMLWindow2 w3=+w
 out w3.screenLeft

out d.

 Htm el=htm("HTML" "" "" hwb)
 
 MSHTML.IHTMLElement2 k=+el
 int w=k.scrollWidth
 rep
	 out w
	 0.5
	 siz w+40 0 hwb 2
	 if(k.scrollWidth<=w+20) break
	 w=k.scrollWidth
