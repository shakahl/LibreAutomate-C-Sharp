Acc a=acc("Videos" "TEXT" win("Internet Explorer" "IEFrame") "" "" 0x3805 0x40 0x20000040)

Htm el
IServiceProvider sp=+a.a
IDispatch d
sp.QueryService(uuidof(MSHTML.IHTMLElement) IID_IDispatch &d)
el=+d

out el
