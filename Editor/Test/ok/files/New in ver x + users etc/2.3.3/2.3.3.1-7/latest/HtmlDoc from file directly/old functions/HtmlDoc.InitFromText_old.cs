function $HTML

 Initializes the variable with HTML that is passed as text.
 Error if fails.

 See also: <str.GetClipboardHTML>


CreateDocument
Write(HTML)

err+ end _error

if m_flags&1 ;;MSHTML adds meta generator tag. Remove it.
	MSHTML.IHTMLElement e
	foreach e d3.getElementsByTagName("meta")
		_s=e.getAttribute("name" 2)
		if(_s~"GENERATOR") e.outerHTML=""; break
err+
