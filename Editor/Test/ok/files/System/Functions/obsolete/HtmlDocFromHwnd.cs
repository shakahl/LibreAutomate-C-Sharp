 /
function'MSHTML.IHTMLDocument2 [hwnd] [MSHTML.IHTMLWindow2&Window] [flags]

 Obsolete. Use <help>htm</help>.


MSHTML.IHTMLDocument2 doc
if(hwnd) doc=htm(hwnd); else doc=htm(0)
if(&Window) Window=doc.parentWindow
ret doc
err+
