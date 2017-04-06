/exe 4
out
int w=win("Internet Explorer" "IEFrame")
Htm el=htm("HTML" "" "" w 0 0 32)
_s=el.el.outerHTML
 out _s; ret
rep 30
	sub.Thread _s
	 wait 0 H mac("sub.Thread2" "" _s)
 mes 1


#sub Thread
function $HTML

HtmlDoc x
 x.SetOptions(1)
x.InitFromText(HTML)
_s=x.GetText; out _s
 x.Delete
 mes 1
 x.Close


#sub Thread2
function $HTML

MSHTML.IHTMLDocument2 d
int useWB=0
if useWB
	int m_hax=CreateWindowEx(0 "ActiveX" F"SHDocVw.WebBrowser" WS_POPUP 0 0 0 0 HWND_MESSAGE 0 _hinst 0)
	SHDocVw.WebBrowser wb._getcontrol(m_hax)
	wb.Silent=1
	VARIANT f=14 ;;navNoHistory|navNoReadFromCache|navNoWriteToCache ;;MSDN says "not implemented", but works, even with IE5. Actually uses cache, but does not download if not necessary.
	wb.Navigate("about:blank" f)
	opt waitmsg 1; 0; rep() if(!wb.Busy) break; else 0.01
	d=wb.Document
	 wb._setevents("sub.wb")
else
	d._create(uuidof(MSHTML.HTMLDocument))
	 d._setevents("sub.d")

interface# ___IPersist :IUnknown a
interface# ___IPersistStreamInit :___IPersist b Load(IStream'pStm) c d InitNew() {7FD52380-4E07-101B-AE2D-08002B2EC713}
interface# ___IPersistFile :___IPersist b Load(@*pszFileName dwMode) {0000010b-0000-0000-C000-000000000046}

opt waitmsg 1
#if 1 ;;load from memory
___IPersistStreamInit ps=+d
__Stream t.CreateOnHglobal(HTML len(HTML)+1)
ps.InitNew
ps.Load(t)
t=0;ps=0
 3;ret

#else ;;load from file
str sf.expandpath("$temp$\html.htm")
_s=HTML; _s.setfile(sf)
___IPersistFile pf=+d
out 1
pf.Load(@sf STGM_READ|STGM_SHARE_DENY_WRITE)
out 2
pf=0
out 3
d=0 ;;waits forever
out 4
 out sf; sf.findreplace("\" "/"); sf-"file:///"; out sf; d.url=sf
 1;ret
#endif


 wait for page loaded completely
opt waitmsg 1
int waitmax=5
int i; double w1 w2(waitmax)
for i 0 1000000000
	0.01
	 w1=i/1000.0; w2-w1; if(w2<=0) break
	 wait w1
	 out "%s" _s.from(d.readyState)
	 sel(doc.readyState 1) case "complete" goto g1
	_s=d.readyState
	sel(_s 1)
		case "loading" continue
		case else goto g1
		 case "complete" goto g1
		 case "interactive" int _inter; _inter+1; if(_inter>20) goto g1
		 case else _inter=0
end ERR_TIMEOUT
 g1
0.1
 out d.body.innerText

 BEGIN PROJECT
 main_function  Macro2459
 exe_file  $my qm$\Macro2459.qmm
 flags  6
 guid  {B4630D9D-E933-4A2F-8160-2A5FA15C2314}
 END PROJECT


#sub d_onreadystatechange
function MSHTML.IHTMLDocument2'd
 _s= d.readyState
 out _s.ucase

#sub wb_DocumentComplete
function IDispatch'pDisp `&URL ;;SHDocVw.IWebBrowser2'wb
 out URL
