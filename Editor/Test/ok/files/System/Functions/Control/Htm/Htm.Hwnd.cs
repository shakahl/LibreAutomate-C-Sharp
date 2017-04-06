function#

 Gets handle of container "Internet Explorer_Server" control.

 Added in: QM 2.3.6.


if(!el) end ERR_INIT
opt noerrorshere 1

MSHTML.IHTMLDocument2 doc=el.document
IOleWindow ow=+doc
ow.GetWindow(_i)
ret _i
