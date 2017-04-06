 /Dialog88
function hwndCtrl $HTML [waitmax] [$templateFile] [MSHTML.IHTMLDocument2'doc]

 Loads HTML into web browser control.
 Error if fails.
 This function loads HTML from string. To load HTML file, use setwintext instead.

 hwndCtrl - handle of control that hosts web browser control. Normally it is like id(3 hDlg).
 HTML - string containing HTML.
 waitmax - max time (s) to wait until finished loading. Error on timeout. If 0, does not wait.
    Without waiting you may not be able to access the DOM immediately after calling this function, because the HTML is not finished loading.
 templateFile - template file. If used, gets all except body from the file. Then HTML must contain only body. Must be used hwndCtrl and not doc.
    Using template file is recommended.
    It allows you to use relative links.
    It prevents setting Internet security zone. The zone has some restrictions. Just a few: links to local files don't work; javascript and other things may not work if user turned off it in IE Options; form submit may not work; everything behaves differently with different IE versions.
 doc - if used, loads HTML into the document. Else gets document from the web browser control.
    The document can be not attached to a web browser control.
    hwndCtrl can be 0.
    does not wait for frames and iframes.


interface# __IPersist :IUnknown f0
interface# __IPersistStreamInit :__IPersist f1 Load(IStream'pStm) f2 f3 InitNew() {7FD52380-4E07-101B-AE2D-08002B2EC713}

int tf=!empty(templateFile)
if(tf)
	_s=templateFile
	_s.setwintext(hwndCtrl)
	doc=0
	  now wait?

if(!doc)
	SHDocVw.WebBrowser wb._getcontrol(hwndCtrl)
	doc=wb.Document
	if(!doc) _s.setwintext(hwndCtrl); doc=wb.Document

if(_unicode) int enc=DetectStringEncoding(HTML)

if(tf)
	doc.body.innerHTML=HTML
	out doc.domain
else
	if(enc=1 and findrx(HTML "(?i)<meta\s+http-equiv\s*=\s*[''']content-type['''][^>]+\bcharset\b")<0)
		 out "detected utf8"
		HTML=_s.from("[0xef][0xbb][0xbf]" HTML)
	
	__IPersistStreamInit ps=+doc
	__Stream t.CreateOnHglobal(HTML len(HTML)+1)
	ps.InitNew
	ps.Load(t)

if(waitmax)
	opt waitmsg 1
	 if(hwndCtrl)
		 web "" waitmax<<16|1 hwndCtrl
	 else
		int i; double w1 w2(waitmax)
		for i 0 1000000000
			w1=i/1000.0; w2-w1; if(w2<=0) break
			wait w1
			 _s=doc.readyState
			 out _s
			 out "%s %i" _s wb.Busy
			sel(doc.readyState 1) case "complete" ret
		end "wait timeout"
else 0

err+ end _error


 notes in app::WriteHtml and in dlg_tools
