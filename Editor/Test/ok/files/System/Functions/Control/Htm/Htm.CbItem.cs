function# [str&text]

 Returns combo box selected element's 0-based index, and optionally gets its text.

 text - variable for text. Can be 0.


if(!el) end ERR_INIT

MSHTML.IHTMLSelectElement elsel=+el
int i=elsel.selectedIndex
if(&text)
	MSHTML.IHTMLElement elo=elsel.item(i)
	text=elo.innerText
 multi?
ret i

err+ end _error
