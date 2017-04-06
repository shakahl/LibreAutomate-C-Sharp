function Acc'a

 Gets Htm from Acc, and initializes this variable.

 Added in: QM 2.3.3.


if(!a.a) end ERR_BADARG

el=sub_sys.QueryService(a.a uuidof(MSHTML.IHTMLElement))

if(!el) end ERR_OBJECTGET
