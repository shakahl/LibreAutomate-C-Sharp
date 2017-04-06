 When a function returns interface from array (ret a[i]), the returned interface is invalid, and throws error or exception.
 Also tested with HtmlDoc.FindHtmlElement.
 Note: if using AddRef to test, restart QM, or the bug behavior will disappear.

out

_monitor=1

Wsh.Drive d=f_iface
 out d.AddRef
 out d
out d.DriveLetter
outref d
