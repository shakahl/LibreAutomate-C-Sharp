 /
function'SHDocVw.WebBrowser MSHTML.IHTMLDocument2&doc

SHDocVw.WebBrowser wb
IServiceProvider sp=+doc
sp.QueryService(uuidof(SHDocVw.IWebBrowserApp) uuidof(SHDocVw.IWebBrowser2) &wb)
ret wb
