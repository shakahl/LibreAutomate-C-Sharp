function [hwnd] [str&url] [str&title] [str&html] [str&text]

 Gets web page properties.

 hwnd - web browser window handle.
   If used and not 0, gets properties of main web page. This variable may be not initialized.
   Else gets properties of web page where this node is. It may be main web page or frame/iframe.
 url, title, html, text - variables that receive page properties. Can be 0.
   In Chrome cannot get html and text. Error if html or text used.

 ERRORS
 ERR_HWND - hwnd invalid (if hwnd not 0 but invalid).
 ERR_INIT - this variable empty (if hwnd 0).
 ERR_OBJECT - document node not found.
 ERR_FAILED - failed to get a property.


FFNode f
if(hwnd) f.FromDoc(hwnd); else f=Root
err+ end _error

if &url or &title
	FFDOM.ISimpleDOMDocument doc=+f.node
	err end ERR_OBJECT
	
	if(&url) url=doc.URL
	if(&title) title=doc.title

if &html or &text
	f.FindFF(f "1 HTML" 0 0 0x40)
	 if not found, it's probably Chrome. Cannot get node HTML anyway.
	if(!&html) &html=_s
	html=f.HTML(1)
	
	if &text
		HtmlDoc hd.SetOptions(1)
		hd.InitFromText(html)
		text=hd.GetText

err+ end ERR_FAILED
