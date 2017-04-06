function [hwnd] [str&url] [str&title] [str&html] [str&text]

 Gets web page properties.

 hwnd - IE window handle. Also can be child window of class "Internet Explorer_Server".
   If used and not 0, gets properties of main web page. This variable may be not initialized.
   Else gets properties of web page where this element is. It may be main web page or frame/iframe.
 url, title, html, text - variables that receive page properties. Can be 0.

 Added in: QM 2.3.3.

 ERRORS
 ERR_HWND - hwnd invalid (if hwnd not 0 but invalid).
 ERR_INIT - this variable empty (if hwnd 0).
 ERR_OBJECT - document node not found.
 ERR_FAILED - failed to get a property.


MSHTML.IHTMLDocument2 doc
if(hwnd)
	doc=htm(hwnd)
	err end ERR_HWND
else
	if(!el) end ERR_INIT
	doc=el.document

if(&url) url=doc.url
if(&title) title=doc.title
if(&html)
	MSHTML.IHTMLDocument3 doc3=+doc
	html=doc3.documentElement.outerHTML
if(&text) text=doc.body.innerText

err+ end ERR_FAILED
