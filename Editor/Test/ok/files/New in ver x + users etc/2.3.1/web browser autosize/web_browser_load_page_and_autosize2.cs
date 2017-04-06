 /dlg_web_browser_autosize
function hwb $url

out

siz 300 0 hwb 2

opt waitmsg 1
web url 3 hwb
 out "loaded"

Htm el=htm("HTML" "" "" hwb)

MSHTML.IHTMLElement2 k=+el
int w=k.scrollWidth
rep
	out w
	0.5
	siz w+40 0 hwb 2
	if(k.scrollWidth<=w+20) break
	w=k.scrollWidth
