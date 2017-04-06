function~

 Gets URL of container page.
 Obsolete. Use <help>Htm.DocProp</help>.


if(!el) end ERR_INIT

MSHTML.IHTMLDocument2 doc=el.document
ret doc.url

err+ end _error
