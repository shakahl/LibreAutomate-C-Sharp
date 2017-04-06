 /
function hwndCtrl $HTML [waitmax] [MSHTML.IHTMLDocument2'doc]

 Loads HTML into web browser control.
 Error if fails.
 To load HTML file, instead use str.setwintext or WebBrowser.Navigate.
 Added in QM 2.3.2.

 hwndCtrl - handle of control that hosts the web browser control. Normally it is like id(3 hDlg).
 HTML - string containing HTML.
 waitmax - max time (s) to wait until finished loading. Error on timeout. If 0, does not wait.
    Without waiting you may not be able to access the DOM immediately after calling this function, because the HTML is not finished loading.
    To wait later, can be used code like this: web "" waitmax<<16|1 hwndCtrl
 doc - if used, loads HTML into the document. Else gets document from the web browser control.
    The document can be not attached to a web browser control.
    hwndCtrl not used.

 Notes:
 To be notified about link clicks, use BeforeNavigate2 event.
 Web pages loaded from string have some problems:
    Cannot use relative links, because the base url cannot be specified.
    Internet security zone. For example, javascript may not work if turned off in IE Options. Cannot open local script and other files.


if(!doc)
	SHDocVw.WebBrowser wb._getcontrol(hwndCtrl)
	doc=wb.Document
	if(!doc) _s.setwintext(hwndCtrl); doc=wb.Document

if(HTML=0) HTML=""

ARRAY(VARIANT) a.create(1)
a[0]=HTML
doc.write(a)
0

err+ end _error

 if rema
	 MSHTML.IHTMLDocument3 d3=+doc
	 d3.documentElement.removeAttribute("bugfix" 1); err


 Tried to add arg for template file + body.innerText, but problems with encoding etc.
 More notes in app::HtmlWrite and in dlg_tools.
 Various ways of loading HTML: http://qualapps.blogspot.com/2008/10/how-to-load-mshtml-with-data.html
 However with IPersistMoniker works only with IE 5. In my tests, Load failed with IE 6 and 8.
