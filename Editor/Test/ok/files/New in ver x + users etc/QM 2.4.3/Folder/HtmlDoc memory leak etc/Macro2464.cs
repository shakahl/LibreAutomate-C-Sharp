/exe 4
out
 int w=win("Internet Explorer" "IEFrame")
 Htm el=htm("HTML" "" "" w 0 0 32)
 _s=el.el.outerHTML
 _s.getfile("$temp$\html.htm"); out _s; ret
 rep 30
	sub.Thread _s
	 wait 0 H mac("sub.Thread" "" _s)
 mes 1



#sub Thread
function $HTML
MSHTML.IHTMLDocument2 d._create(uuidof(MSHTML.HTMLDocument))

opt waitmsg 1
IPersistStreamInit ps=+d
__Stream t.CreateOnFile("$temp$\html.htm" STGM_READ|STGM_SHARE_DENY_WRITE)
ps.InitNew
ps.Load(t)
t=0;ps=0

 wait for page loaded completely
opt waitmsg 1
rep
	wait 0.01
	out "%s" _s.from(d.readyState)
	 sel(doc.readyState 1) case "complete" goto g1
	_s=d.readyState
	sel(_s 1)
		case "loading" continue
		case else goto g1
		 case "complete" goto g1
		 case "interactive" int _inter; _inter+1; if(_inter>20) goto g1
		 case else _inter=0
 g1
 1
 out d.body.innerText
MSHTML.IHTMLElement body=d.body
out body.innerText

 BEGIN PROJECT
 main_function  Macro2459
 exe_file  $my qm$\Macro2459.qmm
 flags  6
 guid  {B4630D9D-E933-4A2F-8160-2A5FA15C2314}
 END PROJECT
