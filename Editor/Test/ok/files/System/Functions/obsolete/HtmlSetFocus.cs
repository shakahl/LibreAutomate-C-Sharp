 /
function# $tag $name [$html] [`hwnd] [~frame] [index] [flags] [^waits] [navig] ;;flags: 1 name *, 2 name regexp, 4 html *, 8 html regexp, 16 window is IES, 32 error, 0x100-0xA00 name is attribute (id, name, alt, value, type, title, href, onclick, src, classid), 0x1000 tls9=hwndIES.

 Sets keyboard input focus to a textbox, button or other element in a web page.
 Obsolete.


MSHTML.IHTMLElement2 el2=htm(tag name html hwnd frame index flags waits navig)
if(!el2) ret
el2.focus
ret 1
err+ end _error
