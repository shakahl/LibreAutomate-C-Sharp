int w=win("Internet Explorer" "IEFrame")
Htm el=htm("HTML" "" "" w 0 0 32)
_s=el.el.outerHTML
_s.setfile("$temp$\html.htm")
out
out _s
