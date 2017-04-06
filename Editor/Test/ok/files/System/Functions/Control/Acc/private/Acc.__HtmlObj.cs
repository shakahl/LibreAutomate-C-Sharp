function# [Htm&e] [FFNode&f] ;;Returns: 0 none, 1 Htm, 2 FFNode

 Gets Htm or FFNode.


Htm _e; if(!&e) &e=_e
FFNode _f; if(!&f) &f=_f

e=sub_sys.QueryService(a uuidof(MSHTML.IHTMLElement))
if(e) ret 1
f=sub_sys.QueryService(a FFDOM.IID_ISimpleDOMNode uuidof("{0c539790-12e4-11cf-b661-00aa004cd6d8}"))
if(f) ret 2
