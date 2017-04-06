 /
function! MSHTML.IHTMLElement'el RECT&r [_]

Htm e=el
e.GetRect(r)
ret 1
err+
