 /dlg_html_editor
function# [str&s]

if(!&s) &s=_s
DHEDATA- t
MSHTML.IHTMLDocument3 doc3=+t.doc
s=doc3.documentElement.outerHTML
ret Crc32(s s.len)
