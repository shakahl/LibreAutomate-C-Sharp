Acc a=acc("Videos" "TEXT" win("Internet Explorer" "IEFrame") "" "" 0x3805 0x40 0x20000040)

Htm el
GUID* iid=uuidof(MSHTML.IHTMLElement)
IServiceProvider sp=+a.a
IUnknown u
sp.QueryService(iid iid &u)
IDispatch d=+u
el=+d

out el
