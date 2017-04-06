 deb 300
Htm el=htm("SELECT" "num" "" " Internet Explorer" 0 0 0x221)
 Htm el=htm("INPUT" "as_q" "" " Internet Explorer" 0 0 0x221)
if(!el) ret
 out el.HTML

 el.Mouse(1)

 out el.CbIndex(_s)
 out _s

 el.CbSelect(4)
 0.5
 el.CbSelect("50 results")




 ShowText "" el.DocText
 ShowText "" el.DocText(1)

 out el.DocURL
