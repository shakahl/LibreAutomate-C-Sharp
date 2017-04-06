function Acc'a

 Gets FFNode from Acc, and initializes this variable.


if(!a.a) end ERR_BADARG

node=sub_sys.QueryService(a.a FFDOM.IID_ISimpleDOMNode uuidof("{0c539790-12e4-11cf-b661-00aa004cd6d8}"))

if !node
	if(a.__FirefoxNotInstalled(_s)) end F"{ERR_OBJECTGET}. {_s}"
	end ERR_OBJECTGET
