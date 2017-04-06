 /Dialog90
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


interface# ___IPersist :IUnknown a
interface# ___IPersistFile :___IPersist b Load(@*pszFileName dwMode) {0000010b-0000-0000-C000-000000000046}

Q &q
if(!doc)
	SHDocVw.WebBrowser wb._getcontrol(hwndCtrl)
	doc=wb.Document
	if(!doc) _s.setwintext(hwndCtrl); doc=wb.Document

if(HTML=0) HTML=""
if(_unicode and DetectStringEncoding(HTML)=1 and findrx(HTML "(?i)<meta\s+http-equiv\s*=\s*[''']content-type['''][^>]+\bcharset\b")<0)
	lpstr BOM="[0xef][0xbb][0xbf]"
	if(StrCompareN(HTML BOM 3)) HTML=_s.from(BOM HTML)
Q &qq

_s=HTML
str sf.expandpath(F"$temp$\qm_htwbc_{&_s}.htm")
out sf
_s.setfile(sf)
Q &qqq
___IPersistFile pf=+doc
pf.Load(@sf STGM_READ|STGM_SHARE_DENY_NONE)
Q &qqqq

if(waitmax)
	opt waitmsg 1
	 if(hwndCtrl)
		 web "" waitmax<<16|1 hwndCtrl
	 else
		int i; double w1 w2(waitmax)
		for i 0 1000000000
			w1=i/1000.0; w2-w1; if(w2<=0) break
			wait w1
			 out "%s %i" _s.from(doc.readyState) wb&&wb.Busy
			sel(doc.readyState 1) case "complete" goto g1
		end "wait timeout"
else 0
 g1
err+ end _error
Q &qqqqq
outq

 tried to add arg for template file + body.innerText, but problems with encoding etc.
 more notes in app::HtmlWrite and in dlg_tools.
