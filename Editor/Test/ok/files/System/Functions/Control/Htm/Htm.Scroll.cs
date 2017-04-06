function [$scrollcmd] ;;scrollcmd: "down", "pageDown", "up", "pageUp", "right", "pageRight", "left", "pageLeft", "" (default) scroll into view.

 Scrolls web page that contains this element.

 scrollcmd - how to scroll. See above. Default: "" - scroll this element into view.


if(!el) end ERR_INIT

if !empty(scrollcmd)
	MSHTML.IHTMLElement2 el2=+el
	el2.doScroll(scrollcmd)
else el.scrollIntoView

err+ end _error
