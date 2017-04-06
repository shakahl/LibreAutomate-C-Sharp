 /Macro1225
function $s
 out s
int lcid=val(_s.from("0x" s))
 out lcid

BSTR b.alloc(300)
WINAPIV.LCIDToLocaleName(lcid b 300 0)
out _s.ansi(b)

ret 1
