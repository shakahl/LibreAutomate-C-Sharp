 /
function hwndCtrl $HTML [waitmax] [MSHTML.IHTMLDocument2'doc] [$tempFolder]

 Loads HTML into web browser control or IHTMLDocument2 object.
 Error if fails.

 hwndCtrl - handle of control that hosts the web browser control. Normally it is like id(3 hDlg).
 HTML - string containing HTML.
 waitmax - max time (s) to wait until finished loading. Error on timeout. If 0, does not wait.
    Without waiting you may not be able to access the DOM immediately after calling this function, because the HTML is not finished loading.
    To wait later, can be used code like this: web "" waitmax<<16|1 hwndCtrl
 doc - if used, loads HTML into the document. Else gets document from the web browser control.
    The document can be not attached to a web browser control.
    hwndCtrl not used.
 tempFolder - folder where to create temporary file. Example: "$temp qm$".
    If used, will save HTML to a temporary file in the folder, and load the file.
    If not used, will load directly from HTML.
    Generally this function works better when using temporary file, although slower.
    When using temporary file, in the page can be used relative paths of local files (images, scripts, etc).
    The function creates unique filename for the file. It is like qm_htwbc_xxxx_.htm.
    The function deletes the file when the thread ends. Sometimes may fail to delete; then you can delete these files.

 REMARKS
 To be notified about link clicks, use BeforeNavigate2 event.
 To load HTML file into web browser control, instead use str.setwintext or WebBrowser.Navigate.

 Added in: QM 2.3.2.


interface# ___IPersist :IUnknown a
interface# ___IPersistStreamInit :___IPersist b Load(IStream'pStm) c d InitNew() {7FD52380-4E07-101B-AE2D-08002B2EC713}
interface# ___IPersistFile :___IPersist b Load(@*pszFileName dwMode) {0000010b-0000-0000-C000-000000000046}

if !doc
	SHDocVw.WebBrowser wb._getcontrol(hwndCtrl)
	doc=wb.Document
	if(!doc) _s.setwintext(hwndCtrl); doc=wb.Document

if(HTML=0) HTML=""

lpstr BOM="[0xef][0xbb][0xbf]"
int addBOM=_unicode and DetectStringEncoding(HTML)=1 and findrx(HTML "(?i)<meta\s+http-equiv\s*=\s*[''']content-type['''][^>]+\bcharset\b")<0 and StrCompareN(HTML BOM 3)

if empty(tempFolder)
	if(addBOM) HTML=_s.from(BOM HTML)
	___IPersistStreamInit ps=+doc
	__Stream t.CreateOnHglobal(HTML len(HTML)+1)
	ps.InitNew
	ps.Load(t)
else
	str sf.expandpath(F"{tempFolder}\qm_htwbc_{GetCurrentThreadId}_.htm")
	int retry
	 g2
	__HFile f.Create(sf CREATE_ALWAYS GENERIC_WRITE 0 FILE_ATTRIBUTE_TEMPORARY)
	err if(retry) end "failed to create temporary file"; else retry=1; sf.UniqueFileName; goto g2
	IStringMap-- t_m
	if(!t_m) t_m=CreateStringMap(3); atend sub.Atend &t_m
	t_m.Add(sf)
	
	if(addBOM) WriteFile(f BOM 3 &_i 0)
	if(!WriteFile(f HTML len(HTML) &_i 0)) end ERR_FAILED
	f.Close
	
	___IPersistFile pf=+doc
	pf.Load(@sf STGM_READ|STGM_SHARE_DENY_WRITE)

if waitmax
	opt waitmsg 1
	int i; double w1 w2(waitmax)
	for i 0 1000000
		w1=i/1000.0; w2-w1; if(w2<=0) break
		wait w1
		 out "%s %i" _s.from(doc.readyState) wb&&wb.Busy
		 sel(doc.readyState 1) case "loading"; case else goto g1
		sel doc.readyState 1
			case "complete" goto g1
			case "interactive" int _inter; _inter+1; if(_inter>100) goto g1
			case else _inter=0
	end ERR_TIMEOUT
else 0
 g1
err+ end _error


#sub Atend
function IStringMap*m
out __FUNCTION__

m.EnumBegin
rep
	if(!m.EnumNext(_s)) break
	DeleteFileW(@_s)


 Various ways of loading HTML: http://qualapps.blogspot.com/2008/10/how-to-load-mshtml-with-data.html
 Most of them cannot be used because need UTF-16 string, but we don't know what charset to use to convert.
 IPersistStreamInit has a bug in IE 5 and older IE 6. http://support.microsoft.com/?id=323569
 IPersistMoniker works only with IE 5. In my tests, Load failed with IE 6 and 8.
 More notes in app::HtmlWrite and in dlg_tools.
