 /
function IDispatch'pDisp VARIANT&URL VARIANT&Flags VARIANT&TargetFrameName VARIANT&PostData VARIANT&Headers @&Cancel SHDocVw.IWebBrowser2'tw

str s1=URL
 out 1
if(s1="about:blank" or !s1.len) ret
 out 2

out s1

 get post form data. Note that the event is not fired on form submit in IE 5/6. Works only in some conditions, that are different on different IE versions.
if PostData.vt=VT_BYREF|VT_VARIANT
	VARIANT& v=PostData.pvarVal
	 outx v.vt
	if v.vt=VT_ARRAY|VT_UI1 and v.parray
		 out "%i" v.parray
		SAFEARRAY& a=v.parray
		 out "%i %i %i %i %i" a.rgsabound[0].cElements a.rgsabound[0].lLbound a.cDims a.cLocks a.cbElements
		lpstr s=a.pvData; out s


 get post form data from document. On link click.
if s1.end("#x")
	MSHTML.IHTMLDocument3 d=tw.Document
	MSHTML.IHTMLElement el=d.getElementById("cu")
	out el.getAttribute("value" 0)


 Cancel=-1
