function~ [html] ;;html: 0 text, 1 HTML.

 Gets text or HTML of container page.
 Obsolete. Use <help>Htm.DocProp</help>.


if(!el) end ERR_INIT

MSHTML.IHTMLDocument3 doc3=el.document
if(html) ret doc3.documentElement.outerHTML
else ret doc3.documentElement.innerText

err+ end _error
