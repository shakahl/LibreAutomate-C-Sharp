 /Dialog60
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
interface# ___IPersistStreamInit :___IPersist b Load(IStream'pStm) c d InitNew() {7FD52380-4E07-101B-AE2D-08002B2EC713}

if(!doc)
	SHDocVw.WebBrowser wb._getcontrol(hwndCtrl)
	doc=wb.Document
	if(!doc) _s.setwintext(hwndCtrl); doc=wb.Document

if(HTML=0) HTML=""
if(_unicode and DetectStringEncoding(HTML)=1 and findrx(HTML "(?i)<meta\s+http-equiv\s*=\s*[''']content-type['''][^>]+\bcharset\b")<0)
	lpstr BOM="[0xef][0xbb][0xbf]"
	if(StrCompareN(HTML BOM 3)) HTML=_s.from(BOM HTML)

dll "qm.exe" #__LoadHTML IUnknown'doc $html [$baseUrl]
int hr=__LoadHTML(doc HTML)
out hr
out _s.dllerror("" "" hr)
if(hr) ret

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


 tried to add arg for template file + body.innerText, but problems with encoding etc.
 more notes in app::WriteHtml and in dlg_tools.
