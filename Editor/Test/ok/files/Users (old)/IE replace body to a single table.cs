int w1=win("Quick Macros :: View topic")
MSHTML.IHTMLElement table=htm("TABLE" "" "" w1 0 5 0x20)
 table.outerHTML=""
MSHTML.IHTMLElement body=htm("BODY" "" "" w1 0 0 0x20)
body.innerHTML=table.outerHTML
 ...
 MSHTML.IHTMLDocument3 doc3=body.document
 str allhtml=doc3.documentElement.outerHTML
 ShowText "" allhtml
 allhtml.setfile("...")

